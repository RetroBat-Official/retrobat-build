using System;
using System.Drawing;
using System.Windows.Forms;

namespace InstallerHost
{
    public partial class WelcomeControl : UserControl
    {
        private MainForm mainForm;

        private Label lblWelcomeTitle;
        private Label lblWelcomeDesc;
        private Button btnCancel;
        private Button btnNext;

        private int leftMargin = 20;
        private int bottomMargin;
        private int rightMargin;

        public WelcomeControl(MainForm main)
        {
            mainForm = main;
            bottomMargin = mainForm.bottomMargin;
            rightMargin = mainForm.rightMargin;

            InitializeComponent();

            this.Resize += WelcomeControl_Resize; // update layout on resize
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            lblWelcomeTitle = new Label()
            {
                Text = Texts.GetString("Welcome"),
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                AutoSize = false,
                Left = leftMargin,
                Top = 20,
                TextAlign = ContentAlignment.MiddleLeft,
                AutoEllipsis = true,
                MaximumSize = new Size(this.Width - 2 * leftMargin, 0),
            };

            lblWelcomeDesc = new Label()
            {
                Text = Texts.GetString("WelcomeText", BaseForm.branch, BaseForm.version),
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                AutoSize = false,
                Left = leftMargin,
                TextAlign = ContentAlignment.MiddleLeft,
                AutoEllipsis = true,
                MaximumSize = new Size(this.Width - 2 * leftMargin, 0),
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
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right,
            };
            btnNext.Click += (s, e) =>
            {
                Logger.Log("Welcome screen, user clicked NEXT");
                mainForm.ShowLicense();
            };

            this.Controls.Add(lblWelcomeTitle);
            this.Controls.Add(lblWelcomeDesc);
            this.Controls.Add(btnCancel);
            this.Controls.Add(btnNext);

            this.ResumeLayout(false);

            // Initial layout call to size and position controls properly
            LayoutControls();
        }

        private void WelcomeControl_Resize(object sender, EventArgs e)
        {
            LayoutControls();
        }

        private void LayoutControls()
        {
            int maxTextWidth = this.ClientSize.Width - 2 * leftMargin;

            // Update max widths and recalc heights
            lblWelcomeTitle.MaximumSize = new Size(maxTextWidth, 0);
            lblWelcomeTitle.Width = maxTextWidth;
            lblWelcomeTitle.Height = lblWelcomeTitle.PreferredHeight;

            lblWelcomeDesc.MaximumSize = new Size(maxTextWidth, 0);
            lblWelcomeDesc.Width = maxTextWidth;
            lblWelcomeDesc.Top = lblWelcomeTitle.Bottom + 10;
            lblWelcomeDesc.Height = lblWelcomeDesc.PreferredHeight;

            // Position buttons at bottom right with spacing
            btnCancel.Top = this.ClientSize.Height - btnCancel.Height - bottomMargin;
            btnCancel.Left = this.ClientSize.Width - btnCancel.Width - rightMargin;

            btnNext.Top = btnCancel.Top;
            btnNext.Left = btnCancel.Left - btnNext.Width - mainForm.spacing;
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show(Texts.GetString("CancelSure"), Texts.GetString("CancelButtonTitle"), MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                Application.Exit();
            }
        }
    }
}
