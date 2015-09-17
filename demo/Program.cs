using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.IO;


using Pispower.Video.RestfulApi.Catalog;
using Pispower.Video.RestfulApi.Internal;
using Pispower.Video.RestfulApi.Video;
using Pispower.Video.RestfulApi.Video.MultipartUpload;

namespace Pispower.Video.RestfulApi.demo
{
    public class Program
    {
        ///<summary>
	    /// 视频平台提供的 accessKey，该字段会通过公网传输, 
        /// 
	    /// 请从开发者支持/Restful API页面获取
	    ///</summary>
        private static String accessKey = "";

        ///<summary>
	    /// 视频平台提供的 accessSecret，该字段用来生成数字签名，
	    /// 请注意保密， 并且不要通过公网传输。
	    ///    
	    /// 请从 开发者支持/Restful API页面获取
	    ///</summary>
        private static String accessSecret = "";

        private static CatalogService catalogService = new CatalogService(accessKey, accessSecret);

        private static VideoService videoService = new VideoService(accessKey, accessSecret);

        private static MultipartUploadService multipartService = new MultipartUploadService(accessKey, accessSecret);
       
        static void Main(string[] args)
        {
            //新建Catalog
          
           var createCatalog = catalogService.Add("catalog");
           Console.WriteLine("create catalog...");
           Console.WriteLine("name:" + createCatalog.Name + " id:" + createCatalog.Id);
         
           //根据Id查询Catalog
    
           var getCatalog = catalogService.Get(3);
           Console.WriteLine("get catalog...");
           Console.WriteLine("name:" + getCatalog.Name + " videoNumber:" + getCatalog.VideoNumber);
          

            //列出Catalog

            CatalogListRequest catalogRequest = new CatalogListRequest();
            var listCatalog = catalogService.List(catalogRequest);
            Console.WriteLine("list catalog...");
            foreach (var cata in listCatalog)
            {
                Console.WriteLine("id:" + cata.Id + " name:" + cata.Name);
            }
            
             
            //删除一个Catalog
            /**
            catalogService.Delete(41);
            Console.WriteLine("delete catalog...");
            **/

            //根据Id获取一个视频
            
            /**
            var getVideo = videoService.Get("322");
            Console.WriteLine("get video...");
            Console.WriteLine("name:" + getVideo.Name + " status:" + getVideo.Status + " size:" 
			        + getVideo.Size + " type:" + getVideo.Type + " duration:" + getVideo.Duration 
			        + " description:" + getVideo.Description + " catalogId:" + getVideo.CatalogId 
                    + " catalogName:" + getVideo.CatalogName);

		    foreach (var embedCode in getVideo.EmbedCodes)
		    {
			    Console.WriteLine("autoAdaptionCode:" + embedCode.AutoAdaptionCode + " Clarity:"
					    + embedCode.Clarity + " FlashCode:" + embedCode.FlashCode + " Html5Code:"
					    + embedCode.Html5Code + " Resolution:" + embedCode.Resolution + " filePath:" 
					    + embedCode.FilePath  + " portableCode:" + embedCode.PortableCode);
		    }
           **/
          
            //上传一个视频
            /**
            var fileInfo = new System.IO.FileInfo(@"d:\wangjj\Prince.flv");
            var uploadRequest = new VideoUploadRequest();
     
            uploadRequest.Name = "name";
            uploadRequest.Description = "desc";
            uploadRequest.FileInfo = fileInfo;
            var uploadVideo = videoService.Upload(uploadRequest);
            Console.WriteLine("upload video...");
            Console.WriteLine("name:" + uploadVideo.Name + " status:" + uploadVideo.Status + " size:"
                       + uploadVideo.Size + " type:" + uploadVideo.Type + " duration:" + uploadVideo.Duration
                       + " catalogId:" + uploadVideo.CatalogId + " catalogName:" + uploadVideo.CatalogName);
             foreach (var embedCode in uploadVideo.EmbedCodes)
             {
                 Console.WriteLine("autoAdaptionCode:" + embedCode.AutoAdaptionCode + " Clarity:"
                         + embedCode.Clarity + " FlashCode:" + embedCode.FlashCode + " Html5Code:"
                         + embedCode.Html5Code + " Resolution:" + embedCode.Resolution + " filePath:"
                         + embedCode.FilePath + " portableCode:" + embedCode.PortableCode);
             }
             **/

             //列出视频
             /**
             var listRequest = new VideoListRequest();
       
             var listVideo = videoService.List(listRequest);
             Console.WriteLine("list video...");
             foreach (var video in listVideo)
             {
                 Console.WriteLine("name:" + video.Name + " id:" +  video.Id + " status:" + video.Status + " size:"
                            + video.Size + " type:" + video.Type + " duration:" + video.Duration
                            + " catalogId:" + video.CatalogId + " catalogName:" + video.CatalogName);
                 
             }
             **/

             //更新视频信息
             /**
             var updateRequest = new VideoUpdateRequest();
             updateRequest.Name = "testName";
             updateRequest.VideoId = 341;
             updateRequest.Description = "testDescription";
             videoService.Update(updateRequest);
             Console.WriteLine("update video...");
             **/

             //删除一个视频
             /**
             videoService.Delete(236);
             Console.WriteLine("delete video...");
             **/

             //断点续传
             /**
             var file = new System.IO.FileInfo(@"d:\wangjj\Prince.flv");
             int partSize = 2 * 1024 * 1024; // 4M
       
             var fileUploadRequest = new FileUploadRequest();
             fileUploadRequest.PartSize = partSize;
             fileUploadRequest.FileInfo = file;
             var video = multipartService.Upload(fileUploadRequest);        

             Console.WriteLine("multipart upload...");
             Console.WriteLine("name:" + video.Name + " status:" + video.Status + " size:"
                       + video.Size + " type:" + video.Type + " duration:" + video.Duration
                       + " catalogId:" + video.CatalogId + " catalogName:" + video.CatalogName);
             foreach (var embedCode in video.EmbedCodes)
             {
                 Console.WriteLine("autoAdaptionCode:" + embedCode.AutoAdaptionCode + " Clarity:"
                        + embedCode.Clarity + " FlashCode:" + embedCode.FlashCode + " Html5Code:"
                        + embedCode.Html5Code + " Resolution:" + embedCode.Resolution + " filePath:"
                        + embedCode.FilePath + " portableCode:" + embedCode.PortableCode);
             }
            
             **/        
        }

    }
}
