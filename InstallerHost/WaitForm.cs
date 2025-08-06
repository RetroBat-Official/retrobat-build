using System.Drawing;
using System.Windows.Forms;

namespace InstallerHost
{
    public class WaitForm : Form
    {
        private Label labelMessage;

        public WaitForm(string message)
        {
            InitializeComponent();
            labelMessage.Text = message;
            this.ControlBox = false;  // Disable close button
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.TopMost = true;
            this.ShowInTaskbar = false;
        }

        private void InitializeComponent()
        {
            this.labelMessage = new Label();
            this.SuspendLayout();
            // 
            // labelMessage
            // 
            this.labelMessage.Dock = DockStyle.Fill;
            this.labelMessage.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
            this.labelMessage.TextAlign = ContentAlignment.MiddleCenter;
            this.labelMessage.Padding = new Padding(10);
            this.labelMessage.AutoSize = false;
            // 
            // WaitForm
            // 
            this.ClientSize = new Size(300, 100);
            this.Controls.Add(this.labelMessage);
            this.Name = "WaitForm";
            this.Text = "Please wait...";
            this.ResumeLayout(false);
        }
    }
}
