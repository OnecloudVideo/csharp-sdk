using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Pispower.Video.RestfulApi.Video.MultipartUpload
{
    class FileUploadRequest
    {
        /// <summary>
        /// 获取/设置上传到某一Catalog的CatalogId。
        /// </summary>
        public Int32 CatalogId { get; set; }

        /// <summary>
        /// 获取/设置上传文件。
        /// </summary>
        public System.IO.FileInfo FileInfo { get; set; }

        /// <summary>
        /// 获取/设置分片大小。
        /// </summary>
        public int PartSize { get; set; }

    }
}
