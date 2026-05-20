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

namespace CAMASSEMBLYMACHINE.UI
{
    public partial class SubForm_StageManual : Form
    {
        public SubForm_StageManual()
        {
            InitializeComponent();
        }

        private void SubForm_StageManual_Load(object sender, EventArgs e)
        {
            CenterToScreen();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            UpdateData();
        }

        private void UpdateData()
        {
            //uint returnValue1 = 0;
            //uint returnValue2 = 0;
            //Machine.IO.GetIn((int)DI.DECO_JIG_ALIGN1_BWD, ref returnValue1);
            //Machine.IO.GetIn((int)DI.DECO_JIG_ALIGN2_BWD, ref returnValue2);
            //button_DecoOpen.ButtonPush = returnValue1 == 1 && returnValue2 == 1 ? true : false;
            //Machine.IO.GetIn((int)DI.SIDE_C_JIG_ALIGN1_BWD, ref returnValue1);
            //Machine.IO.GetIn((int)DI.SIDE_C_JIG_ALIGN2_BWD, ref returnValue2);
            //button_SideCOpen.ButtonPush = returnValue1 == 1 && returnValue2 == 1 ? true : false;
            //Machine.IO.GetIn((int)DI.SIDE_A_JIG_ALIGN1_BWD, ref returnValue1);
            //Machine.IO.GetIn((int)DI.SIDE_A_JIG_ALIGN2_BWD, ref returnValue2);
            //button_SideAOpen.ButtonPush = returnValue1 == 1 && returnValue2 == 1 ? true : false;
        }

        private void button_DecoOpen_Click(object sender, EventArgs e)
        {
            //Machine.IO.SetOut((int)DO.DECO_JIG_ALIGN1_FWD, 0);
            //Machine.IO.SetOut((int)DO.DECO_JIG_ALIGN1_BWD, 1);
            //Machine.IO.SetOut((int)DO.DECO_JIG_ALIGN2_FWD, 0);
            //Machine.IO.SetOut((int)DO.DECO_JIG_ALIGN2_BWD, 1);

            button_DecoOpen.ButtonPush = true;
        }

        private void button_SideCOpen_Click(object sender, EventArgs e)
        {
            //Machine.IO.SetOut((int)DO.SIDE_C_JIG_ALIGN1_FWD, 0);
            //Machine.IO.SetOut((int)DO.SIDE_C_JIG_ALIGN1_BWD, 1);
            //Machine.IO.SetOut((int)DO.SIDE_C_JIG_ALIGN2_FWD, 0);
            //Machine.IO.SetOut((int)DO.SIDE_C_JIG_ALIGN2_BWD, 1);

            button_SideCOpen.ButtonPush = true;
        }

        private void button_SideAOpen_Click(object sender, EventArgs e)
        {
            //Machine.IO.SetOut((int)DO.SIDE_A_JIG_ALIGN1_FWD, 0);
            //Machine.IO.SetOut((int)DO.SIDE_A_JIG_ALIGN1_BWD, 1);
            //Machine.IO.SetOut((int)DO.SIDE_A_JIG_ALIGN2_FWD, 0);
            //Machine.IO.SetOut((int)DO.SIDE_A_JIG_ALIGN2_BWD, 1);

            button_SideAOpen.ButtonPush = true;
        }

        private void button_Close_Click(object sender, EventArgs e)
        {
            //uint returnValue1 = 0;
            //uint returnValue2 = 0;
            //Machine.IO.GetIn((int)DI.DECO_JIG_ALIGN1_BWD, ref returnValue1);
            //Machine.IO.GetIn((int)DI.DECO_JIG_ALIGN2_BWD, ref returnValue2);
            //Machine.product[(int)UNIT.DECO].exist = returnValue1 == 1 && returnValue2 == 1 ? false : true;
            //Machine.IO.GetIn((int)DI.SIDE_C_JIG_ALIGN1_BWD, ref returnValue1);
            //Machine.IO.GetIn((int)DI.SIDE_C_JIG_ALIGN2_BWD, ref returnValue2);
            //Machine.product[(int)UNIT.SIDE_C].exist = returnValue1 == 1 && returnValue2 == 1 ? false : true;
            //Machine.IO.GetIn((int)DI.SIDE_A_JIG_ALIGN1_BWD, ref returnValue1);
            //Machine.IO.GetIn((int)DI.SIDE_A_JIG_ALIGN2_BWD, ref returnValue2);
            //Machine.product[(int)UNIT.SIDE_A].exist = returnValue1 == 1 && returnValue2 == 1 ? false : true;

            Close();
        }
    }
}
