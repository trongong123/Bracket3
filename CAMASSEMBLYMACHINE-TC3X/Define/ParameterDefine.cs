using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopEng.Utils;
using System.Windows.Forms;
using TopEng.Controls;

namespace CAMASSEMBLYMACHINE.Define
{
    public static class ParameterDefine
    {
        public enum POSITION
        {
            POSITION_UNKNOWN = -1,
            TRAY_TRF_Y_READY_POS,
            TRAY_TRF_Y_LOADING_POS,
            TRAY_TRF_Y_UNLOADING_POS,
            TRAY_TRF_Z_READY_POS,
            TRAY_TRF_Z_LOADING_POS,
            TRAY_TRF_Z_UNLOADING_POS,
            TRAY_TRF_Z_LOADING_RETRY_MAX_COUNT,
            TRAY_TRF_Z_LOADING_RECHECK,
            TRAY_WORK_LOADING_POS,
            TRAY_WORK_WORK_POS,
            TRAY_WORK_UNLOADING_POS,
            TRAY_WORK_UNLOADING_OFFSET,
            PROD_LOADER_X_READY_POS,
            PROD_LOADER_Y_READY_POS,
            PROD_LOADER_Z_READY_POS,
            PROD_LOADER_R_READY_POS,
            PROD_LOADER_Y_TRAY_VISION_AVOID_POS,
            PROD_LOADER_Y_TRAY_TRF_AVOID_POS,
            ASSEMBLER_X_READY_POS,
            ASSEMBLER_Y_READY_POS,
            ASSEMBLER_Z_READY_POS,
            ASSEMBLER_Z_DISCARD_POS,
            ASSEMBLER_R_READY_POS,
            ASSEMBLER_X_NG_BOX_LEFT_POS,
            ASSEMBLER_Y_NG_BOX_LEFT_POS,
            ASSEMBLER_R_NG_BOX_LEFT_POS,
            ASSEMBLER_X_NG_BOX_RIGHT_POS,
            ASSEMBLER_Y_NG_BOX_RIGHT_POS,
            ASSEMBLER_R_NG_BOX_RIGHT_POS,

            JIG_IN_BUF_Z_READY_POS,
            JIG_IN_BUF_Z_PRESS_POS,

