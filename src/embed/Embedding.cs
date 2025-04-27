using System;
using System.Drawing;
using System.Text;
using Steganography.converters;

namespace Steganography.embed
{
    public static class Embedding
    {
        public static Bitmap EmbedData(Bitmap image, string data)
        {
            // Convert data to binary
            byte[] dataBytes = Encoding.UTF8.GetBytes(data);
            string dataBinary = BytesConverter.BytesToBinary(dataBytes);

            // Check if the image can hold the data
            int imageCapacity = (image.Width * image.Height * 3) / 8;
            if (dataBinary.Length > imageCapacity)
            {
                throw new Exception("Image capacity is not enough to hold the data.");
            }

            // Copy the original image data
            Bitmap resultImage = (Bitmap)image.Clone();
            int dataIndex = 0;

            for (int y = 0; y < image.Height && dataIndex < dataBinary.Length; y++)
            {
                for (int x = 0; x < image.Width && dataIndex < dataBinary.Length; x++)
                {
                    Color pixelColor = resultImage.GetPixel(x, y);
                    int r = pixelColor.R;
                    int g = pixelColor.G;
                    int b = pixelColor.B;

                    if (dataIndex < dataBinary.Length)
                    {
                        r = (r & ~1) | (dataBinary[dataIndex] - '0');
                        dataIndex++;
                    }
                    if (dataIndex < dataBinary.Length)
                    {
                        g = (g & ~1) | (dataBinary[dataIndex] - '0');
                        dataIndex++;
                    }
                    if (dataIndex < dataBinary.Length)
                    {
                        b = (b & ~1) | (dataBinary[dataIndex] - '0');
                        dataIndex++;
                    }

                    resultImage.SetPixel(x, y, Color.FromArgb(pixelColor.A, r, g, b));
                }
            }

            return resultImage;
        }
    }
} 