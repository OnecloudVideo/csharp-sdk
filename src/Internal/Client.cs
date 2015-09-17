using System;
using System.Net;
using System.Text;
using System.IO;

namespace Pispower.Video.RestfulApi.Internal
{

    public class Client
    {
        private const string Host = "https://video.cloudak47.com";

        private readonly string _appKey;

        private readonly string _appSecret;

        public Client(string appKey, string appSecret)
        {
            _appKey = appKey;
            _appSecret = appSecret;
        }

        private static string GetUrl(string context)
        {
            var ctx = context;
            if (ctx.StartsWith("/"))
            {
                ctx = ctx.Substring(1);
            }
            return Host + ctx;
        }
                
        public string HttpGet(string apiContext, QueryString queryString)
        {
            return Connect(apiContext, queryString, "GET");
        }

        private string Connect(string apiContext, QueryString queryString, string method)
        {
            var finalParams = AddAdditionParams(queryString).GetEncodedString();
            var url = GetUrl(apiContext) + "?" + finalParams;

            var request = WebRequest.Create(url) as HttpWebRequest;
            request.Method = method;
            request.KeepAlive = true;
            request.ContentType = "application/x-www-form-urlencoded";

            var response = request.GetResponse() as HttpWebResponse;
            var reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
            var result = reader.ReadToEnd();
            reader.Close();
            return result;
        }

        private QueryString AddAdditionParams(QueryString queryStr)
        {
            var qur = queryStr.Clone();
            qur.Add("accessKey",_appKey);
            qur.Add("time", Convert.ToInt64((DateTime.UtcNow - DateTime.Parse("1970-1-1")).TotalMilliseconds) + "");
            qur.Add("sign", Sign(qur));
            return qur;
        }

        private  string Sign(QueryString queryString)
        {
            var sign = _appSecret;
            foreach (var item in queryString.GetSortedDictionary())
            {
                foreach (var val in item.Value)
                {
                    sign += item.Key + val;
                }
            }
            sign += _appSecret;

            return MD5Utility.Compute(sign);
        }

        public string HttpPost(string apiContext, QueryString queryString)
        {
            return Connect(apiContext, queryString, "POST");
        }

        public string HttpUpload(string apiContext, QueryString queryString, FileStream fileStream)
        {
            var finalParams = AddAdditionParams(queryString).GetEncodedString();
            var url = GetUrl(apiContext) + "?" + finalParams;
            var request = WebRequest.Create(url) as HttpWebRequest;
            
            request.Method = "POST";
            request.KeepAlive = true;
            request.Headers["accept-encoding"] = "gzip,deflate";
            request.UserAgent = "onecloud-video-dot-net-client";

            var boundary = MD5Utility.Compute(DateTime.Now.Ticks.ToString("X") + "==");
            request.ContentType = "multipart/form-data; boundary=" + boundary;
            
            var multipartFileHeader = Encoding.UTF8.GetBytes("--" + boundary + "\r\n"
                + "Content-Disposition: form-data; name=\"uploadFile\"; filename=\"" + Path.GetFileName(fileStream.Name) + "\"\r\n"
                + "Content-Type: application/octet-stream\r\n" 
                + "Content-Transfer-Encoding:binary\r\n\r\n");
                       
            var endBoundary = Encoding.UTF8.GetBytes("\r\n--" + boundary + "--\r\n");
           
            request.ContentLength = multipartFileHeader.Length + fileStream.Length + endBoundary.Length;

            using (var stream = request.GetRequestStream())
            {
                // write header
                stream.Write(multipartFileHeader, 0, multipartFileHeader.Length);

                // write file
                var buff = new byte[10240];
                var byteRead = 0;
                var byteWrite = 0;
                while ((byteRead = fileStream.Read(buff, 0, buff.Length)) != 0)
                {
                    byteWrite += byteRead;
                    stream.Write(buff, 0, byteRead);
                }

                // write end boundary
                stream.Write(endBoundary, 0, endBoundary.Length);

            }

            using (var reader = new StreamReader(request.GetResponse().GetResponseStream(), Encoding.UTF8))
            {
                return reader.ReadToEnd();
            }
        }
      
    }
} 
