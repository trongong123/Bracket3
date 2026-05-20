using CAMASSEMBLYMACHINE.Define;
using CAMASSEMBLYMACHINE.Process;
using Cognex.VisionPro;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using TopEng.Controls;
using TopEng.Type;
using TopEng.Utils;
using TopEng.Vision;
using TopEng.Vision.Forms;
using static CAMASSEMBLYMACHINE.Define.ParameterDefine;
using static CAMASSEMBLYMACHINE.Define.SystemDefine;

namespace CAMASSEMBLYMACHINE.UI
{
    public partial class tabCalMapControl : Form
    {
        #region INITIALIZE
        public CAMERA cam;
        private int InspNo;
        private TOOL_TYPE selectedToolType;
        Bitmap canvasLeft, canvasRight;
        Graphics gCanvasLeft, gCanvasRight;
        int cellSize = 0;
        int cols = 0;
        int rows = 0;
        int UI_PANEL_OFFSET = 50;
        private bool isMappingCalStarted = false;
        public tabCalMapControl(CAMERA cam)
        {
            InitializeComponent();
            this.CenterToParent();
            this.cam = cam;
            this.InspNo = (int)cam;
            panelOuterLeft.Controls.Add(panelDataLeft);
            panelOuterRight.Controls.Add(panelDataRight);
            panelDataLeft.Location = panelOuterLeft.Location;
            btnResume.Enabled = false;
            Vision.OKPointAddCallback += Vision_donePointAddCallback;
            Vision.SelectedPointAddCallback += Vision_selectedPointAddCallBack;
            Vision.NGPointAddCallback += Vision_missedPointAddCallback;
            Vision.generateMapCallback += Vision_generateMapCallback;

            if (cam == CAMERA.UNDER || cam == CAMERA.PICKER) btnCalDual.Visible = false;
            #region CANVAS_BUFFER
            this.DoubleBuffered = true;
            canvasLeft = new Bitmap(panelDataLeft.Width, panelDataLeft.Height);
            gCanvasLeft = Graphics.FromImage(canvasLeft);
            gCanvasLeft.SmoothingMode = SmoothingMode.AntiAlias;
            panelDataLeft.BackgroundImage = canvasLeft;
            typeof(Panel).InvokeMember("DoubleBuffered",
                System.Reflection.BindingFlags.SetProperty |
                System.Reflection.BindingFlags.Instance |
                System.Reflection.BindingFlags.NonPublic,
                null, panelDataLeft, new object[] { true });

            canvasRight = new Bitmap(panelDataRight.Width, panelDataRight.Height);
            gCanvasRight = Graphics.FromImage(canvasRight);
            gCanvasRight.SmoothingMode = SmoothingMode.AntiAlias;
            panelDataRight.BackgroundImage = canvasRight;
            typeof(Panel).InvokeMember("DoubleBuffered",
             System.Reflection.BindingFlags.SetProperty |
             System.Reflection.BindingFlags.Instance |
             System.Reflection.BindingFlags.NonPublic,
             null, panelDataRight, new object[] { true });
            #endregion

            chartPixelData = new System.Windows.Forms.DataVisualization.Charting.Chart[2] {chartPixelLeft, chartRightPixel };
        }
        #endregion

        #region MAP_DATA_CALLBACK_EVENT
        private void Vision_generateMapCallback(int xCount, int yCount)
        {
            try
            {
                cols = xCount;
                rows = yCount;
                cellSize = 10;
                if (selectedToolType == TOOL_TYPE.LEFT || selectedToolType == TOOL_TYPE.MAX)
                {
                    panelDataLeft.Invoke((MethodInvoker)(() =>
                    {
                        panelDataLeft.Invalidate();
                        panelDataLeft.Width = cols * cellSize + UI_PANEL_OFFSET;
                        panelDataLeft.Height = rows * cellSize + UI_PANEL_OFFSET;

                        canvasLeft = new Bitmap(panelDataLeft.Width, panelDataLeft.Height);
                        gCanvasLeft = Graphics.FromImage(canvasLeft);
                        gCanvasLeft.SmoothingMode = SmoothingMode.AntiAlias;
                        panelDataLeft.BackgroundImage = canvasLeft;
                    }));
                }

                if (selectedToolType == TOOL_TYPE.RIGHT || selectedToolType == TOOL_TYPE.MAX)
                {
                    panelDataRight.Invoke((MethodInvoker)(() =>
                    {
                        panelDataRight.Invalidate();
                        panelDataRight.Width = cols * cellSize + UI_PANEL_OFFSET;
                        panelDataRight.Height = rows * cellSize + UI_PANEL_OFFSET;

                        canvasRight = new Bitmap(panelDataRight.Width, panelDataRight.Height);
                        gCanvasRight = Graphics.FromImage(canvasRight);
                        gCanvasRight.SmoothingMode = SmoothingMode.AntiAlias;
                        panelDataRight.BackgroundImage = canvasRight;
                    }));
                }
            }
            catch (Exception ex) { }
        }

        private void Vision_missedPointAddCallback(TOOL_TYPE toolType, CalibPoint point)
        {
            DrawPoint(toolType ,point, cam);
            panelDataLeft.Invalidate();
            panelDataRight.Invalidate();
        }

        private void Vision_donePointAddCallback(TOOL_TYPE toolType, CalibPoint point)
        {
            DrawPoint(toolType ,point, cam);
            panelDataLeft.Invalidate();
            panelDataRight.Invalidate();
        }

        private void Vision_selectedPointAddCallBack(TOOL_TYPE toolType, CalibPoint selectedPoint)
        {
            DrawPoint(toolType, selectedPoint, cam, bSelected:true);
            panelDataLeft.Invalidate();
            panelDataRight.Invalidate();
        }
        #endregion

