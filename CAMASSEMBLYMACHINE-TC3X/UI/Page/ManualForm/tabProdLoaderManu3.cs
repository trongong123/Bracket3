using static CAMASSEMBLYMACHINE.Define.SystemDefine;
using static CAMASSEMBLYMACHINE.Define.UIDefine;
using CAMASSEMBLYMACHINE.Process;
using CAMASSEMBLYMACHINE.Define;
using TopEng.Device;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NV_UI;
using TopEng.Controls;
using TopEng.Utils;
using static CAMASSEMBLYMACHINE.Define.ParameterDefine;

namespace CAMASSEMBLYMACHINE.UI
{
    public partial class tabPROD_LOADERManu3 : Form, IForm
    {
        uint TrayTRFValue = 0;
        List<Label> label_alignPos = new List<Label>();

        public tabPROD_LOADERManu3()
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
            UpdateVisionData();
            UpdateState();
            UpdateIOState();
        }

        private void UpdateState()
        {
            disp_XYRReadyPos.ButtonPush = Util.IsAxisInPos(
                (AXIS.PROD_PICKUP_X, ParameterDefine.POSITION.PROD_LOADER_X_READY_POS),
                (AXIS.PROD_PICKUP_Y, ParameterDefine.POSITION.PROD_LOADER_Y_READY_POS));

            var proc = Machine.proclist[(int)Machine.PROCESS.PROD_LOADER] as ProcessProdLoader;
            if (proc.pickupPos != null && proc.pickupPos.Count() == 4 && proc.targetCurr == 0)
            {
                disp_XYRPickupPos.ButtonPush = Util.IsAxisInPos(
                    (AXIS.PROD_PICKUP_X, proc.pickupPos[0]),
                    (AXIS.PROD_PICKUP_Y, proc.pickupPos[1]),
                    (AXIS.PROD_PICKUP_R1, proc.pickupPos[2]),
                    (AXIS.PROD_PICKUP_R2, proc.pickupPos[3]));
            }
            else disp_XYRPickupPos.ButtonPush = false;

            if (proc.pickupPos != null && proc.pickupPos.Count() == 4 && proc.targetCurr == 1)
            {
                disp_XYRPickupPos2.ButtonPush = Util.IsAxisInPos(
                    (AXIS.PROD_PICKUP_X, proc.pickupPos[0]),
                    (AXIS.PROD_PICKUP_Y, proc.pickupPos[1]),
                    (AXIS.PROD_PICKUP_R1, proc.pickupPos[2]),
                    (AXIS.PROD_PICKUP_R2, proc.pickupPos[3]));
            }
            else disp_XYRPickupPos2.ButtonPush = false;

            disp_XYRLeftPlacePos1.ButtonPush = Util.IsAxisInPos(
                (AXIS.PROD_PICKUP_X, RecipeDefine.POSITION.PROD_LOADER_X_BUF_L_L_POS),
                (AXIS.PROD_PICKUP_Y, RecipeDefine.POSITION.PROD_LOADER_Y_BUF_L_L_POS));

            disp_XYRLeftPlacePos2.ButtonPush = Util.IsAxisInPos(
                (AXIS.PROD_PICKUP_X, RecipeDefine.POSITION.PROD_LOADER_X_BUF_L_R_POS),
                (AXIS.PROD_PICKUP_Y, RecipeDefine.POSITION.PROD_LOADER_Y_BUF_L_R_POS));

            disp_XYRRightPlacePos1.ButtonPush = Util.IsAxisInPos(
                (AXIS.PROD_PICKUP_X, RecipeDefine.POSITION.PROD_LOADER_X_BUF_R_L_POS),
                (AXIS.PROD_PICKUP_Y, RecipeDefine.POSITION.PROD_LOADER_Y_BUF_R_L_POS));

            disp_XYRRightPlacePos1.ButtonPush = Util.IsAxisInPos(
                (AXIS.PROD_PICKUP_X, RecipeDefine.POSITION.PROD_LOADER_X_BUF_R_R_POS),
                (AXIS.PROD_PICKUP_Y, RecipeDefine.POSITION.PROD_LOADER_Y_BUF_R_R_POS));

            disp_XYRTrayTRFAvoidPos.ButtonPush = Util.IsAxisInPos(
                (AXIS.PROD_PICKUP_X, ParameterDefine.POSITION.PROD_LOADER_X_READY_POS),
                (AXIS.PROD_PICKUP_Y, ParameterDefine.POSITION.PROD_LOADER_Y_TRAY_VISION_AVOID_POS));

            disp_ZReadyPos.ButtonPush = Util.IsAxisInPos(
                (AXIS.PROD_PICKUP_Z, ParameterDefine.POSITION.PROD_LOADER_Z_READY_POS));

            disp_ZPickupPos.ButtonPush = Util.IsAxisInPos(
                (AXIS.PROD_PICKUP_Z, RecipeDefine.POSITION.PROD_LOADER_Z_PICKUP_POS));

            disp_ZPlacePos.ButtonPush = Util.IsAxisInPos(
                (AXIS.PROD_PICKUP_Z, RecipeDefine.POSITION.PROD_LOADER_Z_PLACE_POS));
        }

