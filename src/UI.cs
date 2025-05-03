using System.Windows.Forms;
using MaterialSkin.Controls;

namespace Steganography
{
    public class Ui
    {
        public PictureBox PictureBox { get; private set; }
        public TextBox TextBox { get; private set; }
        public TextBox RetrievedTextBox { get; private set; }
        public Button LoadImageButton { get; private set; }
        public Button EmbedTextButton { get; private set; }
        public Button RetrieveTextButton { get; private set; }
        public Button RegenerateKeysButton { get; private set; }

        public Ui(MaterialForm form)
        {
            InitializeControls(form);
        }

        private void InitializeControls(MaterialForm form)
        {
            TableLayoutPanel layoutPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 5,
                Padding = new Padding(10)
            };
            layoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60F));
            layoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40F));
            layoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 60F));
            layoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
            layoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
            layoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 40F));
            layoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));

            PictureBox = new PictureBox
            {
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.FixedSingle,
                SizeMode = PictureBoxSizeMode.Zoom
            };
            layoutPanel.Controls.Add(PictureBox, 0, 0);
            layoutPanel.SetRowSpan(PictureBox, 5);

            LoadImageButton = new Button
            {
                Text = "Load Image",
                Dock = DockStyle.Fill
            };
            layoutPanel.Controls.Add(LoadImageButton, 1, 4);

            TabControl tabControl = new TabControl
            {
                Dock = DockStyle.Fill
            };
            layoutPanel.Controls.Add(tabControl, 1, 0);
            layoutPanel.SetRowSpan(tabControl, 4);

            // Embed Tab
            TabPage embedTab = new TabPage("Embed");
            tabControl.TabPages.Add(embedTab);
            TableLayoutPanel embedPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                Padding = new Padding(5)
            };
            embedPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 60F));
            embedPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
            embedTab.Controls.Add(embedPanel);

            TextBox = new TextBox
            {
                Dock = DockStyle.Fill,
                Multiline = true
            };
            embedPanel.Controls.Add(TextBox, 0, 0);

            EmbedTextButton = new Button
            {
                Text = "Embed Text",
                Dock = DockStyle.Fill
            };
            embedPanel.Controls.Add(EmbedTextButton, 0, 1);

            // Retrieve Tab
            TabPage retrieveTab = new TabPage("Retrieve");
            tabControl.TabPages.Add(retrieveTab);
            TableLayoutPanel retrievePanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                Padding = new Padding(5)
            };
            retrievePanel.RowStyles.Add(new RowStyle(SizeType.Percent, 60F));
            retrievePanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
            retrieveTab.Controls.Add(retrievePanel);

            RetrievedTextBox = new TextBox
            {
                Dock = DockStyle.Fill,
                Multiline = true,
                ReadOnly = true
            };
            retrievePanel.Controls.Add(RetrievedTextBox, 0, 0);

            RetrieveTextButton = new Button
            {
                Text = "Retrieve Text",
                Dock = DockStyle.Fill
            };
            retrievePanel.Controls.Add(RetrieveTextButton, 0, 1);

            // Settings Tab
            TabPage settingsTab = new TabPage("Settings");
            tabControl.TabPages.Add(settingsTab);
            TableLayoutPanel settingsPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 1,
                Padding = new Padding(5)
            };
            settingsPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
            settingsTab.Controls.Add(settingsPanel);

            RegenerateKeysButton = new Button
            {
                Text = "Regenerate Keys",
                Dock = DockStyle.Fill
            };
            settingsPanel.Controls.Add(RegenerateKeysButton, 0, 0);

            form.Controls.Add(layoutPanel);
        }
    }
} 