        #region POINT_RELATED_FUNCTION
        private void DrawPoint(TOOL_TYPE toolType, CalibPoint point)
        {
            Brush brush = (point.Xpx != Vision.INVALID_DATA && point.Ypx != Vision.INVALID_DATA)
                          ? Brushes.LimeGreen
                          : Brushes.OrangeRed;
            Pen pen = new Pen(Color.Black, 1);

            var p = MMToPixel(toolType, point.Xmm, point.Ymm);
            if (toolType == TOOL_TYPE.LEFT)
            {
                gCanvasLeft.FillRectangle(brush, (int)p.x, (int)p.y, cellSize, cellSize);
                gCanvasLeft.DrawRectangle(pen, (int)p.x, (int)p.y, cellSize, cellSize);
            }
            else if (toolType == TOOL_TYPE.RIGHT)
            {
                gCanvasRight.FillRectangle(brush, (int)p.x, (int)p.y, cellSize, cellSize);
                gCanvasRight.DrawRectangle(pen, (int)p.x, (int)p.y, cellSize, cellSize);
            }
        }
        private void DrawPoint(TOOL_TYPE toolType, CalibPoint point, CAMERA cam, bool bSelected = false)
        {
            /*            Brush brush = (point.Xpx != Vision.INVALID_DATA && point.Ypx != Vision.INVALID_DATA)
                                      ? Brushes.LimeGreen
                                      : Brushes.OrangeRed;*/
            Brush brush = null;
            if(point.Xpx != Vision.INVALID_DATA && point.Ypx != Vision.INVALID_DATA)
            {
                if(Math.Abs(Math.Round(Vision.GetPredictedXYpx(cam, toolType, point.Xmm, point.Ymm).Xpx, 3) - Math.Round(point.Xpx, 3)) > diffPixel
                    || Math.Abs(Math.Round(Vision.GetPredictedXYpx(cam, toolType, point.Xmm, point.Ymm).Ypx, 3) - Math.Round(point.Ypx, 3)) > diffPixel)
                {
                    brush = Brushes.Yellow;
                }
                else
                {
                    brush = Brushes.LimeGreen;
                }

                if (bSelected) brush = Brushes.Purple;
            }
            else
            {
                brush = Brushes.OrangeRed;
            }
            Pen pen = new Pen(Color.Black, 1);

            var p = MMToPixel(toolType, point.Xmm, point.Ymm);
            if (toolType == TOOL_TYPE.LEFT)
            {
                gCanvasLeft.FillRectangle(brush, (int)p.x, (int)p.y, cellSize, cellSize);
                gCanvasLeft.DrawRectangle(pen, (int)p.x, (int)p.y, cellSize, cellSize);
            }
            else if (toolType == TOOL_TYPE.RIGHT)
            {
                gCanvasRight.FillRectangle(brush, (int)p.x, (int)p.y, cellSize, cellSize);
                gCanvasRight.DrawRectangle(pen, (int)p.x, (int)p.y, cellSize, cellSize);
            }
        }

        private (double x, double y) MMToPixel(TOOL_TYPE toolType, double mmX, double mmY)
        {
            var (startX, startY, endX, endY, step) = GetCalTeachedValue(toolType, true);

            double pixelPerMM = cellSize / step;
            double snapStartX = Math.Min(startX, endX);
            double snapStartY = Math.Min(startY, endY);

            double originY = (toolType == TOOL_TYPE.LEFT ? panelDataLeft.Height : panelDataRight.Height) - UI_PANEL_OFFSET;
            double px = (mmX - snapStartX) * pixelPerMM;
            double py = originY - (mmY - snapStartY) * pixelPerMM;

            return (px, py);
        }


        private (double mmX, double mmY) PixelToMM(TOOL_TYPE toolType, int px, int py)
        {
            (double startX, double startY, double endX, double endY, double step) = GetCalTeachedValue(toolType, true);

            double pixelPerMM = cellSize / step;
            double snapStartX = Math.Min(startX, endX);
            double snapStartY = Math.Min(startY, endY);

            double originY = (toolType == TOOL_TYPE.LEFT ? panelDataLeft.Height : panelDataRight.Height) - UI_PANEL_OFFSET;
            double mmX = px / pixelPerMM + snapStartX;
            double mmY = (originY - py) / pixelPerMM + snapStartY;

            return (mmX, mmY);
        }

        private void panelDataLeft_MouseClickEvent(object sender, MouseEventArgs e)
        {
            int realX = e.X ;
            int realY = e.Y ;
            int col = realX / cellSize;
            int row = realY / cellSize;
            (double Xmm, double Ymm) = PixelToMM(TOOL_TYPE.LEFT, col * cellSize, row * cellSize);
            CalibPoint point = Vision.GetSelectedPointInMap(cam, TOOL_TYPE.LEFT, Xmm, Ymm);
            if (point is null) return;
            tabCalPointDetail detail = new tabCalPointDetail(cam, TOOL_TYPE.LEFT,new List<CalibPoint> { point });
            detail.ShowDialog();
        }

        private void panelDataRight_MouseClick(object sender, MouseEventArgs e)
        {
            int realX = e.X;
            int realY = e.Y;
            int col = realX / cellSize;
            int row = realY / cellSize;
            (double Xmm, double Ymm) = PixelToMM(TOOL_TYPE.RIGHT, col * cellSize, row * cellSize);
            CalibPoint point = Vision.GetSelectedPointInMap(cam, TOOL_TYPE.RIGHT, Xmm, Ymm);
            if (point is null) return;
            tabCalPointDetail detail = new tabCalPointDetail(cam, TOOL_TYPE.RIGHT,new List<CalibPoint> { point });
            detail.ShowDialog();
        }

