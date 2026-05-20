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

namespace CAMASSEMBLYMACHINE.UI
{
    public partial class SubForm_TenKey : Form
    {
        public static bool bAddValueMode = true;
        string[] returnValue;
        double nNowValue = 0;
        double nMinimumValue = 0;
        double nMaximumValue = 0;
        bool bValueDoubleType;

        public SubForm_TenKey(ref string[] pLabel, string pTitle, string pUnit, double pMinValue, double pMaxValue, int pLocX, int pLocY, bool pValFloat)
        {
            InitializeComponent();

            bAddValueMode = true;
            returnValue = pLabel;
            nMinimumValue = pMinValue;
            nMaximumValue = pMaxValue;
            this.Location = new Point(pLocX, pLocY);
            bValueDoubleType = pValFloat;
            if (pTitle == "") pTitle = "Tenkey Key Pad";

            label_KeypadTitle.Text = pTitle + " (" + nMinimumValue.ToString() + " ~ " + nMaximumValue.ToString() + ")";
            nNowValue = Convert.ToDouble(returnValue[0]);
            label_NowValue.Text = returnValue[0];
            label_NewValue.Text = "0";
            label_NewValueUnit.Text = pUnit;
            label_NowValueUnit.Text = pUnit;
            button_KeyPoint.Enabled = pValFloat;

            CenterToScreen();
        }

