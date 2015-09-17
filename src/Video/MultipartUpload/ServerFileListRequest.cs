using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pispower.Video.RestfulApi.Video.MultipartUpload
{
    class ServerFileListRequest
    {
        /// <summary>
        /// 获取/设置文件的FileNameLike
        /// </summary>
        public String FileNameLike { get; set; }

        /// <summary>
        /// 获取/设置文件FileMD5Equal。
        /// </summary>
        public String FileMD5Equal { get; set; }
    }
}
