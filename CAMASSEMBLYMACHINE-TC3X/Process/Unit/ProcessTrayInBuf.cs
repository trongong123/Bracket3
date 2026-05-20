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
    public class ProcessTrayInBuf : IProcess
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

            INIT_CONV_RUN,
            INIT_STOPPER_UP,
            INIT_CONV_STOP,

            CONV_RUN,
            CONV_RUN_REVERSE,
            CONV_STOP,

            STOPPER_UP,
            STOPPER_UP_CHECK,
            STOPPER_DOWN,
            STOPPER_DOWN_CHECK,

            IF_LOADING_POSSIBLE_WAIT,
            IF_LOADING_BUSY,
            IF_LOADING_COMPLETE_CHECK,
            IF_REVERSE_UNLOADING_POSSIBLE_WAIT,
            IF_REVERSE_UNLOADING_BUSY,
            IF_REVERSE_UNLOADING_COMPLETE_CHECK,
            IF_UNLOADING_REQUEST,
            IF_UNLOADING_BUSY,
            IF_UNLOADING_COMPLETE_CHECK,
        }

        public enum MSG
        {
            MSG_SINGLE,

            MSG_PROCESS_INIT,
            MSG_LOADING_PRODUCT,
            MSG_REVERSE_UNLOADING_PRODUCT,
            MSG_UNLOADING_PRODUCT,
        }

        #region PROCESS_STEP_VARIABLES
        public List<STEP> StepList = new List<STEP>();
        public STEP Step = STEP.IDLE;
        public int StepIndex = 0;
        public AUTOSTEP AutoStep = AUTOSTEP.IDLE;
        public override Type StepEnumType => typeof(STEP);
        public override Type MsgEnumType => typeof(MSG);

        uint ret1 = 0;
        uint ret2 = 0;
        uint ret3 = 0;

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

            Machine.Trays[(int)UNITTRAY.TRAY_IN_BUF].ClearInterface();
            Machine.IO.SetOut((int)DO.TRAY_IN_BUF_CONV_RUN, 0);
            Machine.IO.SetOut((int)DO.TRAY_IN_BUF_CONV_RUN_REVERSE, 0);
        }

        public override void Stop()
        {
            stopBit = true;

            Machine.Trays[(int)UNITTRAY.TRAY_IN_BUF].ClearInterface();
            Machine.IO.SetOut((int)DO.TRAY_IN_BUF_CONV_RUN, 0);
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

        public ProcessTrayInBuf()
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
                case ECODE.TIMEOUT_TRAY_CONV_RUN:
                case ECODE.MISMATCH_BETWEEN_DATA_AND_PRODUCT_TRAY_INBUF:
                    var processError = new List<STEP>()
                    { 
                        STEP.CONV_STOP,
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
                        STEP.INIT_STOPPER_UP,
                        STEP.INIT_CONV_RUN,
                        STEP.INIT_PRODUCT_DATA_MISMATCH_CHECK,
                        STEP.IDLE,
                    };

                    StepList = processInit;
                    Step = StepList[StepIndex = 0];
                    break;

                case MSG.MSG_LOADING_PRODUCT:
                    Machine.Trays[(int)UNITTRAY.TRAY_IN_BUF].ClearInterface();

                    var ProcessLoadingProduct = new List<STEP>()
                    {
                        STEP.STOPPER_UP,
                        STEP.IF_LOADING_POSSIBLE_WAIT,
                        STEP.CONV_RUN,
                        STEP.IF_LOADING_BUSY,
                        STEP.IF_LOADING_COMPLETE_CHECK,
                        STEP.CONV_STOP_WITH_DELAY,
                        STEP.IDLE,
                    };

                    StepList = ProcessLoadingProduct;
                    Step = StepList[StepIndex = 0];
                    break;

                case MSG.MSG_UNLOADING_PRODUCT:
                    Machine.Trays[(int)UNITTRAY.TRAY_IN_BUF].ClearInterface();

                    var processUnloading = new List<STEP>()
                    {
                        STEP.IF_UNLOADING_REQUEST,
                        STEP.IF_UNLOADING_BUSY,
                        STEP.STOPPER_DOWN,
                        STEP.CONV_RUN,
                        STEP.IF_UNLOADING_COMPLETE_CHECK,
                        STEP.CONV_STOP_WITH_DELAY,
                        STEP.IDLE,
                    };

                    StepList = processUnloading;
                    Step = StepList[StepIndex = 0];
                    break;

                case MSG.MSG_REVERSE_UNLOADING_PRODUCT:
                    Machine.Trays[(int)UNITTRAY.TRAY_IN_BUF].ClearInterface();

                    var processReversing = new List<STEP>()
                    {
                        STEP.STOPPER_DOWN,
                        STEP.IF_REVERSE_UNLOADING_POSSIBLE_WAIT,
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
                        Machine.Trays[(int)UNITTRAY.TRAY_IN_BUF].exist = Machine.GetTraySensorSignal(TRAY_TYPE.IN_BUF, TRAY_CV_SENSOR.END);
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

                case STEP.INIT_STOPPER_UP:
                    double liftPos = 0;
                    Machine.motion.GetAxisActualPos((int)AXIS.TRAY_WORK_Z, ref liftPos);
                    bool isLiftInLoading = Math.Abs(Machine.param.Position(ParameterDefine.POSITION.TRAY_WORK_LOADING_POS) - liftPos / 1000) <= 0.2;

                    if ((!Machine.GetTraySensorSignal(TRAY_TYPE.IN_BUF, TRAY_CV_SENSOR.END)
                        && !Machine.GetTraySensorSignal(TRAY_TYPE.WORK, TRAY_CV_SENSOR.IN))
                        || !isLiftInLoading)
                        Step = STEP.STOPPER_UP;
                    else NextStep();
                    break;

                case STEP.INIT_CONV_RUN:
                    Machine.IO.SetOut((int)DO.TRAY_IN_BUF_CONV_RUN, 1);
                    timeWait[(int)TIMER.DELAY].Start();
                    Step = STEP.INIT_CONV_STOP;
                    break;

                case STEP.INIT_CONV_STOP:
                    if (timeWait[(int)TIMER.DELAY].Elapsed > Machine.param.Time(ParameterDefine.TIME.TRAY_CONV_STOP_DELAY_TIME))
                    {
                        Machine.IO.SetOut((int)DO.TRAY_IN_BUF_CONV_RUN, 0);
                        NextStep();
                    }
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
                        AutoStep = AUTOSTEP.LOADING;

                    CheckStopBit();
                    break;

                case AUTOSTEP.INIT:
                    initCompl = false;
                    stopBit = false;

                    Machine.Trays[(int)UNITTRAY.TRAY_IN_BUF].ClearInterface();

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
                        AutoStep = AUTOSTEP.PROC_SELECT_FOR_RESTART;
                    break;

                case AUTOSTEP.PROC_SELECT_FOR_RESTART:
                    if (CheckStopBit())
                        break;
                    if (Machine.Trays[(int)UNITTRAY.TRAY_IN_BUF].exist)
                        AutoStep = AUTOSTEP.UNLOADING;
                    else
                        AutoStep = AUTOSTEP.LOADING;
                    break;

                case AUTOSTEP.LOADING:
                    if (CheckStopBit() || Machine.TrayStop)
                        break;

                    SetMessage((int)MSG.MSG_LOADING_PRODUCT);
                    AutoStep = AUTOSTEP.LOADING_COMPL_CHECK;
                    break;

                case AUTOSTEP.LOADING_COMPL_CHECK:
                    if (CheckStopBit() || !Ready() || Error())
                        break;

                    //if (Machine.status.mode == Define.SystemMode.SystemModeAUTO)
                    //    LogUtil.Instance.Log(LOG_TYPE.EDM, "LOADING NEW TRAYS", CONTENT_TYPE.EDM, LogUtil.RESERVED_LOG_LOAD_NEW);
                    LogUtil.Instance.Log(LOG_TYPE.SYSTEM, "The TRAYS has been loaded.", CONTENT_TYPE.INFO);

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

                    AutoStep = AUTOSTEP.COMPLETE;
                    break;
            }

            if (AutoStep != AUTOSTEP.IDLE && AutoStep != AUTOSTEP.ERROR && AutoStep != AUTOSTEP.STOP)
                lastAutoStep = (int)AutoStep;
        }

        private void OnProcessOfInterface()
        {
            int current = (int)UNITTRAY.TRAY_IN_BUF;
            int upstream = (int)UNITTRAY.TRAY_IN;
            int downstream = (int)UNITTRAY.TRAY_WORK;

            uint returnExist = 0;

            switch ((STEP)Step)
            {
                case STEP.IF_LOADING_POSSIBLE_WAIT:
                    if (CheckStopBit())
                        break;

                    if (!Machine.Trays[upstream].unloadingRequest)
                        return;

                    timeWait[(int)TIMER.TIMEOUT].Reset();
                    NextStep();
                    break;

                case STEP.IF_LOADING_BUSY:
                    if (CheckStopBit())
                        break;

                    Machine.Trays[current].loading = true;

                    if (Machine.Trays[upstream].unloading)
                        return;
                    NextStep();
                    break;

                case STEP.IF_LOADING_COMPLETE_CHECK:
                    if (timeWait[(int)TIMER.TIMEOUT].Elapsed > Machine.param.Time(ParameterDefine.TIME.CONV_RUN_TIME_OUT))
                    {
                        SetError(ECODE.TIMEOUT_TRAY_INBUF_LOADING);
                        break;
                    }

                    if (Machine.status.mode == SystemMode.SystemModeDRYRUN) Util.Delay(2000);
                    else
                    {
                        if (!Machine.GetTraySensorSignal(TRAY_TYPE.IN_BUF, TRAY_CV_SENSOR.END)) break;
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
                    break;

                case STEP.IF_UNLOADING_BUSY:
                    Machine.Trays[current].unloading = true;

                    if (!Machine.Trays[downstream].loading)
                        break;

                    timeWait[(int)TIMER.TIMEOUT].Reset();
                    NextStep();
                    break;

                case STEP.IF_UNLOADING_COMPLETE_CHECK:
                    if (CheckStopBit())
                        break;

                    if (timeWait[(int)TIMER.TIMEOUT].Elapsed > Machine.param.Time(ParameterDefine.TIME.CONV_RUN_TIME_OUT))
                    {
                        SetError(ECODE.TIMEOUT_TRAY_INBUF_UNLOADING);
                        break;
                    }

                    if (Machine.status.mode == SystemMode.SystemModeDRYRUN) Util.Delay(2000);
                    else
                    {
                        if (Machine.GetTraySensorSignal(TRAY_TYPE.IN_BUF, TRAY_CV_SENSOR.IN)
                                || Machine.GetTraySensorSignal(TRAY_TYPE.IN_BUF, TRAY_CV_SENSOR.MID)
                                || Machine.GetTraySensorSignal(TRAY_TYPE.IN_BUF, TRAY_CV_SENSOR.END)) break;
                    }

                    Machine.Trays[current].unloadingRequest = false;
                    Machine.Trays[current].unloading = false;

                    if (Machine.Trays[downstream].loading)
                        break;

                    Machine.Trays[current].exist = false;
                    NextStep();
                    break;

                case STEP.IF_REVERSE_UNLOADING_POSSIBLE_WAIT:
                    Machine.Trays[current].reverseRequest = true;
                    Machine.Trays[current].reversing = false;

                    if (!Machine.Trays[downstream].reverseRequest)
                        return;

                    if (Machine.Parts[(int)UNITTRAY.TRAY_IN].exist)
                        return;

                    NextStep();
                    break;

                case STEP.IF_REVERSE_UNLOADING_BUSY:
                    Machine.Trays[current].reversing = true;

                    if (!Machine.Trays[downstream].reversing)
                        break;

                    timeWait[(int)TIMER.TIMEOUT].Reset();
                    timeWait[(int)TIMER.DELAY].Reset();
                    NextStep();
                    break;

                case STEP.IF_REVERSE_UNLOADING_COMPLETE_CHECK:
                    bool ret = Machine.GetTraySensorSignal(TRAY_TYPE.IN_BUF, TRAY_CV_SENSOR.IN);
                    if (Machine.status.mode == SystemMode.SystemModeDRYRUN) ret = true;

                    // 자재 감지 신호가 ON 되면 확인한다.
                    if (!ret)
                        timeWait[(int)TIMER.TRIG].Reset();
                    else
                    {
                        // 자재가 있다고 판단되면 데이터를 생성한다.
                        Machine.Trays[current].reversing = false;
                        Machine.Trays[current].reverseRequest = false;
                        Machine.Trays[current].exist = true;
                        NextStep();
                    }
                    break;
            }
        }

        private void OnProcessOfMotion()
        {
            switch ((STEP)Step)
            {
                case STEP.CONV_RUN:
                    Machine.IO.SetOut((int)DO.TRAY_IN_BUF_CONV_RUN, 1);
                    NextStep();
                    timeWait[(int)TIMER.DELAY].Start();
                    break;

                case STEP.CONV_RUN_REVERSE:
                    Machine.IO.SetOut((int)DO.TRAY_IN_BUF_CONV_RUN_REVERSE, 1);
                    NextStep();
                    timeWait[(int)TIMER.DELAY].Start();
                    break;

                case STEP.CONV_STOP:
                    Machine.IO.SetOut((int)DO.TRAY_IN_BUF_CONV_RUN, 0);
                    Machine.IO.SetOut((int)DO.TRAY_IN_BUF_CONV_RUN_REVERSE, 0);
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
                    Machine.IO.SetOut((int)DO.TRAY_IN_BUF_STOPPER_UP, 1);
                    Machine.IO.SetOut((int)DO.TRAY_IN_BUF_STOPPER_DOWN, 0);

                    timeWait[(int)TIMER.TIMEOUT].Start();
                    Step = STEP.STOPPER_UP_CHECK;
                    break;

                case STEP.STOPPER_UP_CHECK:
                    if (timeWait[(int)TIMER.TIMEOUT].Elapsed > Machine.param.Time(ParameterDefine.TIME.CYLINDER_TIME_OUT))
                    {
                        SetError(ECODE.TIMEOUT_TRAY_INBUF_STOPPER_UP);
                        break;
                    }

                    Machine.IO.GetIn((int)DI.TRAY_IN_BUF_STOPPER_UP, ref ret1);
                    Machine.IO.GetIn((int)DI.TRAY_IN_BUF_STOPPER_DOWN, ref ret2);

                    if (ret1 == 1 && ret2 == 0)
                        NextStep();
                    break;

                case STEP.STOPPER_DOWN:
                    Machine.IO.SetOut((int)DO.TRAY_IN_BUF_STOPPER_UP, 0);
                    Machine.IO.SetOut((int)DO.TRAY_IN_BUF_STOPPER_DOWN, 1);

                    timeWait[(int)TIMER.TIMEOUT].Start();
                    Step = STEP.STOPPER_DOWN_CHECK;
                    break;

                case STEP.STOPPER_DOWN_CHECK:
                    if (timeWait[(int)TIMER.TIMEOUT].Elapsed > Machine.param.Time(ParameterDefine.TIME.CYLINDER_TIME_OUT))
                    {
                        SetError(ECODE.TIMEOUT_TRAY_INBUF_STOPPER_DOWN);
                        break;
                    }

                    Machine.IO.GetIn((int)DI.TRAY_IN_BUF_STOPPER_UP, ref ret1);
                    Machine.IO.GetIn((int)DI.TRAY_IN_BUF_STOPPER_DOWN, ref ret2);

                    if (ret1 == 0 && ret2 == 1)
                        NextStep();
                    break;
            }
        }

        

        public override void SetHeadTarget(int iTarget)
        {
            throw new NotImplementedException();
        }
    }
}