        private void UpdateIOState()
        {
            uint returnValue = 0;

            if (Machine.IO == null)
                return;

            Machine.IO.GetIn((int)DI.CAM_PICKER_Z1_UP, ref returnValue);
            disp_Tool1Up.ButtonPush = returnValue == 1 ? true : false;
            Machine.IO.GetIn((int)DI.CAM_PICKER_Z1_DOWN, ref returnValue);
            disp_Tool1Down.ButtonPush = returnValue == 1 ? true : false;
            Machine.IO.GetOut((int)DO.CAM_PICKER_Z1_UP, ref returnValue);
            button_Tool1UpDown.valueEvent = returnValue;

            Machine.IO.GetIn((int)DI.CAM_PICKER_Z2_UP, ref returnValue);
            disp_Tool2Up.ButtonPush = returnValue == 1 ? true : false;
            Machine.IO.GetIn((int)DI.CAM_PICKER_Z2_DOWN, ref returnValue);
            disp_Tool2Down.ButtonPush = returnValue == 1 ? true : false;
            Machine.IO.GetOut((int)DO.CAM_PICKER_Z2_UP, ref returnValue);
            button_Tool2UpDown.valueEvent = returnValue;

            Machine.IO.GetIn((int)DI.CAM_PICKER_Z1_VACON, ref returnValue);
            disp_Tool1VacOn.ButtonPush = returnValue == 1 ? true : false;
            disp_Tool1VacOff.ButtonPush = returnValue == 0 ? true : false;
            Machine.IO.GetIn((int)DI.CAM_PICKER_Z2_VACON, ref returnValue);
            disp_Tool2VacOn.ButtonPush = returnValue == 1 ? true : false;
            disp_Tool2VacOff.ButtonPush = returnValue == 0 ? true : false;
        }

