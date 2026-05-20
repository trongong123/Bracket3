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
using TopEng.Controls;
using TopEng.Utils;
using CAMASSEMBLYMACHINE.Define;
using static CAMASSEMBLYMACHINE.Define.ParameterDefine;

namespace CAMASSEMBLYMACHINE.UI
{
    public partial class tabRightBufferManu3 : Form, IForm
    {
        public tabRightBufferManu3()
        {
            InitializeComponent();

            // Hide TAB Button
            tabControl1.Appearance = TabAppearance.Buttons;
            tabControl1.SizeMode = TabSizeMode.Fixed;
            tabControl1.ItemSize = new Size(0, 1);

            SetBtnClickEvents();
            SelectButton_ClickEvent(nV_Button_TG_NS_ControlTrasnfer, null);
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
        }

        private void UpdateState()
        {
            disp_YReadyPos.ButtonPush = Util.IsAxisInPos(
                (AXIS.CAM_TRANSFER_RIGHT_Y, RecipeDefine.POSITION.CAM_TRANSFER_Y_READY_R_POS));

            disp_YLoadingPos.ButtonPush = Util.IsAxisInPos(
                (AXIS.CAM_TRANSFER_RIGHT_Y, RecipeDefine.POSITION.CAM_TRANSFER_Y_LOADING_R_POS));

            disp_YPeelingPos.ButtonPush = Util.IsAxisInPos(
                (AXIS.CAM_TRANSFER_RIGHT_Y, RecipeDefine.POSITION.CAM_TRANSFER_Y_PEELING_R_POS));

            disp_YUnloadingPos.ButtonPush = Util.IsAxisInPos(
                (AXIS.CAM_TRANSFER_RIGHT_Y, RecipeDefine.POSITION.CAM_TRANSFER_Y_UNLOADING_R_POS));
        }

        private void UpdateIOState()
        {
            uint returnValue1 = 0;

            if (Machine.IO == null)
                return;

            

            Machine.IO.GetOut((int)DO.RIGHT_PEELING_BLOW, ref returnValue1);
            disp_PeelingBlowOn.ButtonPush = returnValue1 == 1 ? true : false;
            disp_PeelingBlowOff.ButtonPush = returnValue1 == 1 ? false : true;

            Machine.IO.GetOut((int)DO.DUST_SUCTION, ref returnValue1);
            disp_DustSuctionOn.ButtonPush = returnValue1 == 1 ? true : false;
            disp_DustSuctionOff.ButtonPush = returnValue1 == 1 ? false : true;
        }
        #region I/O CONTROL EVENT
        private void button_TRFVacOnOff1_Click(object sender, EventArgs e)
        {
            
        }

        private void button_TRFVacOnOff2_Click(object sender, EventArgs e)
        {
            
        }

        private void button_TRFClampUnclamp1_Click(object sender, EventArgs e)
        {
            
        }

        private void button_TRFClampUnclamp2_Click(object sender, EventArgs e)
        {
            
        }

        private void button_TRFUpDown_Click(object sender, EventArgs e)
        {
            
        }

        private void button_Peeling1Up1_Click(object sender, EventArgs e)
        {
            
        }

        private void button_Peeling1Down1_Click(object sender, EventArgs e)
        {
            
        }

        private void button_Peeling1Up2_Click(object sender, EventArgs e)
        {
            
        }

        private void button_Peeling1Down2_Click(object sender, EventArgs e)
        {
            
        }

        private void button_Peeling2Up1_Click(object sender, EventArgs e)
        {
            
        }

        private void button_Peeling2Down1_Click(object sender, EventArgs e)
        {
            
        }

        private void button_Peeling2Up2_Click(object sender, EventArgs e)
        {
            
        }

        private void button_Peeling2Down2_Click(object sender, EventArgs e)
        {
            
        }

        private void button_Peeling1ClampUnclamp_Click(object sender, EventArgs e)
        {
            
        }

