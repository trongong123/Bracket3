using TopEng.Device;
using TopEng.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CAMASSEMBLYMACHINE;
using static CAMASSEMBLYMACHINE.Define.SystemDefine;
using CAMASSEMBLYMACHINE.Define;

namespace TopEng.Module
{
    public class TOWERLAMP
    {
        public enum TOWERLAMPTYPE
        {
            NOCOLOR = -1,
            RYG_3COLORS,
        }
        public enum COLORELEM
        {
            RED,
            YELLOW,
            GREEN,

            COLORELEMMAX,
        }

        private int[] address = new int[(int)COLORELEM.COLORELEMMAX];
        private double blink_time = 0;
        private IIO control = null;
        private TOWERLAMPTYPE type;

        public TOWERLAMP(IIO IO, int red = -1, int yellow = -1, int green = -1, TOWERLAMPTYPE colortype = TOWERLAMPTYPE.RYG_3COLORS, double blink_time_ms = 1000.0)
        {
            control = IO;

            address[(int)COLORELEM.RED] = red;
            address[(int)COLORELEM.YELLOW] = yellow;
            address[(int)COLORELEM.GREEN] = green;

            type = colortype;
            blink_time = blink_time_ms;
        }

        public void Ready()
        {
            // YELLOW : ON
            uint[] readySet = { 0, 1, 0 };

            for (int i = 0; i < address.Length; i++)
            {
                if (address[i] < 0)
                    continue;
                control.SetOut(address[i], readySet[i]);
            }
        }

        public void Run()
        {
            // GREEN : ON
            uint[] runSet = { 0, 0, 1 };

            for (int i = 0; i < address.Length; i++)
            {
                if (address[i] < 0)
                    continue;
                control.SetOut(address[i], runSet[i]);
            }
        }

        public void RunEmpty()
        {
            // GREEN : ON
            uint[] runSet = { 0, 0, 1 };

            for (int i = 0; i < address.Length; i++)
            {
                if (address[i] < 0)
                    continue;
                control.SetOut(address[i], runSet[i]);
            }
        }

        public void Sleep()
        {
            // GREEN : ON
            uint[] runSet = { 0, 1, 0 };

            for (int i = 0; i < address.Length; i++)
            {
                if (address[i] < 0)
                    continue;
                control.SetOut(address[i], runSet[i]);
            }
        }

        public void Alarm()
        {
            // RED : ON
            uint[] alrmSet = { 1, 0, 0 };

            for (int i = 0; i < address.Length; i++)
            {
                if (address[i] < 0)
                    continue;
                control.SetOut(address[i], alrmSet[i]);
            }
        }

        public void Off()
        {
            // RED : ON
            uint[] alrmSet = { 0, 0, 0 };

            for (int i = 0; i < address.Length; i++)
            {
                if (address[i] < 0)
                    continue;
                control.SetOut(address[i], alrmSet[i]);
            }
        }

        public void CheckEmptyTrayInBuf()
        {
            uint[] runSet = { 0, 0, 1 };

            uint ret1 = 0;
            uint ret2 = 0;
            uint ret3 = 0;
            uint ret4 = 0;
            uint ret5 = 0;
            uint ret6 = 0;

            Machine.IO.GetIn((int)DI.TRAY_IN_BUF_DETECT_IN, ref ret1);
            Machine.IO.GetIn((int)DI.TRAY_IN_BUF_DETECT_MID, ref ret2);
            Machine.IO.GetIn((int)DI.TRAY_IN_BUF_DETECT_END, ref ret3);

            Machine.IO.GetIn((int)DI.TRAY_IN_BUF_DETECT2_IN, ref ret4);
            Machine.IO.GetIn((int)DI.TRAY_IN_BUF_DETECT2_MID, ref ret5);
            Machine.IO.GetIn((int)DI.TRAY_IN_BUF_DETECT2_END, ref ret6);

            //if (ret1 == 0 && ret2 == 0 && ret3 == 0 && ret4 == 0 && ret5 == 0 && ret6 == 0 &&
            //    Machine.status.mode == SystemMode.SystemModeAUTO)
            //{
            //    runSet[1] = 1;
            //}
            for (int i = 0; i < address.Length; i++)
            {
                if (address[i] < 0)
                    continue;
                control.SetOut(address[i], runSet[i]);
            }
        }
    }

    public class BUZZER
    {
        public enum BUZZERELEM
        {
            NOSOUND = -1,
            ALARM,
            START,
            OPCALL,
            HOME,

            BUZZERELEMMAX,
        }

