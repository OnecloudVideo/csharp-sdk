using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pispower.Video.RestfulApi.Video
{
    class Video
    {
        /// <summary>
        /// 获取/设置Video的Id。
        /// </summary>
        public Int32 Id { get; set; }

        /// <summary>
        /// 获取/设置Video的名称。
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// 获取/设置Video的大小。
        /// </summary>
        public long Size { get; set; }

        /// <summary>
        /// 获取/设置Video的时长。
        /// </summary>
        public long Duration { get; set; }

        /// <summary>
        /// 获取/设置Video的状态。
        /// </summary>
        public String Status { get; set; }

        /// <summary>
        /// 获取/设置Video的类型。
        /// </summary>
        public String Type { get; set; }

        /// <summary>
        /// 获取/设置Video的描述信息。
        /// </summary>
        public String Description { get; set; }

        /// <summary>
        /// 获取/设置Video所在Catalog的catalogId。
        /// </summary>
        public Int32 CatalogId { get; set; }

        /// <summary>
        /// 获取/设置Video所在Catalog的catalogName。
        /// </summary>
        public String CatalogName { get; set; }

        /// <summary>
        /// 获取/设置Video的VideoEmbedCode列表。
        /// </summary>
        public List<VideoEmbedCode> EmbedCodes { get; set; }

     }
}
