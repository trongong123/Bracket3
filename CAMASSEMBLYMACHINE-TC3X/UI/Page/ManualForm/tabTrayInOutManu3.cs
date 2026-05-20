using static CAMASSEMBLYMACHINE.Define.SystemDefine;
using static CAMASSEMBLYMACHINE.Define.UIDefine;
using CAMASSEMBLYMACHINE.Process;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TopEng.Type;
using NV_UI;
using TopEng.Controls;
using TopEng.Utils;
using CAMASSEMBLYMACHINE.Define;

namespace CAMASSEMBLYMACHINE.UI
{
    public partial class tabTrayConvManu3 : Form, IForm
    {
        List<Label> label_alignPos = new List<Label>();

        public tabTrayConvManu3()
        {
            InitializeComponent();

            label_alignPos.Add(disp_alignPos1);
            label_alignPos.Add(disp_alignPos2);

            // Hide TAB Button
            tabControl1.Appearance = TabAppearance.Buttons;
            tabControl1.SizeMode = TabSizeMode.Fixed;
            tabControl1.ItemSize = new Size(0, 1);

            SetBtnClickEvents();
            SelectButton_ClickEvent(nV_Button_TG_NS_ControlConveyor, null);
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
            UpdateState();
            UpdateIOState();
            UpdateMachineData();
        }

        private void UpdateState()
        {
            disp_TrayTrasnferYReadyPos.ButtonPush = Util.IsAxisInPos(
                (AXIS.TRAY_TRF_Y, ParameterDefine.POSITION.TRAY_TRF_Y_READY_POS));
            disp_TrayTrasnferYLoadingPos.ButtonPush = Util.IsAxisInPos(
                (AXIS.TRAY_TRF_Y, ParameterDefine.POSITION.TRAY_TRF_Y_LOADING_POS));
            disp_TrayTrasnferYUnloadingPos.ButtonPush = Util.IsAxisInPos(
                (AXIS.TRAY_TRF_Y, ParameterDefine.POSITION.TRAY_TRF_Y_UNLOADING_POS));

            disp_TrayTrasnferZReadyPos.ButtonPush = Util.IsAxisInPos(
                (AXIS.TRAY_TRF_Z, ParameterDefine.POSITION.TRAY_TRF_Z_READY_POS));
            disp_TrayTrasnferZLoadingPos.ButtonPush = Util.IsAxisInPos(
                (AXIS.TRAY_TRF_Z, ParameterDefine.POSITION.TRAY_TRF_Z_LOADING_POS));
            disp_TrayTrasnferZUnloadingPos.ButtonPush = Util.IsAxisInPos(
                (AXIS.TRAY_TRF_Z, ParameterDefine.POSITION.TRAY_TRF_Z_UNLOADING_POS));

            disp_TrayWorkZLoadingPos.ButtonPush = Util.IsAxisInPos(
               (AXIS.TRAY_WORK_Z, ParameterDefine.POSITION.TRAY_WORK_LOADING_POS));
            disp_TrayWorkZWorkPos.ButtonPush = Util.IsAxisInPos(
               (AXIS.TRAY_WORK_Z, ParameterDefine.POSITION.TRAY_WORK_WORK_POS));
            disp_TrayWorkZUnloadingPos.ButtonPush = Util.IsAxisInPos(
                (AXIS.TRAY_WORK_Z, ParameterDefine.POSITION.TRAY_WORK_UNLOADING_POS));
        }

