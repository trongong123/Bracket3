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
    public partial class tabTrayTeach : Form, IForm
    {
        public enum TrayPAGE
        {
            UNKNOWN = -1,
            PP,
            VISION_TRAY
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
        public BTN_TAB_SELECT mLeftTeachingPoint = BTN_TAB_SELECT.YZ_TRANSFER_READY;
        public BTN_TAB_SELECT mRightTeachingPoint = BTN_TAB_SELECT.Z_WORK_LOADING;
        public List<NV_UI.NV_Button_TG_NS> listTeachingPointBtn = new List<NV_UI.NV_Button_TG_NS>();
        tabProdLoaderTrayCalib tabProdLoaderTrayCalib;
        double maxY, maxZ;
        double minY, minZ;
        ParameterDefine.POSITION curY;
        ParameterDefine.POSITION curZ;

        Image OriginZPlus;
        Image OriginZMinus;
        Image ReverseZPlus;
        Image ReverseZMinus;

        public tabTrayTeach()
        {
            InitializeComponent();
            InitializeVariable();
            SetBtnMouseEvents(FunctionBtnClickEvent);

            OriginZPlus = new Bitmap(button_ZPlus.Image);
            OriginZMinus = new Bitmap(button_ZMinus.Image);

            Bitmap bmpP = new Bitmap(button_ZPlus.Image);
            Bitmap bmpM = new Bitmap(button_ZMinus.Image);
            bmpP.RotateFlip(RotateFlipType.RotateNoneFlipY);
            bmpM.RotateFlip(RotateFlipType.RotateNoneFlipY);

            ReverseZPlus = bmpP;
            ReverseZMinus = bmpM;
        }
        private void InitializeVariable()
        {
            this.curPage = TrayPAGE.PP;
            this.eCurrentTask = TEACHING_TASK.NONE;
            this.mCurrentTeachingPoint = BTN_TAB_SELECT.YZ_TRANSFER_READY;
            //this.dTargetValue = new double[Enum.GetValues(typeof(AXIS_DIRECTION)).Length];
            this.dTargetValue2 = new double[Enum.GetValues(typeof(AXIS_DIRECTION2)).Length];

            tabProdLoaderTrayCalib = new tabProdLoaderTrayCalib((int)CAMERA.TRAY);
            tabProdLoaderTrayCalib.TopLevel = false;
            tabProdLoaderTrayCalib.Parent = panel1;
            tabProdLoaderTrayCalib.Dock = DockStyle.Fill;
            tabProdLoaderTrayCalib.Show();

            tbxSpeedValue1.Text = (UIDefine.UIJOGVEL[(int)UIDefine.UIJOGVELINFO.NORMAL] / 1000.0).ToString();
            tbxSpeedValue2.Text = (UIDefine.UIJOGVEL[(int)UIDefine.UIJOGVELINFO.NORMAL] / 1000.0).ToString();
        }

        public void StartTimer(bool enable)
        {
            if (enable)
            {
                timer1.Start();
                if (curPage == TrayPAGE.VISION_TRAY)
                    tabProdLoaderTrayCalib.StartTimer(true);
                UpdateTeachingValue();
            }
            else
            {
                timer1.Stop();
                tabProdLoaderTrayCalib.StartTimer(false);
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

            if (tabControl1.SelectedIndex == 0)
            {
                if (teachingInputValue2.Clicked)
                {
                    Machine.motion.GetAxisServoOn((int)AXIS.TRAY_TRF_Y, ref ServoStatusCheck);
                    if (!ServoStatusCheck)
                        ServoStatus = false;
                    Machine.motion.GetAlarm((int)AXIS.TRAY_TRF_Y, ref AlarmStatusCheck);
                    if (AlarmStatusCheck)
                        AlarmStatus = true;
                }
                if (teachingInputValue5.Clicked)
                {
                    Machine.motion.GetAxisServoOn((int)AXIS.TRAY_TRF_Z, ref ServoStatusCheck);
                    if (!ServoStatusCheck)
                        ServoStatus = false;
                    Machine.motion.GetAlarm((int)AXIS.TRAY_TRF_Z, ref AlarmStatus);
                    if (AlarmStatusCheck)
                        AlarmStatus = true;
                }
            }

            else if (tabControl1.SelectedIndex == 1)
            {
                if (teachingInputValue5.Clicked)
                {
                    Machine.motion.GetAxisServoOn((int)AXIS.TRAY_WORK_Z, ref ServoStatusCheck);
                    if (!ServoStatusCheck)
                        ServoStatus = false;
                    Machine.motion.GetAlarm((int)AXIS.TRAY_WORK_Z, ref AlarmStatus);
                    if (AlarmStatusCheck)
                        AlarmStatus = true;
                }
            }
        }

        private void UpdateIOState()
        {
            uint returnValue = 0;

            if (Machine.IO == null)
                return;

            Machine.IO.GetIn((int)DI.TRAY_IN_STOPPER_UP, ref returnValue);
            disp_InStopperUp.ButtonPush = returnValue == 1 ? true : false;
            Machine.IO.GetIn((int)DI.TRAY_IN_STOPPER_DOWN, ref returnValue);
            disp_InStopperDown.ButtonPush = returnValue == 1 ? true : false;

            Machine.IO.GetIn((int)DI.TRAY_IN_BUF_STOPPER_UP, ref returnValue);
            disp_InBufStopperUp.ButtonPush = returnValue == 1 ? true : false;
            Machine.IO.GetIn((int)DI.TRAY_IN_BUF_STOPPER_DOWN, ref returnValue);
            disp_InBufStopperDown.ButtonPush = returnValue == 1 ? true : false;

            Machine.IO.GetIn((int)DI.TRAY_OUT_BUF_STOPPER_UP, ref returnValue);
            disp_OutBufferStopperUp.ButtonPush = returnValue == 1 ? true : false;
            Machine.IO.GetIn((int)DI.TRAY_OUT_BUF_STOPPER_DOWN, ref returnValue);
            disp_OutBufferStopperDown.ButtonPush = returnValue == 1 ? true : false;
        }

        private void UpdateAxisState()
        {
            double pos = 0;
            double actualPos = 0;
            bool result;
            bool isHW_POSLimit = false; bool isHW_NEGLimit = false;
            HomeState isHW_POSHome = HomeState.HomeStateNEED;
            AXIS selAxisZ = AXIS.TRAY_TRF_Z;
            AXIS selAxisY = AXIS.TRAY_TRF_Y;

            if (tabControl1.SelectedIndex == 0)
            {
                selAxisY = AXIS.TRAY_TRF_Y;
                selAxisZ = AXIS.TRAY_TRF_Z;

                Machine.motion.GetAxisActualPos((int)selAxisY, ref actualPos);
                teachingInputValue2.CurrentPos = actualPos / 1000.0;

                Machine.motion.GetAxis_POSLimit((int)selAxisY, ref isHW_POSLimit);
                Machine.motion.GetAxis_NEGLimit((int)selAxisY, ref isHW_NEGLimit);
                Machine.motion.GetAxisHomeComplete((int)selAxisY, ref isHW_POSHome);

                teachingInputValue2.LimitPlusBackColor = isHW_POSLimit == true ? Color.Red : Color.White;
                teachingInputValue2.LimitMinusBackColor = isHW_NEGLimit == true ? Color.Red : Color.White;
                teachingInputValue2.HomeBackColor = isHW_POSHome == HomeState.HomeStateDONE ? Color.Lime : Color.Red;
            }
            else if (tabControl1.SelectedIndex == 1)
                selAxisZ = AXIS.TRAY_WORK_Z;

            Machine.motion.GetAxisActualPos((int)selAxisZ, ref actualPos);
            teachingInputValue5.CurrentPos = actualPos / 1000.0;

            Machine.motion.GetAxis_POSLimit((int)selAxisZ, ref isHW_POSLimit);
            Machine.motion.GetAxis_NEGLimit((int)selAxisZ, ref isHW_NEGLimit);
            Machine.motion.GetAxisHomeComplete((int)selAxisZ, ref isHW_POSHome);

            teachingInputValue5.LimitPlusBackColor = isHW_POSLimit == true ? Color.Red : Color.White;
            teachingInputValue5.LimitMinusBackColor = isHW_NEGLimit == true ? Color.Red : Color.White;
            teachingInputValue5.HomeBackColor = isHW_POSHome == HomeState.HomeStateDONE ? Color.Lime : Color.Red;
        }

        private void UpdateTargetPosState()
        {
            double targetPosCur = 0;
            double targetPosSet = 0;

            targetPosCur = teachingInputValue2.TargetPos;
            targetPosSet = Machine.param.Position(curY);
            teachingInputValue2.TargetPosBackColor = (Math.Abs(targetPosSet - targetPosCur) < 0.01) == true ? Color.White : Color.Yellow;

            targetPosCur = teachingInputValue5.TargetPos;
            targetPosSet = Machine.param.Position(curZ);
            teachingInputValue5.TargetPosBackColor = (Math.Abs(targetPosSet - targetPosCur) < 0.01) == true ? Color.White : Color.Yellow;
        }
        #endregion

        #region TEACHING_POS_SELECTED_FUNCTION
        public enum BTN_TAB_SELECT
        {
            YZ_TRANSFER_READY,
            YZ_TRANSFER_LOADING,
            YZ_TRANSFER_UNLOADING,

            Z_WORK_LOADING,
            Z_WORK_WORK,
            Z_WORK_UNLOADING,
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
                    case BTN_TAB_SELECT.YZ_TRANSFER_READY:
                        curY = ParameterDefine.POSITION.TRAY_TRF_Y_READY_POS;
                        curZ = ParameterDefine.POSITION.TRAY_TRF_Z_READY_POS;
                        break;
                    case BTN_TAB_SELECT.YZ_TRANSFER_LOADING:
                        curY = ParameterDefine.POSITION.TRAY_TRF_Y_LOADING_POS;
                        curZ = ParameterDefine.POSITION.TRAY_TRF_Z_LOADING_POS;
                        break;
                    case BTN_TAB_SELECT.YZ_TRANSFER_UNLOADING:
                        curY = ParameterDefine.POSITION.TRAY_TRF_Y_UNLOADING_POS;
                        curZ = ParameterDefine.POSITION.TRAY_TRF_Z_UNLOADING_POS;
                        break;
                    case BTN_TAB_SELECT.Z_WORK_LOADING:
                        curZ = ParameterDefine.POSITION.TRAY_WORK_LOADING_POS;
                        break;
                    case BTN_TAB_SELECT.Z_WORK_WORK:
                        curZ = ParameterDefine.POSITION.TRAY_WORK_WORK_POS;
                        break;
                    case BTN_TAB_SELECT.Z_WORK_UNLOADING:
                        curZ = ParameterDefine.POSITION.TRAY_WORK_UNLOADING_POS;
                        break;
                }

                dTargetValue2[(int)AXIS_DIRECTION2.Y] = 1000.0 * Machine.param.Position(curY);
                dTargetValue2[(int)AXIS_DIRECTION2.Z] = 1000.0 * Machine.param.Position(curZ);

                teachingInputValue2.TargetPos = dTargetValue2[(int)AXIS_DIRECTION2.Y] / 1000.0;
                teachingInputValue5.TargetPos = dTargetValue2[(int)AXIS_DIRECTION2.Z] / 1000.0;

                maxY = Machine.param.PositionMAX(curY);
                maxZ = Machine.param.PositionMAX(curZ);

                minY = Machine.param.PositionMIN(curY);
                minZ = Machine.param.PositionMIN(curZ);

                if (tabControl1.SelectedIndex == 0)
                {
                    mLeftTeachingPoint = mCurrentTeachingPoint;

                    teachingInputValue2.Visible = true;
                    button_YPlus.Enabled = true;
                    button_YMinus.Enabled = true;

                    teachingInputValue5.Visible = true;
                    button_ZPlus.Enabled = true;
                    button_ZMinus.Enabled = true;

                    tableLayoutPanel7.SetRow(button_ZPlus, 2);
                    tableLayoutPanel7.SetRow(button_ZMinus, 0);
                    button_ZPlus.Image = OriginZPlus;
                    button_ZMinus.Image = OriginZMinus;
                }
                else if (tabControl1.SelectedIndex == 1)
                {
                    mRightTeachingPoint = mCurrentTeachingPoint;

                    teachingInputValue2.Visible = false;
                    button_YPlus.Enabled = false;
                    button_YMinus.Enabled = false;

                    teachingInputValue5.Visible = true;
                    button_ZPlus.Enabled = true;
                    button_ZMinus.Enabled = true;

                    tableLayoutPanel7.SetRow(button_ZPlus, 0);
                    tableLayoutPanel7.SetRow(button_ZMinus, 2);
                    button_ZPlus.Image = ReverseZPlus;
                    button_ZMinus.Image = ReverseZMinus;
                }
            }
        }
        bool firstStart = true;

        private void Tab_Select_First_Shown(object sender, EventArgs e)
        {
            if (firstStart)
            {
                SelectButton_ClickEvent(nV_Button_TG_NS_YZTransferReady, null);
                firstStart = false;
            }
        }
        #endregion
        #region BUTTON_CLICK_FUNCTION
        public void SetBtnMouseEvents(NV_UI.NV_Button_PB_NS.ClickEventDelegate functionBtnEvent)
        {
            nV_Button_TG_NS_YZTransferReady.ClickEvent += SelectButton_ClickEvent;
            nV_Button_TG_NS_YZTransferLoading.ClickEvent += SelectButton_ClickEvent;
            nV_Button_TG_NS_YZTransferUnloading.ClickEvent += SelectButton_ClickEvent;
            nV_Button_TG_NS_ZWorkLoading.ClickEvent += SelectButton_ClickEvent;
            nV_Button_TG_NS_ZWorkWork.ClickEvent += SelectButton_ClickEvent;
            nV_Button_TG_NS_ZWorkUnloading.ClickEvent += SelectButton_ClickEvent;

            listTeachingPointBtn.Add(nV_Button_TG_NS_YZTransferReady);
            listTeachingPointBtn.Add(nV_Button_TG_NS_YZTransferLoading);
            listTeachingPointBtn.Add(nV_Button_TG_NS_YZTransferUnloading);
            listTeachingPointBtn.Add(nV_Button_TG_NS_ZWorkLoading);
            listTeachingPointBtn.Add(nV_Button_TG_NS_ZWorkWork);
            listTeachingPointBtn.Add(nV_Button_TG_NS_ZWorkUnloading);

            nV_Button_PB_NS_MoveTarget.ClickEvent += functionBtnEvent;
            nV_Button_PB_NS_SaveCurrentPos.ClickEvent += functionBtnEvent;
            nV_Button_PB_NS_SaveTargetPos.ClickEvent += functionBtnEvent;
            nV_Button_PB_NS_SetVelSlow.ClickEvent += functionBtnEvent;
            nV_Button_PB_NS_SetVelNormal.ClickEvent += functionBtnEvent;
            nV_Button_PB_NS_SetVelFast.ClickEvent += functionBtnEvent;

            teachingInputValue2.btnEnableCallbackEvent += CheckBoxClickEvent;
            teachingInputValue2.clickEvent += TargetInputClickEvent;
            teachingInputValue5.btnEnableCallbackEvent += CheckBoxClickEvent;
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
                    LogUtil.Instance.Log(LOG_TYPE.UI, "Tray Move Target Button Click", CONTENT_TYPE.INFO);

                    Dlg_MessageBox formMove = new Dlg_MessageBox(EMESSAGEBOX.WARNING, string.Format("축 이동 하시겠습니까?"));
                    if (formMove.ShowDialog() == DialogResult.Yes)
                    {
                        if (teachingInputValue2.Clicked)
                            MoveTarget(AXIS_DIRECTION2.Y);
                        if (teachingInputValue5.Clicked)
                            MoveTarget(AXIS_DIRECTION2.Z);
                        break;
                    }
                    break;
                case TEACHING_TASK.SAVE_CURRENT:
                    ParameterDefine.POSITION dCurDir;
                    double dCurNewPos = 0;
                    if (teachingInputValue2.Clicked)
                    {
                        dCurDir = curY; dCurNewPos = (double)teachingInputValue2.CurrentPos;
                    }
                    else if (teachingInputValue5.Clicked)
                    {
                        dCurDir = curZ; dCurNewPos = (double)teachingInputValue5.CurrentPos;
                    }
                    else
                        return;
                    double dDifCur = dCurNewPos - Machine.param.Position(dCurDir);
                    Dlg_MessageBox formCurrent = new Dlg_MessageBox(EMESSAGEBOX.WARNING, string.Format(dDifCur.ToString("N3") + " 만큼 차이가 발생했습니다.\n" + "Current 값을 저장하시겠습니까?"));
                    if (formCurrent.ShowDialog() == DialogResult.Yes)
                    {
                        if (teachingInputValue2.Clicked) SaveNewPos((double)teachingInputValue2.CurrentPos, AXIS_DIRECTION2.Y);
                        if (teachingInputValue5.Clicked) SaveNewPos((double)teachingInputValue5.CurrentPos, AXIS_DIRECTION2.Z);
                    }
                    break;
                case TEACHING_TASK.SAVE_TARGET:
                    ParameterDefine.POSITION dTarDir;
                    double dTarNewPos = 0;
                    if (teachingInputValue2.Clicked)
                    {
                        dTarDir = curY; dTarNewPos = (double)teachingInputValue2.TargetPos;
                    }
                    else if (teachingInputValue5.Clicked)
                    {
                        dTarDir = curZ; dTarNewPos = (double)teachingInputValue5.TargetPos;
                    }
                    else
                        return;
                    double dDifTar = dTarNewPos - Machine.param.Position(dTarDir);
                    Dlg_MessageBox formTarget = new Dlg_MessageBox(EMESSAGEBOX.WARNING, string.Format(dDifTar.ToString("N3") + " 만큼 차이가 발생했습니다.\n" + "Target 값을 저장하시겠습니까?"));
                    if (formTarget.ShowDialog() == DialogResult.Yes)
                    {
                        if (teachingInputValue2.Clicked) SaveNewPos((double)teachingInputValue2.TargetPos, AXIS_DIRECTION2.Y);
                        if (teachingInputValue5.Clicked) SaveNewPos((double)teachingInputValue5.TargetPos, AXIS_DIRECTION2.Z);
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

            double vel_y = 1000 * Machine.param.Velocity(ParameterDefine.VELOCITY.TRAY_TRF_Y_VEL);
            double acc_y = 1000 * Machine.param.Velocity(ParameterDefine.VELOCITY.TRAY_TRF_Y_ACC);
            double dec_y = 1000 * Machine.param.Velocity(ParameterDefine.VELOCITY.TRAY_TRF_Y_DEC);
            double vel_z = 1000 * Machine.param.Velocity(ParameterDefine.VELOCITY.TRAY_TRF_Z_VEL);
            double acc_z = 1000 * Machine.param.Velocity(ParameterDefine.VELOCITY.TRAY_TRF_Z_ACC);
            double dec_z = 1000 * Machine.param.Velocity(ParameterDefine.VELOCITY.TRAY_TRF_Z_DEC);
            double pos = 0.0;

            action = new Action(delegate
            {
                switch (Pos)
                {
                    case AXIS_DIRECTION2.Y:
                        if (mCurrentTeachingPoint == BTN_TAB_SELECT.YZ_TRANSFER_READY
                         || mCurrentTeachingPoint == BTN_TAB_SELECT.YZ_TRANSFER_LOADING
                         || mCurrentTeachingPoint == BTN_TAB_SELECT.YZ_TRANSFER_UNLOADING)
                            iAxisdx = (int)AXIS.TRAY_TRF_Y;
                        dTargetValue2[(int)Pos] = teachingInputValue2.TargetPos * 1000.0;
                        Machine.motion.MoveAxisAbs(iAxisdx, dTargetValue2[(int)Pos], vel_y, acc_y, dec_y);
                        break;
                    case AXIS_DIRECTION2.Z:
                        if (mCurrentTeachingPoint == BTN_TAB_SELECT.YZ_TRANSFER_READY
                         || mCurrentTeachingPoint == BTN_TAB_SELECT.YZ_TRANSFER_LOADING
                         || mCurrentTeachingPoint == BTN_TAB_SELECT.YZ_TRANSFER_UNLOADING)
                            iAxisdx = (int)AXIS.TRAY_TRF_Z;
                        else if (mCurrentTeachingPoint == BTN_TAB_SELECT.Z_WORK_LOADING
                         || mCurrentTeachingPoint == BTN_TAB_SELECT.Z_WORK_WORK
                         || mCurrentTeachingPoint == BTN_TAB_SELECT.Z_WORK_UNLOADING)
                        {
                            //다른 속도임 수정하지 않도록 주의!
                            iAxisdx = (int)AXIS.TRAY_WORK_Z;
                            vel_z = 1000 * Machine.param.Velocity(ParameterDefine.VELOCITY.TRAY_WORK_Z_VEL);
                            acc_z = 1000 * Machine.param.Velocity(ParameterDefine.VELOCITY.TRAY_WORK_Z_ACC);
                            dec_z = 1000 * Machine.param.Velocity(ParameterDefine.VELOCITY.TRAY_WORK_Z_DEC);
                        }
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

        private void CheckBoxClickEvent(object sender, EventArgs e)
        {
            TeachingInputValue input = (sender as TeachingInputValue);
            if (input != null && input.Clicked)
            {
                string sTag = input.Tag.ToString();
                switch (sTag)
                {
                    case "InputY":                        
                        if (teachingInputValue5.Clicked)
                            teachingInputValue5.Clicked = false;
                        break;
                    case "InputZ":
                        if (teachingInputValue2.Clicked)
                            teachingInputValue2.Clicked = false;
                        break;
                }
            }
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
                    case "InputZ":
                        min = minZ; max = maxZ;
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
            double MaxSpeedXY = 300 * 1000;
            double MinSpeedXY = 1 * 1000;
            double MaxSpeedRZ = 300 * 1000;
            double MinSpeedRZ = 1 * 1000;

            switch (axis)
            {
                case (int)AXIS.TRAY_TRF_Y:
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

                case (int)AXIS.TRAY_TRF_Z:
                case (int)AXIS.TRAY_WORK_Z:

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

        private void button_YPlus_MouseDown(object sender, MouseEventArgs e)
        {
            if (tabControl1.SelectedIndex == 0)
            {
                Machine.interfer_release_trf_y_distance = false;
                MoveJogAxis((int)AXIS.TRAY_TRF_Y, true);
            }
        }

        private void button_YPlus_MouseUp(object sender, MouseEventArgs e)
        {
            Machine.motion.AxisStopAll(true);
            Machine.interfer_release_trf_y_distance = true;
            Machine.usingMoveJog = false;
        }

        private void button_YMinus_MouseDown(object sender, MouseEventArgs e)
        {
            if (tabControl1.SelectedIndex == 0)
            {
                MoveJogAxis((int)AXIS.TRAY_TRF_Y, false);
            }
        }

        private void button_YMinus_MouseUp(object sender, MouseEventArgs e)
        {
            Machine.motion.AxisStopAll(true);
            Machine.usingMoveJog = false;
        }

        private void button_ZPlus_MouseDown(object sender, MouseEventArgs e)
        {
            if (tabControl1.SelectedIndex == 0)
                MoveJogAxis((int)AXIS.TRAY_TRF_Z, true);
            if (tabControl1.SelectedIndex == 1)
                MoveJogAxis((int)AXIS.TRAY_WORK_Z, true);
        }

        private void button_ZPlus_MouseUp(object sender, MouseEventArgs e)
        {
            Machine.motion.AxisStopAll(true);
            Machine.usingMoveJog = false;
        }

        private void button_ZMinus_MouseDown(object sender, MouseEventArgs e)
        {
            if (tabControl1.SelectedIndex == 0)
            {
                MoveJogAxis((int)AXIS.TRAY_TRF_Z, false);
                Machine.interfer_release_trf_z = false;
            }
            if (tabControl1.SelectedIndex == 1)
                MoveJogAxis((int)AXIS.TRAY_WORK_Z, false);
        }

        private void button_ZMinus_MouseUp(object sender, MouseEventArgs e)
        {
            Machine.motion.AxisStopAll(true);
            Machine.interfer_release_trf_z = true;
            Machine.usingMoveJog = false;
        }

        private void button_ServoOnOff_Click(object sender, EventArgs e)
        {
            bool servoStatus = false;

            if (tabControl1.SelectedIndex == 0)
            {
                if (teachingInputValue5.Clicked)
                {
                    Machine.motion.GetAxisServoOn((int)AXIS.TRAY_TRF_Z, ref servoStatus);
                    if (servoStatus)
                        Machine.motion.SetAxisServoOn((int)AXIS.TRAY_TRF_Z, 0);
                    else
                        Machine.motion.SetAxisServoOn((int)AXIS.TRAY_TRF_Z, 1);
                }
                if (teachingInputValue2.Clicked)
                {
                    Machine.motion.GetAxisServoOn((int)AXIS.TRAY_TRF_Y, ref servoStatus);
                    if (servoStatus)
                        Machine.motion.SetAxisServoOn((int)AXIS.TRAY_TRF_Y, 0);
                    else
                        Machine.motion.SetAxisServoOn((int)AXIS.TRAY_TRF_Y, 1);
                }
            }
            if (tabControl1.SelectedIndex == 1)
            {
                if (teachingInputValue5.Clicked)
                {
                    Machine.motion.GetAxisServoOn((int)AXIS.TRAY_WORK_Z, ref servoStatus);
                    if (servoStatus)
                        Machine.motion.SetAxisServoOn((int)AXIS.TRAY_WORK_Z, 0);
                    else
                        Machine.motion.SetAxisServoOn((int)AXIS.TRAY_WORK_Z, 1);
                }
            }
        }

        private void button_AlarmReset_Click(object sender, EventArgs e)
        {
            for (int i = (int)AXIS.TRAY_TRF_Z; i <= (int)AXIS.TRAY_WORK_Z; ++i)
            {
                Machine.motion.AlarmClear((int)AXIS.AXISMAX);
            }
        }

        private void button_InStopperUpDown_Click(object sender, EventArgs e)
        {
            var procSys = Machine.proclist[(int)Machine.PROCESS.SYS] as ProcessSys;
            if (procSys.doorOpenState)
            {
                Dlg_MessageBox msgBox = new Dlg_MessageBox(EMESSAGEBOX.MSG,
                               string.Format("Please Close the door"));
                msgBox.ShowDialog();
                return;
            }

            Machine.IO.GetIn((int)DI.TRAY_IN_STOPPER_UP, ref ToolZUp);

            if (ToolZUp == 1)
            {
                Machine.IO.SetOut((int)DO.TRAY_IN_STOPPER_UP, 0);
                Machine.IO.SetOut((int)DO.TRAY_IN_STOPPER_DOWN, 1);
            }
            else
            {
                Machine.IO.SetOut((int)DO.TRAY_IN_STOPPER_UP, 1);
                Machine.IO.SetOut((int)DO.TRAY_IN_STOPPER_DOWN, 0);
            }
        }

        private void button_InBufStopperUpDown_Click(object sender, EventArgs e)
        {
            var procSys = Machine.proclist[(int)Machine.PROCESS.SYS] as ProcessSys;
            if (procSys.doorOpenState)
            {
                Dlg_MessageBox msgBox = new Dlg_MessageBox(EMESSAGEBOX.MSG,
                               string.Format("Please Close the door"));
                msgBox.ShowDialog();
                return;
            }

            Machine.IO.GetIn((int)DI.TRAY_IN_BUF_STOPPER_UP, ref ToolZUp);

            if (ToolZUp == 1)
            {
                Machine.IO.SetOut((int)DO.TRAY_IN_BUF_STOPPER_UP, 0);
                Machine.IO.SetOut((int)DO.TRAY_IN_BUF_STOPPER_DOWN, 1);
            }
            else
            {
                Machine.IO.SetOut((int)DO.TRAY_IN_BUF_STOPPER_UP, 1);
                Machine.IO.SetOut((int)DO.TRAY_IN_BUF_STOPPER_DOWN, 0);
            }
        }

        private void button_OutBufferStopperOnOff_Click(object sender, EventArgs e)
        {
            var procSys = Machine.proclist[(int)Machine.PROCESS.SYS] as ProcessSys;
            if (procSys.doorOpenState)
            {
                Dlg_MessageBox msgBox = new Dlg_MessageBox(EMESSAGEBOX.MSG,
                               string.Format("Please Close the door"));
                msgBox.ShowDialog();
                return;
            }

            Machine.IO.GetOut((int)DO.TRAY_OUT_BUF_STOPPER_UP, ref ToolZUp);

            if (ToolZUp == 1)
            {
                Machine.IO.SetOut((int)DO.TRAY_OUT_BUF_STOPPER_UP, 0);
                Machine.IO.SetOut((int)DO.TRAY_OUT_BUF_STOPPER_DOWN, 1);
            }
            else
            {
                Machine.IO.SetOut((int)DO.TRAY_OUT_BUF_STOPPER_UP, 1);
                Machine.IO.SetOut((int)DO.TRAY_OUT_BUF_STOPPER_DOWN, 0);
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
                    case AXIS_DIRECTION2.Y:
                        teachingInputValue2.SetTargetPos(dNewPos);
                        posName = Machine.param.PositionNAME(curY);
                        dOldPos = Machine.param.Position(curY);
                        Machine.param.Position(curY, dNewPos);
                        break;
                    case AXIS_DIRECTION2.Z:
                        teachingInputValue5.SetTargetPos(dNewPos);
                        posName = Machine.param.PositionNAME(curZ);
                        dOldPos = Machine.param.Position(curZ);
                        Machine.param.Position(curZ, dNewPos);
                        break;
                }
                Machine.param.Write();
                string unit = Machine.param.PositionUNIT(curY); //"mm"
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
                tabProdLoaderTrayCalib.StartTimer(false);
                tabProdLoaderTrayCalib.Hide();
            }
            else if (tabControl2.SelectedIndex == 1) //Vision
            {
                curPage = TrayPAGE.VISION_TRAY;
                tabProdLoaderTrayCalib.StartTimer(true);
                tabProdLoaderTrayCalib.Show();
                tabProdLoaderTrayCalib.UpdateLightSetting();
            }
        }
        private void tbxSpeedValue1_DoubleClick(object sender, EventArgs e)
        {
            string[] value = { tbxSpeedValue1.Text };
            SubForm_TenKey keyPad = new SubForm_TenKey(ref value, "XY Vel Set", "Vel Set", 0, 255, 300, 300, false);
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

                teachingInputValue2.Visible = true;
                button_YPlus.Enabled = true;
                button_YMinus.Enabled = true;         

                teachingInputValue5.Visible = true;
                button_ZPlus.Enabled = true;
                button_ZMinus.Enabled = true;

                tableLayoutPanel7.SetRow(button_ZPlus, 2);
                tableLayoutPanel7.SetRow(button_ZMinus, 0);
                button_ZPlus.Image = OriginZPlus;
                button_ZMinus.Image = OriginZMinus;
            }
            else if (tabControl1.SelectedIndex == 1)
            {
                mCurrentTeachingPoint = mRightTeachingPoint;

                teachingInputValue2.Visible = false;
                button_YPlus.Enabled = false;
                button_YMinus.Enabled = false;

                teachingInputValue5.Visible = true;
                button_ZPlus.Enabled = true;
                button_ZMinus.Enabled = true;

                tableLayoutPanel7.SetRow(button_ZPlus, 0);
                tableLayoutPanel7.SetRow(button_ZMinus, 2);
                button_ZPlus.Image = ReverseZPlus;
                button_ZMinus.Image = ReverseZMinus;
            }
            SelectButton_ClickEvent(listTeachingPointBtn[(int)mCurrentTeachingPoint], null);
            return;
        }

        void UpdateTeachingValue()
        {
            if (tabControl2.SelectedIndex == 0) //PP
                SelectButton_ClickEvent(listTeachingPointBtn[(int)mCurrentTeachingPoint], null);
            else if (tabControl2.SelectedIndex == 1) //Vision
                if (tabControl3.SelectedIndex == 0) //Under
                {
                    tabProdLoaderTrayCalib.UpdateLightSetting();
                }
        }
    }
}
