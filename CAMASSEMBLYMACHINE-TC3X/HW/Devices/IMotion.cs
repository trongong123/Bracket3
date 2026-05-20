using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TopEng.Device
{
    public enum MotionState
    {
        MotionStateIDLE,
        MotionStateMOVING,
        MotionStateERROR,
    }

    public enum HomeState
    {
        HomeStateDONE,
        HomeStateDOING,
        HomeStateNEED,
    }

    public class AXISCONFIG
    {
        public int axisNo = 0;

        public class HOMECONFIG
        {
            public int direction { get; set; } = 0;
            public uint signal { get; set; } = 0;
            public uint zphaseUse { get; set; } = 0;
            public double clearTime { get; set; } = 0;
            public double offset { get; set; } = 0;
            public double firstVel { get; set; } = 0;
            public double secondVel { get; set; } = 0;
            public double thirdVel { get; set; } = 0;
            public double lastVel { get; set; } = 0;
            public double firstAcc { get; set; } = 0;
            public double secondAcc { get; set; } = 0;
            public uint completedTrig { get; set; } = 0;
            public double readyPos { get; set; } = 0;
        }

        public HOMECONFIG homeConfig = new HOMECONFIG();

        public class MOTORCONFIG
        {
            public string name { get; set; } = "unnamed";
            public uint pusleType { get; set; } = 4;
            public uint encodeType { get; set; } = 0;
            public uint profileMode { get; set; } = 3;
            public int pulse { get; set; } = 1;
            public double unit { get; set; } = 1;
            public double minVel { get; set; } = 1;
            public double maxVel { get; set; } = 1000000;
            public uint accUnit { get; set; } = 0;
            public double accJerk { get; set; } = 80;
            public double decJerk { get; set; } = 80;
            public uint servoTrig { get; set; } = 1;
            public uint alarmTrig { get; set; } = 1;
            public uint inposTrig { get; set; } = 1;
            public uint inposRange { get; set; } = 50;
            public uint zphaseTrig { get; set; } = 1;
            public uint poslimitTrig { get; set; } = 0;
            public uint negativeTrig { get; set; } = 0;
            public uint homeLevel { get; set; } = 0;
            public double sw_poslimit_puls { get; set; } = 9999999999999;
            public double sw_neglimit_puls { get; set; } = -9999999999999;
        }

        public MOTORCONFIG motorConfig = new MOTORCONFIG();
    }

    public class AXISDATA
    {
        public double commandpos = 0;
        public uint inposRange = 50;
        public double readyPos = 0;
        public HomeState homeState = HomeState.HomeStateNEED;
    }

    public abstract partial class IMotion
    {
        protected bool openned = false;
        public object lockControl = new object();

        public AXISDATA[] axisData = null;

        public IMotion()
        {
        }

        public bool IsOpenned => openned;

        public abstract void Open();
        public abstract void Close();
        public abstract void SetAxisHomeConfig(int axis, AXISCONFIG.HOMECONFIG config);
        public abstract void ReadAxisHomeConfig(string filepath);
        public abstract void SetAxisMotorConfig(int axis, AXISCONFIG.MOTORCONFIG config);
        public abstract void ReadAxisMotorConfig(string filepath);
        public abstract void SetAxisServoOn(int axis, uint enable);
        public abstract void SetAxisHomeStart(int axis);
        public abstract void GetAxisHomeState(int axis, ref HomeState state);
        public abstract void GetAxisHomeComplete(int axis, ref HomeState state);
        public abstract void SetAxisHomeComplete(int axis, HomeState state);
        public abstract void SetAxisHomeReadyPos(int axis);
        public abstract void MoveAxisAbs(int axis, double pos, double vel, double acc, double dec);
        public abstract void MoveAxisInc(int axis, double pos, double vel, double acc, double dec);
        public abstract void MoveAxisJog(int axis, double vel, double acc, double dec);
        public abstract bool MoveAxisDoneCheck(int axis);
        public abstract void GetAxisMotionState(int axis, ref MotionState state);
        public abstract void AxisStop(int axis, bool estop, double decel = 1000000);
        public abstract void AxisStopAll(bool estop, double decel = 1000000);
        public abstract void GetAlarm(int axis, ref bool alaramOn);
        public abstract void AlarmClear(int axis);
        public abstract void GetAxisCommandPos(int axis, ref double pos);
        public abstract void SetAxisCommandPos(int axis, double pos);
        public abstract void GetAxisActualPos(int axis, ref double pos);
        public abstract void SetAxisActualPos(int axis, double pos);
        public abstract void GetAxisInpos(int axis, ref bool inposOn);
        public abstract void SetAxisInposRange(int axis, double range);
        public abstract void GetAxisHW_NEGLimit(int axis, ref bool limitOn);
        public abstract void GetAxisHW_POSLimit(int axis, ref bool limitOn);
        public abstract void GetAxisSW_NEGLimit(int axis, ref bool limitOn);
        public abstract void GetAxis_POSLimit(int axis, ref bool limitOn);
        public abstract void GetAxis_NEGLimit(int axis, ref bool limitOn);
        public abstract void GetAxisSW_POSLimit(int axis, ref bool limitOn);
        public abstract void GetAxisHomeSensor(int axis, ref bool SignalOn);
        public abstract void SetAxisHomeSensorLevel(int axis, uint level);
        public abstract void GetAxisServoOn(int axis, ref bool enable);
    }
}
