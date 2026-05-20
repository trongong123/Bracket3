using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static CAMASSEMBLYMACHINE.Define.SystemDefine;
using static CAMASSEMBLYMACHINE.Define.UIDefine;
using CAMASSEMBLYMACHINE.Process;
using TopEng.Controls;
using CAMASSEMBLYMACHINE.Define;
using TopEng.Utils;
using CAMASSEMBLYMACHINE.UI.SubForm;

namespace CAMASSEMBLYMACHINE.UI
{
    public partial class tabSummaryAuto : Form, IForm
    {
        Form_PaneAlignDisplay trayDisp;
        List<Label> label_tray_alignPos = new List<Label>();
        List<Label> label_cam_alignPos = new List<Label>();
        List<Label> label_jig_alignPos = new List<Label>();
        List<TopEng.Controls.ProcessPictureBox> left_Buffer_unitBox = new List<TopEng.Controls.ProcessPictureBox>();
        List<TopEng.Controls.ProcessPictureBox> right_Buffer_unitBox = new List<TopEng.Controls.ProcessPictureBox>();
        double leftbufferUsed= -1D;
        double rightbufferUsed = -1D;
        bool updatingNow = false;

        #region
        Image imgTrayExist = Properties.Resources.Front_Tray_Left_Image;
        Image imgCamExist = Properties.Resources.camera_selected;
        Image imgJigExist = Properties.Resources.Set_Horizontal;

        #endregion

        enum MONITOR_STATUS
        {
            TRAY_ERROR,
            TRAY_EMPTY,
            TRAY_MOVE,
            TRAY_EXIST,
            PARTS_EMPTY,
            PARTS_VACUUM_LOW,
            PARTS_EXIST,
            JIG_ERROR,
            JIG_EMPTY,
            JIG_MOVE,
            JIG_EXIST,
            DOOR_OPEN,
            DOOR_CLOSE,
            LIGHT_CURTAIN_DETECTED,
            LIGHT_CURTAIN_NOT_DETECTED
        }
        public enum UNIT_TYPE
        {
            TRAY,
            PARTS,
            JIG
        }
        public enum TRAY_DETECT
        {
            MID_REAR,
            FRONT_MID
        }

