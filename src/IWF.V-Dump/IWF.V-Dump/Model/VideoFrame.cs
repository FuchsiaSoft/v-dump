using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IWF.V_Dump.Model
{
    public class VideoFrame
    {
        public string ThumbnailPath { get; set; }
        public string FullPath { get; set; }
        public string MD5 { get; set; }
        public string SHA1 { get; set; }
    }
}
