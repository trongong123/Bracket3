using CAMASSEMBLYMACHINE.Define;
using CAMASSEMBLYMACHINE.Process;
using Cognex.VisionPro;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TopEng.Controls;
using TopEng.Utils;
using TopEng.Vision;
using TopEng.Vision.Forms;
using static CAMASSEMBLYMACHINE.Define.SystemDefine;

namespace CAMASSEMBLYMACHINE.UI
{
    public partial class tabCalPointDetail : Form
    {
        #region INITIALIZE
        List<CalibPoint> pointList;
        CAMERA cam;
        int InspNo;
        TOOL_TYPE toolType;
        DataGridViewCheckBoxColumn checkCol = new DataGridViewCheckBoxColumn();
        bool isDataChanged = false;

        public tabCalPointDetail(CAMERA camera, TOOL_TYPE toolType, List<CalibPoint> point)
        {
            InitializeComponent();
            this.pointList = point;
            this.cam = camera;
            this.toolType = toolType;
            this.InspNo = (int)cam;
            InitUI();
        }

        private void InitUI()
        {
            dataGridView.Columns.Clear();

            // COLUMNS
            string[] headername = { "Result", "Xmm", "Ymm", "Xpx", "Ypx", "Predicted Xpx", "Predicted Ypx" };
            int width = (int)dataGridView.Width / (headername.Count() + 1);
            checkCol.HeaderText = "Select";
            checkCol.ReadOnly = false;
            checkCol.TrueValue = true;
            checkCol.FalseValue = false;
            dataGridView.ReadOnly = false;
            dataGridView.Columns.Add(checkCol);
            tableLayoutPanel1.Controls.Add(dataGridView, 0, 1);
            dataGridView.Dock = DockStyle.Fill;

            for (int i = 0; i < headername.Length; i++)
            {
                dataGridView.Columns.Add(headername[i], headername[i]);
                dataGridView.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
                dataGridView.Columns[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                if (i == 0)
                    dataGridView.Columns[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                dataGridView.Columns[i].Width = width;
                dataGridView.Columns[headername[i]].ReadOnly = true;
            }

            // ROWS
            for (int i = 0; i < pointList.Count(); i++)
            {
                dataGridView.Rows.Add();
                dataGridView.Rows[i].Cells[0].Value = true;
                if (pointList[i].Xpx == Vision.INVALID_DATA || pointList[i].Ypx == Vision.INVALID_DATA)
                {
                    dataGridView.Rows[i].Cells[1].Value = "NG"; 
                    dataGridView.Rows[i].Cells[1].Style.BackColor = Color.OrangeRed;
                    dataGridView.Rows[i].Cells[1].Style.ForeColor = Color.White;
                }
                else
                {
                    dataGridView.Rows[i].Cells[1].Value = "OK";
                    dataGridView.Rows[i].Cells[1].Style.BackColor = Color.LimeGreen;
                    dataGridView.Rows[i].Cells[1].Style.ForeColor = Color.White;
                }

                dataGridView.Rows[i].Cells[2].Value = Math.Round(pointList[i].Xmm, 3).ToString();
                dataGridView.Rows[i].Cells[3].Value = Math.Round(pointList[i].Ymm, 3).ToString();
                dataGridView.Rows[i].Cells[4].Value = Math.Round(pointList[i].Xpx, 3).ToString();
                dataGridView.Rows[i].Cells[5].Value = Math.Round(pointList[i].Ypx, 3).ToString();
                dataGridView.Rows[i].Cells[6].Value = Math.Round(Vision.GetPredictedXYpx(cam, toolType, pointList[i].Xmm, pointList[i].Ymm).Xpx, 3).ToString();
                dataGridView.Rows[i].Cells[7].Value = Math.Round(Vision.GetPredictedXYpx(cam, toolType, pointList[i].Xmm, pointList[i].Ymm).Ypx, 3).ToString();
            }

            dataGridView.CurrentCell = null;

            lblTotal.Text = $"Total: {pointList.Count()}";
            lblSelected.Text = $"Selected: {GetSelectedCalibPointList().Count()}";
        }

        private void RefreshUI()
        {
            try
            {
                lblTotal.Text = $"Total: {pointList.Count()}";
                lblSelected.Text = $"Selected: {GetSelectedCalibPointList().Count()}";
                for (int i = 0; i < pointList.Count(); i++)
                {
                    if (pointList[i].Xpx == Vision.INVALID_DATA || pointList[i].Ypx == Vision.INVALID_DATA)
                    {
                        dataGridView.Rows[i].Cells[1].Value = "NG";
                        dataGridView.Rows[i].Cells[1].Style.BackColor = Color.OrangeRed;
                        dataGridView.Rows[i].Cells[1].Style.ForeColor = Color.White;
                    }
                    else
                    {
                        dataGridView.Rows[i].Cells[1].Value = "OK";
                        dataGridView.Rows[i].Cells[1].Style.BackColor = Color.LimeGreen;
                        dataGridView.Rows[i].Cells[1].Style.ForeColor = Color.White;
                    }

                    dataGridView.Rows[i].Cells[2].Value = Math.Round(pointList[i].Xmm, 3).ToString();
                    dataGridView.Rows[i].Cells[3].Value = Math.Round(pointList[i].Ymm, 3).ToString();
                    dataGridView.Rows[i].Cells[4].Value = Math.Round(pointList[i].Xpx, 3).ToString();
                    dataGridView.Rows[i].Cells[5].Value = Math.Round(pointList[i].Ypx, 3).ToString();
                    dataGridView.Rows[i].Cells[6].Value = Math.Round(Vision.GetPredictedXYpx(cam, toolType, pointList[i].Xmm, pointList[i].Ymm).Xpx, 3).ToString();
                    dataGridView.Rows[i].Cells[7].Value = Math.Round(Vision.GetPredictedXYpx(cam, toolType, pointList[i].Xmm, pointList[i].Ymm).Ypx, 3).ToString();
                }
            }
            catch (Exception ex) { }
        }
        #endregion

        #region MANUAL_DATA_CHANGE
        private void dataGridView_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dataGridView.IsCurrentCellDirty)
            {
                dataGridView.CommitEdit(DataGridViewDataErrorContexts.Commit);
                RefreshUI();
            }
        }