        public tabSummaryAuto()
        {
            InitializeComponent();

            // UNIT BOX Click Event
            unitBoxTrayOutBuf.ClickEvent += disp_ExistTray_Click;
            unitBoxTrayOut.ClickEvent += disp_ExistTray_Click;
            unitBoxTrayInBuf.ClickEvent += disp_ExistTray_Click;
            unitBoxTrayIn.ClickEvent += disp_ExistTray_Click;
            unitBoxTrayWork.ClickEvent += disp_ExistTray_Click;
            unitBoxTrayTRF.ClickEvent += disp_ExistTray_Click;

            unitBoxLoadPickup1.ClickEvent += disp_ExistParts_Click;
            unitBoxLoadPickup2.ClickEvent += disp_ExistParts_Click;
            unitBoxAssemPickup1.ClickEvent += disp_ExistParts_Click;
            unitBoxAssemPickup2.ClickEvent += disp_ExistParts_Click;
            unitBoxTransRight2.ClickEvent += disp_ExistParts_Click;
            unitBoxTransRight1.ClickEvent += disp_ExistParts_Click;
            unitBoxTransLeft2.ClickEvent += disp_ExistParts_Click;
            unitBoxTransLeft1.ClickEvent += disp_ExistParts_Click;
            unitBoxCamBuffer1L1.ClickEvent += disp_ExistParts_Click;
            unitBoxCamBuffer1L2.ClickEvent += disp_ExistParts_Click;
            unitBoxCamBuffer2L1.ClickEvent += disp_ExistParts_Click;
            unitBoxCamBuffer2L2.ClickEvent += disp_ExistParts_Click;
            unitBoxCamBuffer1R1.ClickEvent += disp_ExistParts_Click;
            unitBoxCamBuffer1R2.ClickEvent += disp_ExistParts_Click;
            unitBoxCamBuffer2R1.ClickEvent += disp_ExistParts_Click;
            unitBoxCamBuffer2R2.ClickEvent += disp_ExistParts_Click;
            unitBoxWorkJigParts1.ClickEvent += disp_ExistParts_Click;
            unitBoxWorkJigParts2.ClickEvent += disp_ExistParts_Click;

            unitBoxJigOutput.ClickEvent += disp_ExistJIG_Click;
            unitBoxJigCarrierOut.ClickEvent += disp_ExistJIG_Click;
            unitBoxJigCarrierIn.ClickEvent += disp_ExistJIG_Click;
            unitBoxJigInput.ClickEvent += disp_ExistJIG_Click;
            unitBoxJigWork.ClickEvent += disp_ExistJIG_Click;

            left_Buffer_unitBox.Add(unitBoxTransLeft1);
            left_Buffer_unitBox.Add(unitBoxTransLeft2);
            left_Buffer_unitBox.Add(unitBoxCamBuffer1L1);
            left_Buffer_unitBox.Add(unitBoxCamBuffer1L2);
            left_Buffer_unitBox.Add(unitBoxCamBuffer2L1);
            left_Buffer_unitBox.Add(unitBoxCamBuffer2L2);

            right_Buffer_unitBox.Add(unitBoxTransRight1);
            right_Buffer_unitBox.Add(unitBoxTransRight2);
            right_Buffer_unitBox.Add(unitBoxCamBuffer1R1);
            right_Buffer_unitBox.Add(unitBoxCamBuffer1R2);
            right_Buffer_unitBox.Add(unitBoxCamBuffer2R1);
            right_Buffer_unitBox.Add(unitBoxCamBuffer2R2);


            // UNIT BOX Init
            foreach (Control unit in this.Controls)
            {
                if( unit.GetType() == typeof(TopEng.Controls.ProcessPictureBox))
                {
                    (unit as TopEng.Controls.ProcessPictureBox).ColorStatus = Color.White;
                    (unit as TopEng.Controls.ProcessPictureBox).Image = null;
                }
            }

            label_tray_alignPos.Add(label_TrayAlignPosLeft);
            label_tray_alignPos.Add(label_TrayAlignPosRight);
            label_cam_alignPos.Add(label_CamAlignPosLeft);
            label_cam_alignPos.Add(label_CamAlignPosRight);
            label_jig_alignPos.Add(label_JigAlignPosLeft);
            label_jig_alignPos.Add(label_JigAlignPosRight);

            switch (SystemDefine.machineName.Split('-')[1])
            {
                case "WC":
                    unitBoxJigUpstreamExistCheckWork.Visible = false;
                    unitBoxJigDownstreamRunCheckCarrier.Visible = false;
                    unitBoxJigDownstreamRunCheckWork.TextName = "UWC IN";
                    unitBoxJigUpstreamExistCheckCarrier.TextName = "UWC OUT";
                    break;
                case "UWC":
                    unitBoxJigUpstreamExistCheckWork.TextName = "WC IN";
                    unitBoxJigDownstreamRunCheckCarrier.TextName = "WC OUT";
                    unitBoxJigDownstreamRunCheckWork.TextName = "3X IN";
                    unitBoxJigUpstreamExistCheckCarrier.TextName = "3X OUT";
                    break;
                case "TC3X":
                    unitBoxJigUpstreamExistCheckWork.TextName = "UWC IN";
                    unitBoxJigDownstreamRunCheckCarrier.TextName = "UWC OUT";
                    unitBoxJigDownstreamRunCheckWork.TextName = "5X IN";
                    unitBoxJigUpstreamExistCheckCarrier.TextName = "5X OUT";
                    break;
                case "TC5X":
                    unitBoxJigUpstreamExistCheckWork.TextName = "3X IN";
                    unitBoxJigDownstreamRunCheckCarrier.TextName = "3X OUT";
                    unitBoxJigDownstreamRunCheckWork.TextName = "TAPE IN";
                    unitBoxJigUpstreamExistCheckCarrier.TextName = "TAPE OUT";
                    break;
            }
       }
        private void UpdateStatus()
        {
            uint returnValue = 0;
            uint returnOut = 1;

            #region SAFETY
            Machine.IO.GetIn((int)DI.DOOR_FRONT_TOP1, ref returnValue);
            disp_DoorFrontTop1.ButtonPush = returnValue == 1 ? false : true;
            Machine.IO.GetIn((int)DI.DOOR_FRONT_TOP2, ref returnValue);
            disp_DoorFrontTop2.ButtonPush = returnValue == 1 ? false : true;
            Machine.IO.GetIn((int)DI.DOOR_REAR_TOP1, ref returnValue);
            disp_DoorRear1.ButtonPush = returnValue == 1 ? false : true;
            Machine.IO.GetIn((int)DI.DOOR_REAR_TOP2, ref returnValue);
            disp_DoorRear2.ButtonPush = returnValue == 1 ? false : true;
            
            Machine.IO.GetIn((int)DI.LIGHT_CURTAIN_IN, ref returnValue);
            disp_LightCurtainIn.ButtonPush = returnValue == 0 ? false : true;
            Machine.IO.GetIn((int)DI.LIGHT_CURTAIN_OUT, ref returnValue);
            disp_LightCurtainOut.ButtonPush = returnValue == 0 ? false : true;
            #endregion

            #region TRAY STATUS
            SetUnitColorStatus(unitBoxTrayIn, GetStatusConveyor(TRAY_TYPE.IN, DO.TRAY_IN_CONV_RUN, TRAY_DETECT.MID_REAR));
            SetUnitColorStatus(unitBoxTrayInBuf, GetStatusConveyor(TRAY_TYPE.IN_BUF, DO.TRAY_IN_BUF_CONV_RUN, TRAY_DETECT.MID_REAR));
            SetUnitColorStatus(unitBoxTrayWork, GetStatusConveyor(TRAY_TYPE.WORK, DO.TRAY_WORK_CONV_RUN, TRAY_DETECT.FRONT_MID));
            SetUnitColorStatus(unitBoxTrayOutBuf, GetStatusConveyor(TRAY_TYPE.OUT_BUF, DO.TRAY_OUT_BUF_CONV_RUN, TRAY_DETECT.MID_REAR));
            SetUnitColorStatus(unitBoxTrayOut, GetStatusConveyor(TRAY_TYPE.OUT, DO.TRAY_OUT_CONV_RUN, TRAY_DETECT.MID_REAR));

            SetUnitColorStatus(unitBoxTrayTRF, GetStatusTrayTRF(DI.TRAY_TRF_VACUUM, DO.TRAY_TRF_VACUUM));

            SetUnitImageExistent(unitBoxTrayIn, UNIT_TYPE.TRAY, Machine.Trays[(int)UNITTRAY.TRAY_IN].exist);
            SetUnitImageExistent(unitBoxTrayInBuf, UNIT_TYPE.TRAY, Machine.Trays[(int)UNITTRAY.TRAY_IN_BUF].exist);
            SetUnitImageExistent(unitBoxTrayWork, UNIT_TYPE.TRAY, Machine.Trays[(int)UNITTRAY.TRAY_WORK].exist);
            SetUnitImageExistent(unitBoxTrayOutBuf, UNIT_TYPE.TRAY, Machine.Trays[(int)UNITTRAY.TRAY_OUT_BUF].exist);
            SetUnitImageExistent(unitBoxTrayOut, UNIT_TYPE.TRAY, Machine.Trays[(int)UNITTRAY.TRAY_OUT].exist);
            SetUnitImageExistent(unitBoxTrayTRF, UNIT_TYPE.TRAY, Machine.Trays[(int)UNITTRAY.TRAY_TRF].exist);

            Machine.IO.GetIn((int)DI.TRAY_WORK_DETECT_UPPER_LIMIT, ref returnValue);
            disp_TrayWorkUpperLimit.ButtonPush = returnValue == 1 ? true : false;
            #endregion

            #region JIG STATUS
            SetUnitColorStatus(unitBoxJigInput, GetStatusJig(DI.JIG_IN_BUF_DETECT_IN, DO.JIG_IN_BUF_CONV_RUN));
            SetUnitColorStatus(unitBoxJigWork, GetStatusJig(DI.JIG_WORK_DETECT_IN, DO.JIG_WORK_CONV_RUN));
            SetUnitColorStatus(unitBoxJigOutput, GetStatusJig(DI.JIG_OUT_BUF_DETECT_IN, DO.JIG_OUT_BUF_CONV_RUN));
            SetUnitColorStatus(unitBoxJigCarrierIn, GetStatusJigCarrier(DI.JIG_IN_CARRIER_DETECT_IN, DO.JIG_IN_CARRIER_CONV_RUN));
            SetUnitColorStatus(unitBoxJigCarrierOut, GetStatusJigCarrier(DI.JIG_OUT_CARRIER_DETECT_IN, DO.JIG_OUT_CARRIER_CONV_RUN));

            SetUnitColorStatus(unitBoxJigUpstreamExistCheckWork, GetStatusJigIf(DI.IF_JIG_UPSTREAM_EXIST_CHECK_WORK));
            SetUnitColorStatus(unitBoxJigDownstreamRunCheckWork, GetStatusJigIf(DI.IF_JIG_DOWNSTREAM_RUN_CHECK_WORK));
            SetUnitColorStatus(unitBoxJigUpstreamExistCheckCarrier, GetStatusJigIf(DI.IF_JIG_UPSTREAM_EXIST_CHECK_CARRIER));
            SetUnitColorStatus(unitBoxJigDownstreamRunCheckCarrier, GetStatusJigIf(DI.IF_JIG_DOWNSTREAM_RUN_CHECK_CARRIER));

            SetUnitImageExistent(unitBoxJigInput, UNIT_TYPE.JIG, Machine.Jigs[(int)UNITJIG.JIG_IN_BUF].exist);
            SetUnitImageExistent(unitBoxJigWork, UNIT_TYPE.JIG, Machine.Jigs[(int)UNITJIG.JIG_WORK].exist);
            SetUnitImageExistent(unitBoxJigOutput, UNIT_TYPE.JIG, Machine.Jigs[(int)UNITJIG.JIG_OUT_BUF].exist);
            SetUnitImageExistent(unitBoxJigCarrierIn, UNIT_TYPE.JIG, Machine.Jigs[(int)UNITJIG.JIG_IN_CARRIER].exist);
            SetUnitImageExistent(unitBoxJigCarrierOut, UNIT_TYPE.JIG, Machine.Jigs[(int)UNITJIG.JIG_OUT_CARRIER].exist);

            SetUnitImageExistent(unitBoxJigUpstreamExistCheckWork, UNIT_TYPE.JIG, GetStatusJigIf(DI.IF_JIG_UPSTREAM_EXIST_CHECK_WORK) == MONITOR_STATUS.JIG_EXIST);
            SetUnitImageExistent(unitBoxJigDownstreamRunCheckWork, UNIT_TYPE.JIG, GetStatusJigIf(DI.IF_JIG_DOWNSTREAM_RUN_CHECK_WORK) == MONITOR_STATUS.JIG_EXIST);
            SetUnitImageExistent(unitBoxJigUpstreamExistCheckCarrier, UNIT_TYPE.JIG, GetStatusJigIf(DI.IF_JIG_UPSTREAM_EXIST_CHECK_CARRIER) == MONITOR_STATUS.JIG_EXIST);
            SetUnitImageExistent(unitBoxJigDownstreamRunCheckCarrier, UNIT_TYPE.JIG, GetStatusJigIf(DI.IF_JIG_DOWNSTREAM_RUN_CHECK_CARRIER) == MONITOR_STATUS.JIG_EXIST);
            #endregion

            #region PICKER
            SetUnitColorStatus(unitBoxLoadPickup1, GetStatusCamBuffer(DI.CAM_PICKER_Z1_VACON));
            SetUnitColorStatus(unitBoxLoadPickup2, GetStatusCamBuffer(DI.CAM_PICKER_Z2_VACON));
            SetUnitColorStatus(unitBoxAssemPickup1, GetStatusCamBuffer(DI.ASSEMBLER_Z1_VACON));
            SetUnitColorStatus(unitBoxAssemPickup2, GetStatusCamBuffer(DI.ASSEMBLER_Z2_VACON));

            SetUnitImageExistent(unitBoxLoadPickup1, UNIT_TYPE.PARTS, Machine.Parts[(int)UNITPART.PROD_PICK1].exist);
            SetUnitImageExistent(unitBoxLoadPickup2, UNIT_TYPE.PARTS, Machine.Parts[(int)UNITPART.PROD_PICK2].exist);
            SetUnitImageExistent(unitBoxAssemPickup1, UNIT_TYPE.PARTS, Machine.Parts[(int)UNITPART.ASSEMBLER1].exist);
            SetUnitImageExistent(unitBoxAssemPickup2, UNIT_TYPE.PARTS, Machine.Parts[(int)UNITPART.ASSEMBLER2].exist);
            SetUnitImageExistent(unitBoxWorkJigParts1, UNIT_TYPE.PARTS, Machine.Parts[(int)UNITPART.JIG1].exist);
            SetUnitImageExistent(unitBoxWorkJigParts2, UNIT_TYPE.PARTS, Machine.Parts[(int)UNITPART.JIG2].exist);
            #endregion

            #region BUFFER

            SetUnitImageExistent(unitBoxCamBuffer1L1, UNIT_TYPE.PARTS, Machine.Parts[(int)UNITPART.BUF1_L1].exist);
            SetUnitImageExistent(unitBoxCamBuffer2L1, UNIT_TYPE.PARTS, Machine.Parts[(int)UNITPART.BUF2_L1].exist);
            SetUnitImageExistent(unitBoxCamBuffer1L2, UNIT_TYPE.PARTS, Machine.Parts[(int)UNITPART.BUF1_L2].exist);
            SetUnitImageExistent(unitBoxCamBuffer2L2, UNIT_TYPE.PARTS, Machine.Parts[(int)UNITPART.BUF2_L2].exist);

            SetUnitImageExistent(unitBoxCamBuffer1R1, UNIT_TYPE.PARTS, Machine.Parts[(int)UNITPART.BUF1_R1].exist);
            SetUnitImageExistent(unitBoxCamBuffer2R1, UNIT_TYPE.PARTS, Machine.Parts[(int)UNITPART.BUF2_R1].exist);
            SetUnitImageExistent(unitBoxCamBuffer1R2, UNIT_TYPE.PARTS, Machine.Parts[(int)UNITPART.BUF1_R2].exist);
            SetUnitImageExistent(unitBoxCamBuffer2R2, UNIT_TYPE.PARTS, Machine.Parts[(int)UNITPART.BUF2_R2].exist);
            #endregion

            #region TRANSFER
            SetUnitColorStatus(unitBoxTransLeft1, GetStatusCamBuffer(DI.DETACH_SENSOR_1));
            SetUnitColorStatus(unitBoxTransLeft2, GetStatusCamBuffer(DI.DETACH_SENSOR_2));
            SetUnitColorStatus(unitBoxTransRight1, GetStatusCamBuffer(DI.DETACH_SENSOR_3));
            SetUnitColorStatus(unitBoxTransRight2, GetStatusCamBuffer(DI.DETACH_SENSOR_4));

            SetUnitImageExistent(unitBoxTransLeft1, UNIT_TYPE.PARTS, Machine.Parts[(int)UNITPART.CAM_TRF_L1].exist);
            SetUnitImageExistent(unitBoxTransLeft2, UNIT_TYPE.PARTS, Machine.Parts[(int)UNITPART.CAM_TRF_L2].exist);
            SetUnitImageExistent(unitBoxTransRight1, UNIT_TYPE.PARTS, Machine.Parts[(int)UNITPART.CAM_TRF_R1].exist);
            SetUnitImageExistent(unitBoxTransRight2, UNIT_TYPE.PARTS, Machine.Parts[(int)UNITPART.CAM_TRF_R2].exist);
            #endregion

            bool running = (Machine.status.state == SystemState.SystemStateRUN || Machine.status.state == SystemState.SystemStateRUNEMPTY || Machine.status.state == SystemState.SystemStateSLEEP);
            button_InBufferTrayOut.Enabled = !running;
            button_OutBufferTrayOut.Enabled = !running;

            UpdateNetworkStatus();
            UpdateTrayData();
        }