        private int[] address = new int[(int)BUZZERELEM.BUZZERELEMMAX];
        private IIO control = null;

        public BUZZER(IIO IO, int alarm = -1, int start = -1, int opcall = -1, int home = -1)
        {
            control = IO;

            address[(int)BUZZERELEM.ALARM] = alarm;
            address[(int)BUZZERELEM.START] = start;
            address[(int)BUZZERELEM.OPCALL] = opcall;
            address[(int)BUZZERELEM.HOME] = home;
        }

        public void Error()
        {
            control.SetOut(address[(int)BUZZERELEM.ALARM], 1);
        }

        public void Ready()
        {
            control.SetOut(address[(int)BUZZERELEM.ALARM], 0);
        }

        public bool IsError()
        {
            uint ret = 0;
            control.GetOut(address[(int)BUZZERELEM.ALARM], ref ret);
            return ret == 1 ? true : false;
        }
    }

    public class OPPANEL
    {
        public enum OPINPUTELEM
        {
            NOINPUT = -1,
            START,
            STOP,
            RESET,
            POWERON,
            EMERGENCY,
            MUTING,
            TRAYOUT_IN,
            TRAYOUT_OUT,
            OPINPUTELEMMAX,
        }

        public enum OPOUTPUTELEM
        {
            NOOUTPUT = -1,
            START,
            STOP,
            RESET,
            MUTING,
            TRAYOUT_IN,
            TRAYOUT_OUT,
            OPOUTPUTELEMMAX,
        }

        private int[] input = new int[(int)OPINPUTELEM.OPINPUTELEMMAX];
        private int[] output = new int[(int)OPOUTPUTELEM.OPOUTPUTELEMMAX];
        private IIO control = null;

        public int mutingTime = 10 * 1000;
        public bool mutingOn = true;
        public bool mutingOnDisregard = false;

        StopWatch timeWatcher = new StopWatch();
        StopWatch mutingWatcher = new StopWatch();

        bool startButtonDown = false;
        bool startEventOn = false;
        bool stopButtonDown = false;
        bool stopEventOn = false;
        bool resetButtonDown = false;
        bool resetEventOn = false;
        bool mutingButtonDown = false;
        bool mutingEventOn = false;
        bool[] trayoutButtonDown = { false, false };
        bool[] trayoutEventOn = { false, false };

        public delegate void cbStartButtonEvent();
        public event cbStartButtonEvent StartButtonClicked;
        public delegate void cbStopButtonEvent();
        public event cbStopButtonEvent StopButtonClicked;
        public delegate void cbResetButtonEvent();
        public event cbResetButtonEvent ResetButtonClicked;
        public delegate void cbMutingButtonEvent();
        public event cbMutingButtonEvent MutingButtonClicked;
        public delegate void cbMutingOffEvent();
        public event cbMutingOffEvent MutingOff;
        public delegate void cbTrayoutButtonEvent(int id);
        public event cbTrayoutButtonEvent TrayoutButtonClicked;

        public OPPANEL(IIO IO)
        {
            control = IO;
        }

        public void SetInIO(int start = -1, int stop = -1, int reset = -1)
        {
            input[(int)OPINPUTELEM.START] = start;
            input[(int)OPINPUTELEM.STOP] = stop;
            input[(int)OPINPUTELEM.RESET] = reset;        
        }

        public void SetOutIO(int start = -1, int stop = -1, int reset = -1)
        {
            output[(int)OPOUTPUTELEM.START] = start;
            output[(int)OPOUTPUTELEM.STOP] = stop;
            output[(int)OPOUTPUTELEM.RESET] = reset;
        }

        public void OnStartButtonClicked()
        {
            uint ret = 0;
            control.GetIn(input[(int)OPINPUTELEM.START], ref ret);
            if (ret == 0)
            {
                startButtonDown = false;
                startEventOn = false;
                return;
            }
            if (ret == 1 && !startButtonDown && !startEventOn)
            {
                timeWatcher.Start();
                startButtonDown = true;
            }
            if (startButtonDown && !startEventOn && timeWatcher.Elapsed > 50)
            {
                StartButtonClicked();
                startEventOn = true;
            }
        }

        public void OnStopButtonClicked()
        {
            uint ret = 0;
            control.GetIn(input[(int)OPINPUTELEM.STOP], ref ret);
            if (ret == 0)
            {
                stopButtonDown = false;
                stopEventOn = false;
                return;
            }
            if (ret == 1 && !stopButtonDown && !stopEventOn)
            {
                timeWatcher.Start();
                stopButtonDown = true;
            }
            if (stopButtonDown && !stopEventOn && timeWatcher.Elapsed > 50)
            {
                StopButtonClicked();
                stopEventOn = true;
            }
        }

