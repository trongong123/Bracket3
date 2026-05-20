using NV_UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using CAMASSEMBLYMACHINE.Define;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static CAMASSEMBLYMACHINE.Define.SystemDefine;
using static CAMASSEMBLYMACHINE.Define.UIDefine;
using CAMASSEMBLYMACHINE.Process;
using TopEng.Utils;
using TopEng.Controls;
using TopEng.Device;

namespace CAMASSEMBLYMACHINE.UI
{

    public partial class tabOffsetTeach : Form, IForm
    {
        public enum CAMPAGE
        {
            UNKNOWN = -1,
            PP
        }
        enum TEACHING_TASK
        {
            NONE,
            RESET,
            SAVE,

            SET_VEL_SLOW,
            SET_VEL_NORMAL,
            SET_VEL_FAST
        }

        private TEACHING_TASK eCurrentTask;
        public delegate void BtnEnableCallbackEvent(bool bEnable);
        public event BtnEnableCallbackEvent btnEnableCallbackEvent;
        public BTN_TAB_SELECT mCurrentTeachingPoint;
        private bool bPrevCmdVacOnOrOff;

        uint ToolZUp = 0;
        uint ToolVacOn = 0;
        CAMPAGE curPage = CAMPAGE.UNKNOWN;
        public BTN_TAB_SELECT mLeftTeachingPoint = BTN_TAB_SELECT.TRAY_LEFT_OFFSET;
        public BTN_TAB_SELECT mRightTeachingPoint = BTN_TAB_SELECT.TRAY_RIGHT_OFFSET;
        public List<NV_UI.NV_Button_TG_NS> listTeachingPointBtn = new List<NV_UI.NV_Button_TG_NS>();

        //리셋버튼 누른 순간 저장됨
        double lastCurPosX = 0; 
        double lastCurPosY = 0;

        AXIS axisX, axisY, axisR1, axisR2, axisZ;
        RecipeDefine.CALIBRATION tagX, tagY;

        public tabOffsetTeach()
        {
            InitializeComponent();
            InitializeVariable();
            SetBtnMouseEvents(FunctionBtnClickEvent);
        }
        private void InitializeVariable()
        {
            this.curPage = CAMPAGE.PP;

            this.eCurrentTask = TEACHING_TASK.NONE;
            this.mCurrentTeachingPoint = BTN_TAB_SELECT.TRAY_LEFT_OFFSET;

            //Reset 버튼을 시작할 때 한번씩 눌렀다.
            //처음에 Offset은 0이 나올 것
            Machine.motion.GetAxisActualPos((int)AXIS.PROD_PICKUP_X, ref lastCurPosX);
            Machine.motion.GetAxisActualPos((int)AXIS.PROD_PICKUP_Y, ref lastCurPosY);

            tbxSpeedValue1.Text = (UIDefine.UIJOGVEL[(int)UIDefine.UIJOGVELINFO.NORMAL] / 1000.0).ToString();
            tbxSpeedValue2.Text = (UIDefine.UIJOGVEL[(int)UIDefine.UIJOGVELINFO.NORMAL] / 1000.0).ToString();
        }

        public void StartTimer(bool enable)
        {
            if (enable)
                timer1.Start();
            else
                timer1.Stop();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            UpdateIOState();
            UpdateAxisState();
            UpdateServoState();
        }

        private void SwitchOffOtherButtons(object sender)
        {
            string sTag = (sender as Control).Tag.ToString();
            string compareTag = string.Empty;
            foreach (Control ctrl in tableLayoutPanelButtons.Controls)
            {
                if (ctrl.GetType() == typeof(NV_Button_TG_NS))
                {
                    compareTag = ctrl.Tag.ToString();
                    if (sTag != compareTag)
                    {
                        (ctrl as NV_Button_TG_NS).UserOff();
                    }
                    else
                    {
                        (ctrl as NV_Button_TG_NS).UserOn();
                    }
                }
            }
            foreach (Control ctrl in tableLayoutPanelButtons2.Controls)
            {
                if (ctrl.GetType() == typeof(NV_Button_TG_NS))
                {
                    compareTag = ctrl.Tag.ToString();
                    if (sTag != compareTag)
                    {
                        (ctrl as NV_Button_TG_NS).UserOff();
                    }
                    else
                    {
                        (ctrl as NV_Button_TG_NS).UserOn();
                    }
                }
            }
        }

        #region Update Status
        private void UpdateServoState()
        {
            bool ServoStatus = true;
            bool ServoStatusCheck = true;
            bool AlarmStatus = false;
            bool AlarmStatusCheck = false;

            if (teachingInputValue1.Clicked)
            {
                Machine.motion.GetAxisServoOn((int)axisX, ref ServoStatusCheck);
                if (!ServoStatusCheck)
                    ServoStatus = false;
                Machine.motion.GetAlarm((int)axisX, ref AlarmStatusCheck);
                if (AlarmStatusCheck)
                    AlarmStatus = true;
            }
            if (teachingInputValue2.Clicked)
            {
                Machine.motion.GetAxisServoOn((int)axisY, ref ServoStatusCheck);
                if (!ServoStatusCheck)
                    ServoStatus = false;
                Machine.motion.GetAlarm((int)axisY, ref AlarmStatusCheck);
                if (AlarmStatusCheck)
                    AlarmStatus = true;
            }
            disp_ServoOnOff.ButtonPush = ServoStatus == true ? true : false;
            disp_AlarmReset.ButtonPush = AlarmStatus == false ? true : false;
        }

