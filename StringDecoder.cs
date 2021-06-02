using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace kov.NET.Utils
{
    public static class StringDecoder
    {
        private const string Key = "Ta284WGc29asWL2F";
        private const string IV = "h6iAm3fHwFdVbuIH";
        public static string Decrypt(string str)
        {
            char[] charArray = str.ToCharArray();
            Array.Reverse(charArray);
            string a2 = new string(charArray);

            byte[] encbytes = Convert.FromBase64String(a2);
            AesCryptoServiceProvider encdec = new AesCryptoServiceProvider();
            encdec.BlockSize = 128;
            encdec.KeySize = 256;
            encdec.Key = ASCIIEncoding.ASCII.GetBytes(Key);
            encdec.IV = ASCIIEncoding.ASCII.GetBytes(IV);
            encdec.Padding = PaddingMode.PKCS7;
            encdec.Mode = CipherMode.CBC;

            ICryptoTransform icrypt = encdec.CreateDecryptor(encdec.Key, encdec.IV);

            byte[] dec = icrypt.TransformFinalBlock(encbytes, 0, encbytes.Length);
            icrypt.Dispose();

            return ASCIIEncoding.ASCII.GetString(dec);

        }
        
    }

}
