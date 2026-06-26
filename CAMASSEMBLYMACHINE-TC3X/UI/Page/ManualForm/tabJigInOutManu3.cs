using static CAMASSEMBLYMACHINE.Define.SystemDefine;
using static CAMASSEMBLYMACHINE.Define.UIDefine;
using CAMASSEMBLYMACHINE.Process;
using TopEng.Device;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NV_UI;
using TopEng.Utils;
using CAMASSEMBLYMACHINE.Define;

namespace CAMASSEMBLYMACHINE.UI
{
    public partial class tabJigInOutManu3 : Form, IForm
    {
        List<Label> label_alignPos = new List<Label>();

        public tabJigInOutManu3()
        {
            InitializeComponent();

            label_alignPos.Add(disp_alignPos1);
            label_alignPos.Add(disp_alignPos2);

            // Hide TAB Button
            tabControl1.Appearance = TabAppearance.Buttons;
            tabControl1.SizeMode = TabSizeMode.Fixed;
            tabControl1.ItemSize = new Size(0, 1);

            SetBtnClickEvents();
            SelectButton_ClickEvent(nV_Button_TG_NS_Control, null);
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
            //disp_JigLiftLDPos.ButtonPush = Util.IsAxisInPos(
            //(AXIS.JIG_IN_BUF_Z, ParameterDefine.POSITION.JIG_IN_BUF_LOAD_POS));

            //disp_JigLiftULDPos.ButtonPush = Util.IsAxisInPos(
            //(AXIS.JIG_IN_BUF_Z, ParameterDefine.POSITION.JIG_IN_BUF_UNLOAD_POS));
        }

