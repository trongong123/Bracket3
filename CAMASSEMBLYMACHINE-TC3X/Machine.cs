using static CAMASSEMBLYMACHINE.Define.SystemDefine;
using static CAMASSEMBLYMACHINE.Define.UIDefine;
using CAMASSEMBLYMACHINE.UI;
using TopEng.Utils;
using TopEng.Device;
using TopEng.Device.AjinExt;
using CAMASSEMBLYMACHINE.Define;
using CAMASSEMBLYMACHINE.Process;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using TopEng.Controls;
using Microsoft.Win32;
using System.IO;
using TopEng.Vision;

namespace CAMASSEMBLYMACHINE
{
    public static class Machine
    {
        public static SystemStatus status = new SystemStatus();
        public static AjinExt motion;
        public static AjinIO IO;

        public static ProductData[] Trays = new ProductData[(int)SystemDefine.UNITTRAY.MAXUNIT];
        public static ProductData[] Parts = new ProductData[(int)SystemDefine.UNITPART.MAXUNIT];
        public static ProductData[] Jigs = new ProductData[(int)SystemDefine.UNITJIG.MAXUNIT];
        public static Parameters param = new Parameters();
        public static Recipes recipe = new Recipes();
        public static MachineInfo info = new MachineInfo();

        public static bool DeveloperMode = false;
        public static bool DoorOpenDisregard = false;
        public static bool MutingOnDisregard = false;
        public static bool AloneMode = false; // 상/하부 유닛과 인터페이스를 하지 않는 모드
        public static bool TrayHolding = false;
        public static bool RandomTest = false;
        public static bool interfer_release_y_crash = false;
        public static bool interfer_jog_y_crash = false;
        public static bool interfer_release_y_tray_transfer = false;
        public static bool interfer_release_x_safe_area = false;
        public static bool interfer_release_trf_y_distance = false;
        public static bool interfer_release_trf_z = false;
        public static bool[] buffer_ready_to_place = new bool[2];
        public static bool[] buffer_ready_to_pick = new bool[2];
        public static bool[] prod_loader_start_place = new bool[2];
        public static bool[] assembler_start_pick = new bool[2];
        //public static bool interfer_prod_loader_unloading = false;
        //public static bool interfer_assembler_loading = false;
        //public static bool interfer_prod_picker_loading_to_tray = false;
        //public static bool interfer_tray_trasnfer_busy = false;
        public static bool[] interfer_prod_loader_on_buffer = { false, false }; // product loader가 버퍼와 간섭되는 위치에 존재함
        public static bool[] interfer_assembler_on_buffer = { false, false }; // assembler가 버퍼와 간섭되는 위치에 존재함
        public static bool interfer_jig_align = false;
        public static bool[] interfer_buffer_transfer_prod_loader = { false, false }; // Unit간 Transfer 간섭 회피 위치 확인
        public static bool[] interfer_buffer_transfer_assembler = { false, false };

        public static SYSMODE sysMode = SYSMODE.AUTO;
        public static bool TrayStop = false;
        public static bool PickupStop = false;
        public static bool UnloadStop = false;
        public static bool startCheck = false;
        public static bool trayTransferReady = false;

        public static bool processPause = false;

        public static double totalTime = 0;
        public static int totalCount = 0;
        public static bool usingMoveJog = false;
        public static System.Collections.ArrayList trayNGListX = new System.Collections.ArrayList();
        public static System.Collections.ArrayList trayNGListY = new System.Collections.ArrayList();
        public static bool isUseProductCount = false;
        public enum PROCESS
        {
            SYS,
            HOME,
            EXTERNALEQUIP,
            TRAYIN,
            TRAYINBUF,
            TRAYWORK,
            TRAYOUTBUF,
            TRAYOUT,
            PROD_LOADER,
            ASSEMBLER,
            LEFTBUF,
            RIGHTBUF,
            JIGINBUF,
            JIGWORK,
            JIGOUTBUF,
            JIGINCARRIER,
            JIGOUTCARRIER,

            MAXPROC,
        }

        public enum SYSMODE
        {
            AUTO,
            MANUAL,
            TEACH,
        }

        public static List<IProcess> proclist = new List<IProcess>();