        private void timerStatusUpdate_Tick(object sender, EventArgs e)
        {
            if (updatingNow)
                return;
            updatingNow = true;
            UpdateProcTrayData();
            UpdateProcCamData();
            UpdateProcJigData();
            UpdateBufferUsed();

            label_TrayCount.Text = "Tray Count\n" + 
                Machine.info.Product(StatusDefine.PRODUCT.TRAY_PLACE_COUNT) + " / " +
                (int)Machine.recipe.Option(RecipeDefine.OPTION.UNLOADING_TRAYS_COUNTS);
            var proc1 = Machine.proclist[(int)Machine.PROCESS.ASSEMBLER] as ProcessAssembler;
            int maxPartIn_X_Left = (int)Machine.recipe.Option(RecipeDefine.OPTION.MAX_PART_IN_LEFT_NG_BOX_X);
            int maxPartIn_Y_Left = (int)Machine.recipe.Option(RecipeDefine.OPTION.MAX_PART_IN_LEFT_NG_BOX_Y);
            int maxPartIn_X_Right = (int)Machine.recipe.Option(RecipeDefine.OPTION.MAX_PART_IN_RIGHT_NG_BOX_X);
            int maxPartIn_Y_Right = (int)Machine.recipe.Option(RecipeDefine.OPTION.MAX_PART_IN_RIGHT_NG_BOX_Y);

            int maxCountLeft = maxPartIn_X_Left * maxPartIn_Y_Left;
            int maxCountRight = maxPartIn_X_Right * maxPartIn_Y_Right;

            lblNGBoxStatusLeft.Text = $"{proc1.currentNGCount[(int)TOOL_TYPE.LEFT]} - {maxCountLeft}";
            lblNGBoxStatusRight.Text = $"{proc1.currentNGCount[(int)TOOL_TYPE.RIGHT]} - {maxCountRight}";
            updatingNow = false;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            UpdateStatus();
        }

