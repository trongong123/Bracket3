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
    public partial class tabJigTeach : Form, IForm
    {
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

        private TEACHING_TASK eCurrentTask;
        public delegate void BtnEnableCallbackEvent(bool bEnable);
        public event BtnEnableCallbackEvent btnEnableCallbackEvent;
        public BTN_TAB_SELECT mCurrentTeachingPoint;
        private bool bPrevCmdVacOnOrOff;
        //private double[] dTargetValue;
        private double[] dTargetValue2;

        uint ToolZUp = 0;
        uint ToolVacOn = 0;

        public BTN_TAB_SELECT mLeftTeachingPoint = BTN_TAB_SELECT.Z_JIG_IN_BUF_LOADING;
        public List<NV_UI.NV_Button_TG_NS> listTeachingPointBtn = new List<NV_UI.NV_Button_TG_NS>();
        double maxZ;
        double minZ;
        ParameterDefine.POSITION curZ;
        public tabJigTeach()
        {
            InitializeComponent();
            InitializeVariable();
            SetBtnMouseEvents(FunctionBtnClickEvent);
        }
        private void InitializeVariable()
        {
            this.eCurrentTask = TEACHING_TASK.NONE;
            this.mCurrentTeachingPoint = BTN_TAB_SELECT.Z_JIG_IN_BUF_LOADING;
            //this.dTargetValue = new double[Enum.GetValues(typeof(AXIS_DIRECTION)).Length];
            this.dTargetValue2 = new double[Enum.GetValues(typeof(AXIS_DIRECTION2)).Length];

            tbxSpeedValue2.Text = (UIDefine.UIJOGVEL[(int)UIDefine.UIJOGVELINFO.NORMAL] / 1000.0).ToString();
        }

        public void StartTimer(bool enable)
        {
            if (enable)
            {
                timer1.Start();
            }
            else
            {
                timer1.Stop();
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            UpdateIOState();
            UpdateAxisState();
            UpdateServoState();
            UpdateTargetPosState();
        }

        private void SwitchOffOtherButtons(object sender)
        {
            string sTag = (sender as Control).Tag.ToString();
            string compareTag = string.Empty;
            foreach (Control ctrl in tableLayoutPanelButtons1.Controls)
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


            if (teachingInputValue5.Clicked)
            {
                if(tabControl1.SelectedIndex == 0)
                {
                    Machine.motion.GetAxisServoOn((int)AXIS.JIG_IN_BUF_Z, ref ServoStatusCheck);
                    if (!ServoStatusCheck)
                        ServoStatus = false;
                    Machine.motion.GetAlarm((int)AXIS.JIG_IN_BUF_Z, ref AlarmStatus);
                    if (AlarmStatusCheck)
                        AlarmStatus = true;
                }
                else if (tabControl1.SelectedIndex == 1)
                {
                    Machine.motion.GetAxisServoOn((int)AXIS.JIG_IN_BUF_Z, ref ServoStatus);
                    if (!ServoStatusCheck)
                        ServoStatus = false;
                    Machine.motion.GetAlarm((int)AXIS.JIG_IN_BUF_Z, ref AlarmStatus);
                    if (AlarmStatusCheck)
                        AlarmStatus = true;
                }
            }
        }

        private void UpdateIOState()
        {
            uint returnValue = 0;
        }

        private void UpdateAxisState()
        {
            double pos = 0;
            double actualPos = 0;
            bool result;
            double curTargetPos = 0;
            bool result2;
            bool isHW_POSLimit = false; bool isHW_NEGLimit = false;
            HomeState isHW_POSHome = HomeState.HomeStateNEED;

            Machine.motion.GetAxisActualPos((int)AXIS.JIG_IN_BUF_Z, ref actualPos);
            teachingInputValue5.CurrentPos = actualPos / 1000.0;

            Machine.motion.GetAxis_POSLimit((int)AXIS.JIG_IN_BUF_Z, ref isHW_POSLimit);
            Machine.motion.GetAxis_NEGLimit((int)AXIS.JIG_IN_BUF_Z, ref isHW_NEGLimit);
            Machine.motion.GetAxisHomeComplete((int)AXIS.JIG_IN_BUF_Z, ref isHW_POSHome);

            teachingInputValue5.LimitPlusBackColor = isHW_POSLimit == true ? Color.Red : Color.White;
            teachingInputValue5.LimitMinusBackColor = isHW_NEGLimit == true ? Color.Red : Color.White;
            teachingInputValue5.HomeBackColor = isHW_POSHome == HomeState.HomeStateDONE ? Color.Lime : Color.Red;

            //result = true;
            //pos = 1000 * Machine.param.Position(ParameterDefine.POSITION.JIG_IN_BUF_LOAD_POS);
            //if (Math.Abs(pos - actualPos) > 5) result = false;
            //disp_JigLiftLDPos.ButtonPush = result == true ? true : false;

            //result = true;
            //pos = 1000 * Machine.param.Position(ParameterDefine.POSITION.JIG_IN_BUF_UNLOAD_POS);
            //if (Math.Abs(pos - actualPos) > 5) result = false;
            //disp_JigLiftULDPos.ButtonPush = result == true ? true : false;
        }
        
        private void UpdateTargetPosState()
        {
            double targetPosSet = 0;
            double targetPosCur = 0;

            //if (this.mCurrentTeachingPoint == BTN_TAB_SELECT.Z_JIG_IN_BUF_LOADING)
            //{
            //    // PICKUP POSITION
            //    targetPosCur = teachingInputValue5.TargetPos;
            //    targetPosSet = Machine.param.Position(ParameterDefine.POSITION.JIG_IN_BUF_LOAD_POS);
            //    teachingInputValue5.TargetPosBackColor = (Math.Abs(targetPosSet - targetPosCur) < 0.01) == true ? Color.White : Color.Yellow;
            //}

            //else if (this.mCurrentTeachingPoint == BTN_TAB_SELECT.Z_JIG_IN_BUF_UNLOADING)
            //{
            //    targetPosCur = teachingInputValue5.TargetPos;
            //    targetPosSet = Machine.param.Position(ParameterDefine.POSITION.JIG_IN_BUF_UNLOAD_POS);
            //    teachingInputValue5.TargetPosBackColor = (Math.Abs(targetPosSet - targetPosCur) < 0.01) == true ? Color.White : Color.Yellow;
            //}
        }

        #endregion

        #region TEACHING_POS_SELECTED_FUNCTION
        public enum BTN_TAB_SELECT
        {
            Z_JIG_IN_BUF_LOADING,
            Z_JIG_IN_BUF_UNLOADING,
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
                    case BTN_TAB_SELECT.Z_JIG_IN_BUF_LOADING:
                        //curZ = ParameterDefine.POSITION.JIG_IN_BUF_LOAD_POS;

                        teachingInputValue5.Visible = true;
                        button_ZPlus.Enabled = true;
                        button_ZMinus.Enabled = true;
                        break;
                    case BTN_TAB_SELECT.Z_JIG_IN_BUF_UNLOADING:
                        //curZ = ParameterDefine.POSITION.JIG_IN_BUF_UNLOAD_POS;

                        teachingInputValue5.Visible = true;
                        button_ZPlus.Enabled = true;
                        button_ZMinus.Enabled = true;
                        break;
                }

                dTargetValue2[(int)AXIS_DIRECTION2.Z] = 1000.0 * Machine.param.Position(curZ);

                teachingInputValue5.TargetPos = dTargetValue2[(int)AXIS_DIRECTION2.Z] / 1000.0;

                maxZ = Machine.param.PositionMAX(curZ);

                minZ = Machine.param.PositionMIN(curZ);

                if (tabControl1.SelectedIndex == 0)
                {
                    mLeftTeachingPoint = mCurrentTeachingPoint;
                }
            }
        }
        bool firstStart = true;

        private void Tab_Select_First_Shown(object sender, EventArgs e)
        {
            if (firstStart)
            {
                SelectButton_ClickEvent(nV_Button_TG_NS_ZJigInBufLoading, null);
                firstStart = false;
            }
        }
        #endregion
        #region BUTTON_CLICK_FUNCTION
        public void SetBtnMouseEvents(NV_UI.NV_Button_PB_NS.ClickEventDelegate functionBtnEvent)
        {
            nV_Button_TG_NS_ZJigInBufLoading.ClickEvent += SelectButton_ClickEvent;
            nV_Button_TG_NS_ZJigInBufUnLoading.ClickEvent += SelectButton_ClickEvent;

            listTeachingPointBtn.Add(nV_Button_TG_NS_ZJigInBufLoading);
            listTeachingPointBtn.Add(nV_Button_TG_NS_ZJigInBufUnLoading);

            nV_Button_PB_NS_MoveTarget.ClickEvent += functionBtnEvent;
            nV_Button_PB_NS_SaveCurrentPos.ClickEvent += functionBtnEvent;
            nV_Button_PB_NS_SaveTargetPos.ClickEvent += functionBtnEvent;
            nV_Button_PB_NS_SetVelSlow.ClickEvent += functionBtnEvent;
            nV_Button_PB_NS_SetVelNormal.ClickEvent += functionBtnEvent;
            nV_Button_PB_NS_SetVelFast.ClickEvent += functionBtnEvent;

            teachingInputValue5.clickEvent += TargetInputClickEvent;
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
                    LogUtil.Instance.Log(LOG_TYPE.UI, "Jig Move Target Button Click", CONTENT_TYPE.INFO);

                    Dlg_MessageBox formMove = new Dlg_MessageBox(EMESSAGEBOX.WARNING, string.Format("축 이동 하시겠습니까?"));
                    if (formMove.ShowDialog() == DialogResult.Yes)
                    {
                        if (teachingInputValue5.Clicked)
                            MoveTarget(AXIS_DIRECTION2.Z);
                        break;
                    }
                    break;
                case TEACHING_TASK.SAVE_CURRENT:
                    ParameterDefine.POSITION dCurDir = ParameterDefine.POSITION.POSITION_UNKNOWN;
                    double dCurNewPos = 0;
                    if (teachingInputValue5.Clicked)
                    {
                        dCurDir = curZ; dCurNewPos = (double)teachingInputValue5.CurrentPos;
                    }
                    else
                        return;
                    double dDifCur = dCurNewPos - Machine.param.Position(dCurDir);
                    Dlg_MessageBox formCurrent = new Dlg_MessageBox(EMESSAGEBOX.WARNING, string.Format(dDifCur.ToString("N3") + " 만큼 차이가 발생했습니다.\n" + "Current 값을 저장하시겠습니까?"));
                    if (formCurrent.ShowDialog() == DialogResult.Yes)
                    {
                        if (teachingInputValue5.Clicked) SaveNewPos((double)teachingInputValue5.CurrentPos, AXIS_DIRECTION2.Z);
                    }
                    break;
                case TEACHING_TASK.SAVE_TARGET:
                    ParameterDefine.POSITION dTarDir = ParameterDefine.POSITION.POSITION_UNKNOWN;
                    double dTarNewPos = 0;
                    if (teachingInputValue5.Clicked)
                    {
                        dTarDir = curZ; dTarNewPos = (double)teachingInputValue5.TargetPos;
                    }
                    else
                        return;
                    double dDifTar = dTarNewPos - Machine.param.Position(dTarDir);
                    Dlg_MessageBox formTarget = new Dlg_MessageBox(EMESSAGEBOX.WARNING, string.Format(dDifTar.ToString("N3") + " 만큼 차이가 발생했습니다.\n" + "Target 값을 저장하시겠습니까?"));
                    if (formTarget.ShowDialog() == DialogResult.Yes)
                    {
                        if (teachingInputValue5.Clicked) SaveNewPos((double)teachingInputValue5.TargetPos, AXIS_DIRECTION2.Z);
                    }
                    break;
                case TEACHING_TASK.SET_VEL_SLOW:
                    tbxSpeedValue2.Text = (UIDefine.UIJOGVEL[(int)UIDefine.UIJOGVELINFO.SLOWER] / 1000.0).ToString();
                    break;
                case TEACHING_TASK.SET_VEL_NORMAL:
                    tbxSpeedValue2.Text = (UIDefine.UIJOGVEL[(int)UIDefine.UIJOGVELINFO.NORMAL] / 1000.0).ToString();
                    break;
                case TEACHING_TASK.SET_VEL_FAST:
                    tbxSpeedValue2.Text = (UIDefine.UIJOGVEL[(int)UIDefine.UIJOGVELINFO.FASTER] / 1000.0).ToString();
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

            var proc = Machine.proclist[(int)Machine.PROCESS.SYS] as ProcessSys;
            if (proc.doorOpenState)
            {
                Dlg_MessageBox msgBox = new Dlg_MessageBox(EMESSAGEBOX.MSG,
                               string.Format("Please Close the door"));
                msgBox.ShowDialog();
                return;
            }

            Action action = null;
            int errcode = 0;
            int iAxisdx = 0;
            int iAxisdx2 = 0;
            double vel_z = 1000 * Machine.param.Velocity(ParameterDefine.VELOCITY.JIG_Z_VEL);
            double acc_z = 1000 * Machine.param.Velocity(ParameterDefine.VELOCITY.JIG_Z_ACC);
            double dec_z = 1000 * Machine.param.Velocity(ParameterDefine.VELOCITY.JIG_Z_DEC);
            double pos = 0.0;

            action = new Action(delegate
            {
                switch (Pos)
                {
                    case AXIS_DIRECTION2.Z:
                        iAxisdx = (int)AXIS.JIG_IN_BUF_Z;
                        dTargetValue2[(int)Pos] = teachingInputValue5.TargetPos * 1000.0;
                        Machine.motion.MoveAxisAbs(iAxisdx, dTargetValue2[(int)Pos], vel_z, acc_z, dec_z);
                        break;
                }
            });
            //SetCurrentTask(TEACHING_TASK.MOVE_TARGET);

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
                    case "InputZ":
                        min = minZ; max = maxZ;
                        break;
                }
                SubForm_TenKey keyPad = new SubForm_TenKey(ref value, "Target Pos Set", "Pos Set", min, max, 300, 300, true);
                keyPad.ShowDialog();
                input.textBoxTargetPos.Text = value[0];
            }
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

            var proc = Machine.proclist[(int)Machine.PROCESS.SYS] as ProcessSys;
            if (proc.doorOpenState)
            {
                Dlg_MessageBox msgBox = new Dlg_MessageBox(EMESSAGEBOX.MSG,
                               string.Format("Please Close the door"));
                msgBox.ShowDialog();
                return;
            }

            double dSpeed;
            double MaxSpeedRZ = 300 * 1000;
            double MinSpeedRZ = 1 * 1000;

            switch (axis)
            {              
                case (int)AXIS.JIG_IN_BUF_Z:
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
        
        private void button_ZPlus_MouseDown(object sender, MouseEventArgs e)
        {
            if (tabControl1.SelectedIndex == 0)
                MoveJogAxis((int)AXIS.JIG_IN_BUF_Z, true);
            if (tabControl1.SelectedIndex == 1)
                MoveJogAxis((int)AXIS.JIG_IN_BUF_Z, true);

        }

        private void button_ZPlus_MouseUp(object sender, MouseEventArgs e)
        {
            Machine.motion.AxisStopAll(true);
            Machine.usingMoveJog = false;
        }

        private void button_ZMinus_MouseDown(object sender, MouseEventArgs e)
        {
            if(tabControl1.SelectedIndex == 0)
                MoveJogAxis((int)AXIS.JIG_IN_BUF_Z, false);
            if (tabControl1.SelectedIndex == 1)
                MoveJogAxis((int)AXIS.JIG_IN_BUF_Z, false);
        }

        private void button_ZMinus_MouseUp(object sender, MouseEventArgs e)
        {
            Machine.motion.AxisStopAll(true);
            Machine.usingMoveJog = false;
        }

        private void button_JigLiftLDPos_Click(object sender, EventArgs e)
        {
            //var procSys = Machine.proclist[(int)Machine.PROCESS.SYS] as ProcessSys;
            //if (procSys.doorOpenState)
            //{
            //    Dlg_MessageBox msgBox = new Dlg_MessageBox(EMESSAGEBOX.MSG,
            //                   string.Format("Please Close the door"));
            //    msgBox.ShowDialog();
            //    return;
            //}

            //var proc = Machine.proclist[(int)Machine.PROCESS.JIGINBUF];
            //proc.SetMessage((int)ProcessJigInBuf.MSG.MSG_SINGLE, (int)ProcessJigInBuf.STEP.MOVE_LIFT_LOADING_POS);
        }

        private void button_JigLiftULDPos_Click(object sender, EventArgs e)
        {
            //var procSys = Machine.proclist[(int)Machine.PROCESS.SYS] as ProcessSys;
            //if (procSys.doorOpenState)
            //{
            //    Dlg_MessageBox msgBox = new Dlg_MessageBox(EMESSAGEBOX.MSG,
            //                   string.Format("Please Close the door"));
            //    msgBox.ShowDialog();
            //    return;
            //}

            //var proc = Machine.proclist[(int)Machine.PROCESS.JIGINBUF];
            //proc.SetMessage((int)ProcessJigInBuf.MSG.MSG_SINGLE, (int)ProcessJigInBuf.STEP.MOVE_LIFT_UNLOADING_POS);
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
                    case AXIS_DIRECTION2.Z:
                        teachingInputValue5.SetTargetPos(dNewPos);
                        posName = Machine.param.PositionNAME(curZ);
                        dOldPos = Machine.param.Position(curZ);
                        Machine.param.Position(curZ, dNewPos);
                        break;
                }
                Machine.param.Write();
                string unit = Machine.param.PositionUNIT(curZ); //"mm"
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

        }
        //private void tbxSpeedValue1_DoubleClick(object sender, EventArgs e)
        //{
        //    string[] value = { tbxSpeedValue1.Text };
        //    SubForm_TenKey keyPad = new SubForm_TenKey(ref value, "XY Vel Set", "Vel Set", 0, 255, 300, 300, false);
        //    keyPad.ShowDialog();
        //    tbxSpeedValue1.Text = value[0];
        //}

        private void tbxSpeedValue2_DoubleClick(object sender, EventArgs e)
        {
            string[] value = { tbxSpeedValue2.Text };
            SubForm_TenKey keyPad = new SubForm_TenKey(ref value, "RZ Vel Set", "Vel Set", 0, 255, 300, 300, true);
            keyPad.ShowDialog();
            tbxSpeedValue2.Text = value[0];
        }
        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl2.SelectedIndex == 0) //PP
                SelectButton_ClickEvent(listTeachingPointBtn[(int)mCurrentTeachingPoint], null);
        }
    }
}
