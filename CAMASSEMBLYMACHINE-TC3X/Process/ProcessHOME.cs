using static CAMASSEMBLYMACHINE.Define.SystemDefine;
using TopEng.Device;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CAMASSEMBLYMACHINE.UI;
using CAMASSEMBLYMACHINE.Define;
using System.Windows.Forms;
using TopEng.Controls;
using System.Threading;
using TopEng.Utils;
using static CAMASSEMBLYMACHINE.UI.Form_Origin2;

namespace CAMASSEMBLYMACHINE.Process
{
    class ProcessHOME : IProcess
    {
        public enum STEP
        {
            ERROR = -1,
            IDLE,
            STOP,

            PICKER_UP,
            PICKER_UP_CHECK,
            TRAY_TRANSFER_EXIST_UP,
            TRAY_TRANSFER_EXIST_UP_CHECK,
            CAM_TRANSFER_UP,
            CAM_TRANSFER_UP_CHECK,
            PEELING_RETURN,
            PEELING_RETURN_CHECK,
            PEELING_DOWN,
            PEELING_DOWN_CHECK,
            AXIS_Z_ACTION,
            AXIS_Z_ACTION_CHECK,
            AXIS_TRANSFER_ACTION,
            AXIS_TRANSFER_ACTION_CHECK,
            AXIS_ASSEMBLER_ACTION,
            AXIS_ASSEMBLER_ACTION_CHECK,
            AXIS_ASSEMBLER_R_ACTION,
            AXIS_ASSEMBLER_R_ACTION_CHECK,
            AXIS_ASSEMBLER_XY_ACTION,
            AXIS_ASSEMBLER_XY_ACTION_CHECK,
            AXIS_PICKER_ACTION,
            AXIS_PICKER_HOME_CHECK,

            AXIS_HOME_COMPLETE,

            // INIT
            PROC_INIT_JIG_ASSEMBLER,
            PROC_INIT_JIG_ASSEMBLER_CHECK,
            PROC_INIT_JIG_CONV,
            PROC_INIT_JIG_CONV_CHECK,

            PROC_INIT_TRAY_PROD_LOADER,
            PROC_INIT_TRAY_PROD_LOADER_CHECK,
            PROC_INIT_TRAY_CONV,
            PROC_INIT_TRAY_CONV_CHECK,

            PROC_INIT_BUFFER,
            PROC_INIT_BUFFER_CHECK,

            PROC_INIT_COMPLETE,
        }

        public enum MSG
        {
            MSG_HOME_START,
            MSG_SERVO_ON_START,
            MSG_SERVO_OFF_START,
            MSG_ALARM_CLEAR_START,

            MSG_INITIALIZE
        }

        #region PROCESS_STEP_VARIABLES
        public List<STEP> StepList = new List<STEP>();
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

        int[] RAxisList = { (int)AXIS.PROD_PICKUP_R1, (int)AXIS.PROD_PICKUP_R2, (int)AXIS.ASSEMBLER_R1, (int)AXIS.ASSEMBLER_R2 };
        private SERVO_TASK eServoTask;
        public bool[] SelectAxis = new bool[(int)AXIS.AXISMAX];
        public ProcessHomeRAxis[] HomeRProcess;
        private int iDelayPerTask = 50;
        public bool init_tray = false;
        public bool init_prodLoader = false;
        public bool init_jig = false;
        public bool init_buffer = false;
        public bool bIsHomeRequestedNDone = false;
        public int iHomeSearchTimeOut = 180000;
        public ProcessHOME()
        {
            HomeRProcess = new ProcessHomeRAxis[RAxisList.Length];

            for (int i = 0; i < RAxisList.Length; i++)
            {
                HomeRProcess[i] = new ProcessHomeRAxis(RAxisList[i]);
                HomeRProcess[i].Run();
            }
        }

        public override int GetStep() { return (int)Step; }
        public override int GetAutoStep() { return 0 ; }

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
            Machine.Alarm(error);
        }

