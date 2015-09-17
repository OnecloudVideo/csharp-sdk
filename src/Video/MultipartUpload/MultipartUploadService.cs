using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.Cryptography;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Pispower.Video.RestfulApi.Internal;

namespace Pispower.Video.RestfulApi.Video.MultipartUpload
{
    class MultipartUploadService
    {
        private static int BUFFER_SIZE = 4 * 1024 * 1024;

        private readonly Client client;

        public MultipartUploadService(string appKey, string appScrect)
        {
            client = new Client(appKey, appScrect);
        }
        
        /// <summary>
        /// 断点续传，查询已经上传的分片，与待上传文件分片MD5比较，记录未上传的分片，上传未上传的分片，最后合并分片。
        /// </summary>
        /// <param name="fileUploadRequest"></param>
        /// <returns>
        /// Video 包含id,name,size,type,status,duration,catalogId,catalogName,VideoEmbedCode列表 。
        ///</returns>
        public Video Upload(FileUploadRequest fileUploadRequest)
        {
            var file = fileUploadRequest.FileInfo;
            var partSize = fileUploadRequest.PartSize;

            //对文件分片
            var files = SplitFile(file, partSize);
            //本地文件分片列表
            var localParts = GetLocalParts(files);

            //文件MD5
            var fileMD5 = MD5Utility.Compute(file).ToLower();
            var fileListRequest = new ServerFileListRequest();
            fileListRequest.FileMD5Equal = fileMD5;
            fileListRequest.FileNameLike = file.Name;
          
            //获取服务器上已初始化过的文件
            var serverFileInfo = GetServerFileInfo(fileListRequest);
            //获得服务器上的分片
            var serverParts = GetServerPartInfos(serverFileInfo);
            //区分上传完成的分片，未上传的分片，错误的分片
            var multipartFileparts = getMultipartFileParts(localParts, serverParts);
            //删除服务器上出错的分片
            DeleteParts(multipartFileparts.ErrorParts);
            // 已经上传完成的分片记录到一个列表中备用
            var finishedParts = multipartFileparts.FinishedParts;
            //获取uploadId
            var uploadId = GetUploadId(file.Name, fileMD5, serverFileInfo);
            // 上传未完成的分片
            var uploadedParts = UploadParts(uploadId, multipartFileparts.UnfinishedParts);

            // 将新上传的数据和已经在服务器的数据进行合并。
            finishedParts.AddRange(uploadedParts);

            var CatalogId = "";
            if (fileUploadRequest.CatalogId != 0)
            {
                CatalogId = fileUploadRequest.CatalogId.ToString();
            }
            
            var video = CompleteParts(uploadId, finishedParts, CatalogId);
           
            //清除临时文件
            Delete(files[0].FullName);
            
            return video;
        }
        
        /// <summary>
        /// 获得本地文件分片列表
        /// </summary>
        /// <param name="files">本地文件分片数组</param>
        /// <returns></returns>
        private List<FilePart> GetLocalParts(System.IO.FileInfo[] files)
        {
            var localFileParts = new List<FilePart>();

            foreach (var file in files)
            {
                var filePart = new FilePart();
                var name = Path.GetFileName(file.FullName).Split(new Char[] { '.' });

                filePart.FilePartInfo = file;
                filePart.PartNum = System.Int32.Parse(name[1]);
                using (var fileStream = new FileStream(file.FullName, FileMode.Open, FileAccess.Read))
                {
                    filePart.PartMD5 = MD5Utility.Compute(file).ToLower();
                }
                localFileParts.Add(filePart);
            }
            return localFileParts;
        }
        
        /// <summary>
        /// 获得服务器上已初始化过的文件分片列表
        /// </summary>
        /// <param name="serverFileInfo"></param>
        private List<FilePart> GetServerPartInfos(FileInfo serverFileInfo)
        {
            if (serverFileInfo == null)
            {
                return new List<FilePart>();
            }
            else
            {
                var serverParts = GetParts(serverFileInfo.UploadId);
                return serverParts;
            }
        }
        
        /// <summary>
        /// 获得已初始化的文件信息。
        /// </summary>
        /// <param name="request"></param>
        private FileInfo GetServerFileInfo(ServerFileListRequest request)
        {
            var serverFileList = List(request);
            if (serverFileList.Count == 1)
            {
                return serverFileList[0];
            }
            else if (serverFileList.Count == 0)
            {
                return null;
            }
            else
            {
                throw new PispowerAPIException("count error:There are " + serverFileList.Count + " in server.");
            }
        }
        
