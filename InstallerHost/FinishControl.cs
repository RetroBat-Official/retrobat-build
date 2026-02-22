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
        private Panel panel1;
        private PictureBox bannerPictureBox;
        private HorizontalLineCtrl horizontalLineCtrl1;
        private Label lblMessage;
        private Button btnCancel;
        private Button btnBack;
        private Label lblWelcomeDesc;
        private FlowLayoutPanel linkPanel;

        public FinishControl(MainForm main, string path)
        {
            mainForm = main;
            installPath = path;
            InitializeComponent();

            lblMessage.Text = Texts.GetString("InstallComplete");
            chkRunApp.Text = Texts.GetString("RunRetroBat");            
            btnFinish.Text = Texts.GetString("Finish");
            lblWelcomeDesc.Text = Texts.GetString("InstallCompleteDescription");


            AddLink("Forum", "https://forum.retrobat.org/", Properties.Resources.forum);
            AddLink("Discord", "https://discord.gg/retrobat", Properties.Resources.discord);
            AddLink("Website", "https://www.retrobat.org/", Properties.Resources.website);
            AddLink("Wiki", "https://wiki.retrobat.org/", Properties.Resources.wiki);
        }

        private void InitializeComponent()
        {
            this.chkRunApp = new System.Windows.Forms.CheckBox();
            this.linkPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.btnFinish = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.lblWelcomeDesc = new System.Windows.Forms.Label();
            this.bannerPictureBox = new System.Windows.Forms.PictureBox();
            this.horizontalLineCtrl1 = new InstallerHost.HorizontalLineCtrl();
            this.lblMessage = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnBack = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bannerPictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // chkRunApp
            // 
            this.chkRunApp.Location = new System.Drawing.Point(246, 154);
            this.chkRunApp.Name = "chkRunApp";
            this.chkRunApp.Size = new System.Drawing.Size(406, 24);
            this.chkRunApp.TabIndex = 1;
            this.chkRunApp.Text = "Start retrobat.exe";
            // 
            // linkPanel
            // 
            this.linkPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.linkPanel.Location = new System.Drawing.Point(243, 378);
            this.linkPanel.Name = "linkPanel";
            this.linkPanel.Size = new System.Drawing.Size(467, 30);
            this.linkPanel.TabIndex = 2;
            // 
            // btnFinish
            // 
            this.btnFinish.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnFinish.Location = new System.Drawing.Point(557, 428);
            this.btnFinish.Name = "btnFinish";
            this.btnFinish.Size = new System.Drawing.Size(75, 26);
            this.btnFinish.TabIndex = 3;
            this.btnFinish.Text = "Finish";
            this.btnFinish.Click += new System.EventHandler(this.BtnFinish_Click);
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.Controls.Add(this.lblWelcomeDesc);
            this.panel1.Controls.Add(this.bannerPictureBox);
            this.panel1.Controls.Add(this.linkPanel);
            this.panel1.Controls.Add(this.chkRunApp);
            this.panel1.Controls.Add(this.horizontalLineCtrl1);
            this.panel1.Controls.Add(this.lblMessage);
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(728, 418);
            this.panel1.TabIndex = 5;
            // 
            // lblWelcomeDesc
            // 
            this.lblWelcomeDesc.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblWelcomeDesc.Location = new System.Drawing.Point(243, 79);
            this.lblWelcomeDesc.Name = "lblWelcomeDesc";
            this.lblWelcomeDesc.Size = new System.Drawing.Size(467, 71);
            this.lblWelcomeDesc.TabIndex = 6;
            this.lblWelcomeDesc.Text = "Retrobat has been installer to your computer.\r\n\r\nPress finish to close this wizar" +
    "d.";
            // 
            // bannerPictureBox
            // 
            this.bannerPictureBox.BackColor = System.Drawing.Color.Transparent;
            this.bannerPictureBox.Dock = System.Windows.Forms.DockStyle.Left;
            this.bannerPictureBox.Image = global::InstallerHost.Properties.Resources.retrobat_wizard;
            this.bannerPictureBox.Location = new System.Drawing.Point(0, 0);
            this.bannerPictureBox.Name = "bannerPictureBox";
            this.bannerPictureBox.Size = new System.Drawing.Size(224, 416);
            this.bannerPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.bannerPictureBox.TabIndex = 5;
            this.bannerPictureBox.TabStop = false;
            // 
            // horizontalLineCtrl1
            // 
            this.horizontalLineCtrl1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.horizontalLineCtrl1.Location = new System.Drawing.Point(0, 416);
            this.horizontalLineCtrl1.Name = "horizontalLineCtrl1";
            this.horizontalLineCtrl1.Size = new System.Drawing.Size(728, 2);
            this.horizontalLineCtrl1.TabIndex = 5;
            this.horizontalLineCtrl1.Text = "horizontalLineCtrl1";
            // 
            // lblMessage
            // 
            this.lblMessage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblMessage.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMessage.Location = new System.Drawing.Point(238, 13);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new System.Drawing.Size(472, 60);
            this.lblMessage.TabIndex = 0;
            this.lblMessage.Text = "Installation complete";
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Enabled = false;
            this.btnCancel.Location = new System.Drawing.Point(638, 428);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 26);
            this.btnCancel.TabIndex = 10;
            this.btnCancel.Text = "Cancel";
            // 
            // btnBack
            // 
            this.btnBack.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBack.Enabled = false;
            this.btnBack.Location = new System.Drawing.Point(476, 428);
            this.btnBack.Name = "btnBack";
            this.btnBack.Size = new System.Drawing.Size(75, 26);
            this.btnBack.TabIndex = 11;
            this.btnBack.Text = "< Back";
            // 
            // FinishControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnBack);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.btnFinish);
            this.Name = "FinishControl";
            this.Size = new System.Drawing.Size(728, 468);
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.bannerPictureBox)).EndInit();
            this.ResumeLayout(false);

        }
      
        void AddLink(string text, string url, System.Drawing.Image icon)
        {
            var iconBox = new PictureBox()
            {
                Image = icon,
                SizeMode = PictureBoxSizeMode.Zoom,
                Width = 16,
                Height = 16,
                Margin = new Padding(0, 2, 4, 0)
            };

            var link = new LinkLabel()
            {
                Text = text,
                Tag = url,
                AutoSize = true,
                LinkColor = System.Drawing.Color.RoyalBlue,
                ActiveLinkColor = System.Drawing.Color.DodgerBlue,
                VisitedLinkColor = System.Drawing.Color.Purple,
                Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular),                
            };
            link.LinkClicked += LinkLabel_LinkClicked;            

            var pairPanel = new FlowLayoutPanel()
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
