using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using TopEng.Utils;
using CAMASSEMBLYMACHINE.Define;
using static CAMASSEMBLYMACHINE.Define.SystemDefine;
using System.Threading;

namespace CAMASSEMBLYMACHINE
{
    static class EDM
    {
        public static string getMachineVersion()
        {
            //FileVersionInfo fileVersion = FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly().Location);
            string productName = "CAMASSEMBLYMACHINE-C-V1";
            string Version = SystemDefine.pgmVersion;/*fileVersion.ProductVersion*/
            return $"{productName}_{Version}";
        }

        public static void Start()
        {
            bool report = Machine.param.Option(ParameterDefine.OPTION.REPORT_EDM_SERVER) == 1 ? true : false;
            EDMUtil.RUNMODE runmode = EDMUtil.RUNMODE.AUTO;
            if (Machine.status.mode == SystemMode.SystemModeBYPASS) runmode = EDMUtil.RUNMODE.BYPASS;
            if (Machine.status.mode == SystemMode.SystemModeDRYRUN) runmode = EDMUtil.RUNMODE.DRYRUN;

            if (report)
                EDMUtil.Instance(SystemDefine.EDMLogPath).Start("", getMachineVersion(), "00000000", runmode);
        }

        public static void Stop()
        {
            bool report = Machine.param.Option(ParameterDefine.OPTION.REPORT_EDM_SERVER) == 1 ? true : false;
            if (report)
                EDMUtil.Instance(SystemDefine.EDMLogPath).Stop("", getMachineVersion(), "00000000");
        }

        public static void TowerLampChanged()
        {
            bool report = Machine.param.Option(ParameterDefine.OPTION.REPORT_EDM_SERVER) == 1 ? true : false;
            uint red = 0, yellow = 0, green = 0;
            Machine.IO.GetOut((int)DO.TOWER_LAMP_RED, ref red);
            Machine.IO.GetOut((int)DO.TOWER_LAMP_YELLOW, ref yellow);
            Machine.IO.GetOut((int)DO.TOWER_LAMP_GREEN, ref green);

            if (report)
                EDMUtil.Instance(SystemDefine.EDMLogPath).TowerLamp("", getMachineVersion(), "00000000", red, yellow, green);
        }

        public static void PartCount(uint inputCount, uint totalCount, uint alarmLimit)
        {
            bool report = Machine.param.Option(ParameterDefine.OPTION.REPORT_EDM_SERVER) == 1 ? true : false;
            if (report)
                EDMUtil.Instance(SystemDefine.EDMLogPath).PartsCount("", getMachineVersion(), "00000000", inputCount, totalCount, alarmLimit, "1");
        }

        public static void SetsCount(uint setCount)
        {
            bool report = Machine.param.Option(ParameterDefine.OPTION.REPORT_EDM_SERVER) == 1 ? true : false;
            if (report)
                EDMUtil.Instance(SystemDefine.EDMLogPath).SetsCount("", getMachineVersion(), "00000000", setCount);
        }

        public static void LoadingSets(bool wait)
        {
            bool report = Machine.param.Option(ParameterDefine.OPTION.REPORT_EDM_SERVER) == 1 ? true : false;
            if (report)
                EDMUtil.Instance(SystemDefine.EDMLogPath).LoadingSets("", getMachineVersion(), "00000000", wait);
        }

        public static void UnloadingSetsGood(bool wait)
        {
            bool report = Machine.param.Option(ParameterDefine.OPTION.REPORT_EDM_SERVER) == 1 ? true : false;
            if (report)
                EDMUtil.Instance(SystemDefine.EDMLogPath).UnloadingSetsGood("", getMachineVersion(), "00000000", wait);
        }

        public static void UnloadingSetsNG(string NGreason, bool wait)
        {
            bool report = Machine.param.Option(ParameterDefine.OPTION.REPORT_EDM_SERVER) == 1 ? true : false;
            if (report)
                EDMUtil.Instance(SystemDefine.EDMLogPath).UnloadingSetsNG("", getMachineVersion(), "00000000", NGreason, wait);
        }

        public static void SetNGBoxStatus(int currentCount, int maxCount, bool wait)
        {
            bool report = Machine.param.Option(ParameterDefine.OPTION.REPORT_EDM_SERVER) == 1 ? true : false;
            if (report)
                EDMUtil.Instance(SystemDefine.EDMLogPath).SetNGBoxStatus("", getMachineVersion(), "00000000", currentCount, maxCount, wait);
        }


        public static void Alarm(ECODE err_no)
        {
            bool report = Machine.param.Option(ParameterDefine.OPTION.REPORT_EDM_SERVER) == 1 ? true : false;
            if (report)
                EDMUtil.Instance(SystemDefine.EDMLogPath).Alarm("", getMachineVersion(), "00000000", (uint)err_no);
        }
    }
}
