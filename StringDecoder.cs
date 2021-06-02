using System;
using System.Linq;
using System.Text;

namespace kov.NET.Utils
{
    public static class StringDecoder
    {
        public static string Decrypt(string str)
        {
            char[] charArray = str.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);

        }
        
    }

}
