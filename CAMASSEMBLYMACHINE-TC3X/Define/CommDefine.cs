using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAMASSEMBLYMACHINE.Define
{
    public enum MACHINE_NAME
    {
        WC,
        UWC,
        X5,
        X3,
        TAPE,
        TURN,
        MAX
    }

    public enum EXT_EQUIP_ACTION
    {
        NONE = -1,
        SET_PO,
        CHECK_PO
    }

    public enum MESSAGE_TYPE
    {
        REQUEST,
        RESPONSE,
        MAX
    }

    public class EXT_EQUIP_STRUCTURE
    {
        public static int DATA_MACHINE_NAME = 0;
        public static int DATA_ACTION = 1;
        public static int DATA_MESSAGE_TYPE = 2;
        public static int DATA_MESSAGE_CONTENT = 3;
        public static int DATA_MAX = 4;
    }

    public enum PO_STRUCTURE
    {
        PO_TYPE,
        DATA_NUMBER,
        DATA
    }

    public enum CHECK_PO_RESPONSE_STRUCTURE
    {
        RESULT
    }

    public enum PO_RESULT
    {
        NONE,
        TRUE,
        FALSE
    }
}
