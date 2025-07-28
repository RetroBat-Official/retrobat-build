using System.Windows.Forms;
using System;

namespace CustomInstaller
{
    partial class InstallerForm
    {
        private TabControl tabControl1;
        private TabPage tabPageWelcome;
        private TabPage tabPageLicense;
        private TabPage tabPagePath;
        private TabPage tabPageInstall;
        private TextBox textBoxInstallPath;
        private ProgressBar progressBarInstall;
        private CheckBox checkBoxLaunch;
        private Button buttonNext;
        private Button buttonBack;
        private Button buttonInstall;
        private Button buttonBrowse;

        private void InitializeComponent()
        {
            this.tabControl1 = new TabControl();
            this.tabPageWelcome = new TabPage("Welcome");
            this.tabPageLicense = new TabPage("License Agreement");
            this.tabPagePath = new TabPage("Installation Path");
            this.tabPageInstall = new TabPage("Installing");

            this.textBoxInstallPath = new TextBox();
            this.progressBarInstall = new ProgressBar();
            this.checkBoxLaunch = new CheckBox();
            this.buttonNext = new Button();
            this.buttonBack = new Button();
            this.buttonInstall = new Button();
            this.buttonBrowse = new Button();

            this.buttonNext.Click += buttonNext_Click;
            this.buttonBack.Click += buttonBack_Click;

            // Form properties
            this.ClientSize = new System.Drawing.Size(600, 400);

            // TabControl setup
            this.tabControl1.Dock = DockStyle.Fill;
            this.tabControl1.TabPages.Add(this.tabPageWelcome);
            this.tabControl1.TabPages.Add(this.tabPageLicense);
            this.tabControl1.TabPages.Add(this.tabPagePath);
            this.tabControl1.TabPages.Add(this.tabPageInstall);

            // Add welcome label to Welcome tab
            Label welcomeLabel = new Label()
            {
                Text = "Welcome to RetroBat Installer!",
                AutoSize = true,
                Location = new System.Drawing.Point(20, 20),
                Font = new System.Drawing.Font("Segoe UI", 14, System.Drawing.FontStyle.Bold)
            };
            this.tabPageWelcome.Controls.Add(welcomeLabel);

            // Add controls to Path tab as example
            this.textBoxInstallPath.Location = new System.Drawing.Point(20, 20);
            this.textBoxInstallPath.Width = 400;
            this.buttonBrowse.Text = "Browse...";
            this.buttonBrowse.Location = new System.Drawing.Point(430, 18);

            this.tabPagePath.Controls.Add(this.textBoxInstallPath);
            this.tabPagePath.Controls.Add(this.buttonBrowse);

            // Add buttons at the bottom of the form
            this.buttonBack.Text = "Back";
            this.buttonBack.Location = new System.Drawing.Point(320, 360);
            this.buttonNext.Text = "Next";
            this.buttonNext.Location = new System.Drawing.Point(400, 360);
            this.buttonInstall.Text = "Install";
            this.buttonInstall.Location = new System.Drawing.Point(480, 360);

            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.buttonBack);
            this.Controls.Add(this.buttonNext);
            this.Controls.Add(this.buttonInstall);

            // Disable buttons as needed (for example, disable Install until last page)
            this.buttonInstall.Enabled = false;
            this.buttonBack.Enabled = false;

            // Event handlers can be hooked here (optional)
            // e.g., this.buttonBrowse.Click += ButtonBrowse_Click;

            // Other control properties and layout as needed
        }

        private void buttonNext_Click(object sender, EventArgs e)
        {
            if (tabControl1.SelectedIndex < tabControl1.TabCount - 1)
            {
                tabControl1.SelectedIndex++;
            }
            UpdateButtonStates();
        }

        private void buttonBack_Click(object sender, EventArgs e)
        {
            if (tabControl1.SelectedIndex > 0)
            {
                tabControl1.SelectedIndex--;
            }
            UpdateButtonStates();
        }

        private void UpdateButtonStates()
        {
            buttonBack.Enabled = tabControl1.SelectedIndex > 0;
            buttonNext.Enabled = tabControl1.SelectedIndex < tabControl1.TabCount - 1;
            buttonInstall.Enabled = tabControl1.SelectedIndex == tabControl1.TabCount - 1;
        }
    }
}
