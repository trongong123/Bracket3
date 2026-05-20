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
    public partial class AxisOrigin2 : UserControl
    {
        private bool bClicked = false;
        private bool isAlarm = false;
        private bool isServoOn = false;
        public enum ORIGIN_POS_STATUS
        {
            None,
            HOME,
            HOMING,
            NOT_HOME,
            SERVO_ON,
            SERVO_OFF,
            LIMIT_PLUS,
            LIMIT_PLUS_NOT,
            LIMIT_MINUS,
            LIMIT_MINUS_NOT,
            ALARM,
            NOT_ALARM
        }
        Color colorBoaderSelected = Color.Yellow;
        Color colorBoaderDeSelected = SystemColors.ControlDark;
        Color colorNone = SystemColors.Control;
        Color colorHome = Color.Lime;
        Color colorHoming = Color.Orange;
        Color colorHomeNot = Color.Red;
        Color colorServoOn = Color.Lime;
        Color colorServoOff = Color.Orange;
        Color colorLimitOver = Color.Red;
        Color colorAlarm = Color.Red;
        Color colorAlarmNot = SystemColors.Control;

        /// <summary>
        /// 이름 Text를 수정합니다
        /// </summary>
        [Category("User 속성")]
        [Description("이름 Text를 수정합니다")]
        public string NameText
        {
            get { return labelName.Text; }
            set { labelName.Text = value; }
        }
        /// <summary>
        /// Pos Text를 수정합니다
        /// </summary>
        [Category("User 속성")]
        [Description("이름 Text를 수정합니다")]
        public string PosText
        {
            get { return labelPos.Text; }
            set { labelPos.Text = value; }
        }
        /// <summary>
        /// Home OK 색상을 설정 합니다.
        /// </summary>
        [Category("User 속성")]
        [Description("Home OK 색상을 설정 합니다.")]
        public Color ColorHome
        {
            get { return colorHome; }
            set { /*colorHome = value;*/ }
        }
        /// <summary>
         /// Home OK 색상을 설정 합니다.
         /// </summary>
        [Category("User 속성")]
        [Description("Homing 색상을 설정 합니다.")]
        public Color ColorHoming
        {
            get { return colorHoming; }
            set { /*colorHoming = value;*/ }
        }

        /// <summary>
        /// Home NG 색상을 설정 합니다.
        /// </summary>
        [Category("User 속성")]
        [Description("Home NG 색상을 설정 합니다.")]
        public Color ColorHomeNot
        {
            get { return colorHomeNot; }
            set { /*colorHomeNot = value;*/ }
        }
        /// <summary>
        /// Servo ON색상을 설정 합니다.
        /// </summary>
        [Category("User 속성")]
        [Description("Servo ON색상을 설정 합니다.")]
        public Color ColorServoOn
        {
            get { return colorServoOn; }
            set { /*colorServoOn = value;*/ }
        }
        /// <summary>
        /// Servo ON색상을 설정 합니다.
        /// </summary>
        [Category("User 속성")]
        [Description("Servo ON색상을 설정 합니다.")]
        public Color ColorServoOFF
        {
            get { return colorServoOff; }
            set { /*colorServoOff = value;*/ }
        }
        /// <summary>
        /// Limit Over 색상을 설정 합니다.
        /// </summary>
        [Category("User 속성")]
        [Description("Limit Over 색상을 설정 합니다.")]
        public Color ColorLimit
        {
            get { return colorLimitOver; }
            set { /*colorLimitOver = value;*/ }
        }
        /// <summary>
        /// 선택된 값을 가져옵니다 true = clicked
        /// </summary>
        public bool IsSelected
        {
            get { return bClicked; }
        }
        public ORIGIN_POS_STATUS IsHomeStatus { get { return _homeStatus; } }
        private ORIGIN_POS_STATUS _homeStatus = ORIGIN_POS_STATUS.None;

        public bool IsAlarm
        {
            get { return isAlarm; }
        }
        public bool IsServoOn
        {
            get { return isServoOn; }
        }
        public AxisOrigin2()
        {
            InitializeComponent();
            labelName.Click += OriginMortor_Click;
            labelPos.Click += OriginMortor_Click;
            labelHome.Click += OriginMortor_Click;
            labelServo.Click += OriginMortor_Click;
            labelAlarm.Click += OriginMortor_Click;
            labelLimitPlus.Click += OriginMortor_Click;
            labelLimitMinus.Click += OriginMortor_Click;

            panelHome.Click += OriginMortor_Click;
            panelServo.Click += OriginMortor_Click;
            panelAlarm.Click += OriginMortor_Click;
            panelLimitMinus.Click += OriginMortor_Click;
            panelLimitPlus.Click += OriginMortor_Click;
            tableLayoutPanel1.Click += OriginMortor_Click;
            tableLayoutPanel2.Click += OriginMortor_Click;

            labelName.Click += MyUserControl_Click;
            labelPos.Click += MyUserControl_Click;
            labelHome.Click += MyUserControl_Click;
            labelServo.Click += MyUserControl_Click;
            labelAlarm.Click += MyUserControl_Click;
            labelLimitPlus.Click += MyUserControl_Click;
            labelLimitMinus.Click += MyUserControl_Click;

            panelHome.Click += MyUserControl_Click;
            panelServo.Click += MyUserControl_Click;
            panelAlarm.Click += MyUserControl_Click;
            panelLimitMinus.Click += MyUserControl_Click;
            panelLimitPlus.Click += MyUserControl_Click;
            tableLayoutPanel1.Click += MyUserControl_Click;
            tableLayoutPanel2.Click += MyUserControl_Click;

            InitMortorOriginStatus();
        }
        public void InitMortorOriginStatus()
        {
            SetMortorOriginStatus(ORIGIN_POS_STATUS.NOT_HOME);
            SetMortorOriginStatus(ORIGIN_POS_STATUS.LIMIT_PLUS_NOT);
            SetMortorOriginStatus(ORIGIN_POS_STATUS.LIMIT_MINUS_NOT);
            SetMortorOriginStatus(ORIGIN_POS_STATUS.SERVO_ON);
        }
        public void SetMortorOriginStatus(ORIGIN_POS_STATUS status )
        {
            switch(status)
            {
                case ORIGIN_POS_STATUS.None:            
                    panelHome.BackColor = colorNone;  break;
                case ORIGIN_POS_STATUS.HOME:            
                    _homeStatus = status;
                    panelHome.BackColor = colorHome;
                    labelHome.Text = "HOME";
                    CenterLabelOnPanel(labelHome, panelHome);
                    break;
                case ORIGIN_POS_STATUS.HOMING:
                    _homeStatus = status;
                    panelHome.BackColor = colorHoming;
                    labelHome.Text = "HOMING";
                    CenterLabelOnPanel(labelHome, panelHome);
                    break;
                case ORIGIN_POS_STATUS.NOT_HOME:        
                    _homeStatus = status;
                    panelHome.BackColor = colorHomeNot;
                    labelHome.Text = "NOT HOME";
                    CenterLabelOnPanel(labelHome, panelHome);
                    break;
                case ORIGIN_POS_STATUS.SERVO_ON:        
                    panelServo.BackColor = colorHome;
                    labelServo.Text = "SERVO ON";
                    CenterLabelOnPanel(labelServo, panelServo);
                    isServoOn = true;
                    break;
                case ORIGIN_POS_STATUS.SERVO_OFF:       
                    panelServo.BackColor = colorHomeNot;
                    labelServo.Text = "SERVO OFF";
                    CenterLabelOnPanel(labelServo, panelServo);
                    isServoOn = false;
                    break;
                case ORIGIN_POS_STATUS.LIMIT_PLUS:       
                    panelLimitPlus.BackColor = colorLimitOver; break;
                case ORIGIN_POS_STATUS.LIMIT_PLUS_NOT:   
                    panelLimitPlus.BackColor = colorNone; break;
                case ORIGIN_POS_STATUS.LIMIT_MINUS:       
                    panelLimitMinus.BackColor = colorLimitOver; break;
                case ORIGIN_POS_STATUS.LIMIT_MINUS_NOT:   
                    panelLimitMinus.BackColor = colorNone; break;
                case ORIGIN_POS_STATUS.ALARM:           
                    panelAlarm.BackColor = colorAlarm;
                    labelAlarm.Text = "ALARM";
                    CenterLabelOnPanel(labelAlarm, panelAlarm);
                    isAlarm = true;
                    break;
                case ORIGIN_POS_STATUS.NOT_ALARM:       
                    panelAlarm.BackColor = colorAlarmNot;
                    labelAlarm.Text = "NO ALARM";
                    CenterLabelOnPanel(labelAlarm, panelAlarm);
                    isAlarm = false;
                    break;
            }
        }
        public void SetClickedMotorOrigin(bool clicked)
        {
            bClicked = clicked;
            this.BackColor = bClicked ? colorBoaderSelected : colorBoaderDeSelected;
        }
        private void OriginMortor_Click(object sender, EventArgs e)
        {
            SetClickedMotorOrigin(!bClicked);
        }
        /// <summary>
        /// Text 위치를 중앙으로 변경한다
        /// </summary>
        /// <param name="label"></param>
        /// <param name="panel"></param>
        private void CenterLabelOnPanel(Label label, Panel panel)
        {
            // Label의 중앙 좌표를 계산
            int labelX = (panel.Width - label.Width) / 2;
            int labelY = (panel.Height - label.Height) / 2;

            // Label의 위치를 설정
            label.Location = new Point(labelX, labelY);
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

        private void OriginMortor_Resize(object sender, EventArgs e)
        {
            CenterLabelOnPanel(labelHome, panelHome);
            CenterLabelOnPanel(labelServo, panelServo);
        }

        public event EventHandler ClickEvent;

        private void MyUserControl_Click(object sender, EventArgs e)
        {
            if (ClickEvent != null)
            {
                ClickEvent(this, e);
            }
        }
    }
}
