using System.Windows.Forms;

namespace InstallerHost
{
    public partial class MainForm : BaseForm
    {
        private UserControl currentControl;

        public string version;
        public string branch;

        public MainForm() : base()
        {
            ShowWelcome(); // Show welcome screen on startup
        }

        private void ShowControl(UserControl control)
        {
            if (currentControl != null)
            {
                contentPanel.Controls.Remove(currentControl);
                currentControl.Dispose();
            }

            currentControl = control;
            currentControl.Dock = DockStyle.Fill;
            contentPanel.Controls.Add(currentControl);
            currentControl.BringToFront();
        }

        public void ShowWelcome()
        {
            var welcome = new WelcomeControl(this);
            Logger.Log("Showing Welcome screen.");
            ShowControl(welcome);
        }

        public void ShowLicense()
        {
            var license = new LicenseControl(this);
            Logger.Log("Showing License screen.");
            ShowControl(license);
        }

        public void ShowInstall()
        {
            var install = new InstallControl(this);
            Logger.Log("Showing Install screen.");
            ShowControl(install);
        }

        public void ShowFinish(string installPath)
        {
            var finish = new FinishControl(this, installPath);
            Logger.Log("Showing Finish screen.");
            ShowControl(finish);
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.SuspendLayout();
            // 
            // MainForm
            // 
            this.ClientSize = new System.Drawing.Size(584, 361);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainForm";
            this.ResumeLayout(false);

        }
    }
}
