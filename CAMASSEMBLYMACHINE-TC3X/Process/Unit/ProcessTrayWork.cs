using static CAMASSEMBLYMACHINE.Define.SystemDefine;
using CAMASSEMBLYMACHINE.UI;
using TopEng.Device;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CAMASSEMBLYMACHINE.Define;
using TopEng.Type;
using System.Threading;
using TopEng.Vision;
using TopEng.Utils;

namespace CAMASSEMBLYMACHINE.Process
{
    public class ProcessTrayWork : IProcess
    {
        public enum AUTOSTEP
        {
            ERROR,
            IDLE,
            STOP,
            COMPLETE,
            INIT,
            INIT_CHECK,
            OTHER_PROCESS_INIT_WAIT,
            PROC_SELECT_FOR_RESTART,
            LOADING,
            LOADING_COMPL_CHECK,
            LOCATION,
            LOCATION_COMPL_CHECK,
            ALIGNMENT,
            ALIGNMENT_COMPL_CHECK,
            PICKUP,
            PICKUP_COMPL_CHECK,
            UNLOADING,
            UNLOADING_COMPL_CHECK,
        }

        public enum STEP
        {
            ERROR = -1,
            IDLE,
            STOP,

            INIT_PRODUCT_DATA_MISMATCH_CHECK,

            CONV_STOP_WITH_DELAY,
            CONV_STOP_WITH_DELAY_CHECK,
            REQUEST_VISIOTN_ALIGN,
            REQUEST_VISIOTN_ALIGN_CHECK,
            LIGHT_OFF,

            MOVE_Z_POS_CHECK,
            MOVE_Z_LOADING_POS,
            MOVE_Z_UNLOADING_POS,
            MOVE_Z_DOWN_LOCATE_TRAY,
            MOVE_Z_WORK_POS,
            MOVE_Z_WORK_POS_DOWN,
            MOVE_Z_UP_DETECT_TRAY,
            MOVE_Z_UP_FOR_UNLOADING,
            MOVE_Z_UP_TRAY_LIMIT_CHECK,
            MOVE_Z_DOWN_TRAY_LOC_CHECK,

            IF_LOADING_POSSIBLE_WAIT,
            IF_LOADING_BUSY,
            IF_LOADING_COMPLETE_CHECK,
            IF_UNLOADING_REQUEST,
            IF_UNLOADING_BUSY,
            IF_UNLOADING_COMPLETE_CHECK,
            IF_REVERSE_UNLOADING_REQUEST,
            IF_REVERSE_UNLOADING_BUSY,
            IF_REVERSE_UNLOADING_COMPLETE_CHECK,

            CONV_RUN,
            CONV_RUN_REVERSE,
            CONV_STOP,
            INIT_CONV_RUN,
            INIT_CONV_STOP,

            IF_PICKUP_REQUEST,
            IF_PICKUP_BUSY,
            IF_PICKUP_COMPLETE_CHECK,

            IF_PICKUP_ADJUST_HEIGHT,
            IF_PICKUP_ADJUST_HEIGHT_COMPLETE_CHECK,
        }

        public enum MSG
        {
            MSG_SINGLE,

            MSG_PROCESS_INIT,
            MSG_LOADING_PRODUCT,
            MSG_LOCATION_PRODUCT,
            MSG_ALIGNMENT_PRODUCT,
            MSG_PICKUP_PRODUCT,
            MSG_UNLOADING_PRODUCT,
            MSG_REVERSE_TRANSFER_PRODUCT
        }

        #region PROCESS_STEP_VARIABLES
        public List<STEP> StepList = new List<STEP>();
        public STEP Step = STEP.IDLE;
        public int StepIndex = 0;
        public AUTOSTEP AutoStep = AUTOSTEP.IDLE;
        public override Type StepEnumType => typeof(STEP);
        public override Type MsgEnumType => typeof(MSG);

        public override bool Busy()
        {
            return (AutoStep != AUTOSTEP.IDLE || Step != STEP.IDLE) && (AutoStep != AUTOSTEP.STOP || Step != STEP.STOP) && (AutoStep != AUTOSTEP.ERROR || Step != STEP.ERROR);
        }

        public override bool Ready()
        {
            return Step == STEP.IDLE || Step == STEP.STOP;
        }

        public override bool Stopped()
        {
            return (AutoStep == AUTOSTEP.STOP && Step == STEP.STOP);
        }

        public override bool Error()
        {
            return (AutoStep == AUTOSTEP.ERROR || Step == STEP.ERROR);
        }

        public STEP NextStep()
        {
            if (++StepIndex < StepList.Count())
                Step = StepList[StepIndex];
            else
                Step = STEP.IDLE;
            return Step;
        }

        public override int GetStep() { return (int)Step; }
        public override int GetAutoStep() { return (int)AutoStep; }

        public override void AutoStart()
        {
            stopBit = false;
            AutoStep = AUTOSTEP.INIT;
        }

        public override void EStop(bool pause)
        {
            stopBit = true;

            for (int i = 0; i < StepList.Count; i++)
                StepList[i] = STEP.IDLE;

            AutoStep = AUTOSTEP.IDLE;
            Step = STEP.IDLE;

            Machine.Trays[(int)UNITTRAY.TRAY_WORK].ClearInterface();
            Machine.IO.SetOut((int)DO.TRAY_WORK_CONV_RUN, 0);
            Machine.IO.SetOut((int)DO.TRAY_WORK_CONV_RUN_REVERSE, 0);
            Machine.motion.AxisStop((int)AXIS.TRAY_WORK_Z, true);
        }

        public override void Stop()
        {
            Machine.IO.SetOut((int)DO.TRAY_WORK_CONV_RUN, 0);
            Machine.IO.SetOut((int)DO.TRAY_WORK_CONV_RUN_REVERSE, 0);
            Machine.motion.AxisStop((int)AXIS.TRAY_WORK_Z, true);
            stopBit = true;
        }

        public bool CheckStopBit()
        {
            if (stopBit)
            {
                Step = STEP.IDLE;
                AutoStep = AUTOSTEP.IDLE;
                return true;
            }
            return false;
        }
        #endregion

