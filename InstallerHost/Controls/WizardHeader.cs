using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Allegoria.Controls
{
    [ToolboxBitmap(typeof(Panel))]
    public partial class WizardPanel : UserControl
    {
        public WizardPanel()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            SetStyle(ControlStyles.ResizeRedraw, true);

            InitializeComponent();

            this.BackColor = SystemColors.Window;

            label1.Text = "";
        }

        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
            label1.Font = new Font(this.Font.FontFamily, Font.Size + 4.0f, FontStyle.Bold);
            Refresh();
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            base.OnPaintBackground(e);
           
            if (_image != null)
            {
                int margin = 8;

                var rc = ClientRectangle;
                rc.X = rc.Right - rc.Height;
                rc.Width = rc.Height;
                rc.Inflate(-margin, -margin);

                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBilinear;
                e.Graphics.DrawImage(_image, rc);
            }
         
            e.Graphics.DrawLine(SystemPens.ControlDark,
                ClientRectangle.Left,
                ClientRectangle.Bottom - 1,
                ClientRectangle.Right,
                ClientRectangle.Bottom - 1);
        }
       
        public override string Text
        {
            get
            {
                return label1.Text;
            }
            set
            {
                label1.Text = value;
            }
        }

        [DefaultValue("")]
        public string Title
        {
            get { return label1.Text; }
            set { label1.Text = value; Invalidate(); }
        }

        private Image _image;

        [DefaultValue(null)]
        public Image Image
        {
            get { return _image; }
            set
            {
                if (_image != value)
                {
                    _image = value;
                    Invalidate();
                }
            }
        }
    }
}