        private void button_Peeling2ClampUnclamp_Click(object sender, EventArgs e)
        {
            
        }

        private void button_Peeling1ReturnTurn_Click(object sender, EventArgs e)
        {
            
        }

        private void button_Peeling2ReturnTurn_Click(object sender, EventArgs e)
        {
            
        }

        private void button_PeelingBlow_Click(object sender, EventArgs e)
        {
            uint isVacOn = 0;

            Machine.IO.GetOut((int)DO.RIGHT_PEELING_BLOW, ref isVacOn);
            if (isVacOn == 0)
            {
                LogUtil.Instance.Log(LOG_TYPE.UI, "Right Buffer Dust Box Blow On Button Click", CONTENT_TYPE.INFO);

                Machine.IO.SetOut((int)DO.RIGHT_PEELING_BLOW, 1);
            }
            else if (isVacOn == 1)
            {
                LogUtil.Instance.Log(LOG_TYPE.UI, "Right Buffer Dust Box Blow Off Button Click", CONTENT_TYPE.INFO);

                Machine.IO.SetOut((int)DO.RIGHT_PEELING_BLOW, 0);
            }
        }

        private void button_DustSuctionOnOff_Click(object sender, EventArgs e)
        {
            uint isVacOn = 0;

            Machine.IO.GetOut((int)DO.DUST_SUCTION, ref isVacOn);
            if (isVacOn == 0)
            {
                LogUtil.Instance.Log(LOG_TYPE.UI, "Film Suction On Button Click", CONTENT_TYPE.INFO);

                Machine.IO.SetOut((int)DO.DUST_SUCTION, 1);
            }
            else if (isVacOn == 1)
            {
                LogUtil.Instance.Log(LOG_TYPE.UI, "Film Suction Off Button Click", CONTENT_TYPE.INFO);

                Machine.IO.SetOut((int)DO.DUST_SUCTION, 0);
            }
        }

        uint ret1 = 0;
        uint ret2 = 0;
        uint ret3 = 0;
        uint ret4 = 0;

        //Move Pos 추가 예정
        private void button_YReadyPos_Click(object sender, EventArgs e)
        {
            //LogUtil.Instance.Log(LOG_TYPE.UI, "Right TRF Buffer Move to Ready Pos Button Click", CONTENT_TYPE.INFO);
            //Machine.IO.GetIn((int)DI.PEELING_R1_TURN, ref ret1);
            //Machine.IO.GetIn((int)DI.PEELING_R2_TURN, ref ret2);
            //if (ret1 == 1 || ret2 == 1)
            //{
            //    Dlg_MessageBox formErr =
            //           new Dlg_MessageBox(EMESSAGEBOX.MSG, string.Format($" Interference Cam TRF. Please Return Transfer."));
            //    formErr.TopLevel = true;
            //    formErr.TopMost = true;
            //    formErr.ShowDialog();
            //    return;
            //}
            //Machine.IO.GetIn((int)DI.PEELING_R1_Z1_UP, ref ret1);
            //Machine.IO.GetIn((int)DI.PEELING_R1_Z2_UP, ref ret2);
            //Machine.IO.GetIn((int)DI.PEELING_R2_Z1_UP, ref ret3);
            //Machine.IO.GetIn((int)DI.PEELING_R2_Z2_UP, ref ret4);
            //if (ret1 == 1 || ret2 == 1 || ret3 == 1 || ret4 == 1)
            //{
            //    Dlg_MessageBox formErr2 =
            //           new Dlg_MessageBox(EMESSAGEBOX.MSG, string.Format($" Interference Cam TRF. Please Down All Peeling"));
            //    formErr2.TopLevel = true;
            //    formErr2.TopMost = true;
            //    formErr2.ShowDialog();
            //    return;
            //}
            //var proc = Machine.proclist[(int)Machine.PROCESS.RIGHTBUF] as ProcessBufStageOne;
            //proc.SetMessage((int)ProcessBufStageOne.MSG.MSG_MOVE_READY_POS);
        }