        private void UpdateIOState()
        {
            uint returnValue = 0;
            uint returnValue2 = 0;

            if (Machine.IO == null)
                return;

            #region TRAY EXIST
            Machine.IO.GetIn((int)DI.TRAY_IN_DETECT_IN, ref returnValue);
            disp_TrayInIn1.ButtonPush = returnValue == 0 ? false : true;
            Machine.IO.GetIn((int)DI.TRAY_IN_DETECT_MID, ref returnValue);
            disp_TrayInMid1.ButtonPush = returnValue == 0 ? false : true;
            Machine.IO.GetIn((int)DI.TRAY_IN_DETECT_END, ref returnValue);
            disp_TrayInEnd1.ButtonPush = returnValue == 0 ? false : true;
            Machine.IO.GetIn((int)DI.TRAY_IN_DETECT2_IN, ref returnValue);
            disp_TrayInIn2.ButtonPush = returnValue == 0 ? false : true;
            Machine.IO.GetIn((int)DI.TRAY_IN_DETECT2_MID, ref returnValue);
            disp_TrayInMid2.ButtonPush = returnValue == 0 ? false : true;
            Machine.IO.GetIn((int)DI.TRAY_IN_DETECT2_END, ref returnValue);
            disp_TrayInEnd2.ButtonPush = returnValue == 0 ? false : true;

            Machine.IO.GetIn((int)DI.TRAY_IN_BUF_DETECT_IN, ref returnValue);
            disp_TrayInBufIn1.ButtonPush = returnValue == 0 ? false : true;
            Machine.IO.GetIn((int)DI.TRAY_IN_BUF_DETECT_MID, ref returnValue);
            disp_TrayInBufMid1.ButtonPush = returnValue == 0 ? false : true;
            Machine.IO.GetIn((int)DI.TRAY_IN_BUF_DETECT_END, ref returnValue);
            disp_TrayInBufEnd1.ButtonPush = returnValue == 0 ? false : true;
            Machine.IO.GetIn((int)DI.TRAY_IN_BUF_DETECT2_IN, ref returnValue);
            disp_TrayInBufIn2.ButtonPush = returnValue == 0 ? false : true;
            Machine.IO.GetIn((int)DI.TRAY_IN_BUF_DETECT2_MID, ref returnValue);
            disp_TrayInBufMid2.ButtonPush = returnValue == 0 ? false : true;
            Machine.IO.GetIn((int)DI.TRAY_IN_BUF_DETECT2_END, ref returnValue);
            disp_TrayInBufEnd2.ButtonPush = returnValue == 0 ? false : true;

            Machine.IO.GetIn((int)DI.TRAY_WORK_DETECT_IN, ref returnValue);
            disp_TrayWorkIn1.ButtonPush = returnValue == 0 ? false : true;
            Machine.IO.GetIn((int)DI.TRAY_WORK_DETECT_MID, ref returnValue);
            disp_TrayWorkMid1.ButtonPush = returnValue == 0 ? false : true;
            Machine.IO.GetIn((int)DI.TRAY_WORK_DETECT_END, ref returnValue);
            disp_TrayWorkEnd1.ButtonPush = returnValue == 0 ? false : true;
            Machine.IO.GetIn((int)DI.TRAY_WORK_DETECT2_IN, ref returnValue);
            disp_TrayWorkIn2.ButtonPush = returnValue == 0 ? false : true;
            Machine.IO.GetIn((int)DI.TRAY_WORK_DETECT2_MID, ref returnValue);
            disp_TrayWorkMid2.ButtonPush = returnValue == 0 ? false : true;
            Machine.IO.GetIn((int)DI.TRAY_WORK_DETECT2_END, ref returnValue);
            disp_TrayWorkEnd2.ButtonPush = returnValue == 0 ? false : true;

            Machine.IO.GetIn((int)DI.TRAY_OUT_BUF_DETECT_IN, ref returnValue);
            disp_TrayOutBufIn1.ButtonPush = returnValue == 0 ? false : true;
            Machine.IO.GetIn((int)DI.TRAY_OUT_BUF_DETECT_MID, ref returnValue);
            disp_TrayOutBufMid1.ButtonPush = returnValue == 0 ? false : true;
            Machine.IO.GetIn((int)DI.TRAY_OUT_BUF_DETECT_END, ref returnValue);
            disp_TrayOutBufEnd1.ButtonPush = returnValue == 0 ? false : true;
            Machine.IO.GetIn((int)DI.TRAY_OUT_BUF_DETECT2_IN, ref returnValue);
            disp_TrayOutBufIn2.ButtonPush = returnValue == 0 ? false : true;
            Machine.IO.GetIn((int)DI.TRAY_OUT_BUF_DETECT2_MID, ref returnValue);
            disp_TrayOutBufMid2.ButtonPush = returnValue == 0 ? false : true;
            Machine.IO.GetIn((int)DI.TRAY_OUT_BUF_DETECT2_END, ref returnValue);
            disp_TrayOutBufEnd2.ButtonPush = returnValue == 0 ? false : true;

            Machine.IO.GetIn((int)DI.TRAY_OUT_DETECT_IN, ref returnValue);
            disp_TrayOutIn1.ButtonPush = returnValue == 0 ? false : true;
            Machine.IO.GetIn((int)DI.TRAY_OUT_DETECT_MID, ref returnValue);
            disp_TrayOutMid1.ButtonPush = returnValue == 0 ? false : true;
            Machine.IO.GetIn((int)DI.TRAY_OUT_DETECT_END, ref returnValue);
            disp_TrayOutEnd1.ButtonPush = returnValue == 0 ? false : true;
            Machine.IO.GetIn((int)DI.TRAY_OUT_DETECT2_IN, ref returnValue);
            disp_TrayOutIn2.ButtonPush = returnValue == 0 ? false : true;
            Machine.IO.GetIn((int)DI.TRAY_OUT_DETECT2_MID, ref returnValue);
            disp_TrayOutMid2.ButtonPush = returnValue == 0 ? false : true;
            Machine.IO.GetIn((int)DI.TRAY_OUT_DETECT2_END, ref returnValue);
            disp_TrayOutEnd2.ButtonPush = returnValue == 0 ? false : true;

            #endregion
            #region CONVEYOR RUN
            Machine.IO.GetOut((int)DO.TRAY_IN_CONV_RUN, ref returnValue);
            disp_TrayInConvRunCWStop.ButtonPush = returnValue == 1 ? true : false;
            button_TrayInConvRunCWStop.valueEvent = returnValue;
            Machine.IO.GetOut((int)DO.TRAY_IN_CONV_RUN_REVERSE, ref returnValue);
            disp_TrayInConvRunCCWStop.ButtonPush = returnValue == 1;
            button_TrayInConvRunCCWStop.valueEvent = returnValue;

            Machine.IO.GetOut((int)DO.TRAY_IN_BUF_CONV_RUN, ref returnValue);
            disp_TrayInBufConvRunCWStop.ButtonPush = returnValue == 1 ? true : false;
            button_TrayInBufConvRunCWStop.valueEvent = returnValue;
            Machine.IO.GetOut((int)DO.TRAY_IN_BUF_CONV_RUN_REVERSE, ref returnValue);
            disp_TrayInBufConvRunCCWStop.ButtonPush = returnValue == 1;
            button_TrayInBufConvRunCCWStop.valueEvent = returnValue;

            Machine.IO.GetOut((int)DO.TRAY_WORK_CONV_RUN, ref returnValue);
            disp_TrayWorkConvRunCWStop.ButtonPush = returnValue == 1 ? true : false;
            button_TrayWorkConvRunCWStop.valueEvent = returnValue;
            Machine.IO.GetOut((int)DO.TRAY_WORK_CONV_RUN_REVERSE, ref returnValue);
            disp_TrayWorkConvRunCCWStop.ButtonPush = returnValue == 1;
            button_TrayWorkConvRunCCWStop.valueEvent = returnValue;

            Machine.IO.GetOut((int)DO.TRAY_OUT_BUF_CONV_RUN, ref returnValue);
            disp_TrayOutBufConvRunStop.ButtonPush = returnValue == 1 ? true : false;
            button_TrayOutBufConvRunStop.valueEvent = returnValue;

            Machine.IO.GetOut((int)DO.TRAY_OUT_CONV_RUN, ref returnValue);
            disp_TrayOutConvRunStop.ButtonPush = returnValue == 1 ? true : false;
            button_TrayOutConvRunStop.valueEvent = returnValue;

            #endregion
            #region CYLINDER
            Machine.IO.GetIn((int)DI.TRAY_IN_STOPPER_UP, ref returnValue);
            disp_TrayInStopper_Up.ButtonPush = returnValue == 1 ? true : false;
            Machine.IO.GetIn((int)DI.TRAY_IN_STOPPER_DOWN, ref returnValue);
            disp_TrayInStopper_Down.ButtonPush = returnValue == 1 ? true : false;
            Machine.IO.GetOut((int)DO.TRAY_IN_STOPPER_UP, ref returnValue);
            button_TrayInStopper_UpDown.valueEvent = returnValue;

            Machine.IO.GetIn((int)DI.TRAY_IN_BUF_STOPPER_UP, ref returnValue);
            disp_TrayInBufStopper_Up.ButtonPush = returnValue == 1 ? true : false;
            Machine.IO.GetIn((int)DI.TRAY_IN_BUF_STOPPER_DOWN, ref returnValue);
            disp_TrayInBufStopper_Down.ButtonPush = returnValue == 1 ? true : false;
            Machine.IO.GetOut((int)DO.TRAY_IN_BUF_STOPPER_UP, ref returnValue);
            button_TrayInBufStopper_UpDown.valueEvent = returnValue;

            Machine.IO.GetIn((int)DI.TRAY_OUT_BUF_STOPPER_UP, ref returnValue);
            disp_TrayOutBufStopper_Up.ButtonPush = returnValue == 1 ? true : false;
            Machine.IO.GetIn((int)DI.TRAY_OUT_BUF_STOPPER_DOWN, ref returnValue);
            disp_TrayOutBufStopper_Down.ButtonPush = returnValue == 1 ? true : false;
            Machine.IO.GetOut((int)DO.TRAY_OUT_BUF_STOPPER_UP, ref returnValue);
            button_TrayOutBufStopper_UpDown.valueEvent = returnValue;

            Machine.IO.GetOut((int)DO.TRAY_TRF_VACUUM, ref returnValue);
            disp_TrayTRF_Vac_On.ButtonPush = returnValue == 1 ? true : false;
            disp_TrayTRF_Vac_Off.ButtonPush = returnValue == 0 ? true : false;

            Machine.IO.GetOut((int)DO.TRAY_OUT_CONV_UP, ref returnValue);
            disp_TrayOutConveyorUp.ButtonPush = returnValue == 1 ? true : false;
            button_TrayOutConveyorUpDown.valueEvent = returnValue;
            Machine.IO.GetOut((int)DO.TRAY_OUT_CONV_DOWN, ref returnValue);
            disp_TrayOutConveyorDown.ButtonPush = returnValue == 1 ? true : false;

            Machine.IO.GetIn((int)DI.TRAY_TRF_DETECT_L_DOWN, ref returnValue);
            disp_TrayTRFDetectLeftUp.ButtonPush = returnValue == 0 ? true : false;
            disp_TrayTRFDetectLeftDown.ButtonPush = returnValue == 1 ? true : false;
            Machine.IO.GetIn((int)DI.TRAY_TRF_DETECT_R_DOWN, ref returnValue);
            disp_TrayTRFDetectRightUp.ButtonPush = returnValue == 0 ? true : false;
            disp_TrayTRFDetectRightDown.ButtonPush = returnValue == 1 ? true : false;
            Machine.IO.GetIn((int)DI.TRAY_TRF_EXIST_DETECT, ref returnValue);
            disp_TrayTRFExist.ButtonPush = returnValue == 1 ? true : false;

            Machine.IO.GetOut((int)DO.TRAY_TRF_EXIST_UP, ref returnValue);
            disp_TrayTRFExistUp.ButtonPush = returnValue == 1 ? true : false;
            Machine.IO.GetOut((int)DO.TRAY_TRF_EXIST_DOWN, ref returnValue);
            disp_TrayTRFExistDown.ButtonPush = returnValue == 1 ? true : false;
            #endregion
        }

