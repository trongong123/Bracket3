using TopEng.Device;
using TopEng.Module;
using CAMASSEMBLYMACHINE.Define;
using TopEng.Utils;
using static CAMASSEMBLYMACHINE.Define.SystemDefine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CAMASSEMBLYMACHINE.UI;
using Microsoft.Win32;
using System.Windows.Forms;
using TopEng.Controls;
using static CAMASSEMBLYMACHINE.Machine;

namespace CAMASSEMBLYMACHINE.Process
{
    public class ProcessSys : IProcess
    {
        private TOWERLAMP towerlampControl = null;
        private BUZZER buzzerControl = null;
        public OPPANEL[] oppanelControl = new OPPANEL[2];
        private StopWatch timeWait2 = new StopWatch();
        private StopWatch timeWait_product_update = new StopWatch();
        private StopWatch timeWait_set_count_update = new StopWatch();
        private DateTime edmLogLastTime = DateTime.Now;

        public bool autoRunning = false;
        public bool errorState = false;
        public bool towerLampOff = false;
        public bool doorOpenState = false;
        public bool doorErrorState = false;
        public bool lightcurtainOn = false;
        public bool lightcurtainError = false;
        public bool mutingOn = false;
        public bool FrontEmergencyOn = false;
        public bool RearEmergencyOn = false;

        SystemState currState = SystemState.SystemStateIDLE;
        public bool refreshState = true;
        public bool ResetState = true;

        public string operateGroup = "SHIFT_A";
        private DateTime ShiftAStartTime;
        private DateTime ShiftCStartTime;
        private DateTime ShiftAResetTime;
        private DateTime ShiftCResetTime;
        private DateTime ShiftAResetEndTime;
        private DateTime ShiftCResetEndTime;
        private DateTime ShiftATactResetTime;
        private DateTime ShiftATactResetEndTime;
        private DateTime ShiftCTactResetTime;
        private DateTime ShiftCTactResetEndTime;

        public override Type StepEnumType => null;
        public override Type MsgEnumType => null;

        public ProcessSys(IIO IO)
        {
            // 타워램프 초기화
            int r = (int)DO.TOWER_LAMP_RED;
            int y = (int)DO.TOWER_LAMP_YELLOW;
            int g = (int)DO.TOWER_LAMP_GREEN;
            towerlampControl = new TOWERLAMP(IO, r, y, g, TOWERLAMP.TOWERLAMPTYPE.RYG_3COLORS);

            // 버저 초기화
            int bzAlarm = (int)DO.BUZZER_ON;
            int s2 = -1;
            int s3 = -1;
            int s4 = -1;
            buzzerControl = new BUZZER(IO, bzAlarm, s2, s3, s4);

            // OPPANEL1
            int opiStart = (int)DI.START_SW_OP1;
            int opiStop = (int)DI.STOP_SW_OP1;
            int opiReset = (int)DI.RESET_SW_OP1;
            //int opiPowerOn = (int)DI.POWER_ON;
            //int opiEmergency = (int)DI.EMG_STOP_OP1;
            oppanelControl[0] = new OPPANEL(IO);
            oppanelControl[0].SetInIO(opiStart, opiStop, opiReset);
            int opoStart = (int)DO.START_SW_LAMP_OP1;
            int opoStop = (int)DO.STOP_SW_LAMP_OP1;
            int opoReset = (int)DO.RESET_SW_LAMP_OP1;
            oppanelControl[0].SetOutIO(opoStart, opoStop, opoReset);
            oppanelControl[0].StartButtonClicked += new OPPANEL.cbStartButtonEvent(StartButtonEvent);
            oppanelControl[0].StopButtonClicked += new OPPANEL.cbStopButtonEvent(StopButtonEvent);
            oppanelControl[0].ResetButtonClicked += new OPPANEL.cbResetButtonEvent(ResetButtonEvent);

            // OPPANEL2
            int opiStart2 = (int)DI.START_SW_OP2;
            int opiStop2 = (int)DI.STOP_SW_OP2;
            int opiReset2 = (int)DI.RESET_SW_OP2;
            oppanelControl[1] = new OPPANEL(IO);
            oppanelControl[1].SetInIO(opiStart2, opiStop2, opiReset2);
            int opoStart2 = (int)DO.START_SW_LAMP_OP2;
            int opoStop2 = (int)DO.STOP_SW_LAMP_OP2;
            int opoReset2 = (int)DO.RESET_SW_LAMP_OP2;
            oppanelControl[1].SetOutIO(opoStart2, opoStop2, opoReset2);
            oppanelControl[1].StartButtonClicked += new OPPANEL.cbStartButtonEvent(StartButtonEvent);
            oppanelControl[1].StopButtonClicked += new OPPANEL.cbStopButtonEvent(StopButtonEvent);
            oppanelControl[1].ResetButtonClicked += new OPPANEL.cbResetButtonEvent(ResetButtonEvent);

            foreach (var oppanel in oppanelControl)
            {
                oppanel.mutingOnDisregard = Machine.MutingOnDisregard;
            }
        }

