using NV_UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
//using System.Data;
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
using static CAMASSEMBLYMACHINE.Define.ParameterDefine;

namespace CAMASSEMBLYMACHINE.UI
{
    public partial class tabBufferTeach2 : Form, IForm
    {
        public enum TrayPAGE
        {
            UNKNOWN = -1,
            PP
        }

        enum TEACHING_TASK
        {
            NONE,
            MOVE_TARGET,
            SAVE_CURRENT,
            SAVE_TARGET,

            SET_VEL_SLOW,
            SET_VEL_NORMAL,
            SET_VEL_FAST
        }

        public enum BufferStation
        {
            LEFT = 0,
            RIGHT = 1
        }

        private TEACHING_TASK eCurrentTask;
        public delegate void BtnEnableCallbackEvent(bool bEnable);
        public event BtnEnableCallbackEvent btnEnableCallbackEvent;
        public BTN_TAB_SELECT mCurrentTeachingPoint;
        private bool bPrevCmdVacOnOrOff;
        //private double[] dTargetValue;
        private double[] dTargetValue2;

        uint ToolZUp = 0;
        uint ToolVacOn = 0;

        TrayPAGE curPage = TrayPAGE.UNKNOWN;
        public BTN_TAB_SELECT mTeachingPoint = BTN_TAB_SELECT.Y_TRANSFER_READY;
        public List<NV_UI.NV_Button_TG_NS> listTeachingPointBtn = new List<NV_UI.NV_Button_TG_NS>();

        double maxY;
        double minY;
        RecipeDefine.POSITION curY;
        BufferStation station = BufferStation.LEFT;
        public tabBufferTeach2(BufferStation station)
        {
            InitializeComponent();
            InitializeVariable();

            this.station = station;
            SetBtnMouseEvents(FunctionBtnClickEvent);
        }
        private void InitializeVariable()
        {
            this.curPage = TrayPAGE.PP;
            this.eCurrentTask = TEACHING_TASK.NONE;
            this.mCurrentTeachingPoint = BTN_TAB_SELECT.Y_TRANSFER_READY;
            //this.dTargetValue = new double[Enum.GetValues(typeof(AXIS_DIRECTION)).Length];
            this.dTargetValue2 = new double[Enum.GetValues(typeof(AXIS_DIRECTION2)).Length];

            tbxSpeedValue1.Text = (UIDefine.UIJOGVEL[(int)UIDefine.UIJOGVELINFO.NORMAL] / 1000.0).ToString();
        }

