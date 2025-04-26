using System;
using System.Windows.Forms;

namespace Steganography
{
    public partial class Form1 : Form
    {
        private PictureBox pictureBox;
        private TextBox textBox;
        private TextBox retrievedTextBox;
        private Button loadImageButton;
        private Button embedTextButton;
        private Button retrieveTextButton;

        public Form1()
        {
            InitializeComponent();
            InitializeControls();
        }

        private void InitializeControls()
        {
            pictureBox = new PictureBox
            {
                Location = new System.Drawing.Point(10, 10),
                Size = new System.Drawing.Size(300, 300),
                BorderStyle = BorderStyle.FixedSingle
            };

            textBox = new TextBox
            {
                Location = new System.Drawing.Point(320, 10),
                Size = new System.Drawing.Size(200, 100),
                Multiline = true
            };

            retrievedTextBox = new TextBox
            {
                Location = new System.Drawing.Point(320, 240),
                Size = new System.Drawing.Size(200, 100),
                Multiline = true,
                ReadOnly = true
            };

            loadImageButton = new Button
            {
                Text = "Load Image",
                Location = new System.Drawing.Point(320, 120),
                Size = new System.Drawing.Size(100, 30)
            };
            loadImageButton.Click += LoadImageButton_Click;

            embedTextButton = new Button
            {
                Text = "Embed Text",
                Location = new System.Drawing.Point(320, 160),
                Size = new System.Drawing.Size(100, 30)
            };
            embedTextButton.Click += EmbedTextButton_Click;

            retrieveTextButton = new Button
            {
                Text = "Retrieve Text",
                Location = new System.Drawing.Point(320, 200),
                Size = new System.Drawing.Size(100, 30)
            };
            retrieveTextButton.Click += RetrieveTextButton_Click;

            this.Controls.Add(pictureBox);
            this.Controls.Add(textBox);
            this.Controls.Add(retrievedTextBox);
            this.Controls.Add(loadImageButton);
            this.Controls.Add(embedTextButton);
            this.Controls.Add(retrieveTextButton);

            this.Text = "Steganography Tool";
            this.Size = new System.Drawing.Size(550, 400);
        }

        private void LoadImageButton_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Image Files|*.png;*.jpg;*.jpeg";
                openFileDialog.Title = "Select an Image File";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        pictureBox.Image = new System.Drawing.Bitmap(openFileDialog.FileName);
                        pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error loading image: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void EmbedTextButton_Click(object sender, EventArgs e)
        {
            // Functionality to embed text will be added later
        }

        private void RetrieveTextButton_Click(object sender, EventArgs e)
        {
            // Functionality to retrieve text will be added later
        }
    }
}