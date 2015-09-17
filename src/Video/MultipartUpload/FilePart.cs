using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Pispower.Video.RestfulApi.Video.MultipartUpload
{
    class FilePart
    {
        /// <summary>
        /// 获取/设置分片的PartNum。
        /// </summary>
        public Int32 PartNum { get; set; }

        /// <summary>
        /// 获取/设置分片的PartKey。
        /// </summary>
        public String PartKey { get; set; }

        /// <summary>
        /// 获取/设置分片的MD5。
        /// </summary>
        public String PartMD5 { get; set; }

        /// <summary>
        /// 获取/设置文件分片。
        /// </summary>
        public System.IO.FileInfo FilePartInfo { get; set; }

    }
}
