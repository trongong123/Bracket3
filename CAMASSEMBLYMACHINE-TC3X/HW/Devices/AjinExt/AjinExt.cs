using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static TopEng.Device.AXISCONFIG;
using System.IO;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using CAMASSEMBLYMACHINE.Define;
using static CAMASSEMBLYMACHINE.Define.SystemDefine;
using CAMASSEMBLYMACHINE;

namespace TopEng.Device.AjinExt
{
    public class AjinExt : IMotion
    {
        public AjinExt()
        {

        }

        public override void Open()
        {
            if (SystemDefine.IS_NOTEBOOK_MODE) return;
            try
            {
                if (CAXL.AxlIsOpened() != 1)
                {
                    if (CAXL.AxlOpen(7) != (uint)AXT_FUNC_RESULT.AXT_RT_SUCCESS)
                        throw new Exception("Failed to read library of AjinEXT.");

                    int axisCount = 0;
                    CAXM.AxmInfoGetAxisCount(ref axisCount);
                    axisData = new AXISDATA[axisCount];
                    for (int i = 0; i < axisData.Length; i++)
                        axisData[i] = new AXISDATA();

                    openned = true;
                }
                else
                    openned = true;
            }
            catch
            {
                throw new Exception("Failed to read library of AjinEXT.");
            }
        }

        public override void Close()
        {
            if (SystemDefine.IS_NOTEBOOK_MODE) return;
            CAXL.AxlClose();
            openned = false;
        }

        public bool IsconnectAllSlave()
        {
            if (SystemDefine.IS_NOTEBOOK_MODE) return false;
            // NETWORK_TYPE_ECAT(5)  :
            return CAXL.AxlIsConnectedAllSlaves(5) == 1 ? true : false;
        }

        public override void SetAxisHomeConfig(int axis, HOMECONFIG config)
        {
            if (SystemDefine.IS_NOTEBOOK_MODE) return;
            lock (lockControl)
            {
                CAXM.AxmHomeSetMethod(axis, config.direction, config.signal, config.zphaseUse, config.clearTime, config.offset);
                CAXM.AxmHomeSetVel(axis, config.firstVel, config.secondVel, config.thirdVel, config.lastVel, config.firstAcc, config.secondAcc);
                axisData[axis].readyPos = config.readyPos;
            }
        }

        public override void ReadAxisHomeConfig(string filepath)
        {
            if (SystemDefine.IS_NOTEBOOK_MODE) return;
            List<HOMECONFIG> config = new List<HOMECONFIG>();

            try
            {
                if (File.Exists(filepath))
                {
                    string streamJson = File.ReadAllText(filepath);
                    streamJson = Regex.Unescape(streamJson);
                    config = JsonSerializer.Deserialize<List<HOMECONFIG>>(streamJson);

                    int maxCount = config.Count > axisData.Length ? axisData.Length : config.Count;
                    for (int i = 0; i < maxCount; i++)
                    {
                        SetAxisHomeConfig(i, config[i]);
                    }
                }
            }
            catch
            {
                CAMASSEMBLYMACHINE.UI.MainForm.mainForm.AppForceClose($"homeConfig.json Load Error");
            }
        }