        private (double startX, double startY, double endX, double endY, double step) GetCalTeachedValue(TOOL_TYPE toolType, bool useSnap = false)
        {
            double step = 0, startX = 0, startY = 0, endX = 0, endY = 0;
            double length = 0;
            if (cam == CAMERA.JIG)
            {
                step = Machine.param.Calibration(CALIBRATION.JIG_CAL_MAP_STEP);
                startX = Machine.param.Calibration(CALIBRATION.JIG_CAL_MAP_START_POS_X);
                startY = Machine.param.Calibration(CALIBRATION.JIG_CAL_MAP_START_POS_Y);
                endX = Machine.param.Calibration(CALIBRATION.JIG_CAL_MAP_END_POS_X);
                endY = Machine.param.Calibration(CALIBRATION.JIG_CAL_MAP_END_POS_Y);
                length = Machine.param.Option(ParameterDefine.OPTION.VISION_CAL_CENTER_LENGTH_JIG);
            }
            else if (cam == CAMERA.UNDER)
            {
                if (toolType == TOOL_TYPE.LEFT)
                {
                    startX = Machine.param.Calibration(CALIBRATION.UNDER_LEFT_CAL_MAP_START_POS_X);
                    startY = Machine.param.Calibration(CALIBRATION.UNDER_LEFT_CAL_MAP_START_POS_Y);
                    endX = Machine.param.Calibration(CALIBRATION.UNDER_LEFT_CAL_MAP_END_POS_X);
                    endY = Machine.param.Calibration(CALIBRATION.UNDER_LEFT_CAL_MAP_END_POS_Y);
                }
                else if (toolType == TOOL_TYPE.RIGHT)
                {
                    startX = Machine.param.Calibration(CALIBRATION.UNDER_RIGHT_CAL_MAP_START_POS_X);
                    startY = Machine.param.Calibration(CALIBRATION.UNDER_RIGHT_CAL_MAP_START_POS_Y);
                    endX = Machine.param.Calibration(CALIBRATION.UNDER_RIGHT_CAL_MAP_END_POS_X);
                    endY = Machine.param.Calibration(CALIBRATION.UNDER_RIGHT_CAL_MAP_END_POS_Y);
                }
                step = Machine.param.Calibration(CALIBRATION.UNDER_CAL_MAP_STEP);
                length = Machine.param.Option(ParameterDefine.OPTION.VISION_CAL_CENTER_LENGTH_UNDER);
            }
            else if (cam == CAMERA.PICKER)
            {
                if (toolType == TOOL_TYPE.LEFT)
                {
                    startX = Machine.param.Calibration(CALIBRATION.PICKER_LEFT_CAL_MAP_START_POS_X);
                    startY = Machine.param.Calibration(CALIBRATION.PICKER_LEFT_CAL_MAP_START_POS_Y);
                    endX = Machine.param.Calibration(CALIBRATION.PICKER_LEFT_CAL_MAP_END_POS_X);
                    endY = Machine.param.Calibration(CALIBRATION.PICKER_LEFT_CAL_MAP_END_POS_Y);
                }
                else if (toolType == TOOL_TYPE.RIGHT)
                {
                    startX = Machine.param.Calibration(CALIBRATION.PICKER_RIGHT_CAL_MAP_START_POS_X);
                    startY = Machine.param.Calibration(CALIBRATION.PICKER_RIGHT_CAL_MAP_START_POS_Y);
                    endX = Machine.param.Calibration(CALIBRATION.PICKER_RIGHT_CAL_MAP_END_POS_X);
                    endY = Machine.param.Calibration(CALIBRATION.PICKER_RIGHT_CAL_MAP_END_POS_Y);
                }
                step = Machine.param.Calibration(CALIBRATION.PICKER_CAL_MAP_STEP);
                length = Machine.param.Option(ParameterDefine.OPTION.VISION_CAL_CENTER_LENGTH_PICKER);
            }
            else if (cam == CAMERA.TRAY)
            {
                step = Machine.param.Calibration(CALIBRATION.TRAY_CAL_MAP_STEP);
                startX = Machine.param.Calibration(CALIBRATION.TRAY_CAL_MAP_START_POS_X);
                startY = Machine.param.Calibration(CALIBRATION.TRAY_CAL_MAP_START_POS_Y);
                endX = Machine.param.Calibration(CALIBRATION.TRAY_CAL_MAP_END_POS_X);
                endY = Machine.param.Calibration(CALIBRATION.TRAY_CAL_MAP_END_POS_Y);
                length = Machine.param.Option(ParameterDefine.OPTION.VISION_CAL_CENTER_LENGTH_TRAY);
            }
            if (useSnap)
            {
                startX = Vision.Snap(startX, step);
                startY = Vision.Snap(startY, step);
                endX = Vision.Snap(endX, step);
                endY = Vision.Snap(endY, step);
            }
            return (startX, startY - length, endX, endY - length, step);
        }
        #endregion

