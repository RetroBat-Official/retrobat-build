using System;
using System.Windows.Forms;

namespace InstallerHost
{
    public partial class MainForm : BaseForm
    {
        private UserControl currentControl;

        public MainForm() : base()
        {
            this.DoubleBuffered = true;                        
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            ShowWelcome();          
        }

        private void ShowControl(UserControl control)
        {
            SuspendLayout();            

            if (currentControl != null)
            {
                Focus();
                Controls.Remove(currentControl);
                currentControl = null;
            }
           
            currentControl = control;
            currentControl.Dock = DockStyle.Fill;
            Controls.Add(currentControl);
            currentControl.BringToFront();
            currentControl.Focus();
                        
            ResumeLayout();

            currentControl.Invalidate();
        }

        private WelcomeControl _welcome;

        public void ShowWelcome()
        {
            if (_welcome == null)
                _welcome = new WelcomeControl(this);

            Logger.Log("Showing Welcome screen.");
            ShowControl(_welcome);
        }

        private LicenseControl _license;

        public void ShowLicense()
        {
            if (_license == null)
                _license = new LicenseControl(this);

            Logger.Log("Showing License screen.");
            ShowControl(_license);
        }

        private PrerequisiteControl _prerequisite;

        public void ShowPrerequisites(bool goForward)
        {
            if (_prerequisite == null)
                _prerequisite = new PrerequisiteControl(this);

            if (_prerequisite.SkipIfAllInstalled())
            {
                if (goForward)
                    ShowInstall();
                else
                    ShowLicense();
            }
            else
            {
                Logger.Log("Showing Prerequisites screen.");
                ShowControl(_prerequisite);
            }
        }

        private InstallControl _install;

        public void ShowInstall()
        {
            if (_install == null)
                _install = new InstallControl(this);

            Logger.Log("Showing Install screen.");
            ShowControl(_install);
        }

        public void ShowFinish(string installPath)
        {
            Logger.Log("Showing Finish screen.");
            ShowControl(new FinishControl(this, installPath));
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.SuspendLayout();
            // 
            // MainForm
            // 
            this.ClientSize = new System.Drawing.Size(606, 429);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainForm";
            this.ResumeLayout(false);

        }
    }
}
