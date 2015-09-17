using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pispower.Video.RestfulApi.Catalog
{
    class CatalogListRequest
    {
        /// <summary>
        /// 获取/设置NameLike。
        /// </summary>
        public String NameLike { get; set; }

        /// <summary>
        /// 获取/设置Page。
        /// </summary>
        public Int32 Page { get; set; }

        /// <summary>
        /// 获取/设置MaxResult。
        /// </summary>
        public Int32 MaxResult { get; set; }
    }
}