        public override void SetAxisMotorConfig(int axis, MOTORCONFIG config)
        {
            if (SystemDefine.IS_NOTEBOOK_MODE) return;
            lock (lockControl)
            {
                //++ 지정 축의 펄스 출력 방식을 설정합니다.
                //uMethod : (0)OneHighLowHigh   - 1펄스 방식, PULSE(Active High), 정방향(DIR=Low)  / 역방향(DIR=High)
                //          (1)OneHighHighLow   - 1펄스 방식, PULSE(Active High), 정방향(DIR=High) / 역방향(DIR=Low)
                //          (2)OneLowLowHigh    - 1펄스 방식, PULSE(Active Low),  정방향(DIR=Low)  / 역방향(DIR=High)
                //          (3)OneLowHighLow    - 1펄스 방식, PULSE(Active Low),  정방향(DIR=High) / 역방향(DIR=Low)
                //          (4)TwoCcwCwHigh     - 2펄스 방식, PULSE(CCW:역방향),  DIR(CW:정방향),  Active High     
                //          (5)TwoCcwCwLow      - 2펄스 방식, PULSE(CCW:역방향),  DIR(CW:정방향),  Active Low     
                //          (6)TwoCwCcwHigh     - 2펄스 방식, PULSE(CW:정방향),   DIR(CCW:역방향), Active High
                //          (7)TwoCwCcwLow      - 2펄스 방식, PULSE(CW:정방향),   DIR(CCW:역방향), Active Low
                //          (8)TwoPhase         - 2상(90' 위상차),  PULSE lead DIR(CW: 정방향), PULSE lag DIR(CCW:역방향)
                //          (9)TwoPhaseReverse  - 2상(90' 위상차),  PULSE lead DIR(CCW: 정방향), PULSE lag DIR(CW:역방향)
                CAXM.AxmMotSetPulseOutMethod(axis, config.pusleType);
                //++ 지정 축의 Encoder 입력 방식을 설정합니다.
                // uMethod : (0)ObverseUpDownMode - 정방향 Up/Down
                //           (1)ObverseSqr1Mode   - 정방향 1체배
                //           (2)ObverseSqr2Mode   - 정방향 2체배
                //           (3)ObverseSqr4Mode   - 정방향 4체배
                //           (4)ReverseUpDownMode - 역방향 Up/Down
                //           (5)ReverseSqr1Mode   - 역방향 1체배
                //           (6)ReverseSqr2Mode   - 역방향 2체배
                //           (7)ReverseSqr4Mode   - 역방향 4체배
                CAXM.AxmMotSetEncInputMethod(axis, config.encodeType);
                // uProfileMode : (0)SYM_TRAPEZOID_MODE  - Symmetric Trapezoid
                //                (1)ASYM_TRAPEZOID_MODE - Asymmetric Trapezoid
                //                (2)QUASI_S_CURVE_MODE  - Symmetric Quasi-S Curve
                //                (3)SYM_S_CURVE_MODE    - Symmetric S Curve
                //                (4)ASYM_S_CURVE_MODE   - Asymmetric S Curve
                CAXM.AxmContiSetAbsRelMode(axis, (uint)AXT_MOTION_ABSREL.POS_ABS_MODE);
                //++ 지정 축의 거리/속도/가속도의 제어단위를 설정합니다.
                CAXM.AxmMotSetProfileMode(axis, config.profileMode);
                CAXM.AxmMotSetMinVel(axis, config.minVel);
                CAXM.AxmMotSetMaxVel(axis, config.maxVel);
                CAXM.AxmMotSetAccelUnit(axis, config.accUnit);
                CAXM.AxmMotSetAccelJerk(axis, config.accJerk);
                CAXM.AxmMotSetDecelJerk(axis, config.decJerk);
                CAXM.AxmMotSetMoveUnitPerPulse(axis, config.unit, config.pulse);//
                CAXM.AxmSignalSetServoOnLevel(axis, config.servoTrig);
                CAXM.AxmSignalSetServoAlarm(axis, config.alarmTrig);
                CAXM.AxmSignalSetInpos(axis, config.inposTrig);
                CAXM.AxmHomeSetSignalLevel(axis, config.homeLevel);
                CAXM.AxmSignalSetLimit(axis, (int)AXT_MOTION_STOPMODE.EMERGENCY_STOP, config.poslimitTrig, config.negativeTrig);
                CAXM.AxmSignalSetStop(axis, (int)AXT_MOTION_STOPMODE.EMERGENCY_STOP, (int)AXT_MOTION_LEVEL_MODE.HIGH);
                CAXM.AxmSignalSetZphaseLevel(axis, config.zphaseTrig);

                axisData[axis].inposRange = config.inposRange;
            }
        }

