using System.Windows.Forms;
using System.Drawing;
using MaterialSkin.Controls;

namespace Steganography
{
    public class Ui
    {
        public PictureBox PictureBox { get; private set; }
        public MaterialTextBox TextBox { get; private set; }
        public MaterialTextBox RetrievedTextBox { get; private set; }
        public MaterialButton LoadImageButton { get; private set; }
        public MaterialButton EmbedTextButton { get; private set; }
        public MaterialButton RetrieveTextButton { get; private set; }
        public MaterialButton RegenerateKeysButton { get; private set; }

        private readonly Color _darkBackground = Color.FromArgb(38, 38, 38);
        private readonly Color _darkSecondary = Color.FromArgb(51, 51, 51);
        private readonly Color _textBoxBackground = Color.FromArgb(45, 45, 45);

        public Ui(MaterialForm form)
        {
            InitializeControls(form);
        }

        private void InitializeControls(MaterialForm form)
        {
            // Create the main container
            Panel mainContainer = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = _darkBackground
            };
            form.Controls.Add(mainContainer);

            // Add tab control first
            MaterialTabControl tabControl = new MaterialTabControl
            {
                Multiline = false,
                Depth = 0,
                BackColor = _darkBackground
            };

            // Create tab pages first
            TabPage embedTab = new TabPage("Embed");
            embedTab.BackColor = _darkBackground;
            
            TabPage retrieveTab = new TabPage("Retrieve");
            retrieveTab.BackColor = _darkBackground;
            
            TabPage settingsTab = new TabPage("Settings");
            settingsTab.BackColor = _darkBackground;
            
            tabControl.TabPages.Add(embedTab);
            tabControl.TabPages.Add(retrieveTab);
            tabControl.TabPages.Add(settingsTab);

            // Create tabs
            MaterialTabSelector tabSelector = new MaterialTabSelector
            {
                Dock = DockStyle.Top,
                BaseTabControl = tabControl,
                Height = 48
            };
            mainContainer.Controls.Add(tabSelector);

            // Create a container panel for the content below the tab selector
            Panel contentPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = _darkBackground
            };
            mainContainer.Controls.Add(contentPanel);

            // Main layout panel (inside content panel)
            TableLayoutPanel layoutPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 2,
                Padding = new Padding(16),
                BackColor = _darkBackground,
                AutoSizeMode = AutoSizeMode.GrowAndShrink // Allow growing to fit content
            };
            layoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60F));
            layoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40F));
            layoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            layoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 50F));
            contentPanel.Controls.Add(layoutPanel);

            Panel picturePanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = _darkSecondary,
                Padding = new Padding(1),
                Margin = new Padding(0, 0, 8, 0)
            };
            
            PictureBox = new PictureBox
            {
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.None,
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.Transparent
            };
            picturePanel.Controls.Add(PictureBox);
            layoutPanel.Controls.Add(picturePanel, 0, 0);

            LoadImageButton = new MaterialButton
            {
                Text = "Load Image",
                Dock = DockStyle.Fill,
                Type = MaterialButton.MaterialButtonType.Contained,
                UseAccentColor = true,
                Margin = new Padding(0, 8, 8, 0),
                MinimumSize = new Size(100, 36) // Ensure minimum width
            };
            layoutPanel.Controls.Add(LoadImageButton, 0, 1);

            tabControl.Dock = DockStyle.Fill;
            tabControl.Margin = new Padding(0, 10, 0, 0);
            layoutPanel.Controls.Add(tabControl, 1, 0);
            layoutPanel.SetRowSpan(tabControl, 2);

            // Embed Tab
            TableLayoutPanel embedPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                Padding = new Padding(8, 16, 8, 8),
                BackColor = _darkBackground,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink // Allow growing to fit content
            };
            embedPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            embedPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 50F));
            embedPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F)); // Ensure column takes full width
            embedTab.Controls.Add(embedPanel);

            TextBox = new MaterialTextBox
            {
                Dock = DockStyle.Fill,
                Multiline = true,
                Hint = "Enter text to embed...",
                UseAccent = true,
                BackColor = _textBoxBackground,
                ForeColor = Color.White,
                UseTallSize = false,
                Padding = new Padding(8, 8, 8, 8),
                LeadingIcon = null,
                HideSelection = false,
                MaxLength = int.MaxValue
            };
            embedPanel.Controls.Add(TextBox, 0, 0);

            // Create the embed button directly in the layout cell
            EmbedTextButton = new MaterialButton
            {
                Text = "Embed Text",
                Dock = DockStyle.Fill,
                Type = MaterialButton.MaterialButtonType.Contained,
                UseAccentColor = true,
                Margin = new Padding(0),
                MinimumSize = new Size(100, 36) // Ensure minimum width
            };
            embedPanel.Controls.Add(EmbedTextButton, 0, 1);

            // Retrieve Tab
            TableLayoutPanel retrievePanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                Padding = new Padding(8, 16, 8, 8),
                BackColor = _darkBackground,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink // Allow growing to fit content
            };
            retrievePanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            retrievePanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 50F));
            retrievePanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F)); // Ensure column takes full width
            retrieveTab.Controls.Add(retrievePanel);

            RetrievedTextBox = new MaterialTextBox
            {
                Dock = DockStyle.Fill,
                Multiline = true,
                ReadOnly = true,
                Hint = "Retrieved text will appear here...",
                UseAccent = true,
                BackColor = Color.FromArgb(60, 60, 60),
                ForeColor = Color.White,
                LeadingIcon = null,
                HideSelection = false,
                MaxLength = int.MaxValue,
                UseTallSize = false,
                Padding = new Padding(8, 8, 8, 8)
            };
            retrievePanel.Controls.Add(RetrievedTextBox, 0, 0);

            // Create retrieve button directly in the layout cell
            RetrieveTextButton = new MaterialButton
            {
                Text = "Retrieve Text",
                Dock = DockStyle.Fill,
                Type = MaterialButton.MaterialButtonType.Contained,
                UseAccentColor = true,
                Margin = new Padding(0),
                MinimumSize = new Size(100, 36) // Ensure minimum width
            };
            retrievePanel.Controls.Add(RetrieveTextButton, 0, 1);

            // Settings Tab
            TableLayoutPanel settingsPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                Padding = new Padding(8, 16, 8, 8),
                BackColor = _darkBackground
            };
            settingsPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 60F));
            settingsPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            settingsTab.Controls.Add(settingsPanel);

            RegenerateKeysButton = new MaterialButton
            {
                Text = "Regenerate Keys",
                Dock = DockStyle.Fill,
                Type = MaterialButton.MaterialButtonType.Contained,
                UseAccentColor = true,
                Margin = new Padding(4),
                MinimumSize = new Size(150, 36) // Ensure minimum width
            };
            settingsPanel.Controls.Add(RegenerateKeysButton, 0, 0);
        }
    }
}