        public void OnResetButtonClicked()
        {
            uint ret = 0;
            control.GetIn(input[(int)OPINPUTELEM.RESET], ref ret);
            if (ret == 0)
            {
                resetButtonDown = false;
                resetEventOn = false;
                return;
            }
            if (ret == 1 && !resetButtonDown && !resetEventOn)
            {
                timeWatcher.Start();
                resetButtonDown = true;
            }
            if (resetButtonDown && !resetEventOn && timeWatcher.Elapsed > 50)
            {
                ResetButtonClicked();
                resetEventOn = true;
            }
        }

        public void OnMutingButtonClicked()
        {
            if (!mutingOnDisregard)
                return;

            bool MutingOn = true;

            if (mutingWatcher.Elapsed > mutingTime)
            {
                MutingOn = false;
            }

            if (mutingOn != MutingOn)
            {
                Mute(MutingOn);
                mutingOn = MutingOn;

                if (MutingOff != null && !MutingOn)
                    MutingOff();
            }

            uint ret = 0;
            control.GetIn(input[(int)OPINPUTELEM.MUTING], ref ret);
            if (ret == 0)
            {
                mutingButtonDown = false;
                mutingEventOn = false;
                return;
            }
            if (ret == 1 && !mutingButtonDown && !mutingEventOn)
            {
                timeWatcher.Start();
                mutingButtonDown = true;
            }
            if (mutingButtonDown && !mutingEventOn && timeWatcher.Elapsed > 50)
            {
                MutingButtonClicked();
                mutingEventOn = true;
                mutingWatcher.Reset();
                mutingOn = true;
                Mute(true);
            }
        }

        public void OnTrayoutButtonClicked()
        {
            uint ret = 0;
            int[] intputIndex = { (int)OPINPUTELEM.TRAYOUT_IN , (int)OPINPUTELEM.TRAYOUT_OUT };

            for (int i = 0; i < 2; i++)
            {
                control.GetIn(input[intputIndex[i]], ref ret);
                if (ret == 0)
                {
                    trayoutButtonDown[i] = false;
                    trayoutEventOn[i] = false;
                    continue;
                }
                if (ret == 1 && !trayoutButtonDown[i] && !trayoutEventOn[i])
                {
                    timeWatcher.Start();
                    trayoutButtonDown[i] = true;
                }
                if (trayoutButtonDown[i] && !trayoutEventOn[i] && timeWatcher.Elapsed > 50)
                {
                    TrayoutButtonClicked(i);
                    trayoutEventOn[i] = true;
                }
            }
        }

        public void Start()
        {
            if (output[(int)OPOUTPUTELEM.START] >= 0)
                control.SetOut(output[(int)OPOUTPUTELEM.START], 1);
            if (output[(int)OPOUTPUTELEM.STOP] >= 0)
                control.SetOut(output[(int)OPOUTPUTELEM.STOP], 0);
            if (output[(int)OPOUTPUTELEM.RESET] >= 0)
                control.SetOut(output[(int)OPOUTPUTELEM.RESET], 0);
        }

        public void Stop()
        {
            if (output[(int)OPOUTPUTELEM.START] >= 0)
                control.SetOut(output[(int)OPOUTPUTELEM.START], 0);
            if (output[(int)OPOUTPUTELEM.STOP] >= 0)
                control.SetOut(output[(int)OPOUTPUTELEM.STOP], 1);
            if (output[(int)OPOUTPUTELEM.RESET] >= 0)
                control.SetOut(output[(int)OPOUTPUTELEM.RESET], 0);
        }

        public void Reset()
        {
            if (output[(int)OPOUTPUTELEM.START] >= 0)
                control.SetOut(output[(int)OPOUTPUTELEM.START], 0);
            if (output[(int)OPOUTPUTELEM.STOP] >= 0)
                control.SetOut(output[(int)OPOUTPUTELEM.STOP], 1);
            if (output[(int)OPOUTPUTELEM.RESET] >= 0)
                control.SetOut(output[(int)OPOUTPUTELEM.RESET], 1);
        }

        public void Mute(bool On)
        {
            if (!mutingOnDisregard)
                return;

            uint state = On ? (uint)1 : 0;
            if (output[(int)OPOUTPUTELEM.MUTING] >= 0)
                control.SetOut(output[(int)OPOUTPUTELEM.MUTING], state);
        }
    }
}
