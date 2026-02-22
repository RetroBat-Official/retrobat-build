using System;
using System.Drawing;
using System.Windows.Forms;

namespace InstallerHost
{
    public partial class LicenseControl : UserControl
    {
        private MainForm mainForm;

        private TextBox licenseTextBox;
        private CheckBox chkAgree;
        private Button btnCancel;
        private Button btnNext;
        private Allegoria.Controls.WizardPanel wizardHeader;
        private HorizontalLineCtrl horizontalLineCtrl1;
        private Button btnBack;

        public LicenseControl(MainForm main)
        {
            mainForm = main;
            InitializeComponent();

            wizardHeader.Text = Texts.GetString("LicenseIntro");                
            licenseTextBox.Text = Texts.GetString("LicenseText");
            chkAgree.Text = Texts.GetString("AgreeText");
            btnCancel.Text = Texts.GetString("Cancel");
            btnNext.Text = Texts.GetString("Next >");
            btnBack.Text = Texts.GetString("< Back");            
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            licenseTextBox.Select(0, 0);
            ActiveControl = licenseTextBox;
        }

        private void InitializeComponent()
        {
            this.licenseTextBox = new System.Windows.Forms.TextBox();
            this.chkAgree = new System.Windows.Forms.CheckBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnNext = new System.Windows.Forms.Button();
            this.btnBack = new System.Windows.Forms.Button();
            this.horizontalLineCtrl1 = new InstallerHost.HorizontalLineCtrl();
            this.wizardHeader = new Allegoria.Controls.WizardPanel();
            this.SuspendLayout();
            // 
            // licenseTextBox
            // 
            this.licenseTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.licenseTextBox.BackColor = System.Drawing.SystemColors.Window;
            this.licenseTextBox.Location = new System.Drawing.Point(22, 79);
            this.licenseTextBox.Multiline = true;
            this.licenseTextBox.Name = "licenseTextBox";
            this.licenseTextBox.ReadOnly = true;
            this.licenseTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.licenseTextBox.Size = new System.Drawing.Size(511, 307);
            this.licenseTextBox.TabIndex = 1;
            // 
            // chkAgree
            // 
            this.chkAgree.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.chkAgree.Location = new System.Drawing.Point(22, 396);
            this.chkAgree.Name = "chkAgree";
            this.chkAgree.Size = new System.Drawing.Size(507, 24);
            this.chkAgree.TabIndex = 2;
            this.chkAgree.Text = "I accept the terms of the license agreement";
            this.chkAgree.CheckedChanged += new System.EventHandler(this.chkAgree_CheckedChanged);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnCancel.Location = new System.Drawing.Point(458, 439);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 26);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.BtnCancel_Click);
            // 
            // btnNext
            // 
            this.btnNext.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnNext.Enabled = false;
            this.btnNext.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnNext.Location = new System.Drawing.Point(377, 439);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(75, 26);
            this.btnNext.TabIndex = 4;
            this.btnNext.Text = "Next";
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
            // 
            // btnBack
            // 
            this.btnBack.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBack.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnBack.Location = new System.Drawing.Point(296, 439);
            this.btnBack.Name = "btnBack";
            this.btnBack.Size = new System.Drawing.Size(75, 26);
            this.btnBack.TabIndex = 5;
            this.btnBack.Text = "Back";
            this.btnBack.Click += new System.EventHandler(this.btnBack_Click);
            // 
            // horizontalLineCtrl1
            // 
            this.horizontalLineCtrl1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.horizontalLineCtrl1.Location = new System.Drawing.Point(0, 427);
            this.horizontalLineCtrl1.Name = "horizontalLineCtrl1";
            this.horizontalLineCtrl1.Size = new System.Drawing.Size(548, 2);
            this.horizontalLineCtrl1.TabIndex = 7;
            this.horizontalLineCtrl1.Text = "horizontalLineCtrl1";
            // 
            // wizardHeader
            // 
            this.wizardHeader.BackColor = System.Drawing.SystemColors.Window;
            this.wizardHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.wizardHeader.Image = global::InstallerHost.Properties.Resources.logo_icon;
            this.wizardHeader.Location = new System.Drawing.Point(0, 0);
            this.wizardHeader.Name = "wizardHeader";
            this.wizardHeader.Size = new System.Drawing.Size(548, 60);
            this.wizardHeader.TabIndex = 6;
            this.wizardHeader.Title = "Licence Agreement";
            // 
            // LicenseControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.horizontalLineCtrl1);
            this.Controls.Add(this.wizardHeader);
            this.Controls.Add(this.licenseTextBox);
            this.Controls.Add(this.chkAgree);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnNext);
            this.Controls.Add(this.btnBack);
            this.Name = "LicenseControl";
            this.Size = new System.Drawing.Size(548, 479);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void chkAgree_CheckedChanged(object sender, EventArgs e)
        {
            Logger.Log("Licence screen, licence accepted: " + chkAgree.Checked);
            btnNext.Enabled = chkAgree.Checked;
            ActiveControl = btnNext;
        }

        public void BtnCancel_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show(Texts.GetString("CancelSure"), Texts.GetString("CancelButtonTitle"), MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
                Application.Exit();
        }
        
        private void btnBack_Click(object sender, EventArgs e)
        {
            Logger.Log("Licence screen, user clicked BACK");
            mainForm.ShowWelcome();
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            Logger.Log("Licence screen, user clicked NEXT");
            mainForm.ShowPrerequisites(true);
        }

    }
}
