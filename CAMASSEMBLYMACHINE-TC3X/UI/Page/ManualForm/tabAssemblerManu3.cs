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
    public partial class tabAssemblerManu3 : Form, IForm
    {
        List<Label> label_alignPos = new List<Label>();
        List<Label> label_BarcodeData = new List<Label>();

        public tabAssemblerManu3()
        {
            InitializeComponent();

            label_alignPos.Add(disp_alignPos1);
            label_alignPos.Add(disp_alignPos2);

            label_BarcodeData.Add(disp_BarcodeLeft);
            label_BarcodeData.Add(disp_BarcodeRight);

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
            UpdateBarcodeData();
        }

        private void UpdateState()
        {
            var proc = Machine.proclist[(int)Machine.PROCESS.ASSEMBLER] as ProcessAssembler;

            disp_XYRReadyPos.ButtonPush = Util.IsAxisInPos(
                (AXIS.ASSEMBLER_X, POSITION.ASSEMBLER_X_READY_POS),
                (AXIS.ASSEMBLER_Y, POSITION.ASSEMBLER_Y_READY_POS));

            disp_XYRLeftPickupPos.ButtonPush = Util.IsAxisInPos(
               (AXIS.ASSEMBLER_X, RecipeDefine.POSITION.ASSEMBLER_X_BUF_L_POS),
               (AXIS.ASSEMBLER_Y, RecipeDefine.POSITION.ASSEMBLER_Y_BUF_L_POS));

            disp_XYRRightPickupPos.ButtonPush = Util.IsAxisInPos(
                 (AXIS.ASSEMBLER_X, RecipeDefine.POSITION.ASSEMBLER_X_BUF_R_POS),
                 (AXIS.ASSEMBLER_Y, RecipeDefine.POSITION.ASSEMBLER_Y_BUF_R_POS));

            if (proc.targetCurr == 0)
            {
                disp_XYRAlignPos.ButtonPush = Util.IsAxisInPos(
                    (AXIS.ASSEMBLER_X, proc.alignPosView[0]),
                    (AXIS.ASSEMBLER_Y, proc.alignPosView[1]));
            }
            else disp_XYRAlignPos.ButtonPush = false;

            if (proc.targetCurr == 1)
            {
                disp_XYRAlignPos2.ButtonPush = Util.IsAxisInPos(
                    (AXIS.ASSEMBLER_X, proc.alignPosView[0]),
                    (AXIS.ASSEMBLER_Y, proc.alignPosView[1]));
            }
            else disp_XYRAlignPos2.ButtonPush = false;

            if (proc.placePos != null && proc.placePos.Count() == 4 && proc.targetCurr == 0)
            {
                disp_XYRPlacePos.ButtonPush = Util.IsAxisInPos(
                    (AXIS.ASSEMBLER_X, proc.placePos[0]),
                    (AXIS.ASSEMBLER_Y, proc.placePos[1]),
                    (AXIS.ASSEMBLER_R1, proc.placePos[2]),
                    (AXIS.ASSEMBLER_R2, proc.placePos[3]));
            }
            else disp_XYRPlacePos.ButtonPush = false;

            if (proc.placePos != null && proc.placePos.Count() == 4 && proc.targetCurr == 1)
            {
                disp_XYRPlacePos2.ButtonPush = Util.IsAxisInPos(
                    (AXIS.ASSEMBLER_X, proc.placePos[0]),
                    (AXIS.ASSEMBLER_Y, proc.placePos[1]),
                    (AXIS.ASSEMBLER_R1, proc.placePos[2]),
                    (AXIS.ASSEMBLER_R2, proc.placePos[3]));
            }
            else disp_XYRPlacePos2.ButtonPush = false;

            disp_XYRNGBoxPos.ButtonPush = Util.IsAxisInPos(
                (AXIS.ASSEMBLER_X, ParameterDefine.POSITION.ASSEMBLER_X_NG_BOX_LEFT_POS),
                (AXIS.ASSEMBLER_Y, ParameterDefine.POSITION.ASSEMBLER_Y_NG_BOX_LEFT_POS));

            disp_XYRNGBox2Pos.ButtonPush = Util.IsAxisInPos(
                (AXIS.ASSEMBLER_X, ParameterDefine.POSITION.ASSEMBLER_X_NG_BOX_RIGHT_POS),
                (AXIS.ASSEMBLER_Y, ParameterDefine.POSITION.ASSEMBLER_Y_NG_BOX_RIGHT_POS));

            disp_ZReadyPos.ButtonPush = Util.IsAxisInPos(
                (AXIS.ASSEMBLER_Z, ParameterDefine.POSITION.ASSEMBLER_Z_READY_POS));

            disp_ZPickupPos.ButtonPush = Util.IsAxisInPos(
                (AXIS.ASSEMBLER_Z, RecipeDefine.POSITION.ASSEMBLER_Z_PICKUP_POS));

            disp_ZAlignPos.ButtonPush = Util.IsAxisInPos(
                (AXIS.ASSEMBLER_Z, RecipeDefine.POSITION.ASSEMBLER_Z_UNDER_ALIGN_POS));

            disp_ZPlacePosL.ButtonPush = Util.IsAxisInPos(
              (AXIS.ASSEMBLER_Z, RecipeDefine.POSITION.ASSEMBLER_Z_PLACE_L_POS));

            disp_ZPlacePosR.ButtonPush = Util.IsAxisInPos(
            (AXIS.ASSEMBLER_Z, RecipeDefine.POSITION.ASSEMBLER_Z_PLACE_R_POS));

            disp_ZNGBoxPos.ButtonPush = Util.IsAxisInPos(
              (AXIS.ASSEMBLER_Z, ParameterDefine.POSITION.ASSEMBLER_Z_DISCARD_POS));
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
            Machine.IO.GetOut((int)DO.ASSEMBLER_Z1_UP, ref returnValue);
            button_Tool1UpDown.valueEvent = returnValue;

            Machine.IO.GetIn((int)DI.ASSEMBLER_Z2_UP, ref returnValue);
            disp_Tool2Up.ButtonPush = returnValue == 1 ? true : false;
            Machine.IO.GetIn((int)DI.ASSEMBLER_Z2_DOWN, ref returnValue);
            disp_Tool2Down.ButtonPush = returnValue == 1 ? true : false;
            Machine.IO.GetOut((int)DO.ASSEMBLER_Z2_UP, ref returnValue);
            button_Tool2UpDown.valueEvent = returnValue;

            Machine.IO.GetIn((int)DI.ASSEMBLER_Z1_VACON, ref returnValue);
            disp_Tool1VacOn.ButtonPush = returnValue == 1 ? true : false;
            disp_Tool1VacOff.ButtonPush = returnValue == 0 ? true : false;
            Machine.IO.GetIn((int)DI.ASSEMBLER_Z2_VACON, ref returnValue);
            disp_Tool2VacOn.ButtonPush = returnValue == 1 ? true : false;
            disp_Tool2VacOff.ButtonPush = returnValue == 0 ? true : false;
        }

        private void UpdateMachineData()
        {
            var proc = Machine.proclist[(int)Machine.PROCESS.ASSEMBLER] as ProcessAssembler;

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

        private void UpdateBarcodeData()
        {
            var proc = Machine.proclist[(int)Machine.PROCESS.ASSEMBLER] as ProcessAssembler;

            for (int i = 0; i < 2; i++)
            {
                if (!proc.OnReceivedBarcode)
                {
                    label_BarcodeData[i].Text = $"NONE";
                }
                else
                {
                    label_BarcodeData[i].Text = $"{proc.productID[i]}";
                }
            }
        }

        private void button_XYRReadyPos_Click(object sender, EventArgs e)
        {
            LogUtil.Instance.Log(LOG_TYPE.UI, "Assembler Move to Ready Pos Button Click", CONTENT_TYPE.INFO);
            if (Machine.status.state == SystemState.SystemStateERROR)
            {
                Dlg_MessageBox formErr = new Dlg_MessageBox(EMESSAGEBOX.MSG, string.Format("Please push the reset button"));
                formErr.TopLevel = true;
                formErr.TopMost = true;
                formErr.ShowDialog();
                return;
            }

            var proc = Machine.proclist[(int)Machine.PROCESS.ASSEMBLER];
            proc.SetMessage((int)ProcessAssembler.MSG.MSG_MOVE_READY_POS);
        }

        private void button_XYRLeftPickupPos_Click(object sender, EventArgs e)
        {
            LogUtil.Instance.Log(LOG_TYPE.UI, "Assembler Move to Pickup Left Pos Button Click", CONTENT_TYPE.INFO);
            if (Machine.status.state == SystemState.SystemStateERROR)
            {
                Dlg_MessageBox formErr = new Dlg_MessageBox(EMESSAGEBOX.MSG, string.Format("Please push the reset button"));
                formErr.TopLevel = true;
                formErr.TopMost = true;
                formErr.ShowDialog();
                return;
            }

            var proc = Machine.proclist[(int)Machine.PROCESS.ASSEMBLER] as ProcessAssembler;
            proc.targetCurr = 0;
            proc.SetMessage((int)ProcessAssembler.MSG.MSG_MOVE_PICKUP_POS);
        }

        private void button_XYRRightPickupPos_Click(object sender, EventArgs e)
        {
            LogUtil.Instance.Log(LOG_TYPE.UI, "Assembler Move to Pickup Right Pos Button Click", CONTENT_TYPE.INFO);
            if (Machine.status.state == SystemState.SystemStateERROR)
            {
                Dlg_MessageBox formErr = new Dlg_MessageBox(EMESSAGEBOX.MSG, string.Format("Please push the reset button"));
                formErr.TopLevel = true;
                formErr.TopMost = true;
                formErr.ShowDialog();
                return;
            }

            var proc = Machine.proclist[(int)Machine.PROCESS.ASSEMBLER] as ProcessAssembler;
            proc.targetCurr = 1;
            proc.SetMessage((int)ProcessAssembler.MSG.MSG_MOVE_PICKUP_POS);
        }

        private void button_XYRAlignPos_Click(object sender, EventArgs e)
        {
            LogUtil.Instance.Log(LOG_TYPE.UI, "Assembler Move to Align Left Pos Button Click", CONTENT_TYPE.INFO);
            if (Machine.status.state == SystemState.SystemStateERROR)
            {
                Dlg_MessageBox formErr = new Dlg_MessageBox(EMESSAGEBOX.MSG, string.Format("Please push the reset button"));
                formErr.TopLevel = true;
                formErr.TopMost = true;
                formErr.ShowDialog();
                return;
            }

            var proc = Machine.proclist[(int)Machine.PROCESS.ASSEMBLER];
            proc.SetMessage((int)ProcessAssembler.MSG.MSG_MOVE_ALIGN_POS);
        }

        private void button_XYRAlign2Pos_Click(object sender, EventArgs e)
        {
            LogUtil.Instance.Log(LOG_TYPE.UI, "Assembler Move to Align Right Pos Button Click", CONTENT_TYPE.INFO);
            if (Machine.status.state == SystemState.SystemStateERROR)
            {
                Dlg_MessageBox formErr = new Dlg_MessageBox(EMESSAGEBOX.MSG, string.Format("Please push the reset button"));
                formErr.TopLevel = true;
                formErr.TopMost = true;
                formErr.ShowDialog();
                return;
            }

            var proc = Machine.proclist[(int)Machine.PROCESS.ASSEMBLER];
            proc.SetMessage((int)ProcessAssembler.MSG.MSG_MOVE_ALIGN2_POS);
        }

        private void button_XYRLeftScanPos_Click(object sender, EventArgs e)
        {
            LogUtil.Instance.Log(LOG_TYPE.UI, "Assembler Move to Scan Left Pos Button Click", CONTENT_TYPE.INFO);
            if (Machine.status.state == SystemState.SystemStateERROR)
            {
                Dlg_MessageBox formErr = new Dlg_MessageBox(EMESSAGEBOX.MSG, string.Format("Please push the reset button"));
                formErr.TopLevel = true;
                formErr.TopMost = true;
                formErr.ShowDialog();
                return;
            }

            var proc = Machine.proclist[(int)Machine.PROCESS.ASSEMBLER];
            proc.SetMessage((int)ProcessAssembler.MSG.MSG_MOVE_SCAN_POS);
        }

        private void button_XYRRightScanPos_Click(object sender, EventArgs e)
        {
            LogUtil.Instance.Log(LOG_TYPE.UI, "Assembler Move to Align Right Pos Button Click", CONTENT_TYPE.INFO);
            if (Machine.status.state == SystemState.SystemStateERROR)
            {
                Dlg_MessageBox formErr = new Dlg_MessageBox(EMESSAGEBOX.MSG, string.Format("Please push the reset button"));
                formErr.TopLevel = true;
                formErr.TopMost = true;
                formErr.ShowDialog();
                return;
            }

            var proc = Machine.proclist[(int)Machine.PROCESS.ASSEMBLER];
            proc.SetMessage((int)ProcessAssembler.MSG.MSG_MOVE_SCAN2_POS);
        }

        private void button_XYRPlacePos_Click(object sender, EventArgs e)
        {
            LogUtil.Instance.Log(LOG_TYPE.UI, "Assembler Move to Place Left Pos Button Click", CONTENT_TYPE.INFO);
            if (Machine.status.state == SystemState.SystemStateERROR)
            {
                Dlg_MessageBox formErr = new Dlg_MessageBox(EMESSAGEBOX.MSG, string.Format("Please push the reset button"));
                formErr.TopLevel = true;
                formErr.TopMost = true;
                formErr.ShowDialog();
                return;
            }

            var proc = Machine.proclist[(int)Machine.PROCESS.ASSEMBLER];
            proc.SetMessage((int)ProcessAssembler.MSG.MSG_MOVE_PLACE_POS);
        }

        private void button_XYRPlace2Pos_Click(object sender, EventArgs e)
        {
            LogUtil.Instance.Log(LOG_TYPE.UI, "Assembler Move to Place Right Pos Button Click", CONTENT_TYPE.INFO);
            if (Machine.status.state == SystemState.SystemStateERROR)
            {
                Dlg_MessageBox formErr = new Dlg_MessageBox(EMESSAGEBOX.MSG, string.Format("Please push the reset button"));
                formErr.TopLevel = true;
                formErr.TopMost = true;
                formErr.ShowDialog();
                return;
            }

            var proc = Machine.proclist[(int)Machine.PROCESS.ASSEMBLER];
            proc.SetMessage((int)ProcessAssembler.MSG.MSG_MOVE_PLACE2_POS);
        }

        private void button_XYRNGBoxPos_Click(object sender, EventArgs e)
        {
            LogUtil.Instance.Log(LOG_TYPE.UI, "Assembler Move to NG Box Left Pos Button Click", CONTENT_TYPE.INFO);
            if (Machine.status.state == SystemState.SystemStateERROR)
            {
                Dlg_MessageBox formErr = new Dlg_MessageBox(EMESSAGEBOX.MSG, string.Format("Please push the reset button"));
                formErr.TopLevel = true;
                formErr.TopMost = true;
                formErr.ShowDialog();
                return;
            }

            var proc = Machine.proclist[(int)Machine.PROCESS.ASSEMBLER];
            proc.SetMessage((int)ProcessAssembler.MSG.MSG_MOVE_TRASH_POS);
        }

        private void button_XYRNGBox2Pos_Click(object sender, EventArgs e)
        {
            LogUtil.Instance.Log(LOG_TYPE.UI, "Assembler Move to NG Box Right Pos Button Click", CONTENT_TYPE.INFO);
            if (Machine.status.state == SystemState.SystemStateERROR)
            {
                Dlg_MessageBox formErr = new Dlg_MessageBox(EMESSAGEBOX.MSG, string.Format("Please push the reset button"));
                formErr.TopLevel = true;
                formErr.TopMost = true;
                formErr.ShowDialog();
                return;
            }

            var proc = Machine.proclist[(int)Machine.PROCESS.ASSEMBLER];
            proc.SetMessage((int)ProcessAssembler.MSG.MSG_MOVE_TRASH2_POS);
        }

        private void button_ZReadyPos_Click(object sender, EventArgs e)
        {
            LogUtil.Instance.Log(LOG_TYPE.UI, "Assembler Move to Ready (Z) Pos Button Click", CONTENT_TYPE.INFO);
            if (Machine.status.state == SystemState.SystemStateERROR)
            {
                Dlg_MessageBox formErr = new Dlg_MessageBox(EMESSAGEBOX.MSG, string.Format("Please push the reset button"));
                formErr.TopLevel = true;
                formErr.TopMost = true;
                formErr.ShowDialog();
                return;
            }

            var proc = Machine.proclist[(int)Machine.PROCESS.ASSEMBLER];
            proc.SetMessage((int)ProcessAssembler.MSG.MSG_SINGLE, (int)ProcessAssembler.STEP.MOVE_Z_READY_POS);
        }

        private void button_ZPickupPos_Click(object sender, EventArgs e)
        {
            LogUtil.Instance.Log(LOG_TYPE.UI, "Assembler Move to Pickup (Z) Pos Button Click", CONTENT_TYPE.INFO);
            if (Machine.status.state == SystemState.SystemStateERROR)
            {
                Dlg_MessageBox formErr = new Dlg_MessageBox(EMESSAGEBOX.MSG, string.Format("Please push the reset button"));
                formErr.TopLevel = true;
                formErr.TopMost = true;
                formErr.ShowDialog();
                return;
            }

            var proc = Machine.proclist[(int)Machine.PROCESS.ASSEMBLER];
            proc.SetMessage((int)ProcessAssembler.MSG.MSG_SINGLE, (int)ProcessAssembler.STEP.MOVE_Z_PICKUP_POS);
        }

        private void button_ZAlignPos_Click(object sender, EventArgs e)
        {
            LogUtil.Instance.Log(LOG_TYPE.UI, "Assembler Move to Align (Z) Pos Button Click", CONTENT_TYPE.INFO);
            if (Machine.status.state == SystemState.SystemStateERROR)
            {
                Dlg_MessageBox formErr = new Dlg_MessageBox(EMESSAGEBOX.MSG, string.Format("Please push the reset button"));
                formErr.TopLevel = true;
                formErr.TopMost = true;
                formErr.ShowDialog();
                return;
            }

            var proc = Machine.proclist[(int)Machine.PROCESS.ASSEMBLER];
            proc.SetMessage((int)ProcessAssembler.MSG.MSG_SINGLE, (int)ProcessAssembler.STEP.MOVE_Z_UNDER_ALIGN_POS);
        }

        private void button_ZScanPos_Click(object sender, EventArgs e)
        {
            LogUtil.Instance.Log(LOG_TYPE.UI, "Assembler Move to Scan (Z) Pos Button Click", CONTENT_TYPE.INFO);
            if (Machine.status.state == SystemState.SystemStateERROR)
            {
                Dlg_MessageBox formErr = new Dlg_MessageBox(EMESSAGEBOX.MSG, string.Format("Please push the reset button"));
                formErr.TopLevel = true;
                formErr.TopMost = true;
                formErr.ShowDialog();
                return;
            }

            var proc = Machine.proclist[(int)Machine.PROCESS.ASSEMBLER];
            proc.SetMessage((int)ProcessAssembler.MSG.MSG_SINGLE, (int)ProcessAssembler.STEP.MOVE_Z_SCAN_POS);
        }

        private void button_ZPlacePosL_Click(object sender, EventArgs e)
        {
            LogUtil.Instance.Log(LOG_TYPE.UI, "Assembler Move to Place (Z) Pos Button Click", CONTENT_TYPE.INFO);
            if (Machine.status.state == SystemState.SystemStateERROR)
            {
                Dlg_MessageBox formErr = new Dlg_MessageBox(EMESSAGEBOX.MSG, string.Format("Please push the reset button"));
                formErr.TopLevel = true;
                formErr.TopMost = true;
                formErr.ShowDialog();
                return;
            }

            var proc = Machine.proclist[(int)Machine.PROCESS.ASSEMBLER];
            proc.SetHeadTarget(0);
            proc.SetMessage((int)ProcessAssembler.MSG.MSG_SINGLE, (int)ProcessAssembler.STEP.MOVE_Z_PLACE_POS);
        }

        private void button_ZPlacePosR_Click(object sender, EventArgs e)
        {
            LogUtil.Instance.Log(LOG_TYPE.UI, "Assembler Move to Place (Z) Pos Button Click", CONTENT_TYPE.INFO);
            if (Machine.status.state == SystemState.SystemStateERROR)
            {
                Dlg_MessageBox formErr = new Dlg_MessageBox(EMESSAGEBOX.MSG, string.Format("Please push the reset button"));
                formErr.TopLevel = true;
                formErr.TopMost = true;
                formErr.ShowDialog();
                return;
            }

            var proc = Machine.proclist[(int)Machine.PROCESS.ASSEMBLER];
            proc.SetHeadTarget(1);
            proc.SetMessage((int)ProcessAssembler.MSG.MSG_SINGLE, (int)ProcessAssembler.STEP.MOVE_Z_PLACE_POS);
        }

        private void button_ZNGBoxPos_Click(object sender, EventArgs e)
        {
            LogUtil.Instance.Log(LOG_TYPE.UI, "Assembler Move to NG Box (Z) Pos Button Click", CONTENT_TYPE.INFO);
            if (Machine.status.state == SystemState.SystemStateERROR)
            {
                Dlg_MessageBox formErr = new Dlg_MessageBox(EMESSAGEBOX.MSG, string.Format("Please push the reset button"));
                formErr.TopLevel = true;
                formErr.TopMost = true;
                formErr.ShowDialog();
                return;
            }

            var proc = Machine.proclist[(int)Machine.PROCESS.ASSEMBLER];
            proc.SetMessage((int)ProcessAssembler.MSG.MSG_SINGLE, (int)ProcessAssembler.STEP.MOVE_Z_DISCARD_POS);
        }

        private void button_Tool1UpDown_Click(object sender, EventArgs e)
        {
            uint ret1 = 0;

            Machine.IO.GetOut((int)DO.ASSEMBLER_Z1_UP, ref ret1);

            if (ret1 == 0)
            {
                LogUtil.Instance.Log(LOG_TYPE.UI, "Assembler Left Picker Up Button Click", CONTENT_TYPE.INFO);

                Machine.IO.SetOut((int)DO.ASSEMBLER_Z1_UP, 1);
                Machine.IO.SetOut((int)DO.ASSEMBLER_Z1_DOWN, 0);
            }
            else if (ret1 == 1)
            {
                LogUtil.Instance.Log(LOG_TYPE.UI, "Assembler Left Picker Down Button Click", CONTENT_TYPE.INFO);

                Machine.IO.SetOut((int)DO.ASSEMBLER_Z1_UP, 0);
                Machine.IO.SetOut((int)DO.ASSEMBLER_Z1_DOWN, 1);
            }
        }

        private void button_Tool2UpDown_Click(object sender, EventArgs e)
        {
            uint ret1 = 0;

            Machine.IO.GetOut((int)DO.ASSEMBLER_Z2_UP, ref ret1);

            if (ret1 == 0)
            {
                LogUtil.Instance.Log(LOG_TYPE.UI, "Assembler Right Picker Up Button Click", CONTENT_TYPE.INFO);

                Machine.IO.SetOut((int)DO.ASSEMBLER_Z2_UP, 1);
                Machine.IO.SetOut((int)DO.ASSEMBLER_Z2_DOWN, 0);
            }
            else if (ret1 == 1)
            {
                LogUtil.Instance.Log(LOG_TYPE.UI, "Assembler Right Picker Down Button Click", CONTENT_TYPE.INFO);

                Machine.IO.SetOut((int)DO.ASSEMBLER_Z2_UP, 0);
                Machine.IO.SetOut((int)DO.ASSEMBLER_Z2_DOWN, 1);
            }
        }

        private void button_Tool1VacOnOff_Click(object sender, EventArgs e)
        {
            uint ret1 = 0;

            Machine.IO.GetOut((int)DO.ASSEMBLER_Z1_VACON, ref ret1);

            if (ret1 == 0)
            {
                LogUtil.Instance.Log(LOG_TYPE.UI, "Assembler Left Vacuum On Button Click", CONTENT_TYPE.INFO);

                Machine.IO.SetOut((int)DO.ASSEMBLER_Z1_VACON, 1);
                Machine.IO.SetOut((int)DO.ASSEMBLER_Z1_PURGE, 0);
            }
            else if (ret1 == 1)
            {
                LogUtil.Instance.Log(LOG_TYPE.UI, "Assembler Left Vacuum Off Button Click", CONTENT_TYPE.INFO);

                Machine.IO.SetOut((int)DO.ASSEMBLER_Z1_VACON, 0);
                Machine.IO.SetOut((int)DO.ASSEMBLER_Z1_PURGE, 1);
                Util.Delay((int)Machine.param.Time(ParameterDefine.TIME.PURGE_OFF_WAIT_TIME));
                Machine.IO.SetOut((int)DO.ASSEMBLER_Z1_PURGE, 0);
            }
        }

        private void button_Tool2VacOnOff_Click(object sender, EventArgs e)
        {
            uint ret1 = 0;

            Machine.IO.GetOut((int)DO.ASSEMBLER_Z2_VACON, ref ret1);

            if (ret1 == 0)
            {
                LogUtil.Instance.Log(LOG_TYPE.UI, "Assembler Right Vacuum On Button Click", CONTENT_TYPE.INFO);

                Machine.IO.SetOut((int)DO.ASSEMBLER_Z2_VACON, 1);
                Machine.IO.SetOut((int)DO.ASSEMBLER_Z2_PURGE, 0);
            }
            else if (ret1 == 1)
            {
                LogUtil.Instance.Log(LOG_TYPE.UI, "Assembler Right Vacuum Off Button Click", CONTENT_TYPE.INFO);

                Machine.IO.SetOut((int)DO.ASSEMBLER_Z2_VACON, 0);
                Machine.IO.SetOut((int)DO.ASSEMBLER_Z2_PURGE, 1);
                Util.Delay((int)Machine.param.Time(ParameterDefine.TIME.PURGE_OFF_WAIT_TIME));
                Machine.IO.SetOut((int)DO.ASSEMBLER_Z2_PURGE, 0);
            }
        }

        private void button_PickupLeft_Click(object sender, EventArgs e)
        {
            LogUtil.Instance.Log(LOG_TYPE.UI, "Assembler Pickup Left Process Button Click", CONTENT_TYPE.INFO);
            if (Machine.status.state == SystemState.SystemStateERROR)
            {
                Dlg_MessageBox formErr = new Dlg_MessageBox(EMESSAGEBOX.MSG, string.Format("Please push the reset button"));
                formErr.TopLevel = true;
                formErr.TopMost = true;
                formErr.ShowDialog();
                return;
            }
           
            var proc1 = Machine.proclist[(int)Machine.PROCESS.LEFTBUF];
            //proc1.SetMessage((int)ProcessBufStageOne.MSG.MSG_UNLOADING_PRODUCT);
            var proc2 = Machine.proclist[(int)Machine.PROCESS.ASSEMBLER] as ProcessAssembler;
            proc2.targetCurr = 0;
            proc2.SetMessage((int)ProcessAssembler.MSG.MSG_LOADING_PRODUCT);
        }

        private void button_PickupRight_Click(object sender, EventArgs e)
        {
            LogUtil.Instance.Log(LOG_TYPE.UI, "Assembler Pickup Right Process Button Click", CONTENT_TYPE.INFO);
            if (Machine.status.state == SystemState.SystemStateERROR)
            {
                Dlg_MessageBox formErr = new Dlg_MessageBox(EMESSAGEBOX.MSG, string.Format("Please push the reset button"));
                formErr.TopLevel = true;
                formErr.TopMost = true;
                formErr.ShowDialog();
                return;
            }
            
            var proc1 = Machine.proclist[(int)Machine.PROCESS.RIGHTBUF];
            //proc1.SetMessage((int)ProcessBufStageOne.MSG.MSG_UNLOADING_PRODUCT);
            var proc2 = Machine.proclist[(int)Machine.PROCESS.ASSEMBLER] as ProcessAssembler;
            proc2.targetCurr = 1;
            proc2.SetMessage((int)ProcessAssembler.MSG.MSG_LOADING_PRODUCT);
        }

        private void button_Align_Click(object sender, EventArgs e)
        {
            LogUtil.Instance.Log(LOG_TYPE.UI, "Assembler Align Left Process Button Click", CONTENT_TYPE.INFO);
            if (Machine.status.state == SystemState.SystemStateERROR)
            {
                Dlg_MessageBox formErr = new Dlg_MessageBox(EMESSAGEBOX.MSG, string.Format("Please push the reset button"));
                formErr.TopLevel = true;
                formErr.TopMost = true;
                formErr.ShowDialog();
                return;
            }

            var proc = Machine.proclist[(int)Machine.PROCESS.ASSEMBLER];
            proc.SetMessage((int)ProcessAssembler.MSG.MSG_ALIGN_PRODUCT);
        }

        private void button_Align2_Click(object sender, EventArgs e)
        {
            LogUtil.Instance.Log(LOG_TYPE.UI, "Assembler Align Right Process Button Click", CONTENT_TYPE.INFO);
            if (Machine.status.state == SystemState.SystemStateERROR)
            {
                Dlg_MessageBox formErr = new Dlg_MessageBox(EMESSAGEBOX.MSG, string.Format("Please push the reset button"));
                formErr.TopLevel = true;
                formErr.TopMost = true;
                formErr.ShowDialog();
                return;
            }

            var proc = Machine.proclist[(int)Machine.PROCESS.ASSEMBLER];
            proc.SetMessage((int)ProcessAssembler.MSG.MSG_ALIGN2_PRODUCT);
        }

        private void button_ScanLeft_Click(object sender, EventArgs e)
        {
            LogUtil.Instance.Log(LOG_TYPE.UI, "Assembler Scan Left Process Button Click", CONTENT_TYPE.INFO);
            if (Machine.status.state == SystemState.SystemStateERROR)
            {
                Dlg_MessageBox formErr = new Dlg_MessageBox(EMESSAGEBOX.MSG, string.Format("Please push the reset button"));
                formErr.TopLevel = true;
                formErr.TopMost = true;
                formErr.ShowDialog();
                return;
            }

            var proc = Machine.proclist[(int)Machine.PROCESS.PROD_LOADER];
            proc.SetHeadTarget(0);
            //proc.SetMessage((int)ProcessProdLoader.MSG.);
        }

        private void button_ScanRight_Click(object sender, EventArgs e)
        {
            LogUtil.Instance.Log(LOG_TYPE.UI, "Assembler Scan Right Process Button Click", CONTENT_TYPE.INFO);
            if (Machine.status.state == SystemState.SystemStateERROR)
            {
                Dlg_MessageBox formErr = new Dlg_MessageBox(EMESSAGEBOX.MSG, string.Format("Please push the reset button"));
                formErr.TopLevel = true;
                formErr.TopMost = true;
                formErr.ShowDialog();
                return;
            }

            var proc = Machine.proclist[(int)Machine.PROCESS.ASSEMBLER];
            proc.SetHeadTarget(1);
            //proc.SetMessage((int)ProcessAssembler.MSG.MSG_SCAN_BARCODE);
        }

        private void button_PlaceLeft_Click(object sender, EventArgs e)
        {
            LogUtil.Instance.Log(LOG_TYPE.UI, "Assembler Place Left Process Button Click", CONTENT_TYPE.INFO);
            if (Machine.status.state == SystemState.SystemStateERROR)
            {
                Dlg_MessageBox formErr = new Dlg_MessageBox(EMESSAGEBOX.MSG, string.Format("Please push the reset button"));
                formErr.TopLevel = true;
                formErr.TopMost = true;
                formErr.ShowDialog();
                return;
            }

            var proc1 = Machine.proclist[(int)Machine.PROCESS.JIGWORK];
            proc1.SetMessage((int)ProcessJigWork.MSG.MSG_PLACE_PRODUCT);
            var proc2 = Machine.proclist[(int)Machine.PROCESS.ASSEMBLER] as ProcessAssembler;
            proc2.targetCurr = 0;
            proc2.SetMessage((int)ProcessAssembler.MSG.MSG_UNLOADING_PRODUCT);
        }

        private void button_PlaceRight_Click(object sender, EventArgs e)
        {
            LogUtil.Instance.Log(LOG_TYPE.UI, "Assembler Place Right Process Button Click", CONTENT_TYPE.INFO);
            if (Machine.status.state == SystemState.SystemStateERROR)
            {
                Dlg_MessageBox formErr = new Dlg_MessageBox(EMESSAGEBOX.MSG, string.Format("Please push the reset button"));
                formErr.TopLevel = true;
                formErr.TopMost = true;
                formErr.ShowDialog();
                return;
            }

            var proc1 = Machine.proclist[(int)Machine.PROCESS.JIGWORK];
            proc1.SetMessage((int)ProcessJigWork.MSG.MSG_PLACE_PRODUCT);
            var proc2 = Machine.proclist[(int)Machine.PROCESS.ASSEMBLER] as ProcessAssembler;
            proc2.targetCurr = 1;
            proc2.SetMessage((int)ProcessAssembler.MSG.MSG_UNLOADING_PRODUCT);
        }

        private void button_Trash_Click(object sender, EventArgs e)
        {
            LogUtil.Instance.Log(LOG_TYPE.UI, "Assembler Trash Process Button Click", CONTENT_TYPE.INFO);
            if (Machine.status.state == SystemState.SystemStateERROR)
            {
                Dlg_MessageBox formErr = new Dlg_MessageBox(EMESSAGEBOX.MSG, string.Format("Please push the reset button"));
                formErr.TopLevel = true;
                formErr.TopMost = true;
                formErr.ShowDialog();
                return;
            }

            var proc = Machine.proclist[(int)Machine.PROCESS.ASSEMBLER] as ProcessAssembler;
            proc.targetCurr = 0;
            proc.SetMessage((int)ProcessAssembler.MSG.MSG_DISCARD_NG_PRODUCT);
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
            this.button_XYRReadyPos.MsgEvent = "Move to " + "Assembler " + "Ready Pos ?";

            this.button_ZReadyPos.ClickEvent += new System.EventHandler(this.button_ZReadyPos_Click);
            this.button_ZReadyPos.MsgEvent = "Move to " + "Assembler " + "Ready Pos (Z) ?";

            this.button_XYRAlignPos.ClickEvent += new System.EventHandler(this.button_XYRAlignPos_Click);
            this.button_XYRAlignPos.MsgEvent = "Move to " + "Assembler " + "Align Pos (L) ?";

            this.button_XYRAlign2Pos.ClickEvent += new System.EventHandler(this.button_XYRAlign2Pos_Click);
            this.button_XYRAlign2Pos.MsgEvent = "Move to " + "Assembler " + "Align Pos (R) ?";

            this.button_ZAlignPos.ClickEvent += new System.EventHandler(this.button_ZAlignPos_Click);
            this.button_ZAlignPos.MsgEvent = "Move to " + "Assembler " + "Align Pos (Z) ?";

            this.button_XYRScanLeftPos.ClickEvent += new System.EventHandler(this.button_XYRLeftScanPos_Click);
            this.button_XYRScanLeftPos.MsgEvent = "Move to " + "Assembler " + "Scan Pos (L) ?";

            this.button_XYRScanRightPos.ClickEvent += new System.EventHandler(this.button_XYRRightScanPos_Click);
            this.button_XYRScanRightPos.MsgEvent = "Move to " + "Assembler " + "Scan Pos (R) ?";

            this.button_ZScanPos.ClickEvent += new System.EventHandler(this.button_ZScanPos_Click);
            this.button_ZScanPos.MsgEvent = "Move to " + "Assembler " + "Scan Pos (Z) ?";

            this.button_XYRLeftPickupPos.ClickEvent += new System.EventHandler(this.button_XYRLeftPickupPos_Click);
            this.button_XYRLeftPickupPos.MsgEvent = "Move to " + "Assembler " + "Pickup Pos (L) ?";

            this.button_XYRRightPickupPos.ClickEvent += new System.EventHandler(this.button_XYRRightPickupPos_Click);
            this.button_XYRRightPickupPos.MsgEvent = "Move to " + "Assembler " + "Pickup Pos (R) ?";

            this.button_ZPickupPos.ClickEvent += new System.EventHandler(this.button_ZPickupPos_Click);
            this.button_ZPickupPos.MsgEvent = "Move to " + "Assembler " + "Pickup Pos (Z) ?";

            this.button_XYRPlacePos.ClickEvent += new System.EventHandler(this.button_XYRPlacePos_Click);
            this.button_XYRPlacePos.MsgEvent = "Move to " + "Assembler " + "Place Pos (L) ?";

            this.button_XYRPlace2Pos.ClickEvent += new System.EventHandler(this.button_XYRPlace2Pos_Click);
            this.button_XYRPlace2Pos.MsgEvent = "Move to " + "Assembler " + "Place Pos (R) ?";

            this.button_ZPlacePosL.ClickEvent += new System.EventHandler(this.button_ZPlacePosL_Click);
            this.button_ZPlacePosL.MsgEvent = "Move to " + "Assembler " + "Place Pos Z (L) ?";

            this.button_ZPlacePosR.ClickEvent += new System.EventHandler(this.button_ZPlacePosR_Click);
            this.button_ZPlacePosR.MsgEvent = "Move to " + "Assembler " + "Place Pos Z (R) ?";

            this.button_XYRNGBoxPos.ClickEvent += new System.EventHandler(this.button_XYRNGBoxPos_Click);
            this.button_XYRNGBoxPos.MsgEvent = "Move to " + "Assembler " + "NG Box Pos (L) ?";

            this.button_XYRNGBox2Pos.ClickEvent += new System.EventHandler(this.button_XYRNGBox2Pos_Click);
            this.button_XYRNGBox2Pos.MsgEvent = "Move to " + "Assembler " + "NG Box Pos (R) ?";

            this.button_ZNGBoxPos.ClickEvent += new System.EventHandler(this.button_ZNGBoxPos_Click);
            this.button_ZNGBoxPos.MsgEvent = "Move to " + "Assembler " + "NG Box Pos (Z) ?";

            this.button_Tool1UpDown.ClickEvent += new System.EventHandler(this.button_Tool1UpDown_Click);
            this.button_Tool1UpDown.MsgEvent = "Move to " + "Assembler " + "Left Up ?";
            this.button_Tool1UpDown.MsgEvent2 = "Move to " + "Assembler " + "Left Down ?";

            this.button_Tool1VacOnOff.ClickEvent += new System.EventHandler(this.button_Tool1VacOnOff_Click);
            this.button_Tool1VacOnOff.MsgEvent = "Use " + "Assembler " + "Left VacOn ?";
            this.button_Tool1VacOnOff.MsgEvent2 = "Use " + "Assembler " + "Left VacOff ?";

            this.button_Tool2UpDown.ClickEvent += new System.EventHandler(this.button_Tool2UpDown_Click);
            this.button_Tool2UpDown.MsgEvent = "Use " + "Assembler " + "Right Up ?";
            this.button_Tool2UpDown.MsgEvent2 = "Use " + "Assembler " + "Right Down ?";

            this.button_Tool2VacOnOff.ClickEvent += new System.EventHandler(this.button_Tool2VacOnOff_Click);
            this.button_Tool2VacOnOff.MsgEvent = "Use " + "Assembler " + "Right VacOn ?";
            this.button_Tool2VacOnOff.MsgEvent2 = "Use " + "Assembler " + "Right VacOff ?";

            this.button_PickupLeft.ClickEvent += new System.EventHandler(this.button_PickupLeft_Click);
            this.button_PickupLeft.MsgEvent = "Start " + "Assembler " + "PickupLeft Process ?";

            this.button_PickupRight.ClickEvent += new System.EventHandler(this.button_PickupRight_Click);
            this.button_PickupRight.MsgEvent = "Start " + "Assembler " + "PickupRight Process ?";

            this.button_AlignLeft.ClickEvent += new System.EventHandler(this.button_Align_Click);
            this.button_AlignLeft.MsgEvent = "Start " + "Assembler " + "AlignLeft Process ?";

            this.button_AlignRight.ClickEvent += new System.EventHandler(this.button_Align2_Click);
            this.button_AlignRight.MsgEvent = "Start " + "Assembler " + "AlignRight Process ?";

            this.button_ScanLeft.ClickEvent += new System.EventHandler(this.button_ScanLeft_Click);
            this.button_ScanLeft.MsgEvent = "Start " + "Assembler " + "AlignLeft Process ?";

            this.button_ScanRight.ClickEvent += new System.EventHandler(this.button_ScanRight_Click);
            this.button_ScanRight.MsgEvent = "Start " + "Assembler " + "AlignRight Process ?";

            this.button_PlaceLeft.ClickEvent += new System.EventHandler(this.button_PlaceLeft_Click);
            this.button_PlaceLeft.MsgEvent = "Start " + "Assembler " + "PlaceLeft Process ?";

            this.button_PlaceRight.ClickEvent += new System.EventHandler(this.button_PlaceRight_Click);
            this.button_PlaceRight.MsgEvent = "Start " + "Assembler " + "PlaceRight Process ?";

            this.button_Trash.ClickEvent += new System.EventHandler(this.button_Trash_Click);
            this.button_Trash.MsgEvent = "Start " + "Assembler " + "Trash Process ?";
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
