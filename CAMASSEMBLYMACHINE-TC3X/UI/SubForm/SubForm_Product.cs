using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace CAMASSEMBLYMACHINE.UI.SubForm
{
    public partial class SubForm_Product : Form
    {
        private System.Diagnostics.Process processKeyboard;

        public SubForm_Product()
        {
            InitializeComponent();
        }

        private void SubForm_Product_Load(object sender, EventArgs e)
        {
            CenterToScreen();
            comboBox_Judge.SelectedIndex = 1;
        }

        public void GetInfo(ref string IMEI, ref string Model, ref string ColorCode, ref int Judge)
        {
            IMEI = textBox_IMEI.Text;
            Model = textBox_Model.Text;
            ColorCode = textBox_ColorCode.Text;
            Judge = comboBox_Judge.SelectedIndex;
        }

        private void OpenKeyboard()
        {
            try
            {
                ProcessStartInfo procInfo = new ProcessStartInfo();
                procInfo.UseShellExecute = true;
                procInfo.FileName = "osk.exe";
                procInfo.WorkingDirectory = "C:\\Windows\\System32";
                procInfo.Verb = "runas";
                processKeyboard = System.Diagnostics.Process.Start(procInfo);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            return;
        }

        public bool IsRunningKeyBoard()
        {
            try
            {
                if (processKeyboard != null)
                    System.Diagnostics.Process.GetProcessById(processKeyboard.Id);
                else
                    return false;
            }
            catch (InvalidOperationException)
            {
                return false;
            }
            catch (ArgumentException)
            {
                return false;
            }
            return true;
        }

        public void TerminateKeyBoard()
        {
            if (IsRunningKeyBoard())
            {
                processKeyboard.Kill();
            }
        }

        private void button_Ok_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            this.Close();
        }

        private void button_Cancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void textBox_IMEI_DoubleClick(object sender, EventArgs e)
        {
            OpenKeyboard();
        }

        private void textBox_Model_DoubleClick(object sender, EventArgs e)
        {
            OpenKeyboard();
        }

        private void textBox_ColorCode_DoubleClick(object sender, EventArgs e)
        {
            OpenKeyboard();
        }
    }
}
