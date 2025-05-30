using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;


namespace Personal_Expense_Tracker
{
    class RoundedButton : Button
    {
        protected override void OnPaint(PaintEventArgs pevent)
        {
            GraphicsPath gp = new GraphicsPath();
            gp.AddEllipse(0, 0, Width, Height);
            this.Region = new System.Drawing.Region(gp);
            base.OnPaint(pevent);


        }
    }
}