        #region AUTO_CAL_FUNCTION
        private void btnCalLeft_ClickEvent(object sender, EventArgs e)
        {
            Dlg_MessageBox formErr = new Dlg_MessageBox(EMESSAGEBOX.QUESTION, "Do you want to do Cal Left? \n Current data will be cleared before starting Calibration.");
            formErr.TopLevel = true;
            formErr.TopMost = true;
            if (formErr.ShowDialog() == DialogResult.No) return;

            isMappingCalStarted = true;
            btnResume.Enabled = true;
            LogUtil.Instance.Log(LOG_TYPE.UI, $"{cam} Mapping Left Calibration Button Click", CONTENT_TYPE.INFO);
            int targetCam = Vision.inspection.InspInfo[InspNo].targetCam;
            Vision.cogTool[targetCam].m_cogRecordDisplay = cogRecordDisplay1;
            var proc = Machine.proclist[(int)Machine.PROCESS.ASSEMBLER];
            this.selectedToolType = TOOL_TYPE.LEFT;
            switch (cam)
            {
                case CAMERA.JIG:
                    if (!CheckCalInterlock()) return;
                    proc = Machine.proclist[(int)Machine.PROCESS.ASSEMBLER];
                    ((ProcessAssembler)proc).ResetPauseVar();
                    proc.SetHeadTarget(0);
                    proc.SetMessage((int)ProcessAssembler.MSG.MSG_MAPPING_CAL_JIG_SINGLE);
                    break;
                case CAMERA.UNDER:
                    proc = Machine.proclist[(int)Machine.PROCESS.ASSEMBLER];
                    ((ProcessAssembler)proc).ResetPauseVar();
                    proc.SetHeadTarget(0);
                    proc.SetMessage((int)ProcessAssembler.MSG.MSG_MAPPING_CAL_UNDER_SINGLE);
                    break;
                case CAMERA.TRAY:
                    proc = Machine.proclist[(int)Machine.PROCESS.PROD_LOADER];
                    ((ProcessProdLoader)proc).ResetPauseVar();
                    proc.SetHeadTarget(0);
                    proc.SetMessage((int)ProcessProdLoader.MSG.MSG_MAPPING_CAL_TRAY_SINGLE);
                    break;
                case CAMERA.PICKER:
                    proc = Machine.proclist[(int)Machine.PROCESS.PROD_LOADER];
                    ((ProcessProdLoader)proc).ResetPauseVar();
                    proc.SetHeadTarget(0);
                    proc.SetMessage((int)ProcessProdLoader.MSG.MSG_MAPPING_CAL_PICKER_SINGLE);
                    break;
            }
        }

        private void btnCalRight_ClickEvent(object sender, EventArgs e)
        {
            Dlg_MessageBox formErr = new Dlg_MessageBox(EMESSAGEBOX.QUESTION, "Do you want to do Cal Right? \n Current data will be cleared before starting Calibration.");
            formErr.TopLevel = true;
            formErr.TopMost = true;
            if (formErr.ShowDialog() == DialogResult.No) return;

            isMappingCalStarted = true;
            btnResume.Enabled = true;
            LogUtil.Instance.Log(LOG_TYPE.UI, $"{cam} Mapping Right Calibration Button Click", CONTENT_TYPE.INFO);
            int targetCam = Vision.inspection.InspInfo[InspNo].targetCam;
            Vision.cogTool[targetCam].m_cogRecordDisplay = cogRecordDisplay1;
            var proc = Machine.proclist[(int)Machine.PROCESS.ASSEMBLER];
            this.selectedToolType = TOOL_TYPE.RIGHT;
            switch (cam)
            {
                case CAMERA.JIG:
                    if (!CheckCalInterlock()) return;
                    proc = Machine.proclist[(int)Machine.PROCESS.ASSEMBLER];
                    ((ProcessAssembler)proc).ResetPauseVar();
                    proc.SetHeadTarget(1);
                    proc.SetMessage((int)ProcessAssembler.MSG.MSG_MAPPING_CAL_JIG_SINGLE);
                    break;
                case CAMERA.UNDER:
                    proc = Machine.proclist[(int)Machine.PROCESS.ASSEMBLER];
                    ((ProcessAssembler)proc).ResetPauseVar();
                    proc.SetHeadTarget(1);
                    proc.SetMessage((int)ProcessAssembler.MSG.MSG_MAPPING_CAL_UNDER_SINGLE);
                    break;
                case CAMERA.TRAY:
                    proc = Machine.proclist[(int)Machine.PROCESS.PROD_LOADER];
                    ((ProcessProdLoader)proc).ResetPauseVar();
                    proc.SetHeadTarget(1);
                    proc.SetMessage((int)ProcessProdLoader.MSG.MSG_MAPPING_CAL_TRAY_SINGLE);
                    break;
                case CAMERA.PICKER:
                    proc = Machine.proclist[(int)Machine.PROCESS.PROD_LOADER];
                    ((ProcessProdLoader)proc).ResetPauseVar();
                    proc.SetHeadTarget(1);
                    proc.SetMessage((int)ProcessProdLoader.MSG.MSG_MAPPING_CAL_PICKER_SINGLE);
                    break;
            }
        }

        private async void btnCalDual_ClickEvent(object sender, EventArgs e)
        {
            Dlg_MessageBox formErr = new Dlg_MessageBox(EMESSAGEBOX.QUESTION, "Do you want to do Cal Dual? \n Current data will be cleared before starting Calibration.");
            formErr.TopLevel = true;
            formErr.TopMost = true;
            if (formErr.ShowDialog() == DialogResult.No) return;

            isMappingCalStarted = true;
            btnResume.Enabled = true;
            LogUtil.Instance.Log(LOG_TYPE.UI, $"{cam} Mapping Dual Calibration Button Click", CONTENT_TYPE.INFO);
            int targetCam = Vision.inspection.InspInfo[InspNo].targetCam;
            Vision.cogTool[targetCam].m_cogRecordDisplay = cogRecordDisplay1;
            var proc = Machine.proclist[(int)Machine.PROCESS.ASSEMBLER];
            switch (cam)
            {
                case CAMERA.JIG:
                    if (!CheckCalInterlock()) return;
                    this.selectedToolType = TOOL_TYPE.MAX;
                    ((ProcessAssembler)proc).ResetPauseVar();
                    proc.SetHeadTarget(1);
                    proc.SetMessage((int)ProcessAssembler.MSG.MSG_MAPPING_CAL_JIG_DUAL);
                    break;
                case CAMERA.UNDER:
                    break;
                case CAMERA.TRAY:
                    this.selectedToolType = TOOL_TYPE.MAX;
                    proc = Machine.proclist[(int)Machine.PROCESS.PROD_LOADER];
                    ((ProcessProdLoader)proc).ResetPauseVar();
                    proc.SetHeadTarget(1);
                    proc.SetMessage((int)ProcessProdLoader.MSG.MSG_MAPPING_CAL_TRAY_DUAL);
                    break;
            }
        }