        private void UpdateIOState()
        {
            uint returnValue = 0;

            if (Machine.IO == null)
                return;

            if (axisX == AXIS.PROD_PICKUP_X && axisY == AXIS.PROD_PICKUP_Y)
            {
                Machine.IO.GetIn((int)DI.CAM_PICKER_Z1_UP, ref returnValue);
                disp_Tool1Up.ButtonPush = returnValue == 1 ? true : false;
                Machine.IO.GetIn((int)DI.CAM_PICKER_Z1_DOWN, ref returnValue);
                disp_Tool1Down.ButtonPush = returnValue == 1 ? true : false;
                Machine.IO.GetIn((int)DI.CAM_PICKER_Z2_UP, ref returnValue);
                disp_Tool2Up.ButtonPush = returnValue == 1 ? true : false;
                Machine.IO.GetIn((int)DI.CAM_PICKER_Z2_DOWN, ref returnValue);
                disp_Tool2Down.ButtonPush = returnValue == 1 ? true : false;
                Machine.IO.GetIn((int)DI.CAM_PICKER_Z1_VACON, ref returnValue);
                disp_Tool1VacOn.ButtonPush = returnValue == 1 ? true : false;
                disp_Tool1VacOff.ButtonPush = returnValue == 0 ? true : false;
                Machine.IO.GetIn((int)DI.CAM_PICKER_Z2_VACON, ref returnValue);
                disp_Tool2VacOn.ButtonPush = returnValue == 1 ? true : false;
                disp_Tool2VacOff.ButtonPush = returnValue == 0 ? true : false;
            }
            else if (axisX == AXIS.ASSEMBLER_X && axisY == AXIS.ASSEMBLER_Y)
            {
                Machine.IO.GetIn((int)DI.ASSEMBLER_Z1_UP, ref returnValue);
                disp_Tool1Up.ButtonPush = returnValue == 1 ? true : false;
                Machine.IO.GetIn((int)DI.ASSEMBLER_Z1_DOWN, ref returnValue);
                disp_Tool1Down.ButtonPush = returnValue == 1 ? true : false;
                Machine.IO.GetIn((int)DI.ASSEMBLER_Z2_UP, ref returnValue);
                disp_Tool2Up.ButtonPush = returnValue == 1 ? true : false;
                Machine.IO.GetIn((int)DI.ASSEMBLER_Z2_DOWN, ref returnValue);
                disp_Tool2Down.ButtonPush = returnValue == 1 ? true : false;
                Machine.IO.GetIn((int)DI.ASSEMBLER_Z1_VACON, ref returnValue);
                disp_Tool1VacOn.ButtonPush = returnValue == 1 ? true : false;
                disp_Tool1VacOff.ButtonPush = returnValue == 0 ? true : false;
                Machine.IO.GetIn((int)DI.ASSEMBLER_Z2_VACON, ref returnValue);
                disp_Tool2VacOn.ButtonPush = returnValue == 1 ? true : false;
                disp_Tool2VacOff.ButtonPush = returnValue == 0 ? true : false;
            }
        }

        private void UpdateAxisState()
        {
            double actualPos = 0;
            bool isHW_POSLimit = false; bool isHW_NEGLimit = false;
            HomeState isHW_POSHome = HomeState.HomeStateNEED;

            Machine.motion.GetAxisActualPos((int)axisX, ref actualPos);
            teachingInputValue1.CurrentPos = (actualPos - lastCurPosX) / 1000.0;

            Machine.motion.GetAxisActualPos((int)axisY, ref actualPos);
            teachingInputValue2.CurrentPos = (actualPos - lastCurPosY) / 1000.0;

            teachingInputValue1.TargetPos = Machine.recipe.Calibration(tagX);
            teachingInputValue2.TargetPos = Machine.recipe.Calibration(tagX);

            Machine.motion.GetAxis_POSLimit((int)axisX, ref isHW_POSLimit);
            Machine.motion.GetAxis_NEGLimit((int)axisX, ref isHW_NEGLimit);
            Machine.motion.GetAxisHomeComplete((int)axisX, ref isHW_POSHome);
            teachingInputValue1.LimitPlusBackColor = isHW_POSLimit == true ? Color.Red : Color.White;
            teachingInputValue1.LimitMinusBackColor = isHW_NEGLimit == true ? Color.Red : Color.White;
            teachingInputValue1.HomeBackColor = isHW_POSHome == HomeState.HomeStateDONE ? Color.Lime : Color.Red;

            Machine.motion.GetAxis_POSLimit((int)axisY, ref isHW_POSLimit);
            Machine.motion.GetAxis_NEGLimit((int)axisY, ref isHW_NEGLimit);
            Machine.motion.GetAxisHomeComplete((int)axisY, ref isHW_POSHome);
            teachingInputValue2.LimitPlusBackColor = isHW_POSLimit == true ? Color.Red : Color.White;
            teachingInputValue2.LimitMinusBackColor = isHW_NEGLimit == true ? Color.Red : Color.White;
            teachingInputValue2.HomeBackColor = isHW_POSHome == HomeState.HomeStateDONE ? Color.Lime : Color.Red;
        }
        #endregion

