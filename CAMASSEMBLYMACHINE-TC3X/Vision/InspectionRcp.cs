using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.IO;
using System.Threading;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using Cognex.VisionPro;
using TopEng.Vision;
using TopEng.Utils;
using CAMASSEMBLYMACHINE.Define;
using CAMASSEMBLYMACHINE.Process;

namespace CAMASSEMBLYMACHINE
{
    public class RECIPEINFO
    {
        public string desc { get; set; } = "unknown recipe";
        public string PO_TYPE { get; set; } = "NONE";
        public int PO_DATA_NUMBER { get; set; } = 0;
        public string PO_DATA { get; set; } = "NONE";
    }
    public class INSPECTSETTING
    {
        public LIGHTDATA lightSet { get; set; }
        public double RatioX { get; set; } = 1.0;
        public double RatioY { get; set; } = 1.0;
        public string cameraColorSet { get; set; } = "";
        public bool useExternalRef { get; set; } = false;
        public double ExternalRefError { get; set; } = 200;
        public bool useCameraCenter { get; set; } = false;
        public string resultRecordName { get; set; } = "";
        public bool useInputInspection { get; set; } = false;
    }

    public class VPPFILEINFO
    {
        public string resultToolName { get; set; } = "";
        public int resultCount { get; set; } = 1;
        public bool resultAll { get; set; } = false;
        public string visionDesc { get; set; } = string.Empty;
    }

    public class InspectionRcp
    {
        public INSPECTSETTING inspInfo = new INSPECTSETTING();
        public List<VPPFILEINFO> vppInfo = new List<VPPFILEINFO>();
        public CogToolBlockHandler cogToolBlkProc = new CogToolBlockHandler();
        public CaptureUtil captureProc;

        public string inspectName = "";
        private string recentlyVppPath = "";

        private Thread JobThread = null;
        public ICogImage InputImage = null;
        bool requestInspection = false;
        int targetCam = -1;

        public delegate void MessageCallbackEvent(string msg);
        public event MessageCallbackEvent m_cbMessageEventFunc;
        public delegate void DisplayResultEvent(int camera, string text, CogColorConstants color);
        public event DisplayResultEvent m_cbDisplayResultEventFunc;
        public delegate void cbUpdateTactTimeEvent(int camera, int ms);
        public event cbUpdateTactTimeEvent UpdateTacTime;
        public delegate void cbInspectionResultEvent(string inspname, Dictionary<string, List<VisionResult>> list, bool sucs);
        public event cbInspectionResultEvent InspectionResult;

        public InspectionRcp(int Camera)
        {
            targetCam = Camera;
        }

        public void SendInspectionResultLog(string msg)
        {
            if (m_cbMessageEventFunc != null)
                m_cbMessageEventFunc(msg);
        }

        public void Create(string filepath)
        {
            try
            {
                vppInfo.Clear();
                vppInfo.Add(new VPPFILEINFO());
                WriteInfo(filepath);

                if (File.Exists(filepath + @"\Inspection.vpp"))
                    cogToolBlkProc.Create(filepath + @"\Inspection.vpp");

                if (captureProc == null)
                    captureProc = new CaptureUtil($"{SystemDefine.trainingPath}\\{inspectName}", 2);
                else
                    captureProc.filepath = $"{SystemDefine.trainingPath}\\{inspectName}";

                captureProc.autoDelete = false;
            }
            catch (Exception e)
            {
                LogUtil.Instance.Log(LOG_TYPE.DEVELOPMENT, e.Message, CONTENT_TYPE.EXCEPTION);
            }
        }

        public bool Read(string filepath)
        {
            try
            {
                ReadInfo(filepath);

                if (File.Exists(filepath + @"\Inspection.vpp"))
                    cogToolBlkProc.ReadBlock(filepath + @"\Inspection.vpp");
                recentlyVppPath = filepath;

                if (captureProc == null)
                    captureProc = new CaptureUtil($"{SystemDefine.trainingPath}\\{inspectName}", 2);
                else
                    captureProc.filepath = $"{SystemDefine.trainingPath}\\{inspectName}";

            }
            catch (Exception e)
            {
                LogUtil.Instance.Log(LOG_TYPE.DEVELOPMENT, e.Message, CONTENT_TYPE.EXCEPTION);
            }

            return true;
        }

        public bool Reload()
        {
            if (recentlyVppPath == "")
                return false;
            return Read(recentlyVppPath);
        }

