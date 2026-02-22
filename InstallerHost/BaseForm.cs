using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace InstallerHost
{
    public partial class BaseForm : Form
    {
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

            if (DesignMode)
                return;

            this.Font = SystemFonts.MessageBoxFont;

            string exeName = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string fileName = Path.GetFileNameWithoutExtension(exeName);
            string[] strings = fileName.Split('-');
            version = strings.Length > 1 ? strings[1] : "";
            branch = strings.Length > 2 ? strings[2] : "";

            this.Text = Texts.GetString("WindowsTitle") + branch + " " + version;            
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BaseForm));
            this.SuspendLayout();
            // 
            // BaseForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(606, 429);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "BaseForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.ResumeLayout(false);

        }
    }
}
