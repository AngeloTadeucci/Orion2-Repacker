using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Orion.Window.Common
{
    public static class Helpers
    {
        public static string CreateHash(string sHeaderUOL)
        {
            if (!File.Exists(sHeaderUOL))
            {
                return "";
            }

            using (MD5 md5 = MD5.Create())
            {
                using (FileStream stream = File.OpenRead(sHeaderUOL))
                {
                    byte[] hash = md5.ComputeHash(stream);
                    return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                }
            }
        }
    }
}