        /// <summary>
        /// 上传未上传的文件分片
        /// </summary>
        /// <param name="uploadId">当前上传事件的唯一标识</param>
        /// <param name="unfinishedParts">未上传的分片列表</param>
        private List<FilePart> UploadParts(String uploadId, List<FilePart> unfinishedParts)
        {
            var finishedParts = new List<FilePart>();
            foreach (var filePart in unfinishedParts)
            {
                var part = UploadPart(uploadId, filePart.PartNum, filePart.FilePartInfo);
                //返回的分片MD5是否与本地对应的相等
                if (part.PartMD5.CompareTo(filePart.PartMD5) == 0)
                {
                    finishedParts.Add(part);
                }
                else
                {
                    throw new PispowerAPIException("partMD5 error");
                }
            }
            return finishedParts;
        }
        
        /// <summary>
        /// 获得上传事件的uploadId
        /// </summary>
        /// <param name="fileName">文件名字</param>
        /// <param name="fileMD5">文件MD5</param>
        /// <param name="serverFileInfo">服务器上已初始化的文件信息</param>
        private String GetUploadId(String fileName, String fileMD5, FileInfo serverFileInfo)
        {
            var uploadId = "";
            if (serverFileInfo == null)
            {
                uploadId = Init(fileName, fileMD5);
            }
            else
            {
                uploadId = serverFileInfo.UploadId;
            }
            return uploadId;
        }
        
        /// <summary>
        ///  将本服务器上分片MD5与地文件分片比较，若服务器上的一个分片在本地分片中，
	    ///  则加入finishedParts中，并从localParts中对应删除，否则加入errorParts中。
        /// </summary>
        /// <param name="localParts">本地待文件分片列表</param>
        /// <param name="serverParts">服务器上的文件分片列表</param>
        private MultipartFilePart getMultipartFileParts(List<FilePart> localParts, List<FilePart> serverParts)
        {
            var multipartFilePart = new MultipartFilePart();
            var errorParts = new List<FilePart>();
            var finishedParts = new List<FilePart>();
            foreach (var serverPart in serverParts)
            {
                var index = inLocalParts(serverPart, localParts);
                if (index == -1)
                {
                    errorParts.Add(serverPart);
                }
                else
                {
                    finishedParts.Add(serverPart);
                    localParts.RemoveAt(index);
                }
            }
            multipartFilePart.ErrorParts = errorParts;
            multipartFilePart.FinishedParts = finishedParts;
            multipartFilePart.UnfinishedParts = localParts;

            return multipartFilePart;
        }
        
        /// <summary>
        /// 服务器上的某个分片是否在本地文件分片列表中
        /// </summary>
        /// <param name="serverPart">服务器上的某一分片</param>
        /// <param name="localParts">本地待上传文件分片列表</param>
        private int inLocalParts(FilePart serverPart, List<FilePart> localParts)
        {
            foreach (var localPart in localParts)
            {
                if (serverPart.PartMD5.CompareTo(localPart.PartMD5) == 0 && serverPart.PartNum.CompareTo(localPart.PartNum) == 0)
                {
                    return localParts.IndexOf(localPart);
                }
            }
            return -1;
        }
        
        /// <summary>
        /// 对待上传文件进行初始化
        /// </summary>
        /// <param name="fileName">文件的名字</param>
        /// <param name="fileMD5">文件的MD5</param>
        private String Init(String fileName, String fileMD5)
        {
            QueryString qs = new QueryString();
            qs.Add("fileName", fileName);
            qs.Add("fileMD5", fileMD5);

            var initUpload = client.HttpPost("/video/multipartUpload/init.api", qs);
            var jo = JObject.Parse(initUpload);
            if (jo["statusCode"].ToString().CompareTo("0") == 0)
            {
                return jo["uploadId"].ToString();
            }
            else
            {
                throw new PispowerAPIException(System.Int32.Parse(jo["statusCode"].ToString()), jo["message"].ToString());
            }
        }
        