        private void UpdateMachineData()
        {
            var proc = Machine.proclist[(int)Machine.PROCESS.TRAYWORK] as ProcessTrayWork;

            for (int i = 0; i < 2; i++)
            {
                if (proc.alignSucs[i] < 0)
                {
                    label_alignPos[i].Text = $"( none , none , none )";
                }
                else
                {
                    string xpos = proc.productPosition[i].x.ToString("0.000");
                    string ypos = proc.productPosition[i].y.ToString("0.000");
                    string angle = proc.productAngle[i].ToString("0.000");
                    label_alignPos[i].Text = $"( {xpos} , {ypos} , {angle} )";
                }
            }
        }

        #region CYLINDER_CONTROL
        private void button_TrayInStopper_UpDown_Click(object sender, EventArgs e)
        {
            uint ret1 = 0;

            Machine.IO.GetOut((int)DO.TRAY_IN_STOPPER_UP, ref ret1);

            if (ret1 == 0)
            {
                LogUtil.Instance.Log(LOG_TYPE.UI, "Tray In Stopper Up Button Click", CONTENT_TYPE.INFO);

                Machine.IO.SetOut((int)DO.TRAY_IN_STOPPER_UP, 1);
                Machine.IO.SetOut((int)DO.TRAY_IN_STOPPER_DOWN, 0);
            }
            else if (ret1 == 1)
            {
                LogUtil.Instance.Log(LOG_TYPE.UI, "Tray In Stopper Down Button Click", CONTENT_TYPE.INFO);

                Machine.IO.SetOut((int)DO.TRAY_IN_STOPPER_UP, 0);
                Machine.IO.SetOut((int)DO.TRAY_IN_STOPPER_DOWN, 1);
            }
        }

        private void button_TrayInBufStopper_UpDown_Click(object sender, EventArgs e)
        {
            uint ret1 = 0;

            Machine.IO.GetOut((int)DO.TRAY_IN_BUF_STOPPER_UP, ref ret1);

            if (ret1 == 0)
            {
                LogUtil.Instance.Log(LOG_TYPE.UI, "Tray InBuf Stopper Up Button Click", CONTENT_TYPE.INFO);

                Machine.IO.SetOut((int)DO.TRAY_IN_BUF_STOPPER_UP, 1);
                Machine.IO.SetOut((int)DO.TRAY_IN_BUF_STOPPER_DOWN, 0);
            }
            else if (ret1 == 1)
            {
                LogUtil.Instance.Log(LOG_TYPE.UI, "Tray InBuf Stopper Down Button Click", CONTENT_TYPE.INFO);

                Machine.IO.SetOut((int)DO.TRAY_IN_BUF_STOPPER_UP, 0);
                Machine.IO.SetOut((int)DO.TRAY_IN_BUF_STOPPER_DOWN, 1);
            }
        }

        private void button_TrayOutBufStopper_UpDown_Click(object sender, EventArgs e)
        {
            uint ret1 = 0;

            Machine.IO.GetOut((int)DO.TRAY_OUT_BUF_STOPPER_UP, ref ret1);

            if (ret1 == 0)
            {
                LogUtil.Instance.Log(LOG_TYPE.UI, "Tray OutBuf Stopper Up Button Click", CONTENT_TYPE.INFO);

                Machine.IO.SetOut((int)DO.TRAY_OUT_BUF_STOPPER_UP, 1);
                Machine.IO.SetOut((int)DO.TRAY_OUT_BUF_STOPPER_DOWN, 0);
            }
            else if (ret1 == 1)
            {
                LogUtil.Instance.Log(LOG_TYPE.UI, "Tray OutBuf Stopper Down Button Click", CONTENT_TYPE.INFO);

                Machine.IO.SetOut((int)DO.TRAY_OUT_BUF_STOPPER_UP, 0);
                Machine.IO.SetOut((int)DO.TRAY_OUT_BUF_STOPPER_DOWN, 1);
            }
        }

