using static CAMASSEMBLYMACHINE.Define.SystemDefine;
using static CAMASSEMBLYMACHINE.Define.UIDefine;
using CAMASSEMBLYMACHINE.Process;
using TopEng.Device;
using TopEng.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TopEng.Controls;
using CAMASSEMBLYMACHINE.Define;
using CAMASSEMBLYMACHINE.UI.SubForm;

namespace CAMASSEMBLYMACHINE.UI
{
    public partial class Form_Origin2 : Form
    {
        #region VARIABLES
        public enum BTN_SELECT
        {
            EXIT,
            SELECT_ALL,
            UNSELECT_ALL,
            ALARM_CLEAR,
            SERVO_ON,
            SERVO_OFF,
            ORIGIN
        }

        public enum SERVO_TASK
        {
            SERVO_ON,
            SERVO_OFF,
            ALARM_CLEAR,
            ORIGIN
        }
        #endregion

        AxisOrigin2[] motorArray;
        ProcessHOME proc = Machine.proclist[(int)Machine.PROCESS.HOME] as ProcessHOME;
        bool isAutoOrigin = false;
        Machine.SYSMODE sysmodeLast = Machine.SYSMODE.MANUAL;

        public Form_Origin2()
        {
            InitializeComponent();
            this.Location = new Point(MainForm.mainForm.Location.X, MainForm.mainForm.Location.Y + 89);

            sysmodeLast = Machine.sysMode;
            Machine.sysMode = Machine.SYSMODE.MANUAL;

            nV_Button_PB_NS_Exit.ClickEvent += BTN_SELECT_ClickEvent;
            nV_Button_PB_NS_SelectAll.ClickEvent += BTN_SELECT_ClickEvent;
            nV_Button_PB_NS_UnSelectAll.ClickEvent += BTN_SELECT_ClickEvent;
            nV_Button_PB_NS_ServoOn.ClickEvent += BTN_SELECT_ClickEvent;
            nV_Button_PB_NS_ServoOff.ClickEvent += BTN_SELECT_ClickEvent;
            nV_Button_PB_NS_AlarmClear.ClickEvent += BTN_SELECT_ClickEvent;
            nV_Button_PB_NS_GeneralOrigin.ClickEvent += BTN_SELECT_ClickEvent;

            motorArray = new AxisOrigin2[(int)AXIS.AXISMAX];
            motorArray[(int)AXIS.TRAY_WORK_Z] = axisOriginTrayWorkZ;
            motorArray[(int)AXIS.TRAY_TRF_Y] = axisOriginTrayTRFY;
            motorArray[(int)AXIS.TRAY_TRF_Z] = axisOriginTrayTRFZ;
            motorArray[(int)AXIS.PROD_PICKUP_X] = axisOriginProdLoaderX;
            motorArray[(int)AXIS.PROD_PICKUP_Y] = axisOriginProdLoaderY;
            motorArray[(int)AXIS.PROD_PICKUP_Z] = axisOriginProdLoaderZ;
            motorArray[(int)AXIS.PROD_PICKUP_R1] = axisOriginProdLoaderR1;
            motorArray[(int)AXIS.PROD_PICKUP_R2] = axisOriginProdLoaderR2;
            motorArray[(int)AXIS.ASSEMBLER_X] = axisOriginAssemblerX;
            motorArray[(int)AXIS.ASSEMBLER_Y] = axisOriginAssemblerY;
            motorArray[(int)AXIS.ASSEMBLER_Z] = axisOriginAssemblerZ;
            motorArray[(int)AXIS.ASSEMBLER_R1] = axisOriginAssemblerR1;
            motorArray[(int)AXIS.ASSEMBLER_R2] = axisOriginAssemblerR2;
            motorArray[(int)AXIS.CAM_TRANSFER_LEFT_Y] = axisOriginCamTRF1Y;
            motorArray[(int)AXIS.CAM_TRANSFER_RIGHT_Y] = axisOriginCamTRF2Y;
            motorArray[(int)AXIS.JIG_IN_BUF_Z] = axisOriginJIGInBufZ;

            for (int i = 0; i < (int)AXIS.AXISMAX; ++i)
                motorArray[i].ClickEvent += axisOrigin_Clicked;

        }

        public void ShowForm()
        {
            timer1.Stop();
            proc.bIsHomeRequestedNDone = false;
            timer1.Start();
            this.ShowDialog();
        }

