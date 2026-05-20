using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAMASSEMBLYMACHINE.Define
{
    /**
     * @brief 물류 프로그램에서 정의하고자 하는 상수를 정의하는 클래스
     */
    public static class SystemDefine
    {
        public static string pgmVersion = "1.260520.1.DryRun";

        public static bool IS_NOTEBOOK_MODE = false;
        public static bool NO_VISION_KEY = false;
        public static string notebookModeFile = @"D:\NoteBookMode.txt";
        public static string noVisionKeyFile = @"D:\NoVisionKey.txt";
        public static int CamCountSim = 3;
        // System define
        public static string machineName = "CAMASSEMBLYMACHINE-WC3"; //MES, GEIM 등에 보낼 설비 명
        public static string systemName = "CAMASSEMBLYMACHINE-WC3"; //MES, GEIM 등에 보낼 설비 명
        public static string productName = "TELE";

        public static string rootPath = @"C:\FA\" + machineName;
        public static string systemPath = rootPath + @"\System";
        public static string infoPath = rootPath + @"\Info";
        public static string recipePath = rootPath + @"\Recipe";
        public static string paramPath = rootPath + @"\Param";
        public static string logPath = rootPath + @"\Log";
        public static string alarmPath = rootPath + @"\Error";
        public static string EDMLogPath = @"C:\FA\Log";
        public static string AMRLogPath = $"C:\\FA\\{machineName}\\Log\\CAMASSY\\INPUT";
        public static string capturePath = rootPath + @"\Capture";
        public static string trainingPath = rootPath + @"\Training";
        public static string judgePath = rootPath + @"\Judge";
        public static string productPath = rootPath + @"\Product";

        public static string alarmlistPath = systemPath + @"\errorlist.json";

        public static string externalEquipServerIP = "192.168.10.100";
        public static int externalEquipServerPort = 8888;

        public static string modelTag = "Model : ";
        public static string groupName = "Automation Engineering Group(MX)";
        public static string pathSystemReg = @"SOFTWARE\TOPENGINEERING\SAMSUNG\CAMASSEMBLYMACHINE-WC2";
        public static string pathInfoReg = pathSystemReg + @"\INFO";

        public static string modelname = "";
        public static string passwordOrigin = "0000";
        public static string passwordData = "0000";
        public static int deletelogperiod = 7;
        public static int deleteimageperiod = 1;
        public static double motorVelocity = 100;

        public static bool bAlignCheck = true;
        public static bool UpdateCameraInfo = true;
        public static int CamCount = 0;
        public static bool UseExtRefPoint = false;
        public static bool UseLightOff = true;
        public static bool UseImageSave = true;
        public static bool UseLowQualityImage = false;
        public static bool manualGrab = false;

        public static void UpdateData()
        {
            rootPath = @"C:\FA\" + machineName;
            systemPath = rootPath + @"\System";
            infoPath = rootPath + @"\Info";
            recipePath = rootPath + @"\Recipe";
            paramPath = rootPath + @"\Param";
            logPath = rootPath + @"\Log";
            EDMLogPath = @"C:\FA\Log";
            capturePath = rootPath + @"\Capture";
            trainingPath = rootPath + @"\Training";
            judgePath = rootPath + @"\Judge";
            productPath = rootPath + @"\Product";
            alarmlistPath = systemPath + @"\errorlist.json";
        }

        // System 에러코드 
        #region system error code
        /*
         * System define error code : 수정가능
         */
        public const int ERROR_NOT_ALLOCATE = 0;
        public const int ERRCODE_NO_ERROR = -1000;
        public const int ERRCODE_STOP_YELLOW = 39578;
        

        
        /*
         * System define error code : 삭제하지 말 것 
         * Json의 error code와 code 중복 사용하지 말 것
         */
        #endregion
        public const int ERRCODE_ETHERCAT_MASTER_NOT_FOUND = 30000;
        public const int ERRCODE_ETHERCAT_INIT_FAIL = 30001;
        public const int BUZZER_TIMEOUT = 3000; //ms - default 3sec
        public const int CHECK_IDLE_TIME = 60000; //ms - default 60sec
		public const int CHECK_OOS_TIME = 15000; //ms - default 15sec

        public enum USER_LEVEL
        {
            AUTH_DATA,
            AUTH_ORIGIN,
            DEVELOPER,
        }

        public enum UNITTRAY
        {
            ALL = -1,

            TRAY_IN,
            TRAY_IN_BUF,
            TRAY_WORK,
            TRAY_OUT,
            TRAY_OUT_BUF,
            TRAY_TRF,

            MAXUNIT,
        }

        public enum UNITPART
        {
            ALL = -1,

            TRAY,
            PROD_PICK1,
            PROD_PICK2,
            CAM_TRF_L1,
            CAM_TRF_L2,
            CAM_TRF_R1,
            CAM_TRF_R2,
            PEELING_L1L,
            PEELING_L1R,
            PEELING_R1L,
            PEELING_R1R,
            PEELING_L2L,
            PEELING_L2R,
            PEELING_R2L,
            PEELING_R2R,
            BUF1_L1,
            BUF1_L2,
            BUF2_L1,
            BUF2_L2,
            BUF1_R1,
            BUF1_R2,
            BUF2_R1,
            BUF2_R2,
            ASSEMBLER1,
            ASSEMBLER2,
            JIG1,
            JIG2,

            MAXUNIT,
        }

        public enum UNITJIG
        {
            ALL = -1,

            JIG_IN_BUF,
            JIG_WORK,
            JIG_OUT_BUF,
            JIG_IN_CARRIER,
            JIG_OUT_CARRIER,

            MAXUNIT,
        }

        public enum AXIS
        {
            // TRAY
            TRAY_WORK_Z,
            TRAY_TRF_Y,
            TRAY_TRF_Z,
            // PICKER
            PROD_PICKUP_X,
            PROD_PICKUP_Y,
            PROD_PICKUP_Z,
            PROD_PICKUP_R1,
            PROD_PICKUP_R2,
            ASSEMBLER_X,
            ASSEMBLER_Y,
            ASSEMBLER_Z,
            ASSEMBLER_R1,
            ASSEMBLER_R2,
            // TRANSFER
            CAM_TRANSFER_LEFT_Y, // 버퍼 자재를 이송 및 박리 위치로 이동
            CAM_TRANSFER_RIGHT_Y,
            // JIG
            JIG_IN_BUF_Z,
            AXISMAX
        }

        public enum CAMERA
        {
            TRAY,
            UNDER,
            JIG,
            PICKER,
            JIG2,

            MAX
        }

        public enum TRAY_CV_SENSOR
        {
            IN,
            MID,
            END
        }

        public enum TRAY_TYPE
        {
            IN,
            IN_BUF,
            WORK,
            OUT_BUF,
            OUT
        }

        public enum JIG_TYPE
        {
            UNDER_IN,
            UNDER_OUT,
            TOP_IN,
            TOP_WORK,
            TOP_OUT,
        }

        public enum JIG_SENSOR
        {
            IN,
            OUT,
            CENTER,
            BRIDGE
        }

        public enum JIG_CLASSIFY_RESULT
        {
            OK,
            NG,
            EXIST,
            EMPTY
        }

        public enum DI
        {
            /*[0x00]*/
            DOOR_FRONT_TOP1,
            /*[0x01]*/
            DOOR_FRONT_TOP2,
            /*[0x02]*/
            DOOR_REAR_TOP1,
            /*[0x03]*/
            DOOR_REAR_TOP2,
            /*[0x04]*/
            DOOR_REAR_TOP3,
            /*[0x05]*/
            SP0x05,
            /*[0x06]*/
            SP0x06,
            /*[0x07]*/
            SP0x07,
            /*[0x08]*/
            SP0x08,
            /*[0x09]*/
            SP0x09,
            /*[0x0A]*/
            SP0x0A,
            /*[0x0B]*/
            SP0x0B,
            /*[0x0C]*/
            SP0x0C,
            /*[0x0D]*/
            SP0x0D,
            /*[0x0E]*/
            LIGHT_CURTAIN_IN,
            /*[0x0F]*/
            LIGHT_CURTAIN_OUT,

            /*[0x10]*/
            START_SW_OP1,
            /*[0x11]*/
            STOP_SW_OP1,
            /*[0x12]*/
            RESET_SW_OP1,
            /*[0x13]*/
            START_SW_OP2,
            /*[0x14]*/
            STOP_SW_OP2,
            /*[0x15]*/
            RESET_SW_OP2,
            /*[0x16]*/
            EMO_SW_OP1,
            /*[0x17]*/
            EMO_SW_OP2,
            /*[0x18]*/
            SP0x18,
            /*[0x19]*/
            SP0x19,
            /*[0x1A]*/
            SP0x1A,
            /*[0x1B]*/
            SP0x1B,
            /*[0x1C]*/
            SP0x1C,
            /*[0x1D]*/
            SP0x1D,
            /*[0x1E]*/
            SP0x1E,
            /*[0x1F]*/
            SP0x1F,

            /*[0x20]*/
            NO_INTERFERENCE_PICKER_Y,
            /*[0x21]*/
            JIG_CARRIER_DETECT_CENTER,
            /*[0x22]*/
            JIG_WORK_PALLETE_FWD,
            /*[0x23]*/
            JIG_WORK_PALLETE_BWD,
            /*[0x24]*/
            ASSEMBLER_Z1_VAC_LEVEL,
            /*[0x25]*/
            ASSEMBLER_Z2_VAC_LEVEL,
            /*[0x26]*/
            TRAY_IN_ERROR,
            /*[0x27]*/
            TRAY_IN_BUF_ERROR,
            /*[0x28]*/
            TRAY_WORK_ERROR,
            /*[0x29]*/
            TRAY_STACK_ERROR,
            /*[0x2A]*/
            TRAY_OUT_ERROR,
            /*[0x2B]*/
            JIG_IN_BUF_ERROR,
            /*[0x2C]*/
            JIG_WORK_ERROR,
            /*[0x2D]*/
            JIG_OUT_BUF_ERROR,
            /*[0x2E]*/
            JIG_IN_CARRIER_ERROR,
            /*[0x2F]*/
            JIG_OUT_CARRIER_ERROR,

            /*[0x30]*/
            IF_TRAY_IN_AMR_PID1,
            /*[0x31]*/
            IF_TRAY_IN_AMR_PID2,
            /*[0x32]*/
            IF_TRAY_IN_AMR_PID3,
            /*[0x33]*/
            IF_TRAY_OUT_AMR_PID1,
            /*[0x34]*/
            IF_TRAY_OUT_AMR_PID2,
            /*[0x35]*/
            IF_TRAY_OUT_AMR_PID3,
            /*[0x36]*/
            JIG_WORK_2_PALLET_LOCK_BWD,
            /*[0x37]*/
            JIG_WORK_2_PALLET_LOCK_FWD,
            /*[0x38]*/
            IF_REAR_MACHINE_RUN,
            /*[0x39]*/
            IF_FRONT_MACHINE_RUN,
            /*[0x3A]*/
            IF_JIG_UPSTREAM_EXIST_CHECK_WORK,     // UPSTREAM -> DOWNSTREAM : UPSTREAM의 IF_JIG_ULD_EXIST_WORK가 ON되면 ON
            /*[0x3B]*/
            IF_JIG_UPSTREAM_RUN_CHECK_WORK,       // DOWNSTREAM -> UPSTREAM : DOWNSTREAM의 IF_JIG_ULD_CONV_RUN_WORK가 ON되면 ON
            /*[0x3C]*/
            IF_JIG_DOWNSTREAM_RUN_CHECK_WORK,     // DOWNSTREAM -> UPSTREAM : DOWNSTREAM의 IF_JIG_LD_CONV_RUN_WORK가 ON되면 ON
            /*[0x3D]*/
            IF_JIG_UPSTREAM_EXIST_CHECK_CARRIER,  // UPSTREAM -> DOWNSTREAM : UPSTREAM의 IF_JIG_ULD_EXIST_CARRIER가 ON되면 ON
            /*[0x3E]*/
            IF_JIG_UPSTREAM_RUN_CHECK_CARRIER,    // UPSTREAM -> DOWNSTREAM : UPSTREAM의 IF_JIG_ULD_CONV_RUN_CARRIER가 ON되면 ON
            /*[0x3F]*/
            IF_JIG_DOWNSTREAM_RUN_CHECK_CARRIER,  // DOWNSTREAM -> UPSTREAM : DOWNSTREAM의 IF_JIG_LD_CONV_RUN_CARRIER가 ON되면 ON

            /*[0x40]*/
            ASSEMBLER_Z1_UP,
            /*[0x41]*/
            ASSEMBLER_Z1_DOWN,
            /*[0x42]*/
            ASSEMBLER_Z1_VACON,
            /*[0x43]*/
            ASSEMBLER_Z2_UP,
            /*[0x44]*/
            ASSEMBLER_Z2_DOWN,
            /*[0x45]*/
            ASSEMBLER_Z2_VACON,
            /*[0x46]*/
            CAM_PICKER_Z1_UP,
            /*[0x47]*/
            CAM_PICKER_Z1_DOWN,
            /*[0x48]*/
            CAM_PICKER_Z1_VACON,
            /*[0x49]*/
            CAM_PICKER_Z2_UP,
            /*[0x4A]*/
            CAM_PICKER_Z2_DOWN,
            /*[0x4B]*/
            CAM_PICKER_Z2_VACON,
            /*[0x4C]*/
            JIG_IN_BUF_DETECT_IN,
            /*[0x4D]*/
            JIG_IN_BUF_DETECT_OUT,
            /*[0x4E]*/
            JIG_IN_BUF_STOPPER_UP,
            /*[0x4F]*/
            JIG_IN_BUF_STOPPER_DOWN,

            /*[0x50]*/
            JIG_WORK_DETECT_IN,
            /*[0x51]*/
            JIG_WORK_DETECT_OUT,
            /*[0x52]*/
            JIG_WORK_STOPPER_UP,
            /*[0x53]*/
            JIG_WORK_STOPPER_DOWN,
            /*[0x54]*/
            JIG_OUT_BUF_DETECT_IN,
            /*[0x55]*/
            JIG_OUT_BUF_DETECT_OUT,
            /*[0x56]*/
            JIG_OUT_STOPPER_UP,
            /*[0x57]*/
            JIG_OUT_STOPPER_DOWN,
            /*[0x58]*/
            JIG_IN_CARRIER_DETECT_IN,
            /*[0x59]*/
            JIG_IN_CARRIER_DETECT_OUT,
            /*[0x5A]*/
            JIG_IN_CARRIER_STOPPER_UP,
            /*[0x5B]*/
            JIG_IN_CARRIER_STOPPER_DOWN,
            /*[0x5C]*/
            JIG_OUT_CARRIER_DETECT_IN,
            /*[0x5D]*/
            JIG_OUT_CARRIER_DETECT_OUT,
            /*[0x5E]*/
            JIG_OUT_CARRIER_STOPPER_UP,
            /*[0x5F]*/
            JIG_OUT_CARRIER_STOPPER_DOWN,

            /*[0x60]*/
            TRAY_IN_DETECT_IN,
            /*[0x61]*/
            TRAY_IN_DETECT_MID,
            /*[0x62]*/
            TRAY_IN_DETECT_END,
            /*[0x63]*/
            TRAY_IN_DETECT2_IN,
            /*[0x64]*/
            TRAY_IN_DETECT2_MID,
            /*[0x65]*/
            TRAY_IN_DETECT2_END,
            /*[0x66]*/
            TRAY_IN_STOPPER_UP,
            /*[0x67]*/
            TRAY_IN_STOPPER_DOWN,
            /*[0x68]*/
            TRAY_IN_BUF_DETECT_IN,
            /*[0x69]*/
            TRAY_IN_BUF_DETECT_MID,
            /*[0x6A]*/
            TRAY_IN_BUF_DETECT_END,
            /*[0x6B]*/
            TRAY_IN_BUF_DETECT2_IN,
            /*[0x6C]*/
            TRAY_IN_BUF_DETECT2_MID,
            /*[0x6D]*/
            TRAY_IN_BUF_DETECT2_END,
            /*[0x6E]*/
            TRAY_IN_BUF_STOPPER_UP,
            /*[0x6F]*/
            TRAY_IN_BUF_STOPPER_DOWN,

            /*[0x70]*/
            TRAY_WORK_DETECT_IN,
            /*[0x71]*/
            TRAY_WORK_DETECT_MID,
            /*[0x72]*/
            TRAY_WORK_DETECT_END,
            /*[0x73]*/
            TRAY_WORK_DETECT2_IN,
            /*[0x74]*/
            TRAY_WORK_DETECT2_MID,
            /*[0x75]*/
            TRAY_WORK_DETECT2_END,
            /*[0x76]*/
            TRAY_WORK_DETECT_UPPER_LIMIT,
            /*[0x77]*/
            TRAY_TRF_EXIST_DETECT,
            /*[0x78]*/
            TRAY_OUT_BUF_DETECT_IN,
            /*[0x79]*/
            TRAY_OUT_BUF_DETECT_MID,
            /*[0x7A]*/
            TRAY_OUT_BUF_DETECT_END,
            /*[0x7B]*/
            TRAY_OUT_BUF_DETECT2_IN,
            /*[0x7C]*/
            TRAY_OUT_BUF_DETECT2_MID,
            /*[0x7D]*/
            TRAY_OUT_BUF_DETECT2_END,
            /*[0x7E]*/
            TRAY_OUT_BUF_STOPPER_UP,
            /*[0x7F]*/
            TRAY_OUT_BUF_STOPPER_DOWN,

            /*[0x80]*/
            TRAY_OUT_DETECT_IN,
            /*[0x81]*/
            TRAY_OUT_DETECT_MID,
            /*[0x82]*/
            TRAY_OUT_DETECT_END,
            /*[0x83]*/
            TRAY_OUT_DETECT2_IN,
            /*[0x84]*/
            TRAY_OUT_DETECT2_MID,
            /*[0x85]*/
            TRAY_OUT_DETECT2_END,
            /*[0x87]*/
            TRAY_OUT_CONV_UP,
            /*[0x86]*/
            TRAY_OUT_CONV_DOWN,
            /*[0x88]*/
            TRAY_TRF_VACUUM,
            /*[0x89]*/
            BERNOULLI_PAD_ASSY_VAC_ON,
            /*[0x8A]*/
            SP0x8A,
            /*[0x8B]*/
            SP0x8B,
            /*[0x8C]*/
            TRAY_TRF_VAC_ON1,
            /*[0x8D]*/
            TRAY_TRF_DETECT_L_DOWN,
            /*[0x8E]*/
            TRAY_TRF_VAC_ON2,
            /*[0x8F]*/
            TRAY_TRF_DETECT_R_DOWN,

            /*[0x90]*/
            JIG_ALIGN_PRESS_UP,
            /*[0x91]*/
            JIG_ALIGN_PRESS_DOWN,
            /*[0x92]*/
            JIG_ASSEMBLE_PRESS_UP,
            /*[0x93]*/
            JIG_ASSEMBLE_PRESS_DOWN,
            /*[0x94]*/
            SP0x94,
            /*[0x95]*/
            SP0x95,
            /*[0x96]*/
            SP0x96,
            /*[0x97]*/
            SP0x97,
            /*[0x98]*/
            SP0x98,
            /*[0x99]*/
            SP0x99,
            /*[0x9A]*/
            DETACH_SENSOR_1,
            /*[0x9B]*/
            DETACH_SENSOR_2,
            /*[0x9C]*/
            DETACH_SENSOR_3,
            /*[0x9D]*/
            DETACH_SENSOR_4,
            /*[0x9E]*/
            SP0x9E,
            /*[0x9F]*/
            SP0x9F,

            /*[0xA0]*/
            DIVERTER_UPPER_CV_IN_DETECT,
            /*[0xA1]*/
            DIVERTER_UPPER_CV_MID_DETECT,
            /*[0xA2]*/
            DIVERTER_UPPER_CV_OUT_DETECT,
            /*[0xA3]*/
            SP0xA3,
            /*[0xA4]*/
            DIVERTER_UPPER_CV_STOPPER_UP,
            /*[0xA5]*/
            DIVERTER_UPPER_CV_STOPPER_DOWN,
            /*[0xA6]*/
            SP0xA6,
            /*[0xA7]*/
            SP0xA7,
            /*[0xA8]*/
            SP0xA8,
            /*[0xA9]*/
            SP0xA9,
            /*[0xAA]*/
            DIVERTER_LOWER_CV_IN_DETECT,
            /*[0xAB]*/
            DIVERTER_LOWER_CV_MID_DETECT,
            /*[0xAC]*/
            DIVERTER_LOWER_CV_OUT_DETECT,
            /*[0xAD]*/
            SP0xAD,
            /*[0xAE]*/
            DIVERTER_LOWER_CV_STOPPER_UP,
            /*[0xAF]*/
            DIVERTER_LOWER_CV_STOPPER_DOWN,

            /*[0xB0]*/
            SP0xB0,
            /*[0xB1]*/
            SP0xB1,
            /*[0xB2]*/
            SP0xB2,
            /*[0xB3]*/
            SP0xB3,
            /*[0xB4]*/
            SP0xB4,
            /*[0xB5]*/
            SP0xB5,
            /*[0xB6]*/
            SP0xB6,
            /*[0xB7]*/
            SP0xB7,
            /*[0xB8]*/
            SP0xB8,
            /*[0xB9]*/
            SP0xB9,
            /*[0xBA]*/
            SP0xBA,
            /*[0xBB]*/
            SP0xBB,
            /*[0xBC]*/
            SP0xBC,
            /*[0xBD]*/
            SP0xBD,
            /*[0xBE]*/
            SP0xBE,
            /*[0xBF]*/
            SP0xBF,

            /*[0xC0]*/
            SP0xC0,
            /*[0xC1]*/
            SP0xC1,
            /*[0xC2]*/
            SP0xC2,
            /*[0xC3]*/
            SP0xC3,
            /*[0xC4]*/
            SP0xC4,
            /*[0xC5]*/
            SP0xC5,
            /*[0xC6]*/
            SP0xC6,
            /*[0xC7]*/
            SP0xC7,
            /*[0xC8]*/
            SP0xC8,
            /*[0xC9]*/
            SP0xC9,
            /*[0xCA]*/
            SP0xCA,
            /*[0xCB]*/
            SP0xCB,
            /*[0xCC]*/
            SP0xCC,
            /*[0xCD]*/
            SP0xCD,
            /*[0xCE]*/
            SP0xCE,
            /*[0xCF]*/
            SP0xCF,

            IOMAX
        };
        public enum DO
        {
            /*[0x00]*/
            START_SW_LAMP_OP1,
            /*[0x01]*/
            STOP_SW_LAMP_OP1,
            /*[0x02]*/
            RESET_SW_LAMP_OP1,
            /*[0x03]*/
            START_SW_LAMP_OP2,
            /*[0x04]*/
            STOP_SW_LAMP_OP2,
            /*[0x05]*/
            RESET_SW_LAMP_OP2,
            /*[0x06]*/
            SP0x06,
            /*[0x07]*/
            SP0x07,
            /*[0x08]*/
            SAFETY_RESET,
            /*[0x09]*/
            BYPASS_ON,
            /*[0x0A]*/
            LIGHT_CURTAIN_MUTING_IN,
            /*[0x0B]*/
            LIGHT_CURTAIN_MUTING_OUT,
            /*[0x0C]*/
            SP0x0C,
            /*[0x0D]*/
            SP0x0D,
            /*[0x0E]*/
            SP0x0E,
            /*[0x0F]*/
            SP0x0F,

            /*[0x10]*/
            IF_TRAY_IN_AMR_PID1,
            /*[0x11]*/
            IF_TRAY_IN_AMR_PID2,
            /*[0x12]*/
            IF_TRAY_IN_AMR_PID3,
            /*[0x13]*/
            IF_TRAY_OUT_AMR_PID1,
            /*[0x14]*/
            IF_TRAY_OUT_AMR_PID2,
            /*[0x15]*/
            IF_TRAY_OUT_AMR_PID3,
            /*[0x16]*/
            SP0x16,
            /*[0x17]*/
            SP0x17,
            /*[0x18]*/
            IF_MACHINE_RUN_SIG_FOR_FRONT,
            /*[0x19]*/
            IF_MACHINE_RUN_SIG_FOR_REAR,
            /*[0x1A]*/
            IF_JIG_ULD_EXIST_WORK,        // UPSTREAM -> DOWNSTREAM : DOWNSTREAM의 IF_JIG_UPSTREAM_EXIST_CHECK_WORK Input 신호 On/Off 시킴
            /*[0x1B]*/
            IF_JIG_ULD_CONV_RUN_WORK,     // UPSTREAM -> DOWNSTREAM : DOWNSTREAM의 IF_JIG_UPSTREAM_RUN_CHECK_WORK Input 신호 On/Off 시킴
            /*[0x1C]*/
            IF_JIG_LD_CONV_RUN_WORK,      // DOWNSTREAM -> UPSTREAM : UPSTREAM의 IF_JIG_DOWNSTREAM_RUN_CHECK_WORK Input 신호 On/Off 시킴
            /*[0x1D]*/
            IF_JIG_ULD_EXIST_CARRIER,     // UPSTREAM -> DOWNSTREAM : DOWNSTREAM의 IF_JIG_UPSTREAM_EXIST_CHECK_CARRIER Input 신호 On/Off 시킴
            /*[0x1E]*/
            IF_JIG_ULD_CONV_RUN_CARRIER,  // UPSTREAM -> DOWNSTREAM : DOWNSTREAM의 IF_JIG_UPSTREAM_RUN_CHECK_CARRIER Input 신호 On/Off 시킴
            /*[0x1F]*/
            IF_JIG_LD_CONV_RUN_CARRIER,   // DOWNSTREAM -> UPSTREAM : UPSTREAM의 IF_JIG_DOWNSTREAM_RUN_CHECK_CARRIER Input 신호 On/Off 시킴

            /*[0x20]*/
            JIG_IN_BUF_CONV_RUN,
            /*[0x21]*/
            JIG_WORK_CONV_RUN,
            /*[0x22]*/
            JIG_OUT_BUF_CONV_RUN,
            /*[0x23]*/
            JIG_IN_CARRIER_CONV_RUN,
            /*[0x24]*/
            JIG_OUT_CARRIER_CONV_RUN,
            /*[0x25]*/
            TRAY_IN_CONV_RUN,
            /*[0x26]*/
            TRAY_IN_BUF_CONV_RUN,
            /*[0x27]*/
            TRAY_WORK_CONV_RUN,
            /*[0x28]*/
            TRAY_OUT_BUF_CONV_RUN,
            /*[0x29]*/
            TRAY_OUT_CONV_RUN,
            /*[0x2A]*/
            SP0x2A,
            /*[0x2B]*/
            SP0x2B,
            /*[0x2C]*/
            SP0x2C,
            /*[0x2D]*/
            TRAY_IN_CONV_RUN_REVERSE,
            /*[0x2E]*/
            TRAY_IN_BUF_CONV_RUN_REVERSE,
            /*[0x2F]*/
            TRAY_WORK_CONV_RUN_REVERSE,

            /*[0x30]*/
            ASSEMBLER_Z1_VACON,
            /*[0x31]*/
            ASSEMBLER_Z1_PURGE,
            /*[0x32]*/
            ASSEMBLER_Z2_VACON,
            /*[0x33]*/
            ASSEMBLER_Z2_PURGE,
            /*[0x34]*/
            CAM_PICKER_Z1_VACON,
            /*[0x35]*/
            CAM_PICKER_Z1_PURGE,
            /*[0x36]*/
            CAM_PICKER_Z2_VACON,
            /*[0x37]*/
            CAM_PICKER_Z2_PURGE,
            /*[0x38]*/
            SP0x38,
            /*[0x39]*/
            SP0x39,
            /*[0x3A]*/
            TRAY_TRF_EXIST_UP,
            /*[0x3B]*/
            TRAY_TRF_EXIST_DOWN,
            /*[0x3C]*/
            TOWER_LAMP_RED,
            /*[0x3D]*/
            TOWER_LAMP_YELLOW,
            /*[0x3E]*/
            TOWER_LAMP_GREEN,
            /*[0x3F]*/
            BUZZER_ON,

            /*[0x40]*/
            SP0x40,
            /*[0x41]*/
            SP0x41,
            /*[0x42]*/
            SP0x42,
            /*[0x43]*/
            SP0x43,
            /*[0x44]*/
            SP0x44,
            /*[0x45]*/
            SP0x45,
            /*[0x46]*/
            SP0x46,
            /*[0x47]*/
            SP0x47,
            /*[0x48]*/
            SP0x48,
            /*[0x49]*/
            SP0x49,
            /*[0x4A]*/
            SP0x4A,
            /*[0x4B]*/
            SP0x4B,
            /*[0x4C]*/
            SP0x4C,
            /*[0x4D]*/
            SP0x4D,
            /*[0x4E]*/
            SP0x4E,
            /*[0x4F]*/
            SP0x4F,

            /*[0x50]*/
            SP0x50,
            /*[0x51]*/
            SP0x51,
            /*[0x52]*/
            SP0x52,
            /*[0x53]*/
            SP0x53,
            /*[0x54]*/
            SP0x54,
            /*[0x55]*/
            SP0x55,
            /*[0x56]*/
            SP0x56,
            /*[0x57]*/
            SP0x57,
            /*[0x58]*/
            SP0x58,
            /*[0x59]*/
            SP0x59,
            /*[0x5A]*/
            SP0x5A,
            /*[0x5B]*/
            SP0x5B,
            /*[0x5C]*/
            SP0x5C,
            /*[0x5D]*/
            SP0x5D,
            /*[0x5E]*/
            SP0x5E,
            /*[0x5F]*/
            SP0x5F,

            /*[0x60]*/
            SP0x60,
            /*[0x61]*/
            SP0x61,
            /*[0x62]*/
            SP0x62,
            /*[0x63]*/
            SP0x63,
            /*[0x64]*/
            SP0x64,
            /*[0x65]*/
            SP0x65,
            /*[0x66]*/
            SP0x66,
            /*[0x67]*/
            SP0x67,
            /*[0x68]*/
            SP0x68,
            /*[0x69]*/
            SP0x69,
            /*[0x6A]*/
            SP0x6A,
            /*[0x6B]*/
            SP0x6B,
            /*[0x6C]*/
            SP0x6C,
            /*[0x6D]*/
            SP0x6D,
            /*[0x6E]*/
            SP0x6E,
            /*[0x6F]*/
            SP0x6F,

            /*[0x70]*/
            JIG_WORK2_PALLET_LOCK_FWD,
            /*[0x71]*/
            JIG_WORK2_PALLET_LOCK_BWD,
            /*[0x72]*/
            JIG_ALIGN_PRESS_UP,
            /*[0x73]*/
            JIG_ALIGN_PRESS_DOWN,
            /*[0x74]*/
            JIG_ASSEMBLE_PRESS_UP,
            /*[0x75]*/
            JIG_ASSEMBLE_PRESS_DOWN,
            /*[0x76]*/
            SP0x76,
            /*[0x77]*/
            SP0x77,
            /*[0x78]*/
            SP0x78,
            /*[0x79]*/
            SP0x79,
            /*[0x7A]*/
            SP0x7A,
            /*[0x7B]*/
            SP0x7B,
            /*[0x7C]*/
            SP0x7C,
            /*[0x7D]*/
            SP0x7D,
            /*[0x7E]*/
            SP0x7E,
            /*[0x7F]*/
            SP0x7F,

            /*[0x80]*/
            JIG_IN_BUF_STOPPER_UP,
            /*[0x81]*/
            JIG_IN_BUF_STOPPER_DOWN,
            /*[0x82]*/
            JIG_WORK_STOPPER_UP,
            /*[0x83]*/
            JIG_WORK_STOPPER_DOWN,
            /*[0x84]*/
            JIG_OUT_BUF_STOPPER_UP,
            /*[0x85]*/
            JIG_OUT_BUF_STOPPER_DOWN,
            /*[0x86]*/
            JIG_IN_CARRIER_STOPPER_UP,
            /*[0x87]*/
            JIG_IN_CARRIER_STOPPER_DOWN,
            /*[0x88]*/
            JIG_OUT_CARRIER_STOPPER_UP,
            /*[0x89]*/
            JIG_OUT_CARRIER_STOPPER_DOWN,
            /*[0x8A]*/
            JIG_WORK_PALLETE_FWD,
            /*[0x8B]*/
            JIG_WORK_PALLETE_BWD,
            /*[0x8C]*/
            ASSEMBLER_Z1_UP,
            /*[0x8D]*/
            ASSEMBLER_Z1_DOWN,
            /*[0x8E]*/
            ASSEMBLER_Z2_UP,
            /*[0x8F]*/
            ASSEMBLER_Z2_DOWN,

            /*[0x90]*/
            CAM_PICKER_Z1_UP,
            /*[0x91]*/
            CAM_PICKER_Z1_DOWN,
            /*[0x92]*/
            CAM_PICKER_Z2_UP,
            /*[0x93]*/
            CAM_PICKER_Z2_DOWN,
            /*[0x94]*/
            TRAY_IN_STOPPER_UP,
            /*[0x95]*/
            TRAY_IN_STOPPER_DOWN,
            /*[0x96]*/
            TRAY_IN_BUF_STOPPER_UP,
            /*[0x97]*/
            TRAY_IN_BUF_STOPPER_DOWN,
            /*[0x98]*/
            TRAY_OUT_CONV_UP,
            /*[0x99]*/
            TRAY_OUT_CONV_DOWN,
            /*[0x9A]*/
            TRAY_OUT_BUF_STOPPER_UP,
            /*[0x9B]*/
            TRAY_OUT_BUF_STOPPER_DOWN,
            /*[0x9C]*/
            SP0x9D,
            /*[0x9D]*/
            SP0x9E,
            /*[0x9E]*/
            SP0x9F,
            /*[0x9F]*/
            TRAY_TRF_VACUUM,

            IOMAX
        };
    }
}
