using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CAMASSEMBLYMACHINE.UI
{
    public partial class SubForm_Warning : Form
    {
        public SubForm_Warning(string message, bool YesNo = false)
        {
            InitializeComponent();
            CenterToScreen();
            SetContents(message);

            if (YesNo)
            {
                button_Close.Visible = false;
            }
            else
            {
                button_Yes.Visible = false;
                button_No.Visible = false;
            }
        }

        /**
         * @brief 알람에 표시될 컨텐츠 구성하는 함수
         * @param[in] errCode 에러코드
         */
        private void SetContents(string message)
        {
            textBox1.Text = message;
        }

        private void button_Close_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            this.Close();
        }

        private void button_Yes_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Yes;
            Close();
        }

        private void button_No_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.No;
            Close();
        }
    }
}