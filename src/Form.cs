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
            var materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkinManager.Themes.DARK;
            materialSkinManager.ColorScheme = new ColorScheme(Primary.Grey800, Primary.Grey900, Primary.Grey500, Accent.Red200, TextShade.WHITE);
        }

        private void InitializeControls()
        {
            TableLayoutPanel layoutPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 4,
                Padding = new Padding(10)
            };
            layoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60F));
            layoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40F));
            layoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 60F));
            layoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
            layoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
            layoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 40F));

            _pictureBox = new PictureBox
            {
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.FixedSingle,
                SizeMode = PictureBoxSizeMode.Zoom
            };
            layoutPanel.Controls.Add(_pictureBox, 0, 0);
            layoutPanel.SetRowSpan(_pictureBox, 4);

            _textBox = new TextBox
            {
                Dock = DockStyle.Fill,
                Multiline = true
            };
            layoutPanel.Controls.Add(_textBox, 1, 0);

            _loadImageButton = new Button
            {
                Text = "Load Image",
                Dock = DockStyle.Fill
            };
            _loadImageButton.Click += _loadImageButton_Click;
            layoutPanel.Controls.Add(_loadImageButton, 1, 1);

            _embedTextButton = new Button
            {
                Text = "Embed Text",
                Dock = DockStyle.Fill
            };
            _embedTextButton.Click += _embedTextButton_Click;
            layoutPanel.Controls.Add(_embedTextButton, 1, 2);

            _retrieveTextButton = new Button
            {
                Text = "Retrieve Text",
                Dock = DockStyle.Fill
            };
            _retrieveTextButton.Click += _retrieveTextButton_Click;
            layoutPanel.Controls.Add(_retrieveTextButton, 1, 3);
            layoutPanel.SetColumnSpan(_retrieveTextButton, 1);

            _regenerateKeysButton = new Button
            {
                Text = "Regenerate Keys",
                Dock = DockStyle.Fill
            };
            _regenerateKeysButton.Click += _regenerateKeysButton_Click;
            layoutPanel.Controls.Add(_regenerateKeysButton, 1, 1);
            layoutPanel.SetColumnSpan(_regenerateKeysButton, 1);
            layoutPanel.SetRowSpan(_regenerateKeysButton, 2);

            _retrievedTextBox = new TextBox
            {
                Dock = DockStyle.Fill,
                Multiline = true,
                ReadOnly = true
            };
            layoutPanel.Controls.Add(_retrievedTextBox, 1, 3);

            this.Controls.Add(layoutPanel);

            this.Text = "Steganography Tool";
            this.Size = new Size(800, 600);
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