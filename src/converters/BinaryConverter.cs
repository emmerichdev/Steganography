using System;
using System.Text;

namespace Steganography.converters
{
    public static class BinaryConverter
    {
        public static string BinaryToString(string binary)
        {
            StringBuilder text = new StringBuilder();
            for (int i = 0; i < binary.Length; i += 8)
            {
                string byteString = binary.Substring(i, Math.Min(8, binary.Length - i));
                if (byteString.Length == 8)
                {
                    byte b = Convert.ToByte(byteString, 2);
                    text.Append((char)b);
                }
            }
            return text.ToString();
        }
    }
} 