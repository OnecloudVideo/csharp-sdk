using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Pispower.Video.RestfulApi.Internal;

namespace Pispower.Video.RestfulApi.Video
{
    class VideoService
    {
        private readonly Client client;

        public VideoService(string appKey, string appScrect)
        {
            client = new Client(appKey, appScrect);
        }

        /// <summary>
        /// 列出视频
        /// </summary>
        /// <param name="request"></param>
        /// <returns>
        /// Video列表 每个Video对象包含id,name,size,type,status,duration,catalogId,catalogName。
        ///</returns>
        public  List<Video> List(VideoListRequest request)
        {
            var qs = new QueryString();
            if (null != request.NameLike && !"".Equals(request.NameLike))
            {
                qs.Add("nameLike", request.NameLike);
            }
            if (null != request.CatalogNameLike && !"".Equals(request.CatalogNameLike))
            {
                qs.Add("catalogNameLike", request.CatalogNameLike);
            }
            if (request.CatalogId > 0)
            {
                qs.Add("catalogId", request.CatalogId.ToString());
            }
            if (request.Page > 0)
            {
                qs.Add("page", request.Page.ToString());
            }
            if (request.MaxResult > 0)
            {
                qs.Add("maxResult", request.MaxResult.ToString());
            }
            var listVideo = client.HttpGet("/video/list.api", qs);
            var jo = JObject.Parse(listVideo);

            if (jo["statusCode"].ToString().CompareTo("0") == 0)
            {
                var videos = jo["videos"];
    
                var list = new List<Video>();
                foreach(var jObject in videos)
                {
                    var video = new Video();
                    video.Id = System.Int32.Parse(jObject["id"].ToString());
                    video.CatalogId = System.Int32.Parse(jObject["catalogId"].ToString());
                    video.CatalogName = jObject["catalogName"].ToString();
                    video.Name = jObject["name"].ToString();
                    video.Size = long.Parse(jObject["size"].ToString());
                    video.Status = jObject["status"].ToString();
                    video.Type = jObject["type"].ToString();
                    video.Duration = long.Parse(jObject["duration"].ToString());

                    list.Add(video);
                }
                return list;
            }
            else
            {
                throw new PispowerAPIException(System.Int32.Parse(jo["statusCode"].ToString()), jo["message"].ToString());
            }                          

        }

        /// <summary>
        /// 根据id查询视频
        /// </summary>
        /// <param name="videoId">Video的唯一标识</param>
        /// <returns>
        /// Video 包含id,name,size,type,status,duration,catalogId,catalogName,VideoEmbedCode列表。
        /// </returns>
        public  Video Get(Int32 videoId)
        {
            var qs = new QueryString();
            qs.Add("videoId", videoId.ToString());
            var getVideo = client.HttpGet("/video/get.api", qs);
            var jo = JObject.Parse(getVideo);

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
        /// 删除一个视频
        /// </summary>
        /// <param name="request">Video的唯一标识</param>
        public  void Delete(Int32 videoId)
        {
            var qs = new QueryString();
            qs.Add("videoId", videoId.ToString());

            var deleteVideo = client.HttpPost("/video/delete.api", qs);
            var jo = JObject.Parse(deleteVideo);

            if (jo["statusCode"].ToString().CompareTo("0") != 0)
            {
                throw new PispowerAPIException(System.Int32.Parse(jo["statusCode"].ToString()), jo["message"].ToString());
            }                          
        }

        /// <summary>
        /// 更新视频的名字或描述信息
        /// </summary>
        /// <param name="request"></param>
        public void Update(VideoUpdateRequest request)
        {
            var qs = new QueryString();
            qs.Add("videoId", request.VideoId.ToString());
            qs.Add("name", request.Name);
            if (null != request.Description && !"".Equals(request.Description))
            {
                qs.Add("description", request.Description);
            }
            var updateVideo = client.HttpPost("/video/update.api", qs);
            var jo = JObject.Parse(updateVideo);

            if (jo["statusCode"].ToString().CompareTo("0") != 0)
            {
                throw new PispowerAPIException(System.Int32.Parse(jo["statusCode"].ToString()), jo["message"].ToString());
            }                
        }

        /// <summary>
        /// 上传一个视频
        /// </summary>
        /// <param name="request"></param>
        public Video Upload(VideoUploadRequest request)
        {
            var qs = new QueryString();
            qs.Add("name", request.Name);
            if (null != request.Description && !"".Equals(request.Description))
            {
                qs.Add("description", request.Description);
            }
            if (request.CatalogId > 0)
            {
                qs.Add("catalogId", request.CatalogId.ToString());
            }
            using (var fs = File.OpenRead(request.FileInfo.FullName))
            {
                var uploadVideo = client.HttpUpload("/video/upload.api", qs, fs);
                var jo = JObject.Parse(uploadVideo);

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
        }
    }
}
