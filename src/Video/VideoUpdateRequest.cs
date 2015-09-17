using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pispower.Video.RestfulApi.Video
{
    class VideoUpdateRequest
    {
        /// <summary>
        /// 获取/设置Video的VideoId。
        /// </summary>
        public Int32 VideoId { get; set; }

        /// <summary>
        /// 获取/设置Video的名称。
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// 获取/设置Video的描述信息。
        /// </summary>
        public String Description { get; set; }

    }
}
