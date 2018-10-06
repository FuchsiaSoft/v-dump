using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace IWF.V_Dump.Model
{
    class HashHelper
    {
        public static string GetMD5(string path)
        {
            using (MD5 md5 = MD5.Create())
            using (Stream stream = File.OpenRead(path))
            {
                byte[] hash = md5.ComputeHash(stream);
                string output = BitConverter.ToString(hash).Replace("-", string.Empty);
                return output;
            }
        }

        public static string GetSHA1(string path)
        {
            using (SHA1 sha1 = SHA1.Create())
            using (Stream stream = File.OpenRead(path))
            {
                byte[] hash = sha1.ComputeHash(stream);
                string output = BitConverter.ToString(hash).Replace("-", string.Empty);
                return output;
            }
        }
    }
}