            POSITION_MAX_COUNT
        }
        public enum VELOCITY
        {
            VELOCITY_UNKNOWN = -1,
            TRAY_TRF_Z_VEL,
            TRAY_TRF_Z_APPROACH_VEL,
            TRAY_TRF_Z_ACC,
            TRAY_TRF_Z_DEC,
            TRAY_TRF_Y_VEL,
            TRAY_TRF_Y_ACC,
            TRAY_TRF_Y_DEC,
            TRAY_WORK_Z_VEL,
            TRAY_WORK_Z_APPROACH_VEL,
            TRAY_WORK_Z_ACC,
            TRAY_WORK_Z_DEC,
            JIG_Z_VEL,
            JIG_Z_ACC,
            JIG_Z_DEC,
            PROD_LOADER_X_MOVE_VEL,
            PROD_LOADER_X_MOVE_ACC,
            PROD_LOADER_X_MOVE_DEC,
            PROD_LOADER_Y_MOVE_VEL,
            PROD_LOADER_Y_MOVE_ACC,
            PROD_LOADER_Y_MOVE_DEC,
            PROD_LOADER_Z_MOVE_VEL,
            PROD_LOADER_Z_APPROACH_VEL,
            PROD_LOADER_Z_MOVE_ACC,
            PROD_LOADER_Z_MOVE_DEC,
            PROD_LOADER_R_MOVE_VEL,
            PROD_LOADER_R_MOVE_ACC,
            PROD_LOADER_R_MOVE_DEC,
            PROD_LOADER_X_CAL_MOVE_VEL,
            PROD_LOADER_X_CAL_MOVE_ACC,
            PROD_LOADER_X_CAL_MOVE_DEC,
            PROD_LOADER_Y_CAL_MOVE_VEL,
            PROD_LOADER_Y_CAL_MOVE_ACC,
            PROD_LOADER_Y_CAL_MOVE_DEC,
            PROD_LOADER_Z_CAL_MOVE_VEL,
            PROD_LOADER_Z_CAL_MOVE_ACC,
            PROD_LOADER_Z_CAL_MOVE_DEC,
            CAM_TRANSFER_Y_APPROACH_VEL,
            CAM_TRANSFER_Y_MOVE_VEL,
            CAM_TRANSFER_Y_MOVE_ACC,
            CAM_TRANSFER_Y_MOVE_DEC,
            ASSEMBLER_X_MOVE_VEL,
            ASSEMBLER_X_MOVE_ACC,
            ASSEMBLER_X_MOVE_DEC,
            ASSEMBLER_Y_MOVE_VEL,
            ASSEMBLER_Y_MOVE_ACC,
            ASSEMBLER_Y_MOVE_DEC,
            ASSEMBLER_Z_MOVE_VEL,
            ASSEMBLER_Z_APPROACH_VEL,
            ASSEMBLER_Z_MOVE_ACC,
            ASSEMBLER_Z_MOVE_DEC,
            ASSEMBLER_R_MOVE_VEL,
            ASSEMBLER_R_MOVE_ACC,
            ASSEMBLER_R_MOVE_DEC,
            ASSEMBLER_X_CAL_MOVE_VEL,
            ASSEMBLER_X_CAL_MOVE_ACC,
            ASSEMBLER_X_CAL_MOVE_DEC,
            ASSEMBLER_Y_CAL_MOVE_VEL,
            ASSEMBLER_Y_CAL_MOVE_ACC,
            ASSEMBLER_Y_CAL_MOVE_DEC,
            ASSEMBLER_Z_CAL_MOVE_VEL,
            ASSEMBLER_Z_CAL_MOVE_ACC,
            ASSEMBLER_Z_CAL_MOVE_DEC,
            CAL_XY_MOVE_VEL,

            VELOCITY_MAX_COUNT,
        }
        public enum CALIBRATION
        {
            TRAY_CAL_MAP_START_POS_X,
            TRAY_CAL_MAP_START_POS_Y,
            TRAY_CAL_MAP_END_POS_X,
            TRAY_CAL_MAP_END_POS_Y,
            TRAY_CAL_MAP_STEP,
            TRAY_CAL_MAP_POS_Z,
            
            PICKER_LEFT_CAL_MAP_START_POS_X,
            PICKER_LEFT_CAL_MAP_START_POS_Y,
            PICKER_LEFT_CAL_MAP_END_POS_X,
            PICKER_LEFT_CAL_MAP_END_POS_Y,

            PICKER_RIGHT_CAL_MAP_START_POS_X,
            PICKER_RIGHT_CAL_MAP_START_POS_Y,
            PICKER_RIGHT_CAL_MAP_END_POS_X,
            PICKER_RIGHT_CAL_MAP_END_POS_Y,
            PICKER_CAL_MAP_STEP,
            PICKER_CAL_MAP_POS_Z,

            UNDER_LEFT_CAL_MAP_START_POS_X,
            UNDER_LEFT_CAL_MAP_START_POS_Y,
            UNDER_LEFT_CAL_MAP_END_POS_X,
            UNDER_LEFT_CAL_MAP_END_POS_Y,

            UNDER_RIGHT_CAL_MAP_START_POS_X,
            UNDER_RIGHT_CAL_MAP_START_POS_Y,
            UNDER_RIGHT_CAL_MAP_END_POS_X,
            UNDER_RIGHT_CAL_MAP_END_POS_Y,
            UNDER_CAL_MAP_STEP,
            UNDER_CAL_MAP_POS_Z,

            JIG_CAL_MAP_START_POS_X,
            JIG_CAL_MAP_START_POS_Y,
            JIG_CAL_MAP_END_POS_X,
            JIG_CAL_MAP_END_POS_Y,
            JIG_CAL_MAP_STEP,
            JIG_CAL_MAP_POS_Z,