        public bool Write(string filepath)
        {
            try
            {
                WriteInfo(filepath);
            }
            catch (Exception e)
            {
                LogUtil.Instance.Log(LOG_TYPE.DEVELOPMENT, e.Message, CONTENT_TYPE.EXCEPTION);
            }

            return true;
        }

        private void ReadInfo(string filepath)
        {
            try
            {
                string pathVppInfo = filepath + @"\vppinfo.json";
                if (File.Exists(pathVppInfo))
                {
                    string streamJson = File.ReadAllText(pathVppInfo);
                    streamJson = Regex.Unescape(streamJson);
                    vppInfo = JsonSerializer.Deserialize<List<VPPFILEINFO>>(streamJson);
                }
                string pathInspInfo = filepath + @"\inspinfo.json";
                if (File.Exists(pathInspInfo))
                {
                    string streamJson = File.ReadAllText(pathInspInfo);
                    streamJson = Regex.Unescape(streamJson);
                    inspInfo = JsonSerializer.Deserialize<INSPECTSETTING>(streamJson);
                }
            }
            catch (Exception e)
            {
                LogUtil.Instance.Log(LOG_TYPE.DEVELOPMENT, e.Message, CONTENT_TYPE.EXCEPTION);
            }
        }

        private void WriteInfo(string filepath)
        {
            try
            {
                var options = new JsonSerializerOptions { WriteIndented = true };

                string streamJson = JsonSerializer.Serialize(vppInfo, options);
                streamJson = Regex.Unescape(streamJson);
                string pathVpp = filepath + @"\vppinfo.json";
                if (!IOFunctionUtil.IsDiskSpaceAvailable(filepath, streamJson)) return;
                File.WriteAllText(pathVpp, streamJson);

                string streamJson2 = JsonSerializer.Serialize(inspInfo, options);
                streamJson2 = Regex.Unescape(streamJson2);
                string pathInsp = filepath + @"\inspinfo.json";
                if (!IOFunctionUtil.IsDiskSpaceAvailable(filepath, streamJson2)) return;
                File.WriteAllText(pathInsp, streamJson2);
            }
            catch (Exception e)
            {
                LogUtil.Instance.Log(LOG_TYPE.DEVELOPMENT, e.Message, CONTENT_TYPE.EXCEPTION);
            }
        }

        public bool Run(ICogImage input, bool Send = false, bool SaveTrainImage = false, double overlap = 500, double angleMin = -180, double angleMax = 180)
        {
            if (!ThreadStateIDLE() || requestInspection)
                return false;

            requestInspection = true;

            try
            {
                InputImage = input;

                JobThread = new Thread(new ParameterizedThreadStart(OnThreadJobProc));
                JobThread.Priority = ThreadPriority.Normal;
                JobThread.Start(new object[] { Send, SaveTrainImage, overlap, angleMin, angleMax });

                foreach (ProcessThread proc in System.Diagnostics.Process.GetCurrentProcess().Threads)
                    proc.ProcessorAffinity = System.Diagnostics.Process.GetCurrentProcess().ProcessorAffinity;

                return true;
            }
            catch (Exception e)
            {
                LogUtil.Instance.Log(LOG_TYPE.DEVELOPMENT, e.Message, CONTENT_TYPE.EXCEPTION);
                return false;
            }
        }

        public bool ThreadStateIDLE()
        {
            if (!requestInspection)
                return true;
            if (JobThread == null)
                return true;

            return false;
        }

