using static CAMASSEMBLYMACHINE.Define.SystemDefine;
using TopEng.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace CAMASSEMBLYMACHINE.Process
{
    public struct ProcMsg
    {
        public int msg;

        public ProcMsg(int message = -1)
        {
            msg = message;
        }
    }

    public abstract partial class IProcess
    {
        protected Thread threadProc;
        protected ManualResetEvent threadEvent;
        protected List<StopWatch> timeWait = new List<StopWatch>();
        protected int timeLimit = 30; // millisecond
        protected bool stopBit = false;
        protected bool EStopBit = false;
        public bool initCompl = false;
        public bool firstProduct = true;
        public bool error_proc = false;

        public delegate void MessageCallbackEvent(string msg);
        public event MessageCallbackEvent m_cbMessageEventFunc;
        public delegate void cbUpdateTactTimeEvent(int unit, int ms);
        public event cbUpdateTactTimeEvent m_cbUpdateTacTime;

        // RECOVERY PARAM
        public int lastAutoStep = 0;
        public int lastProcStep = 0;

        public int resumeStepIndex = 0;
        public int sleepAutoStep = 0;
        public abstract Type StepEnumType { get; }
        public abstract Type MsgEnumType { get; }

        public enum TIMER
        {
            TIMEOUT,
            DELAY,
            TACT,
            STATUS,
            TRIG,
            ALIVE,

            MAX,
        }

        public IProcess()
        {
            for (int i = 0; i < (int)TIMER.MAX; i++)
                timeWait.Add(new StopWatch());

            threadEvent = new ManualResetEvent(true);
            threadProc = new Thread(new ThreadStart(OnThreading));
        }

        public void ShowLog(string msg)
        {
            if (m_cbMessageEventFunc != null)
                m_cbMessageEventFunc(msg);
        }

        public void UpdateTactTime(int unit)
        {
            if (m_cbUpdateTacTime != null)
                m_cbUpdateTacTime(unit, timeWait[(int)TIMER.TACT].Elapsed);
        }

        ~IProcess()
        {
        }

        public abstract int GetStep();
        public abstract int GetAutoStep();

        public abstract bool Busy();
        public abstract bool Ready();
        public abstract bool Stopped();
        public abstract bool Error();
        public abstract void TimeOut();
        public abstract void SetMessage(int message, int step = 0);

        public abstract void SetHeadTarget(int iTarget);
        public abstract void AutoStart();
        public abstract void EStop(bool pause);
        public abstract void Stop();
        protected abstract void OnProcessing();
        protected void OnThreading()
        {
            while (threadEvent.WaitOne())
            {
                OnProcessing();
                Thread.Sleep(20);

                if (timeWait[(int)TIMER.TIMEOUT].Elapsed < timeLimit)
                    continue;

                TimeOut();
            }
        }

        public void Run()
        {
            threadEvent.Set();
            threadProc.Start();
        }

        public enum CONVSTATUS
        {
            EMPTY,
            EXIST,
            LOADING,
            UNLOADING,
            ERROR,
        }
    }
}
