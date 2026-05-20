using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopEng.Device;
using TopEng.Module;
using CAMASSEMBLYMACHINE.Define;
using TopEng.Utils;
using static CAMASSEMBLYMACHINE.Define.SystemDefine;

namespace CAMASSEMBLYMACHINE.Process
{
    public class ProcessError : IProcess
    {
        public enum AUTOSTEP
        {
            ERROR,
            IDLE,
            STOP,
            COMPLETE,

            ERROR_QUEUE_EMPTY_CHECK,
            ERROR_REPORT,
            ERROR_REPORT_CHECK,
        }

        public enum STEP
        {
            ERROR = -1,
            IDLE,
            STOP,

            STEP_PROCESS_CYCLE_STOP,
            STEP_PROCESS_CYCLE_STOP_COMPL_CHECK,
            STEP_WRITE_ALARM,
        }

        public enum MSG
        {
            MSG_ALARM,
        }

        public AlarmUtil alarm;
        private object lock_alarm = new object();

        public ProcessError()
        {
            alarm = new AlarmUtil(alarmlistPath, alarmPath);
            AutoStep = AUTOSTEP.ERROR_QUEUE_EMPTY_CHECK;
        }

        #region PROCESS_STEP_VARIABLES
        public List<STEP> StepList = new List<STEP>();
        public STEP Step = STEP.IDLE;
        public int StepIndex = 0;
        public AUTOSTEP AutoStep = AUTOSTEP.IDLE;
        public override Type StepEnumType => typeof(STEP);
        public override Type MsgEnumType => typeof(MSG);

        public override int GetStep() { return (int)Step; }
        public override int GetAutoStep() { return 0; }

        public override bool Busy() { return false; }
        public override bool Ready() { return Step == STEP.IDLE || Step == STEP.STOP; }
        public override bool Stopped() { return false; }
        public override bool Error() { return false; }
        public override void TimeOut() { }
        public override void AutoStart() { }
        public override void EStop(bool pause) { }
        public override void Stop() { }
        public STEP NextStep()
        {
            if (++StepIndex < StepList.Count())
                Step = StepList[StepIndex];
            else
                Step = STEP.IDLE;
            return Step;
        }
        #endregion

        public ECODE last_error_code = ECODE.SYSTEM_OK;
        private Dictionary<ECODE, bool> dic_error = new Dictionary<ECODE, bool>();

        public void Alarm(ECODE err_code)
        {
            lock (lock_alarm)
            {
                // 안전 관련 알람은 즉시 정지한다. (딜레이 없도록)
                if (alarm.GetCategory((int)err_code) == ALARMINFO.CATEGORY.SAFE/* ||
                    alarm.GetCategory((int)err_code) == ALARMINFO.CATEGORY.SYSTEM*/)
                {
                    Machine.status.state = SystemState.SystemStateERROR;
                    LogUtil.Instance.Log(LOG_TYPE.DEVELOPMENT, "BYPASS_OFF", CONTENT_TYPE.DEBUG);
                    Machine.IO.SetOut((int)DO.BYPASS_ON, 0);
                }
                else
                {
                    LogUtil.Instance.Log(LOG_TYPE.DEVELOPMENT, "BYPASS_ON", CONTENT_TYPE.DEBUG);
                    Machine.IO.SetOut((int)DO.BYPASS_ON, 1);
                }

                Machine.EStop(false, true, false);

                // 중복 코드가 들어오면 패스 한다.
                if (err_code == last_error_code/* || alarm.Find((int)err_code)*/)
                    return;

                bool writeLog = false;
                if (Machine.status.state == SystemState.SystemStateRUN)
                {
                    writeLog = true;
                }

                last_error_code = err_code;

                if (!dic_error.ContainsKey(err_code))
                {
                    dic_error.Clear();
                    dic_error.Add(err_code, writeLog);
                }
            }
        }

        public void AlarmClear()
        {
            lock (lock_alarm)
            {
                alarm.Clear();
                dic_error.Clear();
                last_error_code = 0;
                Machine.status.state = SystemState.SystemStateIDLE;
            }
        }

        public void AlarmRemove(ECODE err_code)
        {
            alarm.Remove((int)err_code);
        }