        private void OnThreadJobProc(object pObject)
        {
            bool SendData = (bool)((object[])pObject)[0];
            bool SaveTrainImage = (bool)((object[])pObject)[1];
            double overlap = (double)((object[])pObject)[2];
            double angleMin = (double)((object[])pObject)[3];
            double angleMax = (double)((object[])pObject)[4];

            try
            {
                //captureProc.pauseSave = true;

                if (InputImage.GetType() == typeof(CogImage24PlanarColor))
                {
                    if (inspInfo.cameraColorSet != null && inspInfo.cameraColorSet != "RED" && inspInfo.cameraColorSet != "GREEN" && inspInfo.cameraColorSet != "BLUE")
                    {
                        InputImage = CogImageConvert.GetIntensityImage(InputImage, 0, 0, InputImage.Width, InputImage.Height);
                    }
                    else
                    {
                        InputImage = CogImageConvert.GetRGBImage(InputImage, 0, 0, InputImage.Width, InputImage.Height);

                        //Bitmap bitmap = InputImage.ToBitmap();
                        //BitmapData bmpData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, bitmap.PixelFormat);
                        //IntPtr ptrBmp = bmpData.Scan0;
                        //int bytes = Math.Abs(bmpData.Stride) * bitmap.Height;
                        //byte[] rgbValues = new byte[bytes];
                        //System.Runtime.InteropServices.Marshal.Copy(ptrBmp, rgbValues, 0, bytes);

                        //int numBytes = 0;
                        //for (int y = 0; y < bitmap.Height; y++)
                        //{
                        //    for (int x = 0; x < bitmap.Width; x++)
                        //    {
                        //        numBytes = (y * bitmap.Width * 3) + x * 3;

                        //        if (inspInfo.cameraColorSet == "RED") rgbValues[numBytes + 1] = rgbValues[numBytes + 2] = rgbValues[numBytes + 0];
                        //        if (inspInfo.cameraColorSet == "GREEN") rgbValues[numBytes + 0] = rgbValues[numBytes + 2] = rgbValues[numBytes + 1];
                        //        if (inspInfo.cameraColorSet == "BLUE") rgbValues[numBytes + 0] = rgbValues[numBytes + 1] = rgbValues[numBytes + 2];
                        //    }
                        //}

                        //System.Runtime.InteropServices.Marshal.Copy(rgbValues, 0, ptrBmp, bitmap.Width * bitmap.Height * 3);
                        //bitmap.UnlockBits(bmpData);

                        //InputImage = new CogImage8Grey(bitmap);
                    }
                }
                else
                {
                    InputImage = CogImageConvert.GetIntensityImage(InputImage, 0, 0, InputImage.Width, InputImage.Height);
                }

                if (targetCam == 0)
                {
                    cogToolBlkProc.ChangeInputValue("NG_X", Machine.trayNGListX);
                    cogToolBlkProc.ChangeInputValue("NG_Y", Machine.trayNGListY);
                    cogToolBlkProc.ChangeInputValue("USE_PRODUCT_COUNT", Machine.isUseProductCount);
                    double lowScore = Machine.param.Option(ParameterDefine.OPTION.TRAY_WORK_CLASSIFY_LOW_SCORE);
                    cogToolBlkProc.ChangeInputValue("LOW_SCORE_VALUE", lowScore);
                    ProcessTrayWork procTray = Machine.proclist[(int)Machine.PROCESS.TRAYWORK] as ProcessTrayWork;
                    cogToolBlkProc.ChangeInputValue("TRAY_ANGLE", procTray.trayAngle);
                }
                cogToolBlkProc.Run(InputImage);
                SendProcResult(SendData, SaveTrainImage, overlap, angleMin, angleMax);

                requestInspection = false;
            }
            catch (Exception e)
            {
                LogUtil.Instance.Log(LOG_TYPE.DEVELOPMENT, e.Message, CONTENT_TYPE.EXCEPTION);
                requestInspection = false;
            }

            GC.Collect();
            JobThread = null;
        }

