using static CAMASSEMBLYMACHINE.Define.SystemDefine;
using static CAMASSEMBLYMACHINE.Define.UIDefine;
using System;
using System.Windows.Forms;
using CAMASSEMBLYMACHINE.Define;
using System.Security;
using Cognex.VisionPro;
using Cognex.VisionPro.ImageFile;
using System.Threading;
using CAMASSEMBLYMACHINE.Process;
using TopEng.Utils;
using TopEng.Vision;
using TopEng.Vision.Forms;
using TopEng.Controls;
using TopEng.Type;
using static CAMASSEMBLYMACHINE.Define.ParameterDefine;
using System.Linq;
using System.Drawing;

namespace CAMASSEMBLYMACHINE.UI
{
    public partial class tabAssemblerJigCalib : Form, IForm
    {
        #region VARIABLES
        CALIBRATION[] eParam =
        {
            CALIBRATION.JIG_CAL_MAP_START_POS_X,
            CALIBRATION.JIG_CAL_MAP_START_POS_Y,
            CALIBRATION.JIG_CAL_MAP_END_POS_X,
            CALIBRATION.JIG_CAL_MAP_END_POS_Y,
            CALIBRATION.JIG_CAL_MAP_POS_Z,
            CALIBRATION.JIG_CAL_MAP_STEP,
        };
        string[] tags;
        AXIS[] axislist = { AXIS.ASSEMBLER_X, AXIS.ASSEMBLER_Y, AXIS.ASSEMBLER_Z};
        UIJOGVELINFO veltype = UIJOGVELINFO.NORMAL;
        private int InspNo = -1;
        bool refreshData = true;

        Form_PanePickerJog paneJog;
        tabCalMapControl calMapForm;
        #endregion

        #region INITIALIZE
        public tabAssemblerJigCalib(int Id)
        {
            InitializeComponent();
            tags = eParam.Select(x => x.ToString()).ToArray();
            InspNo = Id;
            SetRatioTextboxEvent();
        }

        private void SetRatioTextboxEvent()
        {
            this.tbxRatioXLeft.Leave += new System.EventHandler(this.tbxRatio_Leave);
            this.tbxRatioYLeft.Leave += new System.EventHandler(this.tbxRatio_Leave);
            this.tbxRatioXRight.Leave += new System.EventHandler(this.tbxRatio_Leave);
            this.tbxRatioYRight.Leave += new System.EventHandler(this.tbxRatio_Leave);
        }

        private void tabVisionManuProp_Load(object sender, EventArgs e)
        {
            cogRecordDisplay1.AutoFit = true;

            if (SystemDefine.manualGrab)
                Grab();
            else
            {
                int targetCam = Vision.inspection.InspInfo[InspNo].targetCam;

                if (targetCam < Vision.cogTool.Count)
                    cogRecordDisplay1.Image = Vision.cogTool[targetCam].m_cogImage;
            }

            paneJog = new Form_PanePickerJog(1);
            paneJog.TopLevel = false;
            paneJog.Parent = tabPage1;
            paneJog.Dock = DockStyle.Fill;
            paneJog.Show();

            InitializeParamInfo();
            UpdateLightSetting();
        }

