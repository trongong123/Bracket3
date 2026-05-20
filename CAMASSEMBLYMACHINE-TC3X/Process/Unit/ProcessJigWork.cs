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
using TopEng.Vision;
using System.Threading;
using TopEng.Utils;
using System.Text.RegularExpressions;

namespace CAMASSEMBLYMACHINE.Process
{
    public class ProcessJigWork : IProcess
    {
        public enum AUTOSTEP
        {
            // 시스템 프로세스
            ERROR,
            IDLE,
            STOP,
            RESUME,
            COMPLETE,

            // 초기화 프로세스
            INIT,
            INIT_CHECK,
            OTHER_PROCESS_INIT_WAIT,
            PROC_SELECT_FOR_RESTART,

            // 실제 자동 프로세스
            LOADING,
            LOADING_COMPL_CHECK,
            ALIGNMENT,
            ALIGNMENT_COMPL_CHECK,
            PLACE,
            PLACE_COMPL_CHECK,
            UNLOADING,
            UNLOADING_COMPL_CHECK,
        }

        public enum STEP
        {
            // 시스템 스텝
            ERROR = -1,
            IDLE,
            STOP,

            INIT_PRODUCT_DATA_MISMATCH_CHECK,

            // TIME
            CONV_STOP_INIT_WITH_DELAY,
            CONV_STOP_INIT_WITH_DELAY_CHECK,
            CONV_STOP_WITH_DELAY,
            CONV_STOP_WITH_DELAY_CHECK,
            CYLINDER_RUN_WAIT,
            CYLINDER_RUN_WAIT_CHECK,
			
            // INTERFACE
            IF_LOADING_POSSIBLE_WAIT,
            IF_LOADING_BUSY,
            IF_LOADING_COMPLETE_CHECK,
            IF_UNLOADING_REQUEST,
            IF_UNLOADING_BUSY,
            IF_UNLOADING_COMPLETE_CHECK,

            IF_PLACE_WAIT_EDM_REPORT,
            JIG_IN_BUF_Z_READY_WAIT,
            IF_PLACE_POSSIBLE_WAIT,
            IF_PLACE_BUSY,
            IF_PLACE_COMPLETE_CHECK,
            JIG_IN_BUF_Z_PRESS_MOVE,
            JIG_IN_BUF_Z_PRESS_MOVE_CHECK,
            JIG_ALIGN_PRESS_DOWN,
            JIG_ALIGN_PRESS_DOWN_CHECK,
            JIG_ALIGN_PRESS_UP,
            JIG_ALIGN_PRESS_UP_CHECK,
            JIG_IN_BUF_Z_READY_MOVE,
            JIG_IN_BUF_Z_READY_MOVE_CHECK,

            // VISION
            REQUEST_VISIOTN_ALIGN,
            REQUEST_VISIOTN_ALIGN_CHECK,
            LIGHT_OFF,

            // MOTION
            CONV_RUN,
            CONV_STOP,

            // I/O
            STOPPER_UP,
            STOPPER_UP_CHECK,
            STOPPER_DOWN,
            STOPPER_DOWN_CHECK,
            PALETTE_LOCK,
            PALETTE_LOCK_CHECK,
            PALETTE_UNLOCK,
            PALETTE_UNLOCK_CHECK,

            INIT_STOPPER_UP,
            DELAY_FOR_PALETTE,
        }

        public enum MSG
        {
            MSG_SINGLE,
            MSG_PROCESS_INIT_CONV,

            MSG_LOADING_PRODUCT,
            MSG_ALIGNMENT_PRODUCT,
            MSG_PLACE_PRODUCT,
            MSG_UNLOADING_PRODUCT,
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
            return (/*AutoStep == AUTOSTEP.STOP && */Step == STEP.STOP);
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

            timeWait[(int)TIMER.TACT].Reset();
        }

        public override void EStop(bool pause)
        {
            EStopBit = true;

            for (int i = 0; i < StepList.Count; i++)
                StepList[i] = STEP.IDLE;

            AutoStep = AUTOSTEP.IDLE;
            Step = STEP.IDLE;

            Machine.Jigs[(int)UNITJIG.JIG_WORK].ClearInterface();
            Machine.IO.SetOut((int)DO.JIG_WORK_CONV_RUN, 0);
            Machine.IO.SetOut((int)DO.JIG_WORK_PALLETE_FWD, 0);
            Machine.IO.SetOut((int)DO.JIG_WORK_PALLETE_BWD, 1);
        }

