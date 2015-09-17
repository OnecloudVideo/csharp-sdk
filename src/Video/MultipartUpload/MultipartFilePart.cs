using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pispower.Video.RestfulApi.Video.MultipartUpload
{
    class MultipartFilePart
    {
        /// <summary>
        /// 获取/设置上传未完成的分片列表。
        /// </summary>
        public List<FilePart> UnfinishedParts { get; set; }

        /// <summary>
        /// 获取/设置上传完成的分片列表。
        /// </summary>
        public List<FilePart> FinishedParts { get; set; }

        /// <summary>
        /// 获取/设置错误的分片列表。
        /// </summary>
        public List<FilePart> ErrorParts { get; set; }

    }
}
