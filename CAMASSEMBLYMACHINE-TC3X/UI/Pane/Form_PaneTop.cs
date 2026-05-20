using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CAMASSEMBLYMACHINE.Define;
using CAMASSEMBLYMACHINE.UI.SubForm;

namespace CAMASSEMBLYMACHINE.UI
{
    public partial class Form_PaneTop : Form
    {
        string testStr = "";
        public Form_PaneTop()
        {
            InitializeComponent();
        }

        public string Department
        {
            get { return button_Department.Text; }
            set { button_Department.Text = value; }
        }

        public string MachineName
        {
            get { return button_MachineName.Text; }
            set { button_MachineName.Text = value; }
        }

        public string ModelName
        {
            get { return button_ModelName.Text; }
            set { button_ModelName.Text = value; }
        }

        public string Version
        {
            get { return button_Version.Text; }
            set { button_Version.Text = value; }
        }

        private void timer_DateTime_Tick(object sender, EventArgs e)
        {
            button_DateTime.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            if (button_ModelName.Text != (SystemDefine.modelTag + SystemDefine.modelname))
                button_ModelName.Text = SystemDefine.modelTag + SystemDefine.modelname;

            MachineName = SystemDefine.systemName + "-" + SystemDefine.productName;
        }
        private void InputText_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (testStr == "TOP")
                {
                    var form = new Form_ProcessView();
                    form.Show();
                }
                else if (testStr == "REG")
                {
                    MainForm.mainForm.ShowHiddenGroup();
                }
                testStr = "";
            }
            else
                testStr += e.KeyCode.ToString();
        }

        private void button_Department_Click(object sender, EventArgs e)
        {
            this.Focus();
        }
    }
}
