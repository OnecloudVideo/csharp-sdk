using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pispower.Video.RestfulApi.Video
{
    class VideoListRequest
    {
        /// <summary>
        /// 获取/设置Video的NameLike。
        /// </summary>
        public String NameLike { get; set; }

        /// <summary>
        /// 获取/设置Video所在Catalog的CatalogNameLike。
        /// </summary>
        public String CatalogNameLike { get; set; }

        /// <summary>
        /// 获取/设置Video所在Catalog的CatalogId。
        /// </summary>
        public Int32 CatalogId { get; set; }

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