        private void InitializeParamInfo()
        {
            // COLUMNS
            string[] headername = { "Name", "StoredPos", "NewPos", "GetPos", "Accept" };
            int[] headersize = { 220, 75, 75, 70, 70 };
            string[] headertype = { "TEXT", "TEXT", "TEXT", "BUTTON", "BUTTON" };

            dataGridViewParam.Columns.Clear();

            for (int i = 0; i < headername.Length; i++)
            {
                if (headertype[i] == "TEXT")
                    dataGridViewParam.Columns.Add(headername[i], headername[i]);
                if (headertype[i] == "BUTTON")
                {
                    DataGridViewButtonColumn newColumn = new DataGridViewButtonColumn();
                    newColumn.HeaderText = headername[i];
                    newColumn.Name = headername[i];
                    dataGridViewParam.Columns.Add(newColumn);
                }

                dataGridViewParam.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
                dataGridViewParam.Columns[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                if (i == 0)
                    dataGridViewParam.Columns[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                dataGridViewParam.Columns[i].Width = headersize[i];
            }

            // ROWS
            for (int i = 0; i < tags.Length; i++)
            {
                dataGridViewParam.Rows.Add();
                dataGridViewParam.Rows[i].Cells[0].Value = Machine.param.calibration.Name(tags[i]);
                dataGridViewParam.Rows[i].Cells[1].Value = Machine.param.calibration[tags[i]].ToString("0.000");
                dataGridViewParam.Rows[i].Cells[2].Value = Machine.param.calibration[tags[i]].ToString("0.000");
                dataGridViewParam.Rows[i].Cells[3].Value = headername[3];
                dataGridViewParam.Rows[i].Cells[4].Value = headername[4];
            }

            dataGridViewParam.CurrentCell = null;
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
            UpdateAxisState();
            UpdateIOState();
            refreshData = false;
        }
        #endregion

        #region VISION_TAB
        private void button_VppEdit_Click(object sender, EventArgs e)
        {
            LogUtil.Instance.Log(LOG_TYPE.UI, "Assembler Jig Vpp Edit Button Click", CONTENT_TYPE.INFO);

            int targetCam = Vision.inspection.InspInfo[InspNo].targetCam;
            var level = new LIGHTDATA();
            level.a = Vision.Camera.cameraInfo[InspNo].lightSet.a;
            level.b = Vision.Camera.cameraInfo[InspNo].lightSet.b;
            level.c = Vision.Camera.cameraInfo[InspNo].lightSet.c;
            level.d = Vision.Camera.cameraInfo[InspNo].lightSet.d;
            Vision.Camera.LightSet(targetCam, level);
            Vision.Camera.LightOn(targetCam, true);

            //Grab();

            Vision.Grab(targetCam);
            Vision.GrabCompleted(targetCam);
            ICogImage image = Vision.cogTool[targetCam].m_cogImage;

            string CameraName = Vision.Camera.Name(InspNo);
            string filepath = SystemDefine.systemPath + $"\\CheckerBoard_{CameraName}.vpp";

            var jobEdit = new Form_ToolBlockEdit(filepath, image);
            jobEdit.ShowDialog();
            Vision.cogToolBlkCheckerBoard[targetCam].ReadBlock(filepath);

        }
        private void btnDrawLine_Click(object sender, EventArgs e)
        {
            CogToolGraphic tool = new CogToolGraphic();

            Point2d center = new Point2d();
            int targetCam = Vision.inspection.InspInfo[InspNo].targetCam;
            center.x = 0.5 * Vision.calibTool[targetCam].CamWidth;
            center.y = 0.5 * Vision.calibTool[targetCam].CamHeight;
            Size2d length = new Size2d();
            length.width = Vision.calibTool[targetCam].CamWidth;
            length.height = Vision.calibTool[targetCam].CamHeight;

            tool.Crossline("Calibration", cogRecordDisplay1, center, length, CogColorConstants.Green, CogGraphicLineStyleConstants.Solid);
        }

        private void btnClearLine_Click(object sender, EventArgs e)
        {
            cogRecordDisplay1.InteractiveGraphics.Clear();
            cogRecordDisplay1.StaticGraphics.Clear();
            cogRecordDisplay1.Record = null;
        }

        private void btnMappingCalibration_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < Machine.proclist.Count(); i++)
            {
                var proc = Machine.proclist[i];
                if ((proc.Ready() || proc.Error()) && !proc.Busy()) { }
                else
                {
                    MessageBox.Show("Please stop machine first!");
                    return;
                }
            }
            calMapForm = new tabCalMapControl(CAMERA.JIG);
            calMapForm.ShowDialog();
        }

        #endregion

        #region PARAMETERS_TAB
        private void dataGridViewParam_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            var dataGridView = (DataGridView)sender;
            int row = e.RowIndex;
            int col = e.ColumnIndex;