        private bool CheckCalInterlock()
        {
            uint IsJigWorkDetectIn = 0, IsJigWorkDetectOut = 0;
            Machine.IO.GetIn((int)DI.JIG_WORK_DETECT_IN, ref IsJigWorkDetectIn);
            Machine.IO.GetIn((int)DI.JIG_WORK_DETECT_OUT, ref IsJigWorkDetectOut);

            if (IsJigWorkDetectIn == 1 || IsJigWorkDetectOut == 1)
            {
                MessageBox.Show("Check JIG", "[Interlock]", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }
            return true;
        }
        #endregion

        #region VPP_EDIT_FUNCTION
        private void btnEditVppLEft_ClickEvent(object sender, EventArgs e)
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

            var jobEdit = new Form_ToolBlockEdit(filepath, image, "CogToolBlock1");
            jobEdit.ShowDialog();
            Vision.cogToolBlkCheckerBoard[targetCam].ReadBlock(filepath);
        }

        private void btnEditVppRight_ClickEvent(object sender, EventArgs e)
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

            var jobEdit = new Form_ToolBlockEdit(filepath, image, "CogToolBlock2");
            jobEdit.ShowDialog();
            Vision.cogToolBlkCheckerBoard[targetCam].ReadBlock(filepath);
        }
        #endregion

        #region UI_FUNCTION
        private void timer1_Tick(object sender, EventArgs e)
        {
            DisplayPointStatus();
            StateChecker();
        }

        private void StateChecker()
        {
            bool isIDLE = false;
            var procLoader = Machine.proclist[(int)Machine.PROCESS.PROD_LOADER];
            var procAssembler = Machine.proclist[(int)Machine.PROCESS.ASSEMBLER];
            if ((procLoader.Ready() || procLoader.Error()) && !procLoader.Busy()
                && (procAssembler.Ready() || procAssembler.Error()) && !procAssembler.Busy()) isIDLE = true;
            else isIDLE = false;

            if (!isIDLE) lblStatus.Text = "Status: Running";
            else lblStatus.Text = "Status: IDLE";
            ButtonEnable(isIDLE);
        }

        private void ButtonEnable(bool enable)
        {
            tableLayoutPanel13.Enabled = enable;
            panelDataLeft.Enabled = enable;
            panelDataRight.Enabled = enable;
            tabVision.Enabled = enable;
            btnExit.Enabled = enable;
        }

        private void DisplayPointStatus()
        {
            double currentPos = 0, teachedPos = 0, toolLength = 0;
            try
            {
                int remainPoint = Vision.GetMapNIndex().mapCount - Vision.GetMapNIndex().index;
                TimeSpan t = TimeSpan.FromMilliseconds(Vision.GetTT() * remainPoint);
                lblRemainTime.Text = $"Estimated Remain Time: {(int)t.TotalHours:D2}:{t.Minutes:D2}:{t.Seconds:D2}";

                if (cam == CAMERA.JIG || cam == CAMERA.UNDER)
                    Machine.motion.GetAxisActualPos((int)AXIS.ASSEMBLER_X, ref currentPos);
                else if (cam == CAMERA.TRAY || cam == CAMERA.PICKER)
                    Machine.motion.GetAxisActualPos((int)AXIS.PROD_PICKUP_X, ref currentPos);
                teachedPos = GetCalTeachedValue(selectedToolType).endX;
                lblPos.Text = $"PosX: {Math.Round(currentPos/1000, 1)} / {Math.Round(teachedPos, 1)} mm";

                if (cam == CAMERA.JIG || cam == CAMERA.UNDER)
                    Machine.motion.GetAxisActualPos((int)AXIS.ASSEMBLER_Y, ref currentPos);
                else if (cam == CAMERA.TRAY || cam == CAMERA.PICKER)
                    Machine.motion.GetAxisActualPos((int)AXIS.PROD_PICKUP_Y, ref currentPos);

                if (cam == CAMERA.JIG)
                    toolLength = Machine.param.Option(ParameterDefine.OPTION.VISION_CAL_CENTER_LENGTH_JIG);
                else if (cam == CAMERA.UNDER)
                    toolLength = Machine.param.Option(ParameterDefine.OPTION.VISION_CAL_CENTER_LENGTH_UNDER);
                else if (cam == CAMERA.TRAY)
                    toolLength = Machine.param.Option(ParameterDefine.OPTION.VISION_CAL_CENTER_LENGTH_TRAY);
                else if (cam == CAMERA.PICKER)
                    toolLength = Machine.param.Option(ParameterDefine.OPTION.VISION_CAL_CENTER_LENGTH_PICKER);
                teachedPos = GetCalTeachedValue(selectedToolType).endY + toolLength;
                lblPos.Text += $"\nPosY: {Math.Round(currentPos/1000, 1)} / {Math.Round(teachedPos, 1)} mm";

                lblTotalPointLeft.Text = $"Total Point \n {Vision.GetMapStatus(cam, TOOL_TYPE.LEFT).target}";
                lblOKPointLeft.Text = $"OK Point \n {Vision.GetMapStatus(cam, TOOL_TYPE.LEFT).done}" +
                    $"({Math.Round((double)Vision.GetMapStatus(cam, TOOL_TYPE.LEFT).done / Vision.GetMapStatus(cam, TOOL_TYPE.LEFT).target * 100),2}%)";
                lblNGPointLeft.Text = $"NG Point \n {Vision.GetMapStatus(cam, TOOL_TYPE.LEFT).miss}" +
                    $"({Math.Round((double)Vision.GetMapStatus(cam, TOOL_TYPE.LEFT).miss / Vision.GetMapStatus(cam, TOOL_TYPE.LEFT).target * 100),2}%)";

                lblTotalPointRight.Text = $"Total Point \n {Vision.GetMapStatus(cam, TOOL_TYPE.RIGHT).target}";
                lblOKPointRight.Text = $"OK Point \n {Vision.GetMapStatus(cam, TOOL_TYPE.RIGHT).done}" +
                    $"({Math.Round((double)Vision.GetMapStatus(cam, TOOL_TYPE.RIGHT).done / Vision.GetMapStatus(cam, TOOL_TYPE.RIGHT).target * 100),2}%)";
                lblNGPointRight.Text = $"NG Point \n {Vision.GetMapStatus(cam, TOOL_TYPE.RIGHT).miss}" +
                    $"({Math.Round((double)Vision.GetMapStatus(cam, TOOL_TYPE.RIGHT).miss / Vision.GetMapStatus(cam, TOOL_TYPE.RIGHT).target * 100),2}%)";
            }
            catch (Exception ex) { }
        }
        #endregion