        public void StartTimer(bool enable)
        {
            timerStatusUpdate.Enabled = enable;
            timer1.Enabled = enable;
        }

        private void UpdateBufferUsed()
        {
            double used;
            used = Machine.param.Option(ParameterDefine.OPTION.USE_LEFT_BUFFER);
            if(leftbufferUsed != used)
            {
                leftbufferUsed = used;
                foreach (var a in left_Buffer_unitBox)
                    a.Used = used == 1 ? true : false;
            }

            used = Machine.param.Option(ParameterDefine.OPTION.USE_RIGHT_BUFFER);
            if (rightbufferUsed != used)
            {
                rightbufferUsed = used;
                foreach (var a in right_Buffer_unitBox)
                    a.Used = used == 1 ? true : false;
            }
        }
        private void UpdateProcTrayData()
        {
            var proc = Machine.proclist[(int)Machine.PROCESS.TRAYWORK] as ProcessTrayWork;

            for (int i = 0; i < 2; i++)
            {
                if (proc.alignSucs[i] < 0)
                {
                    label_tray_alignPos[i].Text = $"0.000, 0.000, 0.000";
                }
                else
                {
                    string xpos = proc.productPosition[i].x.ToString("0.000");
                    string ypos = proc.productPosition[i].y.ToString("0.000");
                    string angle = proc.productAngle[i].ToString("0.000");
                    label_tray_alignPos[i].Text = $"{xpos}, {ypos}, {angle}";
                }
            }
        }

        private void UpdateProcCamData()
        {
            var proc = Machine.proclist[(int)Machine.PROCESS.ASSEMBLER] as ProcessAssembler;

            for (int i = 0; i < 2; i++)
            {
                if (proc.alignSucs[i] < 0)
                {
                    label_cam_alignPos[i].Text = $"0.000, 0.000, 0.000";
                }
                else
                {
                    string xpos = proc.productPosition[i].x.ToString("0.000");
                    string ypos = proc.productPosition[i].y.ToString("0.000");
                    string angle = proc.productAngle[i].ToString("0.000");
                    label_cam_alignPos[i].Text = $"{xpos}, {ypos}, {angle}";
                }
            }
        }

        private void UpdateProcJigData()
        {
            var proc = Machine.proclist[(int)Machine.PROCESS.JIGWORK] as ProcessJigWork;

            for (int i = 0; i < 2; i++)
            {
                if (proc.alignSucs[i] < 0)
                {
                    label_jig_alignPos[i].Text = $"0.000, 0.000, 0.000";
                }
                else
                {
                    string xpos = proc.productPosition[i].x.ToString("0.000");
                    string ypos = proc.productPosition[i].y.ToString("0.000");
                    string angle = proc.productAngle[i].ToString("0.000");
                    label_jig_alignPos[i].Text = $"{xpos}, {ypos}, {angle}";
                }
            }
        }