        private void button_YLoadingPos_Click(object sender, EventArgs e)
        {
            LogUtil.Instance.Log(LOG_TYPE.UI, "Right TRF Buffer Move to Loading Pos Button Click", CONTENT_TYPE.INFO);
            //Machine.IO.GetIn((int)DI.PEELING_R1_TURN, ref ret1);
            //Machine.IO.GetIn((int)DI.PEELING_R2_TURN, ref ret2);
            //if (ret1 == 1 || ret2 == 1)
            //{
            //    Dlg_MessageBox formErr =
            //           new Dlg_MessageBox(EMESSAGEBOX.MSG, string.Format($" Interference Cam TRF. Please Return Transfer."));
            //    formErr.TopLevel = true;
            //    formErr.TopMost = true;
            //    formErr.ShowDialog();
            //    return;
            //}
            //Machine.IO.GetIn((int)DI.PEELING_R1_Z1_UP, ref ret1);
            //Machine.IO.GetIn((int)DI.PEELING_R1_Z2_UP, ref ret2);
            //Machine.IO.GetIn((int)DI.PEELING_R2_Z1_UP, ref ret3);
            //Machine.IO.GetIn((int)DI.PEELING_R2_Z2_UP, ref ret4);
            //if (ret1 == 1 || ret2 == 1 || ret3 == 1 || ret4 == 1)
            //{
            //    Dlg_MessageBox formErr2 =
            //           new Dlg_MessageBox(EMESSAGEBOX.MSG, string.Format($" Interference Cam TRF. Please Down All Peeling"));
            //    formErr2.TopLevel = true;
            //    formErr2.TopMost = true;
            //    formErr2.ShowDialog();
            //    return;
            //}
            var proc = Machine.proclist[(int)Machine.PROCESS.RIGHTBUF] as ProcessBufStageOne;
            proc.SetMessage((int)ProcessBufStageOne.MSG.MSG_MOVE_LOADING_POS);
        }

        private void button_YPeelingPos_Click(object sender, EventArgs e)
        {
            LogUtil.Instance.Log(LOG_TYPE.UI, "Right TRF Buffer Move to Peeling Pos Button Click", CONTENT_TYPE.INFO);
            //Machine.IO.GetIn((int)DI.PEELING_R1_TURN, ref ret1);
            //Machine.IO.GetIn((int)DI.PEELING_R2_TURN, ref ret2);
            //if (ret1 == 1 || ret2 == 1)
            //{
            //    Dlg_MessageBox formErr =
            //           new Dlg_MessageBox(EMESSAGEBOX.MSG, string.Format($" Interference Cam TRF. Please Return Transfer."));
            //    formErr.TopLevel = true;
            //    formErr.TopMost = true;
            //    formErr.ShowDialog();
            //    return;
            //}
            //Machine.IO.GetIn((int)DI.PEELING_R1_Z1_UP, ref ret1);
            //Machine.IO.GetIn((int)DI.PEELING_R1_Z2_UP, ref ret2);
            //Machine.IO.GetIn((int)DI.PEELING_R2_Z1_UP, ref ret3);
            //Machine.IO.GetIn((int)DI.PEELING_R2_Z2_UP, ref ret4);
            //if (ret1 == 1 || ret2 == 1 || ret3 == 1 || ret4 == 1)
            //{
            //    Dlg_MessageBox formErr2 =
            //           new Dlg_MessageBox(EMESSAGEBOX.MSG, string.Format($" Interference Cam TRF. Please Down All Peeling"));
            //    formErr2.TopLevel = true;
            //    formErr2.TopMost = true;
            //    formErr2.ShowDialog();
            //    return;
            //}
            var proc = Machine.proclist[(int)Machine.PROCESS.RIGHTBUF] as ProcessBufStageOne;
            proc.SetMessage((int)ProcessBufStageOne.MSG.MSG_MOVE_PEELING_POS);
        }

