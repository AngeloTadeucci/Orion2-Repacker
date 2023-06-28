using System.Security.Cryptography;

namespace Orion.Window.Common
{
    public static class Helpers
    {
        public static string CreateHash(string sHeaderUOL)
        {
            if (!File.Exists(sHeaderUOL)) return "";

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