        public void AutoOrigin()
        {
            isAutoOrigin = true;
            SelectAllMotor(panel1, true);
            var proc = Machine.proclist[(int)Machine.PROCESS.HOME] as ProcessHOME;
            for (int i = 0; i < (int)AXIS.AXISMAX; i++)
            {
                proc.SelectAxis[i] = true;
            }
            proc.SafetyReset();
            proc.SetMessage((int)ProcessHOME.MSG.MSG_HOME_START);
            Util.Delay(100);
            this.ShowForm();
        }

        #region BUTTON_CLICK_FUNCTION
        private void BTN_SELECT_ClickEvent(object sender, EventArgs e)
        {
            Dlg_MessageBox msgBox;
            string sTag = (sender as Control).Tag.ToString();
            if (Enum.TryParse(sTag, out BTN_SELECT selectBtn))
            {
                if (selectBtn == BTN_SELECT.SERVO_ON || selectBtn == BTN_SELECT.SERVO_OFF || selectBtn == BTN_SELECT.ALARM_CLEAR || selectBtn == BTN_SELECT.ORIGIN)
                {
                    for (int i = 0; i < (int)AXIS.AXISMAX; i++)
                    {
                        proc.SelectAxis[i] = motorArray[i].IsSelected;
                    }
                }

                switch (selectBtn)
                {
                    case BTN_SELECT.EXIT:
                        Machine.sysMode = sysmodeLast;
                        this.Close();
                        break;

                    case BTN_SELECT.SERVO_ON:
                        if (Machine.status.state == Define.SystemState.SystemStateERROR)
                        {
                            msgBox = new Dlg_MessageBox(EMESSAGEBOX.MSG, string.Format("Please press Reset Button in OP Box first!"));
                            msgBox.ShowDialog();
                            break;
                        }
                        msgBox = new Dlg_MessageBox(EMESSAGEBOX.QUESTION, string.Format("Do you want to Servo On?"));
                        if (msgBox.ShowDialog() == DialogResult.Yes)
                        {
                            LogUtil.Instance.Log(LOG_TYPE.SYSTEM, $"MSG_SERVO_ON_START Start", CONTENT_TYPE.INFO);
                            proc.SafetyReset();
                            proc.SetMessage((int)ProcessHOME.MSG.MSG_SERVO_ON_START);
                        }
                        break;
                    case BTN_SELECT.SERVO_OFF:
                        if (Machine.status.state == Define.SystemState.SystemStateERROR)
                        {
                            msgBox = new Dlg_MessageBox(EMESSAGEBOX.MSG, string.Format("Please press Reset Button in OP Box first!"));
                            msgBox.ShowDialog();
                            break;
                        }
                        msgBox = new Dlg_MessageBox(EMESSAGEBOX.QUESTION, string.Format("Do you want to Servo Off?"));
                        if (msgBox.ShowDialog() == DialogResult.Yes)
                        {
                            LogUtil.Instance.Log(LOG_TYPE.SYSTEM, $"MSG_SERVO_OFF_START Start", CONTENT_TYPE.INFO);
                            proc.SafetyReset();
                            proc.SetMessage((int)ProcessHOME.MSG.MSG_SERVO_OFF_START);
                        }
                        break;

                    case BTN_SELECT.ALARM_CLEAR:
                        if (Machine.status.state == Define.SystemState.SystemStateERROR)
                        {
                            msgBox = new Dlg_MessageBox(EMESSAGEBOX.MSG, string.Format("Please press Reset Button in OP Box first!"));
                            msgBox.ShowDialog();
                            break;
                        }
                        msgBox = new Dlg_MessageBox(EMESSAGEBOX.QUESTION, string.Format("Do you want to Clear Alarm?"));
                        if (msgBox.ShowDialog() == DialogResult.Yes)
                        {
                            LogUtil.Instance.Log(LOG_TYPE.SYSTEM, $"MSG_ALARM_CLEAR_START Start", CONTENT_TYPE.INFO);
                            proc.SafetyReset();
                            proc.SetMessage((int)ProcessHOME.MSG.MSG_ALARM_CLEAR_START);
                        }
                        break;

                    case BTN_SELECT.ORIGIN:
                        if (Machine.status.state == Define.SystemState.SystemStateERROR)
                        {
                            msgBox = new Dlg_MessageBox(EMESSAGEBOX.MSG, string.Format("Please press Reset Button in OP Box first!"));
                            msgBox.ShowDialog();
                            break;
                        }
                        if (!motorArray[(int)AXIS.PROD_PICKUP_Z].IsSelected
                            && (motorArray[(int)AXIS.PROD_PICKUP_X].IsSelected || motorArray[(int)AXIS.PROD_PICKUP_Y].IsSelected || motorArray[(int)AXIS.PROD_PICKUP_R1].IsSelected || motorArray[(int)AXIS.PROD_PICKUP_R2].IsSelected))
                        {
                            msgBox = new Dlg_MessageBox(EMESSAGEBOX.MSG, string.Format($"Please select {AXIS.PROD_PICKUP_Z} first!"));
                            msgBox.ShowDialog();
                            break;
                        }

                        if (!motorArray[(int)AXIS.ASSEMBLER_Z].IsSelected
                            && (motorArray[(int)AXIS.ASSEMBLER_X].IsSelected || motorArray[(int)AXIS.ASSEMBLER_Y].IsSelected || motorArray[(int)AXIS.ASSEMBLER_R1].IsSelected || motorArray[(int)AXIS.ASSEMBLER_R2].IsSelected))
                        {
                            msgBox = new Dlg_MessageBox(EMESSAGEBOX.MSG, string.Format($"Please select {AXIS.ASSEMBLER_Z} first!"));
                            msgBox.ShowDialog();
                            break;
                        }

                        msgBox = new Dlg_MessageBox(EMESSAGEBOX.QUESTION, string.Format("Do you want to Set Origin?"));
                        if (msgBox.ShowDialog() == DialogResult.Yes)
                        {
                            LogUtil.Instance.Log(LOG_TYPE.SYSTEM, $"MSG_HOME_START Start", CONTENT_TYPE.INFO);
                            proc.SafetyReset();
                            proc.SetMessage((int)ProcessHOME.MSG.MSG_HOME_START);
                        }
                        break;

                    case BTN_SELECT.SELECT_ALL:
                        SelectAllMotor(panel1, true);
                        break;
                    case BTN_SELECT.UNSELECT_ALL:
                        SelectAllMotor(panel1, false);
                        break;
                }
            }
        }

