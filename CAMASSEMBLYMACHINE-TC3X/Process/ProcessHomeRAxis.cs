using static CAMASSEMBLYMACHINE.Define.SystemDefine;
using TopEng.Device;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CAMASSEMBLYMACHINE.UI;

namespace CAMASSEMBLYMACHINE.Process
{
    class ProcessHomeRAxis : IProcess
    {
        public enum STEP
        {
            ERROR = -1,
            IDLE,
            STOP,
            START,

            HOME_SEARCH_ORGINAL,
            HOME_SEARCH_ORGINAL_CHECK,

            AXIS_MOVE_POS_LITTLE,
            AXIS_HOME_POSITION_CHECK,
            AXIS_MOVE_NEG_LITTLE,
            AXIS_HOME_SEARCH_CHECK,
            AXIS_MOVE_TRIG,
            AXIS_HOME_DONE_CHECK,
            AXIS_MOVE_READY,
            AXIS_MOVE_DONE_CHECK,
        }

        public enum MSG
        {
            MSG_HOME_START,
        }

        #region PROCESS_STEP_VARIABLES
        public List<STEP> StepList = new List<STEP>();
        private STEP _step;
        public STEP Step = STEP.IDLE;
        public int StepIndex = 0;
        public override Type StepEnumType => typeof(STEP);
        public override Type MsgEnumType => typeof(MSG);

        public override bool Busy()
        {
            return Step != STEP.IDLE && Step != STEP.ERROR;
        }

        public override bool Ready()
        {
            return Step == STEP.IDLE;
        }

        public override bool Stopped()
        {
            return (Step == STEP.STOP);
        }

        public override bool Error()
        {
            return Step == STEP.ERROR;
        }

        public STEP NextStep()
        {
            if (++StepIndex < StepList.Count())
                Step = StepList[StepIndex];
            else
                Step = STEP.IDLE;
            return Step;
        }
        #endregion

        public int currentAxis = -1;
        public bool negDirectionOn = false;
        public bool axisPositionning = false;
        public int tryCount = 0;
        public double searchLength = 5 * 1000;
        public double trigLength = 2 * 1000;

        public ProcessHomeRAxis(int axis)
        {
            currentAxis = axis;
        }

        public override int GetStep() { return (int)Step; }
        public override int GetAutoStep() { return (int)0; }

        public override void TimeOut()
        {

        }

        public override void SetMessage(int message, int step = 0)
        {
            if (!Ready() || Error())
            {
                SubForm_Warning dlg = new SubForm_Warning("The equipment is running or in an error state.");
                dlg.ShowDialog();
                return;
            }

            switch ((MSG)message)
            {
                case MSG.MSG_HOME_START:
                    negDirectionOn = false;
                    tryCount = 1;

                    var processHomeStart = new List<STEP>()
                    {
                        STEP.START,
                        STEP.IDLE,
                    };

                    StepList = processHomeStart;
                    Step = StepList[StepIndex = 0];
                    break;
            }
        }

        public override void AutoStart()
        {

        }

        public override void EStop(bool pause)
        {
            stopBit = true;

            for (int i = 0; i < StepList.Count; i++)
                StepList[i] = STEP.IDLE;

            Step = STEP.IDLE;
        }

        public override void Stop()
        {
            stopBit = true;
        }

