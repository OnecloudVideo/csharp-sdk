using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Pispower.Video.RestfulApi.Video
{
    class VideoUploadRequest
    {
        public Int32 CatalogId { get; set; }

        public String Name { get; set; }

        public String Description { get; set; }

        public System.IO.FileInfo FileInfo { get; set; }

    }
}