        private void dataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            var dataGridView = (DataGridView)sender;
            int row = e.RowIndex;
            int col = e.ColumnIndex;

            try
            {
                if (col == 4 || col == 5)
                {
                    string title;
                    if (col == 4) { title = "Xpx"; }
                    else { title = "Ypx"; }
                    double min = -999999;
                    double max = 999999;
                    string unit = "px";
                    string[] value = { "0" };
                    SubForm_TenKey keyPad = new SubForm_TenKey(ref value, title, unit, min, max, 300, 300, true);
                    if (keyPad.ShowDialog() != DialogResult.OK) return;
                    dataGridView.Rows[row].Cells[col].Value = value[0];
                    if (col == 4) pointList[row].Xpx = double.Parse(value[0]);
                    else if (col == 5) pointList[row].Ypx = double.Parse(value[0]);
                    Vision.ChangePointDataInCalMap(cam, toolType, pointList[row].Xmm, pointList[row].Ymm, pointList[row].Xpx, pointList[row].Ypx);
                    RefreshUI();
                    isDataChanged = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Invalid Data Save Error! Please recheck data \n Exc:{ex}");
            }
        }
        #endregion

        #region MAIN_FUNCTION
        private void btnSetPredictedData_ClickEvent(object sender, EventArgs e)
        {
            try
            {
                isDataChanged = true;
                for (int i = 0; i < pointList.Count(); i++)
                {
                    if ((bool)dataGridView.Rows[i].Cells[0].Value == false) continue;
                    double Xpx = Math.Round(Vision.GetPredictedXYpx(cam, toolType, pointList[i].Xmm, pointList[i].Ymm).Xpx, 3);
                    double Ypx = Math.Round(Vision.GetPredictedXYpx(cam, toolType, pointList[i].Xmm, pointList[i].Ymm).Ypx, 3);
                    dataGridView.Rows[i].Cells[4].Value = Xpx.ToString();
                    dataGridView.Rows[i].Cells[5].Value = Ypx.ToString();
                    Vision.ChangePointDataInCalMap(cam, toolType, pointList[i].Xmm, pointList[i].Ymm, Xpx, Ypx);
                }
                RefreshUI();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Invalid Data Save Error! Please recheck data \n Exc:{ex}");
            }
        }