        private void SendProcResult(bool SendData, bool SaveTrainImage, double overlap = 500, double angleMin = -180, double angleMax = 180)
        {
            try
            {
                var results = new Dictionary<string, List<VisionResult>>();

                bool sequenceSucs = true;
                var result_pmaligntool = new Dictionary<string, List<CogToolPMAlign>>();
         
                OutPutsResult VisionProdata = new OutPutsResult();

                SystemDefine.bAlignCheck = true;


                try
                {
                    VisionProdata.Lcenter.x = cogToolBlkProc.GetOutPuts("L_X");
                    VisionProdata.Lcenter.y = cogToolBlkProc.GetOutPuts("L_Y");
                    VisionProdata.Langle = cogToolBlkProc.GetOutPuts("L_R");
                    VisionProdata.Ldirection = cogToolBlkProc.GetOutPutsString("L_CL_DIR");
                    VisionProdata.LdirectionScore = cogToolBlkProc.GetOutPuts("L_CL_DIR_SCORE");
                    VisionProdata.LClassifyClass = cogToolBlkProc.GetOutPutsString("L_CL_CLASS");
                    VisionProdata.LClassifyScore = cogToolBlkProc.GetOutPuts("L_CL_SCORE");
                    VisionProdata.LClassifyImage = cogToolBlkProc.GetOutPutsImage("L_CL_IMAGE");

                    VisionProdata.Rcenter.x = cogToolBlkProc.GetOutPuts("R_X");
                    VisionProdata.Rcenter.y = cogToolBlkProc.GetOutPuts("R_Y");
                    VisionProdata.Rangle = cogToolBlkProc.GetOutPuts("R_R");
                    VisionProdata.Rdirection = cogToolBlkProc.GetOutPutsString("R_CL_DIR");
                    VisionProdata.RdirectionScore = cogToolBlkProc.GetOutPuts("R_CL_DIR_SCORE");
                    VisionProdata.RClassifyClass = cogToolBlkProc.GetOutPutsString("R_CL_CLASS");
                    VisionProdata.RClassifyScore = cogToolBlkProc.GetOutPuts("R_CL_SCORE");
                    VisionProdata.sOCR_ID = cogToolBlkProc.GetOutPutsString("S_OCR");
                    VisionProdata.RClassifyImage = cogToolBlkProc.GetOutPutsImage("R_CL_IMAGE");

                    VisionProdata.currentProductCount = (int)cogToolBlkProc.GetOutPuts("CURRENT_PRODUCT_COUNT");
                    VisionProdata.targetProductCount = (int)cogToolBlkProc.GetOutPuts("TARGET_PRODUCT_COUNT");
                }
                catch (Exception e)
                {
                    VisionProdata.Lcenter.x = Vision.INVALID_DATA;
                    VisionProdata.Lcenter.y = Vision.INVALID_DATA;
                    VisionProdata.Langle = 0;
                    VisionProdata.LIndex = -1;

                    VisionProdata.Rcenter.x = Vision.INVALID_DATA;
                    VisionProdata.Rcenter.y = Vision.INVALID_DATA;
                    VisionProdata.Rangle = 0;
                    VisionProdata.RIndex = -1;

                    LogUtil.Instance.Log(LOG_TYPE.DEVELOPMENT, "SendProcREesult" + e.Message, CONTENT_TYPE.EXCEPTION);
                }
                results.Add("OutPuts", new List<VisionResult>());
                results["OutPuts"].Add(VisionProdata);

                if (Vision.cogTool[targetCam].m_cogRecordDisplay != null)
                {
                    Vision.cogTool[targetCam].m_cogRecordDisplay.InteractiveGraphics.Clear();
                    Vision.cogTool[targetCam].m_cogRecordDisplay.StaticGraphics.Clear();

                    ICogRecord rec;

                    if (inspInfo.resultRecordName.Length > 0)
                    {
                        rec = cogToolBlkProc.Records(inspInfo.resultRecordName);
                        if (rec == null)
                            rec = cogToolBlkProc.Records(0);
                    }
                    else
                        rec = cogToolBlkProc.Records(0);

                    Vision.cogTool[targetCam].m_cogRecordDisplay.Record = rec;
                }
                if (VisionProdata.LClassifyImage != null) Vision.cogTool[targetCam].clImageLeft = VisionProdata.LClassifyImage;
                if (VisionProdata.RClassifyImage != null) Vision.cogTool[targetCam].clImageRight = VisionProdata.RClassifyImage;
                if (SendData && vppInfo.Count > 0 && InspectionResult != null)
                    InspectionResult(inspectName, results, sequenceSucs);
            }
            catch (Exception e)
            {
                LogUtil.Instance.Log(LOG_TYPE.DEVELOPMENT, e.Message, CONTENT_TYPE.EXCEPTION);

                if (SendData && vppInfo.Count > 0 && InspectionResult != null)
                    InspectionResult(inspectName, new Dictionary<string, List<VisionResult>>(), false);
            }
        }

        public void AddText(string groupname, string text, double LocX, double LocY, CogColorConstants color, CogGraphicLabelAlignmentConstants align = CogGraphicLabelAlignmentConstants.TopLeft, string fontname = "Arial", double height = 8)
        {
            if (Vision.cogTool[targetCam].m_cogRecordDisplay == null)
                return;

            CogGraphicLabel label = new CogGraphicLabel();
            label.Font = new Font(new FontFamily(fontname), (float)height, FontStyle.Regular, GraphicsUnit.Point);
            label.SetXYText(LocX, LocY, text);
            label.Color = color;
            label.Alignment = align;
            label.Interactive = false;
            label.SelectedSpaceName = ".";
            label.GraphicDOFEnable = CogGraphicLabelDOFConstants.All;

            Vision.cogTool[targetCam].m_cogRecordDisplay.StaticGraphics.Add(label, groupname);
            //Vision.cogTool[targetCam].m_cogRecordDisplay.Fit(true);
        }
    }
}
