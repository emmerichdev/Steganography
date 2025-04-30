using System;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Imaging;
using Steganography.embed;
using Steganography.retrieve;
using Steganography.security;
using System.Threading.Tasks;

namespace Steganography
{
    public partial class Form : System.Windows.Forms.Form
    {
        private PictureBox _pictureBox;
        private TextBox _textBox;
        private TextBox _retrievedTextBox;
        private Button _loadImageButton;
        private Button _embedTextButton;
        private Button _retrieveTextButton;
        private Button _regenerateKeysButton;
        private Bitmap _originalImage;

        public Form()
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

            _regenerateKeysButton = new Button
            {
                Text = "Regenerate Keys",
                Location = new Point(430, 120),
                Size = new Size(100, 30)
            };
            _regenerateKeysButton.Click += _regenerateKeysButton_Click;

            this.Controls.Add(_pictureBox);
            this.Controls.Add(_textBox);
            this.Controls.Add(_retrievedTextBox);
            this.Controls.Add(_loadImageButton);
            this.Controls.Add(_embedTextButton);
            this.Controls.Add(_retrieveTextButton);
            this.Controls.Add(_regenerateKeysButton);

            this.Text = "Steganography Tool";
            this.Size = new Size(550, 400);
        }

        private async void _loadImageButton_Click(object sender, EventArgs e)
        {
            using OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files|*.png;*.jpg;*.jpeg";
            openFileDialog.Title = "Select an Image File";

            if (openFileDialog.ShowDialog() != DialogResult.OK) return;
            try
            {
                _originalImage = await Task.Run(() => new Bitmap(openFileDialog.FileName));
                _pictureBox.Image = _originalImage;
                _pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading image: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void _embedTextButton_Click(object sender, EventArgs e)
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

            try
            {
                Bitmap bitmap = new Bitmap(_originalImage);
                string encryptedText = Encryption.EncryptString(_textBox.Text);
                string textWithLength = encryptedText.Length.ToString("D4") + encryptedText; // Prefix with length (4 digits)
                Bitmap resultImage = Embedding.EmbedData(bitmap, textWithLength);
                _pictureBox.Image = resultImage;
                MessageBox.Show("Text embedded successfully. Save the image to keep the embedded text.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                using SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "PNG Image|*.png|JPEG Image|*.jpg";
                saveFileDialog.Title = "Save Image With Embedded Text";
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    await Task.Run(() => resultImage.Save(saveFileDialog.FileName, saveFileDialog.FilterIndex == 1 ? ImageFormat.Png : ImageFormat.Jpeg));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error embedding text: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void _retrieveTextButton_Click(object sender, EventArgs e)
        {
            if (_pictureBox.Image == null)
            {
                MessageBox.Show("Please load an image first.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                Bitmap bitmap = new Bitmap(_pictureBox.Image);
                // First retrieve the length (assuming it's the first 4 characters)
                string lengthStr = await Task.Run(() => Retrieving.RetrieveData(bitmap, 4));
                if (int.TryParse(lengthStr, out int textLength))
                {
                    string encryptedText = await Task.Run(() => Retrieving.RetrieveData(bitmap, textLength + 4)).ContinueWith(t => t.Result.Substring(4));
                    string decryptedText = Encryption.DecryptString(encryptedText);
                    _retrievedTextBox.Text = decryptedText;
                    MessageBox.Show("Text retrieved successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("No valid text found in the image.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    _retrievedTextBox.Text = string.Empty;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error retrieving text: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                _retrievedTextBox.Text = string.Empty;
            }
        }

        private void _regenerateKeysButton_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show(
                "Warning: Regenerating keys will make it impossible to decrypt any previously encrypted images.\n\n" +
                "Are you sure you want to continue?",
                "Regenerate Keys",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button2); // No is the default option, always

            if (result != DialogResult.Yes)
            {
                return;
            }

            try
            {
                Encryption.RegenerateKeys();
                MessageBox.Show(
                    "Encryption keys have been regenerated successfully.",
                    "Keys Regenerated",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error regenerating keys: " + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
    }
}