        private void btnEditVpp_ClickEvent(object sender, EventArgs e)
        {
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

            var jobEdit = new Form_ToolBlockEdit(filepath,image);
            if (toolType == TOOL_TYPE.LEFT) jobEdit = new Form_ToolBlockEdit(filepath, image, "CogToolBlock1");
            else if (toolType == TOOL_TYPE.RIGHT) jobEdit = new Form_ToolBlockEdit(filepath, image, "CogToolBlock2");
            jobEdit.ShowDialog();
            Vision.cogToolBlkCheckerBoard[targetCam].ReadBlock(filepath);
        }

        private void btnCal_ClickEvent(object sender, EventArgs e)
        {
            if (GetSelectedCalibPointList().Count() <= 0)
            {
                Dlg_MessageBox msgBox = new Dlg_MessageBox(EMESSAGEBOX.MSG,
                              string.Format("Please select at least 1 Point"));
                msgBox.ShowDialog();
                return;
            }

            Dlg_MessageBox formErr = new Dlg_MessageBox(EMESSAGEBOX.QUESTION, "Do you want to ReCal Selected Point?");
            formErr.TopLevel = true;
            formErr.TopMost = true;
            if (formErr.ShowDialog() == DialogResult.No) return;

            isDataChanged = true;
            int targetCam = Vision.inspection.InspInfo[InspNo].targetCam;
            var proc = Machine.proclist[(int)Machine.PROCESS.ASSEMBLER];
            List<CalibPoint> pointList = AdjustCalibPointList(GetSelectedCalibPointList());
            switch (cam)
            {
                case CAMERA.JIG:
                    proc = Machine.proclist[(int)Machine.PROCESS.ASSEMBLER];
                    ((ProcessAssembler)proc).SetReCalPoint(pointList);
                    proc.SetHeadTarget((int)toolType);
                    proc.SetMessage((int)ProcessAssembler.MSG.MSG_MAPPING_RECAL_JIG_POINTS);
                    break;
                case CAMERA.UNDER:
                    proc = Machine.proclist[(int)Machine.PROCESS.ASSEMBLER];
                    ((ProcessAssembler)proc).SetReCalPoint(pointList);
                    proc.SetHeadTarget((int)toolType);
                    proc.SetMessage((int)ProcessAssembler.MSG.MSG_MAPPING_RECAL_UNDER_POINTS);
                    break;
                case CAMERA.TRAY:
                    proc = Machine.proclist[(int)Machine.PROCESS.PROD_LOADER];
                    ((ProcessProdLoader)proc).SetReCalPoint(pointList);
                    proc.SetHeadTarget((int)toolType);
                    proc.SetMessage((int)ProcessProdLoader.MSG.MSG_MAPPING_RECAL_TRAY_POINTS);
                    break;
                case CAMERA.PICKER:
                    proc = Machine.proclist[(int)Machine.PROCESS.PROD_LOADER];
                    ((ProcessProdLoader)proc).SetReCalPoint(pointList);
                    proc.SetHeadTarget((int)toolType);
                    proc.SetMessage((int)ProcessProdLoader.MSG.MSG_MAPPING_RECAL_PICKER_POINTS);
                    break;
            }
            Task.Run(() =>
            {
                while (true)
                {
                    if ((proc.Ready() || proc.Error()) && !proc.Busy()) break;
                    Util.Delay(500);
                }
                RefreshUI();
            });
        }

