using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

using Newtonsoft.Json.Linq;

using Pispower.Video.RestfulApi.Internal;

namespace Pispower.Video.RestfulApi.Catalog
{
    class CatalogService
    {
        private readonly Client client;

        public CatalogService(string appKey, string appScrect)
        {
            client = new Client(appKey, appScrect);
        }
        
        /// <summary>
        /// 增加一个Catalog
        /// </summary>
        /// <param name="name">Catalog的名字</param>
        /// <returns>Catalog 包含id,name字段，但videoNumber字段的值为空</returns>
        public Catalog Add(String name)
        {
            var qs = new QueryString();
            qs.Add("name", name);
            var createCata = client.HttpPost("/catalog/create.api", qs);
            var jo = JObject.Parse(createCata);

            if (jo["statusCode"].ToString().CompareTo("0") == 0)
            {
                var cata = new Catalog();
                cata.Name = jo["name"].ToString();
                var id = jo["id"].ToString();
                cata.Id = System.Int32.Parse(id);
                return cata;
            }
            else
            {
                throw new PispowerAPIException(System.Int32.Parse(jo["statusCode"].ToString()), jo["message"].ToString());
            }                          
        }

        /// <summary>
        /// 列出Catalog
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Catalog列表，包含id,name字段,但videoNumber字段的值为空</returns>
        public List<Catalog> List(CatalogListRequest request)
        {   
            var qs = new QueryString();
            if (null != request.NameLike && !"".Equals(request.NameLike))
            {
                qs.Add("nameLike", request.NameLike);
            }
            if (request.Page > 0)
            {
                qs.Add("page", request.Page.ToString());
            }
            if (request.MaxResult > 0)
            {
                qs.Add("maxResult", request.MaxResult.ToString());
            }

            var listCata = client.HttpGet("/catalog/list.api", qs);

            var jo = JObject.Parse(listCata);
            if (jo["statusCode"].ToString().CompareTo("0") == 0)
            {
                var catalogs = jo["catalogs"];
                var list = new List<Catalog>();
                foreach (Object obj in catalogs)
                {
                    var jObj = (JObject)obj;
                    var cata = new Catalog();
                    cata.Id = System.Int32.Parse(jObj["id"].ToString());
                    cata.Name = jObj["name"].ToString();

                    list.Add(cata);
                }
                return list;
            }
            else
            {
                throw new PispowerAPIException(System.Int32.Parse(jo["statusCode"].ToString()), jo["message"].ToString());
            }                          
        }

        /// <summary>
        /// 根据id查询Catalog
        /// </summary>
        /// <param name="catalogId">Catalog的唯一标识</param>
        /// <returns>Catalog 包含name,videoNumber字段, 但id字段的值为空</returns>
        public Catalog Get(Int32 catalogId)
        {
            QueryString qs = new QueryString();
            qs.Add("catalogId", catalogId.ToString());
            var getCata = client.HttpGet("/catalog/get.api", qs);
            var jo = JObject.Parse(getCata);

            if (jo["statusCode"].ToString().CompareTo("0") == 0)
            {
                Console.WriteLine("name:" + jo["name"]);
                var cata = new Catalog();
                cata.Name = jo["name"].ToString();
                var videoNum = jo["videoNumber"].ToString();
                cata.VideoNumber = System.Int32.Parse(videoNum);
                return cata;
            }
            else
            {
                throw new PispowerAPIException(System.Int32.Parse(jo["statusCode"].ToString()), jo["message"].ToString());
            }                          
        }

        /// <summary>
        /// 删除一个Catalog
        /// </summary>
        /// <param name="catalogId">Catalog的唯一标识</param>
        public void Delete(Int32 catalogId)
        {
            var qs = new QueryString();
            qs.Add("catalogId", catalogId.ToString());
            var deleteCata = client.HttpPost("/catalog/delete.api", qs);
            var jo = JObject.Parse(deleteCata);

            if (jo["statusCode"].ToString().CompareTo("0") != 0)
            {
                throw new PispowerAPIException(System.Int32.Parse(jo["statusCode"].ToString()), jo["message"].ToString());
            }
            
        }
    }
}
