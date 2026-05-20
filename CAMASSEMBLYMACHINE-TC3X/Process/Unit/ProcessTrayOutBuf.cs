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
using TopEng.Utils;

namespace CAMASSEMBLYMACHINE.Process
{
    public class ProcessTrayOutBuf : IProcess
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
            INIT_UNLOADING,
            INIT_UNLOADING_CHECK,
            PROC_SELECT_FOR_RESTART,
            LOADING,
            LOADING_CHECK,
            LOADING_CHECK_COMPLETE,
            LOADING_COMPL_CHECK,
            UNLOADING,
            UNLOADING_COMPL_CHECK,
        }

        public enum STEP
        {
            ERROR = -1,
            IDLE,
            STOP,
            NEXT_STEP,

            INIT_PRODUCT_DATA_MISMATCH_CHECK,

            CONV_STOP_WITH_DELAY,
            CONV_STOP_WITH_DELAY_CHECK,

            MOVE_TRAY_TRF_Z_POS_CHECK,
            MOVE_TRAY_TRF_Z_DETECT_LOC_CHECK,
            MOVE_TRAY_TRF_Z_VAC_ON_CHECK,
            MOVE_TRAY_TRF_Z_LOADING_READY_POS,
            MOVE_TRAY_TRF_Z_LOADING_POS_SENSING,
            MOVE_TRAY_TRF_Z_UNLOADING_POS_SENSING, // Tray 적재 이후 감지 높이
            MOVE_TRAY_TRF_Z_UNLOADING_POS, // Tray 첫번째 적재시 Teaching 높이
            MOVE_TRAY_TRF_Z_READY_POS,
            MOVE_TRAY_TRF_Z_LOADING_RECHECK_POS,

            MOVE_TRAY_TRF_Y_POS_CHECK,
            MOVE_TRAY_TRF_Y_LOADING_POS,
            MOVE_TRAY_TRF_Y_UNLOADING_POS,
            MOVE_TRAY_TRF_Y_READY_POS,
            CHECK_IF_TRAY_IN_TRANSFER,
            LOADING_RETRY_CHECK,
            CHECK_TRAY_DOWN_INTERLOCK,

            CONV_RUN,
            CONV_STOP,
            INIT_STOPPER_UP,

            STOPPER_UP,
            STOPPER_UP_CHECK,
            STOPPER_DOWN,
            STOPPER_DOWN_CHECK,

            TRAY_TRF_VACON,
            TRAY_TRF_VACON_BYPASS,
            TRAY_TRF_VACON_CHECK,
            TRAY_TRF_VACOFF,
            TRAY_TRF_VACOFF_CHECK,

            TRAY_TRF_DETECT_UP, // Tray 적재 높이 확인용 실린더
            TRAY_TRF_DETECT_UP_CHECK,
            TRAY_TRF_DETECT_DOWN,
            TRAY_TRF_DETECT_DOWN_CHECK,
            TRAY_TRF_EXIST_CHECK,

            IF_LOADING_POSSIBLE_WAIT,
            IF_LOADING_BUSY,
            IF_LOADING_COMPLETE_CHECK,
            IF_UNLOADING_REQUEST,
            IF_UNLOADING_BUSY,
            IF_UNLOADING_COMPLETE_CHECK,

            CHECK_IF_TRAY_EXIST_IN_OUT_BUF_CV,
        }

        public enum MSG
        {
            MSG_SINGLE,

            MSG_PROCESS_INIT,
            MSG_INIT_UNLOADING,
            MSG_LOADING_PRODUCT,
            MSG_UNLOADING_PRODUCT,

            MSG_MOVE_READY_POS,
            MSG_MOVE_LOADING_POS,
            MSG_MOVE_UNLOADING_POS,
        }

        #region PROCESS_STEP_VARIABLES
        private int loadingRetryCount;
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

            Machine.Trays[(int)UNITTRAY.TRAY_OUT_BUF].ClearInterface();
            Machine.IO.SetOut((int)DO.TRAY_OUT_BUF_CONV_RUN, 0);
        }

        public override void Stop()
        {
            stopBit = true;

            Machine.Trays[(int)UNITTRAY.TRAY_OUT_BUF].ClearInterface();
            Machine.IO.SetOut((int)DO.TRAY_OUT_BUF_CONV_RUN, 0);
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

        public ProcessTrayOutBuf()
        {
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
                        STEP.INIT_STOPPER_UP,
                        STEP.MOVE_TRAY_TRF_Z_READY_POS,
                        STEP.MOVE_TRAY_TRF_Y_UNLOADING_POS,
                        STEP.INIT_PRODUCT_DATA_MISMATCH_CHECK,
                        STEP.IDLE,
                    };

                    StepList = processInit;
                    Step = StepList[StepIndex = 0];
                    break;

                case MSG.MSG_INIT_UNLOADING:
                    Machine.Trays[(int)UNITTRAY.TRAY_OUT_BUF].ClearInterface();

                    var processINITUNLoading = new List<STEP>()
                    {
                        STEP.TRAY_TRF_DETECT_UP,
                        STEP.CHECK_IF_TRAY_IN_TRANSFER,
                        STEP.TRAY_TRF_DETECT_DOWN,
                        STEP.CHECK_TRAY_DOWN_INTERLOCK,
                        STEP.MOVE_TRAY_TRF_Z_UNLOADING_POS_SENSING,
                        STEP.TRAY_TRF_VACOFF,
                        STEP.MOVE_TRAY_TRF_Z_READY_POS,
                        STEP.CHECK_IF_TRAY_EXIST_IN_OUT_BUF_CV,
                        STEP.IDLE,
                    };

                    Machine.Trays[(int)UNITTRAY.TRAY_OUT_BUF].exist =
                            Machine.GetTraySensorSignal(TRAY_TYPE.OUT_BUF, TRAY_CV_SENSOR.IN)
                            || Machine.GetTraySensorSignal(TRAY_TYPE.OUT_BUF, TRAY_CV_SENSOR.MID)
                            || Machine.GetTraySensorSignal(TRAY_TYPE.OUT_BUF, TRAY_CV_SENSOR.END);


                    if (!Machine.Trays[(int)UNITTRAY.TRAY_OUT_BUF].exist)
                    {
                        processINITUNLoading[GetStepIndex(processINITUNLoading, STEP.TRAY_TRF_DETECT_DOWN)] = STEP.NEXT_STEP;
                        processINITUNLoading[GetStepIndex(processINITUNLoading, STEP.CHECK_TRAY_DOWN_INTERLOCK)] = STEP.NEXT_STEP;
                        processINITUNLoading[GetStepIndex(processINITUNLoading, STEP.MOVE_TRAY_TRF_Z_UNLOADING_POS_SENSING)] = STEP.MOVE_TRAY_TRF_Z_UNLOADING_POS;
                    }
                    StepList = processINITUNLoading;
                    Step = StepList[StepIndex = 0];
                    break;

                case MSG.MSG_LOADING_PRODUCT:
                    Machine.Trays[(int)UNITTRAY.TRAY_OUT_BUF].ClearInterface();
                    loadingRetryCount = 0;
                    Machine.Trays[(int)UNITTRAY.TRAY_OUT_BUF].exist =
                            Machine.GetTraySensorSignal(TRAY_TYPE.OUT_BUF, TRAY_CV_SENSOR.IN)
                            || Machine.GetTraySensorSignal(TRAY_TYPE.OUT_BUF, TRAY_CV_SENSOR.MID)
                            || Machine.GetTraySensorSignal(TRAY_TYPE.OUT_BUF, TRAY_CV_SENSOR.END);

                    var processLoading = new List<STEP>()
                    {
                        STEP.STOPPER_UP,
                        STEP.TRAY_TRF_VACOFF,
                        STEP.CONV_STOP,
                        STEP.MOVE_TRAY_TRF_Z_READY_POS,
                        STEP.MOVE_TRAY_TRF_Y_READY_POS,
                        STEP.TRAY_TRF_DETECT_UP,
                        STEP.IF_LOADING_POSSIBLE_WAIT,
                        STEP.IF_LOADING_BUSY,
                        STEP.MOVE_TRAY_TRF_Y_LOADING_POS,
                        STEP.MOVE_TRAY_TRF_Z_LOADING_READY_POS,
                        STEP.TRAY_TRF_VACON_BYPASS,
                        STEP.TRAY_TRF_DETECT_UP,
                        STEP.MOVE_TRAY_TRF_Z_LOADING_POS_SENSING,
                        STEP.MOVE_TRAY_TRF_Z_LOADING_RECHECK_POS,
                        STEP.TRAY_TRF_DETECT_DOWN,
                        STEP.LOADING_RETRY_CHECK,
                        STEP.TRAY_TRF_DETECT_UP,
                        STEP.MOVE_TRAY_TRF_Z_READY_POS,
                        STEP.TRAY_TRF_EXIST_CHECK,
                        STEP.MOVE_TRAY_TRF_Y_UNLOADING_POS,
                        STEP.IF_LOADING_COMPLETE_CHECK,
                        STEP.TRAY_TRF_DETECT_DOWN,
                        STEP.CHECK_TRAY_DOWN_INTERLOCK,
                        STEP.MOVE_TRAY_TRF_Z_UNLOADING_POS_SENSING,
                        STEP.TRAY_TRF_VACOFF,
                        STEP.MOVE_TRAY_TRF_Z_READY_POS,
                        STEP.CHECK_IF_TRAY_EXIST_IN_OUT_BUF_CV,
                        STEP.IDLE,
                    };


                    if (!Machine.Trays[(int)UNITTRAY.TRAY_OUT_BUF].exist)
                    {
                        processLoading = new List<STEP>()
                        {
                            STEP.STOPPER_UP,
                            STEP.TRAY_TRF_VACOFF,
                            STEP.CONV_STOP,
                            STEP.MOVE_TRAY_TRF_Z_READY_POS,
                            STEP.MOVE_TRAY_TRF_Y_READY_POS,
                            STEP.TRAY_TRF_DETECT_UP,
                            STEP.IF_LOADING_POSSIBLE_WAIT,
                            STEP.IF_LOADING_BUSY,
                            STEP.MOVE_TRAY_TRF_Y_LOADING_POS,
                            STEP.MOVE_TRAY_TRF_Z_LOADING_READY_POS,
                            STEP.TRAY_TRF_VACON_BYPASS,
                            STEP.TRAY_TRF_DETECT_UP,
                            STEP.MOVE_TRAY_TRF_Z_LOADING_POS_SENSING,
                            STEP.MOVE_TRAY_TRF_Z_LOADING_RECHECK_POS,
                            STEP.TRAY_TRF_DETECT_DOWN,
                            STEP.LOADING_RETRY_CHECK,
                            STEP.TRAY_TRF_DETECT_UP,
                            STEP.MOVE_TRAY_TRF_Z_READY_POS,
                            STEP.TRAY_TRF_EXIST_CHECK,
                            STEP.MOVE_TRAY_TRF_Y_UNLOADING_POS,
                            STEP.IF_LOADING_COMPLETE_CHECK,
                            STEP.MOVE_TRAY_TRF_Z_UNLOADING_POS,
                            STEP.TRAY_TRF_VACOFF,
                            STEP.MOVE_TRAY_TRF_Z_READY_POS,
                            STEP.CHECK_IF_TRAY_EXIST_IN_OUT_BUF_CV,
                            STEP.IDLE,
                        };
                    }
                    StepList = processLoading;
                    Step = StepList[StepIndex = 0];
                    break;

                case MSG.MSG_UNLOADING_PRODUCT:
                    Machine.Trays[(int)UNITTRAY.TRAY_OUT_BUF].ClearInterface();

                    var processUnloading = new List<STEP>()
                    {
                        STEP.IF_UNLOADING_REQUEST,
                        STEP.IF_UNLOADING_BUSY,
                        STEP.STOPPER_DOWN,
                        STEP.CONV_RUN,
                        STEP.IF_UNLOADING_COMPLETE_CHECK,
                        STEP.CONV_STOP,
                        STEP.STOPPER_UP,
                        STEP.IDLE,
                    };

                    StepList = processUnloading;
                    Step = StepList[StepIndex = 0];
                    break;

                case MSG.MSG_MOVE_READY_POS:
                case MSG.MSG_MOVE_LOADING_POS:
                case MSG.MSG_MOVE_UNLOADING_POS:
                    var moveTrayTrfPosition = new List<STEP>()
                    {
                        STEP.MOVE_TRAY_TRF_Z_READY_POS,
                        STEP.MOVE_TRAY_TRF_Y_READY_POS,
                        STEP.IDLE,
                    };

                    if ((MSG)message == MSG.MSG_MOVE_READY_POS) moveTrayTrfPosition[1] = STEP.MOVE_TRAY_TRF_Y_READY_POS;
                    if ((MSG)message == MSG.MSG_MOVE_LOADING_POS) moveTrayTrfPosition[1] = STEP.MOVE_TRAY_TRF_Y_LOADING_POS;
                    if ((MSG)message == MSG.MSG_MOVE_UNLOADING_POS) moveTrayTrfPosition[1] = STEP.MOVE_TRAY_TRF_Y_UNLOADING_POS;

                    StepList = moveTrayTrfPosition;
                    Step = StepList[StepIndex = 0];
                    break;
            }
        }

        protected override void OnProcessing()
        {
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

                case STEP.NEXT_STEP:
                    NextStep();
                    break;

                case STEP.INIT_PRODUCT_DATA_MISMATCH_CHECK:
                    if (Machine.status.mode != SystemMode.SystemModeDRYRUN)
                    {
                        Machine.Trays[(int)UNITTRAY.TRAY_OUT_BUF].exist =
                            Machine.GetTraySensorSignal(TRAY_TYPE.OUT_BUF, TRAY_CV_SENSOR.IN)
                            || Machine.GetTraySensorSignal(TRAY_TYPE.OUT_BUF, TRAY_CV_SENSOR.MID)
                            || Machine.GetTraySensorSignal(TRAY_TYPE.OUT_BUF, TRAY_CV_SENSOR.END);
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
            uint ret1 = 0;

            switch ((AUTOSTEP)AutoStep)
            {
                case AUTOSTEP.ERROR:
                    break;

                case AUTOSTEP.IDLE:
                    break;

                case AUTOSTEP.STOP:
                    break;

                case AUTOSTEP.COMPLETE:
                    if (Machine.sysMode != Machine.SYSMODE.AUTO)
                        AutoStep = AUTOSTEP.IDLE;
                    else
                        AutoStep = AUTOSTEP.LOADING_CHECK;

                    CheckStopBit();
                    break;

                case AUTOSTEP.INIT:
                    initCompl = false;
                    stopBit = false;

                    Machine.Trays[(int)UNITTRAY.TRAY_OUT_BUF].ClearInterface();

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
                    if (CheckStopBit())
                        break;
                    if (Machine.GetProcInitialized() || Machine.sysMode != Machine.SYSMODE.AUTO)
                        AutoStep = AUTOSTEP.INIT_UNLOADING;
                    break;

                case AUTOSTEP.INIT_UNLOADING:
                    if (CheckStopBit())
                        break;

                    SetMessage((int)MSG.MSG_INIT_UNLOADING);
                    AutoStep = AUTOSTEP.INIT_UNLOADING_CHECK;
                    break;

                case AUTOSTEP.INIT_UNLOADING_CHECK:
                    if (CheckStopBit() || !Ready() || Error())
                        break;

                    AutoStep = AUTOSTEP.PROC_SELECT_FOR_RESTART;
                    break;

                case AUTOSTEP.PROC_SELECT_FOR_RESTART:
                    if (CheckStopBit())
                        break;

                    if (Machine.Trays[(int)UNITTRAY.TRAY_OUT_BUF].exist)
                    {
                        AutoStep = AUTOSTEP.LOADING_CHECK;
                    }
                    else
                        AutoStep = AUTOSTEP.LOADING;
                    break;

                case AUTOSTEP.LOADING_CHECK:
                    if (CheckStopBit())
                        break;

                    bool tray_empty = !Machine.Trays[(int)UNITTRAY.TRAY_OUT].exist && !Machine.Trays[(int)UNITTRAY.TRAY_WORK].exist;
                    int target_trays = (int)Machine.recipe.Option(RecipeDefine.OPTION.UNLOADING_TRAYS_COUNTS);


                    if (Machine.Trays[(int)UNITTRAY.TRAY_OUT_BUF].exist)
                    {
                        if (tray_empty || Machine.info.Product(StatusDefine.PRODUCT.TRAY_PLACE_COUNT) >= target_trays)
                        {
                            AutoStep = AUTOSTEP.UNLOADING;
                            break;
                        }
                        else
                        {
                            AutoStep = AUTOSTEP.LOADING;
                            break;
                        }
                    }
                    else
                    {
                        AutoStep = AUTOSTEP.LOADING_CHECK_COMPLETE;
                        break;
                    }

                case AUTOSTEP.LOADING_CHECK_COMPLETE:
                    if (CheckStopBit() || !Ready() || Error())
                        break;

                    AutoStep = AUTOSTEP.LOADING;
                    break;

                case AUTOSTEP.LOADING:
                    if (CheckStopBit())
                        break;

                    SetMessage((int)MSG.MSG_LOADING_PRODUCT);
                    AutoStep = AUTOSTEP.LOADING_COMPL_CHECK;
                    break;

                case AUTOSTEP.LOADING_COMPL_CHECK:
                    if (CheckStopBit() || !Ready() || Error())
                        break;

                    AutoStep = AUTOSTEP.LOADING_CHECK;
                    break;

                case AUTOSTEP.UNLOADING:
                    if (CheckStopBit() || Machine.UnloadStop)
                        break;

                    SetMessage((int)MSG.MSG_UNLOADING_PRODUCT);
                    AutoStep = AUTOSTEP.UNLOADING_COMPL_CHECK;
                    break;

                case AUTOSTEP.UNLOADING_COMPL_CHECK:
                    if (CheckStopBit() || !Ready() || Error())
                        break;

                    AutoStep = AUTOSTEP.COMPLETE;
                    break;
            }

            if (AutoStep != AUTOSTEP.IDLE && AutoStep != AUTOSTEP.ERROR && AutoStep != AUTOSTEP.STOP)
                lastAutoStep = (int)AutoStep;
        }

        private void OnProcessOfInterface()
        {
            int upstream = (int)UNITTRAY.TRAY_WORK;
            int current = (int)UNITTRAY.TRAY_OUT_BUF;
            int downstream = (int)UNITTRAY.TRAY_OUT;

            uint ret1 = 0;

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
                    NextStep();
                    break;

                case STEP.CHECK_IF_TRAY_EXIST_IN_OUT_BUF_CV:
                    if (Machine.status.mode != SystemMode.SystemModeDRYRUN)
                    {
                        Machine.Trays[(int)UNITTRAY.TRAY_OUT_BUF].exist =
                            Machine.GetTraySensorSignal(TRAY_TYPE.OUT_BUF, TRAY_CV_SENSOR.IN)
                            || Machine.GetTraySensorSignal(TRAY_TYPE.OUT_BUF, TRAY_CV_SENSOR.MID)
                            || Machine.GetTraySensorSignal(TRAY_TYPE.OUT_BUF, TRAY_CV_SENSOR.END);

                        if (!Machine.Trays[(int)UNITTRAY.TRAY_OUT_BUF].exist)
                        {
                            if (timeWait[(int)TIMER.TIMEOUT].Elapsed > Machine.param.Time(ParameterDefine.TIME.VACUUM_ON_TIME_OUT))
                                SetError(ECODE.TIMEOUT_TRAY_NOT_EXIST_IN_OUTBUF_CV_AFTER_UNLOAD);
                            break;
                        }
                    }
                    else
                    {
                        Util.Delay(2000);
                    }
                    NextStep();
                    break;

                case STEP.IF_LOADING_COMPLETE_CHECK:
                    if (Machine.Trays[upstream].unloading)
                        break;

                    Machine.info.ProductInc(StatusDefine.PRODUCT.TRAY_PLACE_COUNT, 1);
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

                    Machine.info.Product(StatusDefine.PRODUCT.PRODUCT_PLACE_COUNT, 0);
                    Machine.info.Product(StatusDefine.PRODUCT.TRAY_PLACE_COUNT, 0);

                    timeWait[(int)TIMER.TIMEOUT].Reset();
                    NextStep();
                    break;

                case STEP.IF_UNLOADING_COMPLETE_CHECK:

                    if (timeWait[(int)TIMER.TIMEOUT].Elapsed > Machine.param.Time(ParameterDefine.TIME.CONV_RUN_TIME_OUT))
                    {
                        SetError(ECODE.TIMEOUT_TRAY_CONV_RUN);
                        break;
                    }
                    if (Machine.status.mode == SystemMode.SystemModeDRYRUN) Util.Delay(1000);
                    else
                    {
                        Machine.Trays[(int)UNITTRAY.TRAY_OUT_BUF].exist =
                               Machine.GetTraySensorSignal(TRAY_TYPE.OUT_BUF, TRAY_CV_SENSOR.IN)
                               || Machine.GetTraySensorSignal(TRAY_TYPE.OUT_BUF, TRAY_CV_SENSOR.MID)
                               || Machine.GetTraySensorSignal(TRAY_TYPE.OUT_BUF, TRAY_CV_SENSOR.END);

                        if (Machine.Trays[(int)UNITTRAY.TRAY_OUT_BUF].exist
                            || !Machine.GetTraySensorSignal(TRAY_TYPE.OUT, TRAY_CV_SENSOR.END)) break;
                    }
                    Machine.Trays[current].unloadingRequest = false;
                    Machine.Trays[current].unloading = false;

                    if (Machine.Trays[downstream].loading)
                        break;

                    Machine.Trays[current].exist = false;
                    NextStep();
                    resumeStepIndex = StepIndex;
                    break;
            }
        }

        private void OnProcessOfMotion()
        {
            double approachVel_Z = 1000 * Machine.param.Velocity(ParameterDefine.VELOCITY.TRAY_TRF_Z_APPROACH_VEL);
            double vel_z = 1000 * Machine.param.Velocity(ParameterDefine.VELOCITY.TRAY_TRF_Z_VEL);
            double acc_z = 1000 * Machine.param.Velocity(ParameterDefine.VELOCITY.TRAY_TRF_Z_ACC);
            double dec_z = 1000 * Machine.param.Velocity(ParameterDefine.VELOCITY.TRAY_TRF_Z_DEC);

            double vel_y = 1000 * Machine.param.Velocity(ParameterDefine.VELOCITY.TRAY_TRF_Y_VEL);
            double acc_y = 1000 * Machine.param.Velocity(ParameterDefine.VELOCITY.TRAY_TRF_Y_ACC);
            double dec_y = 1000 * Machine.param.Velocity(ParameterDefine.VELOCITY.TRAY_TRF_Y_DEC);

            double pos = 0.0;
            double apos = 0.0;

            uint ret1 = 0;

            MotionState state = MotionState.MotionStateIDLE;

            switch ((STEP)Step)
            {
                case STEP.MOVE_TRAY_TRF_Z_POS_CHECK:
                    if (!Machine.motion.MoveAxisDoneCheck((int)AXIS.TRAY_TRF_Z))
                        break;

                    NextStep();
                    break;

                case STEP.MOVE_TRAY_TRF_Z_READY_POS:
                case STEP.MOVE_TRAY_TRF_Z_UNLOADING_POS:
                    if (Step == STEP.MOVE_TRAY_TRF_Z_READY_POS) pos = 1000 * Machine.param.Position(ParameterDefine.POSITION.TRAY_TRF_Z_READY_POS);
                    if (Step == STEP.MOVE_TRAY_TRF_Z_UNLOADING_POS) pos = 1000 * Machine.param.Position(ParameterDefine.POSITION.TRAY_TRF_Z_UNLOADING_POS);
                    timeWait[(int)TIMER.TIMEOUT].Start();
                    Machine.motion.MoveAxisAbs((int)AXIS.TRAY_TRF_Z, pos, vel_z, acc_z, dec_z);
                    Step = STEP.MOVE_TRAY_TRF_Z_POS_CHECK;
                    break;

                case STEP.MOVE_TRAY_TRF_Z_LOADING_RECHECK_POS:
                    Machine.motion.GetAxisActualPos((int)AXIS.TRAY_TRF_Z, ref pos);
                    pos -= 1000 * Machine.param.Position(ParameterDefine.POSITION.TRAY_TRF_Z_LOADING_RECHECK);
                    Machine.motion.MoveAxisAbs((int)AXIS.TRAY_TRF_Z, pos, approachVel_Z, acc_z, dec_z);
                    Step = STEP.MOVE_TRAY_TRF_Z_POS_CHECK;
                    break;

                case STEP.MOVE_TRAY_TRF_Z_LOADING_READY_POS:
                    pos = 1000 * (Machine.param.Position(ParameterDefine.POSITION.TRAY_TRF_Z_LOADING_POS) - 20);
                    Machine.motion.MoveAxisAbs((int)AXIS.TRAY_TRF_Z, pos, vel_z, acc_z, dec_z);
                    Step = STEP.MOVE_TRAY_TRF_Z_POS_CHECK;
                    break;

                case STEP.MOVE_TRAY_TRF_Z_LOADING_POS_SENSING:
                    pos = 1000 * Machine.param.Position(ParameterDefine.POSITION.TRAY_TRF_Z_LOADING_POS);
                    Machine.motion.MoveAxisAbs((int)AXIS.TRAY_TRF_Z, pos, approachVel_Z, acc_z, dec_z);
                    Step = STEP.MOVE_TRAY_TRF_Z_VAC_ON_CHECK;
                    break;

                case STEP.MOVE_TRAY_TRF_Z_UNLOADING_POS_SENSING:
                    pos = 1000 * Machine.param.Position(ParameterDefine.POSITION.TRAY_TRF_Z_LOADING_POS);
                    Machine.motion.MoveAxisAbs((int)AXIS.TRAY_TRF_Z, pos, approachVel_Z, acc_z, dec_z);
                    Step = STEP.MOVE_TRAY_TRF_Z_DETECT_LOC_CHECK;
                    break;

                case STEP.MOVE_TRAY_TRF_Z_DETECT_LOC_CHECK:
                    Machine.IO.GetIn((int)DI.TRAY_TRF_EXIST_DETECT, ref ret1);
                    if (ret1 == 1)
                    {
                        Machine.motion.AxisStop((int)AXIS.TRAY_TRF_Z, true);
                        Util.Delay(100);
                    }
                    else
                    {
                        Machine.motion.GetAxisActualPos((int)AXIS.TRAY_TRF_Z, ref pos);
                        double unloadingPos = 1000 * Machine.param.Position(ParameterDefine.POSITION.TRAY_TRF_Z_UNLOADING_POS);
                        if (pos >= unloadingPos)
                        {
                            SetError(ECODE.TIMEOUT_TRAY_TRF_DETECT);
                            break;
                        }
                        break;
                    }

                    Machine.motion.GetAxisMotionState((int)AXIS.TRAY_TRF_Z, ref state);
                    if (state != MotionState.MotionStateIDLE)
                        break;
                    NextStep();
                    break;

                case STEP.MOVE_TRAY_TRF_Z_VAC_ON_CHECK:
                    Machine.IO.GetIn((int)DI.TRAY_TRF_VAC_ON1, ref ret1);
                    if (Machine.status.mode == SystemMode.SystemModeDRYRUN) ret1 = 1;
                    if (ret1 == 1)
                    {
                        Machine.motion.AxisStop((int)AXIS.TRAY_TRF_Z, true);
                        Util.Delay((int)Machine.param.Time(ParameterDefine.TIME.TRAY_TRF_VACUUM_ON_WAIT_TIME));
                    }
                    else
                    {
                        Machine.motion.GetAxisActualPos((int)AXIS.TRAY_TRF_Z, ref pos);
                        double loadingPos = 1000 * Machine.param.Position(ParameterDefine.POSITION.TRAY_TRF_Z_LOADING_POS);
                        if (pos >= loadingPos)
                        {
                            SetError(ECODE.TIMEOUT_TRAY_TRF_VACON);
                            break;
                        }
                        break;
                    }

                    Machine.motion.GetAxisMotionState((int)AXIS.TRAY_TRF_Z, ref state);
                    if (state != MotionState.MotionStateIDLE)
                        break;
                    NextStep();
                    break;

                case STEP.MOVE_TRAY_TRF_Y_POS_CHECK:
                    if (!Machine.motion.MoveAxisDoneCheck((int)AXIS.TRAY_TRF_Y))
                        break;

                    NextStep();
                    break;

                case STEP.MOVE_TRAY_TRF_Y_READY_POS:
                case STEP.MOVE_TRAY_TRF_Y_LOADING_POS:
                case STEP.MOVE_TRAY_TRF_Y_UNLOADING_POS:
                    if (Step == STEP.MOVE_TRAY_TRF_Y_READY_POS)
                    {
                        pos = Machine.param.Position(ParameterDefine.POSITION.TRAY_TRF_Y_READY_POS);
                    }

                    if (Step == STEP.MOVE_TRAY_TRF_Y_UNLOADING_POS)
                    {
                        pos = Machine.param.Position(ParameterDefine.POSITION.TRAY_TRF_Y_UNLOADING_POS);
                    }

                    if (Step == STEP.MOVE_TRAY_TRF_Y_LOADING_POS)
                    {
                        pos = Machine.param.Position(ParameterDefine.POSITION.TRAY_TRF_Y_LOADING_POS);
                    }

                    Machine.motion.MoveAxisAbs((int)AXIS.TRAY_TRF_Y, 1000 * pos, vel_y, acc_y, dec_y);
                    Step = STEP.MOVE_TRAY_TRF_Y_POS_CHECK;
                    break;

                case STEP.CONV_RUN:
                    Machine.IO.SetOut((int)DO.TRAY_OUT_BUF_CONV_RUN, 1);
                    NextStep();
                    timeWait[(int)TIMER.DELAY].Start();
                    break;

                case STEP.CONV_STOP:
                    Machine.IO.SetOut((int)DO.TRAY_OUT_BUF_CONV_RUN, 0);
                    NextStep();
                    timeWait[(int)TIMER.DELAY].Start();
                    break;

                case STEP.INIT_STOPPER_UP:
                    if (!Machine.GetTraySensorSignal(TRAY_TYPE.OUT_BUF, TRAY_CV_SENSOR.END)
                        && !Machine.GetTraySensorSignal(TRAY_TYPE.OUT, TRAY_CV_SENSOR.IN))
                        Step = STEP.STOPPER_UP;
                    else NextStep();
                    break;
            }
        }

        private void OnProcessOfIO()
        {
            uint ret1 = 0;
            uint ret2 = 0;
            uint ret3 = 0;
            uint ret4 = 0;

            switch ((STEP)Step)
            {
                case STEP.STOPPER_UP:
                    Machine.IO.SetOut((int)DO.TRAY_OUT_BUF_STOPPER_UP, 1);
                    Machine.IO.SetOut((int)DO.TRAY_OUT_BUF_STOPPER_DOWN, 0);

                    Step = STEP.STOPPER_UP_CHECK;
                    timeWait[(int)TIMER.DELAY].Start();
                    timeWait[(int)TIMER.TIMEOUT].Start();
                    break;

                case STEP.STOPPER_UP_CHECK:
                    if (timeWait[(int)TIMER.TIMEOUT].Elapsed > Machine.param.Time(ParameterDefine.TIME.CYLINDER_TIME_OUT))
                    {
                        SetError(ECODE.TIMEOUT_TRAY_OUTBUF_STOPPER_UP);
                        break;
                    }
                    Machine.IO.GetIn((int)DI.TRAY_OUT_BUF_STOPPER_UP, ref ret1);
                    Machine.IO.GetIn((int)DI.TRAY_OUT_BUF_STOPPER_DOWN, ref ret2);

                    if (ret1 == 1 && ret2 == 0)
                        NextStep();
                    break;

                case STEP.STOPPER_DOWN:
                    Machine.IO.SetOut((int)DO.TRAY_OUT_BUF_STOPPER_UP, 0);
                    Machine.IO.SetOut((int)DO.TRAY_OUT_BUF_STOPPER_DOWN, 1);

                    Step = STEP.STOPPER_DOWN_CHECK;
                    timeWait[(int)TIMER.DELAY].Start();
                    timeWait[(int)TIMER.TIMEOUT].Start();
                    break;

                case STEP.STOPPER_DOWN_CHECK:
                    if (timeWait[(int)TIMER.TIMEOUT].Elapsed > Machine.param.Time(ParameterDefine.TIME.CYLINDER_TIME_OUT))
                    {
                        SetError(ECODE.TIMEOUT_TRAY_OUTBUF_STOPPER_DOWN);
                        break;
                    }

                    Machine.IO.GetIn((int)DI.TRAY_OUT_BUF_STOPPER_UP, ref ret1);
                    Machine.IO.GetIn((int)DI.TRAY_OUT_BUF_STOPPER_DOWN, ref ret2);

                    if (ret1 == 0 && ret2 == 1)
                        NextStep();
                    break;

                case STEP.TRAY_TRF_VACON_BYPASS:
                case STEP.TRAY_TRF_VACON:
                    if (Machine.status.mode == SystemMode.SystemModeDRYRUN)
                    {
                        NextStep();
                        break;
                    }
                    Machine.IO.SetOut((int)DO.TRAY_TRF_VACUUM, 1);
                    if (Step == STEP.TRAY_TRF_VACON_BYPASS) NextStep();
                    else if (Step == STEP.TRAY_TRF_VACON) Step = STEP.TRAY_TRF_VACON_CHECK;
                    timeWait[(int)TIMER.TIMEOUT].Start();
                    break;

                case STEP.TRAY_TRF_VACON_CHECK:
                    Machine.IO.GetIn((int)DI.TRAY_TRF_VAC_ON1, ref ret1);
                    if (Machine.status.mode == SystemMode.SystemModeDRYRUN) ret1 = 1;
                    if (ret1 == 1)
                    {
                        Util.Delay(1000);
                        NextStep();
                    }
                    break;

                case STEP.CHECK_TRAY_DOWN_INTERLOCK:
                    Machine.IO.GetIn((int)DI.TRAY_TRF_EXIST_DETECT, ref ret1);
                    if (ret1 == 1)
                    {
                        Util.Delay(2000);
                        Machine.IO.GetIn((int)DI.TRAY_TRF_EXIST_DETECT, ref ret1);
                        if (ret1 == 1)
                        {
                            SetError(ECODE.POSITION_ERROR_TRAY_OUTBUF_UNLOAD_SENSING_POS);
                            break;
                        }
                    }
                    NextStep();
                    break;

                case STEP.CHECK_IF_TRAY_IN_TRANSFER:
                    Machine.IO.GetIn((int)DI.TRAY_TRF_EXIST_DETECT, ref ret2);

                    if (ret2 == 0)
                    {
                        int nIndex = GetStepIndex(StepList, STEP.IDLE);
                        if (nIndex >= 0)
                            Step = StepList[StepIndex = nIndex];
                    }

                    NextStep();
                    break;

                case STEP.LOADING_RETRY_CHECK:
                    if (Machine.status.mode == SystemMode.SystemModeDRYRUN)
                    {
                        NextStep();
                        break;
                    }
                    Machine.IO.GetIn((int)DI.TRAY_TRF_EXIST_DETECT, ref ret1);
                    Machine.IO.GetIn((int)DI.TRAY_TRF_VAC_ON1, ref ret2);
                    if (ret1 == 1)
                    {
                        SetError(ECODE.POSITION_ERROR_TRAY_OUTBUF_UNLOAD_SENSING_POS);
                        break;
                    }
                    if (ret2 == 0)
                    {
                        int loadingRetryMaxCount = (int)Machine.param.Position(ParameterDefine.POSITION.TRAY_TRF_Z_LOADING_RETRY_MAX_COUNT);
                        if (loadingRetryCount >= loadingRetryMaxCount)
                        {
                            SetError(ECODE.TIMEOUT_TRAY_TRF_VACON);
                            break;
                        }
                        int nIndex = GetStepIndex(StepList, STEP.TRAY_TRF_VACON_BYPASS);
                        if (nIndex >= 0)
                            Step = StepList[StepIndex = nIndex];
                        loadingRetryCount++;
                    }
                    else if (ret1 == 0 && ret2 == 1)
                    {
                        NextStep();
                    }
                    break;

                case STEP.TRAY_TRF_VACOFF:
                    if (Machine.status.mode == SystemMode.SystemModeDRYRUN)
                    {
                        NextStep();
                        break;
                    }
                    Machine.IO.SetOut((int)DO.TRAY_TRF_VACUUM, 0);

                    Step = STEP.TRAY_TRF_VACOFF_CHECK;
                    timeWait[(int)TIMER.TIMEOUT].Start();
                    break;

                case STEP.TRAY_TRF_VACOFF_CHECK:
                    if (timeWait[(int)TIMER.TIMEOUT].Elapsed > Machine.param.Time(ParameterDefine.TIME.VACUUM_ON_TIME_OUT))
                    {
                        SetError(ECODE.TIMEOUT_TRAY_TRF_VACOFF);
                        break;
                    }

                    Machine.IO.GetIn((int)DI.TRAY_TRF_VAC_ON1, ref ret1);

                    if (ret1 == 0)
                    {
                        Util.Delay(1000);
                        NextStep();
                    }
                    break;

                case STEP.TRAY_TRF_DETECT_UP:
                    Machine.IO.SetOut((int)DO.TRAY_TRF_EXIST_UP, 1);
                    Machine.IO.SetOut((int)DO.TRAY_TRF_EXIST_DOWN, 0);

                    Step = STEP.TRAY_TRF_DETECT_UP_CHECK;
                    timeWait[(int)TIMER.DELAY].Start();
                    timeWait[(int)TIMER.TIMEOUT].Start();
                    break;

                case STEP.TRAY_TRF_DETECT_UP_CHECK:
                    if (timeWait[(int)TIMER.TIMEOUT].Elapsed > Machine.param.Time(ParameterDefine.TIME.CYLINDER_TIME_OUT))
                    {
                        SetError(ECODE.TIMEOUT_TRAY_TRF_UP);
                        break;
                    }

                    Machine.IO.GetIn((int)DI.TRAY_TRF_DETECT_L_DOWN, ref ret2);
                    Machine.IO.GetIn((int)DI.TRAY_TRF_DETECT_R_DOWN, ref ret4);

                    if (ret2 == 0 && ret4 == 0)
                        NextStep();
                    break;

                case STEP.TRAY_TRF_DETECT_DOWN:
                    Machine.IO.SetOut((int)DO.TRAY_TRF_EXIST_UP, 0);
                    Machine.IO.SetOut((int)DO.TRAY_TRF_EXIST_DOWN, 1);

                    Step = STEP.TRAY_TRF_DETECT_DOWN_CHECK;
                    timeWait[(int)TIMER.DELAY].Start();
                    timeWait[(int)TIMER.TIMEOUT].Start();
                    break;

                case STEP.TRAY_TRF_DETECT_DOWN_CHECK:
                    if (timeWait[(int)TIMER.TIMEOUT].Elapsed > Machine.param.Time(ParameterDefine.TIME.CYLINDER_TIME_OUT))
                    {
                        SetError(ECODE.TIMEOUT_TRAY_TRF_DOWN);
                        break;
                    }

                    Machine.IO.GetIn((int)DI.TRAY_TRF_DETECT_L_DOWN, ref ret2);
                    Machine.IO.GetIn((int)DI.TRAY_TRF_DETECT_R_DOWN, ref ret4);

                    if (ret2 == 1 && ret4 == 1)
                        NextStep();
                    break;

                case STEP.TRAY_TRF_EXIST_CHECK:
                    if (Machine.status.mode == SystemMode.SystemModeDRYRUN)
                    {
                        NextStep();
                        break;
                    }
                    Machine.IO.GetIn((int)DI.TRAY_TRF_EXIST_DETECT, ref ret1);

                    if (ret1 == 0)
                        SetError(ECODE.TIMEOUT_TRAY_TRF_VACON);
                    else
                        NextStep();
                    break;
            }
        }

        private int GetStepIndex(List<STEP> stepList, STEP eStep)
        {
            for (int i = 0; i < stepList.Count; i++)
            {
                if (stepList[i].ToString() == eStep.ToString())
                {
                    return i;
                }
            }
            return -1;
        }

        public override void SetHeadTarget(int iTarget)
        {
            throw new NotImplementedException();
        }
    }
}