        #region VISION_TAB_FUNCTION
        private void button_LightOn_ClickEvent(object sender, EventArgs e)
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

        private void button_LightOff_Click(object sender, EventArgs e)
        {

            try
            {
                int targetCam = Vision.inspection.InspInfo[InspNo].targetCam;
                Vision.Camera.LightOn(targetCam, false);
            }
            catch { }
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

            if (cam == CAMERA.JIG)
            {
                var proc = Machine.proclist[(int)Machine.PROCESS.ASSEMBLER];
                proc.SetMessage((int)ProcessAssembler.MSG.MSG_SINGLE, (int)ProcessAssembler.STEP.REQUEST_JIG_CAL_PIXEL);
            }
            else if (cam == CAMERA.UNDER)
            {
                var proc = Machine.proclist[(int)Machine.PROCESS.ASSEMBLER];
                proc.SetMessage((int)ProcessAssembler.MSG.MSG_SINGLE, (int)ProcessAssembler.STEP.REQUEST_UNDER_CAL_PIXEL);
            }
            else if (cam == CAMERA.TRAY)
            {
                var proc = Machine.proclist[(int)Machine.PROCESS.PROD_LOADER];
                proc.SetMessage((int)ProcessProdLoader.MSG.MSG_SINGLE, (int)ProcessProdLoader.STEP.REQUEST_TRAY_CAL_PIXEL);
            }
            else if (cam == CAMERA.PICKER)
            {
                var proc = Machine.proclist[(int)Machine.PROCESS.PROD_LOADER];
                proc.SetMessage((int)ProcessProdLoader.MSG.MSG_SINGLE, (int)ProcessProdLoader.STEP.REQUEST_PICKER_CAL_PIXEL);
            }
        }

        private void button_LightOff_ClickEvent(object sender, EventArgs e)
        {
        }