        public override int GetStep() { return 0; }
        public override int GetAutoStep() { return 0; }

        public override bool Busy() { return false; }
        public override bool Ready() { return true; }
        public override bool Stopped() { return false; }
        public override bool Error() { return false; }

        public override void TimeOut() { }
        public override void SetMessage(int message, int step = 0) { }
        public override void AutoStart() { }
        public override void EStop(bool pause) { }
        public override void Stop() { }

        public void BuzzerOff()
        {
            buzzerControl.Ready();
        }

        public void BuzzerAlarm()
        {
            if (Machine.sysMode != Machine.SYSMODE.AUTO || Machine.status.state == SystemState.SystemStateIDLE)
                return;

            buzzerControl.Error();
        }

        public void BuzzerOn(BUZZER.BUZZERELEM sound)
        {
            if (FrontEmergencyOn || RearEmergencyOn)
                buzzerControl.Error();
            else
                buzzerControl.Ready();

            //if (Machine.sysMode != Machine.SYSMODE.AUTO || Machine.status.state == SystemState.SystemStateIDLE)
            //    return;

            if (sound == BUZZER.BUZZERELEM.ALARM) buzzerControl.Error();
            if (sound == BUZZER.BUZZERELEM.NOSOUND) buzzerControl.Ready();
        }

        protected override void OnProcessing()
        {
            SafetyStateCheck(); // DOOR , LIGHT CURTAIN
            MotionStateCheck();
            WatchDogInterference();
            DisplaySystemStateIDLE();
            DisplaySystemState();


            //if (!Machine.motion.IsconnectAllSlave())
            //{
            //    Machine.Alarm(ECODE.MOTION_DISCONNECTED);
            //    currState = SystemState.SystemStateERROR;
            //}

            //if (Machine.status.state == SystemState.SystemStateERROR && !Machine.motion.IsconnectAllSlave())
            //    return;

            AutonRunning();
            WorkingTimeCheck();
            UpdateEDMData();

            refreshState = false;
        }

        private void DisplaySystemStateIDLE()
        {
            if (Machine.status.state != SystemState.SystemStateIDLE)
                return;

            if (!towerLampOff && doorOpenState)
                towerlampControl.Alarm();
            else
                towerlampControl.Ready();
        }

        public void DisplaySystemState()
        {
            bool report = Machine.param.Option(ParameterDefine.OPTION.REPORT_EDM_SERVER) == 1 ? true : false;

            if (Machine.status.state != currState || refreshState || FrontEmergencyOn || RearEmergencyOn)
            {
                if (Machine.status.state == SystemState.SystemStateIDLE)
                {
                    if (!towerLampOff)
                        towerlampControl.Ready();
                    BuzzerOn(BUZZER.BUZZERELEM.NOSOUND);

                    if (!refreshState && currState != SystemState.SystemStateRUNEMPTY && Machine.sysMode == Machine.SYSMODE.AUTO)
                        EDM.TowerLampChanged();
                }
                if (Machine.status.state == SystemState.SystemStateERROR)
                {
                    if (!towerLampOff)
                        towerlampControl.Alarm();
                    BuzzerOn(BUZZER.BUZZERELEM.ALARM);

                    if (!refreshState && Machine.sysMode == Machine.SYSMODE.AUTO)
                        EDM.TowerLampChanged();
                }
                if (Machine.status.state == SystemState.SystemStateRUN)
                {
                    if (!towerLampOff)
                        towerlampControl.Run();
                    if (!refreshState && Machine.sysMode == Machine.SYSMODE.AUTO)
                        EDM.TowerLampChanged();
                }
                if (Machine.status.state == SystemState.SystemStateRUNEMPTY)
                {
                    if (!towerLampOff)
                        towerlampControl.RunEmpty();
                    if (!refreshState && Machine.sysMode == Machine.SYSMODE.AUTO)
                        EDM.TowerLampChanged();
                }
                if (Machine.status.state == SystemState.SystemStateINIT)
                {
                    if (!towerLampOff)
                        towerlampControl.Alarm();
                    if (!refreshState && Machine.sysMode == Machine.SYSMODE.AUTO)
                        EDM.TowerLampChanged();
                }
                if (Machine.status.state == SystemState.SystemStateSLEEP)
                {
                    if (!towerLampOff)
                        towerlampControl.Sleep();
                }

                currState = Machine.status.state;
            }

            else
            {
                if (currState == SystemState.SystemStateRUN)
                {
                    if (!towerLampOff)
                        towerlampControl.CheckEmptyTrayInBuf();
                }
            }

            if (towerLampOff)
                towerlampControl.Off();
        }