        #region TEACHING_POS_SELECTED_FUNCTION
        public enum BTN_TAB_SELECT
        {
            TRAY_LEFT_OFFSET,
            TRAY_RIGHT_OFFSET,
            ASSEMBLER_LEFT_OFFSET,
            ASSEMBLER_RIGHT_OFFSET,
        }
        enum AXIS_DIRECTION2
        {
            X,
            Y,
            R1,
            R2,
            Z
        }
        private void SelectButton_ClickEvent(object sender, EventArgs e)
        {
            SwitchOffOtherButtons(sender);

            // to do
            string sTag = (sender as Control).Tag.ToString();
            if (Enum.TryParse(sTag, out BTN_TAB_SELECT selectBtn))
            {
                mCurrentTeachingPoint = selectBtn;
                switch (selectBtn)
                {
                    case BTN_TAB_SELECT.TRAY_LEFT_OFFSET:
                        tagX = RecipeDefine.CALIBRATION.PROD_LOADER_OFFSET_LEFT_X;
                        tagY = RecipeDefine.CALIBRATION.PROD_LOADER_OFFSET_LEFT_Y;
                        axisX = AXIS.PROD_PICKUP_X;
                        axisY = AXIS.PROD_PICKUP_Y;
                        axisR1 = AXIS.PROD_PICKUP_R1;
                        axisR2 = AXIS.PROD_PICKUP_R2;
                        axisZ = AXIS.PROD_PICKUP_Z;
                        break;
                    case BTN_TAB_SELECT.TRAY_RIGHT_OFFSET:
                        tagX = RecipeDefine.CALIBRATION.PROD_LOADER_OFFSET_RIGHT_X;
                        tagY = RecipeDefine.CALIBRATION.PROD_LOADER_OFFSET_RIGHT_Y;
                        axisX = AXIS.PROD_PICKUP_X;
                        axisY = AXIS.PROD_PICKUP_Y;
                        axisR1 = AXIS.PROD_PICKUP_R1;
                        axisR2 = AXIS.PROD_PICKUP_R2;
                        axisZ = AXIS.PROD_PICKUP_Z;
                        break;                    
                    case BTN_TAB_SELECT.ASSEMBLER_LEFT_OFFSET:
                        tagX = RecipeDefine.CALIBRATION.ASSEMBLER_OFFSET1_X;;
                        tagY = RecipeDefine.CALIBRATION.ASSEMBLER_OFFSET1_Y; ;
                        axisX = AXIS.ASSEMBLER_X;
                        axisY = AXIS.ASSEMBLER_Y;
                        axisR1 = AXIS.ASSEMBLER_R1;
                        axisR2 = AXIS.ASSEMBLER_R2;
                        axisZ = AXIS.ASSEMBLER_Z;
                        break;
                    case BTN_TAB_SELECT.ASSEMBLER_RIGHT_OFFSET:
                        tagX = RecipeDefine.CALIBRATION.ASSEMBLER_OFFSET2_X;;
                        tagY = RecipeDefine.CALIBRATION.ASSEMBLER_OFFSET2_Y; ;
                        axisX = AXIS.ASSEMBLER_X;
                        axisY = AXIS.ASSEMBLER_Y;
                        axisR1 = AXIS.ASSEMBLER_R1;
                        axisR2 = AXIS.ASSEMBLER_R2;
                        axisZ = AXIS.ASSEMBLER_Z;
                        break;
                }

                teachingInputValue1.TargetPos = Machine.recipe.Calibration(tagX);
                teachingInputValue2.TargetPos = Machine.recipe.Calibration(tagY);
                teachingInputValue1.Visible = true;
                teachingInputValue2.Visible = true;

                button_XPlus.Enabled = true;
                button_XMinus.Enabled = true;
                button_YPlus.Enabled = true;
                button_YMinus.Enabled = true;
                button_R1Plus.Enabled = true;
                button_R1Minus.Enabled = true;
                button_R2Plus.Enabled = true;
                button_R2Minus.Enabled = true;
                button_ZMinus.Enabled = true;
                button_ZPlus.Enabled = true;

                if (tabControl1.SelectedIndex == 0)
                {
                    mLeftTeachingPoint = mCurrentTeachingPoint;
                }
                else if (tabControl1.SelectedIndex == 1)
                {
                    mRightTeachingPoint = mCurrentTeachingPoint;
                }

            }
        }
        bool firstStart = true;