        private void button_TrayTRF_Vac_OnOff_Click(object sender, EventArgs e)
        {
            uint ret1 = 0;

            Machine.IO.GetOut((int)DO.TRAY_TRF_VACUUM, ref ret1);

            if (ret1 == 0)
            {
                LogUtil.Instance.Log(LOG_TYPE.UI, "Tray Transfer Vacuum On Button Click", CONTENT_TYPE.INFO);

                Machine.IO.SetOut((int)DO.TRAY_TRF_VACUUM, 1);
            }
            else if (ret1 == 1)
            {
                LogUtil.Instance.Log(LOG_TYPE.UI, "Tray Transfer Vacuum Off Button Click", CONTENT_TYPE.INFO);

                Machine.IO.SetOut((int)DO.TRAY_TRF_VACUUM, 0);
            }
        }
        #endregion
        #region CONVEYOR
        private void button_TrayInConvRunCWStop_Click(object sender, EventArgs e)
        {
            uint ret1 = 0; uint ret2 = 0;

            Machine.IO.GetOut((int)DO.TRAY_IN_CONV_RUN, ref ret1);
            Machine.IO.GetOut((int)DO.TRAY_IN_CONV_RUN_REVERSE, ref ret2);

            if (ret1 == 1)
            {
                LogUtil.Instance.Log(LOG_TYPE.UI, "Tray In Conveyor Stop Button Click", CONTENT_TYPE.INFO);

                Machine.IO.SetOut((int)DO.TRAY_IN_CONV_RUN, 0);
                Machine.IO.SetOut((int)DO.TRAY_IN_CONV_RUN_REVERSE, 0);
            }
            else
            {
                LogUtil.Instance.Log(LOG_TYPE.UI, "Tray In Conveyor Run CW Button Click", CONTENT_TYPE.INFO);

                Machine.IO.SetOut((int)DO.TRAY_IN_CONV_RUN, 1);
                Machine.IO.SetOut((int)DO.TRAY_IN_CONV_RUN_REVERSE, 0);
            }
        }

        private void button_TrayInConvRunCCWStop_Click(object sender, EventArgs e)
        {
            uint ret1 = 0;  uint ret2 = 0; 

            Machine.IO.GetOut((int)DO.TRAY_IN_CONV_RUN, ref ret1);
            Machine.IO.GetOut((int)DO.TRAY_IN_CONV_RUN_REVERSE, ref ret2);

            if (ret2 == 1)
            {
                LogUtil.Instance.Log(LOG_TYPE.UI, "Tray In Conveyor Stop Button Click", CONTENT_TYPE.INFO);

                Machine.IO.SetOut((int)DO.TRAY_IN_CONV_RUN, 0);
                Machine.IO.SetOut((int)DO.TRAY_IN_CONV_RUN_REVERSE, 0);
            }
            else
            {
                LogUtil.Instance.Log(LOG_TYPE.UI, "Tray In Conveyor Run CCW Button Click", CONTENT_TYPE.INFO);

                Machine.IO.SetOut((int)DO.TRAY_IN_CONV_RUN, 0);
                Machine.IO.SetOut((int)DO.TRAY_IN_CONV_RUN_REVERSE, 1);
            }
        }

        private void button_TrayInBufConvRunCWStop_Click(object sender, EventArgs e)
        {
            uint ret1 = 0; uint ret2 = 0;

            Machine.IO.GetOut((int)DO.TRAY_IN_BUF_CONV_RUN, ref ret1);
            Machine.IO.GetOut((int)DO.TRAY_IN_BUF_CONV_RUN_REVERSE, ref ret2);

            if (ret1 == 1)
            {
                LogUtil.Instance.Log(LOG_TYPE.UI, "Tray In Buf Conveyor Stop Button Click", CONTENT_TYPE.INFO);

                Machine.IO.SetOut((int)DO.TRAY_IN_BUF_CONV_RUN, 0);
                Machine.IO.SetOut((int)DO.TRAY_IN_BUF_CONV_RUN_REVERSE, 0);
            }
            else
            {
                LogUtil.Instance.Log(LOG_TYPE.UI, "Tray In Buf Conveyor Run CW Button Click", CONTENT_TYPE.INFO);

                Machine.IO.SetOut((int)DO.TRAY_IN_BUF_CONV_RUN, 1);
                Machine.IO.SetOut((int)DO.TRAY_IN_BUF_CONV_RUN_REVERSE, 0);
            }
        }

        private void button_TrayInBufConvRunCCWStop_Click(object sender, EventArgs e)
        {
            uint ret1 = 0; uint ret2 = 0;

            Machine.IO.GetOut((int)DO.TRAY_IN_BUF_CONV_RUN, ref ret1);
            Machine.IO.GetOut((int)DO.TRAY_IN_BUF_CONV_RUN_REVERSE, ref ret2);

            if (ret2 == 1)
            {
                LogUtil.Instance.Log(LOG_TYPE.UI, "Tray In Buf Conveyor Stop Button Click", CONTENT_TYPE.INFO);

                Machine.IO.SetOut((int)DO.TRAY_IN_BUF_CONV_RUN, 0);
                Machine.IO.SetOut((int)DO.TRAY_IN_BUF_CONV_RUN_REVERSE, 0);
            }
            else
            {
                LogUtil.Instance.Log(LOG_TYPE.UI, "Tray In Buf Conveyor Run CCW Button Click", CONTENT_TYPE.INFO);

                Machine.IO.SetOut((int)DO.TRAY_IN_BUF_CONV_RUN, 0);
                Machine.IO.SetOut((int)DO.TRAY_IN_BUF_CONV_RUN_REVERSE, 1);
            }
        }

        private void button_TrayWorkConvRunCWStop_Click(object sender, EventArgs e)
        {
            uint ret1 = 0; uint ret2 = 0;

            Machine.IO.GetOut((int)DO.TRAY_WORK_CONV_RUN, ref ret1);
            Machine.IO.GetOut((int)DO.TRAY_WORK_CONV_RUN_REVERSE, ref ret2);

            if (ret1 == 1)
            {
                LogUtil.Instance.Log(LOG_TYPE.UI, "Tray Work Conveyor Stop Button Click", CONTENT_TYPE.INFO);

                Machine.IO.SetOut((int)DO.TRAY_WORK_CONV_RUN, 0);
                Machine.IO.SetOut((int)DO.TRAY_WORK_CONV_RUN_REVERSE, 0);
            }
            else
            {
                LogUtil.Instance.Log(LOG_TYPE.UI, "Tray Work Conveyor Run CW Button Click", CONTENT_TYPE.INFO);

                Machine.IO.SetOut((int)DO.TRAY_WORK_CONV_RUN, 1);
                Machine.IO.SetOut((int)DO.TRAY_WORK_CONV_RUN_REVERSE, 0);
            }
        }