        public override void ReadAxisMotorConfig(string filepath)
        {
            if (SystemDefine.IS_NOTEBOOK_MODE) return;
            List<MOTORCONFIG> config = new List<MOTORCONFIG>();

            try
            {
                if (File.Exists(filepath))
                {
                    string streamJson = File.ReadAllText(filepath);
                    streamJson = Regex.Unescape(streamJson);
                    config = JsonSerializer.Deserialize<List<MOTORCONFIG>>(streamJson);

                    int maxCount = config.Count > axisData.Length ? axisData.Length : config.Count;
                    for (int i = 0; i < maxCount; i++)
                       SetAxisMotorConfig(i, config[i]);
                }
            }
            catch
            {
                CAMASSEMBLYMACHINE.UI.MainForm.mainForm.AppForceClose($"axisConfig.json Load Error");
            }
        }

        public override void SetAxisServoOn(int axis, uint enable)
        {
            if (SystemDefine.IS_NOTEBOOK_MODE) return;
            try
            {
                lock (lockControl)
                {
                    if ((uint)AXT_FUNC_RESULT.AXT_RT_SUCCESS != CAXM.AxmSignalServoOn(axis, enable))
                        throw new Exception($"ServoOn[{axis}] operation failed.");
                }
            }
            catch
            {
            }
        }

        public override void SetAxisHomeStart(int axis)
        {
            if (SystemDefine.IS_NOTEBOOK_MODE) return;
            try
            {
                lock (lockControl)
                {
                    if ((uint)AXT_FUNC_RESULT.AXT_RT_SUCCESS != CAXM.AxmHomeSetStart(axis))
                        throw new Exception($"HomeStart[{axis}] operation failed.");
                    axisData[axis].homeState = HomeState.HomeStateDOING;
                }
            }
            catch
            {
            }
        }

        public override void GetAxisHomeState(int axis, ref HomeState state)
        {
            if (SystemDefine.IS_NOTEBOOK_MODE) return;
            try
            {
                //state = HomeState.HomeStateNEED;

                //uint homeResult = 0;
                //if ((uint)AXT_FUNC_RESULT.AXT_RT_SUCCESS != CAXM.AxmHomeGetResult(axis, ref homeResult))
                //    throw new Exception($"GetHomeState[{axis}] operation failed.");

                //switch ((AXT_MOTION_HOME_RESULT)homeResult)
                //{
                //    case AXT_MOTION_HOME_RESULT.HOME_SUCCESS: axisData[axis].homeState = HomeState.HomeStateDONE; break;
                //    case AXT_MOTION_HOME_RESULT.HOME_SEARCHING: axisData[axis].homeState = HomeState.HomeStateDOING; break;
                //    default:
                //        axisData[axis].homeState = HomeState.HomeStateNEED;
                //        break;
                //}

                state = axisData[axis].homeState;

                //string errString = "";
                //switch ((AXT_MOTION_HOME_RESULT)homeResult)
                //{
                //    case AXT_MOTION_HOME_RESULT.HOME_SUCCESS: state = MotionState.MotionStateIDLE; return;
                //    case AXT_MOTION_HOME_RESULT.HOME_SEARCHING: state = MotionState.MotionStateMOVING; return;
                //    case AXT_MOTION_HOME_RESULT.HOME_ERR_GNT_RANGE: errString = "Gantry origin search criteria, setting error between two axes"; break;
                //    case AXT_MOTION_HOME_RESULT.HOME_ERR_USER_BREAK: errString = "Origin search user stops"; break;
                //    case AXT_MOTION_HOME_RESULT.HOME_ERR_VELOCITY: errString = "Origin search speed error occurs"; break;
                //    case AXT_MOTION_HOME_RESULT.HOME_ERR_AMP_FAULT: errString = "Servo pack alarm occurs Error"; break;
                //    case AXT_MOTION_HOME_RESULT.HOME_ERR_NEG_LIMIT: errString = "(-) direction driving (+) limit sensor detection error"; break;
                //    case AXT_MOTION_HOME_RESULT.HOME_ERR_POS_LIMIT: errString = "(+) direction driving (-) limit sensor detection error"; break;
                //    case AXT_MOTION_HOME_RESULT.HOME_ERR_NOT_DETECT: errString = "Error when specified signal is not detected"; break;
                //    case AXT_MOTION_HOME_RESULT.HOME_ERR_SETTING: errString = "When user-defined parameter occurs constraint condition during driving"; break;
                //    case AXT_MOTION_HOME_RESULT.HOME_ERR_SERVO_OFF: errString = "When servo is off"; break;
                //    case AXT_MOTION_HOME_RESULT.HOME_ERR_TIMEOUT: errString = "Error occurs due to specified time exceeded"; break;
                //    case AXT_MOTION_HOME_RESULT.HOME_ERR_FUNCALL: errString = "Function call failed"; break;
                //    case AXT_MOTION_HOME_RESULT.HOME_ERR_COUPLING: errString = "Gantry Master to Slave Over Distance protection"; break;
                //    case AXT_MOTION_HOME_RESULT.HOME_ERR_UNKNOWN: errString = "Unspecified error"; break;
                //    default:
                //        throw new Exception($"GetHomeState[{axis}] operation failed.");
                //}

                //throw new Exception($"GetHomeState encountered an [{errString}] error.");
            }
            catch
            {
            }
        }