        public void SafetyStateCheck()
        {
            if (SystemDefine.IS_NOTEBOOK_MODE) return;
            uint returnValue = 0;
            uint returnValue2 = 0;

            // EMVERGENCY SWITCH
            Machine.IO.GetIn((int)DI.EMO_SW_OP1, ref returnValue);
            if (returnValue == 1)
            {
                Machine.Alarm(ECODE.EMEGENCY_STOP1_ON);
                FrontEmergencyOn = true;
                Machine.status.state = SystemState.SystemStateERROR;
            }
            else
            {
                Machine.AlarmRemove(ECODE.EMEGENCY_STOP1_ON);
                FrontEmergencyOn = false;
            }
            Machine.IO.GetIn((int)DI.EMO_SW_OP2, ref returnValue);
            if (returnValue == 1)
            {
                Machine.Alarm(ECODE.EMEGENCY_STOP2_ON);
                RearEmergencyOn = true;
                Machine.status.state = SystemState.SystemStateERROR;
            }
            else
            {
                Machine.AlarmRemove(ECODE.EMEGENCY_STOP2_ON);
                RearEmergencyOn = false;
            }
            // LIGHT CURTAIN
            bool LightCurtainOn = false;
            returnValue = 0;
            returnValue2 = 0;

            Machine.IO.GetIn((int)DI.LIGHT_CURTAIN_IN, ref returnValue);
            if (returnValue == 1 && !Machine.DoorOpenDisregard)
            {
                if (Machine.status.state == SystemState.SystemStateRUN
                    || Machine.status.state == SystemState.SystemStateINIT
                    || Machine.Busy()
                    || lightcurtainError)
                {
                    if (!FrontEmergencyOn && !RearEmergencyOn)
                    {
                        Machine.Alarm(ECODE.LIGHT_CURTAIN1_SENSED_SOMETHING);
                        LightCurtainOn = true;

                        if (!lightcurtainError)
                            lightcurtainError = true;
                    }
                }
            }
            else
            {
                Machine.AlarmRemove(ECODE.LIGHT_CURTAIN1_SENSED_SOMETHING);
            }
            Machine.IO.GetIn((int)DI.LIGHT_CURTAIN_OUT, ref returnValue2);
            if (returnValue2 == 1 && !Machine.DoorOpenDisregard)
            {
                if (Machine.status.state == SystemState.SystemStateRUN
                    || Machine.status.state == SystemState.SystemStateINIT
                    || Machine.Busy()
                    || lightcurtainError)
                {
                    if (!FrontEmergencyOn && !RearEmergencyOn)
                    {
                        Machine.Alarm(ECODE.LIGHT_CURTAIN2_SENSED_SOMETHING);
                        LightCurtainOn = true;

                        if (!lightcurtainError)
                            lightcurtainError = true;
                    }
                }
            }
            else
            {
                Machine.AlarmRemove(ECODE.LIGHT_CURTAIN2_SENSED_SOMETHING);
            }

            if (returnValue == 0 && returnValue2 == 0)
                lightcurtainError = false;

            if (lightcurtainOn != LightCurtainOn)
            {
                lightcurtainOn = LightCurtainOn;
                if (!towerLampOff && LightCurtainOn)
                    towerlampControl.Alarm();
                //else
                //    refreshState = true;
            }

            // DOOR OPEN CHECK
            bool DoorOpen = false;

            int[] inputIOList =
            {
                (int)DI.DOOR_FRONT_TOP1, (int)DI.DOOR_FRONT_TOP2,
                (int)DI.DOOR_REAR_TOP1, (int)DI.DOOR_REAR_TOP2,
                //(int)DI.DOOR_LEFT_TOP1, (int)DI.DOOR_LEFT_TOP2, 
                //(int)DI.DOOR_LEFT_TOP3, (int)DI.DOOR_LEFT_TOP4
            };

            ECODE[] errorList =
            {
                ECODE.DOOR_FRONT_TOP1_OPEN, ECODE.DOOR_FRONT_TOP2_OPEN,
                ECODE.DOOR_REAR_TOP1_OPEN, ECODE.DOOR_REAR_TOP2_OPEN,
                //ECODE.DOOR_SIDE_TOP1_OPEN, ECODE.DOOR_SIDE_TOP2_OPEN, 
                //ECODE.DOOR_SIDE_TOP3_OPEN, ECODE.DOOR_SIDE_TOP4_OPEN
            };

            for (int i = 0; i < inputIOList.Length; i++)
            {
                Machine.IO.GetIn(inputIOList[i], ref returnValue);
                if (returnValue == 0 && !Machine.DoorOpenDisregard)
                {
                    if (Machine.status.state == SystemState.SystemStateRUN
                        || Machine.status.state == SystemState.SystemStateINIT
                        || Machine.Busy()
                        || doorErrorState)
                    {
                        if (!FrontEmergencyOn && !RearEmergencyOn)
                        {
                            Machine.Alarm(errorList[i]);
                            if (!doorErrorState)
                                doorErrorState = true;
                        }
                    }
                    DoorOpen = true;
                    break;
                }
                else if (returnValue == 1)
                {
                    Machine.AlarmRemove(errorList[i]);
                }
            }

            doorOpenState = DoorOpen;
            Machine.CheckStateBtnDoorOpen(doorOpenState);
            if (doorOpenState)
            {
                if (Machine.usingMoveJog)
                {
                    //If Door Opened In Teahcing Must Jog Stop
                    Machine.motion.AxisStopAll(true);
                    Machine.usingMoveJog = false;
                }
            }
            else if (doorErrorState)
                doorErrorState = false;
        }

