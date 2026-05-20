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
using System.Threading;
using TopEng.Utils;

namespace CAMASSEMBLYMACHINE.Process
{
    public class ProcessJigOutBuf : IProcess
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
            UNLOADING,
            UNLOADING_COMPL_CHECK,
        }

        public enum STEP
        {
            // 시스템 스텝
            ERROR = -1,
            IDLE,
            STOP,
            NEXT_STEP,
            WAIT_LOOP,

            INIT_PRODUCT_DATA_MISMATCH_CHECK,

            // TIME
            CONV_STOP_INIT_WITH_DELAY,
            CONV_STOP_INIT_WITH_DELAY_CHECK,
            CONV_STOP_WITH_DELAY,
            CONV_STOP_WITH_DELAY_CHECK,

            // INTERFACE
            IF_LOADING_POSSIBLE_WAIT,
            IF_LOADING_BUSY,
            IF_LOADING_COMPLETE_CHECK,
            IF_JIG_ULD_EXIST_ON,
            IF_JIG_ULD_EXIST_OFF,
            IF_DOWNSTREAM_EDM_REPORT,
            IF_DOWNSTREAM_RUN_CHECK,
            IF_CONV_RUN_ON,
            IF_CONV_RUN_OFF,
            IF_DOWNSTREAM_COMPLETE_CHECK,

            // MOTION
            CONV_RUN,
            CONV_STOP,

            // I/O
            STOPPER_UP,
            STOPPER_UP_CHECK,
            STOPPER_DOWN,
            STOPPER_DOWN_CHECK,
            INIT_STOPPER_UP,
        }

        public enum MSG
        {
            MSG_SINGLE,
            MSG_PROCESS_INIT_CONV,
            MSG_LOADING_PRODUCT,
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
        }

        public override void EStop(bool pause)
        {
            EStopBit = true;

            for (int i = 0; i < StepList.Count; i++)
                StepList[i] = STEP.IDLE;

            AutoStep = AUTOSTEP.IDLE;
            Step = STEP.IDLE;

            Machine.Jigs[(int)UNITJIG.JIG_OUT_BUF].ClearInterface();
            Machine.IO.SetOut((int)DO.JIG_OUT_BUF_CONV_RUN, 0);
            Machine.IO.SetOut((int)DO.IF_JIG_ULD_CONV_RUN_WORK, 0);
            Machine.IO.SetOut((int)DO.IF_JIG_ULD_EXIST_WORK, 0);
            Machine.IO.SetOut((int)DO.IF_MACHINE_RUN_SIG_FOR_REAR, 0);
        }

        public override void Stop()
        {
            stopBit = true;

            Machine.Jigs[(int)UNITJIG.JIG_OUT_BUF].ClearInterface();
            Machine.IO.SetOut((int)DO.JIG_OUT_BUF_CONV_RUN, 0);
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

        public ProcessJigOutBuf()
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

                case MSG.MSG_PROCESS_INIT_CONV:
                    var processInit = new List<STEP>()
                    {
                       STEP.CONV_RUN,
                       STEP.CONV_STOP_INIT_WITH_DELAY,
                       STEP.INIT_PRODUCT_DATA_MISMATCH_CHECK,
                       STEP.IDLE,
                    };

                    StepList = processInit;
                    Step = StepList[StepIndex = 0];
                    break;

                case MSG.MSG_LOADING_PRODUCT:
                    Machine.Jigs[(int)UNITJIG.JIG_OUT_BUF].ClearInterface();
                    Machine.IO.SetOut((int)DO.IF_JIG_ULD_CONV_RUN_WORK, 0);
                    Machine.IO.SetOut((int)DO.IF_JIG_ULD_EXIST_WORK, 0);

                    var ProcessLoading = new List<STEP>()
                    {
                        STEP.IF_LOADING_POSSIBLE_WAIT,
                        STEP.STOPPER_UP,
                        STEP.CONV_RUN,
                        STEP.IF_LOADING_BUSY,
                        STEP.IF_LOADING_COMPLETE_CHECK,
                        STEP.CONV_STOP_WITH_DELAY,
                        STEP.IDLE,
                    };

                    StepList = ProcessLoading;
                    Step = StepList[StepIndex = 0];
                    break;

                case MSG.MSG_UNLOADING_PRODUCT:
                    Machine.Jigs[(int)UNITJIG.JIG_OUT_BUF].ClearInterface();

                    var processUnloading = new List<STEP>()
                    {
                        STEP.IF_JIG_ULD_EXIST_ON, // 개발PGM Detect Out 센서 역할
                        STEP.IF_DOWNSTREAM_EDM_REPORT,
                        STEP.IF_DOWNSTREAM_RUN_CHECK,
                        STEP.STOPPER_DOWN,
                        STEP.CONV_RUN,
                        STEP.IF_CONV_RUN_ON,
                        STEP.IF_DOWNSTREAM_COMPLETE_CHECK,
                        STEP.CONV_STOP,
                        STEP.STOPPER_UP,
                        STEP.IF_CONV_RUN_OFF,
                        STEP.IF_JIG_ULD_EXIST_OFF,
                        STEP.IDLE,
                    };
                    if (ProductExistCheck() == CONVSTATUS.UNLOADING)
                    {
                        processUnloading[GetStepIndex(processUnloading, STEP.IF_JIG_ULD_EXIST_ON)] = STEP.NEXT_STEP;
                        processUnloading[GetStepIndex(processUnloading, STEP.IF_DOWNSTREAM_EDM_REPORT)] = STEP.NEXT_STEP;
                        processUnloading[GetStepIndex(processUnloading, STEP.IF_DOWNSTREAM_RUN_CHECK)] = STEP.NEXT_STEP;
                    }
                    StepList = processUnloading;
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
                    error_proc = false;
                    stopBit = false;
                    EStopBit = false;

                    timeWait[(int)TIMER.TIMEOUT].Reset();
                    break;

                case STEP.NEXT_STEP:
                    NextStep();
                    break;

                case STEP.WAIT_LOOP:
                    Machine.IO.SetOut((int)DO.JIG_OUT_BUF_CONV_RUN, 0);
                    break;

                case STEP.STOP:
                    timeWait[(int)TIMER.TIMEOUT].Reset();
                    break;

                case STEP.INIT_PRODUCT_DATA_MISMATCH_CHECK:
                    if (Machine.status.mode != SystemMode.SystemModeDRYRUN)
                    {
                        Machine.Jigs[(int)UNITJIG.JIG_OUT_BUF].exist = Machine.GetJigSignal(JIG_TYPE.TOP_OUT, JIG_SENSOR.IN)
                            && Machine.GetJigSignal(JIG_TYPE.TOP_OUT, JIG_SENSOR.OUT);
                    }

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
            CONVSTATUS convstat = CONVSTATUS.EMPTY;

            switch ((AUTOSTEP)AutoStep)
            {
                case AUTOSTEP.ERROR:
                    break;

                case AUTOSTEP.IDLE:
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
                    Machine.Jigs[(int)UNITJIG.JIG_OUT_BUF].ClearInterface();

                    if (Machine.processPause && StepList != null)
                    {
                        if (StepList.Count > resumeStepIndex)
                        {
                            initCompl = true;
                            AutoStep = AUTOSTEP.OTHER_PROCESS_INIT_WAIT;
                            break;
                        }
                    }

                    initCompl = false;
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
                    if (CheckStopBit())
                        break;
                    if (Machine.GetProcInitialized() || Machine.sysMode != Machine.SYSMODE.AUTO)
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
                    }
                    break;

                case AUTOSTEP.PROC_SELECT_FOR_RESTART:
                    Machine.IO.SetOut((int)DO.IF_MACHINE_RUN_SIG_FOR_REAR, 1);
                    convstat = ProductExistCheck();

                    if (convstat == CONVSTATUS.EMPTY)
                        AutoStep = AUTOSTEP.LOADING;
                    else if (convstat == CONVSTATUS.EXIST)
                        AutoStep = AUTOSTEP.UNLOADING;
                    else if (convstat == CONVSTATUS.LOADING)
                        AutoStep = AUTOSTEP.LOADING;
                    else if (convstat == CONVSTATUS.UNLOADING)
                        AutoStep = AUTOSTEP.UNLOADING;
                    break;

                case AUTOSTEP.LOADING:
                    SetMessage((int)MSG.MSG_LOADING_PRODUCT);

                    AutoStep = AUTOSTEP.LOADING_COMPL_CHECK;
                    break;

                case AUTOSTEP.LOADING_COMPL_CHECK:
                    if (CheckStopBit() || !Ready() || Error())
                        break;

                    AutoStep = AUTOSTEP.UNLOADING;
                    break;

                case AUTOSTEP.UNLOADING:
                    if (CheckStopBit() || Machine.UnloadStop)
                        break;
                    uint ret1 = 0;
                    if (Machine.status.mode != SystemMode.SystemModeDRYRUN)
                    {
                        Machine.IO.GetIn((int)DI.IF_REAR_MACHINE_RUN, ref ret1);
                        if (ret1 != 1) break;
                    }
                    SetMessage((int)MSG.MSG_UNLOADING_PRODUCT);
                    AutoStep = AUTOSTEP.UNLOADING_COMPL_CHECK;
                    break;

                case AUTOSTEP.UNLOADING_COMPL_CHECK:
                    if (Machine.status.mode != SystemMode.SystemModeDRYRUN)
                    {
                        ret1 = 0;
                        Machine.IO.GetIn((int)DI.IF_REAR_MACHINE_RUN, ref ret1);
                        if (ret1 != 1)
                        {
                            Step = StepList[StepIndex] = STEP.WAIT_LOOP;
                            break;
                        }
                        else if (Step == STEP.WAIT_LOOP)
                        {
                            var processUnloading = new List<STEP>()
                        {
                            STEP.IF_JIG_ULD_EXIST_ON, // 개발PGM Detect Out 센서 역할
                            STEP.IF_DOWNSTREAM_EDM_REPORT,
                            STEP.IF_DOWNSTREAM_RUN_CHECK,
                            STEP.STOPPER_DOWN,
                            STEP.CONV_RUN,
                            STEP.IF_CONV_RUN_ON,
                            STEP.IF_DOWNSTREAM_COMPLETE_CHECK,
                            STEP.CONV_STOP,
                            STEP.STOPPER_UP,
                            STEP.IF_CONV_RUN_OFF,
                            STEP.IF_JIG_ULD_EXIST_OFF,
                            STEP.IDLE,
                        };

                            StepList = processUnloading;
                            Step = StepList[StepIndex];
                            if (StepIndex >= GetStepIndex(processUnloading, STEP.CONV_RUN))
                                Machine.IO.SetOut((int)DO.JIG_OUT_BUF_CONV_RUN, 1);
                        }
                    }
                    if (CheckStopBit() || !Ready() || Error())
                        break;

                    AutoStep = AUTOSTEP.COMPLETE;
                    break;
            }

            if ((int)AutoStep > (int)AUTOSTEP.PROC_SELECT_FOR_RESTART)
                lastAutoStep = (int)AutoStep;

            if (EStopBit)
                AutoStep = AUTOSTEP.IDLE;
        }

        private void OnProcessOfInterface()
        {
            bool report = Machine.param.Option(ParameterDefine.OPTION.REPORT_EDM_SERVER) == 1 ? true : false;

            uint returnCurrent = 0;
            uint returnRunning = 0;

            const int UPSTREAM = (int)UNITJIG.JIG_WORK;
            const int CURRENT = (int)UNITJIG.JIG_OUT_BUF;
            //const int DOWNSTREAM = (int)UNITJIG.JIG_WORK;

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
                        SetError(ECODE.TIMEOUT_JIG_OUTBUF_LOADING);
                        break;
                    }

                    bool isJigExist = Machine.GetJigSignal(JIG_TYPE.TOP_OUT, JIG_SENSOR.IN)
                        && Machine.GetJigSignal(JIG_TYPE.TOP_OUT, JIG_SENSOR.OUT);
                    if (Machine.status.mode == SystemMode.SystemModeDRYRUN) isJigExist = true;

                    if (!isJigExist) break;
                    if (Machine.Jigs[UPSTREAM].unloading && !resumeProc) break;

                    // 자재가 있다고 판단되면 데이터를 생성한다.
                    Machine.Jigs[(int)UNITJIG.JIG_OUT_BUF].from_id = Machine.Parts[(int)UNITPART.JIG1].from_id;
                    Machine.Jigs[CURRENT].loading = false;
                    Machine.Jigs[CURRENT].exist = true;
                    NextStep();

                    resumeStepIndex = StepIndex;
                    resumeProc = false;
                    break;
                #endregion
                #region UNLOADING INTERFACE
                case STEP.IF_JIG_ULD_EXIST_ON:
                    resumeStepIndex = StepIndex;

                    if (Machine.AloneMode)
                        NextStep();
                    else
                    {
                        Machine.IO.SetOut((int)DO.IF_JIG_ULD_EXIST_WORK, 1);
                        NextStep();
                    }

                    timeWait[(int)TIMER.TIMEOUT].Reset();
                    timeWait[(int)TIMER.DELAY].Reset();
                    break;

                case STEP.IF_JIG_ULD_EXIST_OFF:
                    resumeStepIndex = StepIndex;

                    if (Machine.AloneMode)
                        NextStep();
                    else
                    {
                        Machine.IO.SetOut((int)DO.IF_JIG_ULD_EXIST_WORK, 0);
                        NextStep();
                    }

                    timeWait[(int)TIMER.TIMEOUT].Reset();
                    timeWait[(int)TIMER.DELAY].Reset();
                    break;

                case STEP.IF_DOWNSTREAM_EDM_REPORT:
                    if (Machine.Jigs[CURRENT].productCount > 0)
                    {
                    }
                    NextStep();
                    break;

                case STEP.IF_DOWNSTREAM_RUN_CHECK:
                    resumeStepIndex = StepIndex;

                    if (Machine.AloneMode)
                        NextStep();
                    else
                    {
                        Machine.IO.GetIn((int)DI.IF_JIG_DOWNSTREAM_RUN_CHECK_WORK, ref returnRunning);
                        if (returnRunning == 1)
                            NextStep();
                    }

                    timeWait[(int)TIMER.TIMEOUT].Reset();
                    timeWait[(int)TIMER.DELAY].Reset();
                    break;

                case STEP.IF_CONV_RUN_ON:
                    resumeStepIndex = StepIndex;

                    if (Machine.AloneMode)
                        NextStep();
                    else
                    {
                        Machine.IO.SetOut((int)DO.IF_JIG_ULD_CONV_RUN_WORK, 1);
                        NextStep();
                    }

                    timeWait[(int)TIMER.TIMEOUT].Reset();
                    timeWait[(int)TIMER.DELAY].Reset();
                    break;

                case STEP.IF_CONV_RUN_OFF:
                    resumeStepIndex = StepIndex;

                    if (Machine.AloneMode)
                        NextStep();
                    else
                    {
                        Machine.IO.SetOut((int)DO.IF_JIG_ULD_CONV_RUN_WORK, 0);
                        NextStep();
                    }

                    timeWait[(int)TIMER.TIMEOUT].Reset();
                    timeWait[(int)TIMER.DELAY].Reset();
                    break;

                case STEP.IF_DOWNSTREAM_COMPLETE_CHECK:
                    Machine.IO.GetIn((int)DI.IF_JIG_DOWNSTREAM_RUN_CHECK_WORK, ref returnRunning);
                    bool isUnloadComplete =
                       returnRunning == 0
                       && !Machine.GetJigSignal(JIG_TYPE.TOP_OUT, JIG_SENSOR.IN)
                       && !Machine.GetJigSignal(JIG_TYPE.TOP_OUT, JIG_SENSOR.OUT);

                    // 더미런 시에는 항상 OFF 이다.
                    if (Machine.status.mode == SystemMode.SystemModeDRYRUN) isUnloadComplete = true;

                    // Next CONVEYOR STOP 시, returnRunning = 0
                    if (!isUnloadComplete)
                        timeWait[(int)TIMER.TRIG].Reset();
                    else
                    {
                        //if (timeWait[(int)TIMER.TRIG].Elapsed < 1000)
                        //    break;
                        //else
                        {
                            if (Machine.Jigs[CURRENT].productCount > 0)
                            {
                                EDM.UnloadingSetsGood(true);
                                Util.Delay(50);
                                EDM.UnloadingSetsGood(false);
                                Util.Delay(50);
                                EDM.UnloadingSetsGood(true);
                                Util.Delay(50);
                            	EDM.UnloadingSetsGood(false);
                                Machine.info.ProductInc(StatusDefine.PRODUCT.PRODUCT_INPUT_TOTAL_COUNT, 2);
                                Machine.info.ProductInc(StatusDefine.PRODUCT.PRODUCT_OUTPUT_GOOD_COUNT, 2);
                                Machine.info.JigInc(Machine.Jigs[(int)UNITJIG.JIG_OUT_BUF].from_id, StatusDefine.PRODUCT.PRODUCT_OUTPUT_GOOD_COUNT, 2);
                            }

                            Machine.Jigs[CURRENT].exist = false;
                            Machine.Jigs[CURRENT].productCount = 0;
                            NextStep();

                            resumeStepIndex = StepIndex;
                            resumeProc = false;
                        }
                    }
                    break;
                #endregion
            }
        }

        private void OnProcessOfMotion()
        {
            switch ((STEP)Step)
            {
                case STEP.INIT_STOPPER_UP:
                    if (Machine.GetJigSignal(JIG_TYPE.TOP_OUT, JIG_SENSOR.OUT)
                        && !Machine.GetJigSignal(JIG_TYPE.TOP_OUT, JIG_SENSOR.IN)) NextStep();
                    else Step = STEP.STOPPER_UP;
                    break;

                case STEP.CONV_RUN:
                    resumeStepIndex = StepIndex;

                    Machine.IO.SetOut((int)DO.JIG_OUT_BUF_CONV_RUN, 1);
                    NextStep();
                    timeWait[(int)TIMER.DELAY].Start();
                    break;

                case STEP.CONV_STOP:
                    Machine.IO.SetOut((int)DO.JIG_OUT_BUF_CONV_RUN, 0);
                    NextStep();
                    timeWait[(int)TIMER.DELAY].Start();
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

                    Machine.IO.SetOut((int)DO.JIG_OUT_BUF_STOPPER_UP, 1);
                    Machine.IO.SetOut((int)DO.JIG_OUT_BUF_STOPPER_DOWN, 0);

                    Step = STEP.STOPPER_UP_CHECK;
                    timeWait[(int)TIMER.DELAY].Start();
                    timeWait[(int)TIMER.TIMEOUT].Start();
                    break;

                case STEP.STOPPER_UP_CHECK:
                    if (timeWait[(int)TIMER.TIMEOUT].Elapsed > Machine.param.Time(ParameterDefine.TIME.CYLINDER_TIME_OUT))
                    {
                        SetError(ECODE.TIMEOUT_JIG_OUTBUF_STOPPER_UP);
                        break;
                    }

                    Machine.IO.GetIn((int)DI.JIG_OUT_STOPPER_UP, ref ret1);
                    Machine.IO.GetIn((int)DI.JIG_OUT_STOPPER_DOWN, ref ret2);

                    if (ret1 == 1 && ret2 == 0)
                        NextStep();
                    break;

                case STEP.STOPPER_DOWN:
                    resumeStepIndex = StepIndex;

                    Machine.IO.SetOut((int)DO.JIG_OUT_BUF_STOPPER_UP, 0);
                    Machine.IO.SetOut((int)DO.JIG_OUT_BUF_STOPPER_DOWN, 1);

                    Step = STEP.STOPPER_DOWN_CHECK;
                    timeWait[(int)TIMER.TIMEOUT].Start();
                    timeWait[(int)TIMER.DELAY].Start();
                    break;

                case STEP.STOPPER_DOWN_CHECK:
                    if (timeWait[(int)TIMER.TIMEOUT].Elapsed > Machine.param.Time(ParameterDefine.TIME.CYLINDER_TIME_OUT))
                    {
                        SetError(ECODE.TIMEOUT_JIG_OUTBUF_STOPPER_DOWN);
                        break;
                    }

                    Machine.IO.GetIn((int)DI.JIG_OUT_STOPPER_UP, ref ret1);
                    Machine.IO.GetIn((int)DI.JIG_OUT_STOPPER_DOWN, ref ret2);

                    if (ret1 == 0 && ret2 == 1)
                        NextStep();
                    break;
            }
        }

        public override void SetHeadTarget(int iTarget)
        {
            throw new NotImplementedException();
        }

        private CONVSTATUS ProductExistCheck()
        {
            uint existIn = 0;
            uint existOut = 0;
            uint stopperDown = 0;

            Machine.IO.GetIn((int)DI.JIG_OUT_BUF_DETECT_IN, ref existIn);
            Machine.IO.GetIn((int)DI.JIG_OUT_BUF_DETECT_OUT, ref existOut);
            Machine.IO.GetIn((int)DI.JIG_OUT_STOPPER_DOWN, ref stopperDown);

            if (existIn == 1 && existOut == 1) return CONVSTATUS.EXIST;
            else if (existIn == 0 && existOut == 0 && stopperDown == 0) return CONVSTATUS.EMPTY;
            else if (stopperDown == 1) return CONVSTATUS.UNLOADING;
            else if (stopperDown == 0) return CONVSTATUS.LOADING;
            return CONVSTATUS.ERROR;
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
    }
}
