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
    public static class RecipeDefine
    {
        public enum POSITION
        {
            POSITION_UNKNOWN = -1,
            PROD_LOADER_X_BUF_L_L_POS,
            PROD_LOADER_Y_BUF_L_L_POS,
            PROD_LOADER_X_BUF_L_R_POS,
            PROD_LOADER_Y_BUF_L_R_POS, 
            PROD_LOADER_X_BUF_R_L_POS,
            PROD_LOADER_Y_BUF_R_L_POS,
            PROD_LOADER_X_BUF_R_R_POS,
            PROD_LOADER_Y_BUF_R_R_POS,
            PROD_LOADER_Z_PICKUP_POS,
            PROD_LOADER_Z_PICKUP_STEP,
            PROD_LOADER_Z_PLACE_POS,
            PROD_LOADER_R_PICKUP_POS,
            PROD_LOADER_R_PLACE_L_L_POS,
            PROD_LOADER_R_PLACE_L_R_POS,
            PROD_LOADER_R_PLACE_R_L_POS,
            PROD_LOADER_R_PLACE_R_R_POS,
            PROD_LOADER_X_ALIGN_L_L_POS,
            PROD_LOADER_Y_ALIGN_L_L_POS,
            PROD_LOADER_X_ALIGN_L_R_POS,
            PROD_LOADER_Y_ALIGN_L_R_POS,
            PROD_LOADER_X_ALIGN_R_L_POS,
            PROD_LOADER_Y_ALIGN_R_L_POS,
            PROD_LOADER_X_ALIGN_R_R_POS,
            PROD_LOADER_Y_ALIGN_R_R_POS,
            PROD_LOADER_Z_ALIGN_POS,
            PROD_LOADER_R_ALIGN_L_L_POS,
            PROD_LOADER_R_ALIGN_L_R_POS,
            PROD_LOADER_R_ALIGN_R_L_POS,
            PROD_LOADER_R_ALIGN_R_R_POS,

            CAM_TRANSFER_Y_ALIGN_READY_L_POS,
            CAM_TRANSFER_Y_ALIGN_READY_R_POS,
            CAM_TRANSFER_Y_ALIGN_L_POS,
            CAM_TRANSFER_Y_ALIGN_R_POS,
            CAM_TRANSFER_Y_READY_L_POS,
            CAM_TRANSFER_Y_READY_R_POS,
            CAM_TRANSFER_Y_LOADING_L_POS,
            CAM_TRANSFER_Y_LOADING_R_POS,
            CAM_TRANSFER_Y_PEELING_L_POS,
            CAM_TRANSFER_Y_PEELING_R_POS,
            CAM_TRANSFER_Y_UNLOADING_L_POS,
            CAM_TRANSFER_Y_UNLOADING_R_POS,
            ASSEMBLER_X_BUF_L_POS,
            ASSEMBLER_Y_BUF_L_POS,
            ASSEMBLER_X_BUF_R_POS,
            ASSEMBLER_Y_BUF_R_POS,
            ASSEMBLER_X_UNDER_ALIGN_POS,
            ASSEMBLER_Y_UNDER_ALIGN_POS,
            ASSEMBLER_X_UNDER_ALIGN2_POS,
            ASSEMBLER_Y_UNDER_ALIGN2_POS,
            ASSEMBLER_Z_UNDER_ALIGN_POS,
            ASSEMBLER_Z_SCAN_POS,
            ASSEMBLER_Z_PICKUP_POS,
            ASSEMBLER_Z_PLACE_L_POS,
            ASSEMBLER_Z_PLACE_R_POS,
            ASSEMBLER_R_PICKUP_POS,
            ASSEMBLER_R_UNDER_ALIGN_POS,
            ASSEMBLER_R_PLACE_POS,
            ASSEMBLER_X_SCANNER_L_POS,
            ASSEMBLER_Y_SCANNER_L_POS,
            ASSEMBLER_R_SCANNER_L_POS,
            ASSEMBLER_X_SCANNER_R_POS,
            ASSEMBLER_Y_SCANNER_R_POS,
            ASSEMBLER_R_SCANNER_R_POS,
            POSITION_MAX_COUNT
        }

        public enum CALIBRATION
        {
            CALIBRATION_UNKNOWN = -1,
            PROD_LOADER_OFFSET_LEFT_X,
            PROD_LOADER_OFFSET_LEFT_Y,
            PROD_LOADER_OFFSET_RIGHT_X,
            PROD_LOADER_OFFSET_RIGHT_Y,
            PROD_LOADER_OFFSET_LEFT_DEG180_X,
            PROD_LOADER_OFFSET_LEFT_DEG180_Y,
            PROD_LOADER_OFFSET_RIGHT_DEG180_X,
            PROD_LOADER_OFFSET_RIGHT_DEG180_Y,
            ASSEMBLER_OFFSET1_X,
            ASSEMBLER_OFFSET1_Y,
            ASSEMBLER_OFFSET1_R,
            ASSEMBLER_OFFSET2_X,
            ASSEMBLER_OFFSET2_Y,
            ASSEMBLER_OFFSET2_R,

            CALIBRATION_MAX_COUNT
        }

        public enum OPTION
        {
            OPTION_UNKNOWN = -1,
            UNLOADING_COUNT_TYPE,
            UNLOADING_TRAYS_COUNTS,
            UNLOADING_PRODUCTS_COUNTS,
            USAGE_JIGS_COUNTS,
            PATTERN_OVERLAP_LENGTH,
            USE_ASSEMBLY_MODE,
            USE_PEELING_UP,
            USE_PEELING_DOWN,
            PEELING_USE,
            LD_PICKER_ANGLE_FIX,
            CAM_TRF_GRIP_USE,
            TRF_CAM_ALIGN_USE,
            
            MAX_PART_IN_LEFT_NG_BOX_X,
            MAX_PART_IN_LEFT_NG_BOX_Y,
            MAX_PART_IN_RIGHT_NG_BOX_X,
            MAX_PART_IN_RIGHT_NG_BOX_Y,
            LEFT_NG_DISTANCE_X,
            LEFT_NG_DISTANCE_Y,
            RIGHT_NG_DISTANCE_X,
            RIGHT_NG_DISTANCE_Y,

            PRODUCT_COUNT_IN_TRAY1,
            PRODUCT_COUNT_IN_TRAY2,
            PRODUCT_COUNT_IN_TRAY3,
            PRODUCT_COUNT_IN_TRAY4,
            PRODUCT_COUNT_IN_TRAY5,
            OPTION_MAX_COUNT
        }

        public static List<string> position = new List<string>();
        public static List<string> calibration = new List<string>();
        public static List<string> option = new List<string>();

        public static void DefineData()
        {
            position.Clear();
            for (int i = 0; i < (int)POSITION.POSITION_MAX_COUNT; i++)
                position.Add(Enum.GetName(typeof(POSITION), i));

            calibration.Clear();
            for (int i = 0; i < (int)CALIBRATION.CALIBRATION_MAX_COUNT; i++)
                calibration.Add(Enum.GetName(typeof(CALIBRATION), i));

            option.Clear();
            for (int i = 0; i < (int)OPTION.OPTION_MAX_COUNT; i++)
                option.Add(Enum.GetName(typeof(OPTION), i));
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
                    Environment.Exit(0); ;

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
    }
}
