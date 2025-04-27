using System;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;

namespace Steganography
{
    public partial class Form1 : Form
    {
        private PictureBox _pictureBox;
        private TextBox _textBox;
        private TextBox _retrievedTextBox;
        private Button _loadImageButton;
        private Button _embedTextButton;
        private Button _retrieveTextButton;

        public Form1()
        {
            InitializeComponent();
            InitializeControls();
        }

        private void InitializeControls()
        {
            _pictureBox = new PictureBox
            {
                Location = new Point(10, 10),
                Size = new Size(300, 300),
                BorderStyle = BorderStyle.FixedSingle
            };

            _textBox = new TextBox
            {
                Location = new Point(320, 10),
                Size = new Size(200, 100),
                Multiline = true
            };

            _retrievedTextBox = new TextBox
            {
                Location = new Point(320, 240),
                Size = new Size(200, 100),
                Multiline = true,
                ReadOnly = true
            };

            _loadImageButton = new Button
            {
                Text = "Load Image",
                Location = new Point(320, 120),
                Size = new Size(100, 30)
            };
            _loadImageButton.Click += _loadImageButton_Click;

            _embedTextButton = new Button
            {
                Text = "Embed Text",
                Location = new Point(320, 160),
                Size = new Size(100, 30)
            };
            _embedTextButton.Click += _embedTextButton_Click;

            _retrieveTextButton = new Button
            {
                Text = "Retrieve Text",
                Location = new Point(320, 200),
                Size = new Size(100, 30)
            };
            _retrieveTextButton.Click += _retrieveTextButton_Click;

            this.Controls.Add(_pictureBox);
            this.Controls.Add(_textBox);
            this.Controls.Add(_retrievedTextBox);
            this.Controls.Add(_loadImageButton);
            this.Controls.Add(_embedTextButton);
            this.Controls.Add(_retrieveTextButton);

            this.Text = "Steganography Tool";
            this.Size = new Size(550, 400);
        }

        private void _loadImageButton_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Image Files|*.png;*.jpg;*.jpeg";
                openFileDialog.Title = "Select an Image File";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        _pictureBox.Image = new Bitmap(openFileDialog.FileName);
                        _pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error loading image: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void _embedTextButton_Click(object sender, EventArgs e)
        {
            if (_pictureBox.Image == null)
            {
                MessageBox.Show("Please load an image first.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (string.IsNullOrEmpty(_textBox.Text))
            {
                MessageBox.Show("Please enter text to embed.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Bitmap bitmap = new Bitmap(_pictureBox.Image);
            string text = _textBox.Text;
            byte[] textBytes = Encoding.UTF8.GetBytes(text);
            int textLength = textBytes.Length;

            // Check if image can hold the text
            int imageCapacity = (bitmap.Width * bitmap.Height * 3) / 8;
            if (textLength + 4 > imageCapacity) // +4 for length header
            {
                MessageBox.Show("Text is too long to embed in this image.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Embed length of text as 4 bytes
            int bitIndex = 0;
            for (int i = 0; i < 32; i++)
            {
                int x = (bitIndex / 3) % bitmap.Width;
                int y = (bitIndex / 3) / bitmap.Width;
                Color pixel = bitmap.GetPixel(x, y);
                int bit = (textLength >> (31 - i)) & 1;
                pixel = Color.FromArgb(pixel.A, (pixel.R & ~1) | bit, pixel.G, pixel.B);
                bitmap.SetPixel(x, y, pixel);
                bitIndex += 3;
            }

            for (int i = 0; i < textLength * 8; i++)
            {
                int x = (bitIndex / 3) % bitmap.Width;
                int y = (bitIndex / 3) / bitmap.Width;
                Color pixel = bitmap.GetPixel(x, y);
                int bit = (textBytes[i / 8] >> (7 - (i % 8))) & 1;
                pixel = Color.FromArgb(pixel.A, (pixel.R & ~1) | bit, pixel.G, pixel.B);
                bitmap.SetPixel(x, y, pixel);
                bitIndex += 3;
            }

            _pictureBox.Image = bitmap;
            MessageBox.Show("Text embedded successfully. Save the image to keep the embedded text.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "PNG Image|*.png|JPEG Image|*.jpg";
                saveFileDialog.Title = "Save Image With Embedded Text";
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    bitmap.Save(saveFileDialog.FileName, saveFileDialog.FilterIndex == 1 ? ImageFormat.Png : ImageFormat.Jpeg);
                }
            }
        }

        private void _retrieveTextButton_Click(object sender, EventArgs e)
        {
            if (_pictureBox.Image == null)
            {
                MessageBox.Show("Please load an image first.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Bitmap bitmap = new Bitmap(_pictureBox.Image);
            int textLength = 0;
            int bitIndex = 0;

            // Read the length of the embedded text (first 32 bits)
            for (int i = 0; i < 32; i++)
            {
                int x = (bitIndex / 3) % bitmap.Width;
                int y = (bitIndex / 3) / bitmap.Width;
                Color pixel = bitmap.GetPixel(x, y);
                int bit = pixel.R & 1;
                textLength = (textLength << 1) | bit;
                bitIndex += 3;
            }

            if (textLength <= 0 || textLength > (bitmap.Width * bitmap.Height * 3) / 8)
            {
                MessageBox.Show("No valid text found in the image.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                _retrievedTextBox.Text = string.Empty;
                return;
            }

            byte[] textBytes = new byte[textLength];
            for (int i = 0; i < textLength * 8; i++)
            {
                int x = (bitIndex / 3) % bitmap.Width;
                int y = (bitIndex / 3) / bitmap.Width;
                Color pixel = bitmap.GetPixel(x, y);
                int bit = pixel.R & 1;
                textBytes[i / 8] = (byte)((textBytes[i / 8] << 1) | bit);
                bitIndex += 3;
            }

            string retrievedText = Encoding.UTF8.GetString(textBytes);
            _retrievedTextBox.Text = retrievedText;
            MessageBox.Show("Text retrieved successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}