        public bool MotionStateCheck(bool error_check = false)
        {
            if (!Machine.Busy() && !error_check)
                return true;

            if (Machine.proclist[(int)PROCESS.HOME].Busy()) return true;
            if (!Machine.Busy() && !error_check)
                return true;

            bool alarmOn = false;
            bool servoOn = false;
            HomeState homeState = HomeState.HomeStateNEED;
            bool limitOn = false;

            for (int i = 0; i < (int)AXIS.AXISMAX; i++)
            {
                Machine.motion.GetAlarm(i, ref alarmOn);

                if (alarmOn)
                {
                    Machine.Alarm(ECODE.SERVO_ALARM_AXIS0 + i);
                    if (error_check) return false;
                }

                Machine.motion.GetAxisServoOn(i, ref servoOn);
                if (!servoOn)
                {
                    Machine.Alarm(ECODE.SERVO_OFF_AXIS0 + i);
                    if (error_check) return false;
                }

                if (Machine.proclist[(int)Machine.PROCESS.HOME].Busy())
                    continue;

                Machine.motion.GetAxisHW_NEGLimit(i, ref limitOn);
                if (limitOn)
                {
                    Machine.Alarm(ECODE.SERVO_HW_NEG_LIMIT_AXIS0 + i);
                    if (error_check) return false;
                }

                Machine.motion.GetAxisHW_POSLimit(i, ref limitOn);
                if (limitOn)
                {
                    Machine.Alarm(ECODE.SERVO_HW_POS_LIMIT_AXIS0 + i);
                    if (error_check) return false;
                }
            }

            return true;
        }