        public Point2d[] productPosition = new Point2d[2];
        public double[] productAngle = { 0, 0 };
        public int[] alignSucs = { -1, -1 };
        private bool receivedData = false;
        private int dryRunLoadingCount = 0;
        public bool doingAlignment = false;
        public int adjustHeightCount = 0;
        public Dictionary<double, int> angleList = new Dictionary<double, int>();
        public double currentTrayAlignPos = 0;
        public string trayAngle = string.Empty;
        public int vision_fail_count = 0;

        public int tray_alignment_retry = 0;
        public int currentProductCount = 0;
        public int targetCurr = 0;
        public ProcessTrayWork()
        {
            for (int i = 0; i < productPosition.Length; i++)
                productPosition[i] = new Point2d();
        }

        public void cbInspectionResultEvent(string inspname, Dictionary<string, List<VisionResult>> list, bool success)
        {
            Machine.Parts[(int)UNITPART.TRAY].productCount = 0;
            CheckVisionData(list, TOOL_TYPE.LEFT);
            CheckVisionData(list, TOOL_TYPE.RIGHT);
            receivedData = true;
        }

        private void CheckVisionData(Dictionary<string, List<VisionResult>> list, TOOL_TYPE toolType)
        {
            int currentTool = (int)toolType;
            try
            {
                if (list["OutPuts"].Count < 1) throw new Exception("Outputs data empty");
                var tempresult = list["OutPuts"][0] as OutPutsResult;

                if (Machine.isUseProductCount)
                {
                    trayAngle = tempresult.Ldirection;
                    currentProductCount = tempresult.currentProductCount;
                }
                LogUtil.Instance.Log(LOG_TYPE.INSPECTION, $"Current product count in Tray {tempresult.targetProductCount}", CONTENT_TYPE.INFO);

                if (toolType == TOOL_TYPE.RIGHT && (tempresult.Rcenter.x == Vision.INVALID_DATA || tempresult.Rcenter.y == Vision.INVALID_DATA)) throw new Exception("Cognex not return data");
                if (toolType == TOOL_TYPE.LEFT && (tempresult.Lcenter.x == Vision.INVALID_DATA || tempresult.Lcenter.y == Vision.INVALID_DATA)) throw new Exception("Cognex not return data");

                double lowScore = Machine.param.Option(ParameterDefine.OPTION.TRAY_WORK_CLASSIFY_LOW_SCORE);
                double currentScore = toolType == TOOL_TYPE.LEFT ? tempresult.LClassifyScore : tempresult.RClassifyScore;
                string currentClass = toolType == TOOL_TYPE.LEFT ? tempresult.LClassifyClass : tempresult.RClassifyClass;

                LogUtil.Instance.Log(LOG_TYPE.INSPECTION, $"TRAY HEAD_{toolType}: ClassifyClass {currentClass}, ClassifyScore {currentScore}", CONTENT_TYPE.INFO);
                
                if (toolType == TOOL_TYPE.LEFT)
                {
                    if (tempresult.LClassifyScore <= lowScore)
                    {
                        CaptureUtil.CAPTURETYPE type = tempresult.LClassifyClass == "OK" ? CaptureUtil.CAPTURETYPE.LOW_SCORE_OK : CaptureUtil.CAPTURETYPE.LOW_SCORE_NG;
                        Vision.CaptureByInput("TRAY", type, Vision.cogTool[0].clImageLeft);
                        SetError(ECODE.TRAY_LEFT_CLASSIFY_LOW_SCORE);
                        return;
                    }
                    else Vision.CaptureByInput("TRAY", CaptureUtil.CAPTURETYPE.CLASSIFY_OK, Vision.cogTool[0].clImageLeft);
                }

                if (toolType == TOOL_TYPE.RIGHT)
                {
                    if (tempresult.RClassifyScore <= lowScore)
                    {
                        CaptureUtil.CAPTURETYPE type = tempresult.RClassifyClass == "OK" ? CaptureUtil.CAPTURETYPE.LOW_SCORE_OK : CaptureUtil.CAPTURETYPE.LOW_SCORE_NG;
                        Vision.CaptureByInput("TRAY", type, Vision.cogTool[0].clImageRight);
                        SetError(ECODE.TRAY_RIGHT_CLASSIFY_LOW_SCORE);
                        return;
                    }
                    else Vision.CaptureByInput("TRAY", CaptureUtil.CAPTURETYPE.CLASSIFY_OK, Vision.cogTool[0].clImageRight);
                }

                productPosition[currentTool].x = toolType == TOOL_TYPE.LEFT ? tempresult.Lcenter.x : tempresult.Rcenter.x;
                productPosition[currentTool].y = toolType == TOOL_TYPE.LEFT ? tempresult.Lcenter.y : tempresult.Rcenter.y;

                double adjustDir = 0;
                if (trayAngle == "DEG0") adjustDir += 0;
                else if (trayAngle == "DEG90") adjustDir += 90;
                else if (trayAngle == "DEG180") adjustDir += 180;
                else if (trayAngle == "DEG270") adjustDir += 270;
                else throw new Exception("Angle Error");

                if (Math.Abs(tempresult.Langle) >= 9999 || Math.Abs(tempresult.Rangle) >= 9999) productAngle[currentTool] = 0;
                else productAngle[currentTool] = toolType == TOOL_TYPE.LEFT ? tempresult.Langle : tempresult.Rangle;
                productAngle[currentTool] += adjustDir;
                alignSucs[currentTool] = 0;


                if (Vision.IsSameWithLastData(CAMERA.TRAY, (TOOL_TYPE)currentTool, new Point2d(productPosition[currentTool].x, productPosition[currentTool].y)))
                    throw new Exception("New data is same with last data");
            }
            catch (Exception ex)
            {
                LogUtil.Instance.Log(LOG_TYPE.INSPECTION, $"Exception occured in Tray Insp {ex.Message}", CONTENT_TYPE.INFO);
                productPosition[currentTool].x = 0.0;
                productPosition[currentTool].y = 0.0;
                productAngle[currentTool] = 0.0;
                alignSucs[currentTool] = -1;
            }
            if (alignSucs[currentTool] >= 0) Machine.Parts[(int)UNITPART.TRAY].productCount++;
            LogUtil.Instance.Log(LOG_TYPE.INSPECTION, 
                $"Tray Inspection Data: " +
                $"[alignSucs: {alignSucs[currentTool]}]"+
                $"[target Head: {currentTool}]" +
                $"[x: {productPosition[currentTool].x}, y: {productPosition[currentTool].y}, r: {productAngle[currentTool]}]", CONTENT_TYPE.INFO);
        }