        private void button_YUnloadingPos_Click(object sender, EventArgs e)
        {
            LogUtil.Instance.Log(LOG_TYPE.UI, "Right TRF Buffer Move to Unloading Pos Button Click", CONTENT_TYPE.INFO);
            //Machine.IO.GetIn((int)DI.PEELING_R1_TURN, ref ret1);
            //Machine.IO.GetIn((int)DI.PEELING_R2_TURN, ref ret2);
            //if (ret1 == 1 || ret2 == 1)
            //{
            //    Dlg_MessageBox formErr =
            //           new Dlg_MessageBox(EMESSAGEBOX.MSG, string.Format($" Interference Cam TRF. Please Return Transfer."));
            //    formErr.TopLevel = true;
            //    formErr.TopMost = true;
            //    formErr.ShowDialog();
            //    return;
            //}
            //Machine.IO.GetIn((int)DI.PEELING_R1_Z1_UP, ref ret1);
            //Machine.IO.GetIn((int)DI.PEELING_R1_Z2_UP, ref ret2);
            //Machine.IO.GetIn((int)DI.PEELING_R2_Z1_UP, ref ret3);
            //Machine.IO.GetIn((int)DI.PEELING_R2_Z2_UP, ref ret4);
            //if (ret1 == 1 || ret2 == 1 || ret3 == 1 || ret4 == 1)
            //{
            //    Dlg_MessageBox formErr2 =
            //           new Dlg_MessageBox(EMESSAGEBOX.MSG, string.Format($" Interference Cam TRF. Please Down All Peeling"));
            //    formErr2.TopLevel = true;
            //    formErr2.TopMost = true;
            //    formErr2.ShowDialog();
            //    return;
            //}
            var proc = Machine.proclist[(int)Machine.PROCESS.RIGHTBUF] as ProcessBufStageOne;
            proc.SetMessage((int)ProcessBufStageOne.MSG.MSG_MOVE_UNLOADING_POS);
        }
        #endregion

        private void button_Loading_Click(object sender, EventArgs e)
        {
            LogUtil.Instance.Log(LOG_TYPE.UI, "Right Buffer Loading Process Button Click", CONTENT_TYPE.INFO);

            var proc = Machine.proclist[(int)Machine.PROCESS.RIGHTBUF];
            //proc.SetMessage((int)ProcessBufStageOne.MSG.MSG_LOADING_PRODUCT);
        }

        private void button_Transfer_Click(object sender, EventArgs e)
        {
            LogUtil.Instance.Log(LOG_TYPE.UI, "Right Buffer Trasnfer Process Button Click", CONTENT_TYPE.INFO);

            var proc = Machine.proclist[(int)Machine.PROCESS.RIGHTBUF];
            proc.SetMessage((int)ProcessBufStageOne.MSG.MSG_TRANSFER_LOAD_PRODUCT);
        }

        private void button_PeelingUpP_Click(object sender, EventArgs e)
        {
            LogUtil.Instance.Log(LOG_TYPE.UI, "Right Buffer Peeling Up Process Button Click", CONTENT_TYPE.INFO);

            var proc = Machine.proclist[(int)Machine.PROCESS.RIGHTBUF];
            proc.SetMessage((int)ProcessBufStageOne.MSG.MSG_PEELING_UP_PRODUCT);
        }

        private void button_PeelingDownP_Click(object sender, EventArgs e)
        {
            LogUtil.Instance.Log(LOG_TYPE.UI, "Right Buffer Peeling Down Process Button Click", CONTENT_TYPE.INFO);

            var proc = Machine.proclist[(int)Machine.PROCESS.RIGHTBUF];
            proc.SetMessage((int)ProcessBufStageOne.MSG.MSG_PEELING_DOWN_PRODUCT);
        }