            CALIBRATION_MAX_COUNT
        }
        public enum TIME
        {
            TIME_UNKNOWN = -1,
            TRAY_CONV_STOP_DELAY_TIME,
            JIG_CARRIER_CONV_STOP_DELAY_TIME,
            JIG_WORK_CONV_STOP_DELAY_TIME,
            JIG_CONV_STOP_DELAY_TIME,
            JIG_CONV_RUN_TIME_OUT,
            MUTING_ON_TIME,
            TRAY_TRF_VACUUM_ON_WAIT_TIME,
            VACUUM_ON_WAIT_TIME,
            VACUUM_ON_TIME_OUT,
            PURGE_OFF_WAIT_TIME,
            PURGE_OFF_TIME,
            VISION_TIME_OUT,
            CONV_RUN_TIME_OUT,
            CYLINDER_RUN_WAIT_TIME,
            CYLINDER_TIME_OUT,
            ASSEMBLE_WAIT_TIME,
            ASSEMBLER_VACUUM_ON_WAIT_TIME,
            STOPPER_UP_WAIT,
            INIT_CONV_RUN_TIME,
            BUF_STAGE_VAC_ON_WAIT_TIME,
            INTERFACE_FAILED_TIME,
            PEELING_BLOW_WAIT_TIME,
			PEELING_TURN_DELAY_TIME,
            PEELING_RETURN_DELAY_TIME,
            PEELING_CLAMP_DELAY_TIME,
            PEELING_UNCLAMP_DELAY_TIME,
            ASSEMBLING_DELAY_TIME,
            TIME_MAX_COUNT
        }

        public enum OPTION
        {
            OPTION_UNKNOWN = -1,
            TRAY_LOADING_COUNT_FOR_DRYRUN,
            USE_IMAGE_GATHERING_FOR_EL,
            USE_IMAGE_GATHERING_FOR_ASSEMBLY_DEFECT,
            USE_LEFT_BUFFER,
            USE_RIGHT_BUFFER,
            USE_TRAY_WORK_ANGLE_TRAIN,
            USE_TRAY_WORK_ONE_VISION_SHOT,
            VISION_CAL_CENTER_LENGTH_TRAY,
            VISION_CAL_CENTER_LENGTH_PICKER,
            VISION_CAL_CENTER_LENGTH_UNDER,
            VISION_CAL_CENTER_LENGTH_JIG,
            USE_JIG_THETA,
            USE_XY_SATURATION,
            USE_PICKER_XY_SATURATION,
            REPORT_EDM_SERVER,
            USE_PROC_LOG,
            USE_ID_SCAN,
            USE_PICKER_UNDER_VISION,
            ALIGNMENT_ERROR_CHECK_COUNT,
            TRAY_WORK_CLASSIFY_LOW_SCORE,
            JIG_WORK_CLASSIFY_LOW_SCORE,
            UNDER_VISION_TARGET_ANGLE_RANGE,
            USE_TRAY_PRODUCT_COUNT_ALARM,
            PEELING_UNCLAMP_REPEAT_COUNT,
            OPTION_MAX_COUNT
        }

        public enum INTERFERENCE
        {
            INTERFERENCE_UNKNOWN = -1,
            PROD_LOADER_INF_X_RELEASE_Y_POS, // X 조명 간섭 위치 (Axis Y)
            PROD_LOADER_INF_X_R_POS, // X 조명 간섭 위치 (Axis X)
            PROD_LOADER_INF_X_L_POS, // X 조명 간섭 위치 (Axis X)
            PROD_LOADER_INF_TRAY_TRF_SAFE_DISTANCE, // TRF Y & PICK Y 안전 간격
            TRAY_TRF_Z_LOADING_LIMIT_POS, // TRF Z 조명 간섭 위치

            INTERFERENCE_MAX_COUNT
        }

