using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace kov.NET.Utils
{
    public static class StringDecoder
    {
        public static string Decrypt(string text, int key)
        {
            StringBuilder input = new StringBuilder(text);
            StringBuilder output = new StringBuilder(text.Length);
            char Textch;
            for (int iCount = 0; iCount < text.Length; iCount++)
            {
                Textch = input[iCount];
                Textch = (char)(Textch ^ key);
                output.Append(Textch);
            }
            return output.ToString();
        }

    }

}
