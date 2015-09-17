using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pispower.Video.RestfulApi.Catalog
{
    class Catalog
    {
        /// <summary>
        /// 获取/设置Catalog的Id。
        /// </summary>
        public Int32 Id { get; set; }

        /// <summary>
        /// 获取/设置Catalog的名称。
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// 获取/设置Catalog中的视频个数。
        /// </summary>
        public int VideoNumber { get; set; }

    }
}