        private void disp_ExistJIG_Click(object sender, EventArgs e)
        {
            if (!(sender as TopEng.Controls.ProcessPictureBox).Used) return;
            string sTag = (sender as Control).Tag.ToString();
            if (Enum.TryParse(sTag, out UNITJIG selectUnit))
            {
                ManageData(Machine.Jigs[(int)selectUnit]);
            }     
        }
        private void disp_ExistParts_Click(object sender, EventArgs e)
        {
            if (!(sender as TopEng.Controls.ProcessPictureBox).Used) return;
            string sTag = (sender as Control).Tag.ToString();
            if (Enum.TryParse(sTag, out UNITPART selectUnit))
            {
                if (selectUnit == UNITPART.PROD_PICK1 || selectUnit == UNITPART.PROD_PICK2)
                    ManageDataUnit(Machine.Parts[(int)UNITPART.PROD_PICK1], Machine.Parts[(int)UNITPART.PROD_PICK2], "PROD_PICK");

                else if (selectUnit == UNITPART.CAM_TRF_L1 || selectUnit == UNITPART.CAM_TRF_L2)
                    ManageDataUnit(Machine.Parts[(int)UNITPART.CAM_TRF_L1], Machine.Parts[(int)UNITPART.CAM_TRF_L2], "TRF_LEFT");

                else if (selectUnit == UNITPART.CAM_TRF_R1 || selectUnit == UNITPART.CAM_TRF_R2)
                    ManageDataUnit(Machine.Parts[(int)UNITPART.CAM_TRF_R1], Machine.Parts[(int)UNITPART.CAM_TRF_R2], "TRF_RIGHT");

                else if (selectUnit == UNITPART.BUF1_L1 || selectUnit == UNITPART.BUF1_L2)
                    ManageDataUnit(Machine.Parts[(int)UNITPART.BUF1_L1], Machine.Parts[(int)UNITPART.BUF1_L2], "BUF1_L");

                else if (selectUnit == UNITPART.BUF2_L1 || selectUnit == UNITPART.BUF2_L2)
                    ManageDataUnit(Machine.Parts[(int)UNITPART.BUF2_L1], Machine.Parts[(int)UNITPART.BUF2_L2], "BUF2_L");

                else if (selectUnit == UNITPART.BUF1_R1 || selectUnit == UNITPART.BUF1_R2)
                    ManageDataUnit(Machine.Parts[(int)UNITPART.BUF1_R1], Machine.Parts[(int)UNITPART.BUF1_R2], "BUF1_R");

                else if (selectUnit == UNITPART.BUF2_R1 || selectUnit == UNITPART.BUF2_R2)
                    ManageDataUnit(Machine.Parts[(int)UNITPART.BUF2_R1], Machine.Parts[(int)UNITPART.BUF2_R2], "BUF2_R");

                else if (selectUnit == UNITPART.ASSEMBLER1 || selectUnit == UNITPART.ASSEMBLER2)
                    ManageDataUnit(Machine.Parts[(int)UNITPART.ASSEMBLER1], Machine.Parts[(int)UNITPART.ASSEMBLER2], "Assembler");

                else if (selectUnit == UNITPART.JIG1 || selectUnit == UNITPART.JIG2)
                    ManageDataUnit(Machine.Parts[(int)UNITPART.JIG1], Machine.Parts[(int)UNITPART.JIG2], "WorkJig");
            }
        }
        private void disp_ExistTray_Click(object sender, EventArgs e)
        {
            if (!(sender as TopEng.Controls.ProcessPictureBox).Used) return;
            string sTag = (sender as Control).Tag.ToString();
            if (Enum.TryParse(sTag, out UNITTRAY selectUnit))
            {
                ManageData(Machine.Trays[(int)selectUnit]);
            }
        }
        private void ManageData(ProductData data)
        {
            if (Machine.status.state == SystemState.SystemStateRUN
                || Machine.status.state == SystemState.SystemStateRUNEMPTY
                || Machine.status.state == SystemState.SystemStateSLEEP) return;

            Form_InitState form = new Form_InitState();
            DialogResult dr = form.Show();
            if (DialogResult.OK == dr)
            {
                LogUtil.Instance.Log(LOG_TYPE.UI, "Tray or Jig Data is Changed", CONTENT_TYPE.INFO);
                data.exist = true;
            }
            else if (DialogResult.No == dr)
            {
                LogUtil.Instance.Log(LOG_TYPE.UI, "Tray or Jig Data is Changed", CONTENT_TYPE.INFO);
                data.exist = false;
            }
        }
        private void ManageDataUnit(ProductData dataLeft, ProductData dataRight, string dataName)
        {
            if (Machine.status.state == SystemState.SystemStateRUN
                || Machine.status.state == SystemState.SystemStateRUNEMPTY
                || Machine.status.state == SystemState.SystemStateSLEEP) return;

            Form_InitState2 form = new Form_InitState2(dataName, dataLeft.exist, dataRight.exist);
            DialogResult dr = form.Show();
            if (DialogResult.OK == dr)
            {
                dataLeft.exist = form.isExistLeft;
                dataRight.exist = form.isExistRight;
                LogUtil.Instance.Log(LOG_TYPE.UI, dataName + " Data " + "is Changed", CONTENT_TYPE.INFO);
            }
        }

        private Color GetStatusColor(MONITOR_STATUS status)
        {
            Color bgColor = Color.White;
            switch (status)
            {
                case MONITOR_STATUS.TRAY_EMPTY: bgColor = Color.White; break;
                case MONITOR_STATUS.TRAY_ERROR: bgColor = Color.DimGray; break;
                case MONITOR_STATUS.TRAY_MOVE: bgColor = Color.Yellow; break;
                case MONITOR_STATUS.TRAY_EXIST: bgColor = Color.Lime; break;
                case MONITOR_STATUS.PARTS_EMPTY: bgColor = Color.White; break;
                case MONITOR_STATUS.PARTS_VACUUM_LOW: bgColor = Color.Yellow; break;
                case MONITOR_STATUS.PARTS_EXIST: bgColor = Color.Lime; break;
                case MONITOR_STATUS.JIG_EMPTY: bgColor = Color.White; break;
                case MONITOR_STATUS.JIG_EXIST: bgColor = Color.Lime; break;
                case MONITOR_STATUS.JIG_MOVE: bgColor = Color.Yellow; break;
                case MONITOR_STATUS.JIG_ERROR: bgColor = Color.DimGray; break;
                case MONITOR_STATUS.DOOR_CLOSE: bgColor = Color.White; break;
                case MONITOR_STATUS.DOOR_OPEN: bgColor = Color.DimGray; break;
                case MONITOR_STATUS.LIGHT_CURTAIN_NOT_DETECTED: bgColor = Color.White; break;
                case MONITOR_STATUS.LIGHT_CURTAIN_DETECTED: bgColor = Color.DimGray; break;
            }
            return bgColor;
        }
        private void SetUnitColorStatus(ProcessPictureBox ctrl, MONITOR_STATUS status)
        {
            if (ctrl.GetType() == typeof(ProcessPictureBox))
            {
                Color statusColor = GetStatusColor(status);
                if( (ctrl as ProcessPictureBox).ColorStatus != statusColor)
                {
                    (ctrl as ProcessPictureBox).ColorStatus = statusColor;
                }
            }
        }

        private MONITOR_STATUS GetStatusJig(DI inputSensor, DO outputConv)
        {
            uint returnValue1 = 0, returnValue2 = 0;
            Machine.IO.GetIn((int)inputSensor, ref returnValue1);
            Machine.IO.GetIn((int)inputSensor + 1, ref returnValue2);

            uint returnOutValue = 0;
            Machine.IO.GetOut((int)outputConv, ref returnOutValue);
            if (returnValue1 == 1 && returnValue2 == 1)
                return returnOutValue == 1 ? MONITOR_STATUS.JIG_MOVE : MONITOR_STATUS.JIG_EXIST;
            else if (returnValue1 == 0 && returnValue2 == 0)
                return returnOutValue == 1 ? MONITOR_STATUS.JIG_MOVE : MONITOR_STATUS.JIG_EMPTY;
            else if (returnValue1 == 1 && returnValue2 == 0)
                return returnOutValue == 1 ? MONITOR_STATUS.JIG_MOVE : MONITOR_STATUS.JIG_ERROR;
            else if (returnValue1 == 0 && returnValue2 == 1)
                return returnOutValue == 1 ? MONITOR_STATUS.JIG_MOVE : MONITOR_STATUS.JIG_ERROR;
            else
                return MONITOR_STATUS.JIG_ERROR;
        }