        public override void Stop()
        {
            stopBit = true;

            Machine.Jigs[(int)UNITJIG.JIG_WORK].ClearInterface();
            Machine.IO.SetOut((int)DO.JIG_WORK_CONV_RUN, 0);
            Machine.IO.SetOut((int)DO.JIG_WORK_PALLETE_FWD, 0);
            Machine.IO.SetOut((int)DO.JIG_WORK_PALLETE_BWD, 1);
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

        // 초기화 변수
        private bool resumeProc = false;

        private int[] CURRENT_PARTS = { (int)UNITPART.JIG1, (int)UNITPART.JIG2 };

        public Point2d[] productPosition = new Point2d[2];
        public double[] productAngle = { 0, 0 };
        public int[] alignSucs = { -1, -1 };
        private bool receivedData = false;
        private int targetPick = 0;
        private int[] upstreamparts = { (int)UNITPART.ASSEMBLER1, (int)UNITPART.ASSEMBLER2 };

        public bool doneAlignment = false;
        
        int vision_fail_count;
        int vision_retry_count;

        public JIG_CLASSIFY_RESULT[] jigCLResult = { JIG_CLASSIFY_RESULT.NG, JIG_CLASSIFY_RESULT.NG };
        public int current_jig_id = -1;
        public int targetCurr = 0;
        public ProcessJigWork()
        {
            for (int i = 0; i < productPosition.Length; i++)
                productPosition[i] = new Point2d();
        }

        public void cbInspectionResultEvent(string inspname, Dictionary<string, List<VisionResult>> list, bool success)
        {
            CheckVisionData(list, TOOL_TYPE.LEFT);
            CheckVisionData(list, TOOL_TYPE.RIGHT);

            Machine.Parts[(int)UNITPART.JIG1].exist = false;
            Machine.Parts[(int)UNITPART.JIG2].exist = false;
            receivedData = true;
        }

        private void CheckVisionData(Dictionary<string, List<VisionResult>> list, TOOL_TYPE toolType)
        {
            int currentTool = (int)toolType;
            try
            {
                if (list["OutPuts"].Count < 1) throw new Exception("Outputs data empty");
                var tempresult = list["OutPuts"][0] as OutPutsResult;

                int.TryParse(tempresult.sOCR_ID, out current_jig_id);

                string clResultClass = toolType == TOOL_TYPE.LEFT ? tempresult.LClassifyClass : tempresult.RClassifyClass;
                double clResultScore = toolType == TOOL_TYPE.LEFT ? tempresult.LClassifyScore : tempresult.RClassifyScore; ;

                LogUtil.Instance.Log(LOG_TYPE.INSPECTION, $"JIG HEAD_{toolType}: ClassifyClass {clResultClass}, ClassifyScore {clResultScore}", CONTENT_TYPE.INFO);
                double lowScore = Machine.param.Option(ParameterDefine.OPTION.JIG_WORK_CLASSIFY_LOW_SCORE);
                if (toolType == TOOL_TYPE.LEFT)
                {
                    if (clResultScore <= lowScore)
                    {
                        CaptureUtil.CAPTURETYPE type = tempresult.LClassifyClass == JIG_CLASSIFY_RESULT.OK.ToString() ? CaptureUtil.CAPTURETYPE.LOW_SCORE_OK : CaptureUtil.CAPTURETYPE.LOW_SCORE_NG;
                        Vision.Capture("JIG", type, true);
                        SetError(ECODE.JIG_LEFT_CLASSIFY_LOW_SCORE);
                        return;
                    }
                }
                if (toolType == TOOL_TYPE.RIGHT)
                {
                    if (clResultScore <= lowScore)
                    {
                        CaptureUtil.CAPTURETYPE type = tempresult.RClassifyClass == JIG_CLASSIFY_RESULT.OK.ToString() ? CaptureUtil.CAPTURETYPE.LOW_SCORE_OK : CaptureUtil.CAPTURETYPE.LOW_SCORE_NG;
                        Vision.Capture("JIG", type, true);
                        SetError(ECODE.JIG_RIGHT_CLASSIFY_LOW_SCORE);
                        return;
                    }
                }

                if (clResultClass == JIG_CLASSIFY_RESULT.OK.ToString()) jigCLResult[currentTool] = JIG_CLASSIFY_RESULT.OK;
                else if (clResultClass == JIG_CLASSIFY_RESULT.NG.ToString()) jigCLResult[currentTool] = JIG_CLASSIFY_RESULT.NG;
                else if (clResultClass == JIG_CLASSIFY_RESULT.EXIST.ToString()) jigCLResult[currentTool] = JIG_CLASSIFY_RESULT.EXIST;
                else if (clResultClass == JIG_CLASSIFY_RESULT.EMPTY.ToString()) jigCLResult[currentTool] = JIG_CLASSIFY_RESULT.EMPTY;                

                if (toolType == TOOL_TYPE.RIGHT && (tempresult.Rcenter.x == Vision.INVALID_DATA || tempresult.Rcenter.y == Vision.INVALID_DATA)) throw new Exception("Cognex not return data");
                if (toolType == TOOL_TYPE.LEFT && (tempresult.Lcenter.x == Vision.INVALID_DATA || tempresult.Lcenter.y == Vision.INVALID_DATA)) throw new Exception("Cognex not return data");

                productPosition[currentTool].x = toolType == TOOL_TYPE.LEFT ? tempresult.Lcenter.x : tempresult.Rcenter.x;
                productPosition[currentTool].y = toolType == TOOL_TYPE.LEFT ? tempresult.Lcenter.y : tempresult.Rcenter.y;
                productAngle[currentTool] = toolType == TOOL_TYPE.LEFT ? tempresult.Langle : tempresult.Rangle;
                alignSucs[currentTool] = 0;

                if (Vision.IsSameWithLastData(CAMERA.JIG, (TOOL_TYPE)currentTool, new Point2d(productPosition[currentTool].x, productPosition[currentTool].y)))
                    throw new Exception("New data is same with last data");
            }
            catch (Exception ex)
            {
                if (jigCLResult[currentTool] == JIG_CLASSIFY_RESULT.EXIST || jigCLResult[currentTool] == JIG_CLASSIFY_RESULT.EMPTY || jigCLResult[currentTool] == JIG_CLASSIFY_RESULT.NG)
                {
                    alignSucs[currentTool] = 0;
                }
                else
                {
                    productPosition[currentTool].x = 0.0;
                    productPosition[currentTool].y = 0.0;
                    productAngle[currentTool] = 0.0;
                    alignSucs[currentTool] = -1;
                    //jigCLResult[currentTool] = JIG_CLASSIFY_RESULT.NG;
                }
            }
            LogUtil.Instance.Log(LOG_TYPE.INSPECTION,
                $"Jig Inspection Data: " +
                $"[alignSucs: {alignSucs[currentTool]}]" +
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
                case ECODE.ALIGN_ERROR_JIG_SEARCH_FAIL_LEFT_TARGET:
                case ECODE.ALIGN_ERROR_JIG_SEARCH_FAIL_RIGHT_TARGET:
                case ECODE.ALIGN_ERROR_DETECT_LEFT_JIG_NOT_OK_TO_ASSEMBLE:
                case ECODE.ALIGN_ERROR_DETECT_RIGHT_JIG_NOT_OK_TO_ASSEMBLE:
                    var processError = new List<STEP>()
                    {
                        STEP.PALETTE_UNLOCK,
                        STEP.ERROR
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

                case MSG.MSG_PROCESS_INIT_CONV:
                    var processInit = new List<STEP>()
                    {
                        STEP.LIGHT_OFF,
                        STEP.JIG_ALIGN_PRESS_UP,
                        STEP.JIG_ALIGN_PRESS_UP_CHECK,
                        STEP.JIG_IN_BUF_Z_READY_MOVE,
                        STEP.JIG_IN_BUF_Z_READY_MOVE_CHECK,
                        STEP.PALETTE_UNLOCK,
                        STEP.CONV_RUN,
                        STEP.CONV_STOP_INIT_WITH_DELAY,
                        STEP.INIT_PRODUCT_DATA_MISMATCH_CHECK,
                        STEP.IDLE,
                    };

                    StepList = processInit;
                    Step = StepList[StepIndex = 0];
                    break;

                case MSG.MSG_LOADING_PRODUCT:
                    Machine.Jigs[(int)UNITJIG.JIG_WORK].ClearInterface();
                    doneAlignment = false;

                    var ProcessLoadingProduct = new List<STEP>()
                    {
                        STEP.LIGHT_OFF,
                        STEP.IF_LOADING_POSSIBLE_WAIT,
                        STEP.STOPPER_UP,
                        STEP.CONV_RUN,
                        STEP.IF_LOADING_BUSY,
                        STEP.IF_LOADING_COMPLETE_CHECK,
                        STEP.DELAY_FOR_PALETTE,
                        STEP.PALETTE_LOCK,
                        STEP.CONV_STOP,
                        STEP.IDLE,
                    };

                    StepList = ProcessLoadingProduct;
                    Step = StepList[StepIndex = 0];
                    break;

                case MSG.MSG_ALIGNMENT_PRODUCT:
                    vision_fail_count = 0;
                    vision_retry_count = 0;

                    var processAlign = new List<STEP>()
                    {
                        STEP.PALETTE_LOCK,
                        STEP.REQUEST_VISIOTN_ALIGN,
                        STEP.IDLE,
                    };

                    StepList = processAlign;
                    Step = StepList[StepIndex = 0];
                    break;

                case MSG.MSG_PLACE_PRODUCT:
                    Machine.Parts[(int)UNITPART.JIG1].ClearInterface();
                    Machine.Parts[(int)UNITPART.JIG2].ClearInterface();

                    var processPlace = new List<STEP>()
                    {
                        STEP.IF_PLACE_WAIT_EDM_REPORT,
                        STEP.JIG_IN_BUF_Z_READY_WAIT,
                        STEP.IF_PLACE_POSSIBLE_WAIT,
                        STEP.IF_PLACE_BUSY,
                        STEP.IF_PLACE_COMPLETE_CHECK,
                        STEP.JIG_IN_BUF_Z_PRESS_MOVE,
                        STEP.JIG_IN_BUF_Z_PRESS_MOVE_CHECK,
                        STEP.JIG_ALIGN_PRESS_DOWN,
                        STEP.JIG_ALIGN_PRESS_DOWN_CHECK,
                        STEP.JIG_ALIGN_PRESS_UP,
                        STEP.JIG_ALIGN_PRESS_UP_CHECK,
                        STEP.JIG_IN_BUF_Z_READY_MOVE,
                        STEP.JIG_IN_BUF_Z_READY_MOVE_CHECK,
                        STEP.IDLE,
                    };

                    StepList = processPlace;
                    Step = StepList[StepIndex = 0];
                    break;

                case MSG.MSG_UNLOADING_PRODUCT:
                    Machine.Jigs[(int)UNITJIG.JIG_WORK].ClearInterface();
                    doneAlignment = false;

                    var processUnloadingProduct = new List<STEP>()
                    {
                        STEP.LIGHT_OFF,
                        STEP.PALETTE_UNLOCK,
                        STEP.IF_UNLOADING_REQUEST,
                        STEP.IF_UNLOADING_BUSY,
                        STEP.STOPPER_DOWN,
                        STEP.CONV_RUN,
                        STEP.IF_UNLOADING_COMPLETE_CHECK,
                        STEP.CONV_STOP,
                        STEP.STOPPER_UP,
                        STEP.IDLE,
                    };

                    StepList = processUnloadingProduct;
                    Step = StepList[StepIndex = 0];
                    break;
            }
        }

        protected override void OnProcessing()
        {
            CONVSTATUS stat;

            switch ((STEP)Step)
            {
                case STEP.ERROR:
                    AutoStep = AUTOSTEP.ERROR;
                    error_proc = false;
                    timeWait[(int)TIMER.TIMEOUT].Reset();
                    EStopBit = false;
                    break;

                case STEP.IDLE:
                    stopBit = false;
                    EStopBit = false;
                    error_proc = false;
                    timeWait[(int)TIMER.TIMEOUT].Reset();
                    break;
					
				case STEP.STOP:
                    timeWait[(int)TIMER.TIMEOUT].Reset();
                    break;

                case STEP.INIT_PRODUCT_DATA_MISMATCH_CHECK:
                    if (Machine.status.mode != SystemMode.SystemModeDRYRUN)
                    {
                        Machine.Jigs[(int)UNITJIG.JIG_WORK].exist = Machine.GetJigSignal(JIG_TYPE.TOP_WORK, JIG_SENSOR.IN)
                            && Machine.GetJigSignal(JIG_TYPE.TOP_WORK, JIG_SENSOR.OUT);
                    }

                    //if (Machine.GetJigSignal(JIG_TYPE.UNDER_IN, JIG_SENSOR.BRIDGE))
                    //{
                    //    SetError(ECODE.JIG_INCAREEER_SENSOR_ERROR);
                    //}
                    NextStep();
                    break;

                case STEP.CONV_STOP_INIT_WITH_DELAY:
                    timeWait[(int)TIMER.DELAY].Start();

                    Step = STEP.CONV_STOP_INIT_WITH_DELAY_CHECK;
                    break;

                case STEP.CONV_STOP_INIT_WITH_DELAY_CHECK:
                    if (timeWait[(int)TIMER.DELAY].Elapsed > Machine.param.Time(ParameterDefine.TIME.JIG_CARRIER_CONV_STOP_DELAY_TIME))
                        Step = STEP.CONV_STOP;
                    break;

                case STEP.CONV_STOP_WITH_DELAY:
                    timeWait[(int)TIMER.DELAY].Start();

                    Step = STEP.CONV_STOP_WITH_DELAY_CHECK;
                    timeWait[(int)TIMER.DELAY].Start();
                    break;

                case STEP.CONV_STOP_WITH_DELAY_CHECK:
                    if (timeWait[(int)TIMER.DELAY].Elapsed > Machine.param.Time(ParameterDefine.TIME.JIG_CONV_STOP_DELAY_TIME))
                        Step = STEP.CONV_STOP;
                    break;

                case STEP.CYLINDER_RUN_WAIT:
                    timeWait[(int)TIMER.DELAY].Reset();
                    Step = STEP.CYLINDER_RUN_WAIT_CHECK;
                    break;

                case STEP.CYLINDER_RUN_WAIT_CHECK:
                    if (timeWait[(int)TIMER.DELAY].Elapsed > 300)//Machine.param.Time(ParameterDefine.TIME.CYLINDER_RUN_WAIT_TIME))
                        NextStep();
                    break;

                case STEP.REQUEST_VISIOTN_ALIGN:
                    receivedData = false;
                    if (Machine.status.mode == SystemMode.SystemModeDRYRUN)
                    {
                        doneAlignment = true;
                        alignSucs[0] = 0;
                        jigCLResult[0] = JIG_CLASSIFY_RESULT.OK;
                        alignSucs[1] = 0;
                        jigCLResult[1] = JIG_CLASSIFY_RESULT.OK;
                        NextStep();
                    }
                    else
                    {
                        if (!Vision.RunGrab("JIG", false, true))
                            break;
                        bool saveTrainImage = Machine.param.Option(ParameterDefine.OPTION.USE_IMAGE_GATHERING_FOR_ASSEMBLY_DEFECT) == 1 ? true : false;
                        if (!Vision.Run("JIG", true, false, saveTrainImage))
                            break;
                        Step = STEP.REQUEST_VISIOTN_ALIGN_CHECK;
                        timeWait[(int)TIMER.TIMEOUT].Reset();
                    }
                    break;

                case STEP.REQUEST_VISIOTN_ALIGN_CHECK:
                    if (vision_fail_count >= 3 || timeWait[(int)TIMER.TIMEOUT].Elapsed > Machine.param.Time(ParameterDefine.TIME.VISION_TIME_OUT))
                    {
                        if (alignSucs[0] < 0 || alignSucs[1] < 0)
                        {
                            if (alignSucs[0] < 0) SetError(ECODE.ALIGN_ERROR_JIG_SEARCH_FAIL_LEFT_TARGET);

                            if (alignSucs[1] < 0) SetError(ECODE.ALIGN_ERROR_JIG_SEARCH_FAIL_RIGHT_TARGET);

                            Vision.Capture("JIG", CaptureUtil.CAPTURETYPE.RESULT, false);
                            Vision.Capture("JIG", CaptureUtil.CAPTURETYPE.FAIL, true);
                            break;
                        }
                        vision_fail_count++;
                        Step = STEP.REQUEST_VISIOTN_ALIGN;
                        break;
                    }

                    if (!receivedData)
                        break;


                    if (alignSucs[0] < 0 || alignSucs[1] < 0)
                    {
                        vision_fail_count++;
                        Step = STEP.REQUEST_VISIOTN_ALIGN;
                        break;
                    }

                    Vision.PauseTrainCapture("JIG", false);
                    Vision.Capture("JIG", CaptureUtil.CAPTURETYPE.RESULT, false);

                    for (int toolType = 0; toolType < (int)TOOL_TYPE.MAX; toolType++)
                    {
                        if (jigCLResult[toolType] == JIG_CLASSIFY_RESULT.NG)
                        {
                            if (Machine.status.mode == SystemMode.SystemModeAUTO && Machine.sysMode == Machine.SYSMODE.AUTO)
                            {
                                Machine.info.ProductInc(StatusDefine.PRODUCT.PRODUCT_INPUT_TOTAL_COUNT, 1);
                                Machine.info.ProductInc(StatusDefine.PRODUCT.PRODUCT_OUTPUT_NG_COUNT, 1);
                                Machine.info.JigInc(current_jig_id, StatusDefine.PRODUCT.PRODUCT_OUTPUT_NG_COUNT, 1);
                            }
                        }
                    }

                    if (jigCLResult[(int)TOOL_TYPE.LEFT] == JIG_CLASSIFY_RESULT.NG)
                    {
                        Vision.Capture("JIG", CaptureUtil.CAPTURETYPE.CLASSIFY_NG, true);
                        if (vision_retry_count >= 3)
                        {
                            vision_retry_count = 0;
                            SetError(ECODE.ALIGN_ERROR_DETECT_LEFT_JIG_NOT_OK_TO_ASSEMBLE); break;

                        }
                        vision_retry_count++;
                        Step = STEP.REQUEST_VISIOTN_ALIGN;
                        break;
                    }
                    if (jigCLResult[(int)TOOL_TYPE.RIGHT] == JIG_CLASSIFY_RESULT.NG)
                    {
                        Vision.Capture("JIG", CaptureUtil.CAPTURETYPE.CLASSIFY_NG, true);
                        if (vision_retry_count >= 3)
                        {
                            vision_retry_count = 0;
                            SetError(ECODE.ALIGN_ERROR_DETECT_RIGHT_JIG_NOT_OK_TO_ASSEMBLE); break;
                        }
                        vision_retry_count++;
                        Step = STEP.REQUEST_VISIOTN_ALIGN;
                        break;
                    }

                    Vision.Capture("JIG", CaptureUtil.CAPTURETYPE.CLASSIFY_OK, true);

                    Machine.Parts[(int)UNITPART.JIG1].exist = jigCLResult[(int)TOOL_TYPE.LEFT] == JIG_CLASSIFY_RESULT.EXIST;
                    Machine.Parts[(int)UNITPART.JIG2].exist = jigCLResult[(int)TOOL_TYPE.RIGHT] == JIG_CLASSIFY_RESULT.EXIST;
                    doneAlignment = true;
                    NextStep();
                    break;

                case STEP.LIGHT_OFF:
                    Vision.Camera.LightOn((int)CAMERA.JIG, false);
                    Thread.Sleep(100);
                    NextStep();
                    break;
            }

            OnProcessOfAutoRun();
            OnProcessOfMotion();
            OnProcessOfIO();
            OnProcessOfInterface();

            if (EStopBit)
                Step = STEP.IDLE;

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
            var proc = Machine.proclist[(int)Machine.PROCESS.JIGWORK] as ProcessJigWork;
            CONVSTATUS convstat = CONVSTATUS.EMPTY;
            bool report = Convert.ToBoolean(Machine.param.Option(ParameterDefine.OPTION.REPORT_EDM_SERVER));

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
                        AutoStep = AUTOSTEP.LOADING;

                    CheckStopBit();
                    break;

                case AUTOSTEP.INIT:
                    if (Machine.processPause && StepList != null)
                    {
                        if (StepList.Count > resumeStepIndex)
                        {
                            initCompl = true;
                            AutoStep = AUTOSTEP.OTHER_PROCESS_INIT_WAIT;
                            break;
                        }
                    }

                    Machine.Jigs[(int)UNITJIG.JIG_WORK].ClearInterface();
                    Machine.Parts[(int)UNITPART.JIG1].ClearInterface();
                    Machine.Parts[(int)UNITPART.JIG2].ClearInterface();

                    initCompl = false;
                    alignSucs[0] = -1;
                    alignSucs[1] = -1;
                    doneAlignment = false;

                    resumeProc = false;
                    SetMessage((int)MSG.MSG_PROCESS_INIT_CONV);

                    AutoStep = AUTOSTEP.INIT_CHECK;
                    break;

                case AUTOSTEP.INIT_CHECK:
                    if (CheckStopBit() || !Ready() || Error())
                        break;

                    initCompl = true;
                    resumeProc = false;
                    AutoStep = AUTOSTEP.OTHER_PROCESS_INIT_WAIT;
                    break;

                case AUTOSTEP.OTHER_PROCESS_INIT_WAIT:
                    if (CheckStopBit() || Machine.GetProcInitialized() || Machine.sysMode != Machine.SYSMODE.AUTO)
                    {
                        if (Machine.processPause && StepList != null)
                        {
                            if (StepList.Count > resumeStepIndex)
                            {
                                StepIndex = resumeStepIndex;
                                Step = StepList[StepIndex];
                                AutoStep = (AUTOSTEP)lastAutoStep;
                                resumeProc = true;
                                break;
                            }
                        }
                        else
                            AutoStep = AUTOSTEP.PROC_SELECT_FOR_RESTART;
                        break;
                    }
                    break;

                case AUTOSTEP.PROC_SELECT_FOR_RESTART:
                    if (CheckStopBit())
                        break;
                    convstat = ProductExistCheck();

                    if (convstat == CONVSTATUS.EMPTY)
                        AutoStep = AUTOSTEP.LOADING;
                    else if (convstat == CONVSTATUS.EXIST)
                        AutoStep = AUTOSTEP.ALIGNMENT;
                    else if (convstat == CONVSTATUS.LOADING)
                        AutoStep = AUTOSTEP.LOADING;
                    else if (convstat == CONVSTATUS.UNLOADING)
                        AutoStep = AUTOSTEP.UNLOADING;
                    break;

                case AUTOSTEP.LOADING:
                    SetMessage((int)MSG.MSG_LOADING_PRODUCT);

                    if (Machine.recipe.Option(RecipeDefine.OPTION.USE_ASSEMBLY_MODE) == 0)
                    {
                        EDM.LoadingSets(true);
                        Thread.Sleep(100);
                        EDM.LoadingSets(true);
                    }

                    AutoStep = AUTOSTEP.LOADING_COMPL_CHECK;
                    break;

                case AUTOSTEP.LOADING_COMPL_CHECK:
                    if (CheckStopBit() || !Ready() || Error())
                        break;

                    AutoStep = AUTOSTEP.ALIGNMENT;
                    break;

                case AUTOSTEP.ALIGNMENT:
                    //if (Machine.EmptyPart())
                    //{
                    //    AutoStep = AUTOSTEP.UNLOADING;
                    //    break;
                    //}
                    if (CheckStopBit())// || Machine.interfer_jig_align)
                        break;

                    receivedData = false;
                    //doingAlignment = true;

                    if (Machine.status.mode == SystemMode.SystemModeBYPASS)
                        AutoStep = AUTOSTEP.UNLOADING;
                    else
                    {
                        SetMessage((int)MSG.MSG_ALIGNMENT_PRODUCT);
                        AutoStep = AUTOSTEP.ALIGNMENT_COMPL_CHECK;
                    }
                    break;

                case AUTOSTEP.ALIGNMENT_COMPL_CHECK:
                    if (CheckStopBit() || !Ready() || Error())
                        break;

                    doneAlignment = true;

                    if (Machine.Parts[(int)UNITPART.JIG1].exist && Machine.Parts[(int)UNITPART.JIG2].exist)
                    {
                        Machine.Jigs[(int)UNITJIG.JIG_WORK].productCount = 2;
                        AutoStep = AUTOSTEP.UNLOADING;
                    }
                    else if (jigCLResult[(int)TOOL_TYPE.LEFT] == JIG_CLASSIFY_RESULT.EMPTY && jigCLResult[(int)TOOL_TYPE.RIGHT] == JIG_CLASSIFY_RESULT.EMPTY)
                    {
                        Machine.Jigs[(int)UNITJIG.JIG_WORK].productCount = 0;
                        AutoStep = AUTOSTEP.UNLOADING;
                    }
                    else
                    {
                        Machine.Parts[(int)UNITPART.JIG1].from_id = current_jig_id - 1;
                        Machine.Parts[(int)UNITPART.JIG2].from_id = current_jig_id - 1;

                        Machine.Jigs[(int)UNITJIG.JIG_WORK].productCount = 0;
                        AutoStep = AUTOSTEP.PLACE;
                        doneAlignment = true;
                    }
                    break;

                case AUTOSTEP.PLACE:
                    SetMessage((int)MSG.MSG_PLACE_PRODUCT);
                    AutoStep = AUTOSTEP.PLACE_COMPL_CHECK;
                    break;

                case AUTOSTEP.PLACE_COMPL_CHECK:
                    if (CheckStopBit() || !Ready() || Error())
                        break;

                    if (!Machine.Parts[(int)UNITPART.JIG1].exist || !Machine.Parts[(int)UNITPART.JIG2].exist)
                        AutoStep = AUTOSTEP.PLACE;
                    else
                        AutoStep = AUTOSTEP.UNLOADING;
                    break;

                case AUTOSTEP.UNLOADING:
                    if (CheckStopBit())
                        break;
                    SetMessage((int)MSG.MSG_UNLOADING_PRODUCT);
                    AutoStep = AUTOSTEP.UNLOADING_COMPL_CHECK;
                    break;

                case AUTOSTEP.UNLOADING_COMPL_CHECK:
                    if (CheckStopBit() || !Ready() || Error())
                        break;

                    if (Machine.status.state == SystemState.SystemStateRUN || Machine.status.state == SystemState.SystemStateRUNEMPTY)
                        UpdateTactTime((int)Machine.PROCESS.JIGWORK);

                    timeWait[(int)TIMER.TACT].Reset();

                    AutoStep = AUTOSTEP.COMPLETE;
                    break;
            }

            if ((int)AutoStep > (int)AUTOSTEP.PROC_SELECT_FOR_RESTART)
                lastAutoStep = (int)AutoStep;

            if (EStopBit)
                AutoStep = AUTOSTEP.IDLE;
				
            if (sleepAutoStep == (int)AutoStep)
            {
                if ((int)AutoStep > (int)AUTOSTEP.PROC_SELECT_FOR_RESTART)
                {
                    if (timeWait[(int)TIMER.ALIVE].Elapsed > 60 * 1000)
                        Machine.status.state = SystemState.SystemStateSLEEP;
                }
            }
            else
            {
                timeWait[(int)TIMER.ALIVE].Reset();
                sleepAutoStep = (int)AutoStep;
                if (Machine.status.state == SystemState.SystemStateSLEEP)
                    Machine.status.state = SystemState.SystemStateRUN;
            }
        }

        private void OnProcessOfInterface()
        {
            bool report = Machine.param.Option(ParameterDefine.OPTION.REPORT_EDM_SERVER) == 1 ? true : false;

            uint returnCurrent = 0;
            uint returnRunning = 0;

			const int UPSTREAM = (int)UNITJIG.JIG_IN_BUF;
            const int CURRENT = (int)UNITJIG.JIG_WORK;
            const int DOWNSTREAM = (int)UNITJIG.JIG_OUT_BUF;

            switch ((STEP)Step)
            {
                #region LOADING INTERFACE
                case STEP.IF_LOADING_POSSIBLE_WAIT:
                    resumeStepIndex = StepIndex;

                    // 상부 장치의 배출 요청을 기다린다.
                    if (!Machine.Jigs[UPSTREAM].unloadingRequest)
                        return;

                    NextStep();
                    break;

                case STEP.IF_LOADING_BUSY:
                    // 언로딩 상태로 변경한다.
                    Machine.Jigs[CURRENT].loading = true;
                    // 상부 장치가 언로딩이 끝날 때까지 대기한다.
                    if (!Machine.Jigs[UPSTREAM].unloading && !resumeProc)
                        break;

                    NextStep();

                    timeWait[(int)TIMER.TIMEOUT].Reset();
                    timeWait[(int)TIMER.DELAY].Reset();
                    break;

                case STEP.IF_LOADING_COMPLETE_CHECK:
                    if (timeWait[(int)TIMER.TIMEOUT].Elapsed > Machine.param.Time(ParameterDefine.TIME.CONV_RUN_TIME_OUT))
                    {
                        SetError(ECODE.TIMEOUT_JIG_WORK_LOADING);
                        break;
                    }

                    bool isJigExist = Machine.GetJigSignal(JIG_TYPE.TOP_WORK, JIG_SENSOR.IN)
                       && Machine.GetJigSignal(JIG_TYPE.TOP_WORK, JIG_SENSOR.OUT);
                    // 더미런 시에는 항상 ON 이다.
                    if (Machine.status.mode == SystemMode.SystemModeDRYRUN) isJigExist = true;

                    if (!isJigExist) break;
                    if (Machine.Jigs[UPSTREAM].unloading && !resumeProc) break;

                    // 자재가 있다고 판단되면 데이터를 생성한다.
                    Machine.Jigs[CURRENT].loading = false;
                    Machine.Jigs[CURRENT].exist = true;

                    Machine.Parts[(int)UNITPART.JIG1].exist = false;
                    Machine.Parts[(int)UNITPART.JIG2].exist = false;
                    NextStep();

                    resumeStepIndex = StepIndex;
                    resumeProc = false;
                    break;
                #endregion
                #region UNLOADING INTERFACE
                case STEP.IF_UNLOADING_REQUEST:
                    resumeStepIndex = StepIndex;

                    // 배출 요청 신호를 ON 한다.
                    Machine.Jigs[CURRENT].unloadingRequest = true;

                    NextStep();

                    timeWait[(int)TIMER.TIMEOUT].Reset();
                    timeWait[(int)TIMER.DELAY].Reset();
                    break;

                case STEP.IF_UNLOADING_BUSY:
                    // 언로딩 상태로 변경한다.
                    Machine.Jigs[CURRENT].unloading = true;
                    // 하류 장치가 로딩 상태가 될 때까지 기다린다.
                    if (!Machine.Jigs[DOWNSTREAM].loading && !resumeProc)
                        break;

                    Machine.Jigs[(int)UNITJIG.JIG_OUT_BUF].productCount = Machine.Jigs[(int)UNITJIG.JIG_WORK].productCount;
                    timeWait[(int)TIMER.TIMEOUT].Start();
                    NextStep();
                    break;

                case STEP.IF_UNLOADING_COMPLETE_CHECK:
                    if (timeWait[(int)TIMER.TIMEOUT].Elapsed > Machine.param.Time(ParameterDefine.TIME.CONV_RUN_TIME_OUT))
                    {
                        SetError(ECODE.TIMEOUT_JIG_WORK_UNLOADING);
                        break;
                    }

                    isJigExist = Machine.GetJigSignal(JIG_TYPE.TOP_WORK, JIG_SENSOR.IN)
                        || Machine.GetJigSignal(JIG_TYPE.TOP_WORK, JIG_SENSOR.OUT);
                    // 더미런 시에는 항상 OFF 이다.
                    if (Machine.status.mode == SystemMode.SystemModeDRYRUN) isJigExist = false;

                    if (isJigExist) break;

                    Machine.Jigs[CURRENT].unloadingRequest = false;
                    Machine.Jigs[CURRENT].unloading = false;

                    // 하류 장치가 로딩을 완료할 때까지 기다린다.
                    if (Machine.Jigs[DOWNSTREAM].loading && !resumeProc)
                        break;
                    // 하류 장치에 자재 정보가 생성되면 종료 한다.
                    if (!Machine.Jigs[DOWNSTREAM].exist)
                        break;

                    Machine.Jigs[CURRENT].exist = false;
                    Machine.Parts[(int)UNITPART.JIG1].exist = false;
                    Machine.Parts[(int)UNITPART.JIG2].exist = false;
                    Machine.Jigs[(int)UNITJIG.JIG_WORK].productCount = 0;

                    NextStep();

                    resumeStepIndex = StepIndex;
                    resumeProc = false;
                    break;
                #endregion
                case STEP.IF_PLACE_WAIT_EDM_REPORT:
                    EDM.LoadingSets(true);
                    NextStep();
                    break;

                case STEP.JIG_IN_BUF_Z_READY_WAIT:
                    if (!Util.IsAxisInPos((AXIS.JIG_IN_BUF_Z, Machine.param.Position(ParameterDefine.POSITION.JIG_IN_BUF_Z_READY_POS))))
                        break;
                    NextStep();
                    break;

                case STEP.IF_PLACE_POSSIBLE_WAIT:
                    resumeStepIndex = StepIndex;

                    if (Machine.Parts[(int)UNITPART.ASSEMBLER1].unloadingRequest)
                        targetPick = 0;
                    else if (Machine.Parts[(int)UNITPART.ASSEMBLER2].unloadingRequest)
                        targetPick = 1;
                    else
                        break;

                    NextStep();
                    break;

                case STEP.IF_PLACE_BUSY:
                    if (!Machine.Parts[upstreamparts[targetPick]].unloading)
                        return;
                    Machine.Parts[CURRENT_PARTS[targetPick]].loading = true;

                    EDM.LoadingSets(false);
                    Machine.info.ProductInc(StatusDefine.PRODUCT.PRODUCT_INPUT_PARTS_COUNT, 1);
					Machine.info.JigInc(current_jig_id - 1, StatusDefine.PRODUCT.PRODUCT_INPUT_PARTS_COUNT, 1);
                    Machine.Jigs[(int)UNITJIG.JIG_WORK].productCount++;
                    NextStep();

                    timeWait[(int)TIMER.TIMEOUT].Reset();
                    timeWait[(int)TIMER.DELAY].Reset();
                    break;

                case STEP.IF_PLACE_COMPLETE_CHECK:
                    if (Machine.Parts[upstreamparts[targetPick]].unloading)
                        return;

                    Machine.Parts[CURRENT_PARTS[targetPick]].loading = false;
                    Machine.Parts[CURRENT_PARTS[targetPick]].exist = true;

                    if (Machine.Parts[(int)UNITPART.JIG1].exist && Machine.Parts[(int)UNITPART.JIG2].exist)
                        NextStep();
                    else
                        Step = STEP.IDLE;

                    resumeStepIndex = StepIndex;
                    break;

                case STEP.JIG_IN_BUF_Z_PRESS_MOVE_CHECK:
                    if (!Machine.motion.MoveAxisDoneCheck((int)AXIS.JIG_IN_BUF_Z))
                        break;
                    NextStep();
                    break;

                case STEP.JIG_ALIGN_PRESS_DOWN_CHECK:
                    Machine.IO.GetIn((int)DI.JIG_ALIGN_PRESS_DOWN, ref returnCurrent);
                    if (returnCurrent != 1) break;
                    NextStep();
                    break;

                case STEP.JIG_ALIGN_PRESS_UP_CHECK:
                    Machine.IO.GetIn((int)DI.JIG_ALIGN_PRESS_UP, ref returnCurrent);
                    if (returnCurrent != 1) break;
                    NextStep();
                    break;

                case STEP.JIG_IN_BUF_Z_READY_MOVE_CHECK:
                    if (!Machine.motion.MoveAxisDoneCheck((int)AXIS.JIG_IN_BUF_Z))
                        break;
                    NextStep();
                    break;
            }
        }

        private void OnProcessOfMotion()
        {
            switch ((STEP)Step)
            {
                case STEP.INIT_STOPPER_UP:
                    if ((Machine.GetJigSignal(JIG_TYPE.TOP_WORK, JIG_SENSOR.OUT)
                        || Machine.GetJigSignal(JIG_TYPE.TOP_OUT, JIG_SENSOR.IN))
                        && !Machine.GetJigSignal(JIG_TYPE.TOP_WORK, JIG_SENSOR.IN)) NextStep();
                    else Step = STEP.STOPPER_UP;
                    break;

                case STEP.DELAY_FOR_PALETTE:
                    Util.Delay((int)Machine.param.Time(ParameterDefine.TIME.JIG_WORK_CONV_STOP_DELAY_TIME));
                    NextStep();
                    break;

                case STEP.CONV_RUN:
                    resumeStepIndex = StepIndex;

                    Machine.IO.SetOut((int)DO.JIG_WORK_CONV_RUN, 1);
                    NextStep();
                    timeWait[(int)TIMER.DELAY].Start();
                    break;

                case STEP.CONV_STOP:
                    Machine.IO.SetOut((int)DO.JIG_WORK_CONV_RUN, 0);
                    NextStep();
                    timeWait[(int)TIMER.DELAY].Start();
                    break;

                case STEP.JIG_IN_BUF_Z_PRESS_MOVE:
                    if (Machine.Parts[(int)UNITPART.ASSEMBLER1].unloading || Machine.Parts[(int)UNITPART.ASSEMBLER2].unloading)
                        break;
                    Machine.motion.MoveAxisAbs((int)AXIS.JIG_IN_BUF_Z,
                        1000 * Machine.param.Position(ParameterDefine.POSITION.JIG_IN_BUF_Z_PRESS_POS),
                        Machine.param.Velocity(ParameterDefine.VELOCITY.JIG_Z_VEL),
                        Machine.param.Velocity(ParameterDefine.VELOCITY.JIG_Z_ACC),
                        Machine.param.Velocity(ParameterDefine.VELOCITY.JIG_Z_DEC));
                    Step = STEP.JIG_IN_BUF_Z_PRESS_MOVE_CHECK;
                    break;
                case STEP.JIG_IN_BUF_Z_READY_MOVE:
                    Machine.motion.MoveAxisAbs((int)AXIS.JIG_IN_BUF_Z,
                        1000 * Machine.param.Position(ParameterDefine.POSITION.JIG_IN_BUF_Z_READY_POS),
                        Machine.param.Velocity(ParameterDefine.VELOCITY.JIG_Z_VEL),
                        Machine.param.Velocity(ParameterDefine.VELOCITY.JIG_Z_ACC),
                        Machine.param.Velocity(ParameterDefine.VELOCITY.JIG_Z_DEC));
                    Step = STEP.JIG_IN_BUF_Z_READY_MOVE_CHECK;
                    break;
            }
        }

        private void OnProcessOfIO()
        {
            uint ret1 = 0;
            uint ret2 = 0;

            switch ((STEP)Step)
            {
                case STEP.STOPPER_UP:
                    resumeStepIndex = StepIndex;

                    Machine.IO.SetOut((int)DO.JIG_WORK_STOPPER_UP, 1);
                    Machine.IO.SetOut((int)DO.JIG_WORK_STOPPER_DOWN, 0);

                    Step = STEP.STOPPER_UP_CHECK;
                    timeWait[(int)TIMER.DELAY].Start();
                    timeWait[(int)TIMER.TIMEOUT].Start();
                    break;

                case STEP.STOPPER_UP_CHECK:
                    if (timeWait[(int)TIMER.TIMEOUT].Elapsed > Machine.param.Time(ParameterDefine.TIME.CYLINDER_TIME_OUT))
                    {
                        SetError(ECODE.TIMEOUT_JIG_WORK_STOPPER_UP);
                        break;
                    }

                    Machine.IO.GetIn((int)DI.JIG_WORK_STOPPER_UP, ref ret1);
                    Machine.IO.GetIn((int)DI.JIG_WORK_STOPPER_DOWN, ref ret2);

                    if (ret1 == 1 && ret2 == 0)
                        NextStep();
                    break;

                case STEP.STOPPER_DOWN:
                    resumeStepIndex = StepIndex;

                    Machine.IO.SetOut((int)DO.JIG_WORK_STOPPER_UP, 0);
                    Machine.IO.SetOut((int)DO.JIG_WORK_STOPPER_DOWN, 1);

                    Step = STEP.STOPPER_DOWN_CHECK;
                    timeWait[(int)TIMER.DELAY].Start();
                    timeWait[(int)TIMER.TIMEOUT].Start();
                    break;

                case STEP.STOPPER_DOWN_CHECK:
                    if (timeWait[(int)TIMER.TIMEOUT].Elapsed > Machine.param.Time(ParameterDefine.TIME.CYLINDER_TIME_OUT))
                    {
                        SetError(ECODE.TIMEOUT_JIG_WORK_STOPPER_DOWN);
                        break;
                    }

                    Machine.IO.GetIn((int)DI.JIG_WORK_STOPPER_UP, ref ret1);
                    Machine.IO.GetIn((int)DI.JIG_WORK_STOPPER_DOWN, ref ret2);

                    if (ret1 == 0 && ret2 == 1)
                        NextStep();
                    break;

                case STEP.PALETTE_LOCK:
                    resumeStepIndex = StepIndex;
                    Machine.IO.SetOut((int)DO.JIG_WORK_PALLETE_FWD, 1);
                    Machine.IO.SetOut((int)DO.JIG_WORK_PALLETE_BWD, 0);

                    Step = STEP.PALETTE_LOCK_CHECK;
                    timeWait[(int)TIMER.DELAY].Start();
                    timeWait[(int)TIMER.TIMEOUT].Start();
                    break;

                case STEP.PALETTE_LOCK_CHECK:
                    if (Machine.status.mode == SystemMode.SystemModeDRYRUN)
                    {
                        NextStep();
                        break;
                    }

                    if (timeWait[(int)TIMER.TIMEOUT].Elapsed > Machine.param.Time(ParameterDefine.TIME.CYLINDER_TIME_OUT))
                    {
                        SetError(ECODE.TIMEOUT_JIG_WORK_PALLETE_LOCK);
                        break;
                    }

                    Machine.IO.GetIn((int)DI.JIG_WORK_PALLETE_FWD, ref ret1);
                    Machine.IO.GetIn((int)DI.JIG_WORK_PALLETE_BWD, ref ret2);

                    if (ret1 == 1 && ret2 == 0)
                    {
                        Step = STEP.CYLINDER_RUN_WAIT;
                        break;
                        //NextStep();
                    }
                    break;

                case STEP.PALETTE_UNLOCK:
                    resumeStepIndex = StepIndex;
                    Machine.IO.SetOut((int)DO.JIG_WORK_PALLETE_FWD, 0);
                    Machine.IO.SetOut((int)DO.JIG_WORK_PALLETE_BWD, 1);

                    Step = STEP.PALETTE_UNLOCK_CHECK;
                    timeWait[(int)TIMER.DELAY].Start();
                    timeWait[(int)TIMER.TIMEOUT].Start();
                    break;

                case STEP.PALETTE_UNLOCK_CHECK:
                    if (timeWait[(int)TIMER.TIMEOUT].Elapsed > Machine.param.Time(ParameterDefine.TIME.CYLINDER_TIME_OUT))
                    {
                        SetError(ECODE.TIMEOUT_JIG_WORK_PALLETE_UNLOCK);
                        break;
                    }

                    Machine.IO.GetIn((int)DI.JIG_WORK_PALLETE_FWD, ref ret1);
                    Machine.IO.GetIn((int)DI.JIG_WORK_PALLETE_BWD, ref ret2);

                    if (ret1 == 0 && ret2 == 1)
                        NextStep();
                    break;

                case STEP.JIG_ALIGN_PRESS_DOWN:
                    Machine.IO.SetOut((int)DO.JIG_ALIGN_PRESS_UP, 0);
                    Machine.IO.SetOut((int)DO.JIG_ALIGN_PRESS_DOWN, 1);
                    Step = STEP.JIG_ALIGN_PRESS_DOWN_CHECK;
                    break;
                case STEP.JIG_ALIGN_PRESS_UP:
                    Machine.IO.SetOut((int)DO.JIG_ALIGN_PRESS_DOWN, 0);
                    Machine.IO.SetOut((int)DO.JIG_ALIGN_PRESS_UP, 1);
                    Step = STEP.JIG_ALIGN_PRESS_UP_CHECK;
                    break;
            }
        }

        public override void SetHeadTarget(int iTarget)
        {
            this.targetCurr = iTarget;
        }

        private CONVSTATUS ProductExistCheck()
        {
            uint existIn = 0;
            uint existOut = 0;
            uint stopperDown = 0;

            Machine.IO.GetIn((int)DI.JIG_WORK_DETECT_IN, ref existIn);
            Machine.IO.GetIn((int)DI.JIG_WORK_DETECT_OUT, ref existOut);
            Machine.IO.GetIn((int)DI.JIG_WORK_STOPPER_DOWN, ref stopperDown);

            if (existIn == 1 && existOut == 1) return CONVSTATUS.EXIST;
            else if (existIn == 0 && existOut == 0) return CONVSTATUS.EMPTY;
            else if (stopperDown == 1) return CONVSTATUS.UNLOADING;
            else if (stopperDown == 0) return CONVSTATUS.LOADING;

            return CONVSTATUS.ERROR;
        }
    }
}