        public void AutonRunning()
        {
            if (Machine.status.state == SystemState.SystemStateRUN || Machine.status.state == SystemState.SystemStateRUNEMPTY || Machine.status.state == SystemState.SystemStateSLEEP)
            {
                foreach (var panel in oppanelControl)
                    panel.Start();
            }
            if (Machine.status.state == SystemState.SystemStateIDLE)
            {
                foreach (var panel in oppanelControl)
                    panel.Stop();
            }
            if (Machine.status.state == SystemState.SystemStateERROR)
            {
                foreach (var panel in oppanelControl)
                    panel.Reset();
            }

            foreach (var panel in oppanelControl)
            {
                panel.OnStartButtonClicked();
                panel.OnStopButtonClicked();
                panel.OnResetButtonClicked();
            }

            if (Machine.status.state == SystemState.SystemStateERROR)
                return;

            bool running = false;
            bool error = false;

            foreach (var proc in Machine.proclist)
            {
                if (proc.Busy())
                    running = true;
                if (proc.Error() || Program.errorProc.alarm.hasError())
                    error = true;
                if (FrontEmergencyOn || RearEmergencyOn)
                    error = true;
            }

            if (error)
                Machine.status.state = SystemState.SystemStateERROR;

            if (running && !error)
            {
                if (((ProcessHOME)(Machine.proclist[(int)Machine.PROCESS.HOME])).Busy())
                {
                    Machine.status.state = SystemState.SystemStateINIT;
                }
                else if (Machine.sysMode == Machine.SYSMODE.AUTO)
                {
                    if (Machine.EmptyProduct() && timeWait2.Elapsed > 60 * 1000)
                    {
                        if (Machine.status.state != SystemState.SystemStateSLEEP)
                            Machine.status.state = SystemState.SystemStateRUNEMPTY;
                    }
                    else
                    {
                        if (Machine.status.state != SystemState.SystemStateSLEEP)
                            Machine.status.state = SystemState.SystemStateRUN;
                        if (!Machine.EmptyProduct())
                            timeWait2.Reset();
                    }
                }
            }
            else if (!error)
            {
                Machine.status.state = SystemState.SystemStateIDLE;
                timeWait2.Reset();
            }

            if (Machine.sysMode != Machine.SYSMODE.AUTO)
                return;


            if (autoRunning != running || refreshState)
            {
                autoRunning = running;

                foreach (var panel in oppanelControl)
                {
                    if (autoRunning)
                        panel.Start();
                    else
                        panel.Stop();
                }
            }
        }

