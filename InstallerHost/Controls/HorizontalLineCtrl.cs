using System.Drawing;
using System.Windows.Forms;

namespace InstallerHost
{
    [ToolboxBitmap(typeof(LinkLabel))]
    public class HorizontalLineCtrl : Control
    {
        public HorizontalLineCtrl()
        {
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            g.DrawLine(SystemPens.ControlDark,
                ClientRectangle.Left, ClientRectangle.Top + ClientRectangle.Height / 2,
                ClientRectangle.Right, ClientRectangle.Top + ClientRectangle.Height / 2);

            g.DrawLine(SystemPens.ControlLightLight,
                ClientRectangle.Left, ClientRectangle.Top + ClientRectangle.Height / 2 + 1,
                ClientRectangle.Right, ClientRectangle.Top + ClientRectangle.Height / 2 + 1);
        }
    }
}