        private void SelectAllMotor(Control board, bool clicked)
        {
            foreach (Control ctrl in board.Controls)
            {
                if (ctrl.GetType() == typeof(AxisOrigin2))
                {
                    (ctrl as AxisOrigin2).SetClickedMotorOrigin(clicked);
                }
            }
        }
        #endregion

        #region DISPLAY_STATE
        private void DisplayMotorStatus()
        {
            foreach (AXIS axis in Enum.GetValues(typeof(AXIS)))
            {
                if (axis == AXIS.AXISMAX) continue;

                if (motorArray[(int)axis] == null) break;
                bool servoOn = false;
                Machine.motion.GetAxisServoOn((int)axis, ref servoOn);
                if (servoOn)
                    motorArray[(int)axis].SetMortorOriginStatus(AxisOrigin2.ORIGIN_POS_STATUS.SERVO_ON);
                else
                    motorArray[(int)axis].SetMortorOriginStatus(AxisOrigin2.ORIGIN_POS_STATUS.SERVO_OFF);

                HomeState homeState = HomeState.HomeStateNEED;
                Machine.motion.GetAxisHomeComplete((int)axis, ref homeState);
                if (homeState == HomeState.HomeStateDOING)
                    motorArray[(int)axis].SetMortorOriginStatus(AxisOrigin2.ORIGIN_POS_STATUS.HOMING);
                else if (homeState == HomeState.HomeStateDONE)
                    motorArray[(int)axis].SetMortorOriginStatus(AxisOrigin2.ORIGIN_POS_STATUS.HOME);
                else if (homeState == HomeState.HomeStateNEED)
                    motorArray[(int)axis].SetMortorOriginStatus(AxisOrigin2.ORIGIN_POS_STATUS.NOT_HOME);

                bool isAlramOn = false;
                Machine.motion.GetAlarm((int)axis, ref isAlramOn);
                if (isAlramOn)
                    motorArray[(int)axis].SetMortorOriginStatus(AxisOrigin2.ORIGIN_POS_STATUS.ALARM);
                else
                    motorArray[(int)axis].SetMortorOriginStatus(AxisOrigin2.ORIGIN_POS_STATUS.NOT_ALARM);

                bool isHW_POSLimit = false;
                Machine.motion.GetAxis_POSLimit((int)axis, ref isHW_POSLimit);
                if (isHW_POSLimit)
                    motorArray[(int)axis].SetMortorOriginStatus(AxisOrigin2.ORIGIN_POS_STATUS.LIMIT_PLUS);
                else
                    motorArray[(int)axis].SetMortorOriginStatus(AxisOrigin2.ORIGIN_POS_STATUS.LIMIT_PLUS_NOT);

                bool isHW_NEGLimit = false;
                Machine.motion.GetAxis_NEGLimit((int)axis, ref isHW_NEGLimit);
                if (isHW_NEGLimit)
                    motorArray[(int)axis].SetMortorOriginStatus(AxisOrigin2.ORIGIN_POS_STATUS.LIMIT_MINUS);
                else
                    motorArray[(int)axis].SetMortorOriginStatus(AxisOrigin2.ORIGIN_POS_STATUS.LIMIT_MINUS_NOT);

                double actualPos = 0;
                Machine.motion.GetAxisActualPos((int)axis, ref actualPos);
                motorArray[(int)axis].PosText = (actualPos / 1000.0).ToString("N3");
            }
        }
        #endregion

