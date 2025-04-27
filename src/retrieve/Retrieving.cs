using System.Drawing;
using System.Text;
using Steganography.converters;

namespace Steganography.retrieve
{
    public static class Retrieving
    {
        public static string RetrieveData(Bitmap image, int dataLength)
        {
            StringBuilder dataBinary = new StringBuilder();
            int dataIndex = 0;

            for (int y = 0; y < image.Height && dataIndex < dataLength * 8; y++)
            {
                for (int x = 0; x < image.Width && dataIndex < dataLength * 8; x++)
                {
                    Color pixelColor = image.GetPixel(x, y);
                    dataBinary.Append(pixelColor.R & 1);
                    dataIndex++;
                    if (dataIndex < dataLength * 8)
                    {
                        dataBinary.Append(pixelColor.G & 1);
                        dataIndex++;
                    }
                    if (dataIndex < dataLength * 8)
                    {
                        dataBinary.Append(pixelColor.B & 1);
                        dataIndex++;
                    }
                }
            }

            return BinaryConverter.BinaryToString(dataBinary.ToString());
        }
    }
} 