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
using System.Security;
using Cognex.VisionPro;
using Cognex.VisionPro.ImageFile;
using System.Threading;
using TopEng.Device;
using static CAMASSEMBLYMACHINE.Define.ParameterDefine;

namespace CAMASSEMBLYMACHINE.UI
{
    public partial class tabAssemblerTeach : Form, IForm
    {
        public enum AssemblerPAGE
        {
            UNKNOWN = -1,
            PP,
            VISION_UNDER,
            VISION_JIG,
            VISION_JIG2
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
        AssemblerPAGE curPage = AssemblerPAGE.UNKNOWN;
        public BTN_TAB_SELECT mLeftTeachingPoint = BTN_TAB_SELECT.XYRZ_LEFT_PICKUP;
        public BTN_TAB_SELECT mRightTeachingPoint = BTN_TAB_SELECT.XYRZ_RIGHT_PICKUP;
        public List<NV_UI.NV_Button_TG_NS> listTeachingPointBtn = new List<NV_UI.NV_Button_TG_NS>();
        tabAssemblerUnderCalib tabAssemblerUnderCalib; //tabAssemblerUnderCalib;
        tabAssemblerJigCalib tabAssemblerJigCalib; //tabAssemblerPickerJigCalib
        double maxX, maxY, maxR1, maxR2, maxZ;
        double minX, minY, minR1, minR2, minZ;
        RecipeDefine.POSITION curX, curY, curR1, curR2, curZ;
        public tabAssemblerTeach()
        {
            InitializeComponent();
            InitializeVariable();
            SetBtnMouseEvents(FunctionBtnClickEvent);
        }
        private void InitializeVariable()
        {
            this.curPage = AssemblerPAGE.PP; 
            this.eCurrentTask = TEACHING_TASK.NONE;
            this.mCurrentTeachingPoint = BTN_TAB_SELECT.XYRZ_LEFT_PICKUP;
            //this.dTargetValue = new double[Enum.GetValues(typeof(AXIS_DIRECTION)).Length];
            this.dTargetValue2 = new double[Enum.GetValues(typeof(AXIS_DIRECTION2)).Length];

            tabAssemblerUnderCalib = new tabAssemblerUnderCalib((int)CAMERA.UNDER);
            tabAssemblerUnderCalib.TopLevel = false;
            tabAssemblerUnderCalib.Parent = panelUnder;
            tabAssemblerUnderCalib.Dock = DockStyle.Fill;
            tabAssemblerUnderCalib.Show();

            tabAssemblerJigCalib = new tabAssemblerJigCalib((int)CAMERA.JIG);
            tabAssemblerJigCalib.TopLevel = false;
            tabAssemblerJigCalib.Parent = panelJIG;
            tabAssemblerJigCalib.Dock = DockStyle.Fill;
            tabAssemblerJigCalib.Show();

            tbxSpeedValue1.Text = (UIDefine.UIJOGVEL[(int)UIDefine.UIJOGVELINFO.NORMAL] / 1000.0).ToString();
            tbxSpeedValue2.Text = (UIDefine.UIJOGVEL[(int)UIDefine.UIJOGVELINFO.NORMAL] / 1000.0).ToString();
        }