        public override void TimeOut()
        {

        }

        private void SetError(ECODE error)
        {
            if (error_proc)
                return;
            error_proc = true;

            // ERROR MESSAGE
            switch ((ECODE)error)
            {
                case ECODE.TRAY_NO_EXIST_LEFT_MATERIALS:
                case ECODE.TRAY_NO_EXIST_RIGHT_MATERIALS:
                case ECODE.ALIGN_ERROR_DETECT_REMAIN_PARTS:
                    var processError = new List<STEP>()
                    {
                        STEP.ERROR,
                    };

                    StepList = processError;
                    Step = StepList[StepIndex = 0];
                    break;

                default:
                    var defaultError = new List<STEP>()
                    {
                        STEP.ERROR,
                    };

                    StepList = defaultError;
                    Step = StepList[StepIndex = 0];
                    break;
            }

            AutoStep = AUTOSTEP.STOP;
            Machine.Alarm(error);
        }

        public override void SetMessage(int message, int step = 0)
        {
            if (!Ready() || Error())
            {
                bool MessageBoxShown = false;
                if (!MessageBoxShown)
                {
                    string msg;
                    msg = $"The equipment is running or in an error state.[{Enum.GetName(typeof(MSG), message)}]";
                    MessageBox.Show(msg);
                    MessageBoxShown = true;
                    return;
                }
            }

            switch ((MSG)message)
            {
                case MSG.MSG_SINGLE:
                    var processSingle = new List<STEP>()
                    {
                        STEP.IDLE,
                        STEP.IDLE,
                    };

                    StepList = processSingle;
                    StepList[0] = (STEP)step;
                    Step = StepList[StepIndex = 0];
                    break;

                case MSG.MSG_PROCESS_INIT:
                    var processInit = new List<STEP>()
                    {
                        STEP.INIT_CONV_RUN,
                        STEP.INIT_PRODUCT_DATA_MISMATCH_CHECK,
                        STEP.IDLE,
                    };

                    StepList = processInit;
                    Step = StepList[StepIndex = 0];
                    break;

                case MSG.MSG_LOADING_PRODUCT:
                    currentTrayAlignPos = 0;
                    Machine.Trays[(int)UNITTRAY.TRAY_WORK].ClearInterface();

                    var ProcessLoading = new List<STEP>()
                    {
                        STEP.MOVE_Z_LOADING_POS,
                        STEP.IF_LOADING_POSSIBLE_WAIT,
                        STEP.CONV_RUN,
                        STEP.IF_LOADING_BUSY,
                        STEP.IF_LOADING_COMPLETE_CHECK,
                        STEP.CONV_STOP_WITH_DELAY,
                        STEP.IDLE,
                    };

                    StepList = ProcessLoading;
                    Step = StepList[StepIndex = 0];
                    break;

                case MSG.MSG_LOCATION_PRODUCT:
                    var processLocation = new List<STEP>()
                    {
                        STEP.LIGHT_OFF,
                        STEP.MOVE_Z_UP_DETECT_TRAY,
                        STEP.MOVE_Z_DOWN_LOCATE_TRAY,
                        STEP.MOVE_Z_WORK_POS_DOWN,
                        STEP.IDLE,
                    };

                    StepList = processLocation;
                    Step = StepList[StepIndex = 0];
                    break;

                case MSG.MSG_PICKUP_PRODUCT:
                    Machine.Parts[(int)UNITPART.TRAY].ClearInterface();

                    var processPickup = new List<STEP>()
                    {
                        STEP.IF_PICKUP_REQUEST,
                        STEP.IF_PICKUP_BUSY,
                        STEP.IF_PICKUP_COMPLETE_CHECK,
                        STEP.IDLE,
                    };

                    StepList = processPickup;
                    Step = StepList[StepIndex = 0];
                    break;

                case MSG.MSG_ALIGNMENT_PRODUCT:
                    vision_fail_count = 0;
                    alignSucs[0] = -1;
                    alignSucs[1] = -1;

                    var processAlign = new List<STEP>()
                    {
                        STEP.REQUEST_VISIOTN_ALIGN,
                        STEP.IDLE,
                    };

                    StepList = processAlign;
                    Step = StepList[StepIndex = 0];
                    break;

                case MSG.MSG_UNLOADING_PRODUCT:
                    Machine.Trays[(int)UNITTRAY.TRAY_WORK].ClearInterface();

                    var processUnloading = new List<STEP>()
                    {
                        STEP.IF_UNLOADING_REQUEST,
                        STEP.IF_UNLOADING_BUSY,
                        STEP.MOVE_Z_UP_FOR_UNLOADING,
                        STEP.IF_UNLOADING_COMPLETE_CHECK,
                        STEP.IDLE,
                    };

                    StepList = processUnloading;
                    Step = StepList[StepIndex = 0];
                    break;

                case MSG.MSG_REVERSE_TRANSFER_PRODUCT:
                    Machine.Trays[(int)UNITTRAY.TRAY_IN].ClearInterface();

                    var processReversing = new List<STEP>()
                    {
                        STEP.MOVE_Z_LOADING_POS,
                        STEP.IF_REVERSE_UNLOADING_REQUEST,
                        STEP.IF_REVERSE_UNLOADING_BUSY,
                        STEP.CONV_RUN_REVERSE,
                        STEP.IF_REVERSE_UNLOADING_COMPLETE_CHECK,
                        STEP.CONV_STOP,
                        STEP.IDLE,
                    };

                    StepList = processReversing;
                    Step = StepList[StepIndex = 0];
                    break;
            }
        }

