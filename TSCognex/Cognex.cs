using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using Cognex.VisionPro;
using Cognex.VisionPro.ImageProcessing;
using Cognex.VisionPro.Display;
using Cognex.VisionPro.PMAlign;
using Cognex.VisionPro.ImageFile;
using Cognex.VisionPro.ToolBlock;
using Cognex.VisionPro.Implementation;
using Cognex.VisionPro.Blob;
using Cognex.VisionPro.ID;
using Cognex.VisionPro.OCRMax;
using Cognex.VisionPro.CalibFix;
using Cognex.VisionPro.Caliper;
using TopEng.Type;
using TopEng.Utils;
using Cognex.VisionPro.ViDiEL;
using Cognex.Vision.ViDiEL;
using System.Runtime.CompilerServices;
using System.Threading;
using Cognex.Vision;

namespace TopEng.Vision
{
    public class CogLicense
    {
        public bool Certification = false;
        public int Counts = 0;
        public string[] Features = null;
        public int RemainingDays = -1;
        public bool ViewList = false;
        public string ReleaseNumber = "";
        public bool HighVersion = false; // 9.5 High
        
        public void ValidLicense()
        {
            try
            {
                Startup.Initialize(Startup.ProductKey.VProX);

                CogVisionToolMultiThreading.Enable = true;
                CogVisionToolMultiThreading.ThreadCountMode = CogVisionToolMultiThreadingThreadCountModeConstants.UserSpecified;
                CogVisionToolMultiThreading.ThreadCount = 32;

                Certification = true;
            }
            catch (Exception e)
            {
                Certification = false;
                throw e;
            }
        }

        public void Shutdown()
        {
            Startup.Shutdown();
        }
    }

    //public class CognexSystem
    //{
    //    public LICENSE license = new LICENSE();
    //    public CognexSystem()
    //    {
    //        CogLocalizer.SetVisionProCulture();
    //    }

    //    public void ValidLicense()
    //    {
    //        try
    //        {
    //            license.ValidLicense();
    //        }
    //        catch (Exception e)
    //        {
    //            throw e;
    //        }
    //    }
    //}

    public enum CAMERATYPE
    {
        CameraLink,
        GigE,
    }

    public class ICogAcqFifoToolInfo
    {
        public CAMERATYPE type;
        public string name;
        public string serial;
        public string ip;

        public string videoFormat;
        public double exposure;
        public double brightness;
        public double contrast;
        public double gain = 0;
        public bool gainSetUse = false;

        public Size2d resolution = new Size2d();
        public Size2d pixel = new Size2d();

        public int flipRot = 0;
        public bool median = false;
        public bool equalization = false;
        public bool pixelMap = false;
        public bool pixelMapView = false;
        public int pixelMapVal;
        public bool quantize = false;
        public bool quantizeView = false;
        public int quantizeLevel;

        // STATE
        public bool operationCompl;
        public int eventCount = 0;
        public bool grabOk;
    }

    public class CognexTool
    {
        public ICogAcqFifo m_cogAcqFifo;
        public ICogAcqTrigger m_cogAcqTrigger;
        public ICogAcqFifoToolInfo m_cogAcqFifoInfo = new ICogAcqFifoToolInfo();
        public ICogImage m_cogImage;
        public ICogImage m_cogImageNoDevice;
        public ICogImage m_cogPixelMap;
        public ICogImage m_cogQuantize;
        public bool useAcqFifo = false;
        public CogDisplay m_cogDisplay;
        public CogRecordDisplay m_cogRecordDisplay;
        public ICogImage clImageLeft;
        public ICogImage clImageRight;

        public CaptureUtil m_capture;