        private List<CalibPoint> AdjustCalibPointList(List<CalibPoint> pointList)
        {
            double toolLength = 0;
            if (cam == CAMERA.JIG)
                toolLength = Machine.param.Option(ParameterDefine.OPTION.VISION_CAL_CENTER_LENGTH_JIG);
            else if (cam == CAMERA.UNDER)
                toolLength = Machine.param.Option(ParameterDefine.OPTION.VISION_CAL_CENTER_LENGTH_UNDER);
            else if (cam == CAMERA.TRAY)
                toolLength = Machine.param.Option(ParameterDefine.OPTION.VISION_CAL_CENTER_LENGTH_TRAY);
            else if (cam == CAMERA.PICKER)
                toolLength = Machine.param.Option(ParameterDefine.OPTION.VISION_CAL_CENTER_LENGTH_PICKER);

            List<CalibPoint> temp = new List<CalibPoint>();
            for (int i = 0; i < pointList.Count(); i++)
            {
                temp.Add(new CalibPoint (pointList[i].Xmm, pointList[i].Ymm, pointList[i].Xpx, pointList[i].Ypx));
                temp[i].Ymm += toolLength;
            }
            return temp;
        }

        private List<CalibPoint> GetSelectedCalibPointList()
        {
            List<CalibPoint> returnList = new List<CalibPoint>();
            for (int i = 0; i < pointList.Count(); i++)
            {
                if ((bool)dataGridView.Rows[i].Cells[0].Value == false) continue;
                returnList.Add(pointList[i]);
            }
            return returnList;
        }

        private void nV_Button_PB_NS2_ClickEvent(object sender, EventArgs e)
        {
            if (GetSelectedCalibPointList().Count() != 1)
            {
                Dlg_MessageBox msgBox = new Dlg_MessageBox(EMESSAGEBOX.MSG,
                              string.Format("Please select ONLY 1 Point"));
                msgBox.ShowDialog();
                return;
            }

            Dlg_MessageBox formErr = new Dlg_MessageBox(EMESSAGEBOX.QUESTION, "Do you want to move to Selected Point?");
            formErr.TopLevel = true;
            formErr.TopMost = true;
            if (formErr.ShowDialog() == DialogResult.No) return;

            int targetCam = Vision.inspection.InspInfo[InspNo].targetCam;
            var proc = Machine.proclist[(int)Machine.PROCESS.ASSEMBLER];
            List<CalibPoint> pointList = AdjustCalibPointList(GetSelectedCalibPointList());
            switch (cam)
            {
                case CAMERA.JIG:
                    proc = Machine.proclist[(int)Machine.PROCESS.ASSEMBLER];
                    ((ProcessAssembler)proc).SetReCalPoint(pointList);
                    proc.SetHeadTarget((int)toolType);
                    proc.SetMessage((int)ProcessAssembler.MSG.MSG_MOVE_RECAL_POINT_XY_JIG);
                    break;
                case CAMERA.UNDER:
                    proc = Machine.proclist[(int)Machine.PROCESS.ASSEMBLER];
                    ((ProcessAssembler)proc).SetReCalPoint(pointList);
                    proc.SetHeadTarget((int)toolType);
                    proc.SetMessage((int)ProcessAssembler.MSG.MSG_MOVE_RECAL_POINT_XY_UNDER);
                    break;
                case CAMERA.TRAY:
                    proc = Machine.proclist[(int)Machine.PROCESS.PROD_LOADER];
                    ((ProcessProdLoader)proc).SetReCalPoint(pointList);
                    proc.SetHeadTarget((int)toolType);
                    proc.SetMessage((int)ProcessProdLoader.MSG.MSG_MOVE_RECAL_POINT_XY_TRAY);
                    break;
                case CAMERA.PICKER:
                    proc = Machine.proclist[(int)Machine.PROCESS.PROD_LOADER];
                    ((ProcessProdLoader)proc).SetReCalPoint(pointList);
                    proc.SetHeadTarget((int)toolType);
                    proc.SetMessage((int)ProcessProdLoader.MSG.MSG_MOVE_RECAL_POINT_XY_PICKER);
                    break;
            }
        }

        private void tabCalPointDetail_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (isDataChanged) Vision.SaveCalibList(cam, toolType);
        }
        #endregion
    }
}