        private void button_Test_ClickEvent(object sender, EventArgs e)
        {
            
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

        private void button_Live_ClickEvent(object sender, EventArgs e)
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

        private void textBox_RLv_Click(object sender, EventArgs e)
        {
            string[] value = { textBox_RLv.Text };
            SubForm_TenKey keyPad = new SubForm_TenKey(ref value, "RED Light(Lv)", "Lv", 0, 255, 300, 300, false);
            keyPad.ShowDialog();
            textBox_RLv.Text = value[0];
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

        private void textBox_GLv_Click(object sender, EventArgs e)
        {
            string[] value = { textBox_GLv.Text };
            SubForm_TenKey keyPad = new SubForm_TenKey(ref value, "GREEN Light(Lv)", "Lv", 0, 255, 300, 300, false);
            keyPad.ShowDialog();
            textBox_GLv.Text = value[0];
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

        private void textBox_BLv_Click(object sender, EventArgs e)
        {
            string[] value = { textBox_BLv.Text };
            SubForm_TenKey keyPad = new SubForm_TenKey(ref value, "BLUE Light(Lv)", "Lv", 0, 255, 300, 300, false);
            keyPad.ShowDialog();
            textBox_BLv.Text = value[0];
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

        private void textBox_BLv2_Click(object sender, EventArgs e)
        {
            string[] value = { textBox_BLv2.Text };
            SubForm_TenKey keyPad = new SubForm_TenKey(ref value, "BLUE Light 2(Lv)", "Lv", 0, 255, 300, 300, false);
            keyPad.ShowDialog();
            textBox_BLv2.Text = value[0];
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

        private void button_ExpSave_Click(object sender, EventArgs e)
        {
            LogUtil.Instance.Log(LOG_TYPE.DATA, $"Assembler Jig Calibration / LightSet Save Button Click", CONTENT_TYPE.INFO);

            Vision.Camera.cameraInfo[InspNo].lightSet.a = Convert.ToInt16(textBox_RLv.Text);
            Vision.Camera.cameraInfo[InspNo].lightSet.b = Convert.ToInt16(textBox_GLv.Text);
            Vision.Camera.cameraInfo[InspNo].lightSet.c = Convert.ToInt16(textBox_BLv.Text);
            Vision.Camera.cameraInfo[InspNo].lightSet.d = Convert.ToInt16(textBox_BLv2.Text);

            Vision.Camera.Write();
        }
        #endregion

        #region MAIN_FUNCTION
        private void btnPause_ClickEvent(object sender, EventArgs e)
        {
            if (cam == CAMERA.JIG || cam == CAMERA.UNDER)
            {
                ProcessAssembler proc = (ProcessAssembler)Machine.proclist[(int)Machine.PROCESS.ASSEMBLER];
                proc.PauseCal();

            }
            else if (cam == CAMERA.TRAY || cam == CAMERA.PICKER)
            {
                ProcessProdLoader proc = (ProcessProdLoader)Machine.proclist[(int)Machine.PROCESS.PROD_LOADER];
                proc.PauseCal();
            }
            Machine.EStop(false, false);
        }

        private void btnResume_ClickEvent(object sender, EventArgs e)
        {
            if (cam == CAMERA.JIG || cam == CAMERA.UNDER)
            {
                ProcessAssembler proc = (ProcessAssembler)Machine.proclist[(int)Machine.PROCESS.ASSEMBLER];
                proc.ResumeCal();

            }
            else if (cam == CAMERA.TRAY || cam == CAMERA.PICKER)
            {
                ProcessProdLoader proc = (ProcessProdLoader)Machine.proclist[(int)Machine.PROCESS.PROD_LOADER];
                proc.ResumeCal();
            }
        }

        private void btnSelectNGLeft_ClickEvent(object sender, EventArgs e)
        {
            tabCalPointDetail detail = new tabCalPointDetail(cam, TOOL_TYPE.LEFT, GetNGPointListInCalMap(cam, TOOL_TYPE.LEFT));
            detail.ShowDialog();
        }

        private void btnSelectNGRight_ClickEvent(object sender, EventArgs e)
        {
            tabCalPointDetail detail = new tabCalPointDetail(cam, TOOL_TYPE.RIGHT, GetNGPointListInCalMap(cam, TOOL_TYPE.RIGHT));
            detail.ShowDialog();
        }

        static List<double>[] pixelDataLeft = new List<double>[2] { new List<double>(), new List<double>()};
        static List<double>[] pixelDataRight= new List<double>[2] { new List<double>(), new List<double>() };
        static System.Windows.Forms.DataVisualization.Charting.Chart[] chartPixelData;
        int diffPixel = 3;
        private void nV_Button_PB_NS1_ClickEvent(object sender, EventArgs e)
        {
            //Display Real Data (mm)
            selectedToolType = TOOL_TYPE.LEFT;
            Vision.ShowCalMapDataOnUI(cam, TOOL_TYPE.LEFT);
            selectedToolType = TOOL_TYPE.RIGHT;
            Vision.ShowCalMapDataOnUI(cam, TOOL_TYPE.RIGHT);

            //Display Pixel Data
            for (int i=0; i<2; i++)
            {
                pixelDataLeft[i].Clear();
                pixelDataRight[i].Clear();
            }
            selectedToolType = TOOL_TYPE.LEFT;
            List<CalibPoint> mapLeft = Vision.GetCalMapData(cam, selectedToolType);
            foreach(var item in mapLeft)
            {
                pixelDataLeft[0].Add(Math.Round(item.Xpx,3));
                pixelDataLeft[1].Add(Math.Round(item.Ypx,3));
            }


            selectedToolType = TOOL_TYPE.RIGHT;
            List<CalibPoint> mapRight = Vision.GetCalMapData(cam, selectedToolType);
            foreach (var item in mapRight)
            {
                pixelDataRight[0].Add(Math.Round(item.Xpx,3));
                pixelDataRight[1].Add(Math.Round(item.Ypx,3));
            }

            DrawGraphic(cam);
        }
        private void DrawGraphic(CAMERA cam)
        {
            List<double>[] pixelList = new List<double>[2] {new List<double>(), new List<double>() };

            for (int i=0; i< (int)TOOL_TYPE.MAX; i++)
            {
                if (i == (int)TOOL_TYPE.LEFT) pixelList = pixelDataLeft;
                if (i == (int)TOOL_TYPE.RIGHT) pixelList = pixelDataRight;

                chartPixelData[i].Series.Clear();
                chartPixelData[i].ChartAreas.Clear();
                ChartArea areaPixel = new ChartArea("Mapping Pixel");
                chartPixelData[i].ChartAreas.Add(areaPixel);

                areaPixel.AxisY.Minimum = double.NaN;
                areaPixel.AxisY.Maximum = double.NaN;
                areaPixel.AxisX.Minimum = double.NaN;
                areaPixel.AxisX.Maximum = double.NaN;

                areaPixel.AxisY.Interval = 0;
                areaPixel.AxisX.Interval = 0;

                areaPixel.AxisY.ScaleView.ZoomReset();
                areaPixel.AxisX.ScaleView.ZoomReset();

                Series seriesPixel = new Series("XYPixel");
                seriesPixel.ChartType = SeriesChartType.Point;
                seriesPixel.BorderWidth = 2;

                chartPixelData[i].ChartAreas[0].RecalculateAxesScale();

                if (cam == CAMERA.TRAY)
                {
                    for (int index = 0; index < pixelList[i].Count; index++)
                    {
                        seriesPixel.Points.AddXY(pixelList[1][index], pixelList[0][index]);
                    }
                    foreach (var item in seriesPixel.Points)
                    {
                        item.ToolTip = "X=#VALX - Y=#VALY";
                    }
                }
                else
                {
                    for (int index = 0; index < pixelList[i].Count; index++)
                    {
                        seriesPixel.Points.AddXY(pixelList[0][index], pixelList[1][index]);
                    }
                    foreach (var item in seriesPixel.Points)
                    {
                        item.ToolTip = "X=#VALX - Y=#VALY";
                    }
                }

                if (seriesPixel.Points.Count > 0)
                {
                    double minX = seriesPixel.Points.Min(p => p.XValue);
                    double maxX = seriesPixel.Points.Max(p => p.XValue);
                    double minY = seriesPixel.Points.Min(p => p.YValues[0]);
                    double maxY = seriesPixel.Points.Max(p => p.YValues[0]);

                    double padding_Y = (maxY - minY) * 0.1;
                    areaPixel.AxisY.Minimum = minY - padding_Y;
                    areaPixel.AxisY.Maximum = maxY + padding_Y;

                    double padding_X = (maxY - minY) * 0.1;
                    areaPixel.AxisX.Minimum = minX - padding_X;
                    areaPixel.AxisX.Maximum = maxX + padding_X;

                }
                areaPixel.RecalculateAxesScale();
                chartPixelData[i].Series.Add(seriesPixel);
                chartPixelData[i].Series[0].IsVisibleInLegend = false;
                chartPixelData[i].Show();
            }
        }

        private void chartPixelLeft_MouseClick(object sender, MouseEventArgs e)
        {
            var result = (sender as Chart).HitTest(e.X, e.Y);

            if (result.ChartElementType != ChartElementType.DataPoint) return;

            var point = result.Series.Points[result.PointIndex];
            List<CalibPoint> mapLeft = Vision.GetCalMapData(cam, TOOL_TYPE.LEFT);

            //refresh
            Vision.ShowCalMapDataOnUI(cam, TOOL_TYPE.LEFT, mapLeft[result.PointIndex]);
            Vision.ShowCalMapDataOnUI(cam, TOOL_TYPE.RIGHT);

            ShowCalibPointModifyDialogFromChartPoint(point, cam, TOOL_TYPE.LEFT);
        }

        private void chartRightPixel_MouseClick(object sender, MouseEventArgs e)
        {
            var result = (sender as Chart).HitTest(e.X, e.Y);

            if (result.ChartElementType != ChartElementType.DataPoint) return;
                
            var point = result.Series.Points[result.PointIndex];

            List<CalibPoint> mapRight = Vision.GetCalMapData(cam, TOOL_TYPE.RIGHT);

            Vision.ShowCalMapDataOnUI(cam, TOOL_TYPE.LEFT);
            Vision.ShowCalMapDataOnUI(cam, TOOL_TYPE.RIGHT, mapRight[result.PointIndex]);


            ShowCalibPointModifyDialogFromChartPoint(point, cam, TOOL_TYPE.RIGHT);
        }


        private void ShowCalibPointModifyDialogFromChartPoint(DataPoint point, CAMERA cam, TOOL_TYPE toolType)
        {
            double x = point.XValue;
            double y = point.YValues[0];

            double Xmm = 0.0;
            double Ymm = 0.0;

            List<CalibPoint> data = Vision.GetCalMapData(cam, toolType);
            foreach (var item in data)
            {
                if (x == Math.Round(item.Xpx, 3) && y == Math.Round(item.Ypx, 3))
                {
                    Xmm = item.Xmm;
                    Ymm = item.Ymm;

                    break;
                }
            }

            CalibPoint point_Selected = Vision.GetSelectedPointInMap(cam, toolType, Xmm, Ymm);
            if (point_Selected is null) return;
            tabCalPointDetail detail = new tabCalPointDetail(cam, toolType, new List<CalibPoint> { point_Selected });
            detail.ShowDialog();
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            diffPixel = (int)numericUpDown1.Value;
            selectedToolType = TOOL_TYPE.LEFT;
            Vision.ShowCalMapDataOnUI(cam, TOOL_TYPE.LEFT);
            selectedToolType = TOOL_TYPE.RIGHT;
            Vision.ShowCalMapDataOnUI(cam, TOOL_TYPE.RIGHT);
        }

        private List<CalibPoint> GetNGPointListInCalMap(CAMERA cam, TOOL_TYPE toolType)
        {
            List<CalibPoint> data = Vision.GetCalMapData(cam, toolType);
            List<CalibPoint> ngList = new List<CalibPoint>();
            foreach (CalibPoint point in data)
            {
                if (point.Xpx == Vision.INVALID_DATA || point.Ypx == Vision.INVALID_DATA) ngList.Add(point);
            }
            return ngList;
        }

        private void nV_Button_PB_NS_Exit_ClickEvent(object sender, EventArgs e)
        {
            string message = "Do you want to close? ";
            message += isMappingCalStarted ? "\n Calibration Data will be save automatically" : string.Empty;
            Dlg_MessageBox formErr = new Dlg_MessageBox(EMESSAGEBOX.QUESTION, message);
            formErr.TopLevel = true;
            formErr.TopMost = true;

            if (formErr.ShowDialog() == DialogResult.Yes)
            {
                Vision.OKPointAddCallback -= Vision_donePointAddCallback;
                Vision.NGPointAddCallback -= Vision_missedPointAddCallback;
                Vision.generateMapCallback -= Vision_generateMapCallback;
                if (isMappingCalStarted)
                {
                    Vision.SaveCalibList(cam, TOOL_TYPE.LEFT);
                    Vision.SaveCalibList(cam, TOOL_TYPE.RIGHT);
                }
                this.Close();
            }
        }
        #endregion

    }
}