        private void WorkingTimeCheck()
        {
            DateTime now = DateTime.Now;

            if (timeWait[(int)TIMER.STATUS].Elapsed > 60 * 1000)
            {
                ResetState = false;
            }

            if (ResetState || Machine.status.state == SystemState.SystemStateIDLE)
            {
                ShiftAStartTime = new DateTime(now.Year, now.Month, now.Day,
                    Machine.param.workingtime["A_WORK_START"].Hour,
                    Machine.param.workingtime["A_WORK_START"].Minute,
                    Machine.param.workingtime["A_WORK_START"].Second);
                ShiftCStartTime = new DateTime(now.Year, now.Month, now.Day,
                    Machine.param.workingtime["C_WORK_START"].Hour,
                    Machine.param.workingtime["C_WORK_START"].Minute,
                    Machine.param.workingtime["C_WORK_START"].Second);
                ShiftAResetTime = new DateTime(now.Year, now.Month, now.Day,
                    Machine.param.workingtime["A_RESET1_START"].Hour,
                    Machine.param.workingtime["A_RESET1_START"].Minute,
                    Machine.param.workingtime["A_RESET1_START"].Second);
                ShiftCResetTime = new DateTime(now.Year, now.Month, now.Day,
                    Machine.param.workingtime["C_RESET1_START"].Hour,
                    Machine.param.workingtime["C_RESET1_START"].Minute,
                    Machine.param.workingtime["C_RESET1_START"].Second);
                ShiftAResetEndTime = new DateTime(now.Year, now.Month, now.Day,
                    Machine.param.workingtime["A_RESET1_END"].Hour,
                    Machine.param.workingtime["A_RESET1_END"].Minute,
                    Machine.param.workingtime["A_RESET1_END"].Second);
                ShiftCResetEndTime = new DateTime(now.Year, now.Month, now.Day,
                    Machine.param.workingtime["C_RESET1_END"].Hour,
                    Machine.param.workingtime["C_RESET1_END"].Minute,
                    Machine.param.workingtime["C_RESET1_END"].Second);
                ShiftATactResetTime = new DateTime(now.Year, now.Month, now.Day,
                   Machine.param.workingtime["A_TACTRESET_START"].Hour,
                   Machine.param.workingtime["A_TACTRESET_START"].Minute,
                   Machine.param.workingtime["A_TACTRESET_START"].Second);
                ShiftCTactResetTime = new DateTime(now.Year, now.Month, now.Day,
                  Machine.param.workingtime["C_TACTRESET_START"].Hour,
                  Machine.param.workingtime["C_TACTRESET_START"].Minute,
                  Machine.param.workingtime["C_TACTRESET_START"].Second);
                ShiftATactResetEndTime = new DateTime(now.Year, now.Month, now.Day,
                  Machine.param.workingtime["A_TACTRESET_END"].Hour,
                  Machine.param.workingtime["A_TACTRESET_END"].Minute,
                  Machine.param.workingtime["A_TACTRESET_END"].Second);
                ShiftCTactResetEndTime = new DateTime(now.Year, now.Month, now.Day,
                  Machine.param.workingtime["C_TACTRESET_END"].Hour,
                  Machine.param.workingtime["C_TACTRESET_END"].Minute,
                  Machine.param.workingtime["C_TACTRESET_END"].Second);

                //if ((now - ShiftAStartTime).TotalSeconds > 0)
                //    ShiftAStartTime = ShiftAStartTime.AddDays(1);
                //if ((now - ShiftCStartTime).TotalSeconds > 0)
                //    ShiftAStartTime = ShiftCStartTime.AddDays(1);
                //if ((now - ShiftAResetTime).TotalSeconds > 0)
                //    ShiftAResetTime = ShiftAResetTime.AddDays(1);
                //if ((now - ShiftCResetTime).TotalSeconds > 0)
                //    ShiftAResetTime = ShiftCResetTime.AddDays(1);
                //if ((now - ShiftAResetEndTime).TotalSeconds > 0)
                //    ShiftAResetEndTime = ShiftAResetEndTime.AddDays(1);
                //if ((now - ShiftCResetEndTime).TotalSeconds > 0)
                //    ShiftAResetEndTime = ShiftCResetEndTime.AddDays(1);

                if (now <= ShiftCStartTime && now >= ShiftAStartTime)
                    operateGroup = "SHIFT_A";
                else if (now >= ShiftCStartTime && now <= ShiftAStartTime)
                    operateGroup = "SHIFT_C";
            }

            if (now <= ShiftAResetEndTime && now >= ShiftAResetTime && !ResetState)
            {
                operateGroup = "SHIFT_A";
                ResetState = true;
                //ShiftAStartTime = ShiftAStartTime.AddDays(1);
                timeWait[(int)TIMER.STATUS].Reset();
                Machine.ClearProductCount();
                Vision.DeleteCaptureFile();
            }
            else if (now <= ShiftCResetEndTime && now >= ShiftCResetTime && !ResetState)
            {
                operateGroup = "SHIFT_C";
                ResetState = true;
                //ShiftCStartTime = ShiftCStartTime.AddDays(1);
                timeWait[(int)TIMER.STATUS].Reset();
                Machine.ClearProductCount();
                Vision.DeleteCaptureFile();
            }
            if (now <= ShiftATactResetEndTime && now >= ShiftATactResetTime && !ResetState)
            {
                operateGroup = "SHIFT_A";
                ResetState = true;
                timeWait[(int)TIMER.TACT].Reset();
                timeWait[(int)TIMER.STATUS].Reset();
                Machine.ClearTacTimeInfo();
            }
            else if (now <= ShiftCTactResetEndTime && now >= ShiftCTactResetTime && !ResetState)
            {
                operateGroup = "SHIFT_C";
                ResetState = true;
                timeWait[(int)TIMER.TACT].Reset();
                timeWait[(int)TIMER.STATUS].Reset();
                Machine.ClearTacTimeInfo();
            }
        }