        private void button_TrayWorkConvRunCCWStop_Click(object sender, EventArgs e)
        {
            uint ret1 = 0; uint ret2 = 0;

            Machine.IO.GetOut((int)DO.TRAY_WORK_CONV_RUN, ref ret1);
            Machine.IO.GetOut((int)DO.TRAY_WORK_CONV_RUN_REVERSE, ref ret2);

            if (ret2 == 1)
            {
                LogUtil.Instance.Log(LOG_TYPE.UI, "Tray Work Conveyor Stop Button Click", CONTENT_TYPE.INFO);

                Machine.IO.SetOut((int)DO.TRAY_WORK_CONV_RUN, 0);
                Machine.IO.SetOut((int)DO.TRAY_WORK_CONV_RUN_REVERSE, 0);
            }
            else
            {
                LogUtil.Instance.Log(LOG_TYPE.UI, "Tray Work Conveyor Run CCW Button Click", CONTENT_TYPE.INFO);

                Machine.IO.SetOut((int)DO.TRAY_WORK_CONV_RUN, 0);
                Machine.IO.SetOut((int)DO.TRAY_WORK_CONV_RUN_REVERSE, 1);
            }
        }

        private void button_TrayOutBufConvRunStop_Click(object sender, EventArgs e)
        {
            uint ret1 = 0;

            Machine.IO.GetOut((int)DO.TRAY_OUT_BUF_CONV_RUN, ref ret1);

            if (ret1 == 0)
            {
                LogUtil.Instance.Log(LOG_TYPE.UI, "Tray OutBuf Conveyor Run Button Click", CONTENT_TYPE.INFO);

                Machine.IO.SetOut((int)DO.TRAY_OUT_BUF_CONV_RUN, 1);
            }
            else if (ret1 == 1)
            {
                LogUtil.Instance.Log(LOG_TYPE.UI, "Tray OutBuf Conveyor Stop Button Click", CONTENT_TYPE.INFO);

                Machine.IO.SetOut((int)DO.TRAY_OUT_BUF_CONV_RUN, 0);
            }
        }

        private void button_TrayOutConvRunStop_Click(object sender, EventArgs e)
        {
            uint ret1 = 0;

            Machine.IO.GetOut((int)DO.TRAY_OUT_CONV_RUN, ref ret1);

            if (ret1 == 0)
            {
                LogUtil.Instance.Log(LOG_TYPE.UI, "Tray Out Conveyor Run Button Click", CONTENT_TYPE.INFO);

                Machine.IO.SetOut((int)DO.TRAY_OUT_CONV_RUN, 1);
            }
            else if (ret1 == 1)
            {
                LogUtil.Instance.Log(LOG_TYPE.UI, "Tray Out Conveyor Stop Button Click", CONTENT_TYPE.INFO);

                Machine.IO.SetOut((int)DO.TRAY_OUT_CONV_RUN, 0);
            }
        }

        private void button_TrayOutConveyorUpDown_Click(object sender, EventArgs e)
        {
            uint ret1 = 0;
            uint ret2 = 0;

            Machine.IO.GetOut((int)DO.TRAY_OUT_CONV_UP, ref ret1);
            Machine.IO.GetOut((int)DO.TRAY_OUT_CONV_DOWN, ref ret2);

            if (ret1 == 0)
            {
                LogUtil.Instance.Log(LOG_TYPE.UI, "Tray Out Conveyor Up Button Click", CONTENT_TYPE.INFO);

                Machine.IO.SetOut((int)DO.TRAY_OUT_CONV_UP, 1);
                Machine.IO.SetOut((int)DO.TRAY_OUT_CONV_DOWN, 0);
            }
            else if (ret1 == 1)
            {
                LogUtil.Instance.Log(LOG_TYPE.UI, "Tray Out Conveyor Down Button Click", CONTENT_TYPE.INFO);

                Machine.IO.SetOut((int)DO.TRAY_OUT_CONV_UP, 0);
                Machine.IO.SetOut((int)DO.TRAY_OUT_CONV_DOWN, 1);
            }
        }

        private void button_TrayTRFExistUpDown_Click(object sender, EventArgs e)
        {
            uint ret1 = 0;
            uint ret2 = 0;

            Machine.IO.GetOut((int)DO.TRAY_TRF_EXIST_UP, ref ret1);
            Machine.IO.GetOut((int)DO.TRAY_TRF_EXIST_DOWN, ref ret2);

            if (ret1 == 0)
            {
                LogUtil.Instance.Log(LOG_TYPE.UI, "Tray Trasnfer Exist Up Button Click", CONTENT_TYPE.INFO);

                Machine.IO.SetOut((int)DO.TRAY_TRF_EXIST_UP, 1);
                Machine.IO.SetOut((int)DO.TRAY_TRF_EXIST_DOWN, 0);
            }
            else if (ret1 == 1)
            {
                LogUtil.Instance.Log(LOG_TYPE.UI, "Tray Trasnfer Exist Down Button Click", CONTENT_TYPE.INFO);

                Machine.IO.SetOut((int)DO.TRAY_TRF_EXIST_UP, 0);
                Machine.IO.SetOut((int)DO.TRAY_TRF_EXIST_DOWN, 1);
            }
        }

        #endregion
        #region POSITION
        private void button_TrayTransferYReadyPos_Click(object sender, EventArgs e)
        {
            LogUtil.Instance.Log(LOG_TYPE.UI, "Tray Trasnfer Y Move to Ready Pos Button Click", CONTENT_TYPE.INFO);

            var proc = Machine.proclist[(int)Machine.PROCESS.TRAYOUTBUF];
            proc.SetMessage((int)ProcessTrayOutBuf.MSG.MSG_MOVE_READY_POS);
        }

        private void button_TrayTransferYLoadingPos_Click(object sender, EventArgs e)
        {
            LogUtil.Instance.Log(LOG_TYPE.UI, "Tray Trasnfer Y Move to Loading Pos Button Click", CONTENT_TYPE.INFO);

            var proc = Machine.proclist[(int)Machine.PROCESS.TRAYOUTBUF];
            proc.SetMessage((int)ProcessTrayOutBuf.MSG.MSG_MOVE_LOADING_POS);
        }