            if (dataGridView.Columns[e.ColumnIndex] is DataGridViewButtonColumn && e.RowIndex >= 0)
            {
                try
                {
                    if (col == (int)UIPARAMINFO.GETPOS)
                    {
                        int targetAxis = Machine.param.calibration.targetAxis(tags[row]);
                        double actualPos = 0;
                        Machine.motion.GetAxisActualPos(targetAxis, ref actualPos);
                        dataGridViewParam.Rows[row].Cells[(int)UIPARAMINFO.EDITED].Value = (0.001 * actualPos).ToString("0.000");
                    }

                    if (col == (int)UIPARAMINFO.ACCEPT)
                    {
                        string newValue = dataGridViewParam.Rows[row].Cells[(int)UIPARAMCOLDATA.EDITED].Value.ToString();
                        string curValue = dataGridViewParam.Rows[row].Cells[(int)UIPARAMCOLDATA.STORED].Value.ToString();
                        string unit = Machine.param.position.Unit(row);
                        dataGridViewParam.Rows[row].Cells[(int)UIPARAMINFO.STORED].Value = newValue;
                        Console.WriteLine(Convert.ToDouble(newValue));
                        Machine.param.calibration[tags[row]] = Convert.ToDouble(newValue);
                        Machine.param.Write();

                        string logText = $"[{Machine.param.calibration.Name(tags[row])}] data has changed. [{curValue} {unit} → {newValue} {unit}]";
                        LogUtil.Instance.Log(LOG_TYPE.DATA, logText, CONTENT_TYPE.INFO);
                    }
                }
                catch
                {

                }
            }
            else
            {
                try
                {
                    if (col == (int)UIPARAMCOLTEACH.EDITED)
                    {
                        string title = dataGridViewParam.Rows[row].Cells[(int)UIPARAMCOLTEACH.NAME].Value.ToString();
                        double min = Machine.param.calibration.Min(tags[row]);
                        double max = Machine.param.calibration.Max(tags[row]);
                        string unit = dataGridViewParam.Rows[row].Cells[(int)UIPARAMCOLDATA.UNIT].Value.ToString();
                        string[] value = { dataGridViewParam.Rows[row].Cells[(int)UIPARAMCOLTEACH.STORED].Value.ToString() };
                        SubForm_TenKey keyPad = new SubForm_TenKey(ref value, title, unit, min, max, 300, 300, true);
                        keyPad.ShowDialog();
                        dataGridViewParam.Rows[row].Cells[(int)UIPARAMCOLTEACH.EDITED].Value = value[0];
                    }
                }
                catch
                {

                }
            }
        }
        #endregion

        #region POSITION_TAB
        private void UpdateAxisState()
        {
            ledMapStartXY.ButtonPush = Util.IsAxisInPos(
                (AXIS.ASSEMBLER_X, CALIBRATION.JIG_CAL_MAP_START_POS_X),
                (AXIS.ASSEMBLER_Y, CALIBRATION.JIG_CAL_MAP_START_POS_Y));

            ledMapEndXY.ButtonPush = Util.IsAxisInPos(
                (AXIS.ASSEMBLER_X, CALIBRATION.JIG_CAL_MAP_END_POS_X),
                (AXIS.ASSEMBLER_Y, CALIBRATION.JIG_CAL_MAP_END_POS_Y));

            ledZReadyPos.ButtonPush = Util.IsAxisInPos(
                (AXIS.ASSEMBLER_Z, POSITION.ASSEMBLER_Z_READY_POS));

            ledZCalPos.ButtonPush = Util.IsAxisInPos(
                (AXIS.ASSEMBLER_Z, CALIBRATION.JIG_CAL_MAP_POS_Z));

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

            Machine.IO.GetIn((int)DI.ASSEMBLER_Z2_UP, ref returnValue);
            disp_Tool2Up.ButtonPush = returnValue == 1 ? true : false;
            Machine.IO.GetIn((int)DI.ASSEMBLER_Z2_DOWN, ref returnValue);
            disp_Tool2Down.ButtonPush = returnValue == 1 ? true : false;
            Machine.IO.GetOut((int)DO.ASSEMBLER_Z2_UP, ref returnValue);
        }