        public override void GetAxisHomeComplete(int axis, ref HomeState state)
        {
            if (SystemDefine.IS_NOTEBOOK_MODE) return;
            try
            {
                uint homeResult = 0;
                if ((uint)AXT_FUNC_RESULT.AXT_RT_SUCCESS != CAXM.AxmHomeGetResult(axis, ref homeResult))
                    throw new Exception($"GetHomeState[{axis}] operation failed.");

                if ((AXT_MOTION_HOME_RESULT)homeResult == AXT_MOTION_HOME_RESULT.HOME_SUCCESS)
                    axisData[axis].homeState = HomeState.HomeStateDONE;

                state = axisData[axis].homeState;
            }
            catch
            {
            }
        }

        public override void SetAxisHomeComplete(int axis, HomeState state)
        {
            if (SystemDefine.IS_NOTEBOOK_MODE) return;
            try
            {
                axisData[axis].homeState = state;
                lock (lockControl)
                {
                    uint i = 0;
                    CAXM.AxmHomeGetSignalLevel(axis, ref i);
                    uint homeState = state == HomeState.HomeStateDONE ? (uint)AXT_MOTION_HOME_RESULT.HOME_SUCCESS : (uint)AXT_MOTION_HOME_RESULT.HOME_RESERVED;
                    if ((uint)AXT_FUNC_RESULT.AXT_RT_SUCCESS != CAXM.AxmHomeSetResult(axis, homeState))
                        throw new Exception($"HomeStart[{axis}] operation failed.");
                }
            }
            catch
            {
            }
        }

        public override void SetAxisHomeReadyPos(int axis)
        {
            if (SystemDefine.IS_NOTEBOOK_MODE) return;
            try
            {
                lock (lockControl)
                {
                    Thread.Sleep(200);
                    if ((uint)AXT_FUNC_RESULT.AXT_RT_SUCCESS != CAXM.AxmStatusSetCmdPos(axis, axisData[axis].readyPos))
                        throw new Exception($"SetAxisCommandPos[{axis}] operation failed.");
                    if ((uint)AXT_FUNC_RESULT.AXT_RT_SUCCESS != CAXM.AxmStatusSetActPos(axis, axisData[axis].readyPos))
                        throw new Exception($"SetAxisCommandPos[{axis}] operation failed.");
                }
            }
            catch
            {
            }
        }

        public override void MoveAxisAbs(int axis, double pos, double vel, double acc, double dec)
        {
            if (SystemDefine.IS_NOTEBOOK_MODE) return;
            if (!Machine.CheckTrayTransferInterlockForABS(axis, pos)) return;
            try
            {
                lock (lockControl)
                {
                    if (SystemDefine.motorVelocity > 0 && SystemDefine.motorVelocity <= 100) vel = vel * SystemDefine.motorVelocity / 100;
                    axisData[axis].commandpos = pos;
                    // CHANGE ABSOLUTE MODE
                    if ((uint)AXT_FUNC_RESULT.AXT_RT_SUCCESS != CAXM.AxmMotSetAbsRelMode(axis, 0))
                        throw new Exception($"MoveAbs[{axis}] operation failed.");
                    // MOVE
                    if ((uint)AXT_FUNC_RESULT.AXT_RT_SUCCESS != CAXM.AxmMoveStartPos(axis, pos, vel, acc, dec))
                        throw new Exception($"MoveAbs[{axis}] operation failed.");
                }
            }
            catch
            {
            }
        }