        private MONITOR_STATUS GetStatusJigCarrier(DI inputSensor, DO outputConv)
        {
            uint returnValue1 = 0, returnValue2 = 0;
            Machine.IO.GetIn((int)inputSensor, ref returnValue1);
            Machine.IO.GetIn((int)inputSensor + 1, ref returnValue2);

            uint returnOutValue = 0;
            Machine.IO.GetOut((int)outputConv, ref returnOutValue);
            if (returnValue1 == 0 && returnValue2 == 0)
                return returnOutValue == 1 ? MONITOR_STATUS.JIG_MOVE : MONITOR_STATUS.JIG_EMPTY;
            else if (returnValue1 == 1 && returnValue2 == 0)
                return returnOutValue == 1 ? MONITOR_STATUS.JIG_MOVE : MONITOR_STATUS.JIG_ERROR;
            else if (returnValue1 == 0 && returnValue2 == 1)
                return returnOutValue == 1 ? MONITOR_STATUS.JIG_MOVE : MONITOR_STATUS.JIG_EXIST;
            else
                return MONITOR_STATUS.JIG_ERROR;
        }

        private MONITOR_STATUS GetStatusJigIf(DI input)
        {
            uint returnValue1 = 0;
            Machine.IO.GetIn((int)input, ref returnValue1);
            return  returnValue1 == 1 ? MONITOR_STATUS.JIG_EXIST : MONITOR_STATUS.JIG_EMPTY;
        }

        private MONITOR_STATUS GetStatusCamBuffer(DI input)
        {
            uint returnValue1 = 0, returnOut1 = 0;


            if (input != DI.IOMAX)
                Machine.IO.GetIn((int)input, ref returnValue1);



            if ((returnValue1 == 1) && (returnOut1 == 1))
                return MONITOR_STATUS.PARTS_EXIST;
            else if ((returnValue1 == 0) && (returnOut1 == 1))
                return MONITOR_STATUS.PARTS_VACUUM_LOW;
            else
                return MONITOR_STATUS.PARTS_EMPTY;
        }

        private MONITOR_STATUS GetStatusTrayTRF(DI input, DO output)
        {
            uint returnValue1 = 0, returnOut1 = 0;

            if (output != DO.IOMAX)
                Machine.IO.GetOut((int)output, ref returnOut1);
            if (input != DI.IOMAX)
                Machine.IO.GetIn((int)input, ref returnValue1);

            if (output == DO.IOMAX && returnValue1 == 1)
                returnOut1 = 1;

            if ((returnValue1 == 1) && (returnOut1 == 1))
                return MONITOR_STATUS.TRAY_EXIST;
            else if ((returnValue1 == 0) && (returnOut1 == 1))
                return MONITOR_STATUS.TRAY_MOVE;
            else
                return MONITOR_STATUS.TRAY_EMPTY;
        }

        private MONITOR_STATUS GetStatusConveyor(DI inputSensor, DO outputConv, TRAY_DETECT direction)
        {
            uint returnValue1 = 0, returnValue2 = 0, returnValue3 = 0;

            Machine.IO.GetIn((int)inputSensor, ref returnValue1);
            Machine.IO.GetIn((int)inputSensor + 1, ref returnValue2);
            Machine.IO.GetIn((int)inputSensor + 2, ref returnValue3);

            uint returnOutValue = 0;
            Machine.IO.GetOut((int)outputConv, ref returnOutValue);

            if (direction == TRAY_DETECT.MID_REAR)
            {
                if (returnValue1 == 1 || returnValue2 == 1 || returnValue3 == 1)
                    return returnOutValue == 1 ? MONITOR_STATUS.TRAY_MOVE : MONITOR_STATUS.TRAY_EXIST;
                else if (returnValue1 == 0 && returnValue2 == 0 && returnValue3 == 0)
                    return MONITOR_STATUS.TRAY_EMPTY;
                else
                    return MONITOR_STATUS.TRAY_ERROR;
            }
            else // TRAY_DETECT.FRONT_MID
            {
                if (returnValue1 == 1 || returnValue2 == 1 || returnValue3 == 1)
                    return returnOutValue == 1 ? MONITOR_STATUS.TRAY_MOVE : MONITOR_STATUS.TRAY_EXIST;
                else if (returnValue1 == 0 && returnValue2 == 0 && returnValue3 == 0)
                    return MONITOR_STATUS.TRAY_EMPTY;
                else
                    return MONITOR_STATUS.TRAY_ERROR;
            }
        }

        private MONITOR_STATUS GetStatusConveyor(TRAY_TYPE trayType, DO outputConv, TRAY_DETECT direction)
        {
            uint returnValue1 = 0, returnValue2 = 0, returnValue3 = 0;

            returnValue1 = Convert.ToUInt16(Machine.GetTraySensorSignal(trayType, TRAY_CV_SENSOR.IN));
            returnValue2 = Convert.ToUInt16(Machine.GetTraySensorSignal(trayType, TRAY_CV_SENSOR.MID));
            returnValue3 = Convert.ToUInt16(Machine.GetTraySensorSignal(trayType, TRAY_CV_SENSOR.END));

            uint returnOutValue = 0;
            Machine.IO.GetOut((int)outputConv, ref returnOutValue);

            if (direction == TRAY_DETECT.MID_REAR)
            {
                if (returnValue1 == 1 || returnValue2 == 1 || returnValue3 == 1)
                    return returnOutValue == 1 ? MONITOR_STATUS.TRAY_MOVE : MONITOR_STATUS.TRAY_EXIST;
                else if (returnValue1 == 0 && returnValue2 == 0 && returnValue3 == 0)
                    return MONITOR_STATUS.TRAY_EMPTY;
                else
                    return MONITOR_STATUS.TRAY_ERROR;
            }
            else // TRAY_DETECT.FRONT_MID
            {
                if (returnValue1 == 1 || returnValue2 == 1 || returnValue3 == 1)
                    return returnOutValue == 1 ? MONITOR_STATUS.TRAY_MOVE : MONITOR_STATUS.TRAY_EXIST;
                else if (returnValue1 == 0 && returnValue2 == 0 && returnValue3 == 0)
                    return MONITOR_STATUS.TRAY_EMPTY;
                else
                    return MONITOR_STATUS.TRAY_ERROR;
            }
        }

        private void SetUnitImageExistent(TopEng.Controls.ProcessPictureBox ctrl, UNIT_TYPE unit,  bool existData)
        {
            switch(unit)
            {
                case UNIT_TYPE.TRAY:
                    if (existData && ctrl.Image != imgTrayExist)
                        ctrl.Image = imgTrayExist;
                    else if (!existData && ctrl.Image != null)
                        ctrl.Image = null;
                    //else
                    //    ctrl.Image = null;
                    break;
                case UNIT_TYPE.PARTS:
                    if (existData && ctrl.Image != imgCamExist)
                        ctrl.Image = imgCamExist;
                    else if (!existData && ctrl.Image != null)
                        ctrl.Image = null;
                    //else
                    //    ctrl.Image = null;
                    break;
                case UNIT_TYPE.JIG:
                    if (existData && ctrl.Image != imgJigExist)
                        ctrl.Image = imgJigExist;
                    else if (!existData && ctrl.Image != null)
                        ctrl.Image = null;
                    //else
                    //    ctrl.Image = null;
                    break;
            }
        }