        protected override void OnProcessing()
        {
            double angleMin = -180;
            double angleMax = 180;
            double threshold = 500;
            uint ret1 = 0;

            switch ((STEP)Step)
            {
                case STEP.ERROR:
                    AutoStep = AUTOSTEP.ERROR;
                    error_proc = false;
                    timeWait[(int)TIMER.TIMEOUT].Reset();
                    break;

                case STEP.IDLE:
                    error_proc = false;
                    timeWait[(int)TIMER.TIMEOUT].Reset();
                    break;

                case STEP.STOP:
                    timeWait[(int)TIMER.TIMEOUT].Reset();
                    break;

                case STEP.INIT_PRODUCT_DATA_MISMATCH_CHECK:
                    if (Machine.status.mode != SystemMode.SystemModeDRYRUN)
                    {
                        Machine.Trays[(int)UNITTRAY.TRAY_WORK].exist = Machine.GetTraySensorSignal(TRAY_TYPE.WORK, TRAY_CV_SENSOR.END);
                    }

                    NextStep();
                    break;

                case STEP.CONV_STOP_WITH_DELAY:
                    timeWait[(int)TIMER.DELAY].Start();

                    Step = STEP.CONV_STOP_WITH_DELAY_CHECK;
                    break;

                case STEP.CONV_STOP_WITH_DELAY_CHECK:
                    if (timeWait[(int)TIMER.DELAY].Elapsed > Machine.param.Time(ParameterDefine.TIME.TRAY_CONV_STOP_DELAY_TIME))
                        Step = STEP.CONV_STOP;
                    break;

                case STEP.REQUEST_VISIOTN_ALIGN:
                    receivedData = false;

                    if (angleList.Count > 0 && Machine.sysMode == Machine.SYSMODE.AUTO && Machine.param.Option(ParameterDefine.OPTION.USE_TRAY_WORK_ANGLE_TRAIN) == 1)
                    {
                        var maxKey = angleList.Keys.Max();
                        angleMin = maxKey - 10;
                        angleMax = maxKey + 10;
                    }
                    else
                    {
                        angleMin = -180;
                        angleMax = 180;
                    }

                    if (Machine.status.mode == SystemMode.SystemModeDRYRUN)
                    {
                        doingAlignment = true;
                        alignSucs[0] = 0;
                        alignSucs[1] = 0;
                        NextStep();
                    }
                    else
                    {
                        alignSucs[0] = -1;
                        alignSucs[1] = -1;

                        doingAlignment = false;

                        Vision.ClearTrainCaptureQueue("TRAY");
                        Vision.PauseTrainCapture("TRAY", true);

                        if (!Vision.RunGrab("TRAY", false, true))
                            break;
                        bool saveTrainImage = Machine.param.Option(ParameterDefine.OPTION.USE_IMAGE_GATHERING_FOR_EL) == 1 ? true : false;
                        double pattern_overlap_length = Machine.recipe.Option(RecipeDefine.OPTION.PATTERN_OVERLAP_LENGTH);
                        if (!Vision.Run("TRAY", true, false, saveTrainImage, pattern_overlap_length, angleMin, angleMax))
                            break;
                        Step = STEP.REQUEST_VISIOTN_ALIGN_CHECK;
                        timeWait[(int)TIMER.TIMEOUT].Reset();
                    }
                    break;

                case STEP.REQUEST_VISIOTN_ALIGN_CHECK:
                    //if (CheckStopBit())
                    //    break;
                    if (timeWait[(int)TIMER.TIMEOUT].Elapsed > 5 * Machine.param.Time(ParameterDefine.TIME.VISION_TIME_OUT))
                    {
                        if (vision_fail_count >= 5)
                        {
                            SetError(ECODE.TRAY_NO_EXIST_LEFT_MATERIALS);
                            SetError(ECODE.TRAY_NO_EXIST_RIGHT_MATERIALS);
                            break;
                        }

                        vision_fail_count++;
                        Step = STEP.REQUEST_VISIOTN_ALIGN;
                        break;
                    }
                    if (!receivedData)
                        break;

                    if (Machine.Parts[(int)UNITPART.TRAY].productCount == 0)
                    {
                        Vision.PauseTrainCapture("TRAY", false);
                        Vision.Capture("TRAY", CaptureUtil.CAPTURETYPE.EMPTY, true);

                    }

                    Vision.Capture("TRAY", CaptureUtil.CAPTURETYPE.RESULT, false);
                    Vision.Capture("TRAY", CaptureUtil.CAPTURETYPE.SUCCESS, true);

                    doingAlignment = true;
                    NextStep();
                    break;

                case STEP.LIGHT_OFF:
                    Vision.Camera.LightOn((int)CAMERA.TRAY, false);
                    Thread.Sleep(100);
                    NextStep();
                    break;
            }

            OnProcessOfAutoRun();
            OnProcessOfMotion();
            OnProcessOfIO();
            OnProcessOfInterface();

            if (Machine.param.Option(ParameterDefine.OPTION.USE_PROC_LOG) == 1)
            {
                if ((int)AutoStep != lastAutoStep)
                {
                    ProcLogUtil.Instance.Log(PROCLOG_TYPE.PROCESS, $"[{this.GetType().Name}] {Enum.GetName(typeof(AUTOSTEP), AutoStep)}");
                    lastAutoStep = (int)AutoStep;
                }
                if ((int)Step != lastProcStep)
                {
                    ProcLogUtil.Instance.Log(PROCLOG_TYPE.PROCESS, $"[{this.GetType().Name}] {Enum.GetName(typeof(STEP), Step)}");
                    lastProcStep = (int)Step;
                }
            }
        }