        public override void MoveAxisInc(int axis, double pos, double vel, double acc, double dec)
        {
            if (SystemDefine.IS_NOTEBOOK_MODE) return;
            try
            {
                lock (lockControl)
                {
                    if (SystemDefine.motorVelocity > 0 && SystemDefine.motorVelocity <= 100) vel = vel * SystemDefine.motorVelocity / 100;
                    double actpos = 0;
                    if ((uint)AXT_FUNC_RESULT.AXT_RT_SUCCESS != CAXM.AxmStatusGetCmdPos(axis, ref actpos))
                        throw new Exception($"SetAxisCommandPos[{axis}] operation failed.");
                    axisData[axis].commandpos = actpos + pos;

                    // CHANGE RELATIVE MODE
                    if ((uint)AXT_FUNC_RESULT.AXT_RT_SUCCESS != CAXM.AxmMotSetAbsRelMode(axis, 1))
                        throw new Exception($"MoveInc[{axis}] operation failed.");
                    // MOVE
                    if ((uint)AXT_FUNC_RESULT.AXT_RT_SUCCESS != CAXM.AxmMoveStartPos(axis, pos, vel, acc, dec))
                        throw new Exception($"MoveInc[{axis}] operation failed.");
                }
            }
            catch
            {
            }
        }

        public override void MoveAxisJog(int axis, double vel, double acc, double dec)
        {
            if (SystemDefine.IS_NOTEBOOK_MODE) return;
            if (!Machine.CheckTrayTransferInterlockForJOG(axis, vel)) return;
            try
            {
                lock (lockControl)
                {
                    if (SystemDefine.motorVelocity > 0 && SystemDefine.motorVelocity <= 100) vel = vel * SystemDefine.motorVelocity / 100;
                    // MOVE
                    if ((uint)AXT_FUNC_RESULT.AXT_RT_SUCCESS != CAXM.AxmMoveVel(axis, vel, acc, dec))
                        throw new Exception($"MoveAxisJog[{axis}] operation failed.");
                }
            }
            catch
            {
            }
        }

        public override bool MoveAxisDoneCheck(int axis)
        {
            if (SystemDefine.IS_NOTEBOOK_MODE) return false;
            try
            {
                lock (lockControl)
                {
                    uint ret = 0;
                    CAXM.AxmSignalGetInpos(axis, ref ret);
                    double actpos = 0;

                    if (ret == 0)
                    {
                        if ((uint)AXT_FUNC_RESULT.AXT_RT_SUCCESS != CAXM.AxmStatusGetCmdPos(axis, ref actpos))
                            throw new Exception($"SetAxisCommandPos[{axis}] operation failed.");
                    }
                    else
                    {
                        if ((uint)AXT_FUNC_RESULT.AXT_RT_SUCCESS != CAXM.AxmStatusGetActPos(axis, ref actpos))
                            throw new Exception($"SetAxisCommandPos[{axis}] operation failed.");
                    }
                    if (Math.Abs(axisData[axis].commandpos - actpos) > axisData[axis].inposRange)
                        return false;

                    uint status = 0;
                    if ((uint)AXT_FUNC_RESULT.AXT_RT_SUCCESS != CAXM.AxmStatusReadInMotion(axis, ref status))
                        throw new Exception($"GetAxisMotionState[{axis}] operation failed.");
                    if (Convert.ToBoolean(status))
                        return false;
                }
            }
            catch
            {
            }

            return true;
        }

