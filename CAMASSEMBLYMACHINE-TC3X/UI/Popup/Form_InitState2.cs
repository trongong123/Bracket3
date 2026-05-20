using NV_UI;
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
    public partial class Form_InitState2 : Form
    {
        public bool isExistLeft = false;
        public bool isExistRight = false;

        public string unitName = "";

        public enum STATE_INIT_ACTION_TYPE
        {
            EXIT,
            ALL,
            CLEAR,
            SET,
            LEFT,
            RIGHT
        }

        public enum unitLR
        {
            LEFT,
            RIGHT
        }

        public Form_InitState2(string unitName, bool existLeft, bool existRight)
        {
            InitializeComponent();
            nV_Button_TG_NS_All.ClickEvent += Function_ClickEvent;
            nV_Button_TG_NS_Clear.ClickEvent += Function_ClickEvent;
            nV_Button_TG_NS_Set.ClickEvent += Function_ClickEvent;
            nV_Button_PB_NS_Exit.ClickEvent += Function_ClickEvent;

            unitBoxWorkJigParts1.ClickEvent += Function_ClickEvent;
            unitBoxWorkJigParts2.ClickEvent += Function_ClickEvent;

            isExistLeft = existLeft;
            isExistRight = existRight;

            SetUnitImageExistent((int)unitLR.LEFT);
            SetUnitImageExistent((int)unitLR.RIGHT);

            unitBoxWorkJigParts1.TextName = unitName + "_" + "Left";
            unitBoxWorkJigParts2.TextName = unitName + "_" + "Right";
        }

        public DialogResult Show()
        {
            nV_Button_TG_NS_Set.Enabled = true;
            nV_Button_TG_NS_Clear.Enabled = true;

            return this.ShowDialog();
        }

        private void Function_ClickEvent(object sender, EventArgs e)
        {
            string sTag = (sender as Control).Tag.ToString();
            if(Enum.TryParse(sTag, out STATE_INIT_ACTION_TYPE btnFunction))
            {
                switch (btnFunction)
                {
                    case STATE_INIT_ACTION_TYPE.ALL:
                        isExistLeft = true;
                        isExistRight = true;
                        SetUnitImageExistent((int)unitLR.LEFT);
                        SetUnitImageExistent((int)unitLR.RIGHT);
                        break;
                    case STATE_INIT_ACTION_TYPE.CLEAR:
                        isExistLeft = false;
                        isExistRight = false;
                        SetUnitImageExistent((int)unitLR.LEFT);
                        SetUnitImageExistent((int)unitLR.RIGHT);
                        break;
                    case STATE_INIT_ACTION_TYPE.SET:
                        this.DialogResult = DialogResult.OK;
                        break;            
                    case STATE_INIT_ACTION_TYPE.EXIT:
                        this.Close();
                        break;
                    case STATE_INIT_ACTION_TYPE.LEFT:
                        isExistLeft = !isExistLeft;
                        SetUnitImageExistent((int)unitLR.LEFT);
                        break;
                    case STATE_INIT_ACTION_TYPE.RIGHT:
                        isExistRight = !isExistRight;
                        SetUnitImageExistent((int)unitLR.RIGHT);
                        break;
                }
            }   
        }

        private void SetUnitImageExistent(int isRight)
        {
            if (isRight == 1)
            {
                if (isExistRight)
                    unitBoxWorkJigParts2.Image = Properties.Resources.camera_selected;
                else
                    unitBoxWorkJigParts2.Image = null;
            }
            else
            {
                if (isExistLeft)
                    unitBoxWorkJigParts1.Image = Properties.Resources.camera_selected;
                else
                    unitBoxWorkJigParts1.Image = null;
            }
        }
    }
}