        private void button_TransferUnload_Click(object sender, EventArgs e)
        {
            LogUtil.Instance.Log(LOG_TYPE.UI, "Right Buffer Transfer Unload Process Button Click", CONTENT_TYPE.INFO);

            var proc = Machine.proclist[(int)Machine.PROCESS.RIGHTBUF];
            proc.SetMessage((int)ProcessBufStageOne.MSG.MSG_TRANSFER_UNLOAD_PRODUCT);
        }

        private void button_Unloading_Click(object sender, EventArgs e)
        {
            LogUtil.Instance.Log(LOG_TYPE.UI, "Right Buffer Unloading Process Button Click", CONTENT_TYPE.INFO);

            var proc = Machine.proclist[(int)Machine.PROCESS.RIGHTBUF];
            //proc.SetMessage((int)ProcessBufStageOne.MSG.MSG_UNLOADING_PRODUCT);
        }
        
        private void button_bothPeel1Zup_Click(object sender, EventArgs e)
        {
            
        }

        private void button_bothPeel1ZDown_Click(object sender, EventArgs e)
        {
            
        }

        private void button_bothPeel2Zup_Click(object sender, EventArgs e)
        {
            
        }

        private void button_bothPeel2ZDown_Click(object sender, EventArgs e)
        {
            
        }