        public override void GetAxisMotionState(int axis, ref MotionState state)
        {
            if (SystemDefine.IS_NOTEBOOK_MODE) return;
            try
            {
                lock (lockControl)
                {
                    uint status = 0;
                    if ((uint)AXT_FUNC_RESULT.AXT_RT_SUCCESS != CAXM.AxmStatusReadInMotion(axis, ref status))
                        throw new Exception($"GetAxisMotionState[{axis}] operation failed.");

                    bool error = false;
                    GetAlarm(axis, ref error);
                    if (error)
                        state = MotionState.MotionStateERROR;
                    else
                    {
                        bool motionDone = Convert.ToBoolean(status);

                        if (motionDone)
                            state = MotionState.MotionStateMOVING;
                        else
                        {
                            double actualPos = 0;
                            double commandPos = 0;

                            GetAxisActualPos(axis, ref actualPos);
                            GetAxisCommandPos(axis, ref commandPos);

                            if (Math.Abs(actualPos - commandPos) < 5)
                                state = MotionState.MotionStateIDLE;
                        }
                    }
                }
            }
            catch
            {
            }
        }

        public override void AxisStop(int axis, bool estop, double decel = 1000000)
        {
            if (SystemDefine.IS_NOTEBOOK_MODE) return;
            try
            {
                if (estop)
                {
                    if ((uint)AXT_FUNC_RESULT.AXT_RT_SUCCESS != CAXM.AxmMoveEStop(axis))
                        throw new Exception($"Stop[{axis}] operation failed.");
                    if (axisData[axis].homeState == HomeState.HomeStateDOING)
                        axisData[axis].homeState = HomeState.HomeStateNEED;
                }
                else
                {
                    if ((uint)AXT_FUNC_RESULT.AXT_RT_SUCCESS != CAXM.AxmMoveStop(axis, decel))
                        throw new Exception($"Stop[{axis}] operation failed.");
                }

                if (axisData[axis].homeState == HomeState.HomeStateDOING)
                    axisData[axis].homeState = HomeState.HomeStateNEED;
            }
            catch
            {
            }
        }

        public override void AxisStopAll(bool estop, double decel = 1000000)
        {
            if (SystemDefine.IS_NOTEBOOK_MODE) return;
            for (int i = 0; i < axisData.Length; i++)
                AxisStop(i, estop, decel);
        }

        public override void GetAlarm(int axis, ref bool alaramOn)
        {
            if (SystemDefine.IS_NOTEBOOK_MODE) return;
            try
            {
                uint state = 0;
                if ((uint)AXT_FUNC_RESULT.AXT_RT_SUCCESS != CAXM.AxmSignalReadServoAlarm(axis, ref state))
                    throw new Exception($"GetAxisInposition[{axis}] operation failed.");
                alaramOn = Convert.ToBoolean(state);

                if (alaramOn)
                    axisData[axis].homeState = HomeState.HomeStateNEED;
            }
            catch
            {
            }
        }

        public override void AlarmClear(int axis)
        {
            if (SystemDefine.IS_NOTEBOOK_MODE) return;
            try
            {
                lock (lockControl)
                {
                    // lBitNo : Bit Number(0 - 4)
                    // uOnOff : FALSE(0), TRUE(1)
                    if ((uint)AXT_FUNC_RESULT.AXT_RT_SUCCESS != CAXM.AxmSignalWriteOutputBit(axis, 1, 1))
                        throw new Exception($"Stop[{axis}] operation failed.");
                }
            }
            catch
            {
            }
        }

        public override void GetAxisCommandPos(int axis, ref double pos)
        {
            if (SystemDefine.IS_NOTEBOOK_MODE) return;
            try
            {
                if ((uint)AXT_FUNC_RESULT.AXT_RT_SUCCESS != CAXM.AxmStatusGetCmdPos(axis, ref pos))
                    throw new Exception($"GetAxisActualPos[{axis}] operation failed.");
            }
            catch
            {
            }
        }

        public override void SetAxisCommandPos(int axis, double pos)
        {
            if (SystemDefine.IS_NOTEBOOK_MODE) return;
            try
            {
                if ((uint)AXT_FUNC_RESULT.AXT_RT_SUCCESS != CAXM.AxmStatusSetCmdPos(axis, pos))
                    throw new Exception($"SetAxisCommandPos[{axis}] operation failed.");
            }
            catch
            {
            }
        }

