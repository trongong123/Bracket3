using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static CAMASSEMBLYMACHINE.Define.SystemDefine;
using TopEng.Vision;

namespace CAMASSEMBLYMACHINE.UI
{
    public partial class Form_PaneAlignDisplay : Form
    {
        public int width = 200;
        public int height = 300;
        public Dictionary<int, int> focusIndex = new Dictionary<int, int>();
        public Dictionary<int, PMAlignResult> alignResult;

        public Form_PaneAlignDisplay(Dictionary<int, PMAlignResult> result)
        {
            InitializeComponent();
            alignResult = result;
        }

        public void UpdateObejctData()
        {

        }

        private void Form_PaneAlignDisplay_Paint(object sender, PaintEventArgs e)
        {
            Graphics graphic = e.Graphics;

            Pen normalPen = new Pen(Color.Black);
            SolidBrush focusBrush = new SolidBrush(Color.Green);

            double Ratio = (double)((double)Size.Width / (double)width) < (double)((double)Size.Height / (double)height) ? (double)((double)Size.Width / (double)width) : (double)((double)Size.Height / (double)height);

            foreach (var result in alignResult)
            {
                int posx = (int)(0.001 * result.Value.pos.x * Ratio + 0.5 * Size.Width);
                int posy = (int)(0.001 * result.Value.pos.y * Ratio + 0.5 * Size.Height);
                Rectangle rec = new Rectangle(posx, posy, 10, 10);

                if (focusIndex.ContainsKey(result.Key))
                {
                    graphic.FillRectangle(focusBrush, rec);
                }
                else
                    graphic.DrawRectangle(normalPen, rec); 
            }
        }

        private void Form_PaneAlignDisplay_Resize(object sender, EventArgs e)
        {
            Invalidate();
        }
    }
}
