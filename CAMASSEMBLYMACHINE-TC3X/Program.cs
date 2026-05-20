using CAMASSEMBLYMACHINE.UI;
using CAMASSEMBLYMACHINE.Define;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;
using TopEng.Utils;
using static CAMASSEMBLYMACHINE.Define.SystemDefine;
using CAMASSEMBLYMACHINE.Process;
using System.Reflection;

namespace CAMASSEMBLYMACHINE
{
    static class Program
    {
        public static ProcessError errorProc;
        public static TopEng.Vision.CogLicense license = new TopEng.Vision.CogLicense();

        /// <summary>
        /// 해당 애플리케이션의 주 진입점입니다.
        /// </summary>
        [STAThread]
        static void Main()
        {
            license.ValidLicense();

            System.Diagnostics.Process[] processes = null;
            string curProcess = System.Diagnostics.Process.GetCurrentProcess().ProcessName.ToUpper();
            processes = System.Diagnostics.Process.GetProcessesByName(curProcess);
            if (processes.Length > 1)
            {
                MessageBox.Show("Same program has executed.\r\nProgram will be closed");
                return;
            }

            //bin\\pathSystemReg
            string firstLine = @"C:\FA";
            string firstFilePath = System.IO.Path.Combine(firstLine, "pathSystemReg");

            if (System.IO.File.Exists(firstFilePath))
            {
                using (System.IO.StreamReader sr = new System.IO.StreamReader(firstFilePath))
                {
                    SystemDefine.pathSystemReg = $@"{sr.ReadLine()}";
                    SystemDefine.machineName = SystemDefine.pathSystemReg.Split('\\')[3];
                    SystemDefine.UpdateData();
                }
            }
            else
            {
                MessageBox.Show("Please Input pathSystemReg File in " + firstLine);
                return;
            }

            ReadRegistery();

            ParameterDefine.DefineData();
            RecipeDefine.DefineData();
            StatusDefine.DefineData();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Application.Run(new MainForm());
            license.Shutdown();
        }