        private void Tab_Select_First_Shown(object sender, EventArgs e)
        {
            if (firstStart)
            {
                SelectButton_ClickEvent(nV_Button_TG_NS_TrayLeftOffset, null);
                firstStart = false;
            }
        }
        #endregion
        #region BUTTON_CLICK_FUNCTION
        public void SetBtnMouseEvents(NV_UI.NV_Button_PB_NS.ClickEventDelegate functionBtnEvent)
        {
            nV_Button_TG_NS_TrayLeftOffset.ClickEvent += SelectButton_ClickEvent;
            nV_Button_TG_NS_TrayRightOffset.ClickEvent += SelectButton_ClickEvent;;
            nV_Button_TG_NS_AssemblerLeftOffset.ClickEvent += SelectButton_ClickEvent;
            nV_Button_TG_NS_AssemblerRightOffset.ClickEvent += SelectButton_ClickEvent;

            listTeachingPointBtn.Add(nV_Button_TG_NS_TrayLeftOffset);
            listTeachingPointBtn.Add(nV_Button_TG_NS_TrayRightOffset);
            listTeachingPointBtn.Add(nV_Button_TG_NS_AssemblerLeftOffset);
            listTeachingPointBtn.Add(nV_Button_TG_NS_AssemblerRightOffset);

            nV_Button_PB_NS_Reset.ClickEvent += functionBtnEvent;
            nV_Button_PB_NS_Save.ClickEvent += functionBtnEvent;
            nV_Button_PB_NS_SetVelSlow.ClickEvent += functionBtnEvent;
            nV_Button_PB_NS_SetVelNormal.ClickEvent += functionBtnEvent;
            nV_Button_PB_NS_SetVelFast.ClickEvent += functionBtnEvent;

            teachingInputValue1.btnEnableCallbackEvent += CheckBoxClickEvent;
            teachingInputValue2.btnEnableCallbackEvent += CheckBoxClickEvent;
        }

        private void FunctionBtnClickEvent(object sender, EventArgs e)
        {
            string strBtnTag = string.Empty;

            strBtnTag = (sender as Control).Tag.ToString();
            // 버튼  Tag가 설정되어 있지 않을 때
            if (strBtnTag == null) return;
            if (Enum.TryParse(strBtnTag, out TEACHING_TASK eBtnType) == false) return;

            switch (eBtnType)
            {
                case TEACHING_TASK.RESET:
                    Dlg_MessageBox formReset = new Dlg_MessageBox(EMESSAGEBOX.WARNING, string.Format("기준 좌표를 리셋하시겠습니까?"));
                    if (formReset.ShowDialog() == DialogResult.Yes)
                    {
                        if (teachingInputValue1.Clicked)
                            Machine.motion.GetAxisActualPos((int)axisX, ref lastCurPosX);
                        if (teachingInputValue2.Clicked)
                            Machine.motion.GetAxisActualPos((int)axisY, ref lastCurPosY);
                        break;
                    }
                    break;
                case TEACHING_TASK.SAVE:
                    Dlg_MessageBox formSave = new Dlg_MessageBox(EMESSAGEBOX.WARNING, string.Format("Offset 값을 저장하시겠습니까?"));
                    if (formSave.ShowDialog() == DialogResult.Yes)
                    {
                        //현재 저장된 OFFSET값에 CurrentPos(는 마지막 Reset해서 저장한 Act Pos와 현재 Act Pos의 차이)를 더함
                        if (teachingInputValue1.Clicked)
                        {
                            double dNewPos = Machine.recipe.Calibration(tagX) + teachingInputValue1.CurrentPos;
                            SaveNewPos(dNewPos, AXIS_DIRECTION2.X);
                        }
                        if (teachingInputValue2.Clicked)
                        {
                            double dNewPos = Machine.recipe.Calibration(tagY) + teachingInputValue2.CurrentPos;
                            SaveNewPos(dNewPos, AXIS_DIRECTION2.Y);
                        }
                        break;
                    }
                    break;
                case TEACHING_TASK.SET_VEL_SLOW:
                    tbxSpeedValue1.Text = (UIDefine.UIJOGVEL[(int)UIDefine.UIJOGVELINFO.SLOWER] / 1000.0).ToString();
                    tbxSpeedValue2.Text = (UIDefine.UIJOGVEL[(int)UIDefine.UIJOGVELINFO.SLOWER] / 1000.0).ToString();
                    break;
                case TEACHING_TASK.SET_VEL_NORMAL:
                    tbxSpeedValue1.Text = (UIDefine.UIJOGVEL[(int)UIDefine.UIJOGVELINFO.NORMAL] / 1000.0).ToString();
                    tbxSpeedValue2.Text = (UIDefine.UIJOGVEL[(int)UIDefine.UIJOGVELINFO.NORMAL] / 1000.0).ToString();
                    break;
                case TEACHING_TASK.SET_VEL_FAST:
                    tbxSpeedValue1.Text = (UIDefine.UIJOGVEL[(int)UIDefine.UIJOGVELINFO.FASTER] / 1000.0).ToString();
                    tbxSpeedValue2.Text = (UIDefine.UIJOGVEL[(int)UIDefine.UIJOGVELINFO.FASTER] / 1000.0).ToString();
                    break;
            }
        }

        private void CheckBoxClickEvent(object sender, EventArgs e)
        {
            TeachingInputValue input = (sender as TeachingInputValue);
            if (input != null && input.Clicked)
            {
                string sTag = input.Tag.ToString();
                switch (sTag)
                {
                    case "InputX":
                        if (teachingInputValue2.Clicked)
                            teachingInputValue2.Clicked = false;
                        break;
                    case "InputY":
                        if (teachingInputValue1.Clicked)
                            teachingInputValue1.Clicked = false;
                        break;                   
                }
            }
        }

        public string SpeedValueXY
        {
            get { return tbxSpeedValue1.Text; }
            set { tbxSpeedValue1.Text = value; }
        }

        public string SpeedValueRZ
        {
            get { return tbxSpeedValue2.Text; }
            set { tbxSpeedValue2.Text = value; }
        }

