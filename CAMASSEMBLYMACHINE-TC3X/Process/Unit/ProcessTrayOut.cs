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
    public class ProcessTrayOut : IProcess
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

            CONV_RUN,
            CONV_STOP,
            INIT_CONV_RUN,
            INIT_CONV_STOP,

            TRAY_OUT_CONV_UP,
            TRAY_OUT_CONV_UP_CHECK,
            TRAY_OUT_CONV_DOWN,
            TRAY_OUT_CONV_DOWN_CHECK,

            IF_LOADING_POSSIBLE_WAIT,
            IF_LOADING_BUSY,
            IF_LOADING_COMPLETE_CHECK,
            IF_UNLOADING_EXIST_CHECK,
            IF_UNLOADING_COMPLETE_CHECK,
        }

        public enum MSG
        {
            MSG_SINGLE,

            MSG_PROCESS_INIT,
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

            Machine.Trays[(int)UNITTRAY.TRAY_OUT].ClearInterface();
            Machine.IO.SetOut((int)DO.TRAY_OUT_CONV_RUN, 0);
        }

        public override void Stop()
        {
            stopBit = true;

            Machine.Trays[(int)UNITTRAY.TRAY_OUT].ClearInterface();
            Machine.IO.SetOut((int)DO.TRAY_OUT_CONV_RUN, 0);
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

        public ProcessTrayOut()
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
                        STEP.TRAY_OUT_CONV_DOWN,
                        //STEP.INIT_CONV_RUN,
                        STEP.INIT_PRODUCT_DATA_MISMATCH_CHECK,
                        STEP.IDLE,
                    };

                    StepList = processInit;
                    Step = StepList[StepIndex = 0];
                    break;

                case MSG.MSG_LOADING_PRODUCT:
                    Machine.Trays[(int)UNITTRAY.TRAY_OUT].ClearInterface();

                    var processLoading = new List<STEP>()
                    {
                        STEP.TRAY_OUT_CONV_DOWN,
                        STEP.CONV_STOP,
                        STEP.IF_LOADING_POSSIBLE_WAIT,
                        STEP.CONV_RUN,
                        STEP.IF_LOADING_BUSY,
                        STEP.IF_LOADING_COMPLETE_CHECK,
                        STEP.CONV_STOP,
                        STEP.IDLE,
                    };

                    StepList = processLoading;
                    Step = StepList[StepIndex = 0];
                    break;

                case MSG.MSG_UNLOADING_PRODUCT:
                    Machine.Trays[(int)UNITTRAY.TRAY_OUT].ClearInterface();

                    var processUnloading = new List<STEP>()
                    {
                        STEP.IF_UNLOADING_EXIST_CHECK,
                        //STEP.CONV_RUN,
                        STEP.TRAY_OUT_CONV_UP,
                        STEP.IF_UNLOADING_COMPLETE_CHECK,
                        //STEP.CONV_STOP,
                        STEP.IDLE,
                    };

                    StepList = processUnloading;
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

                case STEP.INIT_PRODUCT_DATA_MISMATCH_CHECK:
                    if (Machine.status.mode != SystemMode.SystemModeDRYRUN)
                    {
                        Machine.Trays[(int)UNITTRAY.TRAY_OUT].exist =
                           Machine.GetTraySensorSignal(TRAY_TYPE.OUT, TRAY_CV_SENSOR.IN)
                           || Machine.GetTraySensorSignal(TRAY_TYPE.OUT, TRAY_CV_SENSOR.MID)
                           || Machine.GetTraySensorSignal(TRAY_TYPE.OUT, TRAY_CV_SENSOR.END);
                    }

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

                    Machine.Trays[(int)UNITTRAY.TRAY_OUT].ClearInterface();

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
                    if (Machine.Trays[(int)UNITTRAY.TRAY_OUT].exist)
                        AutoStep = AUTOSTEP.UNLOADING;
                    else
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

                    AutoStep = AUTOSTEP.UNLOADING;
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
            int upstream = (int)UNITTRAY.TRAY_OUT_BUF;
            int current = (int)UNITTRAY.TRAY_OUT;

            uint ret1 = 0;
            uint ret2 = 0;

            Machine.Trays[(int)UNITTRAY.TRAY_OUT].exist =
                           Machine.GetTraySensorSignal(TRAY_TYPE.OUT, TRAY_CV_SENSOR.IN)
                           || Machine.GetTraySensorSignal(TRAY_TYPE.OUT, TRAY_CV_SENSOR.MID)
                           || Machine.GetTraySensorSignal(TRAY_TYPE.OUT, TRAY_CV_SENSOR.END);


            switch ((STEP)Step)
            {
                case STEP.IF_LOADING_POSSIBLE_WAIT:
                    if (!Machine.Trays[upstream].unloadingRequest)
                        return;

                    NextStep();
                    timeWait[(int)TIMER.TIMEOUT].Start();
                    break;

                case STEP.IF_LOADING_BUSY:
                    if (!Machine.Trays[upstream].unloading)
                        break;
                    Machine.Trays[current].loading = true;
                    NextStep();
                    break;

                case STEP.IF_LOADING_COMPLETE_CHECK:
                    if (timeWait[(int)TIMER.TIMEOUT].Elapsed > Machine.param.Time(ParameterDefine.TIME.CONV_RUN_TIME_OUT))
                    {
                        SetError(ECODE.TIMEOUT_TRAY_OUT_LOADING);
                        break;
                    }

                    if (Machine.status.mode == SystemMode.SystemModeDRYRUN) Util.Delay(2000);
                    else
                    {
                        if (!Machine.GetTraySensorSignal(TRAY_TYPE.OUT, TRAY_CV_SENSOR.END)) break;
                    }

                    if (Machine.Trays[upstream].unloading)
                        break;

                    Machine.Trays[current].loading = false;
                    Machine.Trays[current].exist = true;
                    NextStep();
                    break;

                case STEP.IF_UNLOADING_EXIST_CHECK:
                    if (CheckStopBit())
                        break;
                    if (Machine.status.mode == SystemMode.SystemModeDRYRUN) NextStep();
                    else
                    {
                        if (!Machine.Trays[(int)UNITTRAY.TRAY_OUT].exist) break;
                    }

                    timeWait[(int)TIMER.TIMEOUT].Reset();
                    timeWait[(int)TIMER.DELAY].Reset();

                    NextStep();
                    break;

                case STEP.IF_UNLOADING_COMPLETE_CHECK:
                    if (Machine.status.mode != SystemMode.SystemModeDRYRUN)
                    {
                        if (Machine.Trays[(int)UNITTRAY.TRAY_OUT].exist) break;
                    }
                    else
                    {
                        Util.Delay(2000);
                    }

                    Machine.Trays[current].exist = false;
                    NextStep();
                    break;
            }
        }

        private void OnProcessOfMotion()
        {
            uint ret1 = 0, ret2 = 0, ret3 = 0;
            switch ((STEP)Step)
            {
                case STEP.CONV_RUN:
                    Machine.IO.SetOut((int)DO.TRAY_OUT_CONV_RUN, 1);
                    NextStep();
                    timeWait[(int)TIMER.DELAY].Start();
                    break;

                case STEP.CONV_STOP:
                    Machine.IO.SetOut((int)DO.TRAY_OUT_CONV_RUN, 0);
                    NextStep();
                    timeWait[(int)TIMER.DELAY].Start();
                    break;

                case STEP.INIT_CONV_RUN:
                    Machine.IO.SetOut((int)DO.TRAY_OUT_CONV_RUN, 1);
                    timeWait[(int)TIMER.DELAY].Start();
                    Step = STEP.INIT_CONV_STOP;
                    break;

                case STEP.INIT_CONV_STOP:
                    if (timeWait[(int)TIMER.DELAY].Elapsed > Machine.param.Time(ParameterDefine.TIME.TRAY_CONV_STOP_DELAY_TIME) || ret3 == 1)
                    {
                        Machine.IO.SetOut((int)DO.TRAY_OUT_CONV_RUN, 0);
                        NextStep();
                    }
                    break;
            }
        }

        private void OnProcessOfIO()
        {
            uint ret1 = 0;
            uint ret2 = 0;

            switch ((STEP)Step)
            {
                case STEP.TRAY_OUT_CONV_DOWN:
                    Machine.IO.SetOut((int)DO.TRAY_OUT_CONV_DOWN, 1);
                    Machine.IO.SetOut((int)DO.TRAY_OUT_CONV_UP, 0);

                    Step = STEP.TRAY_OUT_CONV_DOWN_CHECK;
                    timeWait[(int)TIMER.DELAY].Start();
                    timeWait[(int)TIMER.TIMEOUT].Start();
                    break;

                case STEP.TRAY_OUT_CONV_DOWN_CHECK:
                    if (timeWait[(int)TIMER.TIMEOUT].Elapsed > Machine.param.Time(ParameterDefine.TIME.CYLINDER_TIME_OUT))
                    {
                        SetError(ECODE.TIMEOUT_TRAY_OUT_CONV_DOWN);
                        break;
                    }

                    Machine.IO.GetIn((int)DI.TRAY_OUT_CONV_DOWN, ref ret1);
                    Machine.IO.GetIn((int)DI.TRAY_OUT_CONV_UP, ref ret2);

                    if (ret1 == 1 && ret2 == 0)
                        NextStep();
                    break;

                case STEP.TRAY_OUT_CONV_UP:
                    Machine.IO.SetOut((int)DO.TRAY_OUT_CONV_DOWN, 0);
                    Machine.IO.SetOut((int)DO.TRAY_OUT_CONV_UP, 1);

                    Step = STEP.TRAY_OUT_CONV_UP_CHECK;
                    timeWait[(int)TIMER.DELAY].Start();
                    timeWait[(int)TIMER.TIMEOUT].Start();
                    break;

                case STEP.TRAY_OUT_CONV_UP_CHECK:
                    if (timeWait[(int)TIMER.TIMEOUT].Elapsed > Machine.param.Time(ParameterDefine.TIME.CYLINDER_TIME_OUT))
                    {
                        SetError(ECODE.TIMEOUT_TRAY_OUT_CONV_UP);
                        break;
                    }

                    Machine.IO.GetIn((int)DI.TRAY_OUT_CONV_DOWN, ref ret1);
                    Machine.IO.GetIn((int)DI.TRAY_OUT_CONV_UP, ref ret2);

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
