using static CAMASSEMBLYMACHINE.Define.SystemDefine;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CAMASSEMBLYMACHINE.UI.SubForm
{
    public partial class SubForm_Login : Form
    {
        private USER_LEVEL userLevel;
        private System.Diagnostics.Process processKeyboard;

        public SubForm_Login(USER_LEVEL level)
        {
            userLevel = level;
            InitializeComponent();

            nV_Button_PB_NS3.ClickEvent += nV_Button_PB_NS3_Click;
        }

        ~SubForm_Login()
        {
            TerminateKeyBoard();
        }

        private void LoginUI_Load(object sender, EventArgs e)
        {
            CenterToScreen();
            this.ActiveControl = tbPassword;
            tbPassword.SelectAll();

            if (userLevel == USER_LEVEL.AUTH_ORIGIN) lbLoginID.Text = "Access Origin";
            else if (userLevel == USER_LEVEL.DEVELOPER) lbLoginID.Text = "Developer";
            else lbLoginID.Text = "Access Data";
        }

        public bool CheckVerification()
        {
            bool returnValue = false;
            if (userLevel == USER_LEVEL.AUTH_DATA)
            {
                if (passwordData == tbPassword.Text)
                    returnValue = true;
                if (passwordOrigin == tbPassword.Text)
                    returnValue = true;
            }

            if (userLevel == USER_LEVEL.AUTH_ORIGIN)
            {
                if (passwordOrigin == tbPassword.Text)
                    returnValue = true;
            }

            if (!returnValue)
            {
                MessageBox.Show("Invalid passowrd entered.\n\r  Please type valid password again.", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            else
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
                return true;
            }
        }

        private void tbPassword_MouseClick(object sender, MouseEventArgs e)
        {
            tbPassword.Text = "0";
            string[] value = { tbPassword.Text };
            SubForm_TenKey keyPad = new SubForm_TenKey(ref value, "Password", "", 0, 99999, 300, 300, false);
            keyPad.ShowDialog();
            tbPassword.Text = value[0];
            tbPassword.SelectionStart = tbPassword.Text.Length;
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

        private void tbPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                CheckVerification();
            }
        }

        private void PictureBox1_DoubleClick(object sender, EventArgs e)
        {
            OpenKeyboard();
        }

        private void nV_Button_PB_NS3_Click(object sender, EventArgs e)
        {
            CheckVerification();
        }
    }
}

