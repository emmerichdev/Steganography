using System;
using System.Text;

namespace Steganography.converters
{
    public static class BytesConverter
    {
        public static string BytesToBinary(byte[] bytes)
        {
            StringBuilder binary = new StringBuilder();
            foreach (byte b in bytes)
            {
                string binaryString = Convert.ToString(b, 2).PadLeft(8, '0');
                binary.Append(binaryString);
            }
            return binary.ToString();
        }
    }
} 