        private void button_TrayTransferYUnloadingPos_Click(object sender, EventArgs e)
        {
            LogUtil.Instance.Log(LOG_TYPE.UI, "Tray Trasnfer Y Move to Unloading Pos Button Click", CONTENT_TYPE.INFO);

            var proc = Machine.proclist[(int)Machine.PROCESS.TRAYOUTBUF];
            proc.SetMessage((int)ProcessTrayOutBuf.MSG.MSG_MOVE_UNLOADING_POS);
        }

        private void button_TrayTransferZReadyPos_Click(object sender, EventArgs e)
        {
            LogUtil.Instance.Log(LOG_TYPE.UI, "Tray Trasnfer Z Move to Ready Pos Button Click", CONTENT_TYPE.INFO);

            var proc = Machine.proclist[(int)Machine.PROCESS.TRAYOUTBUF];
            proc.SetMessage((int)ProcessTrayOutBuf.MSG.MSG_SINGLE, (int)ProcessTrayOutBuf.STEP.MOVE_TRAY_TRF_Z_READY_POS);
        }

        private void button_TrayTransferZLoadingPos_Click(object sender, EventArgs e)
        {
            LogUtil.Instance.Log(LOG_TYPE.UI, "Tray Trasnfer Z Move to Loading Pos Button Click", CONTENT_TYPE.INFO);

            var proc = Machine.proclist[(int)Machine.PROCESS.TRAYOUTBUF];
            proc.SetMessage((int)ProcessTrayOutBuf.MSG.MSG_SINGLE, (int)ProcessTrayOutBuf.STEP.MOVE_TRAY_TRF_Z_LOADING_POS_SENSING);
        }

        private void button_TrayTransferZUnloadingPos_Click(object sender, EventArgs e)
        {
            LogUtil.Instance.Log(LOG_TYPE.UI, "Tray Trasnfer Z Move to Unloading Pos Button Click", CONTENT_TYPE.INFO);

            var proc = Machine.proclist[(int)Machine.PROCESS.TRAYOUTBUF];
            proc.SetMessage((int)ProcessTrayOutBuf.MSG.MSG_SINGLE, (int)ProcessTrayOutBuf.STEP.MOVE_TRAY_TRF_Z_UNLOADING_POS);
        }

        private void button_TrayWorkZLoadingPos_Click(object sender, EventArgs e)
        {
            LogUtil.Instance.Log(LOG_TYPE.UI, "Tray Work Z Move to Loading Pos Button Click", CONTENT_TYPE.INFO);

            var proc = Machine.proclist[(int)Machine.PROCESS.TRAYWORK];
            proc.SetMessage((int)ProcessTrayWork.MSG.MSG_SINGLE, (int)ProcessTrayWork.STEP.MOVE_Z_LOADING_POS);
        }

        private void button_TrayWorkZWorkPos_Click(object sender, EventArgs e)
        {
            LogUtil.Instance.Log(LOG_TYPE.UI, "Tray Work Z Move to Work Pos Button Click", CONTENT_TYPE.INFO);

            var proc = Machine.proclist[(int)Machine.PROCESS.TRAYWORK];
            proc.SetMessage((int)ProcessTrayWork.MSG.MSG_SINGLE, (int)ProcessTrayWork.STEP.MOVE_Z_WORK_POS);
        }

        private void button_TrayWorkZUnloadingPos_Click(object sender, EventArgs e)
        {
            //LogUtil.Instance.Log(LOG_TYPE.UI, "Tray Work Z Move to Unloading Pos Button Click", CONTENT_TYPE.INFO);

            //var proc = Machine.proclist[(int)Machine.PROCESS.TRAYWORK];
            //proc.SetMessage((int)ProcessTrayWork.MSG.MSG_SINGLE, (int)ProcessTrayWork.STEP.MOVE_Z_UNLOADING_POS);
        }

        #endregion
        #region PROCESS
        private void button_TrayInLoading_Click(object sender, EventArgs e)
        {
            LogUtil.Instance.Log(LOG_TYPE.UI, "Tray In Loading Process Button Click", CONTENT_TYPE.INFO);

            var proc1 = Machine.proclist[(int)Machine.PROCESS.TRAYIN];
            proc1.SetMessage((int)ProcessTrayIn.MSG.MSG_LOADING_PRODUCT);
        }

        private void button_TrayInToInBuf_Click(object sender, EventArgs e)
        {
            LogUtil.Instance.Log(LOG_TYPE.UI, "Tray In To InBuf Process Button Click", CONTENT_TYPE.INFO);

            var proc1 = Machine.proclist[(int)Machine.PROCESS.TRAYIN];
            proc1.SetMessage((int)ProcessTrayIn.MSG.MSG_UNLOADING_PRODUCT);
            var proc2 = Machine.proclist[(int)Machine.PROCESS.TRAYINBUF];
            proc2.SetMessage((int)ProcessTrayInBuf.MSG.MSG_LOADING_PRODUCT);
        }

        private void button_TrayInBufToWork_Click(object sender, EventArgs e)
        {
            LogUtil.Instance.Log(LOG_TYPE.UI, "Tray InBuf to Work Process Button Click", CONTENT_TYPE.INFO);

            var proc1 = Machine.proclist[(int)Machine.PROCESS.TRAYINBUF];
            proc1.SetMessage((int)ProcessTrayInBuf.MSG.MSG_UNLOADING_PRODUCT);
            var proc2 = Machine.proclist[(int)Machine.PROCESS.TRAYWORK];
            proc2.SetMessage((int)ProcessTrayWork.MSG.MSG_LOADING_PRODUCT);
        }

        private void button_TrayWorkToOutBuf_Click(object sender, EventArgs e)
        {
            LogUtil.Instance.Log(LOG_TYPE.UI, "Tray Work To OutBuf Process Button Click", CONTENT_TYPE.INFO);

            var proc1 = Machine.proclist[(int)Machine.PROCESS.TRAYWORK];
            proc1.SetMessage((int)ProcessTrayWork.MSG.MSG_UNLOADING_PRODUCT);
            var proc2 = Machine.proclist[(int)Machine.PROCESS.TRAYOUTBUF];
            proc2.SetMessage((int)ProcessTrayOutBuf.MSG.MSG_LOADING_PRODUCT);
        }

