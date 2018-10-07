using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IWF.V_Dump.Model
{
    public class OutputModel
    {
        public string Name { get; set; }
        public string MD5 { get; set; }
        public string SHA1 { get; set; }
        public string PHash { get; set; }
        public bool HasFace { get; set; }
    }
}
