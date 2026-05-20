using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TopEng.Controls
{
    public partial class TeachingInputValue : UserControl
    {
        private bool bClicked = false;
        private bool bClickEventOff = false;
        public delegate void CheckStateChangedEvent(object sender, EventArgs e);
        public event CheckStateChangedEvent btnEnableCallbackEvent;
        public delegate void ClickEvent(object sender, EventArgs e);
        public event ClickEvent clickEvent;
        private bool bVisibleCurrentPos = true;
        private bool bVisibleTargetPos = true;

        /// <summary>
        /// 클릭 상태 값 입니다
        /// </summary>
        [Category("User 속성")]
        [Description("클릭 상태 값 입니다")]
        public bool Clicked
        {
            get { return bClicked; }
            set
            {
                bClicked = value;
                checkBox1.Checked = bClicked;
            }
        }
        /// <summary>
        /// 축 이름을 설정합니다
        /// </summary>
        [Category("User 속성")]
        [Description("축 이름을 설정합니다")]
        public string AxisText
        {
            get { return labelAxis.Text; }
            set { labelAxis.Text = value; }
        }
        /// <summary>
        /// 타겟 값을 설정합니다
        /// </summary>
        [Category("User 속성")]
        [DefaultValue(0D)]
        [Description("타겟 값을 설정합니다")]
        public double TargetPos
        {
            get { return Convert.ToDouble(textBoxTargetPos.Text); }
            set 
            {
                if (value == 0)
                    textBoxTargetPos.Text = value.ToString();
                else
                    textBoxTargetPos.Text = string.Format("{0:0.000}", value);
            }
        }
        /// <summary>
        /// 타겟 값의 배경 색을 바꿉니다.
        /// </summary>
        [Category("User 속성")]
        [Description("현재 값의 배경색을 설정합니다")]
        public Color CurrentPosBackColor
        {
            get { return textBoxCurrentPos.BackColor; }
            set
            {
                textBoxCurrentPos.BackColor = value;
            }
        }
        /// <summary>
        /// 타겟 값의 배경 색을 바꿉니다.
        /// </summary>
        [Category("User 속성")]
        [Description("타겟 값의 배경색을 설정합니다")]
        public Color TargetPosBackColor
        {
            get { return textBoxTargetPos.BackColor; }
            set
            {
                textBoxTargetPos.BackColor = value;
            }
        }
        /// <summary>
        /// 현재 값을 설정합니다
        /// </summary>
        [Category("User 속성")]
        [DefaultValue(0D)]
        [Description("현재 값을 설정합니다")]
        public double CurrentPos
        {
            get { return Convert.ToDouble(textBoxCurrentPos.Text); }
            set
            {
                double dAbs = Math.Abs(value);
                if (dAbs > 0 && dAbs < 0.001)
                {
                    if (value.ToString().Length > 4)
                        textBoxCurrentPos.Text = value.ToString().Substring(0, 4);
                    else
                        textBoxCurrentPos.Text = value.ToString().Substring(0, value.ToString().Length-1);
                }
                else
                    textBoxCurrentPos.Text = string.Format("{0:0.000}", value);
            }
        }
        /// <summary>
        /// currentPos Visible
        /// </summary>
        [Category("User 속성")]
        [Description("Current Pos 값 표시")]
        public bool visibleCurrentPos
        {
            get { return textBoxCurrentPos.Visible; }
            set
            {
                bVisibleCurrentPos = value;
                textBoxCurrentPos.Visible = bVisibleCurrentPos;
            }
        }
        /// <summary>
        /// currentPos Visible
        /// </summary>
        [Category("User 속성")]
        [Description("Target Pos 값 표시")]
        public bool visibleTargetPos
        {
            get { return textBoxTargetPos.Visible; }
            set
            {
                bVisibleTargetPos = value;
                textBoxTargetPos.Visible = bVisibleTargetPos;
            }
        }
        /// <summary>
        /// 타겟 값의 배경 색을 바꿉니다.
        /// </summary>
        [Category("User 속성")]
        [Description("현재 값의 배경색을 설정합니다")]
        public Color LimitPlusBackColor
        {
            get { return panelLimitPlus.BackColor; }
            set
            {
                panelLimitPlus.BackColor = value;
            }
        }
        /// <summary>
        /// 타겟 값의 배경 색을 바꿉니다.
        /// </summary>
        [Category("User 속성")]
        [Description("현재 값의 배경색을 설정합니다")]
        public Color LimitMinusBackColor
        {
            get { return panelLimitMinus.BackColor; }
            set
            {
                panelLimitMinus.BackColor = value;
            }
        }
        /// <summary>
        /// 타겟 값의 배경 색을 바꿉니다.
        /// </summary>
        [Category("User 속성")]
        [Description("현재 값의 배경색을 설정합니다")]
        public Color HomeBackColor
        {
            get { return panelHome.BackColor; }
            set
            {
                panelHome.BackColor = value;
            }
        }
        public TeachingInputValue()
        {
            InitializeComponent();
            bClicked = false;

            textBoxCurrentPos.KeyPress += textBox_content_KeyPress;
            textBoxTargetPos.KeyPress += textBox_content_KeyPress;
            textBoxTargetPos.BackColor = textBoxTargetPos.BackColor;

            bClickEventOff = false;
            panelAxis.Click += TeachingInputValue_ClickEvent;
            labelAxis.Click += TeachingInputValue_ClickEvent;
            tableLayoutPanel1.Click += TeachingInputValue_ClickEvent;
        }
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x2000000;
                return cp;
            }
        }
        private void textBox_content_KeyPress(object sender, KeyPressEventArgs e)
        {
            var textbox = sender as TextBox;
            int keyCode = (int)e.KeyChar;
            //46은 점 ( . )  45 -
            if ((keyCode < 48 || keyCode > 57) && keyCode != 8 && keyCode != 46 && keyCode != 45)
            {
                e.Handled = true;
            }
            if (keyCode == 46)
            {
                //null 일경우 || 이미 .이 있는경우
                if (string.IsNullOrEmpty(textbox.Text) || textbox.Text.Contains('.') == true)
                {
                    e.Handled = true;
                }
            }
            if (keyCode == 45)
            {
                //null 일경우 || 이미 .이 있는경우
                if (string.IsNullOrEmpty(textbox.Text) || textbox.Text.Contains('-') == true)
                {
                    e.Handled = true;
                }
            }
        }
        private void TeachingInputValue_ClickEvent(object sender, EventArgs e)
        {
            bClicked = !bClicked;
            checkBox1.Checked = bClicked;
            //panelAxis.BackColor = bClicked ? Color.Yellow : Color.White;
        }
        public void SetTargetPos(double dPos)
        {
            textBoxTargetPos.Invoke((Action)delegate
            {
                textBoxTargetPos.Text = string.Format("{0:0.000}", dPos);
            });
        }
        public void TurnOnClickEvent()
        {
            if (bClickEventOff)
            {
                bClickEventOff = false;
                panelAxis.Click += TeachingInputValue_ClickEvent;
                labelAxis.Click += TeachingInputValue_ClickEvent;
                tableLayoutPanel1.Click += TeachingInputValue_ClickEvent;
            }
        }
        public void TurnOffClickEvent()
        {
            if (!bClickEventOff)
            {
                bClickEventOff = true;
                panelAxis.Click -= TeachingInputValue_ClickEvent;
                labelAxis.Click -= TeachingInputValue_ClickEvent;
                tableLayoutPanel1.Click -= TeachingInputValue_ClickEvent;
            }
        }

        private void checkBox1_CheckStateChanged(object sender, EventArgs e)
        {
            bClicked = checkBox1.Checked;
            if (btnEnableCallbackEvent != null && bClicked)
                btnEnableCallbackEvent(this, null);
        }

        private void textBoxTargetPos_Click(object sender, EventArgs e)
        {
            if (clickEvent != null)
                clickEvent(this, null);
        }
    }
}