        public static List<string> position = new List<string>();
        public static List<string> velocity = new List<string>();
        public static List<string> calibration = new List<string>();
        public static List<string> time = new List<string>();
        public static List<string> option = new List<string>();
        public static List<string> interference = new List<string>();

        public static void DefineData()
        {
            position.Clear();
            for (int i = 0; i < (int)POSITION.POSITION_MAX_COUNT; i++)
                position.Add(Enum.GetName(typeof(POSITION), i));

            velocity.Clear();
            for (int i = 0; i < (int)VELOCITY.VELOCITY_MAX_COUNT; i++)
                velocity.Add(Enum.GetName(typeof(VELOCITY), i));

            calibration.Clear();
            for (int i = 0; i < (int)CALIBRATION.CALIBRATION_MAX_COUNT; i++)
                calibration.Add(Enum.GetName(typeof(CALIBRATION), i));

            time.Clear();
            for (int i = 0; i < (int)TIME.TIME_MAX_COUNT; i++)
                time.Add(Enum.GetName(typeof(TIME), i));

            option.Clear();
            for (int i = 0; i < (int)OPTION.OPTION_MAX_COUNT; i++)
                option.Add(Enum.GetName(typeof(OPTION), i));

            interference.Clear();
            for (int i = 0; i < (int)INTERFERENCE.INTERFERENCE_MAX_COUNT; i++)
                interference.Add(Enum.GetName(typeof(INTERFERENCE), i));
        }

        public static bool PositionDataCheck(Dictionary<string, PARAMSTRUCT> dic, string path)
        {
            bool isCheckError = false;
            foreach (var paramString in position)
            {
                if (!dic.ContainsKey(paramString))
                {
                    Dlg_MessageBox formErr =
                        new Dlg_MessageBox(EMESSAGEBOX.MSG,
                        string.Format($"[{paramString}] Key is not Exist\n" +
                        $"Program is Exit"),
                        path);
                    formErr.TopLevel = true;
                    formErr.TopMost = true;
                    formErr.ShowDialog();

                    //Force Quit
                    System.Diagnostics.Process.GetCurrentProcess().Kill();
                    Application.ExitThread();
                    Environment.Exit(0);

                    dic[paramString] = new PARAMSTRUCT
                    {
                        Name = paramString,
                        Tag = paramString,
                        Unit = "mm",
                        Min = -99999,
                        Max = 99999
                    };
                    isCheckError = true;
                }
            }
            if (isCheckError) return false;
            return true;
        }

        public static bool VelocityDataCheck(Dictionary<string, PARAMSTRUCT> dic, string path)
        {
            bool isCheckError = false;
            foreach (var paramString in velocity)
            {
                if (!dic.ContainsKey(paramString))
                {
                    Dlg_MessageBox formErr =
                        new Dlg_MessageBox(EMESSAGEBOX.MSG,
                        string.Format($"[{paramString}] Key is not Exist\n" +
                        $"Program is Exit"),
                        path);
                    formErr.TopLevel = true;
                    formErr.TopMost = true;
                    formErr.ShowDialog();

                    //Force Quit
                    System.Diagnostics.Process.GetCurrentProcess().Kill();
                    Application.ExitThread();
                    Environment.Exit(0);

                    string unit = "mm/s";
                    if (paramString.Contains("VEL"))
                        unit = "mm/s";
                    else if (paramString.Contains("ACC") || paramString.Contains("DEC"))
                        unit = "mm/s²";

                    dic[paramString] = new PARAMSTRUCT
                    {
                        Name = paramString,
                        Tag = paramString,
                        Unit = "mm",
                        Min = -99999,
                        Max = 99999
                    };
                    isCheckError = true;
                }
            }
            if (isCheckError) return false;
            return true;
        }