        private void MoveJogAxis(int axis, bool direction)
        {
            string dirStr = direction == true ? "Plus" : "Minus";
            LogUtil.Instance.Log(LOG_TYPE.UI, ((AXIS)axis).ToString() + " Move Jog to " + dirStr, CONTENT_TYPE.INFO);

            if (Machine.status.state == SystemState.SystemStateERROR)
            {
                Dlg_MessageBox formErr = new Dlg_MessageBox(EMESSAGEBOX.MSG, string.Format("Please push the reset button"));
                formErr.TopLevel = true;
                formErr.TopMost = true;
                formErr.ShowDialog();
                return;
            }

            var proc = Machine.proclist[(int)Machine.PROCESS.SYS] as ProcessSys;
            if (proc.doorOpenState)
            {
                Dlg_MessageBox msgBox = new Dlg_MessageBox(EMESSAGEBOX.MSG,
                               string.Format("Please Close the door"));
                msgBox.ShowDialog();
                return;
            }

            double dSpeed;
            double MaxSpeedXY = 1000 * 1000;
            double MinSpeedXY = 1 * 1000;
            double MaxSpeedRZ = 300 * 1000;
            double MinSpeedRZ = 1 * 1000;

            switch (axis)
            {
                case (int)AXIS.PROD_PICKUP_X:
                case (int)AXIS.PROD_PICKUP_Y:
                case (int)AXIS.ASSEMBLER_X:
                case (int)AXIS.ASSEMBLER_Y:
                    if (!double.TryParse(SpeedValueXY, out dSpeed))
                    {
                        MessageBox.Show("Invalid Speed Value", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    dSpeed = Math.Abs(dSpeed) * 1000;
                    
                    if(dSpeed > MaxSpeedXY)
                    {
                        MessageBox.Show("Max Speed 1000 m/s. Please Change Value.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }

                    if (dSpeed < MinSpeedXY)
                    {
                        MessageBox.Show("Min Speed 1 m/s. Please Change Value.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }

                    double acceleration = 10 * dSpeed;
                    double delceleration = 10 * dSpeed;

                    if (!direction)
                        dSpeed *= (-1.0);

                    Machine.motion.MoveAxisJog(axis, dSpeed, acceleration, delceleration);
                    Machine.usingMoveJog = true;
                    break;
                case (int)AXIS.PROD_PICKUP_Z:
                case (int)AXIS.PROD_PICKUP_R1:
                case (int)AXIS.PROD_PICKUP_R2:
                case (int)AXIS.ASSEMBLER_Z:
                case (int)AXIS.ASSEMBLER_R1:
                case (int)AXIS.ASSEMBLER_R2:
                    if (!double.TryParse(SpeedValueRZ, out dSpeed))
                    {
                        MessageBox.Show("Invalid Speed Value", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    dSpeed = Math.Abs(dSpeed) * 1000;

                    if (dSpeed > MaxSpeedRZ)
                    {
                        MessageBox.Show("Max Speed 300 m/s. Please Change Value.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }

                    if (dSpeed < MinSpeedRZ)
                    {
                        MessageBox.Show("Min Speed 1 m/s. Please Change Value.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }

                    double acceleration2 = 10 * dSpeed;
                    double delceleration2 = 10 * dSpeed;

                    if (!direction)
                        dSpeed *= (-1.0);

                    Machine.motion.MoveAxisJog(axis, dSpeed, acceleration2, delceleration2);
                    Machine.usingMoveJog = true;
                    break;
            }
        }

        private void button_R1Plus_MouseDown(object sender, MouseEventArgs e)
        {
            MoveJogAxis((int)axisR1, true);
        }

        private void button_R1Plus_MouseUp(object sender, MouseEventArgs e)
        {
            Machine.motion.AxisStopAll(true);
            Machine.usingMoveJog = false;
        }

        private void button_R1Minus_MouseDown(object sender, MouseEventArgs e)
        {
            MoveJogAxis((int)axisR1, false);
        }

        private void button_R1Minus_MouseUp(object sender, MouseEventArgs e)
        {
            Machine.motion.AxisStopAll(true);
            Machine.usingMoveJog = false;
        }

        private void button_R2Plus_MouseDown(object sender, MouseEventArgs e)
        {
            MoveJogAxis((int)axisR2, true);
        }

        private void button_R2Plus_MouseUp(object sender, MouseEventArgs e)
        {
            Machine.motion.AxisStopAll(true);
            Machine.usingMoveJog = false;
        }

        private void button_R2Minus_MouseDown(object sender, MouseEventArgs e)
        {
            MoveJogAxis((int)axisR2, false);
        }

        private void button_R2Minus_MouseUp(object sender, MouseEventArgs e)
        {
            Machine.motion.AxisStopAll(true);
            Machine.usingMoveJog = false;
        }

        private void button_YPlus_MouseDown(object sender, MouseEventArgs e)
        {            
            if (axisY == AXIS.PROD_PICKUP_Y)
                Machine.interfer_release_x_safe_area = true;
            else if (axisY == AXIS.ASSEMBLER_Y)
                Machine.interfer_release_y_crash = true;
            MoveJogAxis((int)axisY, true);
        }

        private void button_YPlus_MouseUp(object sender, MouseEventArgs e)
        {
            if (axisY == AXIS.PROD_PICKUP_Y)
                Machine.interfer_release_x_safe_area = false;
            else if (axisY == AXIS.ASSEMBLER_Y)
                Machine.interfer_release_y_crash = false;
            Machine.motion.AxisStopAll(true);
            Machine.usingMoveJog = false;
        }

        private void button_YMinus_MouseDown(object sender, MouseEventArgs e)
        {
            if (axisY == AXIS.PROD_PICKUP_Y)
                Machine.interfer_release_y_crash = true;
            MoveJogAxis((int)axisY, false);
        }

        private void button_YMinus_MouseUp(object sender, MouseEventArgs e)
        {
            if (axisY == AXIS.PROD_PICKUP_Y)
                Machine.interfer_release_y_crash = false;
            Machine.motion.AxisStopAll(true);
            Machine.usingMoveJog = false;
        }

        private void button_XPlus_MouseDown(object sender, MouseEventArgs e)
        {
            if (axisX == AXIS.PROD_PICKUP_X)
                Machine.interfer_release_x_safe_area = true;
            MoveJogAxis((int)axisX, true);
        }

        private void button_XPlus_MouseUp(object sender, MouseEventArgs e)
        {
            if (axisX == AXIS.PROD_PICKUP_X)
                Machine.interfer_release_x_safe_area = false;
            Machine.motion.AxisStopAll(true);
            Machine.usingMoveJog = false;
        }

        private void button_XMinus_MouseDown(object sender, MouseEventArgs e)
        {
            if (axisX == AXIS.PROD_PICKUP_X)
                Machine.interfer_release_x_safe_area = true;
            MoveJogAxis((int)axisX, false);
        }

        private void button_XMinus_MouseUp(object sender, MouseEventArgs e)
        {
            if (axisX == AXIS.PROD_PICKUP_X)
                Machine.interfer_release_x_safe_area = false;
            Machine.motion.AxisStopAll(true);
            Machine.usingMoveJog = false;
        }

        private void button_ZPlus_MouseDown(object sender, MouseEventArgs e)
        {
            MoveJogAxis((int)axisZ, true);
        }

        private void button_ZPlus_MouseUp(object sender, MouseEventArgs e)
        {
            Machine.motion.AxisStopAll(true);
            Machine.usingMoveJog = false;
        }

        private void button_ZMinus_MouseDown(object sender, MouseEventArgs e)
        {
            MoveJogAxis((int)axisZ, false);
        }

        private void button_ZMinus_MouseUp(object sender, MouseEventArgs e)
        {
            Machine.motion.AxisStopAll(true);
            Machine.usingMoveJog = false;
        }

        private void button_ServoOnOff_Click(object sender, EventArgs e)
        {
            bool servoStatus = false;

            if (teachingInputValue1.Clicked)
            {
                Machine.motion.GetAxisServoOn((int)axisX, ref servoStatus);
                if (servoStatus)
                    Machine.motion.SetAxisServoOn((int)axisX, 0);
                else
                    Machine.motion.SetAxisServoOn((int)axisX, 1);
            }
            if (teachingInputValue2.Clicked)
            {
                Machine.motion.GetAxisServoOn((int)axisY, ref servoStatus);
                if (servoStatus)
                    Machine.motion.SetAxisServoOn((int)axisY, 0);
                else
                    Machine.motion.SetAxisServoOn((int)axisY, 1);
            }
        }

        private void button_AlarmReset_Click(object sender, EventArgs e)
        {
            for (int i = (int)axisX; i <= (int)axisY; ++i)
            {
                Machine.motion.AlarmClear(i);
            }
        }

        private void button_Tool1UpDown_Click(object sender, EventArgs e)
        {
            var proc = Machine.proclist[(int)Machine.PROCESS.SYS] as ProcessSys;
            if (proc.doorOpenState)
            {
                Dlg_MessageBox msgBox = new Dlg_MessageBox(EMESSAGEBOX.MSG,
                               string.Format("Please Close the door"));
                msgBox.ShowDialog();
                return;
            }

            if (axisX == AXIS.PROD_PICKUP_X && axisY == AXIS.PROD_PICKUP_Y)
            {
                Machine.IO.GetIn((int)DI.CAM_PICKER_Z1_UP, ref ToolZUp);

                if (ToolZUp == 1)
                {
                    LogUtil.Instance.Log(LOG_TYPE.UI, "ProdLoader Left Down Button Click", CONTENT_TYPE.INFO);

                    Machine.IO.SetOut((int)DO.CAM_PICKER_Z1_UP, 0);
                    Machine.IO.SetOut((int)DO.CAM_PICKER_Z1_DOWN, 1);
                }
                else
                {
                    LogUtil.Instance.Log(LOG_TYPE.UI, "ProdLoader Left Up Button Click", CONTENT_TYPE.INFO);

                    Machine.IO.SetOut((int)DO.CAM_PICKER_Z1_UP, 1);
                    Machine.IO.SetOut((int)DO.CAM_PICKER_Z1_DOWN, 0);
                }
            }
            else if (axisX == AXIS.ASSEMBLER_X && axisY == AXIS.ASSEMBLER_Y)
            {
                Machine.IO.GetIn((int)DI.ASSEMBLER_Z1_UP, ref ToolZUp);

                if (ToolZUp == 1)
                {
                    LogUtil.Instance.Log(LOG_TYPE.UI, "Assembler Left Down Button Click", CONTENT_TYPE.INFO);

                    Machine.IO.SetOut((int)DO.ASSEMBLER_Z1_UP, 0);
                    Machine.IO.SetOut((int)DO.ASSEMBLER_Z1_DOWN, 1);
                }
                else
                {
                    LogUtil.Instance.Log(LOG_TYPE.UI, "Assembler Left Up Button Click", CONTENT_TYPE.INFO);

                    Machine.IO.SetOut((int)DO.ASSEMBLER_Z1_UP, 1);
                    Machine.IO.SetOut((int)DO.ASSEMBLER_Z1_DOWN, 0);
                }
            }
        }

        private void button_Tool2UpDown_Click(object sender, EventArgs e)
        {
            var proc = Machine.proclist[(int)Machine.PROCESS.SYS] as ProcessSys;
            if (proc.doorOpenState)
            {
                Dlg_MessageBox msgBox = new Dlg_MessageBox(EMESSAGEBOX.MSG,
                               string.Format("Please Close the door"));
                msgBox.ShowDialog();
                return;
            }

            if (axisX == AXIS.PROD_PICKUP_X && axisY == AXIS.PROD_PICKUP_Y)
            {
                Machine.IO.GetIn((int)DI.CAM_PICKER_Z2_UP, ref ToolZUp);

                if (ToolZUp == 1)
                {
                    LogUtil.Instance.Log(LOG_TYPE.UI, "ProdLoader Right Down Button Click", CONTENT_TYPE.INFO);

                    Machine.IO.SetOut((int)DO.CAM_PICKER_Z2_UP, 0);
                    Machine.IO.SetOut((int)DO.CAM_PICKER_Z2_DOWN, 1);
                }
                else
                {
                    LogUtil.Instance.Log(LOG_TYPE.UI, "ProdLoader Right Up Button Click", CONTENT_TYPE.INFO);

                    Machine.IO.SetOut((int)DO.CAM_PICKER_Z2_UP, 1);
                    Machine.IO.SetOut((int)DO.CAM_PICKER_Z2_DOWN, 0);
                }
            }
            else if (axisX == AXIS.ASSEMBLER_X && axisY == AXIS.ASSEMBLER_Y)
            {
                Machine.IO.GetIn((int)DI.ASSEMBLER_Z2_UP, ref ToolZUp);

                if (ToolZUp == 1)
                {
                    LogUtil.Instance.Log(LOG_TYPE.UI, "Assembler Right Down Button Click", CONTENT_TYPE.INFO);

                    Machine.IO.SetOut((int)DO.ASSEMBLER_Z2_UP, 0);
                    Machine.IO.SetOut((int)DO.ASSEMBLER_Z2_DOWN, 1);
                }
                else
                {
                    LogUtil.Instance.Log(LOG_TYPE.UI, "Assembler Right Up Button Click", CONTENT_TYPE.INFO);

                    Machine.IO.SetOut((int)DO.ASSEMBLER_Z2_UP, 1);
                    Machine.IO.SetOut((int)DO.ASSEMBLER_Z2_DOWN, 0);
                }
            }
        }

        private void button_Tool1VacOnOff_Click(object sender, EventArgs e)
        {
            var proc = Machine.proclist[(int)Machine.PROCESS.SYS] as ProcessSys;
            if (proc.doorOpenState)
            {
                Dlg_MessageBox msgBox = new Dlg_MessageBox(EMESSAGEBOX.MSG,
                               string.Format("Please Close the door"));
                msgBox.ShowDialog();
                return;
            }

            if (axisX == AXIS.PROD_PICKUP_X && axisY == AXIS.PROD_PICKUP_Y)
            {
                Machine.IO.GetOut((int)DO.CAM_PICKER_Z1_VACON, ref ToolVacOn);

                if (ToolVacOn == 1)
                {
                    LogUtil.Instance.Log(LOG_TYPE.UI, "ProdLoader Left Vacuum Off Button Click", CONTENT_TYPE.INFO);

                    Machine.IO.SetOut((int)DO.CAM_PICKER_Z1_VACON, 0);
                    Machine.IO.SetOut((int)DO.CAM_PICKER_Z1_PURGE, 0);
                }
                else
                {
                    LogUtil.Instance.Log(LOG_TYPE.UI, "ProdLoader Left Vacuum On Button Click", CONTENT_TYPE.INFO);

                    Machine.IO.SetOut((int)DO.CAM_PICKER_Z1_VACON, 1);
                    Machine.IO.SetOut((int)DO.CAM_PICKER_Z1_PURGE, 0);
                }
            }
            else if (axisX == AXIS.ASSEMBLER_X && axisY == AXIS.ASSEMBLER_Y)
            {
                Machine.IO.GetOut((int)DO.ASSEMBLER_Z1_VACON, ref ToolVacOn);

                if (ToolVacOn == 1)
                {
                    LogUtil.Instance.Log(LOG_TYPE.UI, "Assembler Left Vacuum Off Button Click", CONTENT_TYPE.INFO);

                    Machine.IO.SetOut((int)DO.ASSEMBLER_Z1_VACON, 0);
                    Machine.IO.SetOut((int)DO.ASSEMBLER_Z1_PURGE, 0);
                }
                else
                {
                    LogUtil.Instance.Log(LOG_TYPE.UI, "Assembler Left Vacuum On Button Click", CONTENT_TYPE.INFO);

                    Machine.IO.SetOut((int)DO.ASSEMBLER_Z1_VACON, 1);
                    Machine.IO.SetOut((int)DO.ASSEMBLER_Z1_PURGE, 0);
                }
            }
        }

        private void button_Tool2VacOnOff_Click(object sender, EventArgs e)
        {
            var proc = Machine.proclist[(int)Machine.PROCESS.SYS] as ProcessSys;
            if (proc.doorOpenState)
            {
                Dlg_MessageBox msgBox = new Dlg_MessageBox(EMESSAGEBOX.MSG,
                               string.Format("Please Close the door"));
                msgBox.ShowDialog();
                return;
            }

            if (axisX == AXIS.PROD_PICKUP_X && axisY == AXIS.PROD_PICKUP_Y)
            {
                Machine.IO.GetOut((int)DO.CAM_PICKER_Z2_VACON, ref ToolVacOn);

                if (ToolVacOn == 1)
                {
                    LogUtil.Instance.Log(LOG_TYPE.UI, "ProdLoader Right Vacuum Off Button Click", CONTENT_TYPE.INFO);

                    Machine.IO.SetOut((int)DO.CAM_PICKER_Z2_VACON, 0);
                    Machine.IO.SetOut((int)DO.CAM_PICKER_Z2_PURGE, 0);
                }
                else
                {
                    LogUtil.Instance.Log(LOG_TYPE.UI, "ProdLoader Right Vacuum On Button Click", CONTENT_TYPE.INFO);

                    Machine.IO.SetOut((int)DO.CAM_PICKER_Z2_VACON, 1);
                    Machine.IO.SetOut((int)DO.CAM_PICKER_Z2_PURGE, 0);
                }
            }
            else if (axisX == AXIS.ASSEMBLER_X && axisY == AXIS.ASSEMBLER_Y)
            {
                Machine.IO.GetOut((int)DO.ASSEMBLER_Z2_VACON, ref ToolVacOn);

                if (ToolVacOn == 1)
                {
                    LogUtil.Instance.Log(LOG_TYPE.UI, "Assembler Right Vacuum Off Button Click", CONTENT_TYPE.INFO);

                    Machine.IO.SetOut((int)DO.ASSEMBLER_Z2_VACON, 0);
                    Machine.IO.SetOut((int)DO.ASSEMBLER_Z2_PURGE, 0);
                }
                else
                {
                    LogUtil.Instance.Log(LOG_TYPE.UI, "Assembler Right Vacuum On Button Click", CONTENT_TYPE.INFO);

                    Machine.IO.SetOut((int)DO.ASSEMBLER_Z2_VACON, 1);
                    Machine.IO.SetOut((int)DO.ASSEMBLER_Z2_PURGE, 0);
                }
            }
        }
        #endregion

        #region EXTRA_FUNCTION
        private async void SaveNewPos(double dNewPos, AXIS_DIRECTION2 dir)
        {
            Action action = null;
            double dOldPos = 0; string posName = "";
            action = new Action(delegate
            {
                switch (dir)
                {
                    case AXIS_DIRECTION2.X:
                        teachingInputValue1.SetTargetPos(dNewPos);
                        posName = Machine.recipe.CalibrationNAME(tagX);
                        dOldPos = Machine.recipe.Calibration(tagX);
                        Machine.recipe.Calibration(tagX, dNewPos);
                        break;
                    case AXIS_DIRECTION2.Y:
                        teachingInputValue2.SetTargetPos(dNewPos);
                        posName = Machine.recipe.CalibrationNAME(tagY);
                        dOldPos = Machine.recipe.Calibration(tagY);
                        Machine.recipe.Calibration(tagY, dNewPos);
                        break;
                }
                Machine.recipe.Write();
                string unit = Machine.recipe.position.Unit(0); //"mm"
                string logText = $"[{posName}] data has changed. [{dOldPos} {unit} → {dNewPos} {unit}]";
                LogUtil.Instance.Log(LOG_TYPE.DATA, logText, CONTENT_TYPE.INFO);
            });

            if (action != null)
            {
                await Task.Run(action);
            }
        }
        #endregion
        private void tbxSpeedValue1_DoubleClick(object sender, EventArgs e)
        {
            string[] value = { tbxSpeedValue1.Text };
            SubForm_TenKey keyPad = new SubForm_TenKey(ref value, "XY Vel Set", "Vel Set", 0, 255, 300, 300, true);
            keyPad.ShowDialog();
            tbxSpeedValue1.Text = value[0];
        }

        private void tbxSpeedValue2_DoubleClick(object sender, EventArgs e)
        {
            string[] value = { tbxSpeedValue2.Text };
            SubForm_TenKey keyPad = new SubForm_TenKey(ref value, "RZ Vel Set", "Vel Set", 0, 255, 300, 300, true);
            keyPad.ShowDialog();
            tbxSpeedValue2.Text = value[0];
        }
        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl1.SelectedIndex == 0)
            {
                mCurrentTeachingPoint = mLeftTeachingPoint;
            }
            else if (tabControl1.SelectedIndex == 1)
            {
                mCurrentTeachingPoint = mRightTeachingPoint;
            }
            SelectButton_ClickEvent(listTeachingPointBtn[(int)mCurrentTeachingPoint], null);
            return;
        }
    }
}