        public enum BTN_TAB_SELECT
        {
            MANUAL_CONTROL_TRANSFER,
            MANUAL_CONTROL_PEELING,
            MANUAL_PROCESS //셋업때 제거
        }
        public void SetBtnClickEvents()
        {
            nV_Button_TG_NS_ControlTrasnfer.ClickEvent += SelectButton_ClickEvent;
            nV_Button_TG_NS_ControlPeeling.ClickEvent += SelectButton_ClickEvent;

            nV_Button_TG_NS_Process.ClickEvent += SelectButton_ClickEvent;

            this.button_TRFVacOnOff1.ClickEvent += new System.EventHandler(this.button_TRFVacOnOff1_Click);
            this.button_TRFVacOnOff2.ClickEvent += new System.EventHandler(this.button_TRFVacOnOff2_Click);
            this.button_TRFClampUnclamp1.ClickEvent += new System.EventHandler(this.button_TRFClampUnclamp1_Click);
            this.button_TRFClampUnclamp2.ClickEvent += new System.EventHandler(this.button_TRFClampUnclamp2_Click);
            this.button_TRFUpDown.ClickEvent += new System.EventHandler(this.button_TRFUpDown_Click);

            this.button_YReadyPos.ClickEvent += new System.EventHandler(this.button_YReadyPos_Click);
            this.button_YReadyPos.MsgEvent = "Move to " + "Right Trasnfer " + " Ready Pos ?";
            this.button_YLoadingPos.ClickEvent += new System.EventHandler(this.button_YLoadingPos_Click);
            this.button_YLoadingPos.MsgEvent = "Move to " + "Right Trasnfer " + "Loading Pos ?";
            this.button_YPeelingPos.ClickEvent += new System.EventHandler(this.button_YPeelingPos_Click);
            this.button_YPeelingPos.MsgEvent = "Move to " + "Right Trasnfer " + "Peeling Pos ?";
            this.button_YUnloadingPos.ClickEvent += new System.EventHandler(this.button_YUnloadingPos_Click);
            this.button_YUnloadingPos.MsgEvent = "Move to " + "Right Trasnfer " + "Unloading Pos ?";

            this.button_Peeling1Up1.ClickEvent += new System.EventHandler(this.button_Peeling1Up1_Click);
            this.button_Peeling1Down1.ClickEvent += new System.EventHandler(this.button_Peeling1Down1_Click);
            this.button_Peeling1Up2.ClickEvent += new System.EventHandler(this.button_Peeling1Up2_Click);
            this.button_Peeling1Down2.ClickEvent += new System.EventHandler(this.button_Peeling1Down2_Click);

            this.button_Peeling2Up1.ClickEvent += new System.EventHandler(this.button_Peeling2Up1_Click);
            this.button_Peeling2Down1.ClickEvent += new System.EventHandler(this.button_Peeling2Down1_Click);
            this.button_Peeling2Up2.ClickEvent += new System.EventHandler(this.button_Peeling2Up2_Click);
            this.button_Peeling2Down2.ClickEvent += new System.EventHandler(this.button_Peeling2Down2_Click);

            this.button_Peeling1ClampUnclamp.ClickEvent += new System.EventHandler(this.button_Peeling1ClampUnclamp_Click);
            this.button_Peeling2ClampUnclamp.ClickEvent += new System.EventHandler(this.button_Peeling2ClampUnclamp_Click);

            this.button_Peeling1ReturnTurn.ClickEvent += new System.EventHandler(this.button_Peeling1ReturnTurn_Click);
            this.button_Peeling1ReturnTurn.MsgEvent = "Set " + "Right Peeling 1" + "Turn ?";
            this.button_Peeling1ReturnTurn.MsgEvent2 = "Set " + "Right Peeling 1" + "Return ?";

            this.button_Peeling2ReturnTurn.ClickEvent += new System.EventHandler(this.button_Peeling2ReturnTurn_Click);
            this.button_Peeling2ReturnTurn.MsgEvent = "Set " + "Right Peeling 2" + "Turn ?";
            this.button_Peeling2ReturnTurn.MsgEvent2 = "Set " + "Right Peeling 2" + "Return ?";

            this.button_PeelingBlow.ClickEvent += new System.EventHandler(this.button_PeelingBlow_Click);
            this.button_PeelingBlow.MsgEvent = "Set " + "Right DustBox " + "Blow ?";

            this.button_DustSuctionOnOff.ClickEvent += new System.EventHandler(this.button_DustSuctionOnOff_Click);

            this.button_Loading.ClickEvent += new System.EventHandler(this.button_Loading_Click);
            this.button_Loading.MsgEvent = "Start " + "Right Buffer " + "Loading Process ?";
            this.button_Transfer.ClickEvent += new System.EventHandler(this.button_Transfer_Click);
            this.button_Transfer.MsgEvent = "Start " + "Right Buffer " + "Transfer Process ?";
            this.button_PeelingUpP.ClickEvent += new System.EventHandler(this.button_PeelingUpP_Click);
            this.button_PeelingUpP.MsgEvent = "Start " + "Right Buffer " + "Peeling Up Process ?";
            this.button_PeelingDownP.ClickEvent += new System.EventHandler(this.button_PeelingDownP_Click);
            this.button_PeelingDownP.MsgEvent = "Start " + "Right Buffer " + "Peeling Down Process ?";
            this.button_TransferUnload.ClickEvent += new System.EventHandler(this.button_TransferUnload_Click);
            this.button_TransferUnload.MsgEvent = "Start " + "Right Buffer " + "Trasnfer Unload Process ?";
            this.button_Unloading.ClickEvent += new System.EventHandler(this.button_Unloading_Click);
            this.button_Unloading.MsgEvent = "Start " + "Right Buffer " + "Unloading Process ?";

            this.button_bothPeel1Zup.ClickEvent += new System.EventHandler(this.button_bothPeel1Zup_Click);
            this.button_bothPeel1ZDown.ClickEvent += new System.EventHandler(this.button_bothPeel1ZDown_Click);
            this.button_bothPeel2Zup.ClickEvent += new System.EventHandler(this.button_bothPeel2Zup_Click);
            this.button_bothPeel2ZDown.ClickEvent += new System.EventHandler(this.button_bothPeel2ZDown_Click);
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
                    case BTN_TAB_SELECT.MANUAL_CONTROL_TRANSFER:
                        tabControl1.SelectedIndex = 0;
                        break;
                    case BTN_TAB_SELECT.MANUAL_CONTROL_PEELING:
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