        public static bool CalibrationDataCheck(Dictionary<string, PARAMSTRUCT> dic, string path)
        {
            bool isCheckError = false;
            foreach (var paramString in calibration)
            {
                if (!dic.ContainsKey(paramString))
                {
                    Dlg_MessageBox formErr =
                        new Dlg_MessageBox(EMESSAGEBOX.MSG,
                        string.Format($"[{paramString}] Key is not Exist\n" +
                        $"Program is Exit"),
                        path);
                    formErr.TopLevel = true;
                    formErr.TopMost = true;
                    formErr.ShowDialog();

                    //Force Quit
                    System.Diagnostics.Process.GetCurrentProcess().Kill();
                    Application.ExitThread();
                    Environment.Exit(0);

                    dic[paramString] = new PARAMSTRUCT
                    {
                        Name = paramString,
                        Tag = paramString,
                        Unit = "mm",
                        Min = -99999,
                        Max = 99999
                    };
                    isCheckError = true;
                }
            }
            if (isCheckError) return false;
            return true;
        }

        public static bool TimeDataCheck(Dictionary<string, PARAMSTRUCT> dic, string path)
        {
            bool isCheckError = false;
            foreach (var paramString in time)
            {
                if (!dic.ContainsKey(paramString))
                {
                    Dlg_MessageBox formErr =
                        new Dlg_MessageBox(EMESSAGEBOX.MSG,
                        string.Format($"[{paramString}] Key is not Exist\n" +
                        $"Program is Exit"),
                        path);
                    formErr.TopLevel = true;
                    formErr.TopMost = true;
                    formErr.ShowDialog();

                    //Force Quit
                    System.Diagnostics.Process.GetCurrentProcess().Kill();
                    Application.ExitThread();
                    Environment.Exit(0);

                    dic[paramString] = new PARAMSTRUCT
                    {
                        Name = paramString,
                        Tag = paramString,
                        Unit = "ms",
                        Min = 1,
                        Max = 99999
                    };
                    isCheckError = true;
                }
            }
            if (isCheckError) return false;
            return true;
        }

        public static bool OptionDataCheck(Dictionary<string, PARAMSTRUCT> dic, string path)
        {
            bool isCheckError = false;
            foreach (var paramString in option)
            {
                if (!dic.ContainsKey(paramString))
                {
                    Dlg_MessageBox formErr =
                        new Dlg_MessageBox(EMESSAGEBOX.MSG,
                        string.Format($"[{paramString}] Key is not Exist\n" +
                        $"Program is Exit"),
                        path);
                    formErr.TopLevel = true;
                    formErr.TopMost = true;
                    formErr.ShowDialog();

                    //Force Quit
                    System.Diagnostics.Process.GetCurrentProcess().Kill();
                    Application.ExitThread();
                    Environment.Exit(0);

                    dic[paramString] = new PARAMSTRUCT
                    {
                        Name = paramString,
                        Tag = paramString,
                        Unit = "On/Off",
                        Min = 0,
                        Max = 1
                    };
                    isCheckError = true;
                }
            }
            if (isCheckError) return false;
            return true;
        }

        public static bool InterferenceDataCheck(Dictionary<string, PARAMSTRUCT> dic, string path)
        {
            bool isCheckError = false;
            foreach (var paramString in interference)
            {
                if (!dic.ContainsKey(paramString))
                {
                    Dlg_MessageBox formErr =
                        new Dlg_MessageBox(EMESSAGEBOX.MSG,
                        string.Format($"[{paramString}] Key is not Exist\n" +
                        $"Program is Exit"),
                        path);
                    formErr.TopLevel = true;
                    formErr.TopMost = true;
                    formErr.ShowDialog();

                    //Force Quit
                    System.Diagnostics.Process.GetCurrentProcess().Kill();
                    Application.ExitThread();
                    Environment.Exit(0);

                    dic[paramString] = new PARAMSTRUCT
                    {
                        Name = paramString,
                        Tag = paramString,
                        Unit = "mm",
                        Min = -99999,
                        Max = 99999
                    };
                    isCheckError = true;
                }
            }
            if (isCheckError) return false;
            return true;
        }
    }
}