        private void OnProcessOfAutoRun()
        {
            var proc = Machine.proclist[(int)Machine.PROCESS.PROD_LOADER] as ProcessProdLoader;
            uint ret1 = 0;
            double apos = 0;
            bool tray_empty = !Machine.Trays[(int)UNITTRAY.TRAY_OUT_BUF].exist && !Machine.Trays[(int)UNITTRAY.TRAY_IN_BUF].exist;

            switch ((AUTOSTEP)AutoStep)
            {
                case AUTOSTEP.ERROR:
                    break;

                case AUTOSTEP.IDLE:
                    if (CheckStopBit())
                        stopBit = false;
                    break;

                case AUTOSTEP.STOP:
                    break;

                case AUTOSTEP.COMPLETE:
                    if (Machine.sysMode != Machine.SYSMODE.AUTO)
                        AutoStep = AUTOSTEP.IDLE;
                    else
                        AutoStep = AUTOSTEP.LOCATION;

                    CheckStopBit();
                    break;

                case AUTOSTEP.INIT:
                    initCompl = false;
                    alignSucs[0] = -1;
                    alignSucs[1] = -1;
                    doingAlignment = false;
                    tray_alignment_retry = 0;
                    angleList.Clear();

                    Machine.Trays[(int)UNITTRAY.TRAY_WORK].ClearInterface();
                    Machine.Parts[(int)UNITPART.TRAY].ClearInterface();

                    if (Machine.status.mode == SystemMode.SystemModeBYPASS)
                    {
                        initCompl = true;
                        AutoStep = AUTOSTEP.IDLE;
                    }

                    SetMessage((int)MSG.MSG_PROCESS_INIT);
                    AutoStep = AUTOSTEP.INIT_CHECK;
                    break;

                case AUTOSTEP.INIT_CHECK:
                    if (CheckStopBit() || !Ready() || Error())
                        break;

                    initCompl = true;
                    AutoStep = AUTOSTEP.OTHER_PROCESS_INIT_WAIT;
                    break;

                case AUTOSTEP.OTHER_PROCESS_INIT_WAIT:
                    if (CheckStopBit() || Machine.GetProcInitialized() || Machine.sysMode != Machine.SYSMODE.AUTO)
                        AutoStep = AUTOSTEP.PROC_SELECT_FOR_RESTART;
                    break;

                case AUTOSTEP.PROC_SELECT_FOR_RESTART:
                    if (CheckStopBit())
                        break;
                    if (Machine.Trays[(int)UNITTRAY.TRAY_WORK].exist)
                        AutoStep = AUTOSTEP.LOCATION;
                    else
                        AutoStep = AUTOSTEP.LOADING;
                    break;

                case AUTOSTEP.LOADING:
                    if (CheckStopBit())
                        break;
                    Machine.Trays[(int)UNITTRAY.TRAY_WORK].exist = false;

                    SetMessage((int)MSG.MSG_LOADING_PRODUCT);
                    AutoStep = AUTOSTEP.LOADING_COMPL_CHECK;
                    break;

                case AUTOSTEP.LOADING_COMPL_CHECK:
                    if (CheckStopBit() || !Ready() || Error())
                        break;

                    Machine.Parts[(int)UNITPART.TRAY].exist = true;
                    dryRunLoadingCount = 0;
                    doingAlignment = false;
                    angleList.Clear();
                    AutoStep = AUTOSTEP.LOCATION;
                    break;

                case AUTOSTEP.LOCATION:
                    if (!Machine.Trays[(int)UNITTRAY.TRAY_WORK].exist)
                    {
                        AutoStep = AUTOSTEP.LOADING;
                        break;
                    }

                    Machine.IO.GetIn((int)DI.TRAY_IN_BUF_STOPPER_UP, ref ret1);
                    if (ret1 != 1 && Machine.GetTraySensorSignal(TRAY_TYPE.IN_BUF, TRAY_CV_SENSOR.END))
                    {
                        SetError(ECODE.TRAY_INBUF_STOPPER_DOWN_WHILE_TRAY_WORK_MOVING_ERROR);
                        break;
                    }

                    if (Machine.status.mode == SystemMode.SystemModeDRYRUN)
                        SetMessage((int)MSG.MSG_SINGLE, (int)STEP.MOVE_Z_WORK_POS);
                    else
                        SetMessage((int)MSG.MSG_LOCATION_PRODUCT);
                    AutoStep = AUTOSTEP.LOCATION_COMPL_CHECK;
                    break;

                case AUTOSTEP.LOCATION_COMPL_CHECK:
                    if (CheckStopBit() || !Ready() || Error())
                        break;

                    Machine.motion.GetAxisActualPos((int)AXIS.TRAY_WORK_Z, ref apos);
                    ProcLogUtil.Instance.Log(PROCLOG_TYPE.PROCESS, $"[{this.GetType().Name}] Tray Work Z Align Pos: {apos / 1000}");
                    if (Math.Abs(apos / 1000 - currentTrayAlignPos) >= 2)
                    {
                        Machine.trayNGListX.Clear();
                        Machine.trayNGListY.Clear();
                        trayAngle = string.Empty;
                        Machine.isUseProductCount = true;
                        ProcLogUtil.Instance.Log(PROCLOG_TYPE.PROCESS, $"[{this.GetType().Name}] Clear tray NG list");
                    }
                    currentTrayAlignPos = apos / 1000;
                    Machine.Parts[(int)UNITPART.TRAY].exist = true;
                    if (Machine.status.mode == SystemMode.SystemModeDRYRUN) Machine.Parts[(int)UNITPART.TRAY].productCount = 10;
                    AutoStep = AUTOSTEP.ALIGNMENT;
                    tray_alignment_retry = 0;

                    doingAlignment = false;
                    break;

                case AUTOSTEP.ALIGNMENT:
                    Machine.motion.GetAxisActualPos((int)AXIS.PROD_PICKUP_Y, ref apos);
                    if (apos > 1000 * Machine.param.Position(ParameterDefine.POSITION.PROD_LOADER_Y_TRAY_VISION_AVOID_POS))
                        break;

                    if (Machine.status.mode == SystemMode.SystemModeBYPASS)
                        AutoStep = AUTOSTEP.IDLE;
                    else
                    {
                        SetMessage((int)MSG.MSG_ALIGNMENT_PRODUCT);
                        AutoStep = AUTOSTEP.ALIGNMENT_COMPL_CHECK;
                    }
                    break;

                case AUTOSTEP.ALIGNMENT_COMPL_CHECK:
                    if (CheckStopBit() || !Ready() || Error())
                        break;

                    if (Machine.isUseProductCount)
                    {
                        Machine.isUseProductCount = false;
                        LogUtil.Instance.Log(LOG_TYPE.INSPECTION,$"" +
                            $"Check ProductCount in Tray: " +
                            $"Current: {currentProductCount}, " +
                            $"Tray Angle = {trayAngle}", CONTENT_TYPE.INFO);
                        if ((currentProductCount != (int)Machine.recipe.Option(RecipeDefine.OPTION.PRODUCT_COUNT_IN_TRAY1)
                            && currentProductCount != (int)Machine.recipe.Option(RecipeDefine.OPTION.PRODUCT_COUNT_IN_TRAY2)
                            && currentProductCount != (int)Machine.recipe.Option(RecipeDefine.OPTION.PRODUCT_COUNT_IN_TRAY3)
                            && currentProductCount != (int)Machine.recipe.Option(RecipeDefine.OPTION.PRODUCT_COUNT_IN_TRAY4)
                            && currentProductCount != (int)Machine.recipe.Option(RecipeDefine.OPTION.PRODUCT_COUNT_IN_TRAY5))
                            || trayAngle == string.Empty)
                        {
                            if (Machine.param.Option(ParameterDefine.OPTION.USE_TRAY_PRODUCT_COUNT_ALARM) == 1)
                            {
                                SetError(ECODE.MISMATCH_PRODUCT_COUNT_IN_TRAY);
                                break;
                            }
                        }
                    }
                    if (Machine.Parts[(int)UNITPART.TRAY].productCount > 0)
                    {
                        Machine.Parts[(int)UNITPART.TRAY].exist = true;
                        doingAlignment = true;

                        AutoStep = AUTOSTEP.PICKUP;
                    }
                    else
                    {
                        int pickedProduct = (Machine.trayNGListX.Count + Machine.trayNGListY.Count) / 2;
                        LogUtil.Instance.Log(LOG_TYPE.INSPECTION, $"Tray Unloading: picked {pickedProduct}, count {currentProductCount}", CONTENT_TYPE.INFO);
                        Machine.Parts[(int)UNITPART.TRAY].exist = false;
                        AutoStep = AUTOSTEP.UNLOADING;
                    }
                    break;

                case AUTOSTEP.PICKUP:
                    if (!proc.pickupOn)
                        return;

                    if (Machine.Parts[(int)UNITPART.TRAY].productCount == 0)
                    {
                        AutoStep = AUTOSTEP.ALIGNMENT;
                    }
                    else if (alignSucs[0] >= 0 && alignSucs[1] >= 0 ||
                        !Machine.Parts[(int)UNITPART.PROD_PICK1].exist && alignSucs[0] >= 0 ||
                        !Machine.Parts[(int)UNITPART.PROD_PICK2].exist && alignSucs[1] >= 0)
                    {
                        //if (Machine.interfer_tray_trasnfer_busy)
                        //    break;

                        SetMessage((int)MSG.MSG_PICKUP_PRODUCT);
                        AutoStep = AUTOSTEP.PICKUP_COMPL_CHECK;
                    }
                    else
                    {
                        if (tray_alignment_retry < 2)
                        {
                            doingAlignment = false;
                            tray_alignment_retry++;
                            AutoStep = AUTOSTEP.ALIGNMENT;
                            break;
                        }

                        Vision.PauseTrainCapture("TRAY", false);

                        if (!Machine.Parts[(int)UNITPART.PROD_PICK1].exist && alignSucs[0] < 0)
                        {
                            SetError(ECODE.TRAY_NO_EXIST_LEFT_MATERIALS);
                            break;
                        }
                        if (!Machine.Parts[(int)UNITPART.PROD_PICK2].exist && alignSucs[1] < 0)
                        {
                            SetError(ECODE.TRAY_NO_EXIST_RIGHT_MATERIALS);
                            break;
                        }

                        doingAlignment = false;
                        AutoStep = AUTOSTEP.ALIGNMENT;
                        //SetError(ECODE.ALIGN_ERROR_TRAY_NO_EXIST_RIGHT_TARGET);
                        break;
                    }
                    break;

                case AUTOSTEP.PICKUP_COMPL_CHECK:
                    if (CheckStopBit() || !Ready() || Error())
                        break;

                    Machine.Parts[(int)UNITPART.TRAY].productCount--;
                    if (0 >= Machine.Parts[(int)UNITPART.TRAY].productCount)
                    {
                        AutoStep = AUTOSTEP.ALIGNMENT;
                        break;
                    }

                    if (Machine.Parts[(int)UNITPART.PROD_PICK1].exist && Machine.Parts[(int)UNITPART.PROD_PICK2].exist)
                        AutoStep = AUTOSTEP.ALIGNMENT;
                    else
                        AutoStep = AUTOSTEP.PICKUP;
                    break;

                case AUTOSTEP.UNLOADING:
                    if (CheckStopBit() || Machine.TrayHolding)
                        break;

                    tray_alignment_retry = 0;

                    SetMessage((int)MSG.MSG_UNLOADING_PRODUCT);
                    AutoStep = AUTOSTEP.UNLOADING_COMPL_CHECK;
                    break;

                case AUTOSTEP.UNLOADING_COMPL_CHECK:
                    if (CheckStopBit() || !Ready() || Error())
                        break;

                    angleList.Clear();

                    if (Machine.status.mode == SystemMode.SystemModeDRYRUN)
                    {
                        if (dryRunLoadingCount < Machine.param.Option(ParameterDefine.OPTION.TRAY_LOADING_COUNT_FOR_DRYRUN))
                            AutoStep = AUTOSTEP.COMPLETE;
                        else
                        {
                            Machine.Trays[(int)UNITTRAY.TRAY_WORK].exist = false;
                            AutoStep = AUTOSTEP.LOADING;
                        }
                        break;
                    }

                    doingAlignment = false;
                    dryRunLoadingCount++;
                    AutoStep = AUTOSTEP.COMPLETE;
                    break;
            }

            if (AutoStep != AUTOSTEP.IDLE && AutoStep != AUTOSTEP.ERROR && AutoStep != AUTOSTEP.STOP)
                lastAutoStep = (int)AutoStep;
        }