        /// <summary>
        /// 列出服务器上已初始化的文件信息
        /// </summary>
        /// <param name="request"></param>
        /// <returns>服务器上已初始化的文件信息</returns>
        public List<FileInfo> List(ServerFileListRequest request)
        {
            var qs = new QueryString();
            if (null != request.FileNameLike && !"".Equals(request.FileNameLike))
            {
                qs.Add("fileNameLike", request.FileNameLike);
            }
            if (null != request.FileMD5Equal && !"".Equals(request.FileMD5Equal))
            {
                qs.Add("fileMD5Equal", request.FileMD5Equal);
            }
            var list = client.HttpGet("/video/multipartUpload/list.api", qs);

            var jo = JObject.Parse(list);
            if (jo["statusCode"].ToString().CompareTo("0") == 0)
            {
                var serverFileList = new List<FileInfo>();
                var multipartUploads = jo["multipartUploads"];

                foreach (var jObj in multipartUploads)
                {
                    var fileInfo = new FileInfo();
                    fileInfo.FileMD5 = jObj["fileMD5"].ToString();
                    fileInfo.FileName = jObj["fileName"].ToString();
                    fileInfo.UploadId = jObj["uploadId"].ToString();
                    serverFileList.Add(fileInfo);
                }
                return serverFileList;
            }
            else
            {
                throw new PispowerAPIException(System.Int32.Parse(jo["statusCode"].ToString()), jo["message"].ToString());
            }
        }
        
        /// <summary>
        /// 获取服务器上文件分片的信息
        /// </summary>
        /// <param name="uploadId">当前上传事件的唯一标识</param>
        /// <returns>服务器上的分片信息列表</returns>
        public List<FilePart> GetParts(String uploadId)
        {
            var qs = new QueryString();
            qs.Add("uploadId", uploadId);

            var getParts = client.HttpGet("/video/multipartUpload/getParts.api", qs);
            var jo = JObject.Parse(getParts);
           
            if (jo["statusCode"].ToString().CompareTo("0") == 0)
            {
                var serverParts = new List<FilePart>();
                foreach (var jObj in jo["uploadedParts"])
                {
                    var filePart = new FilePart();
                    filePart.PartKey = jObj["partKey"].ToString();
                    filePart.PartMD5 = jObj["partMD5"].ToString();
                    filePart.PartNum = System.Int32.Parse(jObj["partNumber"].ToString());
                    serverParts.Add(filePart);
                }
                return serverParts;
            }
            else
            {
                throw new PispowerAPIException(System.Int32.Parse(jo["statusCode"].ToString()), jo["message"].ToString());
            }
        }

        /// <summary>
        /// 中止一个上传进程 
        /// </summary>
        /// <param name="uploadId">当前上传事件的唯一标识</param>
        /// <returns>上传文件的名字</returns>
        public String Abort(String uploadId)
        {
            var qs = new QueryString();
            qs.Add("uploadId", uploadId);

            var abortParts = client.HttpPost("/video/getParts.api", qs);
            var jo = JObject.Parse(abortParts);
            if (jo["statusCode"].ToString().CompareTo("0") == 0)
            {
                return jo["fileName"].ToString();
            }
            else
            {
                throw new PispowerAPIException(System.Int32.Parse(jo["statusCode"].ToString()), jo["message"].ToString());
            }
        }
      
        /// <summary>
        /// 删除服务器上错误的分片
        /// </summary>
        /// <param name="errorParts">服务器上错误的分片列表</param>
        private void DeleteParts(List<FilePart> errorParts)
        {
            if (errorParts.Count == 0)
            {
                return;
            }
            var qs = new QueryString();

            foreach (var filePart in errorParts)
            {
                qs.Add("partKeys", filePart.PartKey);
            }

            var deleteParts = client.HttpPost("/video/multipartUpload/deleteParts.api", qs);
            var jo = JObject.Parse(deleteParts);
          
            if (jo["statusCode"].ToString().CompareTo("0") != 0)
            {
                throw new PispowerAPIException(System.Int32.Parse(jo["statusCode"].ToString()), jo["message"].ToString());
            }
        }
        
        /// <summary>
        /// 上传一个分片
        /// </summary>
        /// <param name="uploadId">当前上传事件的唯一标识</param>
        /// <param name="partNumber">分片num编号</param>
        /// <param name="part">分片文件</param>
        private FilePart UploadPart(String uploadId, int partNumber, System.IO.FileInfo part)
        {
            QueryString qs = new QueryString();
            qs.Add("uploadId", uploadId);
            qs.Add("partNumber", partNumber.ToString());

            using (var fs = File.OpenRead(part.FullName))
            {
                var uploadPart = client.HttpUpload("/video/multipartUpload/uploadPart.api", qs, fs);
                var jo = JObject.Parse(uploadPart);

                if (jo["statusCode"].ToString().CompareTo("0") == 0)
                {
                    var filePart = new FilePart();
                    filePart.PartNum = partNumber;
                    filePart.PartMD5 = jo["partMD5"].ToString();
                    filePart.PartKey = jo["partKey"].ToString();
                 
                    return filePart;
                }
                else
                {
                    throw new PispowerAPIException(System.Int32.Parse(jo["statusCode"].ToString()), jo["message"].ToString());
                }
            }
        }
        