        #region CHECK_HOMING_FUNCTION
        private bool CheckHomeDone()
        {
            bool ret = true;

            for (int i = 0; i < (int)AXIS.AXISMAX; i++)
            {
                if (motorArray[i].IsHomeStatus != AxisOrigin2.ORIGIN_POS_STATUS.HOME) ret = false;
            }
            return ret;
        }
        #endregion

        #region EXTRA FUNCTION
        private void ButtonEnableDisplay()
        {
            foreach (Control ctrl in panel1.Controls)
            {
                if (ctrl.GetType() == typeof(AxisOrigin2))
                {
                    if ((ctrl as AxisOrigin2).IsSelected)
                    {
                        var proc = Machine.proclist[(int)Machine.PROCESS.HOME] as ProcessHOME;
                        if (proc.GetAutoStep() == (int)ProcessHOME.STEP.IDLE)
                        {
                            nV_Button_PB_NS_Exit.Enabled = true;
                            tableLayoutPanel3.Enabled = true;
                        }
                        else
                        {
                            nV_Button_PB_NS_Exit.Enabled = false;
                            tableLayoutPanel3.Enabled = false;
                        }
                        return;
                    }
                }
            }
            nV_Button_PB_NS_Exit.Enabled = true;
            tableLayoutPanel3.Enabled = false;
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
        #endregion

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Stop();
            DisplayMotorStatus();
            ButtonEnableDisplay();
            if (isAutoOrigin && CheckHomeDone() && proc.bIsHomeRequestedNDone && proc.GetStep() == (int)ProcessHOME.STEP.IDLE)
            {
                Dlg_MessageBox msgBox = new Dlg_MessageBox(EMESSAGEBOX.MSG, string.Format("Auto Origin Complete"));
                if (msgBox.ShowDialog() == DialogResult.Yes)
                {
                    Machine.sysMode = Machine.SYSMODE.AUTO;
                    this.Close();
                    return;
                }
            }
            timer1.Start();
        }
        private void axisOrigin_Clicked(object sender, EventArgs e)
        {
            //버튼 색상 처리가 먼저 된다.
            string sTag = (sender as Control).Tag.ToString();
            if (Enum.TryParse(sTag, out AXIS selectAxis))
            {
                if (selectAxis == AXIS.TRAY_TRF_Y)
                {
                    if (motorArray[(int)AXIS.TRAY_TRF_Y].IsSelected)
                    {
                        motorArray[(int)AXIS.TRAY_TRF_Z].SetClickedMotorOrigin(true);
                    }
                }
                else if (selectAxis == AXIS.TRAY_TRF_Z)
                {
                    if (!motorArray[(int)AXIS.TRAY_TRF_Z].IsSelected)
                    {
                        motorArray[(int)AXIS.TRAY_TRF_Y].SetClickedMotorOrigin(false);
                    }
                }
                if (selectAxis == AXIS.PROD_PICKUP_X)
                {
                    if (motorArray[(int)AXIS.PROD_PICKUP_X].IsSelected)
                    {
                        motorArray[(int)AXIS.PROD_PICKUP_Y].SetClickedMotorOrigin(true);
                        motorArray[(int)AXIS.PROD_PICKUP_Z].SetClickedMotorOrigin(true);
                        motorArray[(int)AXIS.ASSEMBLER_Y].SetClickedMotorOrigin(true);
                        motorArray[(int)AXIS.ASSEMBLER_Z].SetClickedMotorOrigin(true);
                    }
                }
                else if (selectAxis == AXIS.PROD_PICKUP_Y)
                {
                    if (motorArray[(int)AXIS.PROD_PICKUP_Y].IsSelected)
                    {
                        motorArray[(int)AXIS.PROD_PICKUP_Z].SetClickedMotorOrigin(true);
                        motorArray[(int)AXIS.ASSEMBLER_Y].SetClickedMotorOrigin(true);
                        motorArray[(int)AXIS.ASSEMBLER_Z].SetClickedMotorOrigin(true);
                    }
                    else
                    {
                        motorArray[(int)AXIS.PROD_PICKUP_X].SetClickedMotorOrigin(false);
                    }
                }
                else if (selectAxis == AXIS.PROD_PICKUP_R1)
                {
                    if (motorArray[(int)AXIS.PROD_PICKUP_R1].IsSelected)
                    {
                        motorArray[(int)AXIS.PROD_PICKUP_Z].SetClickedMotorOrigin(true);
                    }
                }
                else if (selectAxis == AXIS.PROD_PICKUP_R2)
                {
                    if (motorArray[(int)AXIS.PROD_PICKUP_R2].IsSelected)
                    {
                        motorArray[(int)AXIS.PROD_PICKUP_Z].SetClickedMotorOrigin(true);
                    }
                }
                else if (selectAxis == AXIS.PROD_PICKUP_Z)
                {
                    if (!motorArray[(int)AXIS.PROD_PICKUP_Z].IsSelected)
                    {
                        motorArray[(int)AXIS.PROD_PICKUP_X].SetClickedMotorOrigin(false);
                        motorArray[(int)AXIS.PROD_PICKUP_Y].SetClickedMotorOrigin(false);
                        motorArray[(int)AXIS.PROD_PICKUP_R1].SetClickedMotorOrigin(false);
                        motorArray[(int)AXIS.PROD_PICKUP_R2].SetClickedMotorOrigin(false);
                    }
                }
                else if (selectAxis == AXIS.CAM_TRANSFER_LEFT_Y)
                {
                    if (motorArray[(int)AXIS.CAM_TRANSFER_LEFT_Y].IsSelected)
                    {
                        motorArray[(int)AXIS.ASSEMBLER_Y].SetClickedMotorOrigin(true);
                        motorArray[(int)AXIS.ASSEMBLER_Z].SetClickedMotorOrigin(true);
                    }
                }
                else if (selectAxis == AXIS.CAM_TRANSFER_RIGHT_Y)
                {
                    if (motorArray[(int)AXIS.CAM_TRANSFER_RIGHT_Y].IsSelected)
                    {
                        motorArray[(int)AXIS.ASSEMBLER_Y].SetClickedMotorOrigin(true);
                        motorArray[(int)AXIS.ASSEMBLER_Z].SetClickedMotorOrigin(true);
                    }
                }

                else if (selectAxis == AXIS.ASSEMBLER_X)
                {
                    if (motorArray[(int)AXIS.ASSEMBLER_X].IsSelected)
                    {
                        motorArray[(int)AXIS.ASSEMBLER_Y].SetClickedMotorOrigin(true);
                        motorArray[(int)AXIS.ASSEMBLER_Z].SetClickedMotorOrigin(true);
                    }
                }
                else if (selectAxis == AXIS.ASSEMBLER_Y)
                {
                    if (motorArray[(int)AXIS.ASSEMBLER_Y].IsSelected)
                    {
                        motorArray[(int)AXIS.ASSEMBLER_Z].SetClickedMotorOrigin(true);
                    }
                    else
                    {
                        motorArray[(int)AXIS.PROD_PICKUP_X].SetClickedMotorOrigin(false);
                        motorArray[(int)AXIS.PROD_PICKUP_Y].SetClickedMotorOrigin(false);
                    }
                }
                else if (selectAxis == AXIS.ASSEMBLER_R1)
                {
                    if (motorArray[(int)AXIS.ASSEMBLER_R1].IsSelected)
                    {
                        motorArray[(int)AXIS.ASSEMBLER_Z].SetClickedMotorOrigin(true);
                    }
                }
                else if (selectAxis == AXIS.ASSEMBLER_R2)
                {
                    if (motorArray[(int)AXIS.ASSEMBLER_R2].IsSelected)
                    {
                        motorArray[(int)AXIS.ASSEMBLER_Z].SetClickedMotorOrigin(true);
                    }
                }
                else if (selectAxis == AXIS.ASSEMBLER_Z)
                {
                    if (!motorArray[(int)AXIS.ASSEMBLER_Z].IsSelected)
                    {
                        motorArray[(int)AXIS.ASSEMBLER_X].SetClickedMotorOrigin(false);
                        motorArray[(int)AXIS.ASSEMBLER_Y].SetClickedMotorOrigin(false);
                        motorArray[(int)AXIS.ASSEMBLER_R1].SetClickedMotorOrigin(false);
                        motorArray[(int)AXIS.ASSEMBLER_R2].SetClickedMotorOrigin(false);
                        motorArray[(int)AXIS.PROD_PICKUP_X].SetClickedMotorOrigin(false);
                        motorArray[(int)AXIS.PROD_PICKUP_Y].SetClickedMotorOrigin(false);
                    }
                }
            }
        }
    }
}