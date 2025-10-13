using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Drawing;
using System.Windows.Forms;

namespace InstallerHost
{
    public partial class FinishControl : UserControl
    {
        private MainForm mainForm;
        private string installPath;
        private CheckBox chkRunApp;
        private Button btnFinish;
        private FlowLayoutPanel linkPanel;

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
                Top = lblMessage.Bottom + 30,
                AutoSize = true,
                Checked = false
            };

            linkPanel = new FlowLayoutPanel()
            {
                Left = 20,
                Top = chkRunApp.Bottom + 80,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                WrapContents = false
            };

            AddLink("Forum", "https://forum.retrobat.org/", Properties.Resources.forum);
            AddLink("Discord", "https://discord.gg/retrobat", Properties.Resources.discord);
            AddLink("Website", "https://www.retrobat.org/", Properties.Resources.website);
            AddLink("Wiki", "https://wiki.retrobat.org/", Properties.Resources.wiki);

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
            this.Controls.Add(linkPanel);
            this.Controls.Add(btnFinish);

            this.ResumeLayout(false);
            FinishControl_Resize(this, EventArgs.Empty);
        }

        private void FinishControl_Resize(object sender, EventArgs e)
        {
            btnFinish.Top = this.ClientSize.Height - btnFinish.Height - mainForm.bottomMargin;
            btnFinish.Left = this.ClientSize.Width - btnFinish.Width - mainForm.rightMargin;
        }

        void AddLink(string text, string url, System.Drawing.Image icon)
        {
            PictureBox iconBox = new PictureBox()
            {
                Image = icon,
                SizeMode = PictureBoxSizeMode.Zoom,
                Width = 24,
                Height = 24,
                Margin = new Padding(0, 2, 4, 0)
            };

            LinkLabel link = new LinkLabel()
            {
                Text = text,
                Tag = url,
                AutoSize = true,
                LinkColor = System.Drawing.Color.RoyalBlue,
                ActiveLinkColor = System.Drawing.Color.DodgerBlue,
                VisitedLinkColor = System.Drawing.Color.Purple,
                Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular)
            };
            link.LinkClicked += LinkLabel_LinkClicked;

            FlowLayoutPanel pairPanel = new FlowLayoutPanel()
            {
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                FlowDirection = FlowDirection.LeftToRight,
                Margin = new Padding(0, 0, 15, 0)
            };

            pairPanel.Controls.Add(iconBox);
            pairPanel.Controls.Add(link);

            linkPanel.Controls.Add(pairPanel);
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

        private void LinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (sender is LinkLabel link && link.Tag is string url)
            {
                try
                {
                    Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Unable to open link: " + ex.Message);
                }
            }
        }
    }
}