        private void button_MapStartXY_Click(object sender, EventArgs e)
        {
            LogUtil.Instance.Log(LOG_TYPE.UI, "Assembler Move to Jig Calibration 1 Pos Button Click", CONTENT_TYPE.INFO);

            var procSys = Machine.proclist[(int)Machine.PROCESS.SYS] as ProcessSys;
            if (procSys.doorOpenState)
            {
                Dlg_MessageBox msgBox = new Dlg_MessageBox(EMESSAGEBOX.MSG,
                               string.Format("Please Close the door"));
                msgBox.ShowDialog();
                return;
            }

            var proc = Machine.proclist[(int)Machine.PROCESS.ASSEMBLER];
            proc.SetMessage((int)ProcessAssembler.MSG.MSG_MOVE_MAP_JIG_START_XY);
        }

        private void button_MapEndXY_Click(object sender, EventArgs e)
        {
            LogUtil.Instance.Log(LOG_TYPE.UI, "Assembler Move to Jig Calibration 2 Pos Button Click", CONTENT_TYPE.INFO);

            var procSys = Machine.proclist[(int)Machine.PROCESS.SYS] as ProcessSys;
            if (procSys.doorOpenState)
            {
                Dlg_MessageBox msgBox = new Dlg_MessageBox(EMESSAGEBOX.MSG,
                               string.Format("Please Close the door"));
                msgBox.ShowDialog();
                return;
            }

            var proc = Machine.proclist[(int)Machine.PROCESS.ASSEMBLER];
            proc.SetMessage((int)ProcessAssembler.MSG.MSG_MOVE_MAP_JIG_END_XY);
        }


        private void button_ZReadyPos_Click(object sender, EventArgs e)
        {
            LogUtil.Instance.Log(LOG_TYPE.UI, "Assembler Move to Ready (Z) Pos Button Click", CONTENT_TYPE.INFO);

            var procSys = Machine.proclist[(int)Machine.PROCESS.SYS] as ProcessSys;
            if (procSys.doorOpenState)
            {
                Dlg_MessageBox msgBox = new Dlg_MessageBox(EMESSAGEBOX.MSG,
                               string.Format("Please Close the door"));
                msgBox.ShowDialog();
                return;
            }

            var proc = Machine.proclist[(int)Machine.PROCESS.ASSEMBLER];
            proc.SetMessage((int)ProcessAssembler.MSG.MSG_SINGLE, (int)ProcessAssembler.STEP.MOVE_Z_READY_POS);
        }

        private void button_ZCalPos_Click(object sender, EventArgs e)
        {
            LogUtil.Instance.Log(LOG_TYPE.UI, "Assembler Move to JIg Calibration Center (Z) Pos Button Click", CONTENT_TYPE.INFO);

            var procSys = Machine.proclist[(int)Machine.PROCESS.SYS] as ProcessSys;
            if (procSys.doorOpenState)
            {
                Dlg_MessageBox msgBox = new Dlg_MessageBox(EMESSAGEBOX.MSG,
                               string.Format("Please Close the door"));
                msgBox.ShowDialog();
                return;
            }

            var proc = Machine.proclist[(int)Machine.PROCESS.ASSEMBLER];
            proc.SetMessage((int)ProcessAssembler.MSG.MSG_SINGLE, (int)ProcessAssembler.STEP.MOVE_Z_JIG_CAL_POS);
        }