        private void button_towerlampOff_Click(object sender, EventArgs e)
        {
            if (Machine.Busy())
                return;

            var proc = Machine.proclist[(int)Machine.PROCESS.SYS] as ProcessSys;
            proc.towerLampOff = true;
        }

        private void button_OutBufferTrayOut_Click(object sender, EventArgs e)
        {
            LogUtil.Instance.Log(LOG_TYPE.UI, "Tray Out Buffer Tray Out Button Click", CONTENT_TYPE.INFO);

            if (Machine.Busy())
            {
                Dlg_MessageBox form = new Dlg_MessageBox(EMESSAGEBOX.MSG, string.Format("Please only Use in Stop State"));
                form.ShowDialog();
            }
            else
            {
                Dlg_MessageBox msgBoxW2 = new Dlg_MessageBox(EMESSAGEBOX.WARNING,
                                     string.Format("Do you want to Out Buffer tray out ?"));
                if (msgBoxW2.ShowDialog() == DialogResult.Yes)
                {
                    uint ret1 = 0;
                    uint ret2 = 0;
                    uint ret3 = 0;

                    ret1 = Convert.ToUInt16(Machine.GetTraySensorSignal(TRAY_TYPE.OUT_BUF, TRAY_CV_SENSOR.IN));
                    ret2 = Convert.ToUInt16(Machine.GetTraySensorSignal(TRAY_TYPE.OUT_BUF, TRAY_CV_SENSOR.MID));
                    ret3 = Convert.ToUInt16(Machine.GetTraySensorSignal(TRAY_TYPE.OUT_BUF, TRAY_CV_SENSOR.END));

                    if (ret1 == 0 && ret2 == 0 && ret3 == 0)
                    {
                        Dlg_MessageBox formT = new Dlg_MessageBox(EMESSAGEBOX.MSG, string.Format("No Tray In Out Buffer"));
                        formT.ShowDialog();
                        return;
                    }

                    var proc1 = Machine.proclist[(int)Machine.PROCESS.TRAYOUTBUF];
                    proc1.SetMessage((int)ProcessTrayOutBuf.MSG.MSG_UNLOADING_PRODUCT);
                    var proc2 = Machine.proclist[(int)Machine.PROCESS.TRAYOUT];
                    proc2.SetMessage((int)ProcessTrayOut.MSG.MSG_LOADING_PRODUCT);
                }
            }
        }

        private void button_InBufferTrayOut_Click(object sender, EventArgs e)
        {
            LogUtil.Instance.Log(LOG_TYPE.UI, "Tray In Buffer Tray Out Button Click", CONTENT_TYPE.INFO);

            if (Machine.Busy())
            {
                Dlg_MessageBox form = new Dlg_MessageBox(EMESSAGEBOX.MSG, string.Format("Please only Use in Stop State"));
                form.ShowDialog();
            }
            else
            {
                Dlg_MessageBox msgBoxW2 = new Dlg_MessageBox(EMESSAGEBOX.WARNING,
                                     string.Format("Do you want to In Buffer tray out ?"));
                if (msgBoxW2.ShowDialog() == DialogResult.Yes)
                {
                    uint ret1 = 0;
                    uint ret2 = 0;
                    uint ret3 = 0;

                    ret1 = Convert.ToUInt16(Machine.GetTraySensorSignal(TRAY_TYPE.IN_BUF, TRAY_CV_SENSOR.IN));
                    ret2 = Convert.ToUInt16(Machine.GetTraySensorSignal(TRAY_TYPE.IN_BUF, TRAY_CV_SENSOR.MID));
                    ret3 = Convert.ToUInt16(Machine.GetTraySensorSignal(TRAY_TYPE.IN_BUF, TRAY_CV_SENSOR.END));

                    if (ret1 == 0 && ret2 == 0 && ret3 == 0)
                    {
                        Dlg_MessageBox formT = new Dlg_MessageBox(EMESSAGEBOX.MSG, string.Format("No Tray In In Buffer"));
                        formT.ShowDialog();
                        return;
                    }
                   
                    var proc1 = Machine.proclist[(int)Machine.PROCESS.TRAYINBUF];
                    proc1.SetMessage((int)ProcessTrayInBuf.MSG.MSG_REVERSE_UNLOADING_PRODUCT);
                    var proc2 = Machine.proclist[(int)Machine.PROCESS.TRAYIN];
                    proc2.SetMessage((int)ProcessTrayIn.MSG.MSG_REVERSE_UNLOADING_PRODUCT);
                }
            }
        }

        void UpdateNetworkStatus()
        {
            var procEx = Machine.proclist[(int)Machine.PROCESS.EXTERNALEQUIP] as ProcessExternalEquip;
            disp_ConnectStatus1.ButtonPush = procEx.GetConnected();
        }

        void UpdateTrayData()
        {
            try
            {
                var procTrayWork = Machine.proclist[(int)Machine.PROCESS.TRAYWORK] as ProcessTrayWork;
                int pickedProduct = (Machine.trayNGListX.Count + Machine.trayNGListY.Count) / 2;
                lblProductCount.Text = $"{pickedProduct}/{procTrayWork.currentProductCount}";
                lblProductDirection.Text = $"{procTrayWork.trayAngle}";
                if(Machine.param.Option(ParameterDefine.OPTION.USE_TRAY_PRODUCT_COUNT_ALARM) == 1)
                {
                    lblProductCount.ForeColor = Color.Green;
                    lblProductDirection.ForeColor = Color.Green;
                }
                else
                {
                    lblProductCount.ForeColor = Color.Black;
                    lblProductDirection.ForeColor = Color.Black;
                }
            }
            catch(Exception ex) { }
        }

        private void button_GetPOInfo_Click(object sender, EventArgs e)
        {
            Dlg_MessageBox msgBoxW2 = new Dlg_MessageBox(EMESSAGEBOX.WARNING,
                                        string.Format("Do you want to Check PO ?"));
            if (msgBoxW2.ShowDialog() == DialogResult.Yes)
            {
                var proc1 = Machine.proclist[(int)Machine.PROCESS.EXTERNALEQUIP];
                proc1.SetMessage((int)ProcessExternalEquip.MSG.MSG_CHECK_PO);
            }
        }