        private void OnProcessOfInterface()
        {
            int upstream = (int)UNITTRAY.TRAY_IN_BUF;
            int current = (int)UNITTRAY.TRAY_WORK;
            int downstream = (int)UNITTRAY.TRAY_OUT_BUF;

            uint ret1 = 0;

            uint returnExist = 0;
            uint returnMid = 0;
            uint returnComplete = 0;
            uint returnReceived = 0;
            int intValue = 0;

            switch ((STEP)Step)
            {
                case STEP.IF_LOADING_POSSIBLE_WAIT:
                    if (!Machine.Trays[upstream].unloadingRequest)
                        return;

                    NextStep();
                    break;

                case STEP.IF_LOADING_BUSY:
                    if (!Machine.Trays[upstream].unloading)
                        break;
                    Machine.Trays[current].loading = true;
                    timeWait[(int)TIMER.TIMEOUT].Reset();
                    timeWait[(int)TIMER.DELAY].Reset();
                    NextStep();
                    break;

                case STEP.IF_LOADING_COMPLETE_CHECK:
                    if (Machine.status.mode == SystemMode.SystemModeDRYRUN)
                    {

                        Machine.Trays[current].loading = false;
                        Machine.Trays[current].exist = true;
                        NextStep();
                        break;
                    }

                    if (!Machine.GetTraySensorSignal(TRAY_TYPE.WORK, TRAY_CV_SENSOR.END))
                    {
                        if (timeWait[(int)TIMER.TIMEOUT].Elapsed > Machine.param.Time(ParameterDefine.TIME.CONV_RUN_TIME_OUT))
                        {
                            SetError(ECODE.TIMEOUT_TRAY_WORK_LOADING);
                            break;
                        }
                        break;
                    }

                    if (Machine.Trays[upstream].unloading)
                        break;

                    Machine.Trays[current].loading = false;
                    Machine.Trays[current].exist = true;
                    NextStep();
                    break;

                case STEP.IF_UNLOADING_REQUEST:
                    Machine.Trays[current].unloadingRequest = true;

                    NextStep();
                    timeWait[(int)TIMER.TIMEOUT].Reset();
                    timeWait[(int)TIMER.DELAY].Reset();
                    break;

                case STEP.IF_UNLOADING_BUSY:
                    Machine.Trays[current].unloading = true;
                    if (!Machine.Trays[downstream].loading)
                        break;

                    NextStep();
                    break;

                case STEP.IF_UNLOADING_COMPLETE_CHECK:
                    Machine.Trays[current].unloadingRequest = false;
                    Machine.Trays[current].unloading = false;

                    if (Machine.Trays[downstream].loading)
                        break;

                    if (!Machine.GetTraySensorSignal(TRAY_TYPE.WORK, TRAY_CV_SENSOR.IN)
                        && !Machine.GetTraySensorSignal(TRAY_TYPE.WORK, TRAY_CV_SENSOR.MID)
                        && !Machine.GetTraySensorSignal(TRAY_TYPE.WORK, TRAY_CV_SENSOR.END))
                        Machine.Trays[(int)UNITTRAY.TRAY_WORK].exist = false;

                    NextStep();
                    break;

                case STEP.IF_PICKUP_REQUEST:
                    Machine.Parts[(int)UNITPART.TRAY].unloadingRequest = true;

                    NextStep();
                    timeWait[(int)TIMER.TIMEOUT].Reset();
                    timeWait[(int)TIMER.DELAY].Reset();

                    adjustHeightCount = 0;
                    break;

                case STEP.IF_PICKUP_BUSY:
                    Machine.Parts[(int)UNITPART.TRAY].unloading = true;
                    if (!Machine.Parts[(int)UNITPART.PROD_PICK1].loading)
                    {
                        break;
                    }

                    NextStep();
                    break;

                case STEP.IF_PICKUP_COMPLETE_CHECK:
                    Machine.Parts[(int)UNITPART.TRAY].unloadingRequest = false;
                    Machine.Parts[(int)UNITPART.TRAY].unloading = false;

                    if (Machine.Parts[(int)UNITPART.PROD_PICK1].loading)
                    {
                        //if (Machine.Parts[(int)UNITPART.PROD_PICK1].adjust && adjustHeightCount < 7)
                        //    Step = STEP.IF_PICKUP_ADJUST_HEIGHT;
                        break;
                    }

                    //Machine.Parts[(int)UNITPART.TRAY].productCount--;
                    NextStep();
                    break;

                case STEP.IF_PICKUP_ADJUST_HEIGHT:
                    Machine.motion.MoveAxisInc((int)AXIS.TRAY_WORK_Z, 2 * 1000, 5 * 1000, 10 * 1000, 10 * 1000);
                    Step = STEP.IF_PICKUP_ADJUST_HEIGHT_COMPLETE_CHECK;
                    break;

                case STEP.IF_PICKUP_ADJUST_HEIGHT_COMPLETE_CHECK:
                    if (!Machine.motion.MoveAxisDoneCheck((int)AXIS.TRAY_WORK_Z))
                        break;

                    adjustHeightCount++;
                    Machine.Parts[(int)UNITPART.PROD_PICK1].adjust = false;
                    Step = STEP.IF_PICKUP_COMPLETE_CHECK;
                    break;

                case STEP.IF_REVERSE_UNLOADING_REQUEST:
                    Machine.Trays[current].reverseRequest = true;
                    Machine.Trays[current].reversing = false;

                    if (Machine.Trays[upstream].exist)
                        break;

                    timeWait[(int)TIMER.TIMEOUT].Reset();
                    timeWait[(int)TIMER.DELAY].Reset();
                    NextStep();
                    break;

                case STEP.IF_REVERSE_UNLOADING_BUSY:
                    Machine.Trays[current].reversing = true;
                    timeWait[(int)TIMER.TRIG].Reset();

                    NextStep();
                    break;

                case STEP.IF_REVERSE_UNLOADING_COMPLETE_CHECK:
                    // 자재의 상태를 확인한다.
                    // 조명에 의한 신호 일 수 있으니 0.1초 대기 한다.

                    bool ret = Machine.GetTraySensorSignal(TRAY_TYPE.WORK, TRAY_CV_SENSOR.MID) || Machine.GetTraySensorSignal(TRAY_TYPE.WORK, TRAY_CV_SENSOR.END);
                    // 더미런 시에는 항상 ON 이다.
                    if (Machine.status.mode == SystemMode.SystemModeDRYRUN) ret = true;

                    // 자재 감지가 OFF인지 CHECK 한다.
                    //if (returnExist == 0)
                    //    timeWait[(int)TIMER.TRIG].Reset();

                    // REVERSE 배출 완료 여부 확인한다.
                    if (ret)
                    {
                        Machine.Trays[current].reversing = true;
                        if (timeWait[(int)TIMER.TRIG].Elapsed < 100)
                            break;
                        timeWait[(int)TIMER.TRIG].Reset();
                    }
                    else
                    {
                        if (timeWait[(int)TIMER.TRIG].Elapsed > 1000)
                        {
                            Machine.Trays[current].reversing = false;
                            Machine.Trays[current].reverseRequest = false;
                            Machine.Trays[current].exist = false;
                            NextStep();
                        }
                    }
                    break;
            }
        }