        private void button_TrayOutBufToOut_Click(object sender, EventArgs e)
        {
            LogUtil.Instance.Log(LOG_TYPE.UI, "Tray OutBuf To Out Process Button Click", CONTENT_TYPE.INFO);

            var proc1 = Machine.proclist[(int)Machine.PROCESS.TRAYOUTBUF];
            proc1.SetMessage((int)ProcessTrayOutBuf.MSG.MSG_UNLOADING_PRODUCT);
            var proc2 = Machine.proclist[(int)Machine.PROCESS.TRAYOUT];
            proc2.SetMessage((int)ProcessTrayOut.MSG.MSG_LOADING_PRODUCT);
        }

        private void button_TrayOutUnloading_Click(object sender, EventArgs e)
        {
            LogUtil.Instance.Log(LOG_TYPE.UI, "Tray Out Unloading Process Button Click", CONTENT_TYPE.INFO);

            var proc1 = Machine.proclist[(int)Machine.PROCESS.TRAYOUT];
            proc1.SetMessage((int)ProcessTrayOutBuf.MSG.MSG_UNLOADING_PRODUCT);
        }
        #endregion

        private void button_TrayWorkAlignPositioning_Click(object sender, EventArgs e)
        {
            LogUtil.Instance.Log(LOG_TYPE.UI, "Tray Work Align Positioning Process Button Click", CONTENT_TYPE.INFO);

            var proc1 = Machine.proclist[(int)Machine.PROCESS.TRAYWORK];
            proc1.SetMessage((int)ProcessTrayWork.MSG.MSG_LOCATION_PRODUCT);
        }

        private void button_TrayWorkAlign_Click(object sender, EventArgs e)
        {
            LogUtil.Instance.Log(LOG_TYPE.UI, "Tray Work Align Process Button Click", CONTENT_TYPE.INFO);

            var proc1 = Machine.proclist[(int)Machine.PROCESS.TRAYWORK];
            proc1.SetMessage((int)ProcessTrayWork.MSG.MSG_ALIGNMENT_PRODUCT);
        }