        public void StartTimer(bool enable)
        {
            if (enable)
            {
                timer1.Start();
                UpdateTeachingValue();
            }
            else
            {
                timer1.Stop();
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (curPage == TrayPAGE.PP)
            {
                UpdateIOState();
                UpdateAxisState();
                UpdateServoState();
                UpdateTargetPosState();
            }
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
        }

        #region Update Status
        private void UpdateServoState()
        {
            bool ServoStatus = true;
            bool ServoStatusCheck = true;
            bool AlarmStatus = false;
            bool AlarmStatusCheck = false;

            if (teachingInputValue2.Clicked)
            {
                if (station == BufferStation.LEFT)
                {
                    Machine.motion.GetAxisServoOn((int)AXIS.CAM_TRANSFER_LEFT_Y, ref ServoStatusCheck);
                    if (!ServoStatusCheck)
                        ServoStatus = false;
                    Machine.motion.GetAlarm((int)AXIS.CAM_TRANSFER_LEFT_Y, ref AlarmStatus);
                    if (AlarmStatusCheck)
                        AlarmStatus = true;
                }
                else if (station == BufferStation.RIGHT)
                {
                    Machine.motion.GetAxisServoOn((int)AXIS.CAM_TRANSFER_RIGHT_Y, ref ServoStatusCheck);
                    if (!ServoStatusCheck)
                        ServoStatus = false;
                    Machine.motion.GetAlarm((int)AXIS.CAM_TRANSFER_RIGHT_Y, ref AlarmStatus);
                    if (AlarmStatusCheck)
                        AlarmStatus = true;
                }
            }
        }

        private void UpdateIOState()
        {
            
        }

        private void UpdateAxisState()
        {
            double pos = 0;
            double actualPos = 0;
            bool result;
            bool isHW_POSLimit = false; bool isHW_NEGLimit = false;
            HomeState isHW_POSHome = HomeState.HomeStateNEED;

            if (station == BufferStation.LEFT) //Left
            {
                Machine.motion.GetAxisActualPos((int)AXIS.CAM_TRANSFER_LEFT_Y, ref actualPos);
                teachingInputValue2.CurrentPos = actualPos / 1000.0;

                Machine.motion.GetAxis_POSLimit((int)AXIS.CAM_TRANSFER_LEFT_Y, ref isHW_POSLimit);
                Machine.motion.GetAxis_NEGLimit((int)AXIS.CAM_TRANSFER_LEFT_Y, ref isHW_NEGLimit);
                Machine.motion.GetAxisHomeComplete((int)AXIS.CAM_TRANSFER_LEFT_Y, ref isHW_POSHome);
                
                teachingInputValue2.LimitPlusBackColor = isHW_POSLimit == true ? Color.Red : Color.White;
                teachingInputValue2.LimitMinusBackColor = isHW_NEGLimit == true ? Color.Red : Color.White;
                teachingInputValue2.HomeBackColor = isHW_POSHome == HomeState.HomeStateDONE ? Color.Lime : Color.Red;
            }
            else if (station == BufferStation.RIGHT) //Right
            {
                Machine.motion.GetAxisActualPos((int)AXIS.CAM_TRANSFER_RIGHT_Y, ref actualPos);
                teachingInputValue2.CurrentPos = actualPos / 1000.0;
                Machine.motion.GetAxisHomeComplete((int)AXIS.ASSEMBLER_X, ref isHW_POSHome);

                Machine.motion.GetAxis_POSLimit((int)AXIS.CAM_TRANSFER_RIGHT_Y, ref isHW_POSLimit);
                Machine.motion.GetAxis_NEGLimit((int)AXIS.CAM_TRANSFER_RIGHT_Y, ref isHW_NEGLimit);
                Machine.motion.GetAxisHomeComplete((int)AXIS.CAM_TRANSFER_RIGHT_Y, ref isHW_POSHome);

                teachingInputValue2.LimitPlusBackColor = isHW_POSLimit == true ? Color.Red : Color.White;
                teachingInputValue2.LimitMinusBackColor = isHW_NEGLimit == true ? Color.Red : Color.White;
                teachingInputValue2.HomeBackColor = isHW_POSHome == HomeState.HomeStateDONE ? Color.Lime : Color.Red;
            }
        }

        private void UpdateTargetPosState()
        {
            double targetPosCur = 0;
            double targetPosSet = 0;

            targetPosCur = teachingInputValue2.TargetPos;
            targetPosSet = Machine.recipe.Position(curY);
            teachingInputValue2.TargetPosBackColor = (Math.Abs(targetPosSet - targetPosCur) < 0.01) == true ? Color.White : Color.Yellow;
        }
        #endregion

        #region TEACHING_POS_SELECTED_FUNCTION
        public enum BTN_TAB_SELECT
        {
            Y_TRANSFER_READY,
            Y_TRANSFER_LOADING,
            Y_TRANSFER_PEELING,
            Y_TRANSFER_UNLOADING,
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
                    //Left
                    case BTN_TAB_SELECT.Y_TRANSFER_READY:
                        if (station == BufferStation.LEFT)
                            curY = RecipeDefine.POSITION.CAM_TRANSFER_Y_READY_L_POS;
                        if (station == BufferStation.RIGHT)
                            curY = RecipeDefine.POSITION.CAM_TRANSFER_Y_READY_R_POS;
                        break;         
                    case BTN_TAB_SELECT.Y_TRANSFER_LOADING:
                        if (station == BufferStation.LEFT)
                            curY = RecipeDefine.POSITION.CAM_TRANSFER_Y_LOADING_L_POS;
                        if (station == BufferStation.RIGHT)
                            curY = RecipeDefine.POSITION.CAM_TRANSFER_Y_LOADING_R_POS;
                        break;
                    case BTN_TAB_SELECT.Y_TRANSFER_PEELING:
                        if (station == BufferStation.LEFT)
                            curY = RecipeDefine.POSITION.CAM_TRANSFER_Y_PEELING_L_POS;
                        if (station == BufferStation.RIGHT)
                            curY = RecipeDefine.POSITION.CAM_TRANSFER_Y_PEELING_R_POS;
                        break;
                    case BTN_TAB_SELECT.Y_TRANSFER_UNLOADING:
                        if (station == BufferStation.LEFT)
                            curY = RecipeDefine.POSITION.CAM_TRANSFER_Y_UNLOADING_L_POS;
                        if (station == BufferStation.RIGHT)
                            curY = RecipeDefine.POSITION.CAM_TRANSFER_Y_UNLOADING_R_POS;
                        break;
                }

                teachingInputValue2.Visible = true;
                button_YPlus.Enabled = true;
                button_YMinus.Enabled = true;

                dTargetValue2[(int)AXIS_DIRECTION2.Y] = 1000.0 * Machine.recipe.Position(curY);

                teachingInputValue2.TargetPos = dTargetValue2[(int)AXIS_DIRECTION2.Y] / 1000.0;

                maxY = Machine.recipe.PositionMAX(curY);
                minY = Machine.recipe.PositionMIN(curY);

                if (tabControl1.SelectedIndex == 0)
                {
                    mTeachingPoint = mCurrentTeachingPoint;
                }
            }
        }
        bool firstStart = true;