        private void UpdateVisionData()
        {
            var proc = Machine.proclist[(int)Machine.PROCESS.PROD_LOADER] as ProcessProdLoader;

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

        private void button_XYRReadyPos_Click(object sender, EventArgs e)
        {
            LogUtil.Instance.Log(LOG_TYPE.UI, "ProdLoader Move to Ready Pos Button Click", CONTENT_TYPE.INFO);
            if (Machine.status.state == SystemState.SystemStateERROR)
            {
                Dlg_MessageBox formErr = new Dlg_MessageBox(EMESSAGEBOX.MSG, string.Format("Please push the reset button"));
                formErr.TopLevel = true;
                formErr.TopMost = true;
                formErr.ShowDialog();
                return;
            }

            var proc = Machine.proclist[(int)Machine.PROCESS.PROD_LOADER];
            proc.SetMessage((int)ProcessProdLoader.MSG.MSG_MOVE_READY_POS);
        }

        private void button_XYRPickupPos_Click(object sender, EventArgs e)
        {
            LogUtil.Instance.Log(LOG_TYPE.UI, "ProdLoader Move to Pickup Left Pos Button Click", CONTENT_TYPE.INFO);
            if (Machine.status.state == SystemState.SystemStateERROR)
            {
                Dlg_MessageBox formErr = new Dlg_MessageBox(EMESSAGEBOX.MSG, string.Format("Please push the reset button"));
                formErr.TopLevel = true;
                formErr.TopMost = true;
                formErr.ShowDialog();
                return;
            }

            var proc = Machine.proclist[(int)Machine.PROCESS.PROD_LOADER];
            proc.SetMessage((int)ProcessProdLoader.MSG.MSG_MOVE_PICKUP_L_POS);
        }

        private void button_XYRPickupPos2_Click(object sender, EventArgs e)
        {
            LogUtil.Instance.Log(LOG_TYPE.UI, "ProdLoader Move to Pickup Right Pos Button Click", CONTENT_TYPE.INFO);
            if (Machine.status.state == SystemState.SystemStateERROR)
            {
                Dlg_MessageBox formErr = new Dlg_MessageBox(EMESSAGEBOX.MSG, string.Format("Please push the reset button"));
                formErr.TopLevel = true;
                formErr.TopMost = true;
                formErr.ShowDialog();
                return;
            }

            var proc = Machine.proclist[(int)Machine.PROCESS.PROD_LOADER];
            proc.SetMessage((int)ProcessProdLoader.MSG.MSG_MOVE_PICKUP_R_POS);
        }

        private void button_XYRAlignPos2_Click(object sender, EventArgs e)
        {
            LogUtil.Instance.Log(LOG_TYPE.UI, "ProdLoader Move to Align Right Pos Button Click", CONTENT_TYPE.INFO);
            if (Machine.status.state == SystemState.SystemStateERROR)
            {
                Dlg_MessageBox formErr = new Dlg_MessageBox(EMESSAGEBOX.MSG, string.Format("Please push the reset button"));
                formErr.TopLevel = true;
                formErr.TopMost = true;
                formErr.ShowDialog();
                return;
            }

            var proc = Machine.proclist[(int)Machine.PROCESS.PROD_LOADER];
            proc.SetMessage((int)ProcessProdLoader.MSG.MSG_MOVE_ALIGN_R_POS);
        }

        private void button_XYRAlignPos_Click(object sender, EventArgs e)
        {
            LogUtil.Instance.Log(LOG_TYPE.UI, "ProdLoader Move to Align Left Pos Button Click", CONTENT_TYPE.INFO);
            if (Machine.status.state == SystemState.SystemStateERROR)
            {
                Dlg_MessageBox formErr = new Dlg_MessageBox(EMESSAGEBOX.MSG, string.Format("Please push the reset button"));
                formErr.TopLevel = true;
                formErr.TopMost = true;
                formErr.ShowDialog();
                return;
            }

            var proc = Machine.proclist[(int)Machine.PROCESS.PROD_LOADER];
            proc.SetMessage((int)ProcessProdLoader.MSG.MSG_MOVE_ALIGN_L_POS);
        }

        private void button_XYRLeft1PlacePos_Click(object sender, EventArgs e)
        {
            LogUtil.Instance.Log(LOG_TYPE.UI, "ProdLoader Move to Place Left Pos Button Click", CONTENT_TYPE.INFO);
            if (Machine.status.state == SystemState.SystemStateERROR)
            {
                Dlg_MessageBox formErr = new Dlg_MessageBox(EMESSAGEBOX.MSG, string.Format("Please push the reset button"));
                formErr.TopLevel = true;
                formErr.TopMost = true;
                formErr.ShowDialog();
                return;
            }

            var proc = Machine.proclist[(int)Machine.PROCESS.PROD_LOADER] as ProcessProdLoader;
            proc.targetCurr = 0;
            proc.SetMessage((int)ProcessProdLoader.MSG.MSG_MOVE_PLACE_L_POS);
        }

        private void button_XYRLeft2PlacePos_Click(object sender, EventArgs e)
        {
            LogUtil.Instance.Log(LOG_TYPE.UI, "ProdLoader Move to Place Left Pos Button Click", CONTENT_TYPE.INFO);
            if (Machine.status.state == SystemState.SystemStateERROR)
            {
                Dlg_MessageBox formErr = new Dlg_MessageBox(EMESSAGEBOX.MSG, string.Format("Please push the reset button"));
                formErr.TopLevel = true;
                formErr.TopMost = true;
                formErr.ShowDialog();
                return;
            }

            var proc = Machine.proclist[(int)Machine.PROCESS.PROD_LOADER] as ProcessProdLoader;
            proc.targetCurr = 0;
            proc.SetMessage((int)ProcessProdLoader.MSG.MSG_MOVE_PLACE_R_POS);
        }

        private void button_XYRRight1PlacePos_Click(object sender, EventArgs e)
        {
            LogUtil.Instance.Log(LOG_TYPE.UI, "ProdLoader Move to Place Right Pos Button Click", CONTENT_TYPE.INFO);
            if (Machine.status.state == SystemState.SystemStateERROR)
            {
                Dlg_MessageBox formErr = new Dlg_MessageBox(EMESSAGEBOX.MSG, string.Format("Please push the reset button"));
                formErr.TopLevel = true;
                formErr.TopMost = true;
                formErr.ShowDialog();
                return;
            }

            var proc = Machine.proclist[(int)Machine.PROCESS.PROD_LOADER] as ProcessProdLoader;
            proc.targetCurr = 1;
            proc.SetMessage((int)ProcessProdLoader.MSG.MSG_MOVE_PLACE_L_POS);
        }

        private void button_XYRRight2PlacePos_Click(object sender, EventArgs e)
        {
            LogUtil.Instance.Log(LOG_TYPE.UI, "ProdLoader Move to Place Right Pos Button Click", CONTENT_TYPE.INFO);
            if (Machine.status.state == SystemState.SystemStateERROR)
            {
                Dlg_MessageBox formErr = new Dlg_MessageBox(EMESSAGEBOX.MSG, string.Format("Please push the reset button"));
                formErr.TopLevel = true;
                formErr.TopMost = true;
                formErr.ShowDialog();
                return;
            }

            var proc = Machine.proclist[(int)Machine.PROCESS.PROD_LOADER] as ProcessProdLoader;
            proc.targetCurr = 1;
            proc.SetMessage((int)ProcessProdLoader.MSG.MSG_MOVE_PLACE_R_POS);
        }

        private void button_XYRTrayTRFAvoidPos_Click(object sender, EventArgs e)
        {
            LogUtil.Instance.Log(LOG_TYPE.UI, "ProdLoader Move to Tray Transfer Avoid Pos Button Click", CONTENT_TYPE.INFO);
            if (Machine.status.state == SystemState.SystemStateERROR)
            {
                Dlg_MessageBox formErr = new Dlg_MessageBox(EMESSAGEBOX.MSG, string.Format("Please push the reset button"));
                formErr.TopLevel = true;
                formErr.TopMost = true;
                formErr.ShowDialog();
                return;
            }

            var proc = Machine.proclist[(int)Machine.PROCESS.PROD_LOADER] as ProcessProdLoader;
            proc.SetMessage((int)ProcessProdLoader.MSG.MSG_MOVE_TRAYTRF_AVOID_POS);
        }

        private void button_ZReadyPos_Click(object sender, EventArgs e)
        {
            LogUtil.Instance.Log(LOG_TYPE.UI, "ProdLoader Move to Ready (Z) Pos Button Click", CONTENT_TYPE.INFO);
            if (Machine.status.state == SystemState.SystemStateERROR)
            {
                Dlg_MessageBox formErr = new Dlg_MessageBox(EMESSAGEBOX.MSG, string.Format("Please push the reset button"));
                formErr.TopLevel = true;
                formErr.TopMost = true;
                formErr.ShowDialog();
                return;
            }

            var proc = Machine.proclist[(int)Machine.PROCESS.PROD_LOADER];
            proc.SetMessage((int)ProcessProdLoader.MSG.MSG_SINGLE, (int)ProcessProdLoader.STEP.MOVE_Z_READY_POS);
        }

        private void button_ZPickupPos_Click(object sender, EventArgs e)
        {
            LogUtil.Instance.Log(LOG_TYPE.UI, "ProdLoader Move to Pickup (Z) Pos Button Click", CONTENT_TYPE.INFO);
            if (Machine.status.state == SystemState.SystemStateERROR)
            {
                Dlg_MessageBox formErr = new Dlg_MessageBox(EMESSAGEBOX.MSG, string.Format("Please push the reset button"));
                formErr.TopLevel = true;
                formErr.TopMost = true;
                formErr.ShowDialog();
                return;
            }

            var proc = Machine.proclist[(int)Machine.PROCESS.PROD_LOADER];
            proc.SetMessage((int)ProcessProdLoader.MSG.MSG_SINGLE, (int)ProcessProdLoader.STEP.MOVE_Z_PICKUP_POS);
        }

        private void button_ZAlignPos_Click(object sender, EventArgs e)
        {
            LogUtil.Instance.Log(LOG_TYPE.UI, "ProdLoader Move to Align (Z) Pos Button Click", CONTENT_TYPE.INFO);
            if (Machine.status.state == SystemState.SystemStateERROR)
            {
                Dlg_MessageBox formErr = new Dlg_MessageBox(EMESSAGEBOX.MSG, string.Format("Please push the reset button"));
                formErr.TopLevel = true;
                formErr.TopMost = true;
                formErr.ShowDialog();
                return;
            }

            var proc = Machine.proclist[(int)Machine.PROCESS.PROD_LOADER];
            proc.SetMessage((int)ProcessProdLoader.MSG.MSG_SINGLE, (int)ProcessProdLoader.STEP.MOVE_Z_UNDER_ALIGN_POS);
        }

        private void button_ZPlacePos_Click(object sender, EventArgs e)
        {
            LogUtil.Instance.Log(LOG_TYPE.UI, "ProdLoader Move to Place (Z) Pos Button Click", CONTENT_TYPE.INFO);
            if (Machine.status.state == SystemState.SystemStateERROR)
            {
                Dlg_MessageBox formErr = new Dlg_MessageBox(EMESSAGEBOX.MSG, string.Format("Please push the reset button"));
                formErr.TopLevel = true;
                formErr.TopMost = true;
                formErr.ShowDialog();
                return;
            }

            var proc = Machine.proclist[(int)Machine.PROCESS.PROD_LOADER];
            proc.SetMessage((int)ProcessProdLoader.MSG.MSG_SINGLE, (int)ProcessProdLoader.STEP.MOVE_Z_PLACE_POS);
        }

        private void button_Tool1UpDown_Click(object sender, EventArgs e)
        {
            uint ret1 = 0;

            Machine.IO.GetOut((int)DO.CAM_PICKER_Z1_UP, ref ret1);

            if (ret1 == 0)
            {
                LogUtil.Instance.Log(LOG_TYPE.UI, "ProdLoader Left Picker Up Button Click", CONTENT_TYPE.INFO);

                Machine.IO.SetOut((int)DO.CAM_PICKER_Z1_UP, 1);
                Machine.IO.SetOut((int)DO.CAM_PICKER_Z1_DOWN, 0);
            }
            else if (ret1 == 1)
            {
                LogUtil.Instance.Log(LOG_TYPE.UI, "ProdLoader Left Picker Down Button Click", CONTENT_TYPE.INFO);

                Machine.IO.SetOut((int)DO.CAM_PICKER_Z1_UP, 0);
                Machine.IO.SetOut((int)DO.CAM_PICKER_Z1_DOWN, 1);
            }
        }
        private void button_Tool2UpDown_Click(object sender, EventArgs e)
        {
            uint ret1 = 0;

            Machine.IO.GetOut((int)DO.CAM_PICKER_Z2_UP, ref ret1);

            if (ret1 == 0)
            {
                LogUtil.Instance.Log(LOG_TYPE.UI, "ProdLoader Right Picker Up Button Click", CONTENT_TYPE.INFO);

                Machine.IO.SetOut((int)DO.CAM_PICKER_Z2_UP, 1);
                Machine.IO.SetOut((int)DO.CAM_PICKER_Z2_DOWN, 0);
            }
            else if (ret1 == 1)
            {
                LogUtil.Instance.Log(LOG_TYPE.UI, "ProdLoader Right Picker Down Button Click", CONTENT_TYPE.INFO);

                Machine.IO.SetOut((int)DO.CAM_PICKER_Z2_UP, 0);
                Machine.IO.SetOut((int)DO.CAM_PICKER_Z2_DOWN, 1);
            }
        }

        private void button_Tool1VacOnOff_Click(object sender, EventArgs e)
        {
            uint ret1 = 0;

            Machine.IO.GetOut((int)DO.CAM_PICKER_Z1_VACON, ref ret1);

            if (ret1 == 0)
            {
                LogUtil.Instance.Log(LOG_TYPE.UI, "ProdLoader Left Vacuum On Button Click", CONTENT_TYPE.INFO);

                Machine.IO.SetOut((int)DO.CAM_PICKER_Z1_VACON, 1);
                Machine.IO.SetOut((int)DO.CAM_PICKER_Z1_PURGE, 0);
            }
            else if (ret1 == 1)
            {
                LogUtil.Instance.Log(LOG_TYPE.UI, "ProdLoader Left Vacuum Off Button Click", CONTENT_TYPE.INFO);

                Machine.IO.SetOut((int)DO.CAM_PICKER_Z1_VACON, 0);
                Machine.IO.SetOut((int)DO.CAM_PICKER_Z1_PURGE, 1);
                Util.Delay((int)Machine.param.Time(ParameterDefine.TIME.PURGE_OFF_WAIT_TIME) * 10);
                Machine.IO.SetOut((int)DO.CAM_PICKER_Z1_PURGE, 0);
            }
        }

        private void button_Tool2VacOnOff_Click(object sender, EventArgs e)
        {
            uint ret1 = 0;

            Machine.IO.GetOut((int)DO.CAM_PICKER_Z2_VACON, ref ret1);

            if (ret1 == 0)
            {
                LogUtil.Instance.Log(LOG_TYPE.UI, "ProdLoader Right Vacuum On Button Click", CONTENT_TYPE.INFO);

                Machine.IO.SetOut((int)DO.CAM_PICKER_Z2_VACON, 1);
                Machine.IO.SetOut((int)DO.CAM_PICKER_Z2_PURGE, 0);
            }
            else if (ret1 == 1)
            {
                LogUtil.Instance.Log(LOG_TYPE.UI, "ProdLoader Right Vacuum Off Button Click", CONTENT_TYPE.INFO);

                Machine.IO.SetOut((int)DO.CAM_PICKER_Z2_VACON, 0);
                Machine.IO.SetOut((int)DO.CAM_PICKER_Z2_PURGE, 1);
                Util.Delay((int)Machine.param.Time(ParameterDefine.TIME.PURGE_OFF_WAIT_TIME) * 10);
                Machine.IO.SetOut((int)DO.CAM_PICKER_Z2_PURGE, 0);
            }
        }

        private void button_Pickup1_Click(object sender, EventArgs e)
        {
            LogUtil.Instance.Log(LOG_TYPE.UI, "ProdLoader Pickup Left Process Button Click", CONTENT_TYPE.INFO);
            if (Machine.status.state == SystemState.SystemStateERROR)
            {
                Dlg_MessageBox formErr = new Dlg_MessageBox(EMESSAGEBOX.MSG, string.Format("Please push the reset button"));
                formErr.TopLevel = true;
                formErr.TopMost = true;
                formErr.ShowDialog();
                return;
            }

            var proc1 = Machine.proclist[(int)Machine.PROCESS.TRAYWORK];
            proc1.SetMessage((int)ProcessTrayWork.MSG.MSG_PICKUP_PRODUCT);
            var proc2 = Machine.proclist[(int)Machine.PROCESS.PROD_LOADER] as ProcessProdLoader;
            proc2.targetCurr = 0;
            proc2.SetMessage((int)ProcessProdLoader.MSG.MSG_LOADING_PRODUCT);
        }

        private void button_Pickup2_Click(object sender, EventArgs e)
        {
            LogUtil.Instance.Log(LOG_TYPE.UI, "ProdLoader Pickup Right Process Button Click", CONTENT_TYPE.INFO);
            if (Machine.status.state == SystemState.SystemStateERROR)
            {
                Dlg_MessageBox formErr = new Dlg_MessageBox(EMESSAGEBOX.MSG, string.Format("Please push the reset button"));
                formErr.TopLevel = true;
                formErr.TopMost = true;
                formErr.ShowDialog();
                return;
            }

            var proc1 = Machine.proclist[(int)Machine.PROCESS.TRAYWORK];
            proc1.SetMessage((int)ProcessTrayWork.MSG.MSG_PICKUP_PRODUCT);
            var proc2 = Machine.proclist[(int)Machine.PROCESS.PROD_LOADER] as ProcessProdLoader;
            proc2.targetCurr = 1;
            proc2.SetMessage((int)ProcessProdLoader.MSG.MSG_LOADING_PRODUCT);
        }

        private void button_Align1_Click(object sender, EventArgs e)
        {
            LogUtil.Instance.Log(LOG_TYPE.UI, "ProdLoader Align Left Process Button Click", CONTENT_TYPE.INFO);
            if (Machine.status.state == SystemState.SystemStateERROR)
            {
                Dlg_MessageBox formErr = new Dlg_MessageBox(EMESSAGEBOX.MSG, string.Format("Please push the reset button"));
                formErr.TopLevel = true;
                formErr.TopMost = true;
                formErr.ShowDialog();
                return;
            }

            var proc2 = Machine.proclist[(int)Machine.PROCESS.PROD_LOADER] as ProcessProdLoader;
            proc2.SetMessage((int)ProcessProdLoader.MSG.MSG_ALIGN_PRODUCT);
        }

        private void button_Align2_Click(object sender, EventArgs e)
        {
            LogUtil.Instance.Log(LOG_TYPE.UI, "ProdLoader Align Right Process Button Click", CONTENT_TYPE.INFO);
            if (Machine.status.state == SystemState.SystemStateERROR)
            {
                Dlg_MessageBox formErr = new Dlg_MessageBox(EMESSAGEBOX.MSG, string.Format("Please push the reset button"));
                formErr.TopLevel = true;
                formErr.TopMost = true;
                formErr.ShowDialog();
                return;
            }

            var proc2 = Machine.proclist[(int)Machine.PROCESS.PROD_LOADER] as ProcessProdLoader;
            proc2.SetMessage((int)ProcessProdLoader.MSG.MSG_ALIGN2_PRODUCT);
        }

        private void button_PlaceLeft_Click(object sender, EventArgs e)
        {
            LogUtil.Instance.Log(LOG_TYPE.UI, "ProdLoader Place Left Process Button Click", CONTENT_TYPE.INFO);
            if (Machine.status.state == SystemState.SystemStateERROR)
            {
                Dlg_MessageBox formErr = new Dlg_MessageBox(EMESSAGEBOX.MSG, string.Format("Please push the reset button"));
                formErr.TopLevel = true;
                formErr.TopMost = true;
                formErr.ShowDialog();
                return;
            }

            //double actualPos = 0;
            //double pos = 0;
            //Machine.motion.GetAxisActualPos((int)AXIS.CAM_TRANSFER_LEFT_Y, ref actualPos);
            //pos = 1000 * Machine.param.Position(ParameterDefine.POSITION.CAM_TRANSFER_Y_LOADING_POS);
            //if (Math.Abs(actualPos - pos) <= 5)
            //{
            //    Dlg_MessageBox formErr = new Dlg_MessageBox(EMESSAGEBOX.MSG, string.Format("Left Buf TRF is Not Forward, Please Check Forward Status."));
            //    formErr.ShowDialog();
            //    return;
            //}

            //var proc1 = Machine.proclist[(int)Machine.PROCESS.LEFTBUF];
            //proc1.SetMessage((int)ProcessBufStageOne.MSG.MSG_LOADING_PRODUCT);
            var proc2 = Machine.proclist[(int)Machine.PROCESS.PROD_LOADER] as ProcessProdLoader;
            proc2.targetCurr = 0;
            proc2.SetMessage((int)ProcessProdLoader.MSG.MSG_UNLOADING_PRODUCT);
        }

        private void button_PlaceRight_Click(object sender, EventArgs e)
        {
            LogUtil.Instance.Log(LOG_TYPE.UI, "ProdLoader Place Right Process Button Click", CONTENT_TYPE.INFO);
            if (Machine.status.state == SystemState.SystemStateERROR)
            {
                Dlg_MessageBox formErr = new Dlg_MessageBox(EMESSAGEBOX.MSG, string.Format("Please push the reset button"));
                formErr.TopLevel = true;
                formErr.TopMost = true;
                formErr.ShowDialog();
                return;
            }

            //var proc1 = Machine.proclist[(int)Machine.PROCESS.RIGHTBUF];
            //proc1.SetMessage((int)ProcessBufStageOne.MSG.MSG_LOADING_PRODUCT);
            var proc2 = Machine.proclist[(int)Machine.PROCESS.PROD_LOADER] as ProcessProdLoader;
            proc2.targetCurr = 1;
            proc2.SetMessage((int)ProcessProdLoader.MSG.MSG_UNLOADING_PRODUCT);
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

            this.button_XYRReadyPos.ClickEvent += new System.EventHandler(this.button_XYRReadyPos_Click);
            this.button_XYRReadyPos.MsgEvent = "Move to " + "ProdLoader " + "Ready Pos ?";

            this.button_ZReadyPos.ClickEvent += new System.EventHandler(this.button_ZReadyPos_Click);
            this.button_ZReadyPos.MsgEvent = "Move to " + "ProdLoader " + "Ready Pos (Z) ?";

            this.button_XYRPickupPos.ClickEvent += new System.EventHandler(this.button_XYRPickupPos_Click);
            this.button_XYRPickupPos.MsgEvent = "Move to " + "ProdLoader " + "Pickup Pos (L) ?";

            this.button_XYRPickupPos2.ClickEvent += new System.EventHandler(this.button_XYRPickupPos2_Click);
            this.button_XYRPickupPos2.MsgEvent = "Move to " + "ProdLoader " + "Pickup Pos (R) ?";

            this.button_XYRAlignPos.ClickEvent += new System.EventHandler(this.button_XYRAlignPos_Click);
            this.button_XYRAlignPos.MsgEvent = "Move to " + "ProdLoader " + "Align Pos (L) ?";

            this.button_XYRAlignPos2.ClickEvent += new System.EventHandler(this.button_XYRAlignPos2_Click);
            this.button_XYRAlignPos2.MsgEvent = "Move to " + "ProdLoader " + "Pickup Pos (R) ?";

            this.button_ZPickupPos.ClickEvent += new System.EventHandler(this.button_ZPickupPos_Click);
            this.button_ZPickupPos.MsgEvent = "Move to " + "ProdLoader " + "Pickup Pos (Z) ?";

            this.button_ZAlignPos.ClickEvent += new System.EventHandler(this.button_ZAlignPos_Click);
            this.button_ZAlignPos.MsgEvent = "Move to " + "ProdLoader " + "Align Pos (Z) ?";

            this.button_XYRLeftPlacePos1.ClickEvent += new System.EventHandler(this.button_XYRLeft1PlacePos_Click);
            this.button_XYRLeftPlacePos1.MsgEvent = "Move to " + "ProdLoader " + "Place Pos (L-L) ?";

            this.button_XYRLeftPlacePos2.ClickEvent += new System.EventHandler(this.button_XYRLeft2PlacePos_Click);
            this.button_XYRLeftPlacePos2.MsgEvent = "Move to " + "ProdLoader " + "Place Pos (L-R) ?";

            this.button_XYRRightPlacePos1.ClickEvent += new System.EventHandler(this.button_XYRRight1PlacePos_Click);
            this.button_XYRRightPlacePos1.MsgEvent = "Move to " + "ProdLoader " + "Place Pos (R-L) ?";

            this.button_XYRRightPlacePos2.ClickEvent += new System.EventHandler(this.button_XYRRight2PlacePos_Click);
            this.button_XYRRightPlacePos2.MsgEvent = "Move to " + "ProdLoader " + "Place Pos (R_R) ?";

            this.button_ZPlacePos.ClickEvent += new System.EventHandler(this.button_ZPlacePos_Click);
            this.button_ZPlacePos.MsgEvent = "Move to " + "ProdLoader " + "Place Pos (Z) ?";

            this.button_XYRTrayTRFAvoidPos.ClickEvent += new System.EventHandler(this.button_XYRTrayTRFAvoidPos_Click);
            this.button_XYRTrayTRFAvoidPos.MsgEvent = "Move to " + "ProdLoader " + "Avoid Pos ?";

            this.button_Tool1UpDown.ClickEvent += new System.EventHandler(this.button_Tool1UpDown_Click);
            this.button_Tool1UpDown.MsgEvent = "Move to " + "ProdLoader " + "Left Up ?";
            this.button_Tool1UpDown.MsgEvent2 = "Move to " + "ProdLoader " + "Left Down ?";

            this.button_Tool1VacOnOff.ClickEvent += new System.EventHandler(this.button_Tool1VacOnOff_Click);
            this.button_Tool1VacOnOff.MsgEvent = "Use " + "ProdLoader " + "Left VacOn ?";
            this.button_Tool1VacOnOff.MsgEvent2 = "Use " + "ProdLoader " + "Left VacOff ?";

            this.button_Tool2UpDown.ClickEvent += new System.EventHandler(this.button_Tool2UpDown_Click);
            this.button_Tool2UpDown.MsgEvent = "Use " + "ProdLoader " + "Right Up ?";
            this.button_Tool2UpDown.MsgEvent2 = "Use " + "ProdLoader " + "Right Down ?";

            this.button_Tool2VacOnOff.ClickEvent += new System.EventHandler(this.button_Tool2VacOnOff_Click);
            this.button_Tool2VacOnOff.MsgEvent = "Use " + "ProdLoader " + "Right VacOn ?";
            this.button_Tool2VacOnOff.MsgEvent = "Use " + "ProdLoader " + "Right VacOff ?";

            this.button_PickupLeft.ClickEvent += new System.EventHandler(this.button_Pickup1_Click);
            this.button_PickupLeft.MsgEvent = "Start " + "ProdLoader " + "PickupLeft Process ?";

            this.button_PickupRight.ClickEvent += new System.EventHandler(this.button_Pickup2_Click);
            this.button_PickupRight.MsgEvent = "Start " + "ProdLoader " + "PickupRight Process ?";

            this.button_AlignLeft.ClickEvent += new System.EventHandler(this.button_Align1_Click);
            this.button_AlignLeft.MsgEvent = "Start " + "ProdLoader " + "AlignLeft Process ?";

            this.button_AlignRight.ClickEvent += new System.EventHandler(this.button_Align2_Click);
            this.button_AlignRight.MsgEvent = "Start " + "ProdLoader " + "AlignRight Process ?";

            this.button_PlaceLeft.ClickEvent += new System.EventHandler(this.button_PlaceLeft_Click);
            this.button_PlaceLeft.MsgEvent = "Start " + "ProdLoader " + "PlaceLeft Process ?";

            this.button_PlaceRight.ClickEvent += new System.EventHandler(this.button_PlaceRight_Click);
            this.button_PlaceRight.MsgEvent = "Start " + "ProdLoader " + "PlaceRight Process ?";
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

        private void button_PickerView_Click(object sender, EventArgs e)
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
