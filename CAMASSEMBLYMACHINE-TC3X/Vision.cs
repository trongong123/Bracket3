using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Cognex.VisionPro;
using TopEng.Utils;
using TopEng.Type;
using TopEng.Vision;
using CAMASSEMBLYMACHINE.Define;
using System.Windows.Forms;
using CAMASSEMBLYMACHINE.Process;
using TopEng.Vision.Camera;
using System.Threading;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.IO;
using static CAMASSEMBLYMACHINE.Define.SystemDefine;
using Cognex.VisionPro.ImageFile;
using System.Diagnostics;
using Cognex.VisionPro.CalibFix;

namespace CAMASSEMBLYMACHINE
{
    public class CalibPoint
    {
        public double Xmm { get; set; } = 0;
        public double Ymm { get; set; } = 0;
        public double Xpx { get; set; } = 0;
        public double Ypx { get; set; } = 0;
        public CalibPoint() { }
        public CalibPoint(double xmm, double ymm, double xpx, double ypx)
        {
            Xmm = xmm;
            Ymm = ymm;
            Xpx = xpx;
            Ypx = ypx;
        }
    }

    public class PixelSize
    {
        public double sizeX { get; set; } = 0;
        public double sizeY { get; set; } = 0;
        public double offsetX { get; set; } = 0;
        public double offsetY { get; set; } = 0;
        public PixelSize() { }
        public PixelSize(double xmm, double ymm, double xpx, double ypx)
        {
            sizeX = xmm;
            sizeY = ymm;
            offsetX = xpx;
            offsetY = ypx;
        }
    }

    public class Vision
    {
        public static double INVALID_DATA = -999999999;
        public static CameraControl Camera = new CameraControl();
        public static List<CognexTool> cogTool = new List<CognexTool>();
        public static List<CameraCalibration> calibTool = new List<CameraCalibration>();
        public static Inspection inspection = new Inspection();
        private static object lockObj = new object();
        private static Point2d[,] lastAlignData = new Point2d[(int)CAMERA.MAX, (int)TOOL_TYPE.MAX];
        private static List<(double x, double y)> path = new List<(double x, double y)>();
        private static Stopwatch calTactTime = new Stopwatch();
        private static int calPointIndex = 0;
        public static double[,] calibMapRatioX = new double[(int)CAMERA.MAX, (int)TOOL_TYPE.MAX];
        public static double[,] calibMapRatioY = new double[(int)CAMERA.MAX, (int)TOOL_TYPE.MAX];
        private static List<CalibPoint>[,] calibMap = new List<CalibPoint>[(int)CAMERA.MAX, (int)TOOL_TYPE.MAX];
        public static double[,] cameraAngle = new double[(int)CAMERA.MAX, (int)TOOL_TYPE.MAX];
        public static PixelSize[,] pixelSize = new PixelSize[(int)CAMERA.MAX, (int)TOOL_TYPE.MAX];
        public static TOOL_TYPE currentCalToolType;
        private static object RunLockObject = new object();
        private static StopWatch stopWatch = new StopWatch();
        public delegate void MessageCallbackEvent(string msg);
        public static event MessageCallbackEvent m_cbMessageEventFunc;

        public delegate void DonePointAddCallbackEvent(TOOL_TYPE toolType, CalibPoint point);
        public static event DonePointAddCallbackEvent OKPointAddCallback;

        public delegate void MissedPointAddCallbackEvent(TOOL_TYPE toolType, CalibPoint point);
        public static event MissedPointAddCallbackEvent NGPointAddCallback;

        public delegate void SelectedPointAddCallbackEvent(TOOL_TYPE toolType, CalibPoint point);
        public static event SelectedPointAddCallbackEvent SelectedPointAddCallback;

        public delegate void GenerateMapCallbackEvent(int xCount, int yCount);
        public static event GenerateMapCallbackEvent generateMapCallback;

        public static List<CogToolBlockHandler> cogToolBlkCheckerBoard = new List<CogToolBlockHandler>();
        public static bool useVision = false;