        private void OnProcessOfMotion()
        {
            double vel = 1000 * Machine.param.Velocity(ParameterDefine.VELOCITY.TRAY_WORK_Z_VEL);
            double acc = 1000 * Machine.param.Velocity(ParameterDefine.VELOCITY.TRAY_WORK_Z_ACC);
            double dec = 1000 * Machine.param.Velocity(ParameterDefine.VELOCITY.TRAY_WORK_Z_DEC);
            double pos = 0.0;
            double apos = 0.0;

            MotionState state = MotionState.MotionStateIDLE;
            uint ret1 = 0;

            switch ((STEP)Step)
            {
                case STEP.MOVE_Z_POS_CHECK:
                    if (!Machine.motion.MoveAxisDoneCheck((int)AXIS.TRAY_WORK_Z))
                        break;

                    NextStep();
                    break;

                case STEP.MOVE_Z_LOADING_POS:
                case STEP.MOVE_Z_DOWN_LOCATE_TRAY:
                    pos = 1000 * Machine.param.Position(ParameterDefine.POSITION.TRAY_WORK_LOADING_POS);

                    if (Step == STEP.MOVE_Z_LOADING_POS) Step = STEP.MOVE_Z_POS_CHECK;
                    if (Step == STEP.MOVE_Z_DOWN_LOCATE_TRAY)
                    {
                        vel = 1000 * Machine.param.Velocity(ParameterDefine.VELOCITY.TRAY_WORK_Z_APPROACH_VEL);
                        acc = 5 * vel;
                        dec = 5 * vel;
                        Step = STEP.MOVE_Z_DOWN_TRAY_LOC_CHECK;
                    }

                    Machine.motion.MoveAxisAbs((int)AXIS.TRAY_WORK_Z, pos, vel, acc, dec);
                    break;

                case STEP.MOVE_Z_WORK_POS_DOWN:
                    if (Machine.sysMode != Machine.SYSMODE.AUTO)
                    {
                        NextStep();
                        break;
                    }

                    Machine.motion.MoveAxisInc((int)AXIS.TRAY_WORK_Z, -3 * 1000, vel, acc, dec);
                    Step = STEP.MOVE_Z_POS_CHECK;
                    break;

                case STEP.MOVE_Z_WORK_POS:
                case STEP.MOVE_Z_UP_DETECT_TRAY:
                    pos = 1000 * Machine.param.Position(ParameterDefine.POSITION.TRAY_WORK_WORK_POS);
                    Machine.motion.MoveAxisAbs((int)AXIS.TRAY_WORK_Z, pos, vel, acc, dec);

                    if (Step == STEP.MOVE_Z_WORK_POS) Step = STEP.MOVE_Z_POS_CHECK;
                    if (Step == STEP.MOVE_Z_UP_DETECT_TRAY) Step = STEP.MOVE_Z_UP_TRAY_LIMIT_CHECK;
                    break;

                case STEP.MOVE_Z_UP_FOR_UNLOADING:
                    if (Machine.status.mode == SystemMode.SystemModeDRYRUN)
                    {
                        NextStep();
                        break;
                    }

                    pos = 1000 * Machine.param.Position(ParameterDefine.POSITION.TRAY_WORK_UNLOADING_OFFSET);
                    vel = 1000 * Machine.param.Velocity(ParameterDefine.VELOCITY.TRAY_WORK_Z_APPROACH_VEL);
                    acc = 5 * vel;
                    dec = 5 * vel;
                    Machine.motion.MoveAxisInc((int)AXIS.TRAY_WORK_Z, pos, vel, acc, dec);

                    Step = STEP.MOVE_Z_POS_CHECK;
                    break;

                case STEP.INIT_CONV_RUN:
                    Machine.IO.SetOut((int)DO.TRAY_WORK_CONV_RUN, 1);
                    timeWait[(int)TIMER.DELAY].Start();
                    Step = STEP.INIT_CONV_STOP;
                    break;

                case STEP.INIT_CONV_STOP:
                    if (timeWait[(int)TIMER.DELAY].Elapsed > Machine.param.Time(ParameterDefine.TIME.TRAY_CONV_STOP_DELAY_TIME))
                    {
                        Machine.IO.SetOut((int)DO.TRAY_WORK_CONV_RUN, 0);
                        NextStep();
                    }
                    break;

                case STEP.CONV_RUN:
                    Machine.IO.SetOut((int)DO.TRAY_WORK_CONV_RUN, 1);
                    NextStep();
                    timeWait[(int)TIMER.DELAY].Start();
                    break;

                case STEP.CONV_RUN_REVERSE:
                    Machine.IO.SetOut((int)DO.TRAY_WORK_CONV_RUN_REVERSE, 1);
                    NextStep();
                    timeWait[(int)TIMER.DELAY].Start();
                    break;

                case STEP.CONV_STOP:
                    Machine.IO.SetOut((int)DO.TRAY_WORK_CONV_RUN, 0);
                    Machine.IO.SetOut((int)DO.TRAY_WORK_CONV_RUN_REVERSE, 0);

                    NextStep();
                    timeWait[(int)TIMER.DELAY].Start();
                    break;

                case STEP.MOVE_Z_UP_TRAY_LIMIT_CHECK:
                    Machine.IO.GetIn((int)DI.TRAY_WORK_DETECT_UPPER_LIMIT, ref ret1);
                    if (ret1 == 1)
                        Machine.motion.AxisStop((int)AXIS.TRAY_WORK_Z, true);

                    Machine.motion.GetAxisMotionState((int)AXIS.TRAY_WORK_Z, ref state);
                    if (state != MotionState.MotionStateIDLE)
                        break;
                    NextStep();
                    break;

                case STEP.MOVE_Z_DOWN_TRAY_LOC_CHECK:
                    Machine.IO.GetIn((int)DI.TRAY_WORK_DETECT_UPPER_LIMIT, ref ret1);
                    if (ret1 == 0)
                        Machine.motion.AxisStop((int)AXIS.TRAY_WORK_Z, true);

                    Machine.motion.GetAxisActualPos((int)AXIS.TRAY_WORK_Z, ref apos);
                    //if (apos < 1000 * Machine.param.Position(ParameterDefine.POSITION.TRAY_WORK_UNLOADING_POS))
                    //{
                    //    Machine.motion.AxisStop((int)AXIS.TRAY_WORK_Z, true);
                    //    Machine.Alarm(ECODE.POSITION_ERROR_TRAY_WORK_Z_POS);
                    //    AutoStep = AUTOSTEP.ERROR;
                    //    break;
                    //}

                    Machine.motion.GetAxisMotionState((int)AXIS.TRAY_WORK_Z, ref state);
                    if (state != MotionState.MotionStateIDLE)
                        break;
                    NextStep();
                    break;
            }
        }

        private void OnProcessOfIO()
        {
        }

        public override void SetHeadTarget(int iTarget)
        {
            this.targetCurr = iTarget;
        }
    }
}