        public static bool Initialize()
        {
            try
            {
                for (int i = 0; i < Trays.Length; i++)
                    Trays[i] = new ProductData();
                for (int i = 0; i < Parts.Length; i++)
                    Parts[i] = new ProductData();
                for (int i = 0; i < Jigs.Length; i++)
                    Jigs[i] = new ProductData();

                param.Read();
                info.Read();

                //server = new RemoteServer("9090");
                //server.BeginReceive(new EventHandler(OnReceiveMessage));

                motion = new AjinExt();
                IO = new AjinIO();

                proclist.Add(new ProcessSys(IO));
                proclist.Add(new ProcessHOME());
                proclist.Add(new ProcessExternalEquip());
                proclist.Add(new ProcessTrayIn());
                proclist.Add(new ProcessTrayInBuf());
                proclist.Add(new ProcessTrayWork());
                proclist.Add(new ProcessTrayOutBuf());
                proclist.Add(new ProcessTrayOut());
                proclist.Add(new ProcessProdLoader());
                proclist.Add(new ProcessAssembler());
                proclist.Add(new ProcessBufStageOne(0));
                proclist.Add(new ProcessBufStageOne(1));
                proclist.Add(new ProcessJigInBuf());
                proclist.Add(new ProcessJigWork());
                proclist.Add(new ProcessJigOutBuf());
                proclist.Add(new ProcessJigInCarrier());
                proclist.Add(new ProcessJigOutCarrier());

                // INITIALIZE MOTION
                motion.Open();
                motion.ReadAxisMotorConfig(SystemDefine.systemPath + @"\axisConfig.json");
                motion.ReadAxisHomeConfig(SystemDefine.systemPath + @"\homeConfig.json");

                if (!SystemDefine.IS_NOTEBOOK_MODE)
                {
                    // 개발자 모드에서는 원점 체크를 하지 않는다.
                    if (Machine.DeveloperMode)
                    {
                        for (int i = 0; i < motion.axisData.Length; i++)
                        {
                            HomeState homeState = HomeState.HomeStateNEED;
                            motion.SetAxisHomeComplete(i, HomeState.HomeStateNEED);
                            motion.GetAxisHomeComplete(i, ref homeState);
                        }
                    }

                    for (int i = 0; i < motion.axisData.Length; i++)
                        motion.AlarmClear(i);
                }
                // 요청사항 : 설비 시작 시 자동으로 서보-온 되도록 하지 않도록
                //for (int i = 0; i < motion.axisData.Length; i++)
                //    motion.SetAxisServoOn(i, 1);

                // INITIALIZE IO
                IO.Open();
                LogUtil.Instance.Log(LOG_TYPE.DEVELOPMENT, "BYPASS_ON", CONTENT_TYPE.DEBUG);
                IO.SetOut((int)DO.BYPASS_ON, 1);
                EStop(false);

                // 생성 타이밍을 맞추기 위해서 여기서 생성
                Program.errorProc = new ProcessError();
                Program.errorProc.Run();

                var proc = proclist[(int)PROCESS.SYS] as ProcessSys;
                Program.errorProc.alarm.AlarmSoundOnEvent += new AlarmUtil.AlarmSoundOnDelegate(proc.BuzzerAlarm);
                Program.errorProc.alarm.AlarmMuteEvent += new AlarmUtil.AlarmMuteDelegate(proc.BuzzerOff);

                // START PROCESS
                for (int i = 0; i < proclist.Count; i++)
                    proclist[i].Run();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static bool ChangeModel(string modelname)
        {
            bool returnValue = recipe.Read(modelname);
            info.CreateJig((int)recipe.Option(RecipeDefine.OPTION.USAGE_JIGS_COUNTS));
            info.Read();

            SystemDefine.productName = "WC";
            string filePath = SystemDefine.recipePath + @"\" + modelname + @"\recipeDefine.csv";
            if (File.Exists(filePath))
            {
                string[] strLine = File.ReadLines(filePath).ToArray();
                if (strLine != null && strLine.Length > 0)
                    SystemDefine.productName = strLine[0];
            }
            return returnValue;
        }

        public static void Start()
        {
            IO.SetOut((int)DO.LIGHT_CURTAIN_MUTING_IN, 0);
            IO.SetOut((int)DO.LIGHT_CURTAIN_MUTING_OUT, 0);

            if (Program.errorProc.alarm.GetAlarmDlg().Visible)
                return;
            if (status.state == SystemState.SystemStateERROR)
            {
                Action action = delegate
                {
                    Dlg_MessageBox formErr = new Dlg_MessageBox(EMESSAGEBOX.MSG, string.Format("Please push the reset button"));
                    formErr.TopLevel = true;
                    formErr.TopMost = true;
                    formErr.ShowDialog();
                };
                MainForm.mainForm.BeginInvoke(action);
                return;
            }
            if (sysMode != SYSMODE.AUTO)
            {
                Action action = delegate
                {
                    Dlg_MessageBox formErr = new Dlg_MessageBox(EMESSAGEBOX.MSG, 
                        string.Format("You Must Auto Start In Auto Mode"));
                    formErr.TopLevel = true;
                    formErr.TopMost = true;
                    formErr.ShowDialog();
                };
                MainForm.mainForm.BeginInvoke(action);
                return; 
            }
            if (status.state != SystemState.SystemStateIDLE)
                return;
            if (Program.errorProc.alarm.hasError())
                return;
            if (!AutoRunCheck())
                return;


            startCheck = true;

            //AlarmClear();
            status.state = SystemState.SystemStateRUN;

            var proc = proclist[(int)PROCESS.SYS] as ProcessSys;
            proc.SafetyStateCheck();
            if (proc.doorOpenState)
            {
                Alarm(ECODE.MACHINE_DOOR_OPENED);
                return;
            }
            if (proc.lightcurtainOn)
            {
                Alarm(ECODE.MACHINE_LIGHT_CURTAIN_SENSED);
                return;
            }
            if (!proc.MotionStateCheck(true))
                return;

            if (!DoorOpenDisregard)
            {
                LogUtil.Instance.Log(LOG_TYPE.DEVELOPMENT, "BYPASS_OFF", CONTENT_TYPE.DEBUG);
                IO.SetOut((int)DO.BYPASS_ON, 0);
            }

            Task.Run(() =>
            {
                SetLightForStart((int)CAMERA.PICKER);
                SetLightForStart((int)CAMERA.TRAY);
                SetLightForStart((int)CAMERA.UNDER);
                SetLightForStart((int)CAMERA.JIG);
                SetLightForStart((int)CAMERA.JIG2);
            });
            Vision.LiveStop((int)CAMERA.PICKER);
            Vision.LiveStop((int)CAMERA.TRAY);
            Vision.LiveStop((int)CAMERA.UNDER);
            Vision.LiveStop((int)CAMERA.JIG);
            Vision.LiveStop((int)CAMERA.JIG2);

            foreach (PROCESS process in Enum.GetValues(typeof(PROCESS)))
            {
                if (process == PROCESS.MAXPROC || process == PROCESS.HOME || process == PROCESS.SYS) continue;
                proclist[(int)process].initCompl = false;
                proclist[(int)process].AutoStart();
            }

            EDM.Start();
            var proc1 = Machine.proclist[(int)Machine.PROCESS.ASSEMBLER] as ProcessAssembler;
            int maxPartIn_X_Left = (int)Machine.recipe.Option(RecipeDefine.OPTION.MAX_PART_IN_LEFT_NG_BOX_X);
            int maxPartIn_Y_Left = (int)Machine.recipe.Option(RecipeDefine.OPTION.MAX_PART_IN_LEFT_NG_BOX_Y);
            int maxPartIn_X_Right = (int)Machine.recipe.Option(RecipeDefine.OPTION.MAX_PART_IN_RIGHT_NG_BOX_X);
            int maxPartIn_Y_Right = (int)Machine.recipe.Option(RecipeDefine.OPTION.MAX_PART_IN_RIGHT_NG_BOX_Y);

            int currentCount = proc1.currentNGCount[0] + proc1.currentNGCount[1];
            int maxCount = maxPartIn_X_Left*maxPartIn_Y_Left + maxPartIn_X_Right*maxPartIn_Y_Right;
            EDM.SetNGBoxStatus(currentCount, maxCount, false);
        }

        private static void SetLightForStart(int InspNo)
        {
            int targetCam = Vision.inspection.InspInfo[InspNo].targetCam;
            var level = new LIGHTDATA();
            level.a = Vision.inspection.recipeData[InspNo].inspInfo.lightSet.a;
            level.b = Vision.inspection.recipeData[InspNo].inspInfo.lightSet.b;
            level.c = Vision.inspection.recipeData[InspNo].inspInfo.lightSet.c;
            level.d = Vision.inspection.recipeData[InspNo].inspInfo.lightSet.d;
            Vision.Camera.LightSet(targetCam, level);
        }

        public static void Stop(bool reportEDM = true)
        {
            startCheck = false;

            foreach (var proc in proclist)
            {
                if (!proc.Error())
                {
                    if (proc is ProcessJigInCarrier) proc.EStop(false);
                    else if (proc is ProcessJigOutCarrier) proc.EStop(false);
                    else if (proc is ProcessJigInBuf) proc.EStop(false);
                    else if (proc is ProcessJigWork) proc.EStop(false);
                    else if (proc is ProcessJigOutBuf) proc.EStop(false);
                    else
                        proc.Stop();
                }
            }

            if (status.state != SystemState.SystemStateERROR)
            {
                LogUtil.Instance.Log(LOG_TYPE.DEVELOPMENT, "BYPASS_ON", CONTENT_TYPE.DEBUG);
                IO.SetOut((int)DO.BYPASS_ON, 1);
            }

            if (reportEDM) EDM.Stop();
        }

        public static void EStop(bool pause, bool estop = true, bool reportEDM = true)
        {
            startCheck = false;

            foreach (PROCESS proc in Enum.GetValues(typeof(PROCESS)))
            {
                if (proc == PROCESS.MAXPROC) continue;
                proclist[(int)proc].EStop(pause);
            }
            motion.AxisStopAll(estop, 5000 * 1000);

            interfer_release_x_safe_area = false;
            interfer_release_y_crash = false;
            interfer_jog_y_crash = false;
            interfer_release_y_tray_transfer = false;
            //interfer_prod_loader_unloading = false;
            //interfer_assembler_loading = false;
            interfer_prod_loader_on_buffer[0] = false;
            interfer_prod_loader_on_buffer[1] = false;
            interfer_assembler_on_buffer[0] = false;
            interfer_assembler_on_buffer[1] = false;
            interfer_jig_align = false;
            //processPause = true;
            processPause = false;

            if (status.state != SystemState.SystemStateERROR)
            {
                LogUtil.Instance.Log(LOG_TYPE.DEVELOPMENT, "BYPASS_ON", CONTENT_TYPE.DEBUG);
                IO.SetOut((int)DO.BYPASS_ON, 1);
            }

            if (reportEDM) EDM.Stop();
        }

        public static bool GetProcInitialized()
        {
            foreach (PROCESS proc in Enum.GetValues(typeof(PROCESS)))
            {
                if (proc == PROCESS.MAXPROC || proc == PROCESS.HOME || proc == PROCESS.SYS) continue;
                if (!proclist[(int)proc].initCompl)
                    return false;
            }

            return true;
        }

        public static bool Busy()
        {
            foreach (var proc in proclist)
            {
                if (proc.Busy())
                    return true;
            }

            return false;
        }

        public static void ClearProduct()
        {
            ClearTray();
            ClearParts();
            ClearJigs();
        }

        public static void ClearTray()
        {
            foreach (var data in Trays)
                data.exist = false;
        }

        public static void ClearParts()
        {
            foreach (var data in Parts)
                data.exist = false;
        }

        public static void ClearJigs()
        {
            foreach (var data in Jigs)
                data.exist = false;
        }

        public static void ClearProductCount()
        {
            string logText = "";

            int totalProduct = info.Product(StatusDefine.PRODUCT.PRODUCT_INPUT_TOTAL_COUNT);
            int inputProduct = info.Product(StatusDefine.PRODUCT.PRODUCT_INPUT_PARTS_COUNT);
            int outGoodProduct = info.Product(StatusDefine.PRODUCT.PRODUCT_OUTPUT_GOOD_COUNT);
            int outNGProduct = info.Product(StatusDefine.PRODUCT.PRODUCT_OUTPUT_NG_COUNT);

            logText = $"[TOTAL:{totalProduct} -> 0][INPUT:{inputProduct} -> 0][GOOD:{outGoodProduct} -> 0][NG:{outNGProduct} -> 0] Product count cleared.";
            info.Product(StatusDefine.PRODUCT.PRODUCT_INPUT_TOTAL_COUNT, 0);
            info.Product(StatusDefine.PRODUCT.PRODUCT_INPUT_PARTS_COUNT, 0);
            info.Product(StatusDefine.PRODUCT.PRODUCT_OUTPUT_GOOD_COUNT, 0);
            info.Product(StatusDefine.PRODUCT.PRODUCT_OUTPUT_NG_COUNT, 0);
            LogUtil.Instance.Log(LOG_TYPE.SYSTEM, logText, CONTENT_TYPE.INFO);

            for (int i = 0; i < info.jig.Count; i++)
            {
                inputProduct = Machine.info.Jig(i, StatusDefine.PRODUCT.PRODUCT_INPUT_PARTS_COUNT);
                outGoodProduct = Machine.info.Jig(i, StatusDefine.PRODUCT.PRODUCT_OUTPUT_GOOD_COUNT);
                outNGProduct = Machine.info.Jig(i, StatusDefine.PRODUCT.PRODUCT_OUTPUT_NG_COUNT);

                logText = $"[JIG:{i + 1}][INPUT:{inputProduct} -> 0][GOOD:{outGoodProduct} -> 0][NG:{outNGProduct} -> 0] Product count cleared.";
                Machine.info.Jig(i, StatusDefine.PRODUCT.PRODUCT_INPUT_PARTS_COUNT, 0);
                Machine.info.Jig(i, StatusDefine.PRODUCT.PRODUCT_OUTPUT_GOOD_COUNT, 0);
                Machine.info.Jig(i, StatusDefine.PRODUCT.PRODUCT_OUTPUT_NG_COUNT, 0);
                LogUtil.Instance.Log(LOG_TYPE.SYSTEM, logText, CONTENT_TYPE.INFO);
            }

            Machine.info.JigClear();
        }

        public static void UpdateTactTime(double tacttime)
        {
            RegistryKey wregKey = Registry.CurrentUser.OpenSubKey(SystemDefine.pathInfoReg, true);
            if (wregKey == null)
                wregKey = Registry.CurrentUser.CreateSubKey(SystemDefine.pathInfoReg);

            totalTime = Convert.ToDouble(wregKey.GetValue("Tact Time Total", 0.00));
            totalCount = Convert.ToInt32(wregKey.GetValue("Pass Count Total", 0));

            totalCount++;
            totalTime += tacttime;

            var time = DateTime.Now;
            string timeString = $"{time.Year}-{time.Month}-{time.Day} {time.Hour}:{time.Minute}:{time.Second}";
            wregKey.SetValue("Tact Time Record Date", timeString);
            wregKey.SetValue("Tact Time Total", Machine.totalTime);
            wregKey.SetValue("Pass Count Total", Machine.totalCount);
        }

        public static void ClearTacTimeInfo()
        {
            RegistryKey wregKey = Registry.CurrentUser.OpenSubKey(SystemDefine.pathInfoReg, true);
            if (wregKey == null)
                wregKey = Registry.CurrentUser.CreateSubKey(SystemDefine.pathInfoReg);

            totalTime = Convert.ToDouble(wregKey.GetValue("Tact Time Total", 0.00));
            totalCount = Convert.ToInt32(wregKey.GetValue("Pass Count Total", 0));

            string tacAvg = (totalTime / totalCount).ToString("0.00");
            if (totalCount == 0)
                tacAvg = "0";
            LogUtil.Instance.Log(LOG_TYPE.SYSTEM, $"[Tact Time Avg:{tacAvg} -> 0] Tact Time Avg cleared.", CONTENT_TYPE.INFO);

            DateTime time = DateTime.Now;
            string timeString = $"{time.Year}-{time.Month}-{time.Day} {time.Hour.ToString("00")}:{time.Minute.ToString("00")}:{time.Second.ToString("00")}";
            wregKey.SetValue("Tact Time Record Date", timeString);

            totalTime = 0; totalCount = 0;
            wregKey.SetValue("Tact Time Total", totalTime);
            wregKey.SetValue("Pass Count Total", totalCount);
        }

        public static bool EmptyProduct()
        {
            if (Machine.status.mode == SystemMode.SystemModeAUTO)
            {
                // 설비에 트레이(공급)이 비면 장비가 IDLE로 바뀌어 사용자에게 공급을 요청하는 상황이라고 표시해줘야 한다.
                foreach (var data in Trays)
                {
                    if (data.exist)
                        return false;
                }
                return true;
            }
            else
            {
                foreach (var data in Jigs)
                {
                    if (data.exist)
                        return false;
                }
                return true;
            }
        }

        public static bool EmptyPart()
        {
            foreach (var data in Parts)
            {
                if (data.exist)
                    return false;
            }
            return true;
        }

        public static bool EmptySupply()
        {
            if (!EmptyPart())
                return false;

            if (Trays[(int)UNITTRAY.TRAY_IN].exist) return false;
            if (Trays[(int)UNITTRAY.TRAY_IN_BUF].exist) return false;

            return true;
        }

        public static bool HomeDone()
        {
            HomeState state = HomeState.HomeStateNEED;
            for (int i = 0; i < (int)AXIS.AXISMAX; i++)
            {
                motion.GetAxisHomeComplete(i, ref state);
                if (state != HomeState.HomeStateDONE)
                    return false;
            }
            return true;
        }

        public static void OnReceiveMessage(object sender, EventArgs e)
        {
            //var msg = e as RemoteMessage;

            //if (msg.Stage == Stage.D)
            //{
            //    var proc = proclist[(int)PROCESS.SEQDECO] as ProcessTRAY;
            //    proc.inspection.OnReceiveMessage(sender, e);
            //}
            //if (msg.Stage == Stage.C)
            //{
            //    var proc = proclist[(int)PROCESS.SEQSIDEC] as ProcessJIGConv;
            //    proc.inspection.OnReceiveMessage(sender, e);
            //}
            //if (msg.Stage == Stage.A)
            //{
            //    var proc = proclist[(int)PROCESS.SEQSIDEA] as ProcessAssyPicker;
            //    proc.inspection.OnReceiveMessage(sender, e);
            //}
        }

        public static void Alarm(ECODE err_code)
        {
            Program.errorProc.Alarm(err_code);

            if (status.state == SystemState.SystemStateRUN || status.state == SystemState.SystemStateRUNEMPTY)
                EDM.Alarm(err_code);
        }

        public static void AlarmClear()
        {
            Machine.EStop(false, true, false);
            Program.errorProc.AlarmClear();
            Machine.status.state = SystemState.SystemStateIDLE;
        }

        public static void AlarmRemove(ECODE err_code)
        {
            Program.errorProc.AlarmRemove(err_code);
        }

        private static bool AutoRunCheck()
        {
            HomeState homeState = HomeState.HomeStateNEED;
            bool systemOk = true;

            for (int i = 0; i < (int)AXIS.AXISMAX; i++)
            {
                Machine.motion.GetAxisHomeComplete(i, ref homeState);
                if (homeState != HomeState.HomeStateDONE)
                {
                    systemOk = false;
                    break;
                }
            }

            if (!systemOk)
            {
                Action action = delegate
                {
                    Dlg_MessageBox form = new Dlg_MessageBox(EMESSAGEBOX.ORIGIN,
                        string.Format("Axis Origin isn't Ready.\n Please Check Origin State!"));
                    form.TopLevel = true;
                    form.TopMost = true;
                    form.ShowDialog();
                };
                MainForm.mainForm.BeginInvoke(action);
            }

            return systemOk;
        }

        public static void CheckStateBtnDoorOpen(bool doorOpenState)
        {
            if (ButtonEnh.doorOpen != doorOpenState)
                ButtonEnh.doorOpen = doorOpenState;
        }

        public static bool CheckTrayTransferInterlockForABS(int moveAxis, double destPos)
        {
            if ((AXIS)moveAxis != AXIS.TRAY_TRF_Y && (AXIS)moveAxis != AXIS.PROD_PICKUP_Y) return true;
            double actualPos = 0.0;
            if ((AXIS)moveAxis == AXIS.TRAY_TRF_Y)
            {
                Machine.motion.GetAxisActualPos((int)AXIS.PROD_PICKUP_Y, ref actualPos);
            }
            else if ((AXIS)moveAxis == AXIS.PROD_PICKUP_Y)
            {
                Machine.motion.GetAxisActualPos((int)AXIS.TRAY_TRF_Y, ref actualPos);
            }

            if (Math.Abs(actualPos - destPos) >= 1000 * Machine.param.Interference(ParameterDefine.INTERFERENCE.PROD_LOADER_INF_TRAY_TRF_SAFE_DISTANCE))
            {
                Machine.Alarm(ECODE.INTERFERENCE_TRAY_TRF);
                return false;
            }
            return true;
        }

        public static bool CheckTrayTransferInterlockForJOG(int moveAxis, double direction)
        {
            if ((AXIS)moveAxis != AXIS.TRAY_TRF_Y && (AXIS)moveAxis != AXIS.PROD_PICKUP_Y) return true;
            if ((AXIS)moveAxis == AXIS.TRAY_TRF_Y && direction >= 0) return true;
            if ((AXIS)moveAxis == AXIS.PROD_PICKUP_Y && direction <= 0) return true;
            double actualPosPicker = 0.0;
            double actualPosTrf = 0.0;
            Machine.motion.GetAxisActualPos((int)AXIS.PROD_PICKUP_Y, ref actualPosPicker);
            Machine.motion.GetAxisActualPos((int)AXIS.TRAY_TRF_Y, ref actualPosTrf);

            if (Math.Abs(actualPosPicker - actualPosTrf) >= 1000 * Machine.param.Interference(ParameterDefine.INTERFERENCE.PROD_LOADER_INF_TRAY_TRF_SAFE_DISTANCE))
            {
                Machine.Alarm(ECODE.INTERFERENCE_TRAY_TRF);
                return false;
            }
            return true;
        }

        public static bool GetTraySensorSignal(TRAY_TYPE trayType, TRAY_CV_SENSOR sensorType)
        {
            uint value1 = 1;
            uint value2 = 1;
            switch (trayType)
            {
                case TRAY_TYPE.IN:
                    if (sensorType == TRAY_CV_SENSOR.IN)
                    {
                        IO.GetIn((int)DI.TRAY_IN_DETECT_IN, ref value1);
                        IO.GetIn((int)DI.TRAY_IN_DETECT2_IN, ref value2);
                    }
                    if (sensorType == TRAY_CV_SENSOR.MID)
                    {
                        IO.GetIn((int)DI.TRAY_IN_DETECT_MID, ref value1);
                        IO.GetIn((int)DI.TRAY_IN_DETECT2_MID, ref value2);
                    }
                    if (sensorType == TRAY_CV_SENSOR.END)
                    {
                        IO.GetIn((int)DI.TRAY_IN_DETECT_END, ref value1);
                        IO.GetIn((int)DI.TRAY_IN_DETECT2_END, ref value2);
                    }
                    break;
                case TRAY_TYPE.IN_BUF:
                    if (sensorType == TRAY_CV_SENSOR.IN)
                    {
                        IO.GetIn((int)DI.TRAY_IN_BUF_DETECT_IN, ref value1);
                        IO.GetIn((int)DI.TRAY_IN_BUF_DETECT2_IN, ref value2);
                    }
                    if (sensorType == TRAY_CV_SENSOR.MID)
                    {
                        IO.GetIn((int)DI.TRAY_IN_BUF_DETECT_MID, ref value1);
                        IO.GetIn((int)DI.TRAY_IN_BUF_DETECT2_MID, ref value2);
                    }
                    if (sensorType == TRAY_CV_SENSOR.END)
                    {
                        IO.GetIn((int)DI.TRAY_IN_BUF_DETECT_END, ref value1);
                        IO.GetIn((int)DI.TRAY_IN_BUF_DETECT2_END, ref value2);
                    }
                    break;
                case TRAY_TYPE.WORK:
                    if (sensorType == TRAY_CV_SENSOR.IN)
                    {
                        IO.GetIn((int)DI.TRAY_WORK_DETECT_IN, ref value1);
                        IO.GetIn((int)DI.TRAY_WORK_DETECT2_IN, ref value2);
                    }
                    if (sensorType == TRAY_CV_SENSOR.MID)
                    {
                        IO.GetIn((int)DI.TRAY_WORK_DETECT_MID, ref value1);
                        IO.GetIn((int)DI.TRAY_WORK_DETECT2_MID, ref value2);
                    }
                    if (sensorType == TRAY_CV_SENSOR.END)
                    {
                        IO.GetIn((int)DI.TRAY_WORK_DETECT_END, ref value1);
                        IO.GetIn((int)DI.TRAY_WORK_DETECT2_END, ref value2);
                    }
                    break;
                case TRAY_TYPE.OUT_BUF:
                    if (sensorType == TRAY_CV_SENSOR.IN)
                    {
                        IO.GetIn((int)DI.TRAY_OUT_BUF_DETECT_IN, ref value1);
                        IO.GetIn((int)DI.TRAY_OUT_BUF_DETECT2_IN, ref value2);
                    }
                    if (sensorType == TRAY_CV_SENSOR.MID)
                    {
                        IO.GetIn((int)DI.TRAY_OUT_BUF_DETECT_MID, ref value1);
                        IO.GetIn((int)DI.TRAY_OUT_BUF_DETECT2_MID, ref value2);
                    }
                    if (sensorType == TRAY_CV_SENSOR.END)
                    {
                        IO.GetIn((int)DI.TRAY_OUT_BUF_DETECT_END, ref value1);
                        IO.GetIn((int)DI.TRAY_OUT_BUF_DETECT2_END, ref value2);
                    }
                    break;
                case TRAY_TYPE.OUT:
                    if (sensorType == TRAY_CV_SENSOR.IN)
                    {
                        IO.GetIn((int)DI.TRAY_OUT_DETECT_IN, ref value1);
                        IO.GetIn((int)DI.TRAY_OUT_DETECT2_IN, ref value2);
                    }
                    if (sensorType == TRAY_CV_SENSOR.MID)
                    {
                        IO.GetIn((int)DI.TRAY_OUT_DETECT_MID, ref value1);
                        IO.GetIn((int)DI.TRAY_OUT_DETECT2_MID, ref value2);
                    }
                    if (sensorType == TRAY_CV_SENSOR.END)
                    {
                        IO.GetIn((int)DI.TRAY_OUT_DETECT_END, ref value1);
                        IO.GetIn((int)DI.TRAY_OUT_DETECT2_END, ref value2);
                    }
                    break;
            }
            return value1 == 1 || value2 == 1;
        }

        public static bool GetJigSignal(JIG_TYPE jigType, JIG_SENSOR sensorType)
        {
            uint value = 0;
            switch (jigType)
            {
                case JIG_TYPE.UNDER_IN:
                    if (sensorType == JIG_SENSOR.IN) Machine.IO.GetIn((int)DI.JIG_IN_CARRIER_DETECT_IN, ref value);
                    if (sensorType == JIG_SENSOR.CENTER) Machine.IO.GetIn((int)DI.JIG_CARRIER_DETECT_CENTER, ref value);
                    if (sensorType == JIG_SENSOR.OUT) Machine.IO.GetIn((int)DI.JIG_IN_CARRIER_DETECT_OUT, ref value);
                    break;
                case JIG_TYPE.UNDER_OUT:
                    if (sensorType == JIG_SENSOR.IN) Machine.IO.GetIn((int)DI.JIG_OUT_CARRIER_DETECT_IN, ref value);
                    if (sensorType == JIG_SENSOR.OUT) Machine.IO.GetIn((int)DI.JIG_OUT_CARRIER_DETECT_OUT, ref value);
                    break;
                case JIG_TYPE.TOP_IN:
                    if (sensorType == JIG_SENSOR.IN) Machine.IO.GetIn((int)DI.JIG_IN_BUF_DETECT_IN, ref value);
                    if (sensorType == JIG_SENSOR.OUT) Machine.IO.GetIn((int)DI.JIG_IN_BUF_DETECT_OUT, ref value);
                    break;
                case JIG_TYPE.TOP_WORK:
                    if (sensorType == JIG_SENSOR.IN) Machine.IO.GetIn((int)DI.JIG_WORK_DETECT_IN, ref value);
                    if (sensorType == JIG_SENSOR.OUT) Machine.IO.GetIn((int)DI.JIG_WORK_DETECT_OUT, ref value);
                    break;
                case JIG_TYPE.TOP_OUT:
                    if (sensorType == JIG_SENSOR.IN) Machine.IO.GetIn((int)DI.JIG_OUT_BUF_DETECT_IN, ref value);
                    if (sensorType == JIG_SENSOR.OUT) Machine.IO.GetIn((int)DI.JIG_OUT_BUF_DETECT_OUT, ref value);
                    break;
            }
            return value == 1;
        }
    }
}