        private void btnClearNGBox_Click(object sender, EventArgs e)
        {
            Dlg_MessageBox msgBoxW2 = new Dlg_MessageBox(EMESSAGEBOX.WARNING,
                                        string.Format("Do you want to Clear NG box ?"));
            if (msgBoxW2.ShowDialog() == DialogResult.Yes)
            {
                var proc1 = Machine.proclist[(int)Machine.PROCESS.ASSEMBLER] as ProcessAssembler;
                int maxPartIn_X_Left = (int)Machine.recipe.Option(RecipeDefine.OPTION.MAX_PART_IN_LEFT_NG_BOX_X);
                int maxPartIn_Y_Left = (int)Machine.recipe.Option(RecipeDefine.OPTION.MAX_PART_IN_LEFT_NG_BOX_Y);
                int maxPartIn_X_Right = (int)Machine.recipe.Option(RecipeDefine.OPTION.MAX_PART_IN_RIGHT_NG_BOX_X);
                int maxPartIn_Y_Right = (int)Machine.recipe.Option(RecipeDefine.OPTION.MAX_PART_IN_RIGHT_NG_BOX_Y);

                proc1.currentNGCountInX[(int)TOOL_TYPE.LEFT] = 0;
                proc1.currentNGCountInX[(int)TOOL_TYPE.RIGHT] = 0;
                proc1.currentNGCountInY[(int)TOOL_TYPE.LEFT] = 0;
                proc1.currentNGCountInY[(int)TOOL_TYPE.RIGHT] = 0;
                proc1.currentNGCount[(int)TOOL_TYPE.LEFT] = 0;
                proc1.currentNGCount[(int)TOOL_TYPE.RIGHT] = 0;
                int maxCount = maxPartIn_X_Left*maxPartIn_Y_Left + maxPartIn_X_Right*maxPartIn_Y_Right;
                EDM.SetNGBoxStatus(0, maxCount, false);
                
            }
        }

        private void label_Left_Click(object sender, EventArgs e)
        {
            if (Machine.status.state != SystemState.SystemStateIDLE) return;
            bool isUseLeftBuf = Machine.param.Option(ParameterDefine.OPTION.USE_LEFT_BUFFER) == 1;
            int newState = isUseLeftBuf ? 0 : 1;
            string newStateText = newState == 0 ? "NOT USE" : "USE";
            Dlg_MessageBox formErr = new Dlg_MessageBox(EMESSAGEBOX.QUESTION, $"Do you want to change Left Buffer to {newStateText}?");
            formErr.TopLevel = true;
            formErr.TopMost = true;
            if (formErr.ShowDialog() == DialogResult.Yes)
            {
                Machine.param.Option(ParameterDefine.OPTION.USE_LEFT_BUFFER, newState);
                Machine.param.Write();
            }
        }

        private void label_Right_Click(object sender, EventArgs e)
        {
            if (Machine.status.state != SystemState.SystemStateIDLE) return;
            bool isUseRightBuf = Machine.param.Option(ParameterDefine.OPTION.USE_RIGHT_BUFFER) == 1;
            int newState = isUseRightBuf ? 0 : 1;
            string newStateText = newState == 0 ? "NOT USE" : "USE";
            Dlg_MessageBox formErr = new Dlg_MessageBox(EMESSAGEBOX.QUESTION, $"Do you want to change Right Buffer to {newStateText}?");
            formErr.TopLevel = true;
            formErr.TopMost = true;
            if (formErr.ShowDialog() == DialogResult.Yes)
            {
                Machine.param.Option(ParameterDefine.OPTION.USE_RIGHT_BUFFER, newState);
                Machine.param.Write();
            }
        }

        private void label_PlaceCount_Click(object sender, EventArgs e)
        {
            Dlg_MessageBox formMsg = new 
                Dlg_MessageBox(EMESSAGEBOX.QUESTION, ($"Do you want Reset Place Count?"));
            formMsg.TopLevel = true;
            formMsg.TopMost = true;
            if (formMsg.ShowDialog() == DialogResult.Yes)
            {
                SubForm_Login dlg = new SubForm_Login(SystemDefine.USER_LEVEL.AUTH_DATA);
                if (DialogResult.OK != dlg.ShowDialog())
                    return;

                int ProductPlaceCount = Machine.info.Product(StatusDefine.PRODUCT.PRODUCT_PLACE_COUNT);
                int TrayPlaceCount = Machine.info.Product(StatusDefine.PRODUCT.TRAY_PLACE_COUNT);

                string logText = 
                    $"[Prodcut Place Count : {ProductPlaceCount} -> 0]" +
                    $"[Tray Place Count: {TrayPlaceCount} -> 0]";
                Machine.info.Product(StatusDefine.PRODUCT.PRODUCT_PLACE_COUNT, 0);
                Machine.info.Product(StatusDefine.PRODUCT.TRAY_PLACE_COUNT, 0);
                LogUtil.Instance.Log(LOG_TYPE.DATA, logText, CONTENT_TYPE.INFO);
            }
        }

        private void btnSkipLeftPart_Click(object sender, EventArgs e)
        {
            if (Machine.status.state != SystemState.SystemStateIDLE) return;
            Dlg_MessageBox formMsg = new
                Dlg_MessageBox(EMESSAGEBOX.QUESTION, ($"Do you want to Skip Left Part in Picker?"));
            formMsg.TopLevel = true;
            formMsg.TopMost = true;
            if (formMsg.ShowDialog() != DialogResult.Yes) return;
            var proc = Machine.proclist[(int)Machine.PROCESS.TRAYWORK] as ProcessTrayWork;
            Machine.trayNGListX.Add(proc.productPosition[0].x);
            Machine.trayNGListY.Add(proc.productPosition[0].y);
            Vision.CaptureByInput("TRAY", TopEng.Vision.CaptureUtil.CAPTURETYPE.TRAIN, Vision.cogTool[0].clImageLeft);
        }

        private void btnSkipRightPart_Click(object sender, EventArgs e)
        {
            if (Machine.status.state != SystemState.SystemStateIDLE) return;
            Dlg_MessageBox formMsg = new
                Dlg_MessageBox(EMESSAGEBOX.QUESTION, ($"Do you want to Skip Right Part in Picker?"));
            formMsg.TopLevel = true;
            formMsg.TopMost = true;
            if (formMsg.ShowDialog() != DialogResult.Yes) return;
            var proc = Machine.proclist[(int)Machine.PROCESS.TRAYWORK] as ProcessTrayWork;
            Machine.trayNGListX.Add(proc.productPosition[1].x);
            Machine.trayNGListY.Add(proc.productPosition[1].y);
            Vision.CaptureByInput("TRAY", TopEng.Vision.CaptureUtil.CAPTURETYPE.TRAIN, Vision.cogTool[0].clImageRight);
        }

        private void btnClearTray_Click(object sender, EventArgs e)
        {
            if (Machine.status.state != SystemState.SystemStateIDLE) return;
            Dlg_MessageBox formMsg = new
                Dlg_MessageBox(EMESSAGEBOX.QUESTION, ($"Do you want to clear Tray Data?"));
            formMsg.TopLevel = true;
            formMsg.TopMost = true;
            if (formMsg.ShowDialog() != DialogResult.Yes) return;

            var proc = Machine.proclist[(int)Machine.PROCESS.TRAYWORK] as ProcessTrayWork;
            proc.currentTrayAlignPos = 0;
            Machine.trayNGListX.Clear();
            Machine.trayNGListY.Clear();
            proc.trayAngle = string.Empty;
            proc.currentProductCount = 0;
        }
    }
}