        public enum BTN_TAB_SELECT
        {
            MANUAL_CONTROL_CONVEYOR,
            MANUAL_CONTROL_TRANSFER,
            MANUAL_PROCESS //셋업때 제거
        }
        public void SetBtnClickEvents()
        {
            nV_Button_TG_NS_ControlConveyor.ClickEvent += SelectButton_ClickEvent;
            nV_Button_TG_NS_ControlTransfer.ClickEvent += SelectButton_ClickEvent;
            nV_Button_TG_NS_Process.ClickEvent += SelectButton_ClickEvent;

            //Conveyor, Stopper
            this.button_TrayTRFVacOnOff.ClickEvent += new System.EventHandler(this.button_TrayTRF_Vac_OnOff_Click);
            
            this.button_TrayInStopper_UpDown.ClickEvent += new System.EventHandler(this.button_TrayInStopper_UpDown_Click);
            this.button_TrayInStopper_UpDown.MsgEvent = "Use In Stopper Up ?";
            this.button_TrayInStopper_UpDown.MsgEvent2 = "Use In Stopper Down ?";

            this.button_TrayInConvRunCWStop.ClickEvent += new System.EventHandler(this.button_TrayInConvRunCWStop_Click);
            this.button_TrayInConvRunCWStop.MsgEvent = "Use In Conveyor Run CW ?";
            this.button_TrayInConvRunCWStop.MsgEvent2 = "Use In Conveyor Stop ?";
            this.button_TrayInConvRunCCWStop.ClickEvent += new System.EventHandler(this.button_TrayInConvRunCCWStop_Click);
            this.button_TrayInConvRunCCWStop.MsgEvent = "Use In Conveyor Run CCW ?";
            this.button_TrayInConvRunCCWStop.MsgEvent2 = "Use In Conveyor Stop ?";

            this.button_TrayInBufStopper_UpDown.ClickEvent += new System.EventHandler(this.button_TrayInBufStopper_UpDown_Click);
            this.button_TrayInBufStopper_UpDown.MsgEvent = "Use In Buf Stopper Up ?";
            this.button_TrayInBufStopper_UpDown.MsgEvent2 = "Use In Buf Stopper Down ?";

            this.button_TrayInBufConvRunCWStop.ClickEvent += new System.EventHandler(this.button_TrayInBufConvRunCWStop_Click);
            this.button_TrayInBufConvRunCWStop.MsgEvent = "Use In Buf Conveyor Run CW ?";
            this.button_TrayInBufConvRunCWStop.MsgEvent2 = "Use In Buf Conveyor Stop ?";
            this.button_TrayInBufConvRunCCWStop.ClickEvent += new System.EventHandler(this.button_TrayInBufConvRunCCWStop_Click);
            this.button_TrayInBufConvRunCCWStop.MsgEvent = "Use In Buf Conveyor Run CCW ?";
            this.button_TrayInBufConvRunCCWStop.MsgEvent2 = "Use In Buf Conveyor Stop ?";

            this.button_TrayWorkConvRunCWStop.ClickEvent += new System.EventHandler(this.button_TrayWorkConvRunCWStop_Click);
            this.button_TrayWorkConvRunCWStop.MsgEvent = "Use Work Conveyor Run CW ?";
            this.button_TrayWorkConvRunCWStop.MsgEvent2 = "Use Work Conveyor Stop ?";
            this.button_TrayWorkConvRunCCWStop.ClickEvent += new System.EventHandler(this.button_TrayWorkConvRunCCWStop_Click);
            this.button_TrayWorkConvRunCCWStop.MsgEvent = "Use Work Conveyor Run CCW ?";
            this.button_TrayWorkConvRunCCWStop.MsgEvent2 = "Use Work Conveyor Stop ?";

            this.button_TrayOutBufStopper_UpDown.ClickEvent += new System.EventHandler(this.button_TrayOutBufStopper_UpDown_Click);
            this.button_TrayOutBufStopper_UpDown.MsgEvent = "Use Out Buf Stopper Up ?";
            this.button_TrayOutBufStopper_UpDown.MsgEvent2 = "Use Out Buf Stopper Down ?";

            this.button_TrayOutBufConvRunStop.ClickEvent += new System.EventHandler(this.button_TrayOutBufConvRunStop_Click);
            this.button_TrayOutBufConvRunStop.MsgEvent = "Use Out Buffer Conveyor Run ?";
            this.button_TrayOutBufConvRunStop.MsgEvent2 = "Use Out Buffer Conveyor Stop ?"; 

            this.button_TrayOutConvRunStop.ClickEvent += new System.EventHandler(this.button_TrayOutConvRunStop_Click);
            this.button_TrayOutConvRunStop.MsgEvent = "Set Out Conveyor CW ?";
            this.button_TrayOutConvRunStop.MsgEvent2 = "Set Out Conveyor CCW ?";

            this.button_TrayOutConveyorUpDown.ClickEvent += new System.EventHandler(this.button_TrayOutConveyorUpDown_Click);
            this.button_TrayOutConveyorUpDown.MsgEvent = "Set Out Conveyor Up ?";
            this.button_TrayOutConveyorUpDown.MsgEvent2 = "Set Out Conveyor Down ?";

            this.button_TrayTRFExistUpDown.ClickEvent += new System.EventHandler(this.button_TrayTRFExistUpDown_Click);
            this.button_TrayTRFExistUpDown.MsgEvent = "Set Out Conveyor Up ?";
            this.button_TrayTRFExistUpDown.MsgEvent2 = "Set Out Conveyor Down ?";

            //Move Pos
            this.button_TrayTransferYReadyPos.ClickEvent += new System.EventHandler(this.button_TrayTransferYReadyPos_Click);
            this.button_TrayTransferYReadyPos.MsgEvent = "Move to " + "Tray Trasnfer Y " + "Ready Pos ?";
            
            this.button_TrayTransferYLoadingPos.ClickEvent += new System.EventHandler(this.button_TrayTransferYLoadingPos_Click);
            this.button_TrayTransferYLoadingPos.MsgEvent = "Move to " + "Tray Trasnfer Y " + "Loading Pos ?";
            
            this.button_TrayTransferYUnloadingPos.ClickEvent += new System.EventHandler(this.button_TrayTransferYUnloadingPos_Click);
            this.button_TrayTransferYUnloadingPos.MsgEvent = "Move to " + "Tray Trasnfer Y " + "Unloading Pos ?";
            
            this.button_TrayTransferZReadyPos.ClickEvent += new System.EventHandler(this.button_TrayTransferZReadyPos_Click);
            this.button_TrayTransferZReadyPos.MsgEvent = "Move to " + "Tray Trasnfer Z " + "Ready Pos ?";
            
            this.button_TrayTransferZLoadingPos.ClickEvent += new System.EventHandler(this.button_TrayTransferZLoadingPos_Click);
            this.button_TrayTransferZLoadingPos.MsgEvent = "Move to " + "Tray Trasnfer Z " + "Loading Pos ?";
            
            this.button_TrayTransferZUnloadingPos.ClickEvent += new System.EventHandler(this.button_TrayTransferZUnloadingPos_Click);
            this.button_TrayTransferZUnloadingPos.MsgEvent = "Move to " + "Tray Trasnfer Z " + "Unloading Pos ?";
            
            this.button_TrayWorkZLoadingPos.ClickEvent += new System.EventHandler(this.button_TrayWorkZLoadingPos_Click);
            this.button_TrayWorkZLoadingPos.MsgEvent = "Move to " + "Tray Work Z " + "Loading Pos ?";
            
            this.button_TrayWorkZWorkPos.ClickEvent += new System.EventHandler(this.button_TrayWorkZWorkPos_Click);
            this.button_TrayWorkZWorkPos.MsgEvent = "Move to " + "Tray Work Z " + "Work Pos ?";
            
            this.button_TrayWorkZUnloadingPos.ClickEvent += new System.EventHandler(this.button_TrayWorkZUnloadingPos_Click);
            this.button_TrayWorkZUnloadingPos.MsgEvent = "Move to " + "Tray Work Z " + "Unloading Pos ?";

            //Process
            this.button_TrayInLoading.ClickEvent += new System.EventHandler(this.button_TrayInLoading_Click);
            this.button_TrayInLoading.MsgEvent = "Start " + "Tray In" + "Loading Process ?";
            
            this.button_TrayInToInBuf.ClickEvent += new System.EventHandler(this.button_TrayInToInBuf_Click);
            this.button_TrayInToInBuf.MsgEvent = "Start " + "Tray In " + "To In Buffer Process ?";

            this.button_TrayInBufToWork.ClickEvent += new System.EventHandler(this.button_TrayInBufToWork_Click);
            this.button_TrayInBufToWork.MsgEvent = "Start " + "Tray In Buffer " + "To Work Process ?";

            this.button_TrayWorkPositioning.ClickEvent += new System.EventHandler(this.button_TrayWorkAlignPositioning_Click);
            this.button_TrayWorkPositioning.MsgEvent = "Start " + "Tray Work " + "Positioning Process ?";
            
            this.button_TrayWorkAlign.ClickEvent += new System.EventHandler(this.button_TrayWorkAlign_Click);
            this.button_TrayWorkAlign.MsgEvent = "Start " + "Tray Work " + "Align Process ?";
            
            this.button_TrayWorkToOutBuf.ClickEvent += new System.EventHandler(this.button_TrayWorkToOutBuf_Click);
            this.button_TrayWorkToOutBuf.MsgEvent = "Start " + "Tray Work " + "To OutBuf Process ?";
            
            this.button_TrayOutBufToOut.ClickEvent += new System.EventHandler(this.button_TrayOutBufToOut_Click);
            this.button_TrayOutBufToOut.MsgEvent = "Start " + "Tray OutBuf " + "To Out Process ?";
            
            this.button_TrayOutUnloading.ClickEvent += new System.EventHandler(this.button_TrayOutUnloading_Click);
            this.button_TrayOutUnloading.MsgEvent = "Start " + "Tray Out " + "Unloading Process ?";
        }
        private void SelectButton_ClickEvent(object sender, EventArgs e)
        {
            SwitchOffOtherButtons(sender);
            // to do
            string sTag = (sender as Control).Tag.ToString();
            if (Enum.TryParse(sTag, out BTN_TAB_SELECT selectBtn))
            {
                switch (selectBtn)
                {
                    case BTN_TAB_SELECT.MANUAL_CONTROL_CONVEYOR:
                        tabControl1.SelectedIndex = 0;
                        break;
                    case BTN_TAB_SELECT.MANUAL_CONTROL_TRANSFER:
                        tabControl1.SelectedIndex = 1;
                        break;
                    case BTN_TAB_SELECT.MANUAL_PROCESS:
                        tabControl1.SelectedIndex = 2;
                        break;
                }
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

        private void button_MovePosView_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 1;
        }

        private void button_Back_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 0;
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
    }
}
