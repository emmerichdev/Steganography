using System;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Imaging;
using Steganography.embed;
using Steganography.retrieve;
using Steganography.security;
using System.Threading.Tasks;
using MaterialSkin;
using MaterialSkin.Controls;

namespace Steganography
{
    public partial class Form : MaterialForm
    {
        private Ui _ui;
        private Bitmap _originalImage;

        public Form()
        {
            InitializeComponent();
            var materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkinManager.Themes.DARK;
            materialSkinManager.ColorScheme = new ColorScheme(Primary.Grey900, Primary.Grey800, Primary.Grey700, Accent.Red400, TextShade.WHITE);
            _ui = new Ui(this);
            SetupEventHandlers();
            this.Text = "Steganography Tool";
            this.Size = new Size(800, 600);
        }

        private void SetupEventHandlers()
        {
            _ui.LoadImageButton.Click += _loadImageButton_Click;
            _ui.EmbedTextButton.Click += _embedTextButton_Click;
            _ui.RetrieveTextButton.Click += _retrieveTextButton_Click;
            _ui.RegenerateKeysButton.Click += _regenerateKeysButton_Click;
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
                _ui.PictureBox.Image = _originalImage;
                _ui.PictureBox.SizeMode = PictureBoxSizeMode.Zoom;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading image: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void _embedTextButton_Click(object sender, EventArgs e)
        {
            if (_ui.PictureBox.Image == null)
            {
                MessageBox.Show("Please load an image first.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (string.IsNullOrEmpty(_ui.TextBox.Text))
            {
                MessageBox.Show("Please enter text to embed.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                Bitmap bitmap = new Bitmap(_originalImage);
                string encryptedText = Encryption.EncryptString(_ui.TextBox.Text);
                string textWithLength = encryptedText.Length.ToString("D4") + encryptedText; // Prefix with length (4 digits)
                Bitmap resultImage = Embedding.EmbedData(bitmap, textWithLength);
                _ui.PictureBox.Image = resultImage;
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
            if (_ui.PictureBox.Image == null)
            {
                MessageBox.Show("Please load an image first.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                Bitmap bitmap = new Bitmap(_ui.PictureBox.Image);
                // First retrieve the length (assuming it's the first 4 characters)
                string lengthStr = await Task.Run(() => Retrieving.RetrieveData(bitmap, 4));
                if (int.TryParse(lengthStr, out int textLength))
                {
                    string encryptedText = await Task.Run(() => Retrieving.RetrieveData(bitmap, textLength + 4)).ContinueWith(t => t.Result.Substring(4));
                    string decryptedText = Encryption.DecryptString(encryptedText);
                    _ui.RetrievedTextBox.Text = decryptedText;
                    MessageBox.Show("Text retrieved successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("No valid text found in the image.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    _ui.RetrievedTextBox.Text = string.Empty;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error retrieving text: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                _ui.RetrievedTextBox.Text = string.Empty;
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