        #region GENERAL_FUNCTION
        public static void Initialize()
        {
            try
            {
                Camera.Open();

                if (SystemDefine.IS_NOTEBOOK_MODE)
                    SystemDefine.CamCount = SystemDefine.CamCountSim;
                else
                    SystemDefine.CamCount = Camera.CameraCount();

                InitVision();
                inspection.InitProcess();
            }
            catch (Exception e)
            {
                LogUtil.Instance.Log(LOG_TYPE.DEVELOPMENT, e.Message, CONTENT_TYPE.EXCEPTION);

                if (MessageBox.Show(e.Message.ToString() + Environment.NewLine + Environment.NewLine +
                    "Vision parameter setting error. The program can not be run normally. Do you want to quit?",
                    "Vision Standard Program", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    Application.ExitThread();  // 18.03.23 
                    Environment.Exit(0);
                }
            }
        }

        private static void InitVision()
        {
            if (SystemDefine.NO_VISION_KEY) return;
            for (int iCamNo = 0; iCamNo < CamCount; iCamNo++)
            {
                CogToolBlockHandler cogToolBlkProc = new CogToolBlockHandler();
                cogToolBlkProc.ReadBlock(SystemDefine.systemPath + $"\\CheckerBoard_{Camera.Name(iCamNo)}.vpp");
                cogToolBlkCheckerBoard.Add(cogToolBlkProc);

                cogTool.Add(new CognexTool(SystemDefine.capturePath, SystemDefine.deleteimageperiod));
                if (!SystemDefine.IS_NOTEBOOK_MODE)
                    Camera.grabber.cameras[iCamNo].m_cbGrabbedImageEventFunc += new HikrobotGrab.GrabbedImageEvent(cogTool[iCamNo].callbackGrabbedImageEvent);

                var newCal = new CameraCalibration(cogToolBlkProc, Camera.Name(iCamNo));

                calibMapRatioX[iCamNo, (int)TOOL_TYPE.LEFT] = Camera.RatioLeftX(iCamNo);
                calibMapRatioY[iCamNo, (int)TOOL_TYPE.LEFT] = Camera.RatioLeftY(iCamNo);
                calibMapRatioX[iCamNo, (int)TOOL_TYPE.RIGHT] = Camera.RatioRightX(iCamNo);
                calibMapRatioY[iCamNo, (int)TOOL_TYPE.RIGHT] = Camera.RatioRightY(iCamNo);
                for (int iToolType = 0; iToolType < (int)TOOL_TYPE.MAX; iToolType++)
                {
                    UpdateCalibListByRatio((CAMERA)iCamNo, (TOOL_TYPE)iToolType);
                    lastAlignData[iCamNo, iToolType] = new Point2d();
                    CheckIfMapContainSameData((CAMERA)iCamNo, (TOOL_TYPE)iToolType);
                    CalculateCameraAngleAverage((CAMERA)iCamNo, (TOOL_TYPE)iToolType);
                    CalculatePixelSize((CAMERA)iCamNo, (TOOL_TYPE)iToolType);
                }
                newCal.CamWidth = Camera.ResolutionX(iCamNo);
                newCal.CamHeight = Camera.ResolutionY(iCamNo);
                calibTool.Add(newCal);
            }
        }

        public static void Disconnect()
        {
            for (int i = 0; i < cogTool.Count; i++)
            {
                if (cogTool[i] != null)
                    cogTool[i].Disconnect();
            }
        }

        public static void Grab(int Id)
        {
            Camera.Grab(Id);
        }

        public static bool GrabCompleted(int Id)
        {
            return Camera.GrabCompleted(Id);
        }

        public static void LiveStart(int Id)
        {
            Camera.LiveStart(Id);
        }

        public static void LiveStop(int Id)
        {
            Camera.LiveStop(Id);
        }

        public static void Capture(string name, CaptureUtil.CAPTURETYPE type, bool raw)
        {
            int index = inspection.GetIndex(name);
            if (index >= 0)
                Capture(index, name, type, raw);
        }

        public static void Capture(int index, string name, CaptureUtil.CAPTURETYPE type, bool raw)
        {
            int targetCam = inspection.InspInfo[index].targetCam;
            if (cogTool[targetCam].m_cogRecordDisplay != null)
            {
                if (raw)
                {
                    cogTool[targetCam].Capture(name, type, Vision.cogTool[targetCam].m_cogImage);
                }
                else
                {
                    var image = Vision.cogTool[targetCam].m_cogRecordDisplay.CreateContentBitmap(Cognex.VisionPro.Display.CogDisplayContentBitmapConstants.Display, null, 0);
                    Bitmap bitmap = new Bitmap(image);
                    cogTool[targetCam].Capture(name, type, new CogImage24PlanarColor(bitmap));
                }
            }
            cogTool[targetCam].SetDeleteImagePeriod(SystemDefine.deleteimageperiod);
        }

        public static void CaptureByInput(string name, CaptureUtil.CAPTURETYPE type, ICogImage image)
        {
            try
            {
                int index = inspection.GetIndex(name);
                int targetCam = inspection.InspInfo[index].targetCam;
                cogTool[targetCam].Capture(name, type, image);
            }
            catch (Exception ex)
            {

            }
        }

        public static void PauseTrainCapture(string name, bool pause)
        {
            int index = inspection.GetIndex(name);
            if (index >= 0)
                PauseTrainCapture(index, pause);
        }

        public static void PauseTrainCapture(int index, bool pause)
        {
            inspection.recipeData[index].captureProc.pauseSave = pause;
        }

        public static void ClearTrainCaptureQueue(string name)
        {
            int index = inspection.GetIndex(name);
            if (index >= 0)
                ClearTrainCaptureQueue(index);
        }

        public static void ClearTrainCaptureQueue(int index)
        {
            inspection.recipeData[index].captureProc.Clear();
        }

        public static void DeleteCaptureFile()
        {
            for (int i = 0; i < inspection.recipeData.Count; i++)
                inspection.recipeData[i].captureProc.DeleteOldFiles();
        }

        public static bool RunGrab(string name, bool noGrab = false, bool noLightSet = false)
        {
            int index = inspection.GetIndex(name);
            if (index < 0)
            {
                return false;
            }

            return RunGrab(index, noGrab, noLightSet);
        }

        public static bool RunGrab(int index, bool noGrab = false, bool noLightSet = false)
        {
            if (useVision)
                return false;

            int targetCam = inspection.InspInfo[index].targetCam;

            if (!noLightSet)
                Camera.LightSet(targetCam, inspection.recipeData[index].inspInfo.lightSet);

            if (!noGrab)
            {
                Vision.Camera.LightOn(targetCam, true);
                Vision.Grab(targetCam);
                if (!GrabCompleted(targetCam))
                {
                    useVision = false;
                    return false;
                }

                if (SystemDefine.UseLightOff)
                    Vision.Camera.LightOn(targetCam, false);
            }

            useVision = false;
            return true;
        }

        public static bool Run(string name, bool SendData, bool noGrab = false, bool SaveTrainImage = false, double overlap = 500, double angleMin = -180, double angleMax = 180)
        {
            int index = inspection.GetIndex(name);
            if (index < 0)
            {
                return false;
            }
            return Run(index, SendData, noGrab, SaveTrainImage, overlap, angleMin, angleMax);
        }

        public static bool Run(int index, bool SendData, bool noGrab = false, bool SaveTrainImage = false, double overlap = 500, double angleMin = -180, double angleMax = 180)
        {
            if (!inspection.recipeData[index].ThreadStateIDLE())
                return false;

            try
            {
                int targetCam = inspection.InspInfo[index].targetCam;
                if (!inspection.recipeData[index].Run(cogTool[targetCam].m_cogImage, SendData, SaveTrainImage, overlap, angleMin, angleMax))
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
            return true;
        }

        public static bool RunCompleted(string name)
        {
            int index = inspection.GetIndex(name);
            if (index < 0)
                return true;

            return RunCompleted(index);
        }

        public static bool RunCompleted(int index)
        {
            StopWatch stopWatch = new StopWatch();
            stopWatch.Reset();
            bool ret = true;

            while (true)
            {
                if (stopWatch.Elapsed > 1000)
                {
                    ret = false;
                    break;
                }
                if (inspection.recipeData[index].ThreadStateIDLE())
                    break;

                Thread.Sleep(10);
            }

            return ret;
        }

        public static void Calibration(string nameCam, int No, Point2d pos, bool SendData, bool noGrab = false)
        {
            int targetCam = Camera.GetIndex(nameCam);
            if (targetCam < 0)
                return;

            Calibration(targetCam, No, pos, SendData, noGrab);
        }

        public static void Calibration(int targetCam, int No, Point2d pos, bool SendData, bool noGrab = false)
        {
            lock (RunLockObject)
            {
                if (!noGrab)
                {
                    Camera.LightSet(targetCam, Camera.cameraInfo[targetCam].lightSet);
                    Vision.Camera.LightOn(targetCam, true);

                    cogTool[targetCam].m_cogImage = null;

                    Vision.Grab(targetCam);
                    if (!GrabCompleted(targetCam))
                        return;
                }

                calibTool[targetCam].Run(cogTool[targetCam].m_cogImage, No, pos, SendData);
                CalibrationCompleted(targetCam);
            }
        }

        public static bool CalibrationCompleted(string name)
        {
            int targetCam = Camera.GetIndex(name);
            if (targetCam < 0)
                return true;

            return CalibrationCompleted(targetCam);
        }

        public static bool CalibrationCompleted(int targetCam)
        {
            StopWatch stopWatch = new StopWatch();
            stopWatch.Reset();
            bool ret = true;

            while (true)
            {
                if (stopWatch.Elapsed > 1000)
                {
                    ret = false;
                    break;
                }
                if (calibTool[targetCam].ThreadStateIDLE())
                    break;

                Thread.Sleep(10);
            }
            ICogRecord rec = calibTool[targetCam].cogToolBlkProc.Records(0);
            if (currentCalToolType == TOOL_TYPE.LEFT)
                rec = calibTool[targetCam].cogToolBlkProc.Records("CogToolBlock1.CogFixtureTool1.OutputImage");
            else if (currentCalToolType == TOOL_TYPE.RIGHT)
                rec = calibTool[targetCam].cogToolBlkProc.Records("CogToolBlock2.CogFixtureTool1.OutputImage");
            else if (currentCalToolType == TOOL_TYPE.MAX)
                rec = calibTool[targetCam].cogToolBlkProc.Records("CogToolBlock1.CogPMAlignTool1.InputImage");
            if (cogTool[targetCam].m_cogRecordDisplay != null)
                cogTool[targetCam].m_cogRecordDisplay.Record = rec;

            return ret;
        }
        #endregion

        #region CAL_MAPPING_FUNCTION
        #region GET_PIXEL_VALUE
        public static Point2d GetPixelCamPos(string nameCam, TOOL_TYPE toolType)
        {
            if (!IsCalToolSearched(nameCam, toolType))
                return null;
            int targetCam = Camera.GetIndex(nameCam);
            if (targetCam < 0)
                return null;
            return calibTool[targetCam].toolPixelValue[(int)toolType];
        }

        private static bool IsCalToolSearched(string nameCam, TOOL_TYPE toolType)
        {
            int targetCam = Camera.GetIndex(nameCam);
            if (targetCam < 0)
                return false;
            if (calibTool[targetCam].toolPixelValue[(int)toolType] != null) return true;
            return false;
        }

        public static bool IsCalDataReceived(string nameCam)
        {
            int targetCam = Camera.GetIndex(nameCam);
            if (targetCam < 0)
                return false;
            return calibTool[targetCam].isReceiveData;
        }

        public static void SetHeadTarget(string nameCam, TOOL_TYPE toolType)
        {
            int targetCam = Camera.GetIndex(nameCam);
            if (targetCam < 0)
                return;
            calibTool[targetCam].SetHeadTarget(toolType);
            currentCalToolType = toolType;
        }

        public static Point2d GetCamCenter(string nameCam)
        {
            int targetCam = Camera.GetIndex(nameCam);
            if (targetCam < 0)
                return null;
            return new Point2d(0.5 * calibTool[targetCam].CamWidth, 0.5 * calibTool[targetCam].CamHeight);
        }
        #endregion

        #region MAPPING_FILE_RELATED_FUNCTION
        public static List<CalibPoint> GetCalMapData(CAMERA camera, TOOL_TYPE toolType) => calibMap[(int)camera, (int)toolType];
        public static void ShowCalMapDataOnUI(CAMERA camera, TOOL_TYPE toolType)
        {
            try
            {
                List<CalibPoint> map = GetCalMapData(camera, toolType);

                double dx = map[1].Xmm - map[0].Xmm;
                double dy = map[1].Ymm - map[0].Ymm;
                double step = Math.Abs(dx + dy);
                double minX = map.Min(p => p.Xmm);
                double maxX = map.Max(p => p.Xmm);
                double rangeX = Math.Abs(maxX - minX);
                double minY = map.Min(p => p.Ymm);
                double maxY = map.Max(p => p.Ymm);
                double rangeY = Math.Abs(maxY - minY);
                generateMapCallback.Invoke((int)(rangeX / step), (int)(rangeY / step));
                foreach (CalibPoint point in map)
                {
                    if (point.Xpx == INVALID_DATA || point.Ypx == INVALID_DATA)
                        NGPointAddCallback.Invoke(toolType, new CalibPoint(point.Xmm, point.Ymm, INVALID_DATA, INVALID_DATA));
                    else
                        OKPointAddCallback.Invoke(toolType, point);
                }
            }
            catch (Exception ex) { }
        }
        public static void ShowCalMapDataOnUI(CAMERA camera, TOOL_TYPE toolType, CalibPoint selectedPoint)
        {
            try
            {
                List<CalibPoint> map = GetCalMapData(camera, toolType);

                double dx = map[1].Xmm - map[0].Xmm;
                double dy = map[1].Ymm - map[0].Ymm;
                double step = Math.Abs(dx + dy);
                double minX = map.Min(p => p.Xmm);
                double maxX = map.Max(p => p.Xmm);
                double rangeX = Math.Abs(maxX - minX);
                double minY = map.Min(p => p.Ymm);
                double maxY = map.Max(p => p.Ymm);
                double rangeY = Math.Abs(maxY - minY);
                generateMapCallback.Invoke((int)(rangeX / step), (int)(rangeY / step));
                foreach (CalibPoint point in map)
                {
                    if (point.Xpx == INVALID_DATA || point.Ypx == INVALID_DATA)
                        NGPointAddCallback.Invoke(toolType, new CalibPoint(point.Xmm, point.Ymm, INVALID_DATA, INVALID_DATA));
                    else if(point== selectedPoint)
                    {
                        SelectedPointAddCallback.Invoke(toolType, point);
                    }
                    else 
                        OKPointAddCallback.Invoke(toolType, point);
                }
            }
            catch (Exception ex) { }
        }
        public static CalibPoint GetSelectedPointInMap(CAMERA camera, TOOL_TYPE toolType, double Xmm, double Ymm)
        {
            foreach (CalibPoint point in calibMap[(int)camera, (int)toolType])
            {
                if (Math.Abs(point.Xmm - Xmm) <= 0.01 && Math.Abs(point.Ymm - Ymm) <= 0.01) return point;
            }
            return null;
        }
        public static void AddOKPointToCalMap(CAMERA camera, TOOL_TYPE toolType, double Xmm, double ymm, double xpx, double ypx)
        {
            calibMap[(int)camera, (int)toolType].Add(new CalibPoint(Xmm, ymm, xpx, ypx));
            OKPointAddCallback.Invoke(toolType, new CalibPoint(Xmm, ymm, xpx, ypx));
        }

        public static void AddNGPointToCalMap(CAMERA camera, TOOL_TYPE toolType, double Xmm, double ymm)
        {
            calibMap[(int)camera, (int)toolType].Add(new CalibPoint(Xmm, ymm, INVALID_DATA, INVALID_DATA));
            NGPointAddCallback.Invoke(toolType, new CalibPoint(Xmm, ymm, INVALID_DATA, INVALID_DATA));
        }

        public static bool ChangePointDataInCalMap(CAMERA camera, TOOL_TYPE toolType, double Xmm, double Ymm, double Xpx, double Ypx)
        {
            try
            {
                foreach (CalibPoint point in calibMap[(int)camera, (int)toolType])
                {
                    if (Math.Abs(point.Xmm - Xmm) <= 0.01 && Math.Abs(point.Ymm - Ymm) <= 0.01)
                    {
                        point.Xpx = Xpx;
                        point.Ypx = Ypx;
                        if (point.Xpx == Vision.INVALID_DATA || point.Ypx == Vision.INVALID_DATA) NGPointAddCallback.Invoke(toolType, point);
                        else OKPointAddCallback.Invoke(toolType, point);
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex) { return false; }
        }

        public static void SaveCalibList(CAMERA camera, TOOL_TYPE toolType)
        {
            try
            {
                CheckIfMapContainSameData(camera, toolType);
                CalculateCameraAngleAverage(camera, toolType);
                CalculatePixelSize(camera, toolType);
                string filePath = GetCalibMapFilePath(camera, toolType);
                var options = new JsonSerializerOptions
                { WriteIndented = true };
                string json = JsonSerializer.Serialize(calibMap[(int)camera, (int)toolType], options);
                json = Regex.Unescape(json);
                if (!IOFunctionUtil.CheckFileExist(filePath))
                    IOFunctionUtil.CreateFileAndDirectory(filePath);
                else
                    IOFunctionUtil.CopyFile(filePath, GetCalibMapBackupFilePath(camera, toolType));
                File.WriteAllText(filePath, json);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Invalid Data Save Error! Please recheck data \n Exc:{ex}");
            }
        }

        public static void UpdateCalibListByRatio(CAMERA cam, TOOL_TYPE toolType)
        {
            calibMap[(int)cam, (int)toolType] = LoadCalibList(cam, toolType);
            double ratioX = calibMapRatioX[(int)cam, (int)toolType];
            double ratioY = calibMapRatioY[(int)cam, (int)toolType];
            double camCenterX = Camera.ResolutionX((int)cam) / 2;
            double camCenterY = Camera.ResolutionY((int)cam) / 2;
            foreach (var param in Vision.calibMap[(int)cam, (int)toolType])
            {
                param.Xpx = camCenterX + (param.Xpx - camCenterX) * ratioX;
                param.Ypx = camCenterY + (param.Ypx - camCenterY) * ratioY;
            }

        }
        public static List<CalibPoint> LoadCalibList(CAMERA cam, TOOL_TYPE toolType)
        {
            try
            {
                string filePath = GetCalibMapFilePath(cam, toolType);
                if (!IOFunctionUtil.CheckFileExist(filePath)) return new List<CalibPoint>();
                string json = File.ReadAllText(filePath);
                json = Regex.Unescape(json);
                return JsonSerializer.Deserialize<List<CalibPoint>>(json);
            }
            catch { return null; }
        }

        private static string GetCalibMapFilePath(CAMERA cam, TOOL_TYPE toolType)
        {
            return SystemDefine.systemPath + @"\CalibMap" + $@"\CalibMap_{cam}_{toolType}.json";
        }

        private static string GetCalibMapBackupFilePath(CAMERA cam, TOOL_TYPE toolType)
        {
            return SystemDefine.systemPath + @"\CalibMap" + @"\Backup" + $@"\CalibMap_{cam}_{toolType}_{DateTime.Now.ToString("yyMMdd_HHmmss")}.json";
        }

        public static void ClearBeforeMapping(CAMERA cam, TOOL_TYPE toolType)
        {
            calPointIndex = 0;
            path.Clear();
            calibMap[(int)cam, (int)toolType].Clear();
        }


        public static void GenerateMoveStep(double startX, double startY, double endX, double endY, double step)
        {
            startX = Snap(startX, step);
            startY = Snap(startY, step);
            endX = Snap(endX, step);
            endY = Snap(endY, step);
            path.Clear();
            calPointIndex = 0;
            double x = startX;
            double y = startY;
            int stepsX = (int)(Math.Abs(endX - startX) / step);
            int stepsY = (int)(Math.Abs(endY - startY) / step);
            double dx = (endX >= startX) ? step : -step;
            double dy = (endY >= startY) ? step : -step;
            bool goingUp = true;
            int xCount = 0, yCount = 0;
            for (int ix = 0; ix <= stepsX; ix++)
            {
                yCount = 0;
                for (int iy = 0; iy <= stepsY; iy++)
                {
                    yCount++;
                    path.Add((x, y));

                    if (iy < stepsY)
                        y += goingUp ? dy : -dy;
                }
                xCount++;
                goingUp = !goingUp;
                if (ix < stepsX)
                    x += dx;
                y = goingUp ? startY : endY;
            }
            generateMapCallback.Invoke(xCount, yCount);
        }

        public static double Snap(double value, double step)
        {
            return Math.Floor(value / step) * step;
        }

        public static bool IsMapCalDone() => calPointIndex == path.Count();
        public static void IncreaseCalMapIndex() => calPointIndex++;
        public static (int index, int mapCount) GetMapNIndex() => (calPointIndex, path.Count());
        public static void StartTTStopWatch() => calTactTime.Restart();
        public static void StopTTStopWatch() => calTactTime.Stop();
        public static int GetTT() => (int)calTactTime.ElapsedMilliseconds;
        public static (int done, int miss, int target) GetMapStatus(CAMERA cam, TOOL_TYPE toolType)
        {
            return (GetPointCount(cam, toolType).okCount, GetPointCount(cam, toolType).ngCount, path.Count());
        }

        private static (int okCount, int ngCount) GetPointCount(CAMERA cam, TOOL_TYPE toolType)
        {
            int ngCount = 0, okCount = 0;
            for (int i = 0; i < calibMap[(int)cam, (int)toolType].Count(); i++)
            {
                if (calibMap[(int)cam, (int)toolType][i].Xpx == INVALID_DATA
                    || calibMap[(int)cam, (int)toolType][i].Ypx == INVALID_DATA) ngCount++;
                else okCount++;
            }
            return (okCount, ngCount);
        }

        public static bool IsMissedPointReachLimit(CAMERA cam, TOOL_TYPE toolType)
        {
            int limit = (int)(path.Count() * 20 / 100); // 20%
            if (GetPointCount(cam, toolType).ngCount > limit) return true;
            return false;
        }

        public static bool IsAllCalPointOK(CAMERA cam, TOOL_TYPE toolType) => GetPointCount(cam, toolType).ngCount == 0;

        public static (double X, double Y) GetMoveStep()
        {
            if (calPointIndex < path.Count()) return path[calPointIndex];
            else return (INVALID_DATA, INVALID_DATA);
        }

        #endregion

        #region CAM_ANGLE_CALCULATOR
        public static double GetCameraAngleByLine(CAMERA cam, TOOL_TYPE toolType, double pixel, double rangePixel)
        {
            try
            {
                Dictionary<double, List<CalibPoint>> rowList = new Dictionary<double, List<CalibPoint>>();
                foreach (CalibPoint point in calibMap[(int)cam, (int)toolType])
                {
                    double roundedX = Math.Round(point.Xmm, 1);
                    if (point.Xpx >= (pixel + rangePixel) || point.Xpx <= (pixel - rangePixel)) continue;
                    if (rowList.ContainsKey(roundedX)) rowList[roundedX].Add(point);
                    else rowList.Add(roundedX, new List<CalibPoint>());
                }
                double returnValue = CalculateAngleByRowList(rowList);
                if (double.IsNaN(returnValue)) throw new Exception("Data is NaN");
                return returnValue;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        private static double CalculateCameraAngleAverage(CAMERA camera, List<CalibPoint> map)
        {
            try
            {
                if (map != null && map.Count <= 0) return 0;
                Dictionary<double, List<CalibPoint>> rowList = new Dictionary<double, List<CalibPoint>>();
                foreach (CalibPoint point in map)
                {
                    double roundedX = Math.Round(point.Xmm, 1);
                    if (rowList.ContainsKey(roundedX)) rowList[roundedX].Add(point);
                    else rowList.Add(roundedX, new List<CalibPoint>());
                }
                double returnValue = CalculateAngleByRowList(rowList);
                if (double.IsNaN(returnValue)) throw new Exception("Data is NaN");
                return returnValue;
            }
            catch (Exception e)
            {
                return 0;
            }
        }

        private static double CalculateAngleByRowList(Dictionary<double, List<CalibPoint>> rowList)
        {
            double angleSum = 0;
            int count = 0;
            foreach (List<CalibPoint> list in rowList.Values)
            {
                if (list.Count() == 0) continue;
                if (double.IsNaN(ComputeAngleFromRow(list)))
                {

                }
                angleSum += ComputeAngleFromRow(list);
                count++;
            }
            double finalAngle = angleSum / count;
            return finalAngle;
        }

        private static double ComputeAngleFromRow(List<CalibPoint> pts)
        {
            double meanX = pts.Average(p => p.Xpx);
            double meanY = pts.Average(p => p.Ypx);

            double num = 0; double den = 0;
            foreach (CalibPoint p in pts)
            {
                if (p.Xpx == Vision.INVALID_DATA || p.Ypx == Vision.INVALID_DATA) continue;
                double dx = p.Xpx - meanX;
                double dy = p.Ypx - meanY;
                dy *= -1;
                num += dx * dy;
                den += dy * dy;

            }
            double angle = Math.Atan(num / den);
            return angle;
        }

        private static void CalculateCameraAngleAverage(CAMERA camera, TOOL_TYPE toolType)
        {
            cameraAngle[(int)camera, (int)toolType] = CalculateCameraAngleAverage(camera, calibMap[(int)camera, (int)toolType]);
        }
        public static double GetCameraAngle(CAMERA cam, TOOL_TYPE toolType) => cameraAngle[(int)cam, (int)toolType];
        #endregion

        #region ALIGN_DATA_CALCULATOR
        public static (double x, double y) ConvertPxToMmByDistance(CAMERA cam, TOOL_TYPE toolType, double x, double y)
        {
            double returnX = 0, returnY = 0;
            if (Camera.RotateType((int)cam) == "DEG0")
            {
                returnX = x * pixelSize[(int)cam, (int)toolType].sizeX;
                returnY = y * pixelSize[(int)cam, (int)toolType].sizeY;
            }
            else if (Camera.RotateType((int)cam) == "DEG90")
            {
                returnX = y * pixelSize[(int)cam, (int)toolType].sizeY;
                returnY = x * pixelSize[(int)cam, (int)toolType].sizeX;
            }
            return (returnX, returnY);
        }

        public static (double x, double y) ConvertPxToMmByCoord(CAMERA cam, TOOL_TYPE toolType, double x, double y)
        {
            double returnX = 0, returnY = 0;
            if (Camera.RotateType((int)cam) == "DEG0")
            {
                returnX = x * pixelSize[(int)cam, (int)toolType].sizeX + pixelSize[(int)cam, (int)toolType].offsetX;
                returnY = y * pixelSize[(int)cam, (int)toolType].sizeY + pixelSize[(int)cam, (int)toolType].offsetY;
            }
            else if (Camera.RotateType((int)cam) == "DEG90")
            {
                returnX = y * pixelSize[(int)cam, (int)toolType].sizeY + pixelSize[(int)cam, (int)toolType].offsetY;
                returnY = x * pixelSize[(int)cam, (int)toolType].sizeX + pixelSize[(int)cam, (int)toolType].offsetX;
            }

            return (returnX, returnY);
        }

        private static void CalculatePixelSize(CAMERA cam, TOOL_TYPE toolType)
        {
            bool isCamRotate = false;
            if (Camera.RotateType((int)cam) == "DEG90") isCamRotate = true;
            else if (Camera.RotateType((int)cam) == "DEG0") isCamRotate = false;

            double sumX = 0, sumY = 0;
            double offsetX = 0, offsetY = 0;
            int countX = 0, countY = 0;
            double rowThreshold = 0.05;
            double colThreshold = 0.05;

            for (int i = 0; i < calibMap[(int)cam, (int)toolType].Count; i++)
            {
                for (int j = i + 1; j < calibMap[(int)cam, (int)toolType].Count; j++)
                {
                    if (calibMap[(int)cam, (int)toolType][i].Xpx == Vision.INVALID_DATA
                        || calibMap[(int)cam, (int)toolType][i].Ypx == Vision.INVALID_DATA) continue;

                    CalibPoint ptsI = calibMap[(int)cam, (int)toolType][i];
                    CalibPoint ptsJ = calibMap[(int)cam, (int)toolType][j];
                    double dPixel;
                    double dMm;
                    double offset;
                    if (Math.Abs(ptsI.Ymm - ptsJ.Ymm) < rowThreshold)
                    {
                        if (isCamRotate)
                        {
                            dPixel = ptsJ.Ypx - ptsI.Ypx;
                            dMm = ptsJ.Xmm - ptsI.Xmm;
                            if (Math.Abs(dPixel) > 1e-6)
                            {
                                sumY += dMm / dPixel;
                                offsetY += (ptsI.Xmm - dMm * ptsI.Ypx / dPixel);
                                countY++;
                            }
                        }
                        else
                        {
                            dPixel = ptsJ.Xpx - ptsI.Xpx;
                            dMm = ptsJ.Xmm - ptsI.Xmm;
                            if (Math.Abs(dPixel) > 1e-6)
                            {
                                sumX += dMm / dPixel;
                                offsetX += (ptsI.Xmm - dMm * ptsI.Xpx / dPixel);
                                countX++;
                            }
                        }
                    }

                    if (Math.Abs(ptsI.Xmm - ptsJ.Xmm) < colThreshold)
                    {
                        if (isCamRotate)
                        {
                            dPixel = ptsJ.Xpx - ptsI.Xpx;
                            dMm = ptsJ.Ymm - ptsI.Ymm;
                            if (Math.Abs(dPixel) > 1e-6)
                            {
                                sumX += dMm / dPixel;
                                offsetX += (ptsI.Ymm - dMm * ptsI.Xpx / dPixel);
                                countX++;
                            }
                        }
                        else
                        {
                            dPixel = ptsJ.Ypx - ptsI.Ypx;
                            dMm = ptsJ.Ymm - ptsI.Ymm;
                            if (Math.Abs(dPixel) > 1e-6)
                            {
                                sumY += dMm / dPixel;
                                offsetY += (ptsI.Ymm - dMm * ptsI.Ypx / dPixel);
                                countY++;
                            }
                        }
                    }
                }
            }

            double scaleX = sumX / countX;
            double scaleY = sumY / countY;
            double originOffsetX = offsetX / countX;
            double originOffsetY = offsetY / countY;
            pixelSize[(int)cam, (int)toolType] = new PixelSize(scaleX, scaleY, originOffsetX, originOffsetY);
        }

        public static (double x, double y) InterpolateMM(CAMERA cam, TOOL_TYPE toolType, double x, double y)
        {
            if (x <= 0 || y <= 0) return (INVALID_DATA, INVALID_DATA);
            CogCalibNPointToNPointTool temp = new CogCalibNPointToNPointTool();
            for (int i = 0; i < calibMap[(int)cam, (int)toolType].Count; i++)
            {
                temp.Calibration.AddPointPair(calibMap[(int)cam, (int)toolType][i].Xmm,
                    calibMap[(int)cam, (int)toolType][i].Ymm,
                    calibMap[(int)cam, (int)toolType][i].Xpx,
                    calibMap[(int)cam, (int)toolType][i].Ypx);
            }
            temp.Calibration.Calibrate();
            ICogTransform2D trans = temp.Calibration.GetComputedUncalibratedFromRawCalibratedTransform();
            double returnX = temp.Calibration.ComputedRMSError; double returnY = 0;
            trans.MapPoint(x, y, out returnX, out returnY);
            return (returnX, returnY);

            double acceptRate = 1; // unit: %
            double acceptPXGapX = Vision.Camera.ResolutionX((int)cam) / 100 * acceptRate;
            double acceptPXGapY = Vision.Camera.ResolutionY((int)cam) / 100 * acceptRate;
            double x1 = 0, y1 = 0;
            var nearest = calibMap[(int)cam, (int)toolType]
                .OrderBy(p =>
                Math.Pow(p.Xpx - x, 2) + Math.Pow(p.Ypx - y, 2)).Take(4).ToList();
            bool isDataInMap = true;
            if (nearest.Count < 4) isDataInMap = false;
            foreach (CalibPoint point in nearest)
            {
                if (Math.Abs(point.Xpx - x) >= acceptPXGapX || Math.Abs(point.Ypx - y) >= acceptPXGapY) isDataInMap = false;
                if (point.Xpx == INVALID_DATA || point.Ypx == INVALID_DATA
                    || point.Xmm == INVALID_DATA || point.Ymm == INVALID_DATA) return (INVALID_DATA, INVALID_DATA);
            }
            if (!isDataInMap)
            {
                (double xByPixelSize, double yByPixelSize) = ConvertPxToMmByCoord(cam, toolType, x, y);
                (double xByModel, double yByModel) = GetPredictedXYmm(cam, toolType, x, y);
                if (Math.Abs(xByPixelSize - xByModel) >= 10 || Math.Abs(yByPixelSize - yByModel) >= 10) // unit: mm
                    return (INVALID_DATA, INVALID_DATA);
                return (xByModel, yByModel);
            }
            else
            {
                double totalWeight = 0;
                double xSum = 0;
                double ySum = 0;
                foreach (var p in nearest)
                {
                    double dist = Math.Sqrt(Math.Pow(p.Xpx - x, 2) + Math.Pow(p.Ypx - y, 2));
                    double w = dist == 0 ? 1e6 : 1.0 / dist;
                    totalWeight += w;
                    xSum += p.Xmm * w;
                    ySum += p.Ymm * w;
                }
                x1 = xSum / totalWeight;
                y1 = ySum / totalWeight;
            }
            return (x1, y1);
        }

        public static (double Xmm, double Ymm) GetPredictedXYmm(CAMERA cam, TOOL_TYPE toolType, double Xpx, double Ypx)
        {
            try
            {
                PxToMmmPoly2D poly2DModel = new PxToMmmPoly2D();
                poly2DModel.Train(calibMap[(int)cam, (int)toolType]);
                (double returnX, double returnY) = poly2DModel.Predict(Xpx, Ypx);
                if (double.IsNaN(returnX) || double.IsNaN(returnY)) throw new Exception("Nan");
                return poly2DModel.Predict(Xpx, Ypx);
            }
            catch (Exception ex) { return (INVALID_DATA, INVALID_DATA); }
        }

        public static (double Xpx, double Ypx) GetPredictedXYpx(CAMERA cam, TOOL_TYPE toolType, double Xmm, double Ymm)
        {
            try
            {
                MmToPxPoly2D poly2DModel = new MmToPxPoly2D();
                poly2DModel.Train(calibMap[(int)cam, (int)toolType]);
                return poly2DModel.Predict(Xmm, Ymm);
            }
            catch (Exception ex) { return (INVALID_DATA, INVALID_DATA); }
        }

        public static Point2d RotationTransformation(TOOL_TYPE toolType, double dJigAngle, double dUnderAngle, double JigR, double dUnderX, double dUnderY, double dAlignX, double dAlignY)
        {
            double angle = dJigAngle - dUnderAngle + JigR * Math.PI / 180.0;

            double dx0 = dUnderX - dAlignX;
            double dy0 = dUnderY - dAlignY;

            double dx_jig = Math.Cos(angle) * dx0 - dy0 * Math.Sin(angle);
            double dy_jig = Math.Sin(angle) * dx0 + Math.Cos(angle) * dy0;

            dx_jig -= dx0;
            dy_jig -= dy0;
            return new Point2d(dx_jig, dy_jig);
        }

        public static Point2d RotationTransformationForSingle(double angle, double dUnderX, double dUnderY, double dCurrentX, double dCurrentY)
        {
            angle = angle * Math.PI / 180.0;

            double dx0 = dUnderX - dCurrentX;
            double dy0 = dUnderY - dCurrentY;

            double dx = Math.Cos(angle) * dx0 - dy0 * Math.Sin(angle);
            double dy = Math.Sin(angle) * dx0 + Math.Cos(angle) * dy0;

            dx -= dx0;
            dy -= dy0;
            return new Point2d(dx, dy);
        }

        #endregion

        #region DATA_UTIL
        private static bool CheckIfMapContainSameData(CAMERA cam, TOOL_TYPE tool)
        {
            List<CalibPoint> pts = calibMap[(int)cam, (int)tool];
            for (int i = 1; i < pts.Count; i++)
            {
                if (pts[i].Xpx == pts[i - 1].Xpx && pts[i].Ypx == pts[i - 1].Ypx)
                {
                    MessageBox.Show($"Calibration Map of {cam}_{tool} containt SAME DATA at index {i} and {i - 1}." +
                        $"\nPlease check and edit it!");
                    return true;
                }
            }
            return false;
        }

        public static bool IsSameWithLastData(CAMERA cam, TOOL_TYPE tool, Point2d newData)
        {
            lock (lockObj)
            {
                bool isSame = (newData.x == lastAlignData[(int)cam, (int)tool].x) && (newData.y == lastAlignData[(int)cam, (int)tool].y);
                if (!isSame)
                {
                    lastAlignData[(int)cam, (int)tool].x = newData.x;
                    lastAlignData[(int)cam, (int)tool].y = newData.y;
                }
                return isSame;
            }
        }
        #endregion
        #endregion
    }

    #region DATA_PREDICT_MODEL
    public class PxToMmmPoly2D
    {
        public double[] coefX = new double[6];
        public double[] coefY = new double[6];
        private double xpMin, xpMax, ypMin, ypMax;
        double[] F(double xp, double yp)
        {
            double xn = (xp - xpMin) / (xpMax - xpMin);
            double yn = (yp - ypMin) / (ypMax - ypMin);
            return new double[]
            {
                    1,
                    xn,
                    yn,
                    xn*xn,
                    xn*yn,
                    yn*yn
            };
        }
        public static double[] Solve6x6(double[,] M, double[] B)
        {
            int n = 6;
            double[,] A = (double[,])M.Clone();
            double[] b = (double[])B.Clone();
            for (int i = 0; i < n; i++)
            {
                double pivot = A[i, i];
                if (Math.Abs(pivot) < 1e-12)
                {
                    throw new Exception("Matrix Error!");
                }
                //normalize pivot row
                for (int j = i; j < n; j++)
                {
                    A[i, j] /= pivot;
                }
                b[i] /= pivot;
                for (int r = i + 1; r < n; r++)
                {
                    double factor = A[r, i];
                    for (int c = i; c < n; c++)
                    {
                        A[r, c] -= factor * A[i, c];
                    }
                    b[r] -= factor * b[i];
                }
            }
            //back sub
            double[] x = new double[n];
            for (int i = n - 1; i >= 0; i--)
            {
                x[i] = b[i];
                for (int j = i + 1; j < n; j++)
                {
                    x[i] -= A[i, j] * x[j];
                }
            }
            return x;
        }
        public void Train(List<CalibPoint> pts)
        {
            double[,] ATA = new double[6, 6];
            double[] ATbX = new double[6];
            double[] ATbY = new double[6];
            xpMin = pts.Min(p => p.Xpx);
            xpMax = pts.Max(p => p.Xpx);
            ypMin = pts.Min(p => p.Ypx);
            ypMax = pts.Max(p => p.Ypx);
            foreach (var p in pts)
            {
                if (p.Xpx == Vision.INVALID_DATA || p.Ypx == Vision.INVALID_DATA) continue;
                double[] f = F(p.Xpx, p.Ypx);
                for (int i = 0; i < 6; i++)
                {
                    for (int j = 0; j < 6; j++)
                    {
                        ATA[i, j] += f[i] * f[j];
                    }
                }
                for (int i = 0; i < 6; i++)
                {
                    ATbX[i] += f[i] * p.Xmm;
                    ATbY[i] += f[i] * p.Ymm;
                }
            }
            coefX = Solve6x6(ATA, ATbX);
            coefY = Solve6x6(ATA, ATbY);
        }

        public (double Xmm, double Ymm) Predict(double Xpx, double Ypx)
        {
            double[] f = F(Xpx, Ypx);
            double xmm = 0; double ymm = 0;
            for (int i = 0; i < 6; i++)
            {
                xmm += coefX[i] * f[i];
                ymm += coefY[i] * f[i];
            }
            return (xmm, ymm);
        }
    }

    public class MmToPxPoly2D
    {
        public double[] coefX = new double[6]; // px
        public double[] coefY = new double[6]; // py

        private double xmMin, xmMax, ymMin, ymMax;

        double[] F(double xm, double ym)
        {
            double xn = (xm - xmMin) / (xmMax - xmMin);
            double yn = (ym - ymMin) / (ymMax - ymMin);

            return new double[]
            {
            1,
            xn,
            yn,
            xn*xn,
            xn*yn,
            yn*yn
            };
        }

        public void Train(List<CalibPoint> pts)
        {
            double[,] ATA = new double[6, 6];
            double[] ATbX = new double[6];
            double[] ATbY = new double[6];

            xmMin = pts.Min(p => p.Xmm);
            xmMax = pts.Max(p => p.Xmm);
            ymMin = pts.Min(p => p.Ymm);
            ymMax = pts.Max(p => p.Ymm);

            foreach (var p in pts)
            {
                if (p.Xpx == Vision.INVALID_DATA || p.Ypx == Vision.INVALID_DATA) continue;

                double[] f = F(p.Xmm, p.Ymm);

                for (int i = 0; i < 6; i++)
                    for (int j = 0; j < 6; j++)
                        ATA[i, j] += f[i] * f[j];

                for (int i = 0; i < 6; i++)
                {
                    ATbX[i] += f[i] * p.Xpx;
                    ATbY[i] += f[i] * p.Ypx;
                }
            }

            coefX = PxToMmmPoly2D.Solve6x6(ATA, ATbX);
            coefY = PxToMmmPoly2D.Solve6x6(ATA, ATbY);
        }

        public (double Xpx, double Ypx) Predict(double xmm, double ymm)
        {
            double[] f = F(xmm, ymm);

            double xpx = 0, ypx = 0;
            for (int i = 0; i < 6; i++)
            {
                xpx += coefX[i] * f[i];
                ypx += coefY[i] * f[i];
            }
            return (xpx, ypx);
        }
    }

    #endregion
}