        private void UpdateEDMData()
        {
            if (timeWait_product_update.Elapsed > 1000)
            {
                uint input_product = (uint)Machine.info.Product(StatusDefine.PRODUCT.PRODUCT_INPUT_PARTS_COUNT);
                uint alarm_limit = (uint)Machine.info.Product(StatusDefine.PRODUCT.PRODUCT_INPUT_ALARM_LIMIT);
                uint total_product = (uint)Machine.info.Product(StatusDefine.PRODUCT.PRODUCT_INPUT_TOTAL_COUNT);

                //EDM.PartCount(input_product, total_product, alarm_limit, report);

                timeWait_product_update.Reset();
            }

            if (timeWait_set_count_update.Elapsed >= 59 * 1000)
            {
                int set_counts = Machine.Jigs[(int)UNITJIG.JIG_WORK].productCount + Machine.Jigs[(int)UNITJIG.JIG_OUT_BUF].productCount;
                EDM.SetsCount((uint)set_counts);

                timeWait_set_count_update.Reset();
            }

            if (DateTime.Now.Minute == 0 && DateTime.Now.Second == 0 && DateTime.Now.Hour != edmLogLastTime.Hour)
            {
                int maxPartIn_X_Left = (int)Machine.recipe.Option(RecipeDefine.OPTION.MAX_PART_IN_LEFT_NG_BOX_X);
                int maxPartIn_Y_Left = (int)Machine.recipe.Option(RecipeDefine.OPTION.MAX_PART_IN_LEFT_NG_BOX_Y);
                int maxPartIn_X_Right = (int)Machine.recipe.Option(RecipeDefine.OPTION.MAX_PART_IN_RIGHT_NG_BOX_X);
                int maxPartIn_Y_Right = (int)Machine.recipe.Option(RecipeDefine.OPTION.MAX_PART_IN_RIGHT_NG_BOX_Y);

                edmLogLastTime = DateTime.Now;
                var proc1 = Machine.proclist[(int)Machine.PROCESS.ASSEMBLER] as ProcessAssembler;
                int currentCount = proc1.currentNGCount[0] + proc1.currentNGCount[1];
                int maxCount = maxPartIn_X_Left * maxPartIn_Y_Left + maxPartIn_X_Right * maxPartIn_Y_Right;
                EDM.SetNGBoxStatus(currentCount, maxCount, false);
            }
        }

        private void StartButtonEvent()
        {
            LogUtil.Instance.Log(LOG_TYPE.SYSTEM, "Start Button Click", CONTENT_TYPE.INFO);
            Machine.Start();
        }

        private void StopButtonEvent()
        {
            LogUtil.Instance.Log(LOG_TYPE.SYSTEM, "Stop Button Click", CONTENT_TYPE.INFO);
            Machine.EStop(false, false);
        }

        private void ResetButtonEvent()
        {
            LogUtil.Instance.Log(LOG_TYPE.SYSTEM, "Reset Button Click", CONTENT_TYPE.INFO);

            Action action = delegate
            {
                for (int i = 0; i < Dlg_MessageBox.listMsgBox.Count; ++i)
                {
                    Dlg_MessageBox dlg = Dlg_MessageBox.listMsgBox[i];
                    dlg.DialogResult = DialogResult.Yes;
                    dlg.Close();
                }
                Dlg_MessageBox.listMsgBox.Clear();
            };
            MainForm.mainForm.BeginInvoke(action);
            Machine.AlarmClear();
        }

        public void GetResetButtonEvent()
        {
            ResetButtonEvent();
        }

        private void MuteButtonEvent()
        {
            //Machine.IO.SetOut((int)DO.BYPASS_ON, 1);
            //mutingOn = true;
        }

        private void MuteOffvent()
        {
            //Machine.IO.SetOut((int)DO.BYPASS_ON, 0);
            //mutingOn = false;
        }

        private void TrayoutButtonEvent(int id)
        {
            if (Machine.status.state != SystemState.SystemStateIDLE || Machine.Busy())
                return;
        }

        private void WatchDogInterference()
        {
            if (Machine.proclist[(int)Machine.PROCESS.HOME].Busy())
                return;

            MotionState state = MotionState.MotionStateIDLE;

            uint interferenceSensor = 0;
            Machine.IO.GetIn((int)DI.NO_INTERFERENCE_PICKER_Y, ref interferenceSensor);

            if (interferenceSensor == 0 && (!Machine.interfer_release_y_crash || Machine.interfer_jog_y_crash))
            {
                Machine.motion.GetAxisMotionState((int)AXIS.PROD_PICKUP_Y, ref state);
                if (state == MotionState.MotionStateMOVING)
                    Machine.Alarm(ECODE.ASSEMBLER_AND_PROD_PICKER_INTERFERENCE);

                Machine.motion.GetAxisMotionState((int)AXIS.ASSEMBLER_Y, ref state);
                if (state == MotionState.MotionStateMOVING)
                    Machine.Alarm(ECODE.ASSEMBLER_AND_PROD_PICKER_INTERFERENCE);
            }

            double actualTrfY = 0;
            Machine.motion.GetAxisActualPos((int)AXIS.TRAY_TRF_Y, ref actualTrfY);
            double unloadingPos = 1000 * Machine.param.Position(ParameterDefine.POSITION.TRAY_TRF_Y_UNLOADING_POS);
            double readyPos = 1000 * Machine.param.Position(ParameterDefine.POSITION.TRAY_TRF_Y_READY_POS);
            if (Math.Abs(actualTrfY - unloadingPos) >= 300 && Math.Abs(actualTrfY - readyPos) >= 300)
                Machine.trayTransferReady = false;
            else
                Machine.trayTransferReady = true;


            double actProdPickupY = 0;
            Machine.motion.GetAxisActualPos((int)AXIS.PROD_PICKUP_Y, ref actProdPickupY);
            double actUnloaderX = 0;
            Machine.motion.GetAxisActualPos((int)AXIS.PROD_PICKUP_X, ref actUnloaderX);
            double relPos = 1000 * Machine.param.Interference(ParameterDefine.INTERFERENCE.PROD_LOADER_INF_X_RELEASE_Y_POS);
            double posX_R = 1000 * Machine.param.Interference(ParameterDefine.INTERFERENCE.PROD_LOADER_INF_X_R_POS);
            double posX_L = 1000 * Machine.param.Interference(ParameterDefine.INTERFERENCE.PROD_LOADER_INF_X_L_POS);

            if (Machine.interfer_release_x_safe_area)
            {
                if (actProdPickupY >= relPos && (actUnloaderX >= posX_R || actUnloaderX <= posX_L))
                {
                    Machine.motion.GetAxisMotionState((int)AXIS.PROD_PICKUP_X, ref state);
                    if (state == MotionState.MotionStateMOVING)
                        Machine.Alarm(ECODE.INTERFERENCE_TRAY_AREA);
                    Machine.motion.GetAxisMotionState((int)AXIS.PROD_PICKUP_Y, ref state);
                    if (state == MotionState.MotionStateMOVING)
                        Machine.Alarm(ECODE.INTERFERENCE_TRAY_AREA);
                }
            }

            if (Machine.interfer_release_trf_z)
            {
                double actualTrfZ = 0;
                Machine.motion.GetAxisActualPos((int)AXIS.TRAY_TRF_Z, ref actualTrfZ);
                if (actualTrfZ >= 1000 * Machine.param.Interference(ParameterDefine.INTERFERENCE.TRAY_TRF_Z_LOADING_LIMIT_POS))
                {
                    Machine.motion.GetAxisMotionState((int)AXIS.TRAY_TRF_Z, ref state);
                    if (state == MotionState.MotionStateMOVING)
                        Machine.Alarm(ECODE.INTERFERENCE_TRF_Z_LOADING);
                }
            }

            if (Machine.interfer_release_trf_y_distance)
            {
                actualTrfY = 0;
                actProdPickupY = 0;
                Machine.motion.GetAxisActualPos((int)AXIS.TRAY_TRF_Y, ref actualTrfY);
                Machine.motion.GetAxisActualPos((int)AXIS.PROD_PICKUP_Y, ref actProdPickupY);
                if (Math.Abs(actProdPickupY - actualTrfY) >= 1000 * Machine.param.Interference(ParameterDefine.INTERFERENCE.PROD_LOADER_INF_TRAY_TRF_SAFE_DISTANCE))
                {
                    Machine.motion.GetAxisMotionState((int)AXIS.TRAY_TRF_Y, ref state);
                    if (state == MotionState.MotionStateMOVING)
                        Machine.Alarm(ECODE.INTERFERENCE_TRAY_TRF);
                    Machine.motion.GetAxisMotionState((int)AXIS.PROD_PICKUP_Y, ref state);
                    if (state == MotionState.MotionStateMOVING)
                        Machine.Alarm(ECODE.INTERFERENCE_TRAY_TRF);
                }
            }
        }
        
        public override void SetHeadTarget(int iTarget)
        {
            throw new NotImplementedException();
        }
    }
}