        private void button_ZCylDown_Click(object sender, EventArgs e)
        {
            uint ToolZUp = 0;
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

        private void button_Z2CylDown_Click(object sender, EventArgs e)
        {
            uint ToolZUp = 0;
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
        #endregion

        #region CAMERA_INFO_TAB
        private void dataGridViewCamInfo_VisibleChanged(object sender, EventArgs e)
        {
            UpdateCameraInfo(dtgvCamInfoLeft, TOOL_TYPE.LEFT);
            UpdateCameraInfo(dtgvCamInfoRight, TOOL_TYPE.RIGHT);
            tbxRatioXLeft.Text = Vision.calibMapRatioX[(int)CAMERA.JIG, (int)TOOL_TYPE.LEFT].ToString();
            tbxRatioYLeft.Text = Vision.calibMapRatioY[(int)CAMERA.JIG, (int)TOOL_TYPE.LEFT].ToString();
            tbxRatioXRight.Text = Vision.calibMapRatioX[(int)CAMERA.JIG, (int)TOOL_TYPE.RIGHT].ToString();
            tbxRatioYRight.Text = Vision.calibMapRatioY[(int)CAMERA.JIG, (int)TOOL_TYPE.RIGHT].ToString();
        }

        private void UpdateCameraInfo(DataGridView dtgvCamInfo, TOOL_TYPE toolType)
        {
            try
            {
                // COLUMNS
                string[] headername = { "Name", $"Value ({toolType})" };
                int[] headersize = { dtgvCamInfo.Width / 2, dtgvCamInfo.Width / 2 };
                string[] headertype = { "TEXT", "TEXT" };

                dtgvCamInfo.Columns.Clear();

                for (int i = 0; i < headername.Length; i++)
                {
                    if (headertype[i] == "TEXT")
                        dtgvCamInfo.Columns.Add(headername[i], headername[i]);
                    if (headertype[i] == "BUTTON")
                    {
                        DataGridViewButtonColumn newColumn = new DataGridViewButtonColumn();
                        newColumn.HeaderText = headername[i];
                        newColumn.Name = headername[i];
                        dtgvCamInfo.Columns.Add(newColumn);
                    }

                    dtgvCamInfo.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
                    dtgvCamInfo.Columns[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    if (i == 0)
                        dtgvCamInfo.Columns[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                    dtgvCamInfo.Columns[i].Width = headersize[i];
                }

                int targetCam = Vision.inspection.InspInfo[InspNo].targetCam;

                dtgvCamInfo.Rows.Add();
                dtgvCamInfo.Rows[0].Cells[0].Value = $"Total Points In Map";
                dtgvCamInfo.Rows[0].Cells[1].Value = Vision.GetCalMapData(CAMERA.JIG, toolType).Count();
                dtgvCamInfo.Rows.Add();
                dtgvCamInfo.Rows[1].Cells[0].Value = $"Angle";
                dtgvCamInfo.Rows[1].Cells[1].Value = Math.Round(Vision.GetCameraAngle(CAMERA.JIG, toolType), 4);
                dtgvCamInfo.Rows.Add();
                dtgvCamInfo.Rows[2].Cells[0].Value = $"Avg Pixel Size X";
                dtgvCamInfo.Rows[2].Cells[1].Value = Math.Round(Vision.pixelSize[(int)CAMERA.JIG, (int)toolType].sizeX, 4);
                dtgvCamInfo.Rows.Add();
                dtgvCamInfo.Rows[3].Cells[0].Value = $"Avg Pixel Size Y";
                dtgvCamInfo.Rows[3].Cells[1].Value = Math.Round(Vision.pixelSize[(int)CAMERA.JIG, (int)toolType].sizeY, 4);

                dtgvCamInfo.CurrentCell = null;
            }
            catch (Exception ex) { }
        }
        #endregion

        #region VISION_FUNCTION
        private void tbxRatio_Leave(object sender, EventArgs e)
        {
            try
            {
                Dlg_MessageBox msgBox = new Dlg_MessageBox(EMESSAGEBOX.QUESTION,
                                         string.Format("Do you want to change Ratio?"));
                msgBox.ShowDialog();
                if (msgBox.DialogResult != DialogResult.Yes)
                {
                    dataGridViewCamInfo_VisibleChanged(null, null);
                    return;
                }
                string tag = (sender as Control).Tag.ToString();
                string value = (sender as Control).Text;
                if (tag == "LEFT_X") Vision.calibMapRatioX[(int)CAMERA.JIG, (int)TOOL_TYPE.LEFT] = double.Parse(value);
                if (tag == "LEFT_Y") Vision.calibMapRatioY[(int)CAMERA.JIG, (int)TOOL_TYPE.LEFT] = double.Parse(value);
                if (tag == "RIGHT_X") Vision.calibMapRatioX[(int)CAMERA.JIG, (int)TOOL_TYPE.RIGHT] = double.Parse(value);
                if (tag == "RIGHT_Y") Vision.calibMapRatioY[(int)CAMERA.JIG, (int)TOOL_TYPE.RIGHT] = double.Parse(value);
                Vision.Camera.RatioLeftX((int)CAMERA.JIG, Vision.calibMapRatioX[(int)CAMERA.JIG, (int)TOOL_TYPE.LEFT]);
                Vision.Camera.RatioLeftY((int)CAMERA.JIG, Vision.calibMapRatioY[(int)CAMERA.JIG, (int)TOOL_TYPE.LEFT]);
                Vision.Camera.RatioRightX((int)CAMERA.JIG, Vision.calibMapRatioX[(int)CAMERA.JIG, (int)TOOL_TYPE.RIGHT]);
                Vision.Camera.RatioRightY((int)CAMERA.JIG, Vision.calibMapRatioY[(int)CAMERA.JIG, (int)TOOL_TYPE.RIGHT]);
                Vision.UpdateCalibListByRatio(CAMERA.JIG, TOOL_TYPE.LEFT);
                Vision.UpdateCalibListByRatio(CAMERA.JIG, TOOL_TYPE.RIGHT);
                Vision.Camera.Write();
            }
            catch (Exception EX)
            {

            }

        }

        public void UpdateLightSetting()
        {
            textBox_RLv.Text = Vision.Camera.cameraInfo[InspNo].lightSet.a.ToString();
            textBox_GLv.Text = Vision.Camera.cameraInfo[InspNo].lightSet.b.ToString();
            textBox_BLv.Text = Vision.Camera.cameraInfo[InspNo].lightSet.c.ToString();
            textBox_BLv2.Text = Vision.Camera.cameraInfo[InspNo].lightSet.d.ToString();

            if (dtgvCamInfoLeft.RowCount > 2)
            {
                dtgvCamInfoLeft.Rows[0].Cells[1].Value = Vision.inspection.recipeData[InspNo].inspInfo.RatioX;
                dtgvCamInfoLeft.Rows[1].Cells[1].Value = Vision.inspection.recipeData[InspNo].inspInfo.RatioY;
            }
        }

        private void button_ExpSave_Click(object sender, EventArgs e)
        {
            LogUtil.Instance.Log(LOG_TYPE.DATA, $"Assembler Jig Calibration / LightSet Save Button Click", CONTENT_TYPE.INFO);

            Vision.Camera.cameraInfo[InspNo].lightSet.a = Convert.ToInt16(textBox_RLv.Text);
            Vision.Camera.cameraInfo[InspNo].lightSet.b = Convert.ToInt16(textBox_GLv.Text);
            Vision.Camera.cameraInfo[InspNo].lightSet.c = Convert.ToInt16(textBox_BLv.Text);
            Vision.Camera.cameraInfo[InspNo].lightSet.d = Convert.ToInt16(textBox_BLv2.Text);

            Vision.Camera.Write();

            Dlg_MessageBox msgBox = new Dlg_MessageBox(EMESSAGEBOX.MSG,
                               string.Format("Light Data is Saved"));
            msgBox.TopLevel = true;
            msgBox.TopMost = true;
            msgBox.ShowDialog();
        }

        private void Grab()
        {
            cogRecordDisplay1.InteractiveGraphics.Clear();
            cogRecordDisplay1.StaticGraphics.Clear();
            cogRecordDisplay1.Record = null;

            int targetCam = Vision.inspection.InspInfo[InspNo].targetCam;

            if (targetCam < Vision.cogTool.Count)
            {
                Vision.cogTool[targetCam].m_cogRecordDisplay = cogRecordDisplay1;
                Vision.Grab(targetCam);
            }

            //Vision.GrabCompleted(targetCam);
        }

        private void button_Test_Click(object sender, EventArgs e)
        {
            int targetCam = Vision.inspection.InspInfo[InspNo].targetCam;
            var level = new LIGHTDATA();
            level.a = Vision.Camera.cameraInfo[InspNo].lightSet.a;
            level.b = Vision.Camera.cameraInfo[InspNo].lightSet.b;
            level.c = Vision.Camera.cameraInfo[InspNo].lightSet.c;
            level.d = Vision.Camera.cameraInfo[InspNo].lightSet.d;
            Vision.Camera.LightSet(targetCam, level);
            Vision.Camera.LightOn(targetCam, true);

            Vision.cogTool[targetCam].m_cogRecordDisplay = cogRecordDisplay1;

            var proc = Machine.proclist[(int)Machine.PROCESS.JIGWORK];
            proc.SetMessage((int)ProcessJigWork.MSG.MSG_SINGLE, (int)ProcessJigWork.STEP.REQUEST_VISIOTN_ALIGN);
        }

        private void button_Grab_Click(object sender, EventArgs e)
        {
            int targetCam = Vision.inspection.InspInfo[InspNo].targetCam;
            var level = new LIGHTDATA();
            level.a = Vision.Camera.cameraInfo[InspNo].lightSet.a;
            level.b = Vision.Camera.cameraInfo[InspNo].lightSet.b;
            level.c = Vision.Camera.cameraInfo[InspNo].lightSet.c;
            level.d = Vision.Camera.cameraInfo[InspNo].lightSet.d;
            Vision.Camera.LightSet(targetCam, level);
            Vision.Camera.LightOn(targetCam, true);

            Grab();
            button_Live.ButtonPush = false;
        }

        private void button_Live_Click(object sender, EventArgs e)
        {
            cogRecordDisplay1.InteractiveGraphics.Clear();
            cogRecordDisplay1.StaticGraphics.Clear();
            cogRecordDisplay1.Record = null;

            int targetCam = Vision.inspection.InspInfo[InspNo].targetCam;
            var level = new LIGHTDATA();
            level.a = Vision.Camera.cameraInfo[InspNo].lightSet.a;
            level.b = Vision.Camera.cameraInfo[InspNo].lightSet.b;
            level.c = Vision.Camera.cameraInfo[InspNo].lightSet.c;
            level.d = Vision.Camera.cameraInfo[InspNo].lightSet.d;
            Vision.Camera.LightSet(targetCam, level);
            Vision.Camera.LightOn(targetCam, true);

            if (targetCam < Vision.cogTool.Count)
            {
                Vision.cogTool[targetCam].m_cogRecordDisplay = cogRecordDisplay1;
                Vision.LiveStart(targetCam);
                button_Live.ButtonPush = true;
            }
        }

        private void button_Load_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog()
            {
                FileName = "Select a Image file",
                Filter = "Image files (*.bmp)|*.bmp",
                Title = "Open Image file"
            };

            if (DialogResult.OK == dlg.ShowDialog())
            {
                try
                {
                    var filepath = dlg.FileName;

                    CogImageFile file = new CogImageFile();
                    file.Open(filepath, CogImageFileModeConstants.Read);
                    var cogImage = file[0];
                    file.Close();

                    int targetCam = Vision.inspection.InspInfo[InspNo].targetCam;
                    cogRecordDisplay1.Image = CogImageConvert.GetIntensityImage(cogImage, 0, 0, cogImage.Width, cogImage.Height);
                    Vision.cogTool[targetCam].m_cogImageNoDevice = cogRecordDisplay1.Image;
                    Vision.cogTool[targetCam].m_cogImage = cogRecordDisplay1.Image;

                    cogRecordDisplay1.InteractiveGraphics.Clear();
                    cogRecordDisplay1.StaticGraphics.Clear();
                }
                catch (SecurityException ex)
                {
                    MessageBox.Show($"Security error.\n\nError message: {ex.Message}\n\n" +
                    $"Details:\n\n{ex.StackTrace}");
                }
            }
        }

        private void button_Save_Click(object sender, EventArgs e)
        {
            int targetCam = Vision.inspection.InspInfo[InspNo].targetCam;
            string name = Vision.inspection.InspInfo[InspNo].Name;
            Vision.cogTool[targetCam].Capture(name, CaptureUtil.CAPTURETYPE.USER, cogRecordDisplay1.Image);
        }

        private void textBox_RLv_TextChanged(object sender, EventArgs e)
        {
            try
            {
                int level = Convert.ToInt16(textBox_RLv.Text);
                if (level < 0) level = 0;
                if (level > 255) level = 255;
                textBox_RLv.Text = level.ToString();
            }
            catch
            {
                textBox_RLv.Text = "0";
            }
        }

        private void textBox_GLv_TextChanged(object sender, EventArgs e)
        {
            try
            {
                int level = Convert.ToInt16(textBox_GLv.Text);
                if (level < 0) level = 0;
                if (level > 255) level = 255;
                textBox_GLv.Text = level.ToString();
            }
            catch
            {
                textBox_GLv.Text = "0";
            }
        }

        private void textBox_BLv_TextChanged(object sender, EventArgs e)
        {
            try
            {
                int level = Convert.ToInt16(textBox_BLv.Text);
                if (level < 0) level = 0;
                if (level > 255) level = 255;
                textBox_BLv.Text = level.ToString();
            }
            catch
            {
                textBox_BLv.Text = "0";
            }
        }

        private void textBox_BLv2_TextChanged(object sender, EventArgs e)
        {
            try
            {
                int level = Convert.ToInt16(textBox_BLv2.Text);
                if (level < 0) level = 0;
                if (level > 255) level = 255;
                textBox_BLv2.Text = level.ToString();
            }
            catch
            {
                textBox_BLv2.Text = "0";
            }
        }

        private void textBox_RLv_Click(object sender, EventArgs e)
        {
            string[] value = { textBox_RLv.Text };
            SubForm_TenKey keyPad = new SubForm_TenKey(ref value, "RED Light(Lv)", "Lv", 0, 255, 300, 300, false);
            keyPad.ShowDialog();
            textBox_RLv.Text = value[0];
        }

        private void textBox_GLv_Click(object sender, EventArgs e)
        {
            string[] value = { textBox_GLv.Text };
            SubForm_TenKey keyPad = new SubForm_TenKey(ref value, "GREEN Light(Lv)", "Lv", 0, 255, 300, 300, false);
            keyPad.ShowDialog();
            textBox_GLv.Text = value[0];
        }

        private void textBox_BLv_Click(object sender, EventArgs e)
        {
            string[] value = { textBox_BLv.Text };
            SubForm_TenKey keyPad = new SubForm_TenKey(ref value, "BLUE Light(Lv)", "Lv", 0, 255, 300, 300, false);
            keyPad.ShowDialog();
            textBox_BLv.Text = value[0];
        }

        private void textBox_BLv2_Click(object sender, EventArgs e)
        {
            string[] value = { textBox_BLv2.Text };
            SubForm_TenKey keyPad = new SubForm_TenKey(ref value, "BLUE Light 2(Lv)", "Lv", 0, 255, 300, 300, false);
            keyPad.ShowDialog();
            textBox_BLv2.Text = value[0];
        }

        private void button_LightOn_Click(object sender, EventArgs e)
        {
            try
            {
                int targetCam = Vision.inspection.InspInfo[InspNo].targetCam;
                int r = Convert.ToInt16(textBox_RLv.Text);
                //int g = Convert.ToInt16(textBox_GLv.Text);
                //int b = Convert.ToInt16(textBox_BLv.Text);

                var level = new LIGHTDATA();
                level.a = Convert.ToInt16(textBox_RLv.Text);
                level.b = Convert.ToInt16(textBox_RLv.Text);
                level.c = Convert.ToInt16(textBox_RLv.Text);
                level.d = Convert.ToInt16(textBox_RLv.Text);

                Vision.Camera.LightSet(targetCam, level);
                Vision.Camera.LightOn(targetCam, true);
            }
            catch
            {

            }
        }

        private void button_LightOff_Click(object sender, EventArgs e)
        {
            try
            {
                int targetCam = Vision.inspection.InspInfo[InspNo].targetCam;
                Vision.Camera.LightOn(targetCam, false);
            }
            catch
            {

            }
        }
        #endregion
    }
}