        public CognexTool(string capturePath = "", int delete_period_days = 1)
        {
            try
            {
                m_cogAcqFifo = null;
                m_cogAcqFifoInfo.videoFormat = "Generic GigE Vision (Mono)";
                m_cogAcqFifoInfo.exposure = 0.01;

                useAcqFifo = false;

                m_capture = new CaptureUtil(capturePath, delete_period_days);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void callbackGrabbedImageEvent(int grabId, Bitmap image)
        {
            if (image.PixelFormat == PixelFormat.Format24bppRgb)
                m_cogImage = new CogImage24PlanarColor(image);
            else
                m_cogImage = new CogImage8Grey(image);
            if (m_cogRecordDisplay != null)
                m_cogRecordDisplay.Image = m_cogImage;
        }

        public void InitAcqFifo(ICogFrameGrabber frame, string videoFormat, CogAcqFifoPixelFormatConstants pixelFormat, int port)
        {
            m_cogAcqFifo = null;
            m_cogAcqFifo = frame.CreateAcqFifo(videoFormat, pixelFormat, port, true);
            m_cogAcqFifo.TimeoutEnabled = true;
            m_cogAcqFifo.Timeout = 5000;
            m_cogAcqFifo.AutoPrepareEnabled = true;
            m_cogAcqFifo.Prepare();

            m_cogAcqFifo.Complete += cbOperationCompleteEvent;
            m_cogAcqFifo.Flush();
            m_cogAcqFifo.OwnedExposureParams.Exposure = m_cogAcqFifoInfo.exposure;
        }

        // 카메라에서 영상을 받아올 때 발생하는 이벤트 (성공/실패 상관없이 발생)
        public void cbOperationCompleteEvent(object sender, CogCompleteEventArgs args)
        {
            m_cogAcqFifoInfo.operationCompl = true;

            try
            {
                bool busyVal;
                int readyVal, pendingVal, ticket, trigVal;

                m_cogAcqFifo.GetFifoState(out pendingVal, out readyVal, out busyVal);
                m_cogAcqFifoInfo.eventCount++;

                if (readyVal > 0)
                {
                    ICogImage acqImg = null;
                    acqImg = m_cogAcqFifo.CompleteAcquire(-1, out ticket, out trigVal);

                    // PREPROCESSING 진행
                    if (acqImg != null)
                    {
                        // FLIP 진행
                        if (m_cogAcqFifoInfo.flipRot != (int)CogIPOneImageFlipRotateOperationConstants.None)
                        {
                            CogIPOneImageFlipRotate cogFlip = new CogIPOneImageFlipRotate();
                            cogFlip.OperationInPixelSpace = (CogIPOneImageFlipRotateOperationConstants)m_cogAcqFifoInfo.flipRot;
                            m_cogImage = cogFlip.Execute(m_cogImage, CogRegionModeConstants.AffineTransform, null);
                            m_cogImage.SelectedSpaceName = "#";
                        }

                        // 메디안 필터 적용
                        if (m_cogAcqFifoInfo.median)
                        {
                            CogIPOneImageMedianNxM cogMedian = new CogIPOneImageMedianNxM();
                            cogMedian.KernelWidth = 5;
                            cogMedian.KernelHeight = 5;
                            m_cogImage = cogMedian.Execute(m_cogImage, CogRegionModeConstants.PixelAlignedBoundingBox, null);
                        }

                        // 평활화 진행
                        if (m_cogAcqFifoInfo.equalization)
                        {
                            CogIPOneImageEqualize cogEqualize = new CogIPOneImageEqualize();
                            m_cogImage = cogEqualize.Execute(m_cogImage, CogRegionModeConstants.PixelAlignedBoundingBox, null);
                        }

                        // 픽셀맵 작성
                        if (m_cogAcqFifoInfo.pixelMap)
                        {
                            m_cogPixelMap = null;

                            CogIPOneImagePixelMap cogPixelMap = new CogIPOneImagePixelMap();

                            byte[] arrByte = new byte[256];
                            for (int i = 0; i < m_cogAcqFifoInfo.pixelMapVal; i++)
                                arrByte[i] = 0;
                            for (int i = m_cogAcqFifoInfo.pixelMapVal; i < 256; i++)
                                arrByte[i] = 255;

                            ICogRegion cogRegion = null;
                            cogPixelMap.SetMap(arrByte);
                            m_cogPixelMap = (ICogImage)cogPixelMap.Execute(m_cogImage, CogRegionModeConstants.AffineTransform, (ICogRegion)cogRegion);
                            GC.Collect();
                        }

                        // 양자화 진행
                        if (m_cogAcqFifoInfo.quantize)
                        {
                            CogIPOneImageQuantize cogQuantize = new CogIPOneImageQuantize();
                            cogQuantize.Levels = (CogIPOneImageQuantizeLevelConstants)m_cogAcqFifoInfo.quantizeLevel;
                            // S2 = 0, S4 = 1, S8 = 2, S16 = 3, S32 = 4, S64 = 5, S128 = 6
                            m_cogQuantize = cogQuantize.Execute(m_cogImage, CogRegionModeConstants.AffineTransform, null);
                        }

                        m_cogAcqFifoInfo.grabOk = true;

                        if (m_cogDisplay.LiveDisplayRunning)
                        {
                            if (m_cogAcqFifoInfo.quantizeView)
                                m_cogDisplay.Image = m_cogQuantize;
                            else if (m_cogAcqFifoInfo.pixelMapView)
                                m_cogDisplay.Image = m_cogPixelMap;
                            else
                                m_cogDisplay.Image = m_cogImage;
                        }
                    }
                }

                m_cogAcqFifoInfo.eventCount = 0;

                GC.Collect();

                if (!m_cogDisplay.LiveDisplayRunning)
                    m_cogAcqFifo.Flush(); // FIFO 비우기

                m_cogAcqFifoInfo.operationCompl = false;

                Process currProc = Process.GetCurrentProcess(); // 메모리 클리어
                currProc.MaxWorkingSet = currProc.MaxWorkingSet;

            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void Exposure(double exposure)
        {
            if (exposure < 0)
                return;
            if (m_cogAcqFifo.OwnedExposureParams.Exposure == exposure)
                return;

            try
            {
                if (exposure == 0)
                    exposure = 5;
                m_cogAcqFifo.OwnedExposureParams.Exposure = exposure;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void Brightness(double brightness)
        {
            if (brightness < 0)
                return;
            if (m_cogAcqFifo.OwnedBrightnessParams.Brightness == brightness)
                return;

            try
            {
                if (brightness == 0)
                    brightness = 0.125;
                m_cogAcqFifo.OwnedBrightnessParams.Brightness = brightness;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void Contrast(double contrast)
        {
            if (contrast < 0)
                return;
            if (m_cogAcqFifo.OwnedContrastParams.Contrast == contrast)
                return;

            try
            {
                if (contrast == 0)
                    contrast = 0.125;
                m_cogAcqFifo.OwnedContrastParams.Contrast = contrast;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void Gain(double gain)
        {

        }

        public void Disconnect()
        {
            try
            {
                if (m_cogAcqFifo != null && useAcqFifo)
                    m_cogAcqFifo.FrameGrabber.Disconnect(true);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void Grab()
        {
            try
            {
                if (m_cogAcqFifo == null)
                {
                    m_cogImage = m_cogImageNoDevice;
                    m_cogAcqFifoInfo.grabOk = true;
                    return;
                }

                m_cogAcqFifoInfo.grabOk = false;

                if (m_cogDisplay.LiveDisplayRunning)
                    return;

                Exposure(m_cogAcqFifoInfo.exposure);
                Brightness(m_cogAcqFifoInfo.brightness);
                Contrast(m_cogAcqFifoInfo.contrast);
                m_cogAcqFifo.StartAcquire();
                //Application.DoEvents();  // Sleep(1) 사용하면 StartAcquire() 비정상 동작함  // 17.08.24 YHLEE
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void Capture(string name, CaptureUtil.CAPTURETYPE type, ICogImage image, bool LowQuality = false, [CallerFilePath] string file = "")
        {
            m_capture.Capture(name, type, image, LowQuality, file);
        }

        public void SetDeleteImagePeriod(int period) => m_capture.SetDeleteImagePeriod(period);

        public bool OnThreadGrabWait()
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
                if (!GetThreadGrabRunState())
                    break;

                Thread.Sleep(10);
            }

            return ret;
        }

        private bool GetThreadGrabRunState()
        {
            if (m_cogAcqFifo == null)
                m_cogAcqFifoInfo.grabOk = true;
            return !m_cogAcqFifoInfo.grabOk;
        }

        public void LiveStart(bool own = true)
        {
            try
            {
                if (m_cogAcqFifo == null)
                    return;

                Exposure(m_cogAcqFifoInfo.exposure);
                Brightness(m_cogAcqFifoInfo.brightness);
                Contrast(m_cogAcqFifoInfo.contrast);

                m_cogDisplay.StartLiveDisplay(m_cogAcqFifo, own);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void LiveStop()
        {
            try
            {
                Exposure(m_cogAcqFifoInfo.exposure);
                Brightness(m_cogAcqFifoInfo.brightness);
                Contrast(m_cogAcqFifoInfo.contrast);

                m_cogDisplay.StopLiveDisplay();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public ICogImage LoadImage(string filepath)
        {
            ICogImage cogImage = null;

            try
            {
                CogImageFile file = new CogImageFile();
                file.Open(filepath, CogImageFileModeConstants.Read);
                cogImage = (ICogImage)file[0];
                file.Close();

                cogImage = CogImageConvert.GetIntensityImage(cogImage, 0, 0, cogImage.Width, cogImage.Height);
            }
            catch (Exception e)
            {
                throw e;
            }

            return cogImage;
        }

        public void SaveImage(string filepath, ICogImage image, ImageFormat format)
        {
            try
            {
                if (format == ImageFormat.Bmp)
                {
                    CogImageFileBMP bmp = new CogImageFileBMP();
                    bmp.Open(filepath, CogImageFileModeConstants.Write);
                    bmp.Append(image);
                    bmp.Close();
                }
                if (format == ImageFormat.Jpeg)
                {
                    CogImageFileJPEG jpeg = new CogImageFileJPEG();
                    jpeg.Open(filepath, CogImageFileModeConstants.Write);
                    jpeg.Append(image);
                    jpeg.Close();
                }

                GC.Collect();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void SaveImage(string filepath, CogDisplay display, ImageFormat foramt, bool dispImg)
        {
            try
            {
                Image image;

                if (dispImg)
                    image = display.CreateContentBitmap(CogDisplayContentBitmapConstants.Custom, null, 0);
                else
                    image = display.CreateContentBitmap(CogDisplayContentBitmapConstants.Display, null, 0);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }

    public class CogToolMessage
    {
        public CogToolMessage()
        {

        }

        public void Display(ref CogDisplay display, float fontSize, double posx, double posy, CogGraphicLabelAlignmentConstants align, CogColorConstants color, string text, string space = "*")
        {
            if (text.Length == 0 || text == null)
                return;

            try
            {
                CogGraphicLabel label = new CogGraphicLabel();
                label.Font = new System.Drawing.Font(new System.Drawing.FontFamily("Arial"), (float)fontSize, FontStyle.Regular, GraphicsUnit.Point);
                label.Text = text;
                label.Color = (CogColorConstants)color;
                label.X = posx;
                label.Y = posy;
                label.Alignment = align;
                label.Interactive = false;
                //----------
                // @ - Root space.
                // # - Pixel space.
                // * - Display pixel space. You can use this special name only when specifying the space name for graphics.
                // ^ - Alias for the root space.
                // . - Current selected space name for an image. You can use this special name only with methods implemented by the CogImage object; there is no "selected space" for a CogCoordinateSpaceTree.
                // .. - Parent space name of the current selected space.
                // $ - The child coordinate space created by a parent space for its children. The VisionPro graphics system uses "$" to allow multigenerational hierarchies of shapes. You can obtain this space using a graphic's GetParentFromChildTransform method.
                //----------
                label.SelectedSpaceName = space;

                display.StaticGraphics.Add(label, "Message");
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }

    public class CogToolBlockHandler
    {
        CogToolBlock CogTools = new CogToolBlock();
        public CogToolBlockHandler()
        {
        }

        public void ChangeInputValue(string toolName, object value)
        {
            try
            {
                CogTools.Inputs[toolName].Value = value;
            }
            catch (Exception ex) { }
        }


        public CogToolBlockHandler(string filepath)
        {
            ReadBlock(filepath);
        }

        public void Create(string filepath)
        {
            try
            {
                CogSerializer.SaveObjectToFile((CogToolBlock)CogTools, filepath);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void ReadBlock(string filepath)
        {
            try
            {
                if (!File.Exists(filepath))
                    return;

                CogTools = CogSerializer.LoadObjectFromFile(filepath) as CogToolBlock;
                //for (int i = 0; i < CogTools.Tools.Count; i++)
                //{
                //    Job.Add(JobManager.Job(i));
                //    independetJob.Add(Job[i].OwnedIndependent);
                //}

                //JobManager.UserQueueFlush();
                //JobManager.FailureQueueFlush();

                //for (int i = 0; i < JobManager.JobCount; i++)
                //{
                //    Job[i].ImageQueueFlush();
                //    independetJob[i].RealTimeQueueFlush();
                //}
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public bool Run(ICogImage input)
        {
            try
            {
                if (CogTools.Tools.Count < 1/* || CogTools.Inputs["InputImage"].Value == null*/)
                    return false;

                CogTools.Inputs["Input"].Value = input;

                CogToolResultConstants result = new CogToolResultConstants();
                //string message = "";

                CogTools.Run();
                //foreach (ICogTool cogTool in CogTools.Tools)
                //{
                //    CogTools.RunTool(cogTool, ref message, ref result);
                //}

                if (result != CogToolResultConstants.Accept)
                    return false;
                return true;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void Display(ref CogRecordDisplay display, string toolname, string subject)
        {
            try
            {
                display.Record = CogTools.Tools[toolname].CreateLastRunRecord().SubRecords[subject];
                display.Fit(true);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public ICogRecord Record(string toolname, string subject)
        {
            try
            {
                return CogTools.Tools[toolname].CreateLastRunRecord().SubRecords[subject];
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public ICogRecord Records(string subject)
        {
            try
            {
                return CogTools.CreateLastRunRecord().SubRecords[subject];
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public ICogRecord Records(int index)
        {
            try
            {
                return CogTools.CreateLastRunRecord().SubRecords[index];
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public CogToolBase GetResult(string toolname, ref string tooltype)
        {
            try
            {
                tooltype = CogTools.Tools[toolname].GetType().Name;
                return (CogToolBase)CogTools.Tools[toolname];
            }
            catch
            {
                return null;
            }
        }

        public double GetOutPuts(string toolname)
        {
            if (!IsOutputExist(toolname)) return -999999999;
            double OutPutValue = Convert.ToDouble(CogTools.Outputs[toolname].Value);
            return OutPutValue;
        }

        public ICogImage GetOutPutsImage(string toolname)
        {
            try
            {
                if (!IsOutputExist(toolname)) return null;
                return (ICogImage)CogTools.Outputs[toolname].Value;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public string GetOutPutsString(string toolname)
        {
            if (!IsOutputExist(toolname)) return string.Empty;
            string OutPutValue = Convert.ToString(CogTools.Outputs[toolname].Value);
            return OutPutValue;
        }

        private bool IsOutputExist(string toolname)
        {
            foreach(CogToolBlockTerminal tool in CogTools.Outputs)
            {
                if (tool.Name == toolname) return true;
            }
            return false;
        }

        public string[] GetToolListName()
        {
            try
            {
                string[] toolname = new string[CogTools.Tools.Count];

                for (int i = 0; i < CogTools.Tools.Count; i++)
                {
                    toolname[i] = CogTools.Tools[i].Name;
                }

                return toolname;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public bool GetEnable(string strToolName)
        {
            return true;

        }
        public bool SetEnable(string strToolName)
        {
            if (strToolName.Contains("CogPMAlignTool"))
            {
                CogPMAlignTool tool = (CogPMAlignTool)CogTools.Tools[strToolName];

            }
            else if (strToolName.Contains("CogFindCircleTool"))
            {
                CogFindCircleTool tool = (CogFindCircleTool)CogTools.Tools[strToolName];
            }
            else if (strToolName.Contains("CogClassifyTool"))
            {
                CogClassifyTool tool = (CogClassifyTool)CogTools.Tools[strToolName];
            }
            else if (strToolName.Contains("CogOCRMaxTool"))
            {
                CogOCRMaxTool tool = (CogOCRMaxTool)CogTools.Tools[strToolName];
            }
            else if (strToolName.Contains("CogIDTool"))
            {
                CogIDTool tool = (CogIDTool)CogTools.Tools[strToolName];
            }
            return true;
        }
    }

    public class ICogToolBase
    {
        public string toolname = "";
        public string tooltype = "";
        public string classifyString = "";
        public string tags = "";
        public string groupname = "";

        public CogToolMessage message = new CogToolMessage();
    }


    public class CogToolClassify : ICogToolBase
    {
        public CogClassifyTool classifyTool = new CogClassifyTool();
        //public CogRecordDisplay recordDisplay = new CogRecordDisplay();
        public List<ClassificationResult> results = new List<ClassificationResult>();

        public CogToolClassify()
        {
            tooltype = "CogClassifyTool";
        }

        public CogToolClassify(string filepath)
        {
            ReadTool(filepath);
        }

        public void Create(string filepath)
        {
            try
            {
                CogSerializer.SaveObjectToFile((CogClassifyTool)classifyTool, filepath);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void ReadTool(string filepath)
        {
            try
            {
                if (!File.Exists(filepath))
                    return;

                classifyTool = CogSerializer.LoadObjectFromFile(filepath) as CogClassifyTool;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public bool Run(ICogImage input)
        {
            try
            {
                classifyTool.InputImage = input as ICogVisionData;
                CogToolResultConstants result = new CogToolResultConstants();
                classifyTool.Run();
                // NOTE: the GeneratePredictedResults() method is only available
                // via the VisionInteropTool property. And note that it may throw.
                classifyTool.VisionInteropTool.GeneratePredictedResults();

                // NOTE: CreateLastRunRecord produces a CogRecord that is
                // equivalent to the underlying VI tool's last run record.
                //recordDisplay.sub = classifyTool.CreateLastRunRecord();

                if (result != CogToolResultConstants.Accept)
                    return false;
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public List<ClassificationResult> GetResult()
        {
            results.Clear();

            ClassifyResult result = classifyTool.VisionInteropTool.Result;
            if (result == null)
                return null;
            if (result.ClassResults == null || result.ClassResults.Count <= 0)
                return null;

            foreach (ClassifyClassResult classOne in result.ClassResults)
            {
                ClassificationResult classifyResult = new ClassificationResult();

                classifyResult.classname = classOne.Class;
                classifyResult.score = classOne.Score;
                results.Add(classifyResult);
            }

            return results;
        }
    }

    public class CogToolFindCircle : ICogToolBase
    {
        public CogFindCircleTool circleTool;
        public CircleResult results = new CircleResult();

        public CogToolFindCircle()
        {
            tooltype = "CogFindCircleTool";
            circleTool = new CogFindCircleTool();
        }

        public CogToolFindCircle(CogFindCircleTool circletool)
        {
            circleTool = circletool;
        }

        public CircleResult GetResult()
        {
            if (circleTool.Results == null)
                return null;

            CogFindCircleResults circleResults = circleTool.Results;
            int alignCount = circleResults.Count;
            if (alignCount < 1)
                return null;
            var result = circleResults.GetCircle();
            if (result == null)
                return null;

            try
            {
                results.center.x = result.CenterX;
                results.center.y = result.CenterY;
                results.radius = result.Radius;
            }
            catch
            {
                return null;
            }

            return results;
        }
    }

    public class CogToolPMAlign : ICogToolBase
    {
        public CogPMAlignTool alignTool;
        public List<PMAlignResult> results = new List<PMAlignResult>();

        public CogToolPMAlign()
        {
            tooltype = "CogPMAlignTool";
            alignTool = new CogPMAlignTool();
        }

        public CogToolPMAlign(CogPMAlignTool aligntool)
        {
            alignTool = aligntool;
        }

        public void Create(bool useROI)
        {
            try
            {
                CogRectangle trainRegion = new CogRectangle();
                trainRegion.Interactive = true;
                trainRegion.SetCenterWidthHeight(200, 200, 200, 100);
                trainRegion.GraphicDOFEnable = CogRectangleDOFConstants.All;
                CogRectangle searchRegion = new CogRectangle();
                searchRegion.Interactive = true;
                searchRegion.SetCenterWidthHeight(200, 200, 200, 100);
                searchRegion.GraphicDOFEnable = CogRectangleDOFConstants.All;

                alignTool.Pattern.TrainAlgorithm = CogPMAlignTrainAlgorithmConstants.PatMax;
                alignTool.Pattern.TrainMode = CogPMAlignTrainModeConstants.Image;
                alignTool.Pattern.TrainRegion = trainRegion;
                alignTool.Pattern.TrainRegionMode = CogRegionModeConstants.PixelAlignedBoundingBox;
                alignTool.Pattern.IgnorePolarity = false;

                alignTool.SearchRegion = null;

                if (useROI)
                {
                    alignTool.RunParams.SearchRegionMode = CogRegionModeConstants.PixelAlignedBoundingBox;
                    alignTool.SearchRegion = searchRegion;
                }

                alignTool.RunParams.ScoreUsingClutter = false;
                alignTool.RunParams.RunAlgorithm = CogPMAlignRunAlgorithmConstants.BestTrained;
                alignTool.RunParams.RunMode = CogPMAlignRunModeConstants.SearchImage;
                alignTool.RunParams.ApproximateNumberToFind = 1;
                alignTool.RunParams.AcceptThreshold = 0.8;
                alignTool.RunParams.ZoneAngle.Configuration = CogPMAlignZoneConstants.LowHigh;
                alignTool.RunParams.ZoneAngle.Low = -70 * (Math.PI / 180.0);
                alignTool.RunParams.ZoneAngle.High = 70 * (Math.PI / 180.0);
                alignTool.RunParams.SaveMatchInfo = true;
                alignTool.RunParams.OwnedFlexParams.SaveDeformationInfo = CogPMAlignFlexDeformationInfoConstants.Transform;
                // Diagnostics
                alignTool.LastRunRecordDiagEnable =
                    CogPMAlignLastRunRecordDiagConstants.InputImageByReference // 복사본 제외
                                                                               //| CogPMAlignLastRunRecordDiagConstants.InputImageDeepCopy // 복사본 포함
                    | CogPMAlignLastRunRecordDiagConstants.FlexDeformationGrid // 굴곡 변형 그리드 표시
                    | CogPMAlignLastRunRecordDiagConstants.SearchImageMask // 진단 검색 이미지 마스크 표시
                    | CogPMAlignLastRunRecordDiagConstants.SearchRegion // 검색 영역 표시
                    | CogPMAlignLastRunRecordDiagConstants.ResultsMatchFeatures // 매치 특징 패턴 표시
                                                                                //| CogPMAlignLastRunRecordDiagConstants.FlexUnwarpedImages // 휘지 않는 굴곡 이미지 표시
                    ;
                alignTool.LastRunRecordEnable =
                    CogPMAlignLastRunRecordConstants.ResultsBoundingBox
                    //| CogPMAlignLastRunRecordConstants.ResultsCoordinateAxes // 좌표 축 표시
                    | CogPMAlignLastRunRecordConstants.ResultsMatchRegion // 매치 영역 표시
                    | CogPMAlignLastRunRecordConstants.ResultsMatchShapeModels // 매치 형태 모델 표시
                    | CogPMAlignLastRunRecordConstants.ResultsOrigin // 원점 표시
                    ;
                alignTool.CurrentRecordEnable =
                    CogPMAlignCurrentRecordConstants.PatternFeaturesCoarse // 거칠기 표시
                    | CogPMAlignCurrentRecordConstants.PatternFeaturesFine // 곱기 표시
                    | CogPMAlignCurrentRecordConstants.TrainImageMask // 트레인 이미지 마스크 표시
                    | CogPMAlignCurrentRecordConstants.TrainShapeModels // 트레인 형태 모델 표시
                    | CogPMAlignCurrentRecordConstants.SearchImageMask // 검색 이미지 마스크 표시
                    ;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void Train(CogImage8Grey input, CogImage8Grey pattern, Point2d translation)
        {
            try
            {
                if (input != null)
                    alignTool.InputImage = input;

                alignTool.Pattern.TrainImage = (CogImage8Grey)pattern;

                CogRectangle trainRegion = (CogRectangle)alignTool.Pattern.TrainRegion;
                trainRegion.Interactive = true;
                trainRegion.SetXYWidthHeight(0, 0, pattern.Width, pattern.Height);
                trainRegion.GraphicDOFEnable = CogRectangleDOFConstants.All;

                alignTool.Pattern.Origin.TranslationX = translation.x;
                alignTool.Pattern.Origin.TranslationY = translation.y;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void Set(CogImage8Grey input, CogImage8Grey pattern, bool useROI, Rect4d roi, Point2d translation, CogImage8Grey mask)
        {
            try
            {
                Create(useROI);
                alignTool.Pattern.TrainImageMask = mask == null ? null : mask;
                Train(input, pattern, translation);

                if (useROI)
                {
                    CogRectangle searchRegion = (CogRectangle)alignTool.SearchRegion;
                    searchRegion.Interactive = true;
                    searchRegion.SetXYWidthHeight(roi.left, roi.top, roi.Width(), roi.Height());
                    searchRegion.GraphicDOFEnable = CogRectangleDOFConstants.All;

                    alignTool.RunParams.SearchRegionMode = CogRegionModeConstants.PixelAlignedBoundingBox;
                }
                else
                    alignTool.SearchRegion = null;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void Display(ref CogDisplay display, bool interactiveClear, bool staticClear, bool fit, bool scoreOn, string text = "")
        {
            try
            {
                if (interactiveClear)
                    display.InteractiveGraphics.Clear();
                if (staticClear)
                    display.StaticGraphics.Clear();

                if (alignTool.SearchRegion != null)
                    display.StaticGraphics.Add((ICogGraphicInteractive)alignTool.SearchRegion, "Search Region");

                CogPMAlignResults results = alignTool.Results;

                if (results != null && results.Count > 0)
                {
                    foreach (CogPMAlignResult result in results)
                    {
                        CogCompositeShape shape = result.CreateResultGraphics(
                            CogPMAlignResultGraphicConstants.Origin
                            | CogPMAlignResultGraphicConstants.MatchRegion
                            | CogPMAlignResultGraphicConstants.MatchFeatures
                            );
                        display.StaticGraphics.Add(shape, "PMAlign" + result.ID.ToString());

                        if (scoreOn)
                        {
                            double dx = result.GetPose().TranslationX + 2 - alignTool.Pattern.Origin.TranslationX;
                            double dy = result.GetPose().TranslationY + 2 - alignTool.Pattern.Origin.TranslationY;
                            double score = result.Score * 100;

                            message.Display(ref display, 10, dx, dy, CogGraphicLabelAlignmentConstants.TopLeft, CogColorConstants.Green, text + score.ToString("##0.00") + "%", ".");
                        }
                    }
                }

                if (fit)
                    display.Fit(false);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void Search(CogImage8Grey input, CogImage8Grey pattern, double acceptThreshold, Rect4d roi, Point2d translation, bool useMask, CogImage8Grey mask)
        {
            try
            {
                if (useMask)
                {
                    Set(input, pattern, true, roi, translation, mask);
                }

                alignTool.RunParams.ScoreUsingClutter = false;
                alignTool.RunParams.AcceptThreshold = acceptThreshold;
                alignTool.InputImage = input;
                alignTool.Run();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private PMAlignResult ResultOne(int index, CogToolClassify classify, string good_class_name, ref ICogImage save_pattern_image, ref string result_class_name)
        {
            if (alignTool.Results == null)
                return null;
            CogPMAlignResults alignResults = alignTool.Results;
            int alignCount = alignResults.Count;
            if (alignCount <= index)
                return null;

            double pat_width = 0;
            double pat_height = 0;
            double pat_corner_distance_x = 0;
            double pat_corner_distance_y = 0;

            if (classify != null)
            {
                if (alignTool.Pattern.TrainRegion is CogRectangleAffine)
                {
                    var pat_corner = alignTool.Pattern.TrainRegion as CogRectangleAffine;
                    if (pat_corner != null)
                    {
                        pat_width = pat_corner.CornerOppositeX - pat_corner.CornerOriginX;
                        pat_height = pat_corner.CornerOppositeY - pat_corner.CornerOriginY;
                        pat_corner_distance_x = pat_corner.CornerOriginX - alignTool.Pattern.Origin.TranslationX;
                        pat_corner_distance_y = pat_corner.CornerOriginY - alignTool.Pattern.Origin.TranslationY;
                    }
                }

                var transform = alignResults[index].GetPose();
                double x = transform.TranslationX + pat_corner_distance_x;
                double y = transform.TranslationY + pat_corner_distance_y;

                //var crop = new CogToolCrop(alignTool.InputImage, x, y, pat_width, pat_height, transform);// x, y, pat_width, pat_height);
                var crop = new CogToolCrop(alignTool.InputImage, pat_corner_distance_x, pat_corner_distance_y, pat_width, pat_height, transform);// x, y, pat_width, pat_height);

                //string folderPath = filepath + @"\";
                //folderPath += DateTime.Now.ToString("yyyy-MM-dd") + @"\";
                bool NGClass = true;

                classify.Run(crop.GetResult());
                List<ClassificationResult> classification = classify.GetResult();
                if (classification == null)
                {
                    result_class_name = "Unknown";
                }
                else
                {
                    result_class_name = $"{classification[0].classname}";

                    if ((classification[0].classname == good_class_name || good_class_name == "ALL") && classification[0].score > 0.5)
                        NGClass = false;
                }

                save_pattern_image = crop.GetResult();

                //if (save_pattern_image)
                //{
                //    if (!Directory.Exists(folderPath))
                //        Directory.CreateDirectory(folderPath);
                //    string filePath = folderPath + @"\" + DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss-ffff") + ".bmp";

                //    CogImageFileBMP bmp = new CogImageFileBMP();
                //    bmp.Open(filePath, CogImageFileModeConstants.Write);
                //    bmp.Append(crop.GetResult());
                //    bmp.Close();
                //}

                if (NGClass)
                    return null;
            }

            PMAlignResult alignresult = new PMAlignResult();
            alignresult.angle = CogMisc.RadToDeg(alignResults[index].GetPose().Rotation);
            alignresult.score = alignResults[index].Score;
            alignresult.pos.x = alignResults[index].GetPose().TranslationX;
            alignresult.pos.y = alignResults[index].GetPose().TranslationY;
            alignresult.classname = result_class_name;

            return alignresult;
        }

        public List<PMAlignResult> GetResult(int index, CogToolClassify classify, string good_class_name, ref ICogImage save_pattern_image, ref string result_class_name, double angleMin = -180, double angleMax = 180)
        {
            results.Clear();
            if (alignTool == null)
                return null;
            if (alignTool.Results == null)
                return null;

            Point2d alignPos = new Point2d();
            string regionType = alignTool.Pattern.TrainRegion.GetType().Name;

            CogPMAlignResults alignResults = alignTool.Results;
            int alignCount = alignResults.Count;
            if (alignCount < 1)
                return results;

            if (index < 0)
            {
                for (int i = 0; i < alignResults.Count; i++)
                {
                    double angle = 180 * alignResults[i].GetPose().Rotation / Math.PI;
                    if ((angle > angleMin && angle < angleMax) ||
                        (angle > angleMin - 360 && angle < angleMax - 360) ||
                        (angle > angleMin + 360 && angle < angleMax + 360))
                    {
                        var resultOne = ResultOne(i, classify, good_class_name, ref save_pattern_image, ref result_class_name);
                        if (resultOne != null)
                        {
                            resultOne.tags = tags;
                            results.Add(resultOne);
                        }

                    }
                }
            }
            else if (index < alignResults.Count)
            {
                double angle = 180 * alignResults[index].GetPose().Rotation / Math.PI;
                if ((angle > angleMin && angle < angleMax) ||
                    (angle > angleMin - 360 && angle < angleMax - 360) ||
                    (angle > angleMin + 360 && angle < angleMax + 360))
                {
                    var resultOne = ResultOne(index, classify, good_class_name, ref save_pattern_image, ref result_class_name);
                    if (resultOne != null)
                    {
                        resultOne.tags = tags;
                        results.Add(resultOne);
                    }
                }
            }

            return results;
        }

        public string MakePacketData()
        {
            string packet = "";
            if (results.Count == 0)
                return "[{RESULT:FAILED}{INSPECTIONTYPE:PM}{RESULTCOUNT:0}{EMPTY}]";

            packet = "[{RESULT:SUCCESS}{INSPECTIONTYPE:PM}{RESULTCOUNT:" + $"{results.Count}" + "}{";
            foreach (PMAlignResult data in results)
            {
                packet += $"(RESULT:OK,POSX:{data.pos.x.ToString("0.000")},POSY:{data.pos.y.ToString("0.000")},ANGLE:{data.angle.ToString("0.000")},SCORE:{(100 * data.score).ToString("0.00")})";
            }

            packet += "}]";
            return packet;
        }

        public void MakeResultData(List<string> resultList)
        {
            foreach (PMAlignResult data in results)
            {
                string resultString = $"(RESULT:OK,POSX:{data.pos.x.ToString("0.000")}, POSY:{data.pos.y.ToString("0.000")},";
                resultString += $"ANGLE:{data.angle.ToString("0.000")},SCORE:{(100 * data.score).ToString("0.00")})";
                resultList.Add(resultString);
            }
        }

        public void MakeResultData(List<string> resultList, List<PMAlignResult> modified_results)
        {
            foreach (PMAlignResult data in modified_results)
            {
                string resultString = $"(RESULT:OK,POSX:{data.pos.x.ToString("0.00000")}, POSY:{data.pos.y.ToString("0.00000")},";
                resultString += $"ANGLE:{data.angle.ToString("0.00000")},SCORE:{(100 * data.score).ToString("0.00")})";
                resultList.Add(resultString);
            }
        }
    }

    public class CogToolCrop : ICogToolBase
    {
        public CogIPOneImageTool IPOneImageTool;
        public CogAffineTransformTool transformTool;

        public CogToolCrop(ICogImage InputImage, double x, double y, double width, double height, CogTransform2DLinear transform)
        {
            tooltype = "CogAffineTransformTool";

            //IPOneImageTool = new CogIPOneImageTool();
            //IPOneImageTool.InputImage = InputImage;
            transformTool = new CogAffineTransformTool();
            transformTool.InputImage = InputImage;

            CogRectangle roi = new CogRectangle();
            roi.Interactive = true;
            roi.SelectedSpaceName = InputImage.SelectedSpaceName;
            roi.SelectedLineWidthInScreenPixels = 1;
            if (width < 0) x += width;
            if (height < 0) y += height;
            roi.SetXYWidthHeight(x, y, Math.Abs(width), Math.Abs(height));
            roi.GraphicDOFEnable = CogRectangleDOFConstants.All;

            //CogTransform2DLinear linear = new CogTransform2DLinear();
            //IPOneImageTool.Region = roi;
            transformTool.Region.SetFromRectangle(roi, transform);
            //transformTool.Region.Rotation = radian;

            //IPOneImageTool.Region.SelectedSpaceName = InputImage.SelectedSpaceName;
            //IPOneImageTool.RegionMode = CogRegionModeConstants.PixelAlignedBoundingBox;
        }

        public ICogImage GetResult()
        {
            //if (IPOneImageTool.InputImage == null)
            //    return null;
            //IPOneImageTool.Run();
            //return IPOneImageTool.OutputImage;
            if (transformTool.InputImage == null)
                return null;
            transformTool.Run();
            return transformTool.OutputImage;
        }

    }

    public class CogToolPMAlignMulti : ICogToolBase
    {
        public CogPMAlignMultiTool alignTool;
        public List<PMAlignResult> results = new List<PMAlignResult>();

        public CogToolPMAlignMulti()
        {
            tooltype = "CogPMAlignMultiTool";
            alignTool = new CogPMAlignMultiTool();
        }

        public CogToolPMAlignMulti(CogPMAlignMultiTool aligntool)
        {
            alignTool = aligntool;
        }

        public List<PMAlignResult> GetResult(bool bestOnly)
        {
            results.Clear();

            Point2d alignPos = new Point2d();
            //string regionType = alignTool.Pattern.TrainRegion.GetType().Name;

            //if ("CogRectangle" == alignTool.Pattern.TrainRegion.GetType().Name)
            //{
            //    CogRectangle trainRegion = (CogRectangle)alignTool.Pattern.TrainRegion;
            //    alignPos.x = trainRegion.X - trainRegion.CenterX;
            //    alignPos.y = trainRegion.Y - trainRegion.CenterY;
            //}

            //if ("CogRectangleAffine" == alignTool.Pattern.TrainRegion.GetType().Name)
            //{
            //    CogRectangleAffine trainRegion = (CogRectangleAffine)alignTool.Pattern.TrainRegion;
            //    alignPos.x = trainRegion.CornerOriginX - trainRegion.CenterX;
            //    alignPos.y = trainRegion.CornerOriginY - trainRegion.CenterY;
            //}

            if (alignTool.Results == null)
                return null;

            CogPMAlignMultiResults alignResults = alignTool.Results;
            int alignCount = alignResults.PMAlignResults.Count;
            if (alignCount < 1)
                return null;

            if (bestOnly)
            {
                PMAlignResult alignresult = new PMAlignResult();
                alignresult.angle = CogMisc.RadToDeg(alignResults.PMAlignResults[0].GetPose().Rotation);
                alignresult.score = alignResults.PMAlignResults[0].Score;
                alignresult.pos.x = alignResults.PMAlignResults[0].GetPose().TranslationX + alignPos.x;
                alignresult.pos.y = alignResults.PMAlignResults[0].GetPose().TranslationY + alignPos.y;
                results.Add(alignresult);
            }
            else
            {
                foreach (CogPMAlignResult result in alignResults.PMAlignResults)
                {
                    PMAlignResult alignresult = new PMAlignResult();
                    alignresult.angle = CogMisc.RadToDeg(result.GetPose().Rotation);
                    alignresult.score = result.Score;
                    alignresult.pos.x = result.GetPose().TranslationX + alignPos.x;
                    alignresult.pos.y = result.GetPose().TranslationY + alignPos.y;
                    results.Add(alignresult);
                }
            }

            return results;
        }
    }

    public class CogToolGraphic : ICogToolBase
    {
        public void Crossline(string title, CogDisplay display, Point2d center, Size2d length,
            CogColorConstants color, CogGraphicLineStyleConstants style, bool clear = false, bool fit = true)
        {
            try
            {
                if (clear)
                {
                    if (display.StaticGraphics.ZOrderGroups.IndexOf(title + "LineA") > 1)
                        display.StaticGraphics.Remove(title + "LineA");
                    if (display.StaticGraphics.ZOrderGroups.IndexOf(title + "LineB") > 1)
                        display.StaticGraphics.Remove(title + "LineB");
                }

                // 가로선
                CogLine LineA = new CogLine();
                LineA.Color = color;
                LineA.GraphicDOFEnable = CogLineDOFConstants.All;
                LineA.Interactive = true;
                LineA.LineStyle = style;
                LineA.SelectedSpaceName = ".";
                LineA.SetFromStartXYEndXY(center.x - 0.5 * length.width, center.y, center.x + 0.5 * length.width, center.y);
                display.InteractiveGraphics.Add(LineA, title + "LineA",true);
                // 세로선
                CogLine LineB = new CogLine();
                LineB.Color = color;
                LineB.GraphicDOFEnable = CogLineDOFConstants.All;
                LineB.Interactive = true;
                LineB.LineStyle = style;
                LineB.SelectedSpaceName = ".";
                LineB.SetFromStartXYEndXY(center.x, center.y - 0.5 * length.height, center.x, center.y + 0.5 * length.height);
                display.InteractiveGraphics.Add(LineB, title + "LineB", true);

                if (fit)
                    display.Fit(false);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }

    public class CogToolBlob : ICogToolBase
    {
        public CogBlobTool blobTool;
        public List<BlobResult> results = new List<BlobResult>();

        public CogToolBlob()
        {
            tooltype = "CogBlobTool";
            blobTool = new CogBlobTool();
        }

        public CogToolBlob(CogBlobTool blobtool)
        {
            blobTool = blobtool;
        }

        public List<BlobResult> GetResult()
        {
            results.Clear();

            CogBlobResultCollection blobResults = blobTool.Results.GetBlobs();

            int blobCount = blobResults.Count;
            foreach (CogBlobResult result in blobResults)
            {
                BlobResult blobresult = new BlobResult();
                blobresult.center.x = result.CenterOfMassX;
                blobresult.center.y = result.CenterOfMassY;
                blobresult.area = result.Area;
                results.Add(blobresult);
            }

            return results;
        }

        public string MakePacketData()
        {
            string packet = "";
            if (results.Count == 0)
                return "[{RESULT:FAILED}{INSPECTIONTYPE:BLOB}{RESULTCOUNT:0}{EMPTY}]";

            packet = "[{RESULT:SUCCESS}{INSPECTIONTYPE:BLOB}{RESULTCOUNT:" + $"{results.Count}" + "}{";
            foreach (BlobResult data in results)
            {
                packet += $"(RESULT:OK,CENX:{data.center.x},CENY:{data.center.y},WIDTH:n/a,HEIGHT:n/a,AREA:{data.area})";
            }

            packet += "}]";
            return packet;
        }

        public void MakeResultData(List<string> resultList)
        {
            foreach (BlobResult data in results)
            {
                string resultString = $"(RESULT:OK,CENX:{data.center.x},CENY:{data.center.y},WIDTH:n/a,HEIGHT:n/a,AREA:{data.area})";
                resultList.Add(resultString);
            }
        }
    }

    public class CogToolID : ICogToolBase
    {
        public CogIDTool idTool;
        public List<IDResult> results = new List<IDResult>();

        public CogToolID()
        {
            tooltype = "CogIDTool"; 
            idTool = new CogIDTool();
        }

        public CogToolID(CogIDTool idtool)
        {
            idTool = idtool;
        }

        public List<IDResult> GetResult()
        {
            results.Clear();

            CogIDResults idResults = idTool.Results;

            int blobCount = idResults.Count;
            foreach (CogIDResult result in idResults)
            {
                IDResult idresult = new IDResult();
                idresult.center.x = result.CenterX;
                idresult.center.y = result.CenterY;
                idresult.angle = result.Angle;
                idresult.text = result.DecodedData.DecodedString;
                results.Add(idresult);
            }

            return results;
        }

        public string MakePacketData()
        {
            string packet = "";
            if (results.Count == 0)
                return "[{RESULT:FAILED}{INSPECTIONTYPE:ID}{RESULTCOUNT:0}{EMPTY}]";

            packet = "[{RESULT:SUCCESS}{INSPECTIONTYPE:ID}{RESULTCOUNT:" + $"{results.Count}" + "}{";
            foreach (IDResult data in results)
            {
                packet += $"(RESULT:OK,TEXT:{data.text})";
            }

            packet += "}]";
            return packet;
        }
    }

    public class CogToolOCRMax : ICogToolBase
    {
        public CogOCRMaxTool ocrTool;
        public OCRResult result = new OCRResult();

        public CogToolOCRMax()
        {
            tooltype = "CogOCRMaxTool";
            ocrTool = new CogOCRMaxTool();
        }

        public CogToolOCRMax(CogOCRMaxTool ocrTool)
        {
            this.ocrTool = ocrTool;
        }

        public OCRResult GetResult()
        {
            result.text = "";

            CogOCRMaxLineResult ocrResults = ocrTool.LineResult;
            result.text = ocrResults.ResultString;

            return result;
        }
    }
}
