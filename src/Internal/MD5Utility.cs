using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace Pispower.Video.RestfulApi.Internal
{
    class MD5Utility
    {
        public static string Compute(System.IO.FileInfo file)
        {
            using(var fileStream = new FileStream(file.FullName, FileMode.Open, FileAccess.Read))
            {
                return BytesToHex(MD5.Create().ComputeHash(fileStream));  
            }
        }

        public static string Compute(string str)
        {
            return BytesToHex(MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(str)));
        }

        private static string BytesToHex(byte[] bytes)
        {
            var hex = "";
            for (var i = 0; i < bytes.Length; i++)
            {
                var aByte = (int)bytes[i];
                if(aByte < 0){
                    aByte += 256;
                }
                if (aByte < 16)
                {
                    hex += "0";
                }
                hex += aByte.ToString("X");
            }
            return hex;
        }
    }
}
