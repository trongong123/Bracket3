using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopEng.Type;
using System.IO;
using System.Diagnostics;
using System.Threading;
using TopEng.Utils;
using TopEng.Vision;
using Cognex.VisionPro;
using Cognex.VisionPro.PMAlign;
using System.Windows.Forms;
using CAMASSEMBLYMACHINE.Define;
using static CAMASSEMBLYMACHINE.Define.SystemDefine;

namespace CAMASSEMBLYMACHINE
{
    public enum TOOL_TYPE
    {
        LEFT = 0,
        RIGHT = 1,
        MAX
    }

    public class CameraCalibration
    {
        public CogToolBlockHandler cogToolBlkProc;
        public Point2d[] toolPixelValue = new Point2d[(int)TOOL_TYPE.MAX];
        public static double INVALID_DATA = -999999999;
        public bool isReceiveData = false;
        public string targetName = "";

        private Thread JobThread = null;
        public ICogImage InputImage = null;
        bool requestInspection = false;

        public double CamWidth = 0;
        public double CamHeight = 0;

        public CameraCalibration(CogToolBlockHandler CheckerBoardTool, string cameraname)
        {
            targetName = cameraname;
            cogToolBlkProc = CheckerBoardTool;
        }

        public void Clear()
        {
            toolPixelValue[(int)TOOL_TYPE.LEFT] = null;
            toolPixelValue[(int)TOOL_TYPE.RIGHT] = null;
            isReceiveData = false;
        }

        public void Run(ICogImage input, int No, Point2d pos, bool Send = false)
        {
            if (requestInspection || JobThread != null)
                return;

            requestInspection = true;

            Clear();
            try
            {
                InputImage = input;

                JobThread = new Thread(new ParameterizedThreadStart(OnThreadJobProc));
                JobThread.Priority = ThreadPriority.Normal;
                JobThread.Start(new object[] { No, Send });

                foreach (ProcessThread proc in System.Diagnostics.Process.GetCurrentProcess().Threads)
                    proc.ProcessorAffinity = System.Diagnostics.Process.GetCurrentProcess().ProcessorAffinity;
            }
            catch (Exception e)
            {
                LogUtil.Instance.Log(LOG_TYPE.DEVELOPMENT, e.Message, CONTENT_TYPE.EXCEPTION);
            }
        }

        public bool ThreadStateIDLE()
        {
            if (!requestInspection)
                return true;

            try
            {
                var Thread = JobThread;
                if (Thread == null)
                {
                    requestInspection = false;
                }
                else
                {
                    if (!Thread.IsAlive)
                        requestInspection = false;
                }
            }
            catch
            {
                return !requestInspection;
            }

            return !requestInspection;
        }

        public void SetHeadTarget(TOOL_TYPE toolType)
        {
            switch (toolType)
            {
                case TOOL_TYPE.LEFT:
                    cogToolBlkProc.ChangeInputValue("USE_LEFT", true);
                    cogToolBlkProc.ChangeInputValue("USE_RIGHT", false);
                    break;
                case TOOL_TYPE.RIGHT:
                    cogToolBlkProc.ChangeInputValue("USE_LEFT", false);
                    cogToolBlkProc.ChangeInputValue("USE_RIGHT", true);
                    break;
                case TOOL_TYPE.MAX:
                    cogToolBlkProc.ChangeInputValue("USE_LEFT", true);
                    cogToolBlkProc.ChangeInputValue("USE_RIGHT", true);
                    break;

            }
        }

        private void OnThreadJobProc(object pObject)
        {
            try
            {
                cogToolBlkProc.Run(InputImage);
                
                double L_X = (double)cogToolBlkProc.GetOutPuts("L_X");
                double L_Y = (double)cogToolBlkProc.GetOutPuts("L_Y");
                double R_X = (double)cogToolBlkProc.GetOutPuts("R_X");
                double R_Y = (double)cogToolBlkProc.GetOutPuts("R_Y");
                if (L_X != INVALID_DATA && L_Y != INVALID_DATA 
                    && !Vision.IsSameWithLastData(CAMERA.TRAY, TOOL_TYPE.LEFT, new Point2d(L_X, L_Y)))
                    toolPixelValue[(int)TOOL_TYPE.LEFT] = new Point2d(L_X, L_Y);
                if (R_X != INVALID_DATA && R_Y != INVALID_DATA
                    && !Vision.IsSameWithLastData(CAMERA.TRAY, TOOL_TYPE.RIGHT, new Point2d(R_X, R_Y)))
                    toolPixelValue[(int)TOOL_TYPE.RIGHT] = new Point2d(R_X, R_Y);
                isReceiveData = true;

                InputImage = null;
            }
            catch (Exception e)
            {
                LogUtil.Instance.Log(LOG_TYPE.DEVELOPMENT, e.Message, CONTENT_TYPE.EXCEPTION);
            }

            GC.Collect();
            JobThread = null;
            requestInspection = false;
        }
    }
}