        /// <summary>
        /// 合并分片
        /// </summary>
        /// <param name="uploadId">当前上传事件的唯一标识</param>
        /// <param name="fileParts"></param> 
        /// <param name="catalogId">上传到该catalogId的Catalog中</param>
        private Video CompleteParts(String uploadId, List<FilePart> fileParts, String catalogId)
        {
            var qs = new QueryString();
            qs.Add("uploadId", uploadId);
            qs.Add("catalogId", catalogId);
            foreach (var filePart in fileParts)
            {
                var partNum = filePart.PartNum;
                var partKey = filePart.PartKey;
                qs.Add("part" + partNum, partKey);
            }
            var completeParts = client.HttpPost("/video/multipartUpload/complete.api", qs);
            var jo = JObject.Parse(completeParts);
            if (jo["statusCode"].ToString().CompareTo("0") == 0)
            {
                var video = new Video();
                video.CatalogId = System.Int32.Parse(jo["catalogId"].ToString());
                video.CatalogName = jo["catalogName"].ToString();
                video.Name = jo["name"].ToString();
                video.Size = long.Parse(jo["size"].ToString());
                video.Status = jo["status"].ToString();
                video.Type = jo["type"].ToString();
                video.Duration = long.Parse(jo["duration"].ToString());

                var embedCodes = new List<VideoEmbedCode>();
                foreach (var jObj in jo["embedCodes"])
                {
                    var embedCode = new VideoEmbedCode();
                    embedCode.AutoAdaptionCode = jObj["autoAdaptionCode"].ToString();
                    embedCode.Clarity = jObj["clarity"].ToString();
                    embedCode.FilePath = jObj["filePath"].ToString();
                    embedCode.FlashCode = jObj["flashCode"].ToString();
                    embedCode.Html5Code = jObj["html5Code"].ToString();
                    embedCode.PortableCode = jObj["portableCode"].ToString();
                    embedCode.Resolution = jObj["resolution"].ToString();

                    embedCodes.Add(embedCode);
                }
                video.EmbedCodes = embedCodes;
                return video;
            }
            else
            {
                throw new PispowerAPIException(System.Int32.Parse(jo["statusCode"].ToString()), jo["message"].ToString());
            }
        }
        
        /// <summary>
        /// 对文件进行分片
        /// </summary>
        /// <param name="fileInfo">待上传文件</param>
        /// <param name="partSize">分片的大小</param>
        private System.IO.FileInfo[] SplitFile(System.IO.FileInfo fileInfo, int partSize)
        {
            var tmpDir = Path.GetTempPath();
            var time = Convert.ToInt64((DateTime.UtcNow - DateTime.Parse("1970-1-1")).TotalMilliseconds);
            var filePartPath = @tmpDir + "onecloud-multipart-upload\\" + fileInfo.Name + time;

            if (!Directory.Exists(filePartPath))
            {
                Directory.CreateDirectory(filePartPath);
            }
            var fileIn = new FileStream(@fileInfo.FullName, FileMode.Open, FileAccess.Read);
            var partCount = fileInfo.Length / partSize;

            if (fileInfo.Length % partSize > 0)
            {
                partCount++;
            }
            long beginIndex = 0, endIndex = 0;

            var buffer = new byte[BUFFER_SIZE];

            for (int i = 0; i < partCount; i++)
            {
                var partNum = i + 1;
                var fileOut = new FileStream(@filePartPath + "\\video." + partNum + ".part", FileMode.Create);
                endIndex = beginIndex + partSize;
                if (endIndex > fileInfo.Length)
                {
                    endIndex = fileInfo.Length;
                }

                while (beginIndex < endIndex)
                {
                    byte[] bff = buffer;

                    if (endIndex - beginIndex < BUFFER_SIZE)
                    {
                        bff = new byte[(int)(endIndex - beginIndex)];
                    }
                    var readCount = fileIn.Read(bff, 0, (int)(endIndex - beginIndex));
                    beginIndex += readCount;

                    var bw = new BinaryWriter(fileOut);
                    bw.Write(bff);
                    bw.Close();
                }
                fileOut.Close();
            }
            fileIn.Close();

            DirectoryInfo directory = new DirectoryInfo(filePartPath);
            var files = directory.GetFiles();

            return files;
        }

        /// <summary>
        //  清除临时文件分片及其所在的目录
        /// </summary>
        /// <param name="request"></param>
        private void Delete(String name)
        {
            var dirName = Path.GetDirectoryName(name);

            Directory.Delete(dirName, true);
        }
    }
}
