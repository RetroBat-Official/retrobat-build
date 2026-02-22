using System;
using System.Windows.Forms;

namespace InstallerHost
{
    public partial class WelcomeControl : UserControl
    {
        private MainForm mainForm;

        private Label lblWelcomeTitle;
        private Label lblWelcomeDesc;
        private Button btnCancel;
        private Panel panel1;
        private HorizontalLineCtrl horizontalLineCtrl1;
        private PictureBox bannerPictureBox;
        private Button btnNext;

        public WelcomeControl(MainForm main)
        {
            mainForm = main;

            InitializeComponent();            

            //LayoutControls();
            lblWelcomeTitle.Text = Texts.GetString("Welcome");
            lblWelcomeDesc.Text = Texts.GetString("WelcomeText", BaseForm.branch, BaseForm.version);
            btnCancel.Text = Texts.GetString("Cancel");
            btnNext.Text = Texts.GetString("Next >");            
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            ActiveControl = btnNext;
        }

        private void InitializeComponent()
        {
            this.lblWelcomeTitle = new System.Windows.Forms.Label();
            this.lblWelcomeDesc = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnNext = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.bannerPictureBox = new System.Windows.Forms.PictureBox();
            this.horizontalLineCtrl1 = new InstallerHost.HorizontalLineCtrl();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bannerPictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // lblWelcomeTitle
            // 
            this.lblWelcomeTitle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblWelcomeTitle.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblWelcomeTitle.Location = new System.Drawing.Point(238, 13);
            this.lblWelcomeTitle.Name = "lblWelcomeTitle";
            this.lblWelcomeTitle.Size = new System.Drawing.Size(472, 59);
            this.lblWelcomeTitle.TabIndex = 0;
            this.lblWelcomeTitle.Text = "Welcome to the RetroBat installation program";
            // 
            // lblWelcomeDesc
            // 
            this.lblWelcomeDesc.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblWelcomeDesc.Location = new System.Drawing.Point(243, 80);
            this.lblWelcomeDesc.Name = "lblWelcomeDesc";
            this.lblWelcomeDesc.Size = new System.Drawing.Size(467, 322);
            this.lblWelcomeDesc.TabIndex = 1;
            this.lblWelcomeDesc.Text = "This wizard will guide you through the installation of RetroBat..";
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnCancel.Location = new System.Drawing.Point(638, 428);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 26);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.BtnCancel_Click);
            // 
            // btnNext
            // 
            this.btnNext.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnNext.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnNext.Location = new System.Drawing.Point(557, 428);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(75, 26);
            this.btnNext.TabIndex = 3;
            this.btnNext.Text = "Next";
            this.btnNext.Click += new System.EventHandler(this.BtnNext_Click);
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.Controls.Add(this.bannerPictureBox);
            this.panel1.Controls.Add(this.horizontalLineCtrl1);
            this.panel1.Controls.Add(this.lblWelcomeTitle);
            this.panel1.Controls.Add(this.lblWelcomeDesc);
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(728, 418);
            this.panel1.TabIndex = 4;
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
            // WelcomeControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnNext);
            this.Name = "WelcomeControl";
            this.Size = new System.Drawing.Size(728, 468);
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.bannerPictureBox)).EndInit();
            this.ResumeLayout(false);

        }

        private void BtnNext_Click(object sender, EventArgs e)
        {
            Logger.Log("Welcome screen, user clicked NEXT");
            mainForm.ShowLicense();
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show(Texts.GetString("CancelSure"), Texts.GetString("CancelButtonTitle"), MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
                Application.Exit();
        }
    }
}
