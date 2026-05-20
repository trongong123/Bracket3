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
    public class StatusDefine
    {
        public enum PRODUCT
        {
            PRODUCT_UNKNOWN = -1,
            PRODUCT_INPUT_TOTAL_COUNT,
            PRODUCT_INPUT_PARTS_COUNT,
            PRODUCT_INPUT_ALARM_LIMIT,
            PRODUCT_OUTPUT_GOOD_COUNT,
            PRODUCT_OUTPUT_NG_COUNT,

            TRAY_ONE_LOADING_QUANTITY,
            TRAY_UNLOADING_COUNT,

            TRAY_PLACE_COUNT,
            PRODUCT_PLACE_COUNT,

            PRODUCT_MAX_COUNT
        }

        public static List<string> product = new List<string>();

        public static void DefineData()
        {
            product.Clear();
            for (int i = 0; i < (int)PRODUCT.PRODUCT_MAX_COUNT; i++)
                product.Add(Enum.GetName(typeof(PRODUCT), i));
        }

        public static bool ProductDataCheck(Dictionary<string, STATUSSTRUCT> dic, string path)
        {
            foreach (var paramString in product)
            {
                if (!dic.ContainsKey(paramString))
                {
                    Dlg_MessageBox formErr = 
                        new Dlg_MessageBox(EMESSAGEBOX.MSG, 
                        string.Format($"[{paramString}] 키가 없습니다."),
                        path);
                    formErr.TopLevel = true;
                    formErr.TopMost = true;
                    formErr.ShowDialog();
                    return false;
                }
            }
            return true;
        }
    }
}
