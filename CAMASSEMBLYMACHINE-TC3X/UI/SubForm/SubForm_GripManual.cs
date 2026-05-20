using static CAMASSEMBLYMACHINE.Define.SystemDefine;
using CAMASSEMBLYMACHINE.Define;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace CAMASSEMBLYMACHINE.UI
{
    public partial class SubForm_GripManual : Form
    {
        public SubForm_GripManual()
        {
            InitializeComponent();
        }

        private void SubForm_GripManual_Load(object sender, EventArgs e)
        {
            CenterToScreen();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            UpdateDate();
        }

        private void UpdateDate()
        {
            uint returnValue = 0;
            //Machine.IO.GetIn((int)DI.GRIP4_OPEN, ref returnValue);
            button_Grip4Open.ButtonPush = returnValue == 1 ? true : false;
        }

        private void button_Grip1Open_Click(object sender, EventArgs e)
        {

            button_Grip1Open.ButtonPush = true;
        }

        private void button_Grip2Open_Click(object sender, EventArgs e)
        {

            button_Grip2Open.ButtonPush = true;
        }

        private void button_Grip3Open_Click(object sender, EventArgs e)
        {
            //Machine.IO.SetOut((int)DO.GRIP3_OPEN, 1);
            //Machine.IO.SetOut((int)DO.GRIP3_CLOSE, 0);

            button_Grip3Open.ButtonPush = true;
        }

        private void button_Grip4Open_Click(object sender, EventArgs e)
        {
            //Machine.IO.SetOut((int)DO.GRIP4_OPEN, 1);
            //Machine.IO.SetOut((int)DO.GRIP4_CLOSE, 0);

            button_Grip4Open.ButtonPush = true;
        }

        private void button_Close_Click(object sender, EventArgs e)
        {
            //uint returnValue = 0;
            //Machine.IO.GetIn((int)DI.GRIP1_OPEN, ref returnValue);
            //Machine.product[(int)UNIT.GRIPPER1].exist = returnValue == 1 ? false : true;
            //Machine.IO.GetIn((int)DI.GRIP2_OPEN, ref returnValue);
            //Machine.product[(int)UNIT.GRIPPER2].exist = returnValue == 1 ? false : true;
            //Machine.IO.GetIn((int)DI.GRIP3_OPEN, ref returnValue);
            //Machine.product[(int)UNIT.GRIPPER3].exist = returnValue == 1 ? false : true;
            //Machine.IO.GetIn((int)DI.GRIP4_OPEN, ref returnValue);
            //Machine.product[(int)UNIT.GRIPPER4].exist = returnValue == 1 ? false : true;

            Close();
        }
    }
}
