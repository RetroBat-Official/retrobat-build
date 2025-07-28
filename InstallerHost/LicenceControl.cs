using System;
using System.Drawing;
using System.Windows.Forms;

namespace InstallerHost
{
    public partial class LicenseControl : UserControl
    {
        private MainForm mainForm;

        private TextBox licenseTextBox;
        private Label lblLicenseIntro;
        private CheckBox chkAgree;
        private Button btnCancel;
        private Button btnNext;
        private Button btnBack;

        public LicenseControl(MainForm main)
        {
            mainForm = main;
            InitializeComponent();

            this.Resize += LicenseControl_Resize;
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            int leftMargin = 20;

            lblLicenseIntro = new Label()
            {
                Text = Texts.GetString("LicenseIntro"),
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                AutoSize = true,
                Left = leftMargin,
                Top = 20,
                Anchor = AnchorStyles.Top | AnchorStyles.Left,
            };

            this.Controls.Add(lblLicenseIntro);
            this.PerformLayout();

            licenseTextBox = new TextBox()
            {
                Multiline = true,
                WordWrap = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Vertical,
                Text = Texts.GetString("LicenseText"),
                Left = leftMargin,
                Top = lblLicenseIntro.Bottom + 10,
                Width = this.ClientSize.Width - 2 * leftMargin,
                Height = 150,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom
            };

            chkAgree = new CheckBox()
            {
                Text = Texts.GetString("AgreeText"),
                Left = leftMargin,
                Top = licenseTextBox.Bottom + 10,
                Width = this.ClientSize.Width - 2 * leftMargin,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            btnCancel = new Button()
            {
                Text = Texts.GetString("Cancel"),
                Width = mainForm.buttonWidth,
                Height = mainForm.buttonHeight,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right,
            };
            btnCancel.Click += BtnCancel_Click;

            btnNext = new Button()
            {
                Text = Texts.GetString("Next"),
                Width = mainForm.buttonWidth,
                Height = mainForm.buttonHeight,
                Enabled = false,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right,
            };

            btnBack = new Button()
            {
                Text = Texts.GetString("Back"),
                Width = mainForm.buttonWidth,
                Height = mainForm.buttonHeight,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right,
            };

            chkAgree.CheckedChanged += (s, e) =>
            {
                Logger.Log("Licence screen, licence accepted: " + chkAgree.Checked);
                btnNext.Enabled = chkAgree.Checked;
            };

            btnNext.Click += (s, e) =>
            {
                Logger.Log("Licence screen, user clicked NEXT");
                mainForm.ShowInstall();
            };

            btnBack.Click += (s, e) =>
            {
                Logger.Log("Licence screen, user clicked BACK");
                mainForm.ShowWelcome();
            };

            this.Controls.Add(licenseTextBox);
            this.Controls.Add(chkAgree);
            this.Controls.Add(btnCancel);
            this.Controls.Add(btnNext);
            this.Controls.Add(btnBack);

            this.ResumeLayout(false);
        }

        private void LicenseControl_Resize(object sender, EventArgs e)
        {
            // Prevent layout from collapsing on very small screens
            if (this.ClientSize.Height < 300 || this.ClientSize.Width < 400)
                return;

            // Resize licenseTextBox
            licenseTextBox.Width = this.ClientSize.Width - 40;

            int spaceBelowTextBox = this.ClientSize.Height - licenseTextBox.Top;
            int buttonAreaHeight = btnCancel.Height + mainForm.bottomMargin + 20 + chkAgree.Height;
            int desiredHeight = spaceBelowTextBox - buttonAreaHeight;

            licenseTextBox.Height = desiredHeight > 100 ? desiredHeight : 100;

            // Reposition checkbox and match width
            chkAgree.Top = licenseTextBox.Bottom + 10;
            chkAgree.Width = this.ClientSize.Width - 2 * mainForm.rightMargin;

            // Position buttons
            btnCancel.Top = this.ClientSize.Height - btnCancel.Height - mainForm.bottomMargin;
            btnCancel.Left = this.ClientSize.Width - btnCancel.Width - mainForm.rightMargin;

            btnNext.Top = btnCancel.Top;
            btnNext.Left = btnCancel.Left - btnNext.Width - mainForm.spacing;

            btnBack.Top = btnCancel.Top;
            btnBack.Left = btnNext.Left - btnBack.Width - mainForm.spacing;
        }

        public void BtnCancel_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show(Texts.GetString("CancelSure"), Texts.GetString("CancelButtonTitle"), MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                Application.Exit();
            }
        }
    }
}