        private void UpdateIOState()
        {
            uint returnValue1 = 0;

            if (Machine.IO == null)
                return;

            #region EXIST SENSOR
            Machine.IO.GetIn((int)DI.JIG_IN_BUF_DETECT_IN, ref returnValue1);
            disp_JigInBufIn.ButtonPush = returnValue1 == 1 ? true : false;
            Machine.IO.GetIn((int)DI.JIG_IN_BUF_DETECT_OUT, ref returnValue1);
            disp_JigInBufOut.ButtonPush = returnValue1 == 1 ? true : false;

            Machine.IO.GetIn((int)DI.JIG_WORK_DETECT_IN, ref returnValue1);
            disp_JigWorkIn.ButtonPush = returnValue1 == 1 ? true : false;
            Machine.IO.GetIn((int)DI.JIG_WORK_DETECT_OUT, ref returnValue1);
            disp_JigWorkOut.ButtonPush = returnValue1 == 1 ? true : false;

            Machine.IO.GetIn((int)DI.JIG_OUT_BUF_DETECT_IN, ref returnValue1);
            disp_JigOutBufIn.ButtonPush = returnValue1 == 1 ? true : false;
            Machine.IO.GetIn((int)DI.JIG_OUT_BUF_DETECT_OUT, ref returnValue1);
            disp_JigOutBufOut.ButtonPush = returnValue1 == 1 ? true : false;

            Machine.IO.GetIn((int)DI.JIG_IN_CARRIER_DETECT_IN, ref returnValue1);
            disp_JigInCarrierIn.ButtonPush = returnValue1 == 1 ? true : false;
            Machine.IO.GetIn((int)DI.JIG_IN_CARRIER_DETECT_OUT, ref returnValue1);
            disp_JigInCarrierOut.ButtonPush = returnValue1 == 1 ? true : false;

            Machine.IO.GetIn((int)DI.JIG_OUT_CARRIER_DETECT_IN, ref returnValue1);
            disp_JigOutCarrierIn.ButtonPush = returnValue1 == 1 ? true : false;
            Machine.IO.GetIn((int)DI.JIG_OUT_CARRIER_DETECT_OUT, ref returnValue1);
            disp_JigOutCarrierOut.ButtonPush = returnValue1 == 1 ? true : false;

            #endregion
            #region CYLINDER SENSOR
            Machine.IO.GetIn((int)DI.JIG_IN_BUF_STOPPER_UP, ref returnValue1);
            disp_JigInBufStopperUp.ButtonPush = returnValue1 == 1 ? true : false;
            Machine.IO.GetIn((int)DI.JIG_IN_BUF_STOPPER_DOWN, ref returnValue1);
            disp_JigInBufStopperDown.ButtonPush = returnValue1 == 1 ? true : false;
            Machine.IO.GetOut((int)DO.JIG_IN_BUF_STOPPER_UP, ref returnValue1);
            button_JigInBufStopperUpDown.valueEvent = returnValue1;

            Machine.IO.GetIn((int)DI.JIG_WORK_STOPPER_UP, ref returnValue1);
            disp_JigWorkStopperUp.ButtonPush = returnValue1 == 1 ? true : false;
            Machine.IO.GetIn((int)DI.JIG_WORK_STOPPER_DOWN, ref returnValue1);
            disp_JigWorkStopperDown.ButtonPush = returnValue1 == 1 ? true : false;
            Machine.IO.GetOut((int)DO.JIG_WORK_STOPPER_UP, ref returnValue1);
            button_JigWorkStopperUpDown.valueEvent = returnValue1;

            Machine.IO.GetIn((int)DI.JIG_OUT_STOPPER_UP, ref returnValue1);
            disp_JigOutBufStopperUp.ButtonPush = returnValue1 == 1 ? true : false;
            Machine.IO.GetIn((int)DI.JIG_OUT_STOPPER_DOWN, ref returnValue1);
            disp_JigOutBufStopperDown.ButtonPush = returnValue1 == 1 ? true : false;
            Machine.IO.GetOut((int)DO.JIG_OUT_BUF_STOPPER_UP, ref returnValue1);
            button_JigOutBufStopperUpDown.valueEvent = returnValue1;

            Machine.IO.GetIn((int)DI.JIG_IN_CARRIER_STOPPER_UP, ref returnValue1);
            disp_JigInCarrrierStopperUp.ButtonPush = returnValue1 == 1 ? true : false;
            Machine.IO.GetIn((int)DI.JIG_IN_CARRIER_STOPPER_DOWN, ref returnValue1);
            disp_JigInCarrrierStopperDown.ButtonPush = returnValue1 == 1 ? true : false;
            Machine.IO.GetOut((int)DO.JIG_IN_CARRIER_STOPPER_UP, ref returnValue1);
            button_JigInCarrierStopperUpDown.valueEvent = returnValue1;

            Machine.IO.GetIn((int)DI.JIG_OUT_CARRIER_STOPPER_UP, ref returnValue1);
            disp_JigOutCarrrierStopperUp.ButtonPush = returnValue1 == 1 ? true : false;
            Machine.IO.GetIn((int)DI.JIG_OUT_CARRIER_STOPPER_DOWN, ref returnValue1);
            disp_JigOutCarrrierStopperDown.ButtonPush = returnValue1 == 1 ? true : false;
            Machine.IO.GetOut((int)DO.JIG_OUT_CARRIER_STOPPER_UP, ref returnValue1);
            button_JigOutCarrierStopperUpDown.valueEvent = returnValue1;

            Machine.IO.GetIn((int)DI.JIG_WORK_PALLETE_FWD, ref returnValue1);
            disp_JigPalletFwd.ButtonPush = returnValue1 == 1 ? true : false;
            Machine.IO.GetIn((int)DI.JIG_WORK_PALLETE_BWD, ref returnValue1);
            disp_JigPalletBwd.ButtonPush = returnValue1 == 1 ? true : false;
            Machine.IO.GetOut((int)DO.JIG_WORK_PALLETE_FWD, ref returnValue1);
            button_JigPalletFwdBwd.valueEvent = returnValue1;

            #endregion
            #region CONVEYOR RUN/STOP
            //CAM 1, MID 1
            Machine.IO.GetOut((int)DO.JIG_IN_BUF_CONV_RUN, ref returnValue1);
            disp_JigInBufConvRunCWStop.ButtonPush = returnValue1 == 1 ? true : false;
            button_JigInBufConvRunCWStop.valueEvent = returnValue1;

            Machine.IO.GetOut((int)DO.JIG_WORK_CONV_RUN, ref returnValue1);
            disp_JigWorkConvRunStop.ButtonPush = returnValue1 == 1 ? true : false;
            button_JigWorkConvRunStop.valueEvent = returnValue1;

            //CAM 6, MID 3
            Machine.IO.GetOut((int)DO.JIG_OUT_BUF_CONV_RUN, ref returnValue1);
            disp_JigOutBufConvRunCWStop.ButtonPush = returnValue1 == 1 ? true : false;
            button_JigOutBufConvRunCWStop.valueEvent = returnValue1;

            Machine.IO.GetOut((int)DO.JIG_IN_CARRIER_CONV_RUN, ref returnValue1);
            disp_JigInCarrierConvRunStop.ButtonPush = returnValue1 == 1 ? true : false;
            button_JigInCarrierConvRunStop.valueEvent = returnValue1;

            //CAM 2,3,4,5, MID 2
            Machine.IO.GetOut((int)DO.JIG_OUT_CARRIER_CONV_RUN, ref returnValue1);
            disp_JigOutCarrierConvRunStop.ButtonPush = returnValue1 == 1 ? true : false;
            button_JigOutCarrierConvRunStop.valueEvent = returnValue1;
            #endregion
        }