        public override void GetAxisActualPos(int axis, ref double pos)
        {
            if (SystemDefine.IS_NOTEBOOK_MODE) return;
            try
            {
                uint ret = 0;
                CAXM.AxmSignalGetInpos(axis, ref ret);

                if (ret == 1)
                {
                    if ((uint)AXT_FUNC_RESULT.AXT_RT_SUCCESS != CAXM.AxmStatusGetActPos(axis, ref pos))
                        throw new Exception($"GetAxisActualPos[{axis}] operation failed.");
                }
                else
                {
                    if ((uint)AXT_FUNC_RESULT.AXT_RT_SUCCESS != CAXM.AxmStatusGetCmdPos(axis, ref pos))
                        throw new Exception($"GetAxisActualPos[{axis}] operation failed.");
                }
            }
            catch
            {
            }
        }

        public override void SetAxisActualPos(int axis, double pos)
        {
            if (SystemDefine.IS_NOTEBOOK_MODE) return;
            try
            {
                if ((uint)AXT_FUNC_RESULT.AXT_RT_SUCCESS != CAXM.AxmStatusSetActPos(axis, pos))
                    throw new Exception($"SetAxisActualPos[{axis}] operation failed.");
            }
            catch
            {
            }
        }

        public override void GetAxisInpos(int axis, ref bool inposOn)
        {
            if (SystemDefine.IS_NOTEBOOK_MODE) return;
            try
            {
                uint state = 0;
                if ((uint)AXT_FUNC_RESULT.AXT_RT_SUCCESS != CAXM.AxmStatusReadMechanical(axis, ref state))
                    throw new Exception($"GetAxisInposition[{axis}] operation failed.");
                inposOn = (state == (uint)AXT_MOTION_QIMECHANICAL_SIGNAL.QIMECHANICAL_INP_LEVEL);
            }
            catch
            {
            }
        }

        public override void SetAxisInposRange(int axis, double range)
        {
            if (SystemDefine.IS_NOTEBOOK_MODE) return;
            try
            {
                lock (lockControl)
                {
                    if ((uint)AXT_FUNC_RESULT.AXT_RT_SUCCESS != CAXM.AxmSignalSetInposRange(axis, range))
                        throw new Exception($"SetAxisInposRange[{axis}] operation failed.");
                }
            }
            catch
            {
            }
        }

        public override void GetAxisHW_NEGLimit(int axis, ref bool limitOn)
        {
            if (SystemDefine.IS_NOTEBOOK_MODE) return;
            try
            {
                lock (lockControl)
                {
                    uint negLimit = 0;
                    uint posLimit = 0;
                    if ((uint)AXT_FUNC_RESULT.AXT_RT_SUCCESS != CAXM.AxmSignalReadLimit(axis, ref posLimit, ref negLimit))
                        throw new Exception($"GetAxisHW_NEGLimit[{axis}] operation failed.");
                    limitOn = (negLimit == 1);

                    //if (limitOn)
                    //    axisData[axis].homeState = HomeState.HomeStateNEED;
                }
            }
            catch
            {
            }
        }

        public override void GetAxisHW_POSLimit(int axis, ref bool limitOn)
        {
            if (SystemDefine.IS_NOTEBOOK_MODE) return;
            try
            {
                lock (lockControl)
                {
                    uint negLimit = 0;
                    uint posLimit = 0;
                    if ((uint)AXT_FUNC_RESULT.AXT_RT_SUCCESS != CAXM.AxmSignalReadLimit(axis, ref posLimit, ref negLimit))
                        throw new Exception($"GetAxisHW_POSLimit[{axis}] operation failed.");
                    limitOn = (posLimit == 1);

                    //if (limitOn)
                    //    axisData[axis].homeState = HomeState.HomeStateNEED;
                }
            }
            catch
            {
            }
        }
        
