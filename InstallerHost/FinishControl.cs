using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace InstallerHost
{
    public partial class FinishControl : UserControl
    {
        private MainForm mainForm;
        private string installPath;
        private CheckBox chkRunApp;
        private Button btnFinish;

        public FinishControl(MainForm main, string path)
        {
            mainForm = main;
            installPath = path;
            InitializeComponent();
            this.Resize += FinishControl_Resize;
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            Label lblMessage = new Label()
            {
                Text = Texts.GetString("InstallComplete"),
                Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold),
                AutoSize = true,
                Left = 20,
                Top = 20
            };

            chkRunApp = new CheckBox()
            {
                Text = Texts.GetString("RunRetroBat"),
                Left = 20,
                Top = lblMessage.Bottom + 20,
                AutoSize = true,
                Checked = true
            };

            btnFinish = new Button()
            {
                Text = Texts.GetString("Finish"),
                Width = mainForm.buttonWidth,
                Height = mainForm.buttonHeight,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right
            };
            btnFinish.Click += BtnFinish_Click;

            this.Controls.Add(lblMessage);
            this.Controls.Add(chkRunApp);
            this.Controls.Add(btnFinish);

            this.ResumeLayout(false);

            FinishControl_Resize(this, EventArgs.Empty);
        }

        private void FinishControl_Resize(object sender, EventArgs e)
        {
            btnFinish.Top = this.ClientSize.Height - btnFinish.Height - mainForm.bottomMargin;
            btnFinish.Left = this.ClientSize.Width - btnFinish.Width - mainForm.rightMargin;
        }

        private void BtnFinish_Click(object sender, EventArgs e)
        {
            if (chkRunApp.Checked)
            {
                string exePath = Path.Combine(installPath, "RetroBat.exe");

                if (File.Exists(exePath))
                {
                    try
                    {
                        Process.Start(exePath);
                        Logger.Log($"Launched installed app: {exePath}");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(Texts.GetString("LaunchFail") + ex.Message);
                        Logger.Log("Failed to launch application: " + ex.ToString());
                    }
                }
                else
                {
                    MessageBox.Show(Texts.GetString("ExeNotFound") + exePath);
                    Logger.Log("Executable not found: " + exePath);
                }
            }

            Application.Exit();
        }
    }
}
