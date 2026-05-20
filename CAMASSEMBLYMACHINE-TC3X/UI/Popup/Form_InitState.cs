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
    public partial class Form_InitState : Form
    {
        public enum STATE_INIT_ACTION_TYPE
        {
            EXIT,
            CLEAR,
            ADD
        }

        public Form_InitState()
        {
            InitializeComponent();
            nV_Button_TG_NS_Clear.ClickEvent += Function_ClickEvent;
            nV_Button_TG_NS_AddNew.ClickEvent += Function_ClickEvent;
            nV_Button_PB_NS_Exit.ClickEvent += Function_ClickEvent;
        }

        public DialogResult Show()
        {
            nV_Button_TG_NS_AddNew.Enabled = true;
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
                    case STATE_INIT_ACTION_TYPE.ADD:
                        this.DialogResult = DialogResult.OK;
                        break;
                    case STATE_INIT_ACTION_TYPE.CLEAR:
                        this.DialogResult = DialogResult.No;
                        break;
                    case STATE_INIT_ACTION_TYPE.EXIT:
                        break;
                }
            }   
            this.Close();
        }
    }
}