        public override void GetAxisSW_NEGLimit(int axis, ref bool limitOn)
        {
            if (SystemDefine.IS_NOTEBOOK_MODE) return;
            try
            {
                lock (lockControl)
                {
                    uint negLimit = 0;
                    uint posLimit = 0;
                    if ((uint)AXT_FUNC_RESULT.AXT_RT_SUCCESS != CAXM.AxmSignalReadSoftLimit(axis, ref posLimit, ref negLimit))
                        throw new Exception($"GetAxisSW_NEGLimit[{axis}] operation failed.");
                    limitOn = (negLimit == 1);

                    //if (limitOn)
                    //    axisData[axis].homeState = HomeState.HomeStateNEED;
                }
            }
            catch
            {
            }
        }

        public override void GetAxisSW_POSLimit(int axis, ref bool limitOn)
        {
            if (SystemDefine.IS_NOTEBOOK_MODE) return;
            try
            {
                lock (lockControl)
                {
                    uint negLimit = 0;
                    uint posLimit = 0;
                    if ((uint)AXT_FUNC_RESULT.AXT_RT_SUCCESS != CAXM.AxmSignalReadSoftLimit(axis, ref posLimit, ref negLimit))
                        throw new Exception($"GetAxisSW_POSLimit[{axis}] operation failed.");
                    limitOn = (posLimit == 1);

                    //if (limitOn)
                    //    axisData[axis].homeState = HomeState.HomeStateNEED;
                }
            }
            catch
            {
            }
        }

        public override void GetAxis_NEGLimit(int axis, ref bool limitOn)
        {
            if (SystemDefine.IS_NOTEBOOK_MODE) return;
            try
            {
                lock (lockControl)
                {
                    bool limitOnHW = false;
                    bool limitOnSW = false;

                    GetAxisHW_NEGLimit(axis, ref limitOnHW);
                    GetAxisSW_NEGLimit(axis, ref limitOnSW);

                    limitOn = limitOnHW || limitOnSW;
                }
            }
            catch
            {
            }
        }

        public override void GetAxis_POSLimit(int axis, ref bool limitOn)
        {
            if (SystemDefine.IS_NOTEBOOK_MODE) return;
            try
            {
                lock (lockControl)
                {
                    bool limitOnHW = false;
                    bool limitOnSW = false;

                    GetAxisHW_POSLimit(axis, ref limitOnHW);
                    GetAxisSW_POSLimit(axis, ref limitOnSW);

                    limitOn = limitOnHW || limitOnSW;
                }
            }
            catch
            {
            }
        }

        public override void GetAxisHomeSensor(int axis, ref bool SignalOn)
        {
            if (SystemDefine.IS_NOTEBOOK_MODE) return;
            try
            {
                lock (lockControl)
                {
                    uint uSignal = 0;
                    if ((uint)AXT_FUNC_RESULT.AXT_RT_SUCCESS != CAXM.AxmHomeReadSignal(axis, ref uSignal))
                        throw new Exception($"GetAxisHomeSensor[{axis}] operation failed.");
                    SignalOn = uSignal == 1 ? true : false;
                }
            }
            catch
            {
            }
        }

        public override void SetAxisHomeSensorLevel(int axis, uint level)
        {
            if (SystemDefine.IS_NOTEBOOK_MODE) return;
            try
            {
                lock (lockControl)
                {
                    if ((uint)AXT_FUNC_RESULT.AXT_RT_SUCCESS != CAXM.AxmHomeSetSignalLevel(axis, level))
                        throw new Exception($"GetAxisHomeSensor[{axis}] operation failed.");
                }
            }
            catch
            {
            }
        }

        public override void GetAxisServoOn(int axis, ref bool enable)
        {
            if (SystemDefine.IS_NOTEBOOK_MODE) return;
            try
            {
                lock (lockControl)
                {
                    uint servoOn = 0;
                    if ((uint)AXT_FUNC_RESULT.AXT_RT_SUCCESS != CAXM.AxmSignalIsServoOn(axis, ref servoOn))
                        throw new Exception($"GetAxisHW_POSLimit[{axis}] operation failed.");
                    enable = (servoOn == 1);

                    if (!enable)
                        axisData[axis].homeState = HomeState.HomeStateNEED;
                }
            }
            catch
            {
            }
        }
    }
}