        public override void SetMessage(int message, int step = 0)
        {
            switch ((MSG)message)
            {
                case MSG.MSG_ALARM:
                    var processAlarm = new List<STEP>()
                    {
                        STEP.STEP_PROCESS_CYCLE_STOP,
                        STEP.STEP_PROCESS_CYCLE_STOP_COMPL_CHECK,
                        STEP.STEP_WRITE_ALARM,
                        STEP.IDLE,
                    };

                    StepList = processAlarm;
                    Step = StepList[StepIndex = 0];
                    break;
            }
        }

        protected override void OnProcessing()
        {
            bool writeLog = true;

            switch ((STEP)Step)
            {
                case STEP.ERROR:
                    AutoStep = AUTOSTEP.ERROR;
                    timeWait[(int)TIMER.TIMEOUT].Reset();
                    break;

                case STEP.IDLE:
                    timeWait[(int)TIMER.TIMEOUT].Reset();
                    break;

                case STEP.STOP:
                    timeWait[(int)TIMER.TIMEOUT].Reset();
                    break;

                //case STEP.STEP_PROCESS_CYCLE_STOP:
                //    // 알람 발생 순간 부터 '정지'를 실행하고 진입한다.
                //    if (alarm.GetCategory((int)last_error_code) == ALARMINFO.CATEGORY.SAFE)
                //    {
                //        Machine.EStop(false);
                //    }
                //    else
                //    {
                //        Machine.Stop();
                //    }
                //    Step = STEP.STEP_PROCESS_CYCLE_STOP_COMPL_CHECK;
                //    break;

                //case STEP.STEP_PROCESS_CYCLE_STOP_COMPL_CHECK:
                //    if (Machine.Busy())
                //        break;
                //    NextStep();
                //    break;

                //case STEP.STEP_WRITE_ALARM:
                //    if (Machine.sysMode != Machine.SYSMODE.AUTO)
                //        writeLog = false;

                //    alarm.Add((int)last_error_code, writeLog);
                //    if (Machine.sysMode == Machine.SYSMODE.AUTO)
                //        Machine.status.state = SystemState.SystemStateERROR;

                //    NextStep();
                //   break;
            }

            OnProcessOfAutoRun();
            OnProcessOfError();
        }

        private void OnProcessOfAutoRun()
        {
            switch((AUTOSTEP)AutoStep)
            {
                case AUTOSTEP.ERROR_QUEUE_EMPTY_CHECK:
                    if (dic_error.Count > 0)
                        AutoStep = AUTOSTEP.ERROR_REPORT;
                    break;

                case AUTOSTEP.ERROR_REPORT:
                    SetMessage((int)MSG.MSG_ALARM);

                    AutoStep = AUTOSTEP.ERROR_REPORT_CHECK;
                    break;

                case AUTOSTEP.ERROR_REPORT_CHECK:
                    if (!Ready() && !Error())
                        break;

                    AutoStep = AUTOSTEP.ERROR_QUEUE_EMPTY_CHECK;
                    break;
            }
        }

        private void OnProcessOfError()
        {
            bool writeLog = true;

            switch ((STEP)Step)
            {
                case STEP.STEP_PROCESS_CYCLE_STOP:
                    if (dic_error.Count == 0)
                    {
                        Step = STEP.IDLE;
                        break;
                    }

                    // 알람 발생 순간 부터 '정지'를 실행하고 진입한다.
                    if (alarm.GetCategory((int)dic_error.First().Key) == ALARMINFO.CATEGORY.SAFE)
                    {
                        Machine.EStop(false, true, false);
                    }
                    else
                    {
                        Machine.Stop(false);
                    }
                    Step = STEP.STEP_PROCESS_CYCLE_STOP_COMPL_CHECK;
                    break;

                case STEP.STEP_PROCESS_CYCLE_STOP_COMPL_CHECK:
                    foreach (var proc in Machine.proclist)
                    {
                        if (!proc.Ready() && !proc.Error())
                            return;
                    }

                    //if (Machine.Busy())
                    //    break;
                    NextStep();
                    break;

                case STEP.STEP_WRITE_ALARM:
                    if (Machine.sysMode != Machine.SYSMODE.AUTO)
                        writeLog = false;

                    if (dic_error.Count > 0)
                        dic_error.Remove(dic_error.First().Key);
                    alarm.Add((int)last_error_code, writeLog);
                    //if (Machine.sysMode == Machine.SYSMODE.AUTO)
                        Machine.status.state = SystemState.SystemStateERROR;

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