        static void ReadRegistery()
        {
            string language = "en-US";
            //RegistryKey regKey = Registry.CurrentUser.OpenSubKey(SystemDefine.pathSystemReg);
            RegistryKey wregKey = Registry.CurrentUser.OpenSubKey(SystemDefine.pathSystemReg, true);

            if (wregKey == null)
                wregKey = Registry.CurrentUser.CreateSubKey(SystemDefine.pathSystemReg);

            if (wregKey != null)
            {
                language = (string)wregKey.GetValue("Language", "en-US");
                // SYSTEM SETTING
                SystemDefine.machineName = (string)wregKey.GetValue("Machine Name", "CAMASSEMBLYMACHINE");
                wregKey.SetValue("Machine Name", SystemDefine.machineName);
                SystemDefine.systemName = (string)wregKey.GetValue("System Name", "CAMASSEMBLYMACHINE-C-V1");
                wregKey.SetValue("System Name", SystemDefine.systemName);
                SystemDefine.externalEquipServerIP = (string)wregKey.GetValue("Server IP", "192.168.10.100");
                wregKey.SetValue("Server IP", SystemDefine.externalEquipServerIP);
                SystemDefine.externalEquipServerPort = (int)wregKey.GetValue("Server Port", 8888);
                wregKey.SetValue("Server Port", SystemDefine.externalEquipServerPort);
                // SYSTEM DATA
                SystemDefine.modelname = (string)wregKey.GetValue("Model Name", "none");
                wregKey.SetValue("Model Name", SystemDefine.modelname);
                SystemDefine.passwordOrigin = (string)wregKey.GetValue("Authority Origin Password", "1234");
                wregKey.SetValue("Developer Password", SystemDefine.passwordOrigin);
                SystemDefine.passwordData = (string)wregKey.GetValue("Authority Data Password", "1234");
                wregKey.SetValue("Data Password", SystemDefine.passwordData);
                // OPTION
                Machine.DeveloperMode = Convert.ToBoolean(wregKey.GetValue("Developer Mode", false));
                wregKey.SetValue("Developer Mode", Machine.DeveloperMode);
                Machine.DoorOpenDisregard = Convert.ToBoolean(wregKey.GetValue("Door-Open Disregard", false));
                wregKey.SetValue("Door-Open Disregard", Machine.DoorOpenDisregard);
                Machine.MutingOnDisregard = Convert.ToBoolean(wregKey.GetValue("Muting On Disregard", false));
                wregKey.SetValue("Door-Open Disregard", Machine.DoorOpenDisregard);
                Machine.AloneMode = false;
                wregKey.SetValue("Alone Mode", Machine.AloneMode);
                Machine.TrayHolding = Convert.ToBoolean(wregKey.GetValue("Tray Holding", false));
                wregKey.SetValue("Tray Holding", Machine.TrayHolding);
                Machine.RandomTest = Convert.ToBoolean(wregKey.GetValue("Random Input Test", false));
                wregKey.SetValue("Random Input Test", Machine.RandomTest);
                // OPTION - VISION
                SystemDefine.CamCountSim = Convert.ToInt16(wregKey.GetValue("Camera Count", "0"));
                wregKey.SetValue("Camera Count", SystemDefine.CamCountSim.ToString());
                SystemDefine.UpdateCameraInfo = Convert.ToBoolean(wregKey.GetValue("Update Camera Info", true));
                wregKey.SetValue("Update Camera Info", SystemDefine.UpdateCameraInfo);
                SystemDefine.deletelogperiod = Convert.ToInt16(wregKey.GetValue("Delete Log Period", "7"));
                wregKey.SetValue("Delete Log Period", SystemDefine.deletelogperiod.ToString());
                SystemDefine.deleteimageperiod = Convert.ToInt16(wregKey.GetValue("Delete Image Period", "1"));
                wregKey.SetValue("Delete Image Period", SystemDefine.deleteimageperiod.ToString());
                SystemDefine.UseLightOff = Convert.ToBoolean(wregKey.GetValue("Use Light Off", true));
                wregKey.SetValue("Use Light Off", SystemDefine.UseLightOff);
                SystemDefine.UseImageSave = Convert.ToBoolean(wregKey.GetValue("Use Image Save", true));
                wregKey.SetValue("Use Image Save", SystemDefine.UseImageSave);
                SystemDefine.UseLowQualityImage = Convert.ToBoolean(wregKey.GetValue("Use Low Quality Image", true));
                wregKey.SetValue("Use Low Quality Image", SystemDefine.UseLowQualityImage);
                SystemDefine.manualGrab = Convert.ToBoolean(wregKey.GetValue("Manual Grab", false));
                wregKey.SetValue("Manual Grab", SystemDefine.manualGrab);
                SystemDefine.motorVelocity = Convert.ToDouble(wregKey.GetValue("Motor Velocity", 100));
                wregKey.SetValue("Motor Velocity", SystemDefine.motorVelocity);
                // NOTEBOOK, VISION MODE
                SystemDefine.IS_NOTEBOOK_MODE = Convert.ToBoolean(wregKey.GetValue("Is Notebook Mode", true));
                wregKey.SetValue("Is Notebook Mode", SystemDefine.IS_NOTEBOOK_MODE);
                SystemDefine.NO_VISION_KEY = Convert.ToBoolean(wregKey.GetValue("No Vision Key", true));
                wregKey.SetValue("No Vision Key", SystemDefine.NO_VISION_KEY);

                SystemDefine.UpdateData();
            }
        }

        public static void UpdateRegistry()
        {
            RegistryKey wregKey = Registry.CurrentUser.OpenSubKey(SystemDefine.pathSystemReg, true);

            if (wregKey == null)
                wregKey = Registry.CurrentUser.CreateSubKey(SystemDefine.pathSystemReg);

            wregKey.SetValue("Model Name", SystemDefine.modelname);
            wregKey.SetValue("Delete Log Period", SystemDefine.deletelogperiod.ToString());
            wregKey.SetValue("Delete Image Period", SystemDefine.deleteimageperiod.ToString());
            wregKey.SetValue("Use Image Save", SystemDefine.UseImageSave);
            wregKey.SetValue("Use Low Quality Image", SystemDefine.UseLowQualityImage);
        }

        public static string GetAppTitle()
        {
            AssemblyTitleAttribute attributes = (AssemblyTitleAttribute)Attribute.GetCustomAttribute(Assembly.GetExecutingAssembly(), typeof(AssemblyTitleAttribute), false);
            return attributes?.Title;
        }
    }
}