        public void StartTimer(bool enable)
        {
            if (enable)
            {
                timer1.Start();
                if(curPage == AssemblerPAGE.VISION_UNDER)
                    tabAssemblerUnderCalib.StartTimer(true);
                else if (curPage == AssemblerPAGE.VISION_JIG)
                    tabAssemblerJigCalib.StartTimer(true);
                UpdateTeachingValue();
            }
            else
            {
                timer1.Stop();
                tabAssemblerUnderCalib.StartTimer(false);
                tabAssemblerJigCalib.StartTimer(false);
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if(curPage == AssemblerPAGE.PP)
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
            //한번 False가 되면 계속 유지
            bool ServoStatus = true;
            bool ServoStatusCheck = true;
            bool AlarmStatus = false;
            bool AlarmStatusCheck = false;

            if (teachingInputValue1.Clicked)
            {
                Machine.motion.GetAxisServoOn((int)AXIS.ASSEMBLER_X, ref ServoStatusCheck);
                if (!ServoStatusCheck)
                    ServoStatus = false;
                Machine.motion.GetAlarm((int)AXIS.ASSEMBLER_X, ref AlarmStatusCheck);
                if (AlarmStatusCheck)
                    AlarmStatus = true;
            }
            if (teachingInputValue2.Clicked)
            {
                Machine.motion.GetAxisServoOn((int)AXIS.ASSEMBLER_Y, ref ServoStatusCheck);
                if (!ServoStatusCheck)
                    ServoStatus = false;
                Machine.motion.GetAlarm((int)AXIS.ASSEMBLER_Y, ref AlarmStatusCheck);
                if (AlarmStatusCheck)
                    AlarmStatus = true;
            }
            if (teachingInputValue3.Clicked)
            {
                Machine.motion.GetAxisServoOn((int)AXIS.ASSEMBLER_R1, ref ServoStatusCheck);
                if (!ServoStatusCheck)
                    ServoStatus = false;
                Machine.motion.GetAlarm((int)AXIS.ASSEMBLER_R1, ref AlarmStatusCheck);
                if (AlarmStatusCheck)
                    AlarmStatus = true;
            }
            if (teachingInputValue4.Clicked)
            {
                Machine.motion.GetAxisServoOn((int)AXIS.ASSEMBLER_R2, ref ServoStatusCheck);
                if (!ServoStatusCheck)
                    ServoStatus = false;
                Machine.motion.GetAlarm((int)AXIS.ASSEMBLER_R2, ref AlarmStatusCheck);
                if (AlarmStatusCheck)
                    AlarmStatus = true;
            }
            if (teachingInputValue5.Clicked)
            {
                Machine.motion.GetAxisServoOn((int)AXIS.ASSEMBLER_Z, ref ServoStatusCheck);
                if (!ServoStatusCheck)
                    ServoStatus = false;
                Machine.motion.GetAlarm((int)AXIS.ASSEMBLER_Z, ref AlarmStatus);
                if (AlarmStatusCheck)
                    AlarmStatus = true;
            }
        }
        private void UpdateIOState()
        {
            uint returnValue = 0;

            if (Machine.IO == null)
                return;

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
        private void UpdateAxisState()
        {
            double pos = 0;
            double actualPos = 0;
            bool result;
            bool isHW_POSLimit = false; bool isHW_NEGLimit = false;
            HomeState isHW_POSHome = HomeState.HomeStateNEED;

            Machine.motion.GetAxisActualPos((int)AXIS.ASSEMBLER_X, ref actualPos);
            teachingInputValue1.CurrentPos = actualPos / 1000.0;

            Machine.motion.GetAxisActualPos((int)AXIS.ASSEMBLER_Y, ref actualPos);
            teachingInputValue2.CurrentPos = actualPos / 1000.0;

            Machine.motion.GetAxisActualPos((int)AXIS.ASSEMBLER_R1, ref actualPos);
            teachingInputValue3.CurrentPos = actualPos / 1000.0;

            Machine.motion.GetAxisActualPos((int)AXIS.ASSEMBLER_R2, ref actualPos);
            teachingInputValue4.CurrentPos = actualPos / 1000.0;

            Machine.motion.GetAxisActualPos((int)AXIS.ASSEMBLER_Z, ref actualPos);
            teachingInputValue5.CurrentPos = actualPos / 1000.0;

            Machine.motion.GetAxis_POSLimit((int)AXIS.ASSEMBLER_X, ref isHW_POSLimit);
            Machine.motion.GetAxis_NEGLimit((int)AXIS.ASSEMBLER_X, ref isHW_NEGLimit);
            Machine.motion.GetAxisHomeComplete((int)AXIS.ASSEMBLER_X, ref isHW_POSHome);

            teachingInputValue1.LimitPlusBackColor = isHW_POSLimit == true ? Color.Red : Color.White;
            teachingInputValue1.LimitMinusBackColor = isHW_NEGLimit == true ? Color.Red : Color.White;
            teachingInputValue1.HomeBackColor = isHW_POSHome == HomeState.HomeStateDONE ? Color.Lime : Color.Red;

            Machine.motion.GetAxis_POSLimit((int)AXIS.ASSEMBLER_Y, ref isHW_POSLimit);
            Machine.motion.GetAxis_NEGLimit((int)AXIS.ASSEMBLER_Y, ref isHW_NEGLimit);
            Machine.motion.GetAxisHomeComplete((int)AXIS.ASSEMBLER_Y, ref isHW_POSHome);

            teachingInputValue2.LimitPlusBackColor = isHW_POSLimit == true ? Color.Red : Color.White;
            teachingInputValue2.LimitMinusBackColor = isHW_NEGLimit == true ? Color.Red : Color.White;
            teachingInputValue2.HomeBackColor = isHW_POSHome == HomeState.HomeStateDONE ? Color.Lime : Color.Red;

            Machine.motion.GetAxis_POSLimit((int)AXIS.ASSEMBLER_R1, ref isHW_POSLimit);
            Machine.motion.GetAxis_NEGLimit((int)AXIS.ASSEMBLER_R1, ref isHW_NEGLimit);
            Machine.motion.GetAxisHomeComplete((int)AXIS.ASSEMBLER_R1, ref isHW_POSHome);

            teachingInputValue3.LimitPlusBackColor = isHW_POSLimit == true ? Color.Red : Color.White;
            teachingInputValue3.LimitMinusBackColor = isHW_NEGLimit == true ? Color.Red : Color.White;
            teachingInputValue3.HomeBackColor = isHW_POSHome == HomeState.HomeStateDONE ? Color.Lime : Color.Red;

            Machine.motion.GetAxis_POSLimit((int)AXIS.ASSEMBLER_R2, ref isHW_POSLimit);
            Machine.motion.GetAxis_NEGLimit((int)AXIS.ASSEMBLER_R2, ref isHW_NEGLimit);
            Machine.motion.GetAxisHomeComplete((int)AXIS.ASSEMBLER_R2, ref isHW_POSHome);

            teachingInputValue4.LimitPlusBackColor = isHW_POSLimit == true ? Color.Red : Color.White;
            teachingInputValue4.LimitMinusBackColor = isHW_NEGLimit == true ? Color.Red : Color.White;
            teachingInputValue4.HomeBackColor = isHW_POSHome == HomeState.HomeStateDONE ? Color.Lime : Color.Red;

            Machine.motion.GetAxis_POSLimit((int)AXIS.ASSEMBLER_Z, ref isHW_POSLimit);
            Machine.motion.GetAxis_NEGLimit((int)AXIS.ASSEMBLER_Z, ref isHW_NEGLimit);
            Machine.motion.GetAxisHomeComplete((int)AXIS.ASSEMBLER_Z, ref isHW_POSHome);

            teachingInputValue5.LimitPlusBackColor = isHW_POSLimit == true ? Color.Red : Color.White;
            teachingInputValue5.LimitMinusBackColor = isHW_NEGLimit == true ? Color.Red : Color.White;
            teachingInputValue5.HomeBackColor = isHW_POSHome == HomeState.HomeStateDONE ? Color.Lime : Color.Red;
        }

        private void UpdateTargetPosState()
        {
            double targetPosCur = 0;
            double targetPosSet = 0;

            targetPosCur = teachingInputValue1.TargetPos;
            targetPosSet = Machine.recipe.Position(curX);
            teachingInputValue1.TargetPosBackColor = (Math.Abs(targetPosSet - targetPosCur) < 0.01) == true ? Color.White : Color.Yellow;

            targetPosCur = teachingInputValue2.TargetPos;
            targetPosSet = Machine.recipe.Position(curY);
            teachingInputValue2.TargetPosBackColor = (Math.Abs(targetPosSet - targetPosCur) < 0.01) == true ? Color.White : Color.Yellow;

            targetPosCur = teachingInputValue3.TargetPos;
            targetPosSet = Machine.recipe.Position(curR1);
            teachingInputValue3.TargetPosBackColor = (Math.Abs(targetPosSet - targetPosCur) < 0.01) == true ? Color.White : Color.Yellow;

            targetPosCur = teachingInputValue4.TargetPos;
            targetPosSet = Machine.recipe.Position(curR2);
            teachingInputValue4.TargetPosBackColor = (Math.Abs(targetPosSet - targetPosCur) < 0.01) == true ? Color.White : Color.Yellow;

            targetPosCur = teachingInputValue5.TargetPos;
            targetPosSet = Machine.recipe.Position(curZ);
            teachingInputValue5.TargetPosBackColor = (Math.Abs(targetPosSet - targetPosCur) < 0.01) == true ? Color.White : Color.Yellow;
        }

        #endregion
        #region TEACHING_POS_SELECTED_FUNCTION
        public enum BTN_TAB_SELECT
        {
            XYRZ_LEFT_PICKUP,
            XYRZ_RIGHT_PICKUP,

            XYRZ_LEFT_ALIGN,
            XYRZ_RIGHT_ALIGN,

            XYRZ_LEFT_PLACE,
            XYRZ_RIGHT_PLACE,

            XYRZ_LEFT_SCANNER,
            XYRZ_RIGHT_SCANNER,
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
                    case BTN_TAB_SELECT.XYRZ_LEFT_PICKUP:
                        curX = RecipeDefine.POSITION.ASSEMBLER_X_BUF_L_POS;
                        curY = RecipeDefine.POSITION.ASSEMBLER_Y_BUF_L_POS;
                        curR2 = curR1 = RecipeDefine.POSITION.ASSEMBLER_R_PICKUP_POS;
                        curZ = RecipeDefine.POSITION.ASSEMBLER_Z_PICKUP_POS;

                        teachingInputValue1.Visible = true;
                        teachingInputValue2.Visible = true;
                        teachingInputValue3.Visible = true;
                        teachingInputValue4.Visible = true;
                        teachingInputValue5.Visible = true;

                        button_XPlus.Enabled = true;                        button_XMinus.Enabled = true;
                        button_YPlus.Enabled = true;                        button_YMinus.Enabled = true;
                        button_R1Plus.Enabled = true;                        button_R1Minus.Enabled = true;
                        button_R2Plus.Enabled = true;                        button_R2Minus.Enabled = true;
                        button_ZMinus.Enabled = true;                        button_ZPlus.Enabled = true;
                        break;
                    case BTN_TAB_SELECT.XYRZ_RIGHT_PICKUP:
                        curX = RecipeDefine.POSITION.ASSEMBLER_X_BUF_R_POS;
                        curY = RecipeDefine.POSITION.ASSEMBLER_Y_BUF_R_POS;
                        curR2 = curR1 = RecipeDefine.POSITION.ASSEMBLER_R_PICKUP_POS;
                        curZ = RecipeDefine.POSITION.ASSEMBLER_Z_PICKUP_POS;

                        teachingInputValue1.Visible = true;
                        teachingInputValue2.Visible = true;
                        teachingInputValue3.Visible = true;
                        teachingInputValue4.Visible = true;
                        teachingInputValue5.Visible = true;

                        button_XPlus.Enabled = true;                        button_XMinus.Enabled = true;
                        button_YPlus.Enabled = true;                        button_YMinus.Enabled = true;
                        button_R1Plus.Enabled = true;                        button_R1Minus.Enabled = true;
                        button_R2Plus.Enabled = true;                        button_R2Minus.Enabled = true;
                        button_ZMinus.Enabled = true;                        button_ZPlus.Enabled = true;
                        break;
                    case BTN_TAB_SELECT.XYRZ_LEFT_ALIGN:
                        curX = RecipeDefine.POSITION.ASSEMBLER_X_UNDER_ALIGN_POS;
                        curY = RecipeDefine.POSITION.ASSEMBLER_Y_UNDER_ALIGN_POS;
                        curR2 = curR1 = RecipeDefine.POSITION.ASSEMBLER_R_UNDER_ALIGN_POS;
                        curZ = RecipeDefine.POSITION.ASSEMBLER_Z_UNDER_ALIGN_POS;

                        teachingInputValue1.Visible = true;
                        teachingInputValue2.Visible = true;
                        teachingInputValue3.Visible = true;
                        teachingInputValue4.Visible = true;
                        teachingInputValue5.Visible = true;

                        button_XPlus.Enabled = true; button_XMinus.Enabled = true;
                        button_YPlus.Enabled = true; button_YMinus.Enabled = true;
                        button_R1Plus.Enabled = true; button_R1Minus.Enabled = true;
                        button_R2Plus.Enabled = true; button_R2Minus.Enabled = true;
                        button_ZMinus.Enabled = true; button_ZPlus.Enabled = true;
                        break;
                    case BTN_TAB_SELECT.XYRZ_RIGHT_ALIGN:
                        curX = RecipeDefine.POSITION.ASSEMBLER_X_UNDER_ALIGN2_POS;
                        curY = RecipeDefine.POSITION.ASSEMBLER_Y_UNDER_ALIGN2_POS;
                        curR2 = curR1 = RecipeDefine.POSITION.ASSEMBLER_R_UNDER_ALIGN_POS;
                        curZ = RecipeDefine.POSITION.ASSEMBLER_Z_UNDER_ALIGN_POS;

                        teachingInputValue1.Visible = true;
                        teachingInputValue2.Visible = true;
                        teachingInputValue3.Visible = true;
                        teachingInputValue4.Visible = true;
                        teachingInputValue5.Visible = true;

                        button_XPlus.Enabled = true; button_XMinus.Enabled = true;
                        button_YPlus.Enabled = true; button_YMinus.Enabled = true;
                        button_R1Plus.Enabled = true; button_R1Minus.Enabled = true;
                        button_R2Plus.Enabled = true; button_R2Minus.Enabled = true;
                        button_ZMinus.Enabled = true; button_ZPlus.Enabled = true;
                        break;
                    case BTN_TAB_SELECT.XYRZ_LEFT_PLACE:
                        //curX = RecipeDefine.POSITION.ASSEMBLER_JIG_CEN1_X;
                        //curY = RecipeDefine.POSITION.ASSEMBLER_JIG_CEN1_Y;
                        curR2 = curR1 = RecipeDefine.POSITION.ASSEMBLER_R_PLACE_POS;
                        curZ = RecipeDefine.POSITION.ASSEMBLER_Z_PLACE_L_POS;

                        teachingInputValue1.Visible = false;
                        teachingInputValue2.Visible = false;
                        teachingInputValue3.Visible = true;
                        teachingInputValue4.Visible = true;
                        teachingInputValue5.Visible = true;

                        button_XPlus.Enabled = false;                        button_XMinus.Enabled = false;
                        button_YPlus.Enabled = false;                        button_YMinus.Enabled = false;
                        button_R1Plus.Enabled = true;                        button_R1Minus.Enabled = true;
                        button_R2Plus.Enabled = true;                        button_R2Minus.Enabled = true;
                        button_ZMinus.Enabled = true;                        button_ZPlus.Enabled = true;
                        break;
                    case BTN_TAB_SELECT.XYRZ_RIGHT_PLACE:
                        //curX = RecipeDefine.POSITION.ASSEMBLER_JIG_CEN2_X;
                        //curY = RecipeDefine.POSITION.ASSEMBLER_JIG_CEN2_Y;
                        curR2 = curR1 = RecipeDefine.POSITION.ASSEMBLER_R_PLACE_POS;
                        curZ = RecipeDefine.POSITION.ASSEMBLER_Z_PLACE_R_POS;

                        teachingInputValue1.Visible = false;
                        teachingInputValue2.Visible = false;
                        teachingInputValue3.Visible = true;
                        teachingInputValue4.Visible = true;
                        teachingInputValue5.Visible = true;

                        button_XPlus.Enabled = false;                        button_XMinus.Enabled = false;
                        button_YPlus.Enabled = false;                        button_YMinus.Enabled = false;
                        button_R1Plus.Enabled = true;                        button_R1Minus.Enabled = true;
                        button_R2Plus.Enabled = true;                        button_R2Minus.Enabled = true;
                        button_ZMinus.Enabled = true;                        button_ZPlus.Enabled = true;
                        break;
                    case BTN_TAB_SELECT.XYRZ_LEFT_SCANNER:
                        curX = RecipeDefine.POSITION.ASSEMBLER_X_SCANNER_L_POS;
                        curY = RecipeDefine.POSITION.ASSEMBLER_Y_SCANNER_L_POS;
                        curR2 = curR1 = RecipeDefine.POSITION.ASSEMBLER_R_SCANNER_L_POS;
                        curZ = RecipeDefine.POSITION.ASSEMBLER_Z_SCAN_POS;

                        teachingInputValue1.Visible = true;
                        teachingInputValue2.Visible = true;
                        teachingInputValue3.Visible = true;
                        teachingInputValue4.Visible = true;
                        teachingInputValue5.Visible = true;

                        button_XPlus.Enabled = true; button_XMinus.Enabled = true;
                        button_YPlus.Enabled = true; button_YMinus.Enabled = true;
                        button_R1Plus.Enabled = true; button_R1Minus.Enabled = true;
                        button_R2Plus.Enabled = true; button_R2Minus.Enabled = true;
                        button_ZMinus.Enabled = true; button_ZPlus.Enabled = true;
                        break;
                    case BTN_TAB_SELECT.XYRZ_RIGHT_SCANNER:
                        curX = RecipeDefine.POSITION.ASSEMBLER_X_SCANNER_R_POS;
                        curY = RecipeDefine.POSITION.ASSEMBLER_Y_SCANNER_R_POS;
                        curR2 = curR1 = RecipeDefine.POSITION.ASSEMBLER_R_SCANNER_R_POS;
                        curZ = RecipeDefine.POSITION.ASSEMBLER_Z_SCAN_POS;

                        teachingInputValue1.Visible = true;
                        teachingInputValue2.Visible = true;
                        teachingInputValue3.Visible = true;
                        teachingInputValue4.Visible = true;
                        teachingInputValue5.Visible = true;

                        button_XPlus.Enabled = true; button_XMinus.Enabled = true;
                        button_YPlus.Enabled = true; button_YMinus.Enabled = true;
                        button_R1Plus.Enabled = true; button_R1Minus.Enabled = true;
                        button_R2Plus.Enabled = true; button_R2Minus.Enabled = true;
                        button_ZMinus.Enabled = true; button_ZPlus.Enabled = true;
                        break;
                }

                dTargetValue2[(int)AXIS_DIRECTION2.X] = 1000.0 * Machine.recipe.Position(curX);
                dTargetValue2[(int)AXIS_DIRECTION2.Y] = 1000.0 * Machine.recipe.Position(curY);
                dTargetValue2[(int)AXIS_DIRECTION2.R1] = 1000.0 * Machine.recipe.Position(curR1);
                dTargetValue2[(int)AXIS_DIRECTION2.R2] = 1000.0 * Machine.recipe.Position(curR2);
                dTargetValue2[(int)AXIS_DIRECTION2.Z] = 1000.0 * Machine.recipe.Position(curZ);

                teachingInputValue1.TargetPos = dTargetValue2[(int)AXIS_DIRECTION2.X] / 1000.0;
                teachingInputValue2.TargetPos = dTargetValue2[(int)AXIS_DIRECTION2.Y] / 1000.0;
                teachingInputValue3.TargetPos = dTargetValue2[(int)AXIS_DIRECTION2.R1] / 1000.0;
                teachingInputValue4.TargetPos = dTargetValue2[(int)AXIS_DIRECTION2.R2] / 1000.0;
                teachingInputValue5.TargetPos = dTargetValue2[(int)AXIS_DIRECTION2.Z] / 1000.0;

                maxX = Machine.recipe.PositionMAX(curX);
                maxY = Machine.recipe.PositionMAX(curY);
                maxR1 = Machine.recipe.PositionMAX(curR1);
                maxR2 = Machine.recipe.PositionMAX(curR2);
                maxZ = Machine.recipe.PositionMAX(curZ);

                minX = Machine.recipe.PositionMIN(curX);
                minY = Machine.recipe.PositionMIN(curY);
                minR1 = Machine.recipe.PositionMIN(curR1);
                minR2 = Machine.recipe.PositionMIN(curR2);
                minZ = Machine.recipe.PositionMIN(curZ);

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
                SelectButton_ClickEvent(nV_Button_TG_NS_XYRZLeftPickUp, null);
                firstStart = false;
            }
        }
        #endregion
        #region BUTTON_CLICK_FUNCTION
        public void SetBtnMouseEvents(NV_UI.NV_Button_PB_NS.ClickEventDelegate functionBtnEvent)
        {
            nV_Button_TG_NS_XYRZLeftPickUp.ClickEvent += SelectButton_ClickEvent;
            nV_Button_TG_NS_XYRZRightPickUp.ClickEvent += SelectButton_ClickEvent;
            nV_Button_TG_NS_XYRZLeftAlign.ClickEvent += SelectButton_ClickEvent;
            nV_Button_TG_NS_XYRZRightAlign.ClickEvent += SelectButton_ClickEvent; 
            nV_Button_TG_NS_XYRZLeftPlace.ClickEvent += SelectButton_ClickEvent;
            nV_Button_TG_NS_XYRZRightPlace.ClickEvent += SelectButton_ClickEvent;
            nV_Button_TG_NS_XYRZLeftScanner.ClickEvent += SelectButton_ClickEvent;
            nV_Button_TG_NS_XYRZRightScanner.ClickEvent += SelectButton_ClickEvent;

            listTeachingPointBtn.Add(nV_Button_TG_NS_XYRZLeftPickUp); 
            listTeachingPointBtn.Add(nV_Button_TG_NS_XYRZRightPickUp);
            listTeachingPointBtn.Add(nV_Button_TG_NS_XYRZLeftAlign);
            listTeachingPointBtn.Add(nV_Button_TG_NS_XYRZRightAlign);
            listTeachingPointBtn.Add(nV_Button_TG_NS_XYRZLeftPlace);
            listTeachingPointBtn.Add(nV_Button_TG_NS_XYRZRightPlace);
            listTeachingPointBtn.Add(nV_Button_TG_NS_XYRZLeftScanner);
            listTeachingPointBtn.Add(nV_Button_TG_NS_XYRZRightScanner);

            nV_Button_PB_NS_MoveTarget.ClickEvent += functionBtnEvent;
            nV_Button_PB_NS_SaveCurrentPos.ClickEvent += functionBtnEvent;
            nV_Button_PB_NS_SaveTargetPos.ClickEvent += functionBtnEvent;
            nV_Button_PB_NS_SetVelSlow.ClickEvent += functionBtnEvent;
            nV_Button_PB_NS_SetVelNormal.ClickEvent += functionBtnEvent;
            nV_Button_PB_NS_SetVelFast.ClickEvent += functionBtnEvent;

            teachingInputValue1.btnEnableCallbackEvent += CheckBoxClickEvent;
            teachingInputValue2.btnEnableCallbackEvent += CheckBoxClickEvent;
            teachingInputValue3.btnEnableCallbackEvent += CheckBoxClickEvent;
            teachingInputValue4.btnEnableCallbackEvent += CheckBoxClickEvent;
            teachingInputValue5.btnEnableCallbackEvent += CheckBoxClickEvent;

            teachingInputValue1.clickEvent += TargetInputClickEvent;
            teachingInputValue2.clickEvent += TargetInputClickEvent;
            teachingInputValue3.clickEvent += TargetInputClickEvent;
            teachingInputValue4.clickEvent += TargetInputClickEvent;
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
                    LogUtil.Instance.Log(LOG_TYPE.UI, "Assembler Move Target Button Click", CONTENT_TYPE.INFO);

                    Dlg_MessageBox formMove = new Dlg_MessageBox(EMESSAGEBOX.WARNING, string.Format("축 이동 하시겠습니까?"));
                    if (formMove.ShowDialog() == DialogResult.Yes)
                    {
                        if (teachingInputValue1.Clicked)
                            MoveTarget(AXIS_DIRECTION2.X);
                        if (teachingInputValue2.Clicked)
                            MoveTarget(AXIS_DIRECTION2.Y);
                        if (teachingInputValue3.Clicked)
                            MoveTarget(AXIS_DIRECTION2.R1);
                        if (teachingInputValue4.Clicked)
                            MoveTarget(AXIS_DIRECTION2.R2);
                        if (teachingInputValue5.Clicked)
                            MoveTarget(AXIS_DIRECTION2.Z);
                        break;
                    }
                    break;
                case TEACHING_TASK.SAVE_CURRENT:
                    RecipeDefine.POSITION dCurDir = RecipeDefine.POSITION.POSITION_UNKNOWN;
                    double dCurNewPos = 0;
                    if (teachingInputValue1.Clicked)
                    {
                        dCurDir = curX; dCurNewPos = (double)teachingInputValue1.CurrentPos;
                    }
                    else if (teachingInputValue2.Clicked)
                    {
                        dCurDir = curY; dCurNewPos = (double)teachingInputValue2.CurrentPos;
                    }
                    else if (teachingInputValue3.Clicked)
                    {
                        dCurDir = curR1; dCurNewPos = (double)teachingInputValue3.CurrentPos;
                    }
                    else if (teachingInputValue4.Clicked)
                    {
                        dCurDir = curR2; dCurNewPos = (double)teachingInputValue4.CurrentPos;
                    }
                    else if (teachingInputValue5.Clicked)
                    {
                        dCurDir = curZ; dCurNewPos = (double)teachingInputValue5.CurrentPos;
                    }
                    else
                        return;
                    double dDifCur = dCurNewPos - Machine.recipe.Position(dCurDir);
                    Dlg_MessageBox formCurrent = new Dlg_MessageBox(EMESSAGEBOX.WARNING, string.Format(dDifCur.ToString("N3") + " 만큼 차이가 발생했습니다.\n" + "Current 값을 저장하시겠습니까?"));
                    if (formCurrent.ShowDialog() == DialogResult.Yes)
                    {
                        if (teachingInputValue1.Clicked) SaveNewPos((double)teachingInputValue1.CurrentPos, AXIS_DIRECTION2.X);
                        if (teachingInputValue2.Clicked) SaveNewPos((double)teachingInputValue2.CurrentPos, AXIS_DIRECTION2.Y);
                        if (teachingInputValue3.Clicked) SaveNewPos((double)teachingInputValue3.CurrentPos, AXIS_DIRECTION2.R1);
                        if (teachingInputValue4.Clicked) SaveNewPos((double)teachingInputValue4.CurrentPos, AXIS_DIRECTION2.R2);
                        if (teachingInputValue5.Clicked) SaveNewPos((double)teachingInputValue5.CurrentPos, AXIS_DIRECTION2.Z);
                    }
                    break;
                case TEACHING_TASK.SAVE_TARGET:
                    RecipeDefine.POSITION dTarDir = RecipeDefine.POSITION.POSITION_UNKNOWN;
                    double dTarNewPos = 0;
                    if (teachingInputValue1.Clicked)
                    {
                        dTarDir = curX; dTarNewPos = (double)teachingInputValue1.TargetPos;
                    }
                    else if (teachingInputValue2.Clicked)
                    {
                        dTarDir = curY; dTarNewPos = (double)teachingInputValue2.TargetPos;
                    }
                    else if (teachingInputValue3.Clicked)
                    {
                        dTarDir = curR1; dTarNewPos = (double)teachingInputValue3.TargetPos;
                    }
                    else if (teachingInputValue4.Clicked)
                    {
                        dTarDir = curR2; dTarNewPos = (double)teachingInputValue4.TargetPos;
                    }
                    else if (teachingInputValue5.Clicked)
                    {
                        dTarDir = curZ; dTarNewPos = (double)teachingInputValue5.TargetPos;
                    }
                    else
                        return;
                    double dDifTar = dTarNewPos - Machine.recipe.Position(dTarDir);
                    Dlg_MessageBox formTarget = new Dlg_MessageBox(EMESSAGEBOX.WARNING, string.Format(dDifTar.ToString("N3") + " 만큼 차이가 발생했습니다.\n" + "Target 값을 저장하시겠습니까?"));
                    if (formTarget.ShowDialog() == DialogResult.Yes)
                    {
                        if (teachingInputValue1.Clicked) SaveNewPos((double)teachingInputValue1.TargetPos, AXIS_DIRECTION2.X);
                        if (teachingInputValue2.Clicked) SaveNewPos((double)teachingInputValue2.TargetPos, AXIS_DIRECTION2.Y);
                        if (teachingInputValue3.Clicked) SaveNewPos((double)teachingInputValue3.TargetPos, AXIS_DIRECTION2.R1);
                        if (teachingInputValue4.Clicked) SaveNewPos((double)teachingInputValue4.TargetPos, AXIS_DIRECTION2.R2);
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

            Machine.interfer_release_y_crash = false;
            Machine.interfer_release_x_safe_area = false;

            Action action = null;
            int errcode = 0;
            int iAxisdx = 0;
            int iAxisdx2 = 0;
            double vel_x = 1000 * Machine.param.Velocity(ParameterDefine.VELOCITY.ASSEMBLER_X_MOVE_VEL);
            double acc_x = 1000 * Machine.param.Velocity(ParameterDefine.VELOCITY.ASSEMBLER_X_MOVE_ACC);
            double dec_x = 1000 * Machine.param.Velocity(ParameterDefine.VELOCITY.ASSEMBLER_X_MOVE_DEC);
            double vel_y = 1000 * Machine.param.Velocity(ParameterDefine.VELOCITY.ASSEMBLER_Y_MOVE_VEL);
            double acc_y = 1000 * Machine.param.Velocity(ParameterDefine.VELOCITY.ASSEMBLER_Y_MOVE_ACC);
            double dec_y = 1000 * Machine.param.Velocity(ParameterDefine.VELOCITY.ASSEMBLER_Y_MOVE_DEC);
            double vel_r = 1000 * Machine.param.Velocity(ParameterDefine.VELOCITY.ASSEMBLER_R_MOVE_VEL);
            double acc_r = 1000 * Machine.param.Velocity(ParameterDefine.VELOCITY.ASSEMBLER_R_MOVE_ACC);
            double dec_r = 1000 * Machine.param.Velocity(ParameterDefine.VELOCITY.ASSEMBLER_R_MOVE_DEC);
            double vel_z = 1000 * Machine.param.Velocity(ParameterDefine.VELOCITY.ASSEMBLER_Z_MOVE_VEL);
            double acc_z = 1000 * Machine.param.Velocity(ParameterDefine.VELOCITY.ASSEMBLER_Z_MOVE_ACC);
            double dec_z = 1000 * Machine.param.Velocity(ParameterDefine.VELOCITY.ASSEMBLER_Z_MOVE_DEC);
            double pos = 0.0;

            action = new Action(delegate
            {
                switch (Pos)
                {
                    case AXIS_DIRECTION2.X:
                        iAxisdx = (int)AXIS.ASSEMBLER_X;
                        dTargetValue2[(int)Pos] = teachingInputValue1.TargetPos * 1000.0;
                        Machine.motion.MoveAxisAbs(iAxisdx, dTargetValue2[(int)Pos], vel_x, acc_x, dec_x);
                        break;
                    case AXIS_DIRECTION2.Y:
                        iAxisdx = (int)AXIS.ASSEMBLER_Y;
                        dTargetValue2[(int)Pos] = teachingInputValue2.TargetPos * 1000.0;
                        Machine.motion.MoveAxisAbs(iAxisdx, dTargetValue2[(int)Pos], vel_y, acc_y, dec_y);
                        break;
                    case AXIS_DIRECTION2.R1:
                        iAxisdx = (int)AXIS.ASSEMBLER_R1;
                        dTargetValue2[(int)Pos] = teachingInputValue3.TargetPos * 1000.0;
                        Machine.motion.MoveAxisAbs(iAxisdx, dTargetValue2[(int)Pos], vel_r, acc_r, dec_r);
                        break;
                    case AXIS_DIRECTION2.R2:
                        iAxisdx = (int)AXIS.ASSEMBLER_R2;
                        dTargetValue2[(int)Pos] = teachingInputValue4.TargetPos * 1000.0;
                        Machine.motion.MoveAxisAbs(iAxisdx, dTargetValue2[(int)Pos], vel_r, acc_r, dec_r);
                        break;
                    case AXIS_DIRECTION2.Z:
                        iAxisdx = (int)AXIS.ASSEMBLER_Z;
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
                    case "InputX":
                        if (teachingInputValue2.Clicked)
                            teachingInputValue2.Clicked = false;
                        if (teachingInputValue3.Clicked)
                            teachingInputValue3.Clicked = false;
                        if (teachingInputValue4.Clicked)
                            teachingInputValue4.Clicked = false;
                        if (teachingInputValue5.Clicked)
                            teachingInputValue5.Clicked = false;
                        break;
                    case "InputY":
                        if (teachingInputValue1.Clicked)
                            teachingInputValue1.Clicked = false;
                        if (teachingInputValue3.Clicked)
                            teachingInputValue3.Clicked = false;
                        if (teachingInputValue4.Clicked)
                            teachingInputValue4.Clicked = false;
                        if (teachingInputValue5.Clicked)
                            teachingInputValue5.Clicked = false;
                        break;
                    case "InputR1":
                        if (teachingInputValue1.Clicked)
                            teachingInputValue1.Clicked = false;
                        if (teachingInputValue2.Clicked)
                            teachingInputValue2.Clicked = false;
                        if (teachingInputValue4.Clicked)
                            teachingInputValue4.Clicked = false;
                        if (teachingInputValue5.Clicked)
                            teachingInputValue5.Clicked = false;
                        break;
                    case "InputR2":
                        if (teachingInputValue1.Clicked)
                            teachingInputValue1.Clicked = false;
                        if (teachingInputValue2.Clicked)
                            teachingInputValue2.Clicked = false;
                        if (teachingInputValue3.Clicked)
                            teachingInputValue3.Clicked = false;
                        if (teachingInputValue5.Clicked)
                            teachingInputValue5.Clicked = false;
                        break;
                    case "InputZ":
                        if (teachingInputValue1.Clicked)
                            teachingInputValue1.Clicked = false;
                        if (teachingInputValue2.Clicked)
                            teachingInputValue2.Clicked = false;
                        if (teachingInputValue3.Clicked)
                            teachingInputValue3.Clicked = false;
                        if (teachingInputValue4.Clicked)
                            teachingInputValue4.Clicked = false;
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
                    case "InputX":
                        min = minX; max = maxX;
                        break;
                    case "InputY":
                        min = minY; max = maxY;
                        break;
                    case "InputR1":
                        min = minR1; max = maxR1;
                        break;
                    case "InputR2":
                        min = minR2; max = maxR2;
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
            double MaxSpeedXY = 1000 * 1000;
            double MinSpeedXY = 1 * 1000;
            double MaxSpeedRZ = 300 * 1000;
            double MinSpeedRZ = 1 * 1000;

            switch (axis)
            {
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
            MoveJogAxis((int)AXIS.ASSEMBLER_R1, true);
        }

        private void button_R1Plus_MouseUp(object sender, MouseEventArgs e)
        {
            Machine.motion.AxisStopAll(true);
            Machine.usingMoveJog = false;
        }

        private void button_R1Minus_MouseDown(object sender, MouseEventArgs e)
        {
            MoveJogAxis((int)AXIS.ASSEMBLER_R1, false);
        }

        private void button_R1Minus_MouseUp(object sender, MouseEventArgs e)
        {
            Machine.motion.AxisStopAll(true);
            Machine.usingMoveJog = false;
        }

        private void button_R2Plus_MouseDown(object sender, MouseEventArgs e)
        {
            MoveJogAxis((int)AXIS.ASSEMBLER_R2, true);
        }

        private void button_R2Plus_MouseUp(object sender, MouseEventArgs e)
        {
            Machine.motion.AxisStopAll(true);
            Machine.usingMoveJog = false;
        }

        private void button_R2Minus_MouseDown(object sender, MouseEventArgs e)
        {
            MoveJogAxis((int)AXIS.ASSEMBLER_R2, false);
        }

        private void button_R2Minus_MouseUp(object sender, MouseEventArgs e)
        {
            Machine.motion.AxisStopAll(true);
            Machine.usingMoveJog = false;
        }

        private void button_YPlus_MouseDown(object sender, MouseEventArgs e)
        {
            Machine.interfer_release_y_crash = true;
            Machine.interfer_jog_y_crash = true;
            MoveJogAxis((int)AXIS.ASSEMBLER_Y, true);
        }

        private void button_YPlus_MouseUp(object sender, MouseEventArgs e)
        {
            Machine.interfer_release_y_crash = false;
            Machine.interfer_jog_y_crash = false;
            Machine.motion.AxisStopAll(true);
            Machine.usingMoveJog = false;
        }

        private void button_YMinus_MouseDown(object sender, MouseEventArgs e)
        {
            Machine.interfer_release_y_crash = true;
            MoveJogAxis((int)AXIS.ASSEMBLER_Y, false);
        }

        private void button_YMinus_MouseUp(object sender, MouseEventArgs e)
        {
            Machine.interfer_release_y_crash = false;
            Machine.motion.AxisStopAll(true);
            Machine.usingMoveJog = false;
        }

        private void button_XPlus_MouseDown(object sender, MouseEventArgs e)
        {
            MoveJogAxis((int)AXIS.ASSEMBLER_X, true);
        }

        private void button_XPlus_MouseUp(object sender, MouseEventArgs e)
        {
            Machine.motion.AxisStopAll(true);
            Machine.usingMoveJog = false;
        }

        private void button_XMinus_MouseDown(object sender, MouseEventArgs e)
        {
            MoveJogAxis((int)AXIS.ASSEMBLER_X, false);
        }

        private void button_XMinus_MouseUp(object sender, MouseEventArgs e)
        {
            Machine.motion.AxisStopAll(true);
            Machine.usingMoveJog = false;
        }

        private void button_ZPlus_MouseDown(object sender, MouseEventArgs e)
        {
            MoveJogAxis((int)AXIS.ASSEMBLER_Z, true);
        }

        private void button_ZPlus_MouseUp(object sender, MouseEventArgs e)
        {
            Machine.motion.AxisStopAll(true);
            Machine.usingMoveJog = false;
        }

        private void button_ZMinus_MouseDown(object sender, MouseEventArgs e)
        {
            MoveJogAxis((int)AXIS.ASSEMBLER_Z, false);
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
                Machine.motion.GetAxisServoOn((int)AXIS.ASSEMBLER_X, ref servoStatus);
                if (servoStatus)
                    Machine.motion.SetAxisServoOn((int)AXIS.ASSEMBLER_X, 0);
                else
                    Machine.motion.SetAxisServoOn((int)AXIS.ASSEMBLER_X, 1);
            }
            if (teachingInputValue2.Clicked)
            {
                Machine.motion.GetAxisServoOn((int)AXIS.ASSEMBLER_Y, ref servoStatus);
                if (servoStatus)
                    Machine.motion.SetAxisServoOn((int)AXIS.ASSEMBLER_Y, 0);
                else
                    Machine.motion.SetAxisServoOn((int)AXIS.ASSEMBLER_Y, 1);
            }
            if (teachingInputValue3.Clicked)
            {
                Machine.motion.GetAxisServoOn((int)AXIS.ASSEMBLER_R1, ref servoStatus);
                if (servoStatus)
                    Machine.motion.SetAxisServoOn((int)AXIS.ASSEMBLER_R1, 0);
                else
                    Machine.motion.SetAxisServoOn((int)AXIS.ASSEMBLER_R1, 1);
            }
            if (teachingInputValue4.Clicked)
            {
                Machine.motion.GetAxisServoOn((int)AXIS.ASSEMBLER_R2, ref servoStatus);
                if (servoStatus)
                    Machine.motion.SetAxisServoOn((int)AXIS.ASSEMBLER_R2, 0);
                else
                    Machine.motion.SetAxisServoOn((int)AXIS.ASSEMBLER_R2, 1);
            }
            if (teachingInputValue5.Clicked)
            {
                Machine.motion.GetAxisServoOn((int)AXIS.ASSEMBLER_Z, ref servoStatus);
                if (servoStatus)
                    Machine.motion.SetAxisServoOn((int)AXIS.ASSEMBLER_Z, 0);
                else
                    Machine.motion.SetAxisServoOn((int)AXIS.ASSEMBLER_Z, 1);
            }        
        }

        private void button_AlarmReset_Click(object sender, EventArgs e)
        {
            for (int i = (int)AXIS.ASSEMBLER_X; i <= (int)AXIS.ASSEMBLER_R2; ++i)
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

            Machine.IO.GetIn((int)DI.ASSEMBLER_Z1_UP, ref ToolZUp);

            if (ToolZUp == 1)
            {
                LogUtil.Instance.Log(LOG_TYPE.UI, "Assembler Left Picker Down Button Click", CONTENT_TYPE.INFO);

                Machine.IO.SetOut((int)DO.ASSEMBLER_Z1_UP, 0);
                Machine.IO.SetOut((int)DO.ASSEMBLER_Z1_DOWN, 1);
            }
            else
            {
                LogUtil.Instance.Log(LOG_TYPE.UI, "Assembler Left Picker Up Button Click", CONTENT_TYPE.INFO);

                Machine.IO.SetOut((int)DO.ASSEMBLER_Z1_UP, 1);
                Machine.IO.SetOut((int)DO.ASSEMBLER_Z1_DOWN, 0);
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

            Machine.IO.GetIn((int)DI.ASSEMBLER_Z2_UP, ref ToolZUp);

            if (ToolZUp == 1)
            {
                LogUtil.Instance.Log(LOG_TYPE.UI, "Assembler Right Picker Down Button Click", CONTENT_TYPE.INFO);

                Machine.IO.SetOut((int)DO.ASSEMBLER_Z2_UP, 0);
                Machine.IO.SetOut((int)DO.ASSEMBLER_Z2_DOWN, 1);
            }
            else
            {
                LogUtil.Instance.Log(LOG_TYPE.UI, "Assembler Right Picker Up Button Click", CONTENT_TYPE.INFO);

                Machine.IO.SetOut((int)DO.ASSEMBLER_Z2_UP, 1);
                Machine.IO.SetOut((int)DO.ASSEMBLER_Z2_DOWN, 0);
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

            Machine.IO.GetOut((int)DO.ASSEMBLER_Z1_VACON, ref ToolVacOn);

            if (ToolVacOn == 1)
            {
                LogUtil.Instance.Log(LOG_TYPE.UI, "Assembler Left Vacuum Off Button Click", CONTENT_TYPE.INFO);

                Machine.IO.SetOut((int)DO.ASSEMBLER_Z1_VACON, 0);
                Machine.IO.SetOut((int)DO.ASSEMBLER_Z1_PURGE, 1);
                Util.Delay((int)Machine.param.Time(TIME.PURGE_OFF_WAIT_TIME));
                Machine.IO.SetOut((int)DO.ASSEMBLER_Z1_PURGE, 0);
            }
            else
            {
                LogUtil.Instance.Log(LOG_TYPE.UI, "Assembler Left Vacuum On Button Click", CONTENT_TYPE.INFO);

                Machine.IO.SetOut((int)DO.ASSEMBLER_Z1_VACON, 1);
                Machine.IO.SetOut((int)DO.ASSEMBLER_Z1_PURGE, 0);
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

            Machine.IO.GetOut((int)DO.ASSEMBLER_Z2_VACON, ref ToolVacOn);

            if (ToolVacOn == 1)
            {
                LogUtil.Instance.Log(LOG_TYPE.UI, "Assembler Right Vacuum Off Button Click", CONTENT_TYPE.INFO);

                Machine.IO.SetOut((int)DO.ASSEMBLER_Z2_VACON, 0);
                Machine.IO.SetOut((int)DO.ASSEMBLER_Z2_PURGE, 1);
                Util.Delay((int)Machine.param.Time(TIME.PURGE_OFF_WAIT_TIME));
                Machine.IO.SetOut((int)DO.ASSEMBLER_Z2_PURGE, 0);
            }
            else
            {
                LogUtil.Instance.Log(LOG_TYPE.UI, "Assembler Right Vacuum On Button Click", CONTENT_TYPE.INFO);

                Machine.IO.SetOut((int)DO.ASSEMBLER_Z2_VACON, 1);
                Machine.IO.SetOut((int)DO.ASSEMBLER_Z2_PURGE, 0);
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
                        posName = Machine.recipe.PositionNAME(curX);
                        dOldPos = Machine.recipe.Position(curX);
                        Machine.recipe.Position(curX, dNewPos);
                        break;
                    case AXIS_DIRECTION2.Y:
                        teachingInputValue2.SetTargetPos(dNewPos);
                        posName = Machine.recipe.PositionNAME(curY);
                        dOldPos = Machine.recipe.Position(curY);
                        Machine.recipe.Position(curY, dNewPos);
                        break;
                    case AXIS_DIRECTION2.R1:
                        teachingInputValue3.SetTargetPos(dNewPos);
                        posName = Machine.recipe.PositionNAME(curR1);
                        dOldPos = Machine.recipe.Position(curR1);
                        Machine.recipe.Position(curR1, dNewPos);
                        break;
                    case AXIS_DIRECTION2.R2:
                        teachingInputValue4.SetTargetPos(dNewPos);
                        posName = Machine.recipe.PositionNAME(curR2);
                        dOldPos = Machine.recipe.Position(curR2);
                        Machine.recipe.Position(curR2, dNewPos);
                        break;
                    case AXIS_DIRECTION2.Z:
                        teachingInputValue5.SetTargetPos(dNewPos);
                        posName = Machine.recipe.PositionNAME(curZ);
                        dOldPos = Machine.recipe.Position(curZ);
                        Machine.recipe.Position(curZ, dNewPos);
                        break;
                }
                Machine.recipe.Write();
                string unit = Machine.recipe.PositionUNIT(curX); //"mm"
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
            if(tabControl2.SelectedIndex == 0) //PP
            {
                curPage = AssemblerPAGE.PP;
                tabAssemblerUnderCalib.StartTimer(false);
                tabAssemblerJigCalib.StartTimer(false);
                tabAssemblerUnderCalib.Hide();
                tabAssemblerJigCalib.Hide();
            }
            else if (tabControl2.SelectedIndex == 1) //Vision
            {
                if (tabControl3.SelectedIndex == 0) //Under
                {
                    curPage = AssemblerPAGE.VISION_UNDER;
                    tabAssemblerUnderCalib.StartTimer(true);
                    tabAssemblerUnderCalib.Show();
                    tabAssemblerUnderCalib.UpdateLightSetting();

                    tabAssemblerJigCalib.StartTimer(false);
                    tabAssemblerJigCalib.Hide();
                }
                else if (tabControl3.SelectedIndex == 1) //Jig
                {
                    curPage = AssemblerPAGE.VISION_JIG;
                    tabAssemblerUnderCalib.StartTimer(false);
                    tabAssemblerUnderCalib.Hide();

                    tabAssemblerJigCalib.StartTimer(true);
                    tabAssemblerJigCalib.Show();
                    tabAssemblerJigCalib.UpdateLightSetting();
                }
            }
        }
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
            if(tabControl1.SelectedIndex == 0)
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

        void UpdateTeachingValue()
        {
            if (tabControl2.SelectedIndex == 0) //PP
                SelectButton_ClickEvent(listTeachingPointBtn[(int)mCurrentTeachingPoint], null);
            else if (tabControl2.SelectedIndex == 1) //Vision
                if (tabControl3.SelectedIndex == 0) //Under
                {
                    tabAssemblerUnderCalib.UpdateLightSetting();
                }
                else if (tabControl3.SelectedIndex == 1)
                {
                    tabAssemblerJigCalib.UpdateLightSetting();
                }
        }
    }
}