        private void button_MenuOk_Click(object sender, EventArgs e)
        {
            try
            {
                string sValue = label_NewValue.Text;
                double dValue = Convert.ToDouble(sValue);
                string sNowValue = label_NowValue.Text;
                double dNowValue = Convert.ToDouble(sNowValue);

                if (bAddValueMode == false)
                {
                    dValue = dNowValue + dValue;
                }

                if (nMinimumValue > dValue)
                {
                    sValue = nMinimumValue.ToString();
                    MessageBox.Show(this, "[" + Convert.ToString(dValue) + "]값이 최소값 [" + nMinimumValue.ToString() + "]값 보다 작아 최소값으로 변경되었습니다.", "최소~최대 입력 범위 오류", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else if (nMaximumValue < dValue)
                {
                    sValue = nMaximumValue.ToString();
                    MessageBox.Show(this, "[" + Convert.ToString(dValue) + "]값이 최대값 [" + nMaximumValue.ToString() + "]값 보다 커서 최대값으로 변경되었습니다.", "최소~최대 입력 범위 오류", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    if (bAddValueMode == false)
                    {
                        sValue = Convert.ToString(dValue);
                    }
                }

                if (sValue == "-")
                {
                    sValue = "0";
                }
                else if (sValue == "-.")
                {
                    sValue = "0";
                }
                else if (sValue == ".")
                {
                    sValue = "0";
                }
                else if (Convert.ToDouble(sValue) == 0)
                {
                    sValue = "0";
                }
                else
                {
                    if (sValue.Length > 1)
                    {
                        if (sValue.Substring(0, 2) == "-.")
                        {
                            sValue = sValue.Replace("-.", "-0.");
                        }
                        else if (sValue.Substring(0, 1) == ".")
                        {
                            sValue = sValue.Replace(".", "0.");
                        }
                    }
                }

                dValue = Convert.ToDouble(sValue);
                if (bValueDoubleType) sValue = dValue.ToString("0.000");
                returnValue[0] = sValue;
                System.Diagnostics.Process curProc = System.Diagnostics.Process.GetCurrentProcess();
                curProc.MaxWorkingSet = curProc.MaxWorkingSet;
                this.DialogResult = DialogResult.OK;
            }
            catch (System.Exception ex)
            {
                this.DialogResult = DialogResult.Cancel;
                MessageBox.Show(this, ex.ToString(), "vistaButton_Exit_Click", MessageBoxButtons.OK,
                    MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
            }

            this.Close();
        }

        private void button_MenuCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        public void KeyInputValue(string sKey)
        {
            string s = label_NewValue.Text.Trim();

            if (s == "0")
            {
                s = sKey;
                label_NewValue.Text = s;
                return;
            }

            if (s.Length > 0)
            {
                if ((s.Substring(0, 1) == "-") && (sKey == "-"))
                {
                    s = s.Substring(1);
                    label_NewValue.Text = s;
                    return;
                }
                if ((s.IndexOf(".", 0) > -1) && (sKey == "."))
                {
                    return;
                }
            }

            switch (sKey)
            {
                case "-":
                    s = sKey + s;
                    break;
                default:
                    s += sKey;
                    break;
            }

            label_NewValue.Text = s;
        }

        private void button_Key0_Click(object sender, EventArgs e)
        {
            KeyInputValue("0");
        }

        private void button_Key1_Click(object sender, EventArgs e)
        {
            KeyInputValue("1");
        }

        private void button_Key2_Click(object sender, EventArgs e)
        {
            KeyInputValue("2");
        }

        private void button_Key3_Click(object sender, EventArgs e)
        {
            KeyInputValue("3");
        }

        private void button_Key4_Click(object sender, EventArgs e)
        {
            KeyInputValue("4");
        }

        private void button_Key5_Click(object sender, EventArgs e)
        {
            KeyInputValue("5");
        }

        private void button_Key6_Click(object sender, EventArgs e)
        {
            KeyInputValue("6");
        }

        private void button_Key7_Click(object sender, EventArgs e)
        {
            KeyInputValue("7");
        }

        private void button_Key8_Click(object sender, EventArgs e)
        {
            KeyInputValue("8");
        }

        private void button_Key9_Click(object sender, EventArgs e)
        {
            KeyInputValue("9");
        }

        private void button_KeyPoint_Click(object sender, EventArgs e)
        {
            KeyInputValue(".");
        }

        private void button_KeyPM_Click(object sender, EventArgs e)
        {
            KeyInputValue("-");
        }

        private void button_KeyBackspace_Click(object sender, EventArgs e)
        {
            string s = label_NewValue.Text.Trim();

            if (s.Length == 0)
            {
                s = "";
            }
            else
            {
                s = s.Substring(0, s.Length - 1);
            }

            label_NewValue.Text = s;
        }

        private void button_KeyClear_Click(object sender, EventArgs e)
        {
            label_NewValue.Text = "0";
        }

        private void button_KeyPM_Add_Click(object sender, EventArgs e)
        {
            if (bAddValueMode)  // 기존 값에서 현재 설정 값 더하기
            {
                bAddValueMode = false;
                button_KeyPM_Add.BackColor = Color.Black;
                button_KeyPM_Add.ForeColor = Color.White;
            }
            else
            {
                bAddValueMode = true;
                button_KeyPM_Add.BackColor = Color.LightYellow;
                button_KeyPM_Add.ForeColor = Color.Black;
            }
        }

        private void SubForm_TenKey_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.D0 || e.KeyCode == Keys.NumPad0)
                button_Key0_Click(this, null);
            if (e.KeyCode == Keys.D1 || e.KeyCode == Keys.NumPad1)
          
                button_Key1_Click(this, null);
            if (e.KeyCode == Keys.D2 || e.KeyCode == Keys.NumPad2)
                button_Key2_Click(this, null);
            if (e.KeyCode == Keys.D3 || e.KeyCode == Keys.NumPad3)
                button_Key3_Click(this, null);
            if (e.KeyCode == Keys.D4 || e.KeyCode == Keys.NumPad4)
                button_Key4_Click(this, null);
            if (e.KeyCode == Keys.D5 || e.KeyCode == Keys.NumPad5)
                button_Key5_Click(this, null);
            if (e.KeyCode == Keys.D6 || e.KeyCode == Keys.NumPad6)
                button_Key6_Click(this, null);
            if (e.KeyCode == Keys.D7 || e.KeyCode == Keys.NumPad7)
                button_Key7_Click(this, null);
            if (e.KeyCode == Keys.D8 || e.KeyCode == Keys.NumPad8)
                button_Key8_Click(this, null);
            if (e.KeyCode == Keys.D9 || e.KeyCode == Keys.NumPad9)
                button_Key9_Click(this, null);
            if (e.KeyCode == Keys.Back)
                button_KeyBackspace_Click(this, null);
            if (e.KeyCode == Keys.OemPeriod)
                button_KeyPoint_Click(this, null);
        }
    }
}
