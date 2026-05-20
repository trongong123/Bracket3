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
    public partial class SubForm_InputText : Form
    {
        public enum BTN_FUNCTION
        {
            OK,
            CANCEL
        }
        private string strInputText;
        public SubForm_InputText(string strTitle, string strInput = "")
        {
            InitializeComponent();
            nV_Button_PB_NS_Title.Text = strTitle;
            nV_Button_PB_NS3.ClickEvent += ButtonClickFunction;
            nV_Button_PB_NS4.ClickEvent += ButtonClickFunction;
            tbInput.Text = strInputText = strInput;
            this.KeyPreview = true;
        }

        private void ButtonClickFunction(object sender, EventArgs e)
        {
            string sTag = (sender as Control).Tag.ToString();
            Enum.TryParse(sTag, out BTN_FUNCTION function);

            switch (function)
            {
                case BTN_FUNCTION.OK:
                    this.DialogResult = DialogResult.OK;
                    this.strInputText = tbInput.Text;
                    this.Close();
                    break;
                case BTN_FUNCTION.CANCEL:
                    this.DialogResult = DialogResult.Cancel;
                    this.Close();
                    break;
            }
        }

        public string GetInputText() => this.strInputText;

        private void InputText_Load(object sender, EventArgs e)
        {
            CenterToScreen();
        }

        private void InputText_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                this.DialogResult = DialogResult.OK;
                this.strInputText = tbInput.Text;
                this.Close();
            }
        }
    }
}