        protected override void OnProcessing()
        {
            HomeState state = HomeState.HomeStateNEED;
            uint ret1 = 0;
            bool signal = false;

            double searchVel = 10 * 1000;
            double trigVel = 0.5 * 1000;
            switch ((STEP)Step)
            {
                case STEP.ERROR:
                    timeWait[(int)TIMER.TIMEOUT].Reset();
                    break;

                case STEP.IDLE:
                    timeWait[(int)TIMER.TIMEOUT].Reset();
                    break;

                case STEP.START:
                    Machine.motion.SetAxisHomeComplete(currentAxis, HomeState.HomeStateDOING);
                    negDirectionOn = false;
                    axisPositionning = false;
                    Step = STEP.HOME_SEARCH_ORGINAL;
                    break;

                case STEP.HOME_SEARCH_ORGINAL:
                    Machine.motion.SetAxisHomeStart(currentAxis);
                    Step = STEP.HOME_SEARCH_ORGINAL_CHECK;
                    break;

                case STEP.HOME_SEARCH_ORGINAL_CHECK:
                    Machine.motion.GetAxisHomeComplete(currentAxis, ref state);
                    if (state != HomeState.HomeStateDONE)
                        break;
                    NextStep();
                    break;

                case STEP.AXIS_MOVE_POS_LITTLE:
                    Machine.motion.MoveAxisInc(currentAxis, tryCount * searchLength, searchVel, 10 * searchVel, 10 * searchVel);
                    Step = STEP.AXIS_HOME_POSITION_CHECK;
                    break;

                case STEP.AXIS_HOME_POSITION_CHECK:
                    Machine.motion.GetAxisHomeSensor(currentAxis, ref signal);
                    if (signal && !axisPositionning)
                    {
                        Machine.motion.AxisStop(currentAxis, true);
                        axisPositionning = true;
                        Step = STEP.AXIS_MOVE_POS_LITTLE;
                    }
                    else
                    {
                        if (Machine.motion.MoveAxisDoneCheck(currentAxis))
                        {
                            tryCount++;
                            Step = STEP.AXIS_MOVE_NEG_LITTLE;
                        }
                    }
                    break;

                case STEP.AXIS_MOVE_NEG_LITTLE:
                    Machine.motion.MoveAxisInc(currentAxis, (-1) * tryCount * searchLength, searchVel, 10 * searchVel, 10 * searchVel);
                    Step = STEP.AXIS_HOME_SEARCH_CHECK;
                    break;

                case STEP.AXIS_HOME_SEARCH_CHECK:
                    Machine.motion.GetAxisHomeSensor(currentAxis, ref signal);
                    if (signal)
                    {
                        Machine.motion.AxisStop(currentAxis, true);
                        Step = STEP.AXIS_MOVE_TRIG;
                    }
                    else
                    {
                        if (Machine.motion.MoveAxisDoneCheck(currentAxis))
                        {
                            tryCount++;
                            Step = STEP.AXIS_MOVE_POS_LITTLE;
                        }
                    }
                    break;

                case STEP.AXIS_MOVE_TRIG:
                    Machine.motion.MoveAxisInc(currentAxis, trigLength, trigVel, 10 * searchVel, 10 * searchVel);
                    Step = STEP.AXIS_HOME_DONE_CHECK;
                    break;

                case STEP.AXIS_HOME_DONE_CHECK:
                    Machine.motion.GetAxisHomeSensor(currentAxis, ref signal);
                    if (!signal)
                    {
                        Machine.motion.AxisStop(currentAxis, true);
                        //Machine.motion.SetAxisHomeComplete(currentAxis, HomeState.HomeStateDONE);
                        Machine.motion.SetAxisHomeReadyPos(currentAxis);

                        Step = STEP.AXIS_MOVE_READY;
                    }
                    else
                    {
                        if (!Machine.motion.MoveAxisDoneCheck(currentAxis))
                            break;

                        Step = STEP.AXIS_MOVE_TRIG;
                    }
                    break;

                case STEP.AXIS_MOVE_READY:
                    Machine.motion.MoveAxisAbs(currentAxis, 0, searchVel, 10 * searchVel, 10 * searchVel);
                    Step = STEP.AXIS_MOVE_DONE_CHECK;
                    break;

                case STEP.AXIS_MOVE_DONE_CHECK:
                    if (!Machine.motion.MoveAxisDoneCheck(currentAxis))
                        break;
                    Machine.motion.SetAxisHomeComplete(currentAxis, HomeState.HomeStateDONE);
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
