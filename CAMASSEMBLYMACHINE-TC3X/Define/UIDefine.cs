using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAMASSEMBLYMACHINE.Define
{
    public static class UIDefine
    {
        public enum UIAXISINFO
        {
            NO,
            NAME,
            ACTUALPOS,
            NLIMIT,
            PLIMIT,
            HOME,
            SERVO,
            ALARM,

            UIAXISINFOMAX
        };

        public enum UIPARAMINFO
        {
            NAME,
            STORED,
            EDITED,
            GETPOS,
            ACCEPT,

            UIPARAMINFOMAX
        };

        public enum UIPARAMCOLTEACH
        {
            NAME,
            STORED,
            EDITED,
            GETPOS,
            ACCEPT,

            UIPARAMCOLTEACHMAX
        };

        public enum UIPARAMCOLDATA
        {
            NAME,
            STORED,
            EDITED,
            UNIT,
            ACCEPT,

            UIPARAMCOLTEACHMAX
        };

        public enum UIWORKINGTIMECOLDATA
        {
            NAME,
            STORED,
            EDITED,
            ACCEPT,

            UIWORKINGTIMECOLTEACHMAX
        };

        public enum UIJOGVELINFO
        {
            SLOWEST,
            SLOWER,
            NORMAL,
            FASTER,
            FASTEST,

            UIVELINFOMAX,
        }

        public static double[] UIJOGVEL = { 1 * 1000, 10 * 1000, 50 * 1000, 100 * 1000, 200 * 1000 };
    }
}
