using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace InstallerHost
{
    public partial class BaseForm : Form
    {
        private SplitContainer splitContainer;
        private PictureBox bannerPictureBox;
        private Panel whiteBox;
        protected Panel contentPanel; // Make protected so derived forms can access it

        public int BannerWidth => 210; // banner width in pixels

        public int buttonWidth = 80;
        public int buttonHeight = 30;
        public int spacing = 10;
        public int bottomMargin = 20;
        public int rightMargin = 20;
        public static string version;
        public static string branch;

        public BaseForm()
        {
            InitializeComponent();
            string exeName = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string fileName = Path.GetFileNameWithoutExtension(exeName);
            string[] strings = fileName.Split('-');
            version = strings.Length > 1 ? strings[1] : "";
            branch = strings.Length > 2 ? strings[2] : "";

            this.Text = Texts.GetString("WindowsTitle") + branch + " " + version;
            InitializeLayout();
        }

        private void InitializeLayout()
        {
            this.Size = new Size(800, 450);
            this.StartPosition = FormStartPosition.CenterScreen;

            this.BackColor = Color.White;

            // Load banner image first so we know its height
            var bannerImage = LoadEmbeddedBitmap("InstallerHost.resources.retrobat_wizard.bmp");

            // Setup SplitContainer
            splitContainer = new SplitContainer
            {
                Dock = DockStyle.Fill,
                FixedPanel = FixedPanel.Panel1,
                IsSplitterFixed = true,
                SplitterWidth = 1,
                SplitterDistance = BannerWidth,
                Panel1MinSize = BannerWidth,
            };

            splitContainer.Panel1.BackColor = Color.White;
            splitContainer.Panel2.BackColor = Color.White;

            // Banner PictureBox fills Panel1
            bannerPictureBox = new PictureBox
            {
                Dock = DockStyle.Fill,
                Image = bannerImage,
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.Transparent
            };
            splitContainer.Panel1.Controls.Add(bannerPictureBox);

            // Create white panel inside Panel2 for content box
            Panel whiteContentPanel = new Panel
            {
                BackColor = Color.White,
                Width = splitContainer.Panel2.ClientSize.Width - 20, // horizontal margin
                Height = bannerImage.Height + 80,
                Left = 5,
                Anchor = AnchorStyles.Left | AnchorStyles.Right,
            };

            // Initially center whiteContentPanel vertically inside Panel2
            whiteContentPanel.Top = (splitContainer.Panel2.ClientSize.Height - whiteContentPanel.Height) / 2;

            // Create your actual contentPanel inside the white box, docked fill with padding
            contentPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20)
            };
            whiteContentPanel.Controls.Add(contentPanel);

            // Add the white content box to Panel2
            splitContainer.Panel2.Controls.Add(whiteContentPanel);

            // Add splitContainer to the form
            this.Controls.Add(splitContainer);

            // Update whiteContentPanel position and width on Panel2 resize to keep vertical centering and margins
            splitContainer.Panel2.Resize += (s, e) =>
            {
                whiteContentPanel.Top = (splitContainer.Panel2.ClientSize.Height - whiteContentPanel.Height) / 2;
                whiteContentPanel.Width = splitContainer.Panel2.ClientSize.Width - 20;
            };
        }

        private Bitmap LoadEmbeddedBitmap(string resourceName)
        {
            var asm = Assembly.GetExecutingAssembly();
            using (var stream = asm.GetManifestResourceStream(resourceName))
            {
                if (stream == null)
                    throw new Exception($"Resource '{resourceName}' not found in assembly.");
                return new Bitmap(stream);
            }
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BaseForm));
            this.SuspendLayout();
            // 
            // BaseForm
            // 
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "BaseForm";
            this.ResumeLayout(false);
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            splitContainer.SplitterDistance = BannerWidth; // enforce banner width after form fully shown
        }
    }
}