        private void UpdateMachineData()
        {
            var proc = Machine.proclist[(int)Machine.PROCESS.JIGWORK] as ProcessJigWork;

            for (int i = 0; i < 2; i++)
            {
                if (proc.alignSucs[i] < 0)
                {
                    label_alignPos[i].Text = $"( none , none )";
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

        private void button_JigInBufStopperUpDown_Click(object sender, EventArgs e)
        {
            uint ret1 = 0;

            Machine.IO.GetOut((int)DO.JIG_IN_BUF_STOPPER_UP, ref ret1);

            if (ret1 == 0)
            {
                LogUtil.Instance.Log(LOG_TYPE.UI, "Jig In Buffer Stopper Up Button Click", CONTENT_TYPE.INFO);

                Machine.IO.SetOut((int)DO.JIG_IN_BUF_STOPPER_UP, 1);
                Machine.IO.SetOut((int)DO.JIG_IN_BUF_STOPPER_DOWN, 0);
            }
            else if (ret1 == 1)
            {
                LogUtil.Instance.Log(LOG_TYPE.UI, "Jig In Buffer Stopper Down Button Click", CONTENT_TYPE.INFO);

                Machine.IO.SetOut((int)DO.JIG_IN_BUF_STOPPER_UP, 0);
                Machine.IO.SetOut((int)DO.JIG_IN_BUF_STOPPER_DOWN, 1);
            }
        }

        private void button_JigWorkStopperUpDown_Click(object sender, EventArgs e)
        {
            uint ret1 = 0;

            Machine.IO.GetOut((int)DO.JIG_WORK_STOPPER_UP, ref ret1);

            if (ret1 == 0)
            {
                LogUtil.Instance.Log(LOG_TYPE.UI, "Jig Work Stopper Up Button Click", CONTENT_TYPE.INFO);

                Machine.IO.SetOut((int)DO.JIG_WORK_STOPPER_UP, 1);
                Machine.IO.SetOut((int)DO.JIG_WORK_STOPPER_DOWN, 0);
            }
            else if (ret1 == 1)
            {
                LogUtil.Instance.Log(LOG_TYPE.UI, "Jig Work Stopper Down Button Click", CONTENT_TYPE.INFO);

                Machine.IO.SetOut((int)DO.JIG_WORK_STOPPER_UP, 0);
                Machine.IO.SetOut((int)DO.JIG_WORK_STOPPER_DOWN, 1);
            }
        }

        private void button_JigOutBufStopperUpDown_Click(object sender, EventArgs e)
        {
            uint ret1 = 0;

            Machine.IO.GetOut((int)DO.JIG_OUT_BUF_STOPPER_UP, ref ret1);

            if (ret1 == 0)
            {
                LogUtil.Instance.Log(LOG_TYPE.UI, "Jig Out Buffer Stopper Up Button Click", CONTENT_TYPE.INFO);

                Machine.IO.SetOut((int)DO.JIG_OUT_BUF_STOPPER_UP, 1);
                Machine.IO.SetOut((int)DO.JIG_OUT_BUF_STOPPER_DOWN, 0);
            }
            else if (ret1 == 1)
            {
                LogUtil.Instance.Log(LOG_TYPE.UI, "Jig Out Buffer Stopper Down Button Click", CONTENT_TYPE.INFO);

                Machine.IO.SetOut((int)DO.JIG_OUT_BUF_STOPPER_UP, 0);
                Machine.IO.SetOut((int)DO.JIG_OUT_BUF_STOPPER_DOWN, 1);
            }
        }

        private void button_JigInCarrierStopperUpDown_Click(object sender, EventArgs e)
        {
            uint ret1 = 0;

            Machine.IO.GetOut((int)DO.JIG_IN_CARRIER_STOPPER_UP, ref ret1);

            if (ret1 == 0)
            {
                LogUtil.Instance.Log(LOG_TYPE.UI, "Jig In Carrier Stopper Up Button Click", CONTENT_TYPE.INFO);

                Machine.IO.SetOut((int)DO.JIG_IN_CARRIER_STOPPER_UP, 1);
                Machine.IO.SetOut((int)DO.JIG_IN_CARRIER_STOPPER_DOWN, 0);
            }
            else if (ret1 == 1)
            {
                LogUtil.Instance.Log(LOG_TYPE.UI, "Jig In Carrier Stopper Down Button Click", CONTENT_TYPE.INFO);

                Machine.IO.SetOut((int)DO.JIG_IN_CARRIER_STOPPER_UP, 0);
                Machine.IO.SetOut((int)DO.JIG_IN_CARRIER_STOPPER_DOWN, 1);
            }
        }

        private void button_JigOutCarrierStopperUpDown_Click(object sender, EventArgs e)
        {
            uint ret1 = 0; //CAM 2,3,4

            Machine.IO.GetOut((int)DO.JIG_OUT_CARRIER_STOPPER_UP, ref ret1);

            if (ret1 == 0)
            {
                LogUtil.Instance.Log(LOG_TYPE.UI, "Jig Out Carrier Stopper Up Button Click", CONTENT_TYPE.INFO);

                Machine.IO.SetOut((int)DO.JIG_OUT_CARRIER_STOPPER_UP, 1);
                Machine.IO.SetOut((int)DO.JIG_OUT_CARRIER_STOPPER_DOWN, 0);
            }
            else if (ret1 == 1)
            {
                LogUtil.Instance.Log(LOG_TYPE.UI, "Jig Out Carrier Stopper Down Button Click", CONTENT_TYPE.INFO);

                Machine.IO.SetOut((int)DO.JIG_OUT_CARRIER_STOPPER_UP, 0);
                Machine.IO.SetOut((int)DO.JIG_OUT_CARRIER_STOPPER_DOWN, 1);
            }
        }

        private void button_JigInBufConvRunCWStop_Click(object sender, EventArgs e)
        {
            uint ret1 = 0; uint ret2 = 0;

            Machine.IO.GetOut((int)DO.JIG_IN_BUF_CONV_RUN, ref ret1);

            if (ret1 == 1)
            {
                LogUtil.Instance.Log(LOG_TYPE.UI, "Jig In Buf Conveyor Stop Button Click", CONTENT_TYPE.INFO);

                Machine.IO.SetOut((int)DO.JIG_IN_BUF_CONV_RUN, 0);
            }
            else
            {
                LogUtil.Instance.Log(LOG_TYPE.UI, "Jig In Buf Conveyor Run CW Button Click", CONTENT_TYPE.INFO);

                Machine.IO.SetOut((int)DO.JIG_IN_BUF_CONV_RUN, 1);
                //Machine.IO.SetOut((int)DO.JIG_IN_BUF_CWCCW, 1);
            }
        }

        private void button_JigWorkRunStop_Click(object sender, EventArgs e)
        {
            uint ret1 = 0;

            Machine.IO.GetOut((int)DO.JIG_WORK_CONV_RUN, ref ret1);

            if (ret1 == 0)
            {
                LogUtil.Instance.Log(LOG_TYPE.UI, "Jig Work Conveyor Run Button Click", CONTENT_TYPE.INFO);

                Machine.IO.SetOut((int)DO.JIG_WORK_CONV_RUN, 1);
            }
            else if (ret1 == 1)
            {
                LogUtil.Instance.Log(LOG_TYPE.UI, "Jig Work Conveyor Stop Button Click", CONTENT_TYPE.INFO);

                Machine.IO.SetOut((int)DO.JIG_WORK_CONV_RUN, 0);
            }
        }

        private void button_JigOutBufConvRunCWStop_Click(object sender, EventArgs e)
        {
            uint ret1 = 0; uint ret2 = 0;

            Machine.IO.GetOut((int)DO.JIG_OUT_BUF_CONV_RUN, ref ret1);

            if (ret1 == 1)
            {
                LogUtil.Instance.Log(LOG_TYPE.UI, "Jig Out Buf Conveyor Stop Button Click", CONTENT_TYPE.INFO);

                Machine.IO.SetOut((int)DO.JIG_OUT_BUF_CONV_RUN, 0);
            }
            else
            {
                LogUtil.Instance.Log(LOG_TYPE.UI, "Jig Out Buf Conveyor Run CW Button Click", CONTENT_TYPE.INFO);

                Machine.IO.SetOut((int)DO.JIG_OUT_BUF_CONV_RUN, 1);
            }
        }

        private void button_JigInCarrierConvRunStop_Click(object sender, EventArgs e)
        {
            uint ret1 = 0;

            Machine.IO.GetOut((int)DO.JIG_IN_CARRIER_CONV_RUN, ref ret1);

            if (ret1 == 0)
            {
                LogUtil.Instance.Log(LOG_TYPE.UI, "Jig In Carrier Conveyor Run Button Click", CONTENT_TYPE.INFO);

                Machine.IO.SetOut((int)DO.JIG_IN_CARRIER_CONV_RUN, 1);
            }
            else if (ret1 == 1)
            {
                LogUtil.Instance.Log(LOG_TYPE.UI, "Jig In Carrier Conveyor Stop Button Click", CONTENT_TYPE.INFO);

                Machine.IO.SetOut((int)DO.JIG_IN_CARRIER_CONV_RUN, 0);
            }
        }

        private void button_JigOutCarrierConvRunStop_Click(object sender, EventArgs e)
        {
            uint ret1 = 0;

            Machine.IO.GetOut((int)DO.JIG_OUT_CARRIER_CONV_RUN, ref ret1);

            if (ret1 == 0)
            {
                LogUtil.Instance.Log(LOG_TYPE.UI, "Jig Out Carrier Conveyor Run Button Click", CONTENT_TYPE.INFO);

                Machine.IO.SetOut((int)DO.JIG_OUT_CARRIER_CONV_RUN, 1);
            }
            else if (ret1 == 1)
            {
                LogUtil.Instance.Log(LOG_TYPE.UI, "Jig Out Carrier Conveyor Stop Button Click", CONTENT_TYPE.INFO);

                Machine.IO.SetOut((int)DO.JIG_OUT_CARRIER_CONV_RUN, 0);
            }
        }

        private void button_JigPalletFwdBwd_Click(object sender, EventArgs e)
        {
            uint ret1 = 0;

            Machine.IO.GetOut((int)DO.JIG_WORK_PALLETE_FWD, ref ret1);

            if (ret1 == 0)
            {
                LogUtil.Instance.Log(LOG_TYPE.UI, "Jig Pallet Move Forward Button Click", CONTENT_TYPE.INFO);

                Machine.IO.SetOut((int)DO.JIG_WORK_PALLETE_FWD, 1);
                Machine.IO.SetOut((int)DO.JIG_WORK_PALLETE_BWD, 0);
            }
            else if (ret1 == 1)
            {
                LogUtil.Instance.Log(LOG_TYPE.UI, "Jig Pallet Move Backward Button Click", CONTENT_TYPE.INFO);

                Machine.IO.SetOut((int)DO.JIG_WORK_PALLETE_FWD, 0);
                Machine.IO.SetOut((int)DO.JIG_WORK_PALLETE_BWD, 1);
            }
        }

        private void button_JigLiftLDPos_Click(object sender, EventArgs e)
        {
            //LogUtil.Instance.Log(LOG_TYPE.UI, "Jig Lift Move Loading Pos Button Click", CONTENT_TYPE.INFO);

            //var proc = Machine.proclist[(int)Machine.PROCESS.JIGINBUF];
            //proc.SetMessage((int)ProcessJigInBuf.MSG.MSG_SINGLE, (int)ProcessJigInBuf.STEP.MOVE_LIFT_LOADING_POS);
        }

        private void button_JigLiftULDPos_Click(object sender, EventArgs e)
        {
            //LogUtil.Instance.Log(LOG_TYPE.UI, "Jig Lift Move Unloading Pos Button Click", CONTENT_TYPE.INFO);

            //var proc = Machine.proclist[(int)Machine.PROCESS.JIGINBUF];
            //proc.SetMessage((int)ProcessJigInBuf.MSG.MSG_SINGLE, (int)ProcessJigInBuf.STEP.MOVE_LIFT_UNLOADING_POS);
        }

        private void button_CarrierLoading_Click(object sender, EventArgs e)
        {
            LogUtil.Instance.Log(LOG_TYPE.UI, "Jig Carrier Loading Process Button Click", CONTENT_TYPE.INFO);

            //JIG IN CARRIER , JIG OUT CARRIER
            var proc = Machine.proclist[(int)Machine.PROCESS.JIGINCARRIER];
            proc.SetMessage((int)ProcessJigInCarrier.MSG.MSG_LOADING_PRODUCT);
        }

        private void button_InBufLoading_Click(object sender, EventArgs e)
        {
            LogUtil.Instance.Log(LOG_TYPE.UI, "Jig In Buffer Loading Process Button Click", CONTENT_TYPE.INFO);

            var proc1 = Machine.proclist[(int)Machine.PROCESS.JIGOUTCARRIER];
            proc1.SetMessage((int)ProcessJigOutCarrier.MSG.MSG_UNLOADING_PRODUCT);
            var proc2 = Machine.proclist[(int)Machine.PROCESS.JIGINBUF];
            proc2.SetMessage((int)ProcessJigInBuf.MSG.MSG_LOADING_PRODUCT);
        }

        private void button_WorkLoading_Click(object sender, EventArgs e)
        {
            LogUtil.Instance.Log(LOG_TYPE.UI, "Jig Work Loading Process Button Click", CONTENT_TYPE.INFO);

            var proc1 = Machine.proclist[(int)Machine.PROCESS.JIGINBUF];
            proc1.SetMessage((int)ProcessJigInBuf.MSG.MSG_UNLOADING_PRODUCT);
            var proc2 = Machine.proclist[(int)Machine.PROCESS.JIGWORK];
            proc2.SetMessage((int)ProcessJigWork.MSG.MSG_LOADING_PRODUCT);
        }

        private void button_JigOutBufLoading_Click(object sender, EventArgs e)
        {
            LogUtil.Instance.Log(LOG_TYPE.UI, "Jig Out Buffer Loading Process Button Click", CONTENT_TYPE.INFO);

            var proc1 = Machine.proclist[(int)Machine.PROCESS.JIGWORK];
            proc1.SetMessage((int)ProcessJigWork.MSG.MSG_UNLOADING_PRODUCT);
            var proc2 = Machine.proclist[(int)Machine.PROCESS.JIGOUTBUF];
            proc2.SetMessage((int)ProcessJigOutBuf.MSG.MSG_LOADING_PRODUCT);
        }

        private void button_JigOutBufUnloading_Click(object sender, EventArgs e)
        {
            LogUtil.Instance.Log(LOG_TYPE.UI, "Jig Out Buffer Unloading Process Button Click", CONTENT_TYPE.INFO);

            var proc = Machine.proclist[(int)Machine.PROCESS.JIGOUTBUF];
            proc.SetMessage((int)ProcessJigOutBuf.MSG.MSG_UNLOADING_PRODUCT);
        }

        private void button_WorkAlign_Click(object sender, EventArgs e)
        {
            LogUtil.Instance.Log(LOG_TYPE.UI, "Jig Work Align Process Button Click", CONTENT_TYPE.INFO);

            var proc2 = Machine.proclist[(int)Machine.PROCESS.JIGWORK];
            proc2.SetMessage((int)ProcessJigWork.MSG.MSG_ALIGNMENT_PRODUCT);
            //proc2.SetMessage((int)ProcessJigWork.MSG.MSG_PLACE_PRODUCT);
        }

        public enum BTN_TAB_SELECT
        {
            MANUAL_CONTROL,
            MANUAL_PROCESS //셋업때 제거
        }
        public void SetBtnClickEvents()
        {
            nV_Button_TG_NS_Control.ClickEvent += SelectButton_ClickEvent;
            nV_Button_TG_NS_Process.ClickEvent += SelectButton_ClickEvent;

            this.button_JigInBufStopperUpDown.ClickEvent += new System.EventHandler(this.button_JigInBufStopperUpDown_Click);
            this.button_JigInBufConvRunCWStop.ClickEvent += new System.EventHandler(this.button_JigInBufConvRunCWStop_Click);
            this.button_JigInBufConvRunCWStop.MsgEvent = "Use Jig In Conveyor Run ?";
            this.button_JigInBufConvRunCWStop.MsgEvent2 = "Use JIg In Conveyor Stop ?";

            this.button_JigWorkStopperUpDown.ClickEvent += new System.EventHandler(this.button_JigWorkStopperUpDown_Click);
            this.button_JigWorkConvRunStop.ClickEvent += new System.EventHandler(this.button_JigWorkRunStop_Click);
            this.button_JigWorkConvRunStop.MsgEvent = "Use Jig Work Conveyor Run ?";
            this.button_JigWorkConvRunStop.MsgEvent2 = "Use Jig Work Conveyor Stop ?";

            this.button_JigPalletFwdBwd.ClickEvent += new System.EventHandler(this.button_JigPalletFwdBwd_Click);

            this.button_JigOutBufStopperUpDown.ClickEvent += new System.EventHandler(this.button_JigOutBufStopperUpDown_Click);
            this.button_JigOutBufConvRunCWStop.ClickEvent += new System.EventHandler(this.button_JigOutBufConvRunCWStop_Click);
            this.button_JigOutBufConvRunCWStop.MsgEvent = "Use In Conveyor Run CW ?";
            this.button_JigOutBufConvRunCWStop.MsgEvent2 = "Use In Conveyor Stop ?";

            this.button_JigInCarrierStopperUpDown.ClickEvent += new System.EventHandler(this.button_JigInCarrierStopperUpDown_Click);
            this.button_JigInCarrierConvRunStop.ClickEvent += new System.EventHandler(this.button_JigInCarrierConvRunStop_Click);
            this.button_JigInCarrierConvRunStop.MsgEvent = "Set Jig In Carrier Conveyor Run ?";
            this.button_JigInCarrierConvRunStop.MsgEvent2 = "Set Jig In Carrier Conveyor Stop ?";

            this.button_JigOutCarrierStopperUpDown.ClickEvent += new System.EventHandler(this.button_JigOutCarrierStopperUpDown_Click);
            this.button_JigOutCarrierConvRunStop.ClickEvent += new System.EventHandler(this.button_JigOutCarrierConvRunStop_Click);
            this.button_JigOutCarrierConvRunStop.MsgEvent = "Set Jig Out Carrier Conveyor Run ?";
            this.button_JigOutCarrierConvRunStop.MsgEvent2 = "Set Jig Out Carrier Conveyor Stop ?";

            this.button_CarrierLoading.ClickEvent += new System.EventHandler(this.button_CarrierLoading_Click);
            this.button_CarrierLoading.MsgEvent = "Start Jig Carrier Loading Process ?";

            this.button_InBufLoading.ClickEvent += new System.EventHandler(this.button_InBufLoading_Click);
            this.button_InBufLoading.MsgEvent = "Start Jig InBuf Loading Process ?";

            this.button_WorkLoading.ClickEvent += new System.EventHandler(this.button_WorkLoading_Click);
            this.button_WorkLoading.MsgEvent = "Start Jig Work Loading Process ?";

            this.button_WorkAlign.ClickEvent += new System.EventHandler(this.button_WorkAlign_Click);
            this.button_WorkAlign.MsgEvent = "Start Jig Work Align Process ?";

            this.button_JigOutBufLoading.ClickEvent += new System.EventHandler(this.button_JigOutBufLoading_Click);
            this.button_JigOutBufLoading.MsgEvent = "Start Jig OutBuf Loading Process ?";

            this.button_JigOutBufUnloading.ClickEvent += new System.EventHandler(this.button_JigOutBufUnloading_Click);
            this.button_JigOutBufUnloading.MsgEvent = "Start Jig OutBuf Unloading Process ?";
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
                    case BTN_TAB_SELECT.MANUAL_CONTROL:
                        tabControl1.SelectedIndex = 0;
                        break;
                    case BTN_TAB_SELECT.MANUAL_PROCESS:
                        tabControl1.SelectedIndex = 1;
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