        public override void SetMessage(int message, int step = 0)
        {
            if (Busy() || Error())
            {
                SubForm_Warning dlg = new SubForm_Warning("The equipment is running or in an error state.");
                dlg.ShowDialog();
                return;
            }

            switch ((MSG)message)
            {
                case MSG.MSG_HOME_START:
                    eServoTask = SERVO_TASK.ORIGIN;
                    this.bIsHomeRequestedNDone = false;
                    var processHomeStart = new List<STEP>()
                    {
                        STEP.PICKER_UP,
                        STEP.TRAY_TRANSFER_EXIST_UP,
                        STEP.AXIS_Z_ACTION,
                        STEP.CAM_TRANSFER_UP,
                        STEP.PEELING_RETURN,
                        STEP.PEELING_DOWN,
                        STEP.AXIS_TRANSFER_ACTION,
                        STEP.AXIS_ASSEMBLER_R_ACTION,
                        STEP.AXIS_ASSEMBLER_XY_ACTION,
                        STEP.AXIS_PICKER_ACTION,

                        STEP.AXIS_HOME_COMPLETE,
                        STEP.IDLE,
                    };

                    StepList = processHomeStart;
                    Step = StepList[StepIndex = 0];
                    break;

                case MSG.MSG_SERVO_ON_START:
                case MSG.MSG_SERVO_OFF_START:
                case MSG.MSG_ALARM_CLEAR_START:
                    if ((MSG)message == MSG.MSG_SERVO_ON_START) eServoTask = SERVO_TASK.SERVO_ON;
                    if ((MSG)message == MSG.MSG_SERVO_OFF_START) eServoTask = SERVO_TASK.SERVO_OFF;
                    if ((MSG)message == MSG.MSG_ALARM_CLEAR_START) eServoTask = SERVO_TASK.ALARM_CLEAR;

                    var processStep = new List<STEP>()
                    {
                        STEP.AXIS_Z_ACTION,
                        STEP.AXIS_TRANSFER_ACTION,
                        STEP.AXIS_ASSEMBLER_R_ACTION,
                        STEP.AXIS_ASSEMBLER_XY_ACTION,
                        STEP.AXIS_PICKER_ACTION,
                        STEP.IDLE,
                    };

                    StepList = processStep;
                    Step = StepList[StepIndex = 0];
                    break;

                case MSG.MSG_INITIALIZE:
                    var processInitTray = new List<STEP>()
                    {
                        STEP.PROC_INIT_JIG_ASSEMBLER,
                        STEP.PROC_INIT_JIG_CONV,
                        STEP.PROC_INIT_TRAY_PROD_LOADER,
                        STEP.PROC_INIT_TRAY_CONV,
                        STEP.PROC_INIT_BUFFER,
                        STEP.PROC_INIT_COMPLETE,
                        STEP.IDLE,
                    };

                    StepList = processInitTray;
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

            for (int i = 0; i < HomeRProcess.Length; i++)
                HomeRProcess[i].EStop(pause);
        }

        public override void Stop()
        {
            stopBit = true;

            for (int i = 0; i < HomeRProcess.Length; i++)
                HomeRProcess[i].Stop();
        }

        protected override void OnProcessing()
        {
            HomeState state = HomeState.HomeStateNEED;
            bool compl = false;
            uint ret1 = 0;
            uint ret2 = 0;

            switch ((STEP)Step)
            {
                case STEP.ERROR:
                    error_proc = false;
                    timeWait[(int)TIMER.TIMEOUT].Reset();
                    break;

                case STEP.IDLE:
                    error_proc = false;
                    timeWait[(int)TIMER.TIMEOUT].Reset();
                    break;

                case STEP.PICKER_UP:
                    Machine.IO.SetOut((int)DO.CAM_PICKER_Z1_UP, 1);
                    Machine.IO.SetOut((int)DO.CAM_PICKER_Z1_DOWN, 0);
                    Machine.IO.SetOut((int)DO.CAM_PICKER_Z2_UP, 1);
                    Machine.IO.SetOut((int)DO.CAM_PICKER_Z2_DOWN, 0);
                    Machine.IO.SetOut((int)DO.ASSEMBLER_Z1_UP, 1);
                    Machine.IO.SetOut((int)DO.ASSEMBLER_Z1_DOWN, 0);
                    Machine.IO.SetOut((int)DO.ASSEMBLER_Z2_UP, 1);
                    Machine.IO.SetOut((int)DO.ASSEMBLER_Z2_DOWN, 0);
                    timeWait[(int)TIMER.TIMEOUT].Reset();
                    Step = STEP.PICKER_UP_CHECK;
                    break;

                case STEP.PICKER_UP_CHECK:
                    Machine.IO.GetIn((int)DI.CAM_PICKER_Z1_UP, ref ret1);
                    if (ret1 == 0)
                    {
                        if (timeWait[(int)TIMER.TIMEOUT].Elapsed > Machine.param.Time(ParameterDefine.TIME.CYLINDER_TIME_OUT))
                        {
                            SetError(ECODE.TIMEOUT_PROD_PICK1_UP);
                            break;
                        }
                        break;
                    }
                    Machine.IO.GetIn((int)DI.CAM_PICKER_Z2_UP, ref ret1);
                    if (ret1 == 0)
                    {
                        if (timeWait[(int)TIMER.TIMEOUT].Elapsed > Machine.param.Time(ParameterDefine.TIME.CYLINDER_TIME_OUT))
                        {
                            SetError(ECODE.TIMEOUT_PROD_PICK2_UP);
                            break;
                        }
                        break;
                    }
                    Machine.IO.GetIn((int)DI.ASSEMBLER_Z1_UP, ref ret1);
                    if (ret1 == 0)
                    {
                        if (timeWait[(int)TIMER.TIMEOUT].Elapsed > Machine.param.Time(ParameterDefine.TIME.CYLINDER_TIME_OUT))
                        {
                            SetError(ECODE.TIMEOUT_ASSEMBLER1_UP);
                            break;
                        }
                        break;
                    }
                    Machine.IO.GetIn((int)DI.ASSEMBLER_Z2_UP, ref ret1);
                    if (ret1 == 0)
                    {
                        if (timeWait[(int)TIMER.TIMEOUT].Elapsed > Machine.param.Time(ParameterDefine.TIME.CYLINDER_TIME_OUT))
                        {
                            SetError(ECODE.TIMEOUT_ASSEMBLER2_UP);
                            break;
                        }
                        break;
                    }
                    NextStep();
                    break;

                case STEP.TRAY_TRANSFER_EXIST_UP:
                    Machine.IO.SetOut((int)DO.TRAY_TRF_EXIST_UP, 1);
                    Machine.IO.SetOut((int)DO.TRAY_TRF_EXIST_DOWN, 0);
                    timeWait[(int)TIMER.TIMEOUT].Reset();
                    Step = STEP.TRAY_TRANSFER_EXIST_UP_CHECK;
                    break;

                case STEP.TRAY_TRANSFER_EXIST_UP_CHECK:
                    Machine.IO.GetOut((int)DO.TRAY_TRF_EXIST_UP, ref ret1);
                    if (ret1 == 0)
                    {
                        if (timeWait[(int)TIMER.TIMEOUT].Elapsed > Machine.param.Time(ParameterDefine.TIME.CYLINDER_TIME_OUT))
                        {
                            SetError(ECODE.TIMEOUT_TRAY_TRF_UP);
                            break;
                        }
                        break;
                    }                    
                    if (ret1 == 1)
                        NextStep();
                    break;

                case STEP.CAM_TRANSFER_UP:
                    //Machine.IO.SetOut((int)DO.CAM_TRF_L_UP, 1);
                    //Machine.IO.SetOut((int)DO.CAM_TRF_R_UP, 1);
                    //Machine.IO.SetOut((int)DO.CAM_TRF_L_DOWN, 0);
                    //Machine.IO.SetOut((int)DO.CAM_TRF_R_DOWN, 0);
                    timeWait[(int)TIMER.TIMEOUT].Reset();
                    Step = STEP.CAM_TRANSFER_UP_CHECK;
                    break;

                case STEP.CAM_TRANSFER_UP_CHECK:
                    //Machine.IO.GetIn((int)DI.CAM_TRF_L_Z_UP, ref ret1);
                    //Machine.IO.GetIn((int)DI.CAM_TRF_R_Z_UP, ref ret2);

                    //if (ret1 == 0)
                    //{
                    //    if (timeWait[(int)TIMER.TIMEOUT].Elapsed > Machine.param.Time(ParameterDefine.TIME.CYLINDER_TIME_OUT))
                    //    {
                    //        SetError(ECODE.TIMEOUT_CAM_TRF_L_UP);
                    //        break;
                    //    }
                    //    break;
                    //}

                    //if (ret2 == 0)
                    //{
                    //    if (timeWait[(int)TIMER.TIMEOUT].Elapsed > Machine.param.Time(ParameterDefine.TIME.CYLINDER_TIME_OUT))
                    //    {
                    //        SetError(ECODE.TIMEOUT_CAM_TRF_R_UP);
                    //        break;
                    //    }
                    //    break;
                    //}

                    //if (ret1 == 1 && ret2 == 1)
                        NextStep();
                    break;

                case STEP.PEELING_RETURN:
                    //Machine.IO.SetOut((int)DO.PEELING_L1_RETURN, 1);
                    //Machine.IO.SetOut((int)DO.PEELING_L1_TURN, 0);
                    //Machine.IO.SetOut((int)DO.PEELING_L2_RETURN, 1);
                    //Machine.IO.SetOut((int)DO.PEELING_L2_TURN, 0);
                    //Machine.IO.SetOut((int)DO.PEELING_R1_RETURN, 1);
                    //Machine.IO.SetOut((int)DO.PEELING_R1_TURN, 0);
                    //Machine.IO.SetOut((int)DO.PEELING_R2_RETURN, 1);
                    //Machine.IO.SetOut((int)DO.PEELING_R2_TURN, 0);

                    timeWait[(int)TIMER.TIMEOUT].Start();
                    Step = STEP.PEELING_RETURN_CHECK;
                    break;

                case STEP.PEELING_RETURN_CHECK:
                    //Machine.IO.GetIn((int)DI.PEELING_L1_RETURN, ref ret1);
                    //if (ret1 == 0)
                    //{
                    //    if (timeWait[(int)TIMER.TIMEOUT].Elapsed > Machine.param.Time(ParameterDefine.TIME.CYLINDER_TIME_OUT))
                    //    {
                    //        SetError(ECODE.TIMEOUT_PEELING_L1_RETURN);
                    //        break;
                    //    }
                    //    break;
                    //}
                    //Machine.IO.GetIn((int)DI.PEELING_L2_RETURN, ref ret1);
                    //if (ret1 == 0)
                    //{
                    //    if (timeWait[(int)TIMER.TIMEOUT].Elapsed > Machine.param.Time(ParameterDefine.TIME.CYLINDER_TIME_OUT))
                    //    {
                    //        SetError(ECODE.TIMEOUT_PEELING_L2_RETURN);
                    //        break;
                    //    }
                    //    break;
                    //}
                    //Machine.IO.GetIn((int)DI.PEELING_R1_RETURN, ref ret1);
                    //if (ret1 == 0)
                    //{
                    //    if (timeWait[(int)TIMER.TIMEOUT].Elapsed > Machine.param.Time(ParameterDefine.TIME.CYLINDER_TIME_OUT))
                    //    {
                    //        SetError(ECODE.TIMEOUT_PEELING_R1_RETURN);
                    //        break;
                    //    }
                    //    break;
                    //}
                    //Machine.IO.GetIn((int)DI.PEELING_R2_RETURN, ref ret1);
                    //if (ret1 == 0)
                    //{
                    //    if (timeWait[(int)TIMER.TIMEOUT].Elapsed > Machine.param.Time(ParameterDefine.TIME.CYLINDER_TIME_OUT))
                    //    {
                    //        SetError(ECODE.TIMEOUT_PEELING_R2_RETURN);
                    //        break;
                    //    }
                    //    break;
                    //}
                    //Thread.Sleep(500);
                    NextStep();
                    break;

                case STEP.PEELING_DOWN:
                    //Machine.IO.SetOut((int)DO.PEELING_L1_Z1_UP, 0);
                    //Machine.IO.SetOut((int)DO.PEELING_L1_Z1_DOWN, 1);
                    //Machine.IO.SetOut((int)DO.PEELING_L1_Z2_UP, 0);
                    //Machine.IO.SetOut((int)DO.PEELING_L1_Z2_DOWN, 1);
                    //Machine.IO.SetOut((int)DO.PEELING_L2_Z1_UP, 0);
                    //Machine.IO.SetOut((int)DO.PEELING_L2_Z1_DOWN, 1);
                    //Machine.IO.SetOut((int)DO.PEELING_L2_Z2_UP, 0);
                    //Machine.IO.SetOut((int)DO.PEELING_L2_Z2_DOWN, 1);

                    //Machine.IO.SetOut((int)DO.PEELING_R1_Z1_UP, 0);
                    //Machine.IO.SetOut((int)DO.PEELING_R1_Z1_DOWN, 1);
                    //Machine.IO.SetOut((int)DO.PEELING_R1_Z2_UP, 0);
                    //Machine.IO.SetOut((int)DO.PEELING_R1_Z2_DOWN, 1);
                    //Machine.IO.SetOut((int)DO.PEELING_R2_Z1_UP, 0);
                    //Machine.IO.SetOut((int)DO.PEELING_R2_Z1_DOWN, 1);
                    //Machine.IO.SetOut((int)DO.PEELING_R2_Z2_UP, 0);
                    //Machine.IO.SetOut((int)DO.PEELING_R2_Z2_DOWN, 1);

                    Step = STEP.PEELING_DOWN_CHECK;
                    timeWait[(int)TIMER.TIMEOUT].Start();
                    break;

                case STEP.PEELING_DOWN_CHECK:
                    //Machine.IO.GetIn((int)DI.PEELING_L1_Z1_DOWN, ref ret1);
                    //Machine.IO.GetIn((int)DI.PEELING_L1_Z2_DOWN, ref ret2);
                    //if (ret1 == 0 || ret2 == 0)
                    //{
                    //    if (timeWait[(int)TIMER.TIMEOUT].Elapsed > Machine.param.Time(ParameterDefine.TIME.CYLINDER_TIME_OUT))
                    //    {
                    //        SetError(ECODE.TIMEOUT_PEELING_L1_DOWN);
                    //        break;
                    //    }
                    //    break;
                    //}
                    //Machine.IO.GetIn((int)DI.PEELING_L2_Z1_DOWN, ref ret1);
                    //Machine.IO.GetIn((int)DI.PEELING_L2_Z2_DOWN, ref ret2);
                    //if (ret1 == 0 || ret2 == 0)
                    //{
                    //    if (timeWait[(int)TIMER.TIMEOUT].Elapsed > Machine.param.Time(ParameterDefine.TIME.CYLINDER_TIME_OUT))
                    //    {
                    //        SetError(ECODE.TIMEOUT_PEELING_L2_DOWN);
                    //        break;
                    //    }
                    //    break;
                    //}
                    //Machine.IO.GetIn((int)DI.PEELING_R1_Z1_DOWN, ref ret1);
                    //Machine.IO.GetIn((int)DI.PEELING_R1_Z2_DOWN, ref ret2);
                    //if (ret1 == 0 || ret2 == 0)
                    //{
                    //    if (timeWait[(int)TIMER.TIMEOUT].Elapsed > Machine.param.Time(ParameterDefine.TIME.CYLINDER_TIME_OUT))
                    //    {
                    //        SetError(ECODE.TIMEOUT_PEELING_R1_DOWN);
                    //        break;
                    //    }
                    //    break;
                    //}
                    //Machine.IO.GetIn((int)DI.PEELING_R1_Z1_DOWN, ref ret1);
                    //Machine.IO.GetIn((int)DI.PEELING_R1_Z2_DOWN, ref ret2);
                    //if (ret1 == 0 || ret2 == 0)
                    //{
                    //    if (timeWait[(int)TIMER.TIMEOUT].Elapsed > Machine.param.Time(ParameterDefine.TIME.CYLINDER_TIME_OUT))
                    //    {
                    //        SetError(ECODE.TIMEOUT_PEELING_R1_DOWN);
                    //        break;
                    //    }
                    //    break;
                    //}

                    NextStep();
                    break;

                case STEP.AXIS_Z_ACTION:
                    DoTaskForAssignedAxisGroup(AXIS.TRAY_TRF_Z, AXIS.TRAY_WORK_Z, AXIS.JIG_IN_BUF_Z, AXIS.PROD_PICKUP_Z, AXIS.ASSEMBLER_Z);
                    timeWait[(int)TIMER.TIMEOUT].Start();
                    Step = STEP.AXIS_Z_ACTION_CHECK;
                    break;

                case STEP.AXIS_Z_ACTION_CHECK:
                    if (timeWait[(int)TIMER.TIMEOUT].Elapsed > iHomeSearchTimeOut)
                    {
                        SetError(ECODE.TIMEOUT_HOME_SEARCH_Z_GROUP);
                        break;
                    }
                    if (!IsAssignedAxisGroupHomeDone(AXIS.TRAY_TRF_Z, AXIS.TRAY_WORK_Z, AXIS.JIG_IN_BUF_Z, AXIS.PROD_PICKUP_Z, AXIS.ASSEMBLER_Z)) break;
                    NextStep();
                    break;

                case STEP.AXIS_TRANSFER_ACTION:
                    DoTaskForAssignedAxisGroup(AXIS.TRAY_TRF_Y, AXIS.CAM_TRANSFER_LEFT_Y, AXIS.CAM_TRANSFER_RIGHT_Y);
                    timeWait[(int)TIMER.TIMEOUT].Start();
                    Step = STEP.AXIS_TRANSFER_ACTION_CHECK;
                    break;

                case STEP.AXIS_TRANSFER_ACTION_CHECK:
                    if (timeWait[(int)TIMER.TIMEOUT].Elapsed > iHomeSearchTimeOut)
                    {
                        SetError(ECODE.TIMEOUT_HOME_SEARCH_TRANSFER_GROUP);
                        break;
                    }
                    if (!IsAssignedAxisGroupHomeDone(AXIS.TRAY_TRF_Y, AXIS.CAM_TRANSFER_LEFT_Y, AXIS.CAM_TRANSFER_RIGHT_Y)) break;
                    NextStep();
                    break;

                case STEP.AXIS_ASSEMBLER_ACTION:
                    DoTaskForAssignedAxisGroup(AXIS.ASSEMBLER_X, AXIS.ASSEMBLER_Y, AXIS.ASSEMBLER_R1, AXIS.ASSEMBLER_R2);
                    timeWait[(int)TIMER.TIMEOUT].Start();
                    Step = STEP.AXIS_ASSEMBLER_ACTION_CHECK;
                    break;

                case STEP.AXIS_ASSEMBLER_ACTION_CHECK:
                    if (timeWait[(int)TIMER.TIMEOUT].Elapsed > iHomeSearchTimeOut)
                    {
                        SetError(ECODE.TIMEOUT_HOME_SEARCH_ASSEMBLER_GROUP);
                        break;
                    }
                    if (!IsAssignedAxisGroupHomeDone(AXIS.ASSEMBLER_X, AXIS.ASSEMBLER_Y, AXIS.ASSEMBLER_R1, AXIS.ASSEMBLER_R2)) break;
                    NextStep();
                    break;

                case STEP.AXIS_ASSEMBLER_R_ACTION:
                    DoTaskForAssignedAxisGroup(AXIS.ASSEMBLER_R1, AXIS.ASSEMBLER_R2);
                    timeWait[(int)TIMER.TIMEOUT].Start();
                    Step = STEP.AXIS_ASSEMBLER_R_ACTION_CHECK;
                    break;
                case STEP.AXIS_ASSEMBLER_R_ACTION_CHECK:
                    if(timeWait[(int)TIMER.TIMEOUT].Elapsed > iHomeSearchTimeOut)
                    {
                        SetError(ECODE.TIMEOUT_HOME_SEARCH_ASSEMBLER_GROUP);
                        break;
                    }
                    if (!IsAssignedAxisGroupHomeDone(AXIS.ASSEMBLER_R1, AXIS.ASSEMBLER_R2)) break;
                    NextStep();
                    break;
                case STEP.AXIS_ASSEMBLER_XY_ACTION:
                    DoTaskForAssignedAxisGroup(AXIS.ASSEMBLER_X, AXIS.ASSEMBLER_Y);
                    timeWait[(int)TIMER.TIMEOUT].Start();
                    Step = STEP.AXIS_ASSEMBLER_XY_ACTION_CHECK;
                    break;
                case STEP.AXIS_ASSEMBLER_XY_ACTION_CHECK:
                    if (timeWait[(int)TIMER.TIMEOUT].Elapsed > iHomeSearchTimeOut)
                    {
                        SetError(ECODE.TIMEOUT_HOME_SEARCH_ASSEMBLER_GROUP);
                        break;
                    }
                     if (!IsAssignedAxisGroupHomeDone(AXIS.ASSEMBLER_X, AXIS.ASSEMBLER_Y)) break;
                    NextStep();
                    break;
                case STEP.AXIS_PICKER_ACTION:
                    DoTaskForAssignedAxisGroup(AXIS.PROD_PICKUP_X, AXIS.PROD_PICKUP_Y, AXIS.PROD_PICKUP_R1, AXIS.PROD_PICKUP_R2);
                    timeWait[(int)TIMER.TIMEOUT].Start();
                    Step = STEP.AXIS_PICKER_HOME_CHECK;
                    break;

                case STEP.AXIS_PICKER_HOME_CHECK:
                    if (timeWait[(int)TIMER.TIMEOUT].Elapsed > iHomeSearchTimeOut)
                    {
                        SetError(ECODE.TIMEOUT_HOME_SEARCH_PICKER_GROUP);
                        break;
                    }
                    if (!IsAssignedAxisGroupHomeDone(AXIS.PROD_PICKUP_X, AXIS.PROD_PICKUP_Y, AXIS.PROD_PICKUP_R1, AXIS.PROD_PICKUP_R2)) break;
                    NextStep();
                    break;

                case STEP.AXIS_HOME_COMPLETE:
                    this.bIsHomeRequestedNDone = true;
                    LogUtil.Instance.Log(LOG_TYPE.SYSTEM, $"STEP.{Step} / All Axis Origin Complete", CONTENT_TYPE.INFO);
                    Action action = delegate
                    {
                        Dlg_MessageBox form = new Dlg_MessageBox(EMESSAGEBOX.MSG, string.Format("Origin Completed"));
                        form.ShowDialog();
                    };
                    MainForm.mainForm.BeginInvoke(action);
                    Machine.status.state = Define.SystemState.SystemStateIDLE;
                    Machine.IO.SetOut((int)DO.BYPASS_ON, 1);
                    NextStep();
                    break;
            }

            OnProcessOfInit();
        }

        void OnProcessOfInit()
        {
            var proc_as = Machine.proclist[(int)Machine.PROCESS.ASSEMBLER] as ProcessAssembler;
            var proc_jig_inbuf = Machine.proclist[(int)Machine.PROCESS.JIGINBUF] as ProcessJigInBuf;
            var proc_jig_outbuf = Machine.proclist[(int)Machine.PROCESS.JIGOUTBUF] as ProcessJigOutBuf;
            var proc_jig_work = Machine.proclist[(int)Machine.PROCESS.JIGWORK] as ProcessJigWork;
            var proc_jig_Incarrier = Machine.proclist[(int)Machine.PROCESS.JIGINCARRIER] as ProcessJigInCarrier;
            var proc_jig_Outcarrier = Machine.proclist[(int)Machine.PROCESS.JIGOUTCARRIER] as ProcessJigOutCarrier;

            var proc_pl = Machine.proclist[(int)Machine.PROCESS.PROD_LOADER] as ProcessProdLoader;
            var proc_tray_inbuf = Machine.proclist[(int)Machine.PROCESS.TRAYINBUF] as ProcessTrayInBuf;
            var proc_tray_outbuf = Machine.proclist[(int)Machine.PROCESS.TRAYOUTBUF] as ProcessTrayOutBuf;
            var proc_tray_work = Machine.proclist[(int)Machine.PROCESS.TRAYWORK] as ProcessTrayWork;
            var proc_tray_in = Machine.proclist[(int)Machine.PROCESS.TRAYIN] as ProcessTrayIn;
            var proc_tray_out = Machine.proclist[(int)Machine.PROCESS.TRAYOUT] as ProcessTrayOut;

            var proc_buffer1 = Machine.proclist[(int)Machine.PROCESS.LEFTBUF] as ProcessBufStageOne;
            var proc_buffer2 = Machine.proclist[(int)Machine.PROCESS.RIGHTBUF] as ProcessBufStageOne;

            switch (Step)
            {
                case STEP.PROC_INIT_JIG_ASSEMBLER:
                    if (init_jig)
                    {
                        proc_as.SetMessage((int)ProcessAssembler.MSG.MSG_PROCESS_INIT);
                        Step = STEP.PROC_INIT_JIG_ASSEMBLER_CHECK;
                    }
                    else
                        NextStep();
                    break;

                case STEP.PROC_INIT_JIG_ASSEMBLER_CHECK:
                    if (proc_as.Busy() && !proc_as.Error())
                        break;

                    NextStep();
                    break;

                case STEP.PROC_INIT_JIG_CONV:
                    if (init_jig)
                    {
                        Step = STEP.PROC_INIT_JIG_CONV_CHECK;
                    }
                    else
                        NextStep();
                    break;

                case STEP.PROC_INIT_JIG_CONV_CHECK:
                    if (proc_jig_inbuf.Busy() && !proc_jig_inbuf.Error())
                        break;
                    if (proc_jig_outbuf.Busy() && !proc_jig_outbuf.Error())
                        break;
                    if (proc_jig_work.Busy() && !proc_jig_work.Error())
                        break;
                    if (proc_jig_Incarrier.Busy() && !proc_jig_Incarrier.Error())
                        break;
                    if (proc_jig_Outcarrier.Busy() && !proc_jig_Outcarrier.Error())
                        break;

                    NextStep();
                    break;

                case STEP.PROC_INIT_TRAY_PROD_LOADER:
                    if (init_prodLoader)
                    {
                        proc_pl.SetMessage((int)ProcessProdLoader.MSG.MSG_PROCESS_INIT);
                        Step = STEP.PROC_INIT_TRAY_PROD_LOADER_CHECK;
                    }
                    else
                        NextStep();
                    break;

                case STEP.PROC_INIT_TRAY_PROD_LOADER_CHECK:
                    if (proc_pl.Busy() && !proc_pl.Error())
                        break;

                    NextStep();
                    break;

                case STEP.PROC_INIT_TRAY_CONV:
                    if (init_tray)
                    {
                        proc_tray_inbuf.SetMessage((int)ProcessTrayInBuf.MSG.MSG_PROCESS_INIT);
                        proc_tray_outbuf.SetMessage((int)ProcessTrayOutBuf.MSG.MSG_PROCESS_INIT);
                        proc_tray_work.SetMessage((int)ProcessTrayWork.MSG.MSG_PROCESS_INIT);
                        proc_tray_in.SetMessage((int)ProcessTrayIn.MSG.MSG_PROCESS_INIT);
                        proc_tray_out.SetMessage((int)ProcessTrayOut.MSG.MSG_PROCESS_INIT);

                        Step = STEP.PROC_INIT_TRAY_CONV_CHECK;
                    }
                    else
                        NextStep();
                    break;

                case STEP.PROC_INIT_TRAY_CONV_CHECK:
                    if (proc_tray_inbuf.Busy() && !proc_tray_inbuf.Error())
                        break;
                    if (proc_tray_outbuf.Busy() && !proc_tray_outbuf.Error())
                        break;
                    if (proc_tray_work.Busy() && !proc_tray_work.Error())
                        break;
                    if (proc_tray_in.Busy() && !proc_tray_in.Error())
                        break;
                    if (proc_tray_out.Busy() && !proc_tray_out.Error())
                        break;

                    NextStep();
                    break;

                case STEP.PROC_INIT_BUFFER:
                    if (init_buffer)
                    {
                        proc_buffer1.SetMessage((int)ProcessBufStageOne.MSG.MSG_PROCESS_INIT);
                        proc_buffer2.SetMessage((int)ProcessBufStageOne.MSG.MSG_PROCESS_INIT);

                        Step = STEP.PROC_INIT_BUFFER_CHECK;
                    }
                    else
                        NextStep();
                    break;

                case STEP.PROC_INIT_BUFFER_CHECK:
                    if (proc_buffer1.Busy() && !proc_buffer1.Error())
                        break;
                    if (proc_buffer2.Busy() && !proc_buffer2.Error())
                        break;

                    NextStep();
                    break;

                case STEP.PROC_INIT_COMPLETE:
                    Action action = delegate
                    {
                        Dlg_MessageBox form = new Dlg_MessageBox(EMESSAGEBOX.MSG, string.Format("Initialize Completed"));
                        form.ShowDialog();
                    };
                    MainForm.mainForm.BeginInvoke(action);
                    Machine.status.state = Define.SystemState.SystemStateIDLE;
                    NextStep();
                    break;
            }
        }

        public override void SetHeadTarget(int iTarget)
        {
            throw new NotImplementedException();
        }

        private void DoTaskForAssignedAxisGroup(params AXIS[] eAxisArray)
        {
            foreach (AXIS eAxis in eAxisArray)
            {
                if (!SelectAxis[(int)eAxis]) continue;
                LogUtil.Instance.Log(LOG_TYPE.SYSTEM, $"STEP.{Step} / {eAxis}: {eServoTask} Start", CONTENT_TYPE.INFO);

                if (eServoTask == SERVO_TASK.SERVO_ON || eServoTask == SERVO_TASK.ORIGIN || eServoTask == SERVO_TASK.ALARM_CLEAR)
                {
                    Machine.motion.AlarmClear((int)eAxis);
                }
                if (eServoTask == SERVO_TASK.SERVO_ON || eServoTask == SERVO_TASK.ORIGIN)
                {
                    Util.Delay(iDelayPerTask);
                    Machine.motion.SetAxisServoOn((int)eAxis, 1);
                }
                else if (eServoTask == SERVO_TASK.SERVO_OFF)
                    Machine.motion.SetAxisServoOn((int)eAxis, 0);


                if (eServoTask == SERVO_TASK.ORIGIN)
                {
                    Util.Delay(iDelayPerTask);
                    if (eAxis == AXIS.ASSEMBLER_R1 || eAxis == AXIS.ASSEMBLER_R2
                        || eAxis == AXIS.PROD_PICKUP_R1 || eAxis == AXIS.PROD_PICKUP_R2)
                    {
                        for (int iIndex = 0; iIndex < RAxisList.Length; iIndex++)
                        {
                            if (RAxisList[iIndex] == (int)eAxis)
                                HomeRProcess[iIndex].SetMessage((int)ProcessHomeRAxis.MSG.MSG_HOME_START);
                        }
                    }
                    else
                    {
                        Machine.motion.SetAxisHomeStart((int)eAxis);
                    }
                }
            }
        }

        private bool IsAssignedAxisGroupHomeDone(params AXIS[] eAxisArray)
        {
            if (eServoTask != SERVO_TASK.ORIGIN) return true;
            HomeState homeState = HomeState.HomeStateDONE;
            foreach (AXIS eAxis in eAxisArray)
            {
                if (!SelectAxis[(int)eAxis]) continue;

                if (eAxis == AXIS.ASSEMBLER_R1 || eAxis == AXIS.ASSEMBLER_R2 || eAxis == AXIS.PROD_PICKUP_R1 || eAxis == AXIS.PROD_PICKUP_R2)
                {
                    for (int iIndex = 0; iIndex < RAxisList.Length; iIndex++)
                    {
                        if (RAxisList[iIndex] == (int)eAxis)
                        {
                            if (HomeRProcess[iIndex].Busy()) return false;
                        }
                    }
                }
                else
                {
                    Machine.motion.GetAxisHomeComplete((int)eAxis, ref homeState);
                    if (homeState != HomeState.HomeStateDONE) return false;
                }
            }
            foreach (AXIS eAxis in eAxisArray)
            {
                if (!SelectAxis[(int)eAxis]) continue;
                LogUtil.Instance.Log(LOG_TYPE.SYSTEM, $"STEP.{Step} / {eAxis} Home Complete", CONTENT_TYPE.INFO);
            }
            return true;
        }

        public void SafetyReset()
        {
            Machine.IO.SetOut((int)DO.SAFETY_RESET, 1);
            Util.Delay(500);
            Machine.IO.SetOut((int)DO.SAFETY_RESET, 0);
            Util.Delay(100);
            if (!Machine.DoorOpenDisregard) Machine.IO.SetOut((int)DO.BYPASS_ON, 0);
        }
    }
}