        private void Tab_Select_First_Shown(object sender, EventArgs e)
        {
            if (firstStart)
            {
                SelectButton_ClickEvent(nV_Button_TG_NS_YReady, null);
                firstStart = false;
            }
        }
        #endregion
        #region BUTTON_CLICK_FUNCTION
        public void SetBtnMouseEvents(NV_UI.NV_Button_PB_NS.ClickEventDelegate functionBtnEvent)
        {
            nV_Button_TG_NS_YReady.ClickEvent += SelectButton_ClickEvent;
            nV_Button_TG_NS_YLoading.ClickEvent += SelectButton_ClickEvent;
            nV_Button_TG_NS_YPeeling.ClickEvent += SelectButton_ClickEvent;
            nV_Button_TG_NS_YUnloading.ClickEvent += SelectButton_ClickEvent;

            listTeachingPointBtn.Add(nV_Button_TG_NS_YReady);
            listTeachingPointBtn.Add(nV_Button_TG_NS_YLoading);
            listTeachingPointBtn.Add(nV_Button_TG_NS_YPeeling);
            listTeachingPointBtn.Add(nV_Button_TG_NS_YUnloading);

            nV_Button_PB_NS_MoveTarget.ClickEvent += functionBtnEvent;
            nV_Button_PB_NS_SaveCurrentPos.ClickEvent += functionBtnEvent;
            nV_Button_PB_NS_SaveTargetPos.ClickEvent += functionBtnEvent;
            nV_Button_PB_NS_SetVelSlow.ClickEvent += functionBtnEvent;
            nV_Button_PB_NS_SetVelNormal.ClickEvent += functionBtnEvent;
            nV_Button_PB_NS_SetVelFast.ClickEvent += functionBtnEvent;

            teachingInputValue2.clickEvent += TargetInputClickEvent;
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
                case TEACHING_TASK.MOVE_TARGET:
                    LogUtil.Instance.Log(LOG_TYPE.UI, "Tray Move Target Button Click", CONTENT_TYPE.INFO);

                    Dlg_MessageBox formMove = new Dlg_MessageBox(EMESSAGEBOX.WARNING, string.Format("축 이동 하시겠습니까?"));
                    if (formMove.ShowDialog() == DialogResult.Yes)
                    {
                        if (teachingInputValue2.Clicked)
                            MoveTarget(AXIS_DIRECTION2.Y);
                        break;
                    }
                    break;
                case TEACHING_TASK.SAVE_CURRENT:
                    RecipeDefine.POSITION dCurDir = curY;
                    double dCurNewPos = 0;
                    if (teachingInputValue2.Clicked)
                    {
                        dCurDir = curY; 
                        dCurNewPos = (double)teachingInputValue2.CurrentPos;
                    }
                    else
                        return;
                    double dDifCur = dCurNewPos - Machine.recipe.Position(dCurDir);
                    Dlg_MessageBox formCurrent = new Dlg_MessageBox(EMESSAGEBOX.WARNING, string.Format(dDifCur.ToString("N3") + " 만큼 차이가 발생했습니다.\n" + "Current 값을 저장하시겠습니까?"));
                    if (formCurrent.ShowDialog() == DialogResult.Yes)
                    {
                        if (teachingInputValue2.Clicked) SaveNewPos((double)teachingInputValue2.CurrentPos, AXIS_DIRECTION2.Y);
                    }
                    break;
                case TEACHING_TASK.SAVE_TARGET:
                    RecipeDefine.POSITION dTarDir;
                    double dTarNewPos = 0;
                    if (teachingInputValue2.Clicked)
                    {
                        dTarDir = curY; dTarNewPos = (double)teachingInputValue2.TargetPos;
                    }
                    else
                        return;
                    double dDifTar = dTarNewPos - Machine.recipe.Position(dTarDir);
                    Dlg_MessageBox formTarget = new Dlg_MessageBox(EMESSAGEBOX.WARNING, string.Format(dDifTar.ToString("N3") + " 만큼 차이가 발생했습니다.\n" + "Target 값을 저장하시겠습니까?"));
                    if (formTarget.ShowDialog() == DialogResult.Yes)
                    {
                        if (teachingInputValue2.Clicked) SaveNewPos((double)teachingInputValue2.TargetPos, AXIS_DIRECTION2.Y);
                    }
                    break;
                case TEACHING_TASK.SET_VEL_SLOW:
                    tbxSpeedValue1.Text = (UIDefine.UIJOGVEL[(int)UIDefine.UIJOGVELINFO.SLOWER] / 1000.0).ToString();
                    break;
                case TEACHING_TASK.SET_VEL_NORMAL:
                    tbxSpeedValue1.Text = (UIDefine.UIJOGVEL[(int)UIDefine.UIJOGVELINFO.NORMAL] / 1000.0).ToString();
                    break;
                case TEACHING_TASK.SET_VEL_FAST:
                    tbxSpeedValue1.Text = (UIDefine.UIJOGVEL[(int)UIDefine.UIJOGVELINFO.FASTER] / 1000.0).ToString();
                    break;
            }
        }
        private async void MoveTarget(AXIS_DIRECTION2 Pos)
        {
            if (Machine.status.state == SystemState.SystemStateERROR)
            {
                Dlg_MessageBox formErr = new Dlg_MessageBox(EMESSAGEBOX.MSG, string.Format("Please push the reset button"));
                formErr.TopLevel = true;
                formErr.TopMost = true;
                formErr.ShowDialog();
                return;
            }

            var procSys = Machine.proclist[(int)Machine.PROCESS.SYS] as ProcessSys;
            if (procSys.doorOpenState)
            {
                Dlg_MessageBox msgBox = new Dlg_MessageBox(EMESSAGEBOX.MSG,
                               string.Format("Please Close the door"));
                msgBox.ShowDialog();
                return;
            }

            if (station == BufferStation.LEFT)
            {
                if (!IsOkayBufLMove())
                    return;
            }

            else if (station == BufferStation.RIGHT)
            {
                if (!IsOkayBufRMove())
                    return;
            }

            Action action = null;
            int errcode = 0;
            int iAxisdx = 0;
            int iAxisdx2 = 0;

            double vel_y = 1000 * Machine.param.Velocity(ParameterDefine.VELOCITY.CAM_TRANSFER_Y_MOVE_VEL);
            //double vel_y2 = 1000 * Machine.param.Velocity(ParameterDefine.VELOCITY.CAM_TRANSFER_Y_APPROACH_VEL);
            double acc_y = 1000 * Machine.param.Velocity(ParameterDefine.VELOCITY.CAM_TRANSFER_Y_MOVE_ACC);
            double dec_y = 1000 * Machine.param.Velocity(ParameterDefine.VELOCITY.CAM_TRANSFER_Y_MOVE_DEC);
            double pos = 0.0;

            action = new Action(delegate
            {
                switch (Pos)
                {
                    case AXIS_DIRECTION2.Y:
                        if (station == BufferStation.LEFT)
                            iAxisdx = (int)AXIS.CAM_TRANSFER_LEFT_Y;
                        else if (station == BufferStation.RIGHT)
                            iAxisdx = (int)AXIS.CAM_TRANSFER_RIGHT_Y;

                        dTargetValue2[(int)Pos] = teachingInputValue2.TargetPos * 1000.0;
                        Machine.motion.MoveAxisAbs(iAxisdx, dTargetValue2[(int)Pos], vel_y, acc_y, dec_y);
                        break;
                }
            });

            if (action != null)
            {
                await Task.Run(action);
            }
            return;
        }

        private void TargetInputClickEvent(object sender, EventArgs e)
        {
            TeachingInputValue input = (sender as TeachingInputValue);
            if (input != null)
            {
                string[] value = { input.textBoxTargetPos.Text };
                string sTag = input.Tag.ToString();
                double max = 100000; double min = 0;
                switch (sTag)
                {
                    case "InputY":
                        min = minY; max = maxY;
                        break;
                }
                SubForm_TenKey keyPad = new SubForm_TenKey(ref value, "Target Pos Set", "Pos Set", min, max, 300, 300, true);
                keyPad.ShowDialog();
                input.textBoxTargetPos.Text = value[0];
            }
        }

        public string SpeedValueXY
        {
            get { return tbxSpeedValue1.Text; }
            set { tbxSpeedValue1.Text = value; }
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

            switch (axis)
            {
                case (int)AXIS.CAM_TRANSFER_LEFT_Y:
                case (int)AXIS.CAM_TRANSFER_RIGHT_Y:
                    if (!double.TryParse(SpeedValueXY, out dSpeed))
                    {
                        MessageBox.Show("Invalid Speed Value", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    dSpeed = Math.Abs(dSpeed) * 1000;

                    if (dSpeed > MaxSpeedXY)
                    {
                        MessageBox.Show("Max Speed 1000 mm/s. Please Change Value.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }

                    if (dSpeed < MinSpeedXY)
                    {
                        MessageBox.Show("Min Speed 1 mm/s. Please Change Value.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }

                    double acceleration = 10 * dSpeed;
                    double delceleration = 10 * dSpeed;

                    if (!direction)
                        dSpeed *= (-1.0);

                    Machine.motion.MoveAxisJog(axis, dSpeed, acceleration, delceleration);
                    Machine.usingMoveJog = true;
                    break;                
            }
        }

        private void button_YPlus_MouseDown(object sender, MouseEventArgs e)
        {
            if (station == BufferStation.LEFT)
            {
                if (!IsOkayBufLMove())
                    return;

                MoveJogAxis((int)AXIS.CAM_TRANSFER_LEFT_Y, true);
            }

            else if (station == BufferStation.RIGHT)
            {
                if (!IsOkayBufRMove())
                    return;

                MoveJogAxis((int)AXIS.CAM_TRANSFER_RIGHT_Y, true);
            }
        }

        private void button_YPlus_MouseUp(object sender, MouseEventArgs e)
        {
            Machine.motion.AxisStopAll(true);
            Machine.usingMoveJog = false;
        }

        private void button_YMinus_MouseDown(object sender, MouseEventArgs e)
        {
            if (station == BufferStation.LEFT)
            {
                if (!IsOkayBufLMove())
                    return;

                MoveJogAxis((int)AXIS.CAM_TRANSFER_LEFT_Y, false);
            }

            else if (station == BufferStation.RIGHT)
            {
                if (!IsOkayBufRMove())
                    return;

                MoveJogAxis((int)AXIS.CAM_TRANSFER_RIGHT_Y, false);
            }
        }

        private void button_YMinus_MouseUp(object sender, MouseEventArgs e)
        {
            Machine.motion.AxisStopAll(true);
            Machine.usingMoveJog = false;
        }

        private void button_ServoOnOff_Click(object sender, EventArgs e)
        {
            bool servoStatus = false;

            if (teachingInputValue2.Clicked)
            {
                if (station == BufferStation.LEFT)
                {
                    Machine.motion.GetAxisServoOn((int)AXIS.CAM_TRANSFER_LEFT_Y, ref servoStatus);
                    if (servoStatus)
                        Machine.motion.SetAxisServoOn((int)AXIS.CAM_TRANSFER_LEFT_Y, 0);
                    else
                        Machine.motion.SetAxisServoOn((int)AXIS.CAM_TRANSFER_LEFT_Y, 1);
                }
                else if (station == BufferStation.RIGHT)
                {
                    Machine.motion.GetAxisServoOn((int)AXIS.CAM_TRANSFER_RIGHT_Y, ref servoStatus);
                    if (servoStatus)
                        Machine.motion.SetAxisServoOn((int)AXIS.CAM_TRANSFER_RIGHT_Y, 0);
                    else
                        Machine.motion.SetAxisServoOn((int)AXIS.CAM_TRANSFER_RIGHT_Y, 1);
                }
            }
        }

        private void button_AlarmReset_Click(object sender, EventArgs e)
        {
            for (int i = (int)AXIS.CAM_TRANSFER_LEFT_Y; i <= (int)AXIS.CAM_TRANSFER_RIGHT_Y; ++i)
            {
                Machine.motion.AlarmClear((int)AXIS.AXISMAX);
            }
        }

        private void button_TRF1ClampUnclamp_Click(object sender, EventArgs e)
        {
            
        }

        private void button_TRF2ClampUnclamp_Click(object sender, EventArgs e)
        {
            
        }

        private void button_Tool1VacOnOff_Click(object sender, EventArgs e)
        {
            
        }

        private void button_Tool2VacOnOff_Click(object sender, EventArgs e)
        {
            
        }

        private void button_TRFUpDown_Click(object sender, EventArgs e)
        {
            
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
                    case AXIS_DIRECTION2.Y:
                        teachingInputValue2.SetTargetPos(dNewPos);
                        posName = Machine.recipe.PositionNAME(curY);
                        dOldPos = Machine.recipe.Position(curY);
                        Machine.recipe.Position(curY, dNewPos);
                        break;
                }
                Machine.recipe.Write();
                string unit = Machine.recipe.PositionUNIT(curY); //"mm"
                string logText = $"[{posName}] data has changed. [{dOldPos} {unit} → {dNewPos} {unit}]";
                LogUtil.Instance.Log(LOG_TYPE.DATA, logText, CONTENT_TYPE.INFO);
            });

            if (action != null)
            {
                await Task.Run(action);
            }
        }
        #endregion

        private void tabControl2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl2.SelectedIndex == 0) //PP
            {
                curPage = TrayPAGE.PP;
            }
        }

        private void tbxSpeedValue1_DoubleClick(object sender, EventArgs e)
        {
            string[] value = { tbxSpeedValue1.Text };
            SubForm_TenKey keyPad = new SubForm_TenKey(ref value, "XY Vel Set", "Vel Set", 0, 255, 300, 300, false);
            keyPad.ShowDialog();
            tbxSpeedValue1.Text = value[0];
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl1.SelectedIndex == 0)
            {
                mCurrentTeachingPoint = mTeachingPoint;
            }
            SelectButton_ClickEvent(listTeachingPointBtn[(int)mCurrentTeachingPoint], null);
            return;
        }

        public bool IsOkayBufLMove()
        {
            //bool isOkay = true;
            //string errorMsg = "";

            //uint ret1 = 0;
            //Machine.IO.GetIn((int)DI.CAM_TRF_L_Z_UP, ref ret1);
            //if (ret1 == 0)
            //{
            //    errorMsg = string.Format("Please Set Left Transfer Up"); isOkay = false;
            //}

            //if (!isOkay)
            //{
            //    Dlg_MessageBox msgBox = new Dlg_MessageBox(EMESSAGEBOX.MSG,
            //                     errorMsg);
            //    msgBox.ShowDialog();
            //    return false;
            //}
            return true;
        }

        public bool IsOkayBufRMove()
        {
            //bool isOkay = true;
            //string errorMsg = "";

            //uint ret1 = 0;
            //Machine.IO.GetIn((int)DI.CAM_TRF_R_Z_UP, ref ret1);
            //if (ret1 == 0)
            //{
            //    errorMsg = string.Format("Please Set Right Transfer Up"); isOkay = false;
            //}

            //if (!isOkay)
            //{
            //    Dlg_MessageBox msgBox = new Dlg_MessageBox(EMESSAGEBOX.MSG,
            //                     errorMsg);
            //    msgBox.ShowDialog();
            //    return false;
            //}

            return true;
        }

        void UpdateTeachingValue()
        {
            if (tabControl2.SelectedIndex == 0) //PP
                SelectButton_ClickEvent(listTeachingPointBtn[(int)mCurrentTeachingPoint], null);
        }
    }
}
