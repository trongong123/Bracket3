using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using CAMASSEMBLYMACHINE.Define;
using static CAMASSEMBLYMACHINE.Define.SystemDefine;

namespace TopEng.Utils
{
    public class AlarmUtil
    {
        public int MaxQueueSize = 500 * 1024; // 500k
        public int MaxStoreDays = 7; //days
        private SynchronizationContext syncContext;
        private ConcurrentQueue<ALARMINFO> queue = new ConcurrentQueue<ALARMINFO>();
        private Object queueLock = new object();
        private string filepath;
        private AlarmList alarmList = new AlarmList();
        private SubForm_Alarm2 alarmDlg = new SubForm_Alarm2();
        private Dictionary<int, ALARMINFO> dic_alarm = new Dictionary<int, ALARMINFO>();
        public bool showDialog = false;
        private bool clearDialog = false;
        private readonly object lockObj = new object();

        public delegate void AlarmLogDelegate(int code, string msg);
        public event AlarmLogDelegate AlarmLogEvent;
        public delegate void AlarmSoundOnDelegate();
        public event AlarmSoundOnDelegate AlarmSoundOnEvent;
        public delegate void AlarmMuteDelegate();
        public event AlarmMuteDelegate AlarmMuteEvent;

        protected Thread threadProc;
        protected ManualResetEvent threadEvent;

        public AlarmUtil(string alarmpath, string logpath)
        {
            syncContext = SynchronizationContext.Current;
            alarmList.Read(alarmpath);
            this.filepath = logpath;
            alarmDlg.BuzzerSoundOnEvent += AlarmSoundOn;
            alarmDlg.BuzzerSoundOffEvent += AlarmMute;
            alarmDlg.TopMost = true;

            threadEvent = new ManualResetEvent(true);
            threadProc = new Thread(new ThreadStart(OnThreading));
            threadEvent.Set();
            threadProc.Start();

            alarmDlg.Show();
            alarmDlg.Hide();
        }

        ~AlarmUtil()
        {
            var alarm = new ALARMINFO();
            while (!queue.IsEmpty)
            {
                queue.TryDequeue(out alarm);
                WriteLog(alarm);
                Thread.Sleep(20);
            }
        }

        public bool hasError()
        {
            return dic_alarm.Count > 0;
        }

        public ALARMINFO.CATEGORY GetCategory(int code)
        {
            var alarm = alarmList.GetAlarm(code);
            if (alarm == null)
                return ALARMINFO.CATEGORY.NONE;
            return alarm.Category;
        }

        public void ExportCSV()
        {
            SaveFileDialog ofd = new SaveFileDialog();
            ofd.Title = "Export error list";
            ofd.FileName = "errorlist";
            ofd.Filter = "CSV File (*.csv) | *.csv";
            DialogResult res = ofd.ShowDialog();

            if (res != DialogResult.OK)
                return;

            string appName = CAMASSEMBLYMACHINE.Program.GetAppTitle();
            string filename = ofd.FileName;
            using (StreamWriter sw = new StreamWriter(filename, false, Encoding.UTF8))
            {
                foreach (var item in alarmList.alarm)
                {
                    var alarm = item.Value;
                    string alarmLevel = "Unknown";
                    if (alarm.ErrorLevel == ALARMINFO.LEVEL.OP)
                        alarmLevel = "Warning";
                    else if (alarm.ErrorLevel == ALARMINFO.LEVEL.EN)
                        alarmLevel = "Critical";

                    sw.WriteLine($"{SystemDefine.systemName + "-" + SystemDefine.productName},{alarm.Code},Y,{alarmLevel},{alarm.ErrorLevel}" +
                        $",{((ECODE)alarm.Code).ToString()},{alarm.Message}, ");
                }

                sw.Close();
            }
        }

        public void Add(int code, bool write = true)
        {
            lock (lockObj)
            {
                var alarm = alarmList.GetAlarm(code);
                if (alarm == null)
                {
                    alarm = new ALARMINFO();
                    alarm.Code = code;
                    alarm.Message = Enum.GetName(typeof(CAMASSEMBLYMACHINE.Define.ECODE), code);
                    alarm.Category = ALARMINFO.CATEGORY.PROCESS;
                    alarm.SoundTime = 1;
                    alarm.ErrorLevel = ALARMINFO.LEVEL.UNKNOWN;
                }
                if (code <= 0)
                    return;

                alarm.Message = Enum.GetName(typeof(CAMASSEMBLYMACHINE.Define.ECODE), code);

                string strCode = ((ECODE)alarm.Code).ToString();
                string strCheckAxisNum = "";
                if (strCode.Contains("AXIS"))
                {
                    string[] arrStrCode = strCode.Split('_');
                    for (int aN = 0; aN < arrStrCode.Length; ++aN)
                    {
                        if (arrStrCode[aN].Contains("AXIS"))
                        {
                            strCheckAxisNum = arrStrCode[aN].Replace("AXIS", "");
                            if (int.TryParse(strCheckAxisNum, out int checkAxis))
                            {
                                string strCheckAxis = ((AXIS)checkAxis).ToString();
                                alarm.Message += " [" + strCheckAxis + "]";
                                //5번같은 경우 축이 다를 수 있으니 Unit도 재지정한다.
                                if (strCheckAxis.ToUpper().Contains("TRF"))
                                    alarm.Unit = ALARMINFO.UNIT.TRAYOUTBUF;
                                else if (strCheckAxis.ToUpper().Contains("WORK"))
                                    alarm.Unit = ALARMINFO.UNIT.TRAYWORK;
                                else if (strCheckAxis.ToUpper().Contains("PROD") ||
                                    strCheckAxis.ToUpper().Contains("UNLOAD"))
                                    alarm.Unit = ALARMINFO.UNIT.LD_PICKER;
                                else if (strCheckAxis.ToUpper().Contains("ASSEMBLER") ||
                                    strCheckAxis.ToUpper().Contains("ASSY"))
                                    alarm.Unit = ALARMINFO.UNIT.ULD_PICKER;
                                else if (strCheckAxis.ToUpper().Contains("LEFT"))
                                    alarm.Unit = ALARMINFO.UNIT.LEFTBUF;
                                else if (strCheckAxis.ToUpper().Contains("RIGHT"))
                                    alarm.Unit = ALARMINFO.UNIT.RIGHTBUF;
                                else if (strCheckAxis.ToUpper().Contains("JIG_IN"))
                                    alarm.Unit = ALARMINFO.UNIT.JIGINBUF;
                                else if (strCheckAxis.ToUpper().Contains("JIG_OUT"))
                                    alarm.Unit = ALARMINFO.UNIT.JIGOUTBUF;
                                else if (strCheckAxis.ToUpper().Contains("ROLL1"))
                                    alarm.Unit = ALARMINFO.UNIT.TAPE_ROLL1;
                                else if (strCheckAxis.ToUpper().Contains("ROLL2"))
                                    alarm.Unit = ALARMINFO.UNIT.TAPE_ROLL2;
                            }
                        }
                    }
                }

                alarm.Date = DateTime.Now;
                alarm.WriteLog = write;

                if (Find(code))
                {
                    if (alarm.Category != ALARMINFO.CATEGORY.SAFE)
                    {
                        alarm.ErrorLevel = ALARMINFO.LEVEL.OP;
                        showDialog = true;
                        Enque(alarm);
                        return;
                    }
                    alarm.WriteLog = false;
                }

                Enque(alarm);
            }
        }

        //public void Add(string message, bool write = true)
        //{
        //    lock (lockObj)
        //    {
        //        var alarm = new ALARMINFO(DateTime.Now, -1, ALARMINFO.TYPE.WARNNING, ALARMINFO.CATEGORY.NONE, message, write);
        //        Enque(alarm);
        //    }
        //}

        public bool Find(int code)
        {
            if (dic_alarm.ContainsKey(code))
                return true;
            return alarmDlg.Find(code);
        }

        public void Clear()
        {
            lock (lockObj)
            {
                dic_alarm.Clear();
                clearDialog = true;
            }
        }

        public void Remove(int alarmCode)
        {
            lock (lockObj)
            {
                alarmDlg.Remove(alarmCode);
            }
        }

        private void OnThreading()
        {
            try
            {
                var alaramlist = new ALARMINFO[MaxQueueSize];
                for (int i = 0; i < MaxQueueSize; i++)
                    alaramlist[i] = new ALARMINFO();

                while (true)
                {
                    syncContext.Send(
                    delegate
                    {
                        if (clearDialog)
                        {
                            if (alarmDlg.GetCount() > 0)
                            {
                                alarmDlg.Clear();
                                alarmDlg.Hide();
                            }
                            clearDialog = false;
                        }

                        if (!queue.IsEmpty)
                        {
                            lock (queueLock)
                            {
                                queue.CopyTo(alaramlist, 0);
                                for (int i = 0; i < queue.Count; i++)
                                    WriteLog(alaramlist[i]);
                                queue = null;
                                queue = new ConcurrentQueue<ALARMINFO>();
                            }

                        }
                        else if (dic_alarm.Count > 0 && showDialog)
                        {
                            ShowDialog();
                        }
                        else if (dic_alarm.Count == 0 && showDialog)
                        {
                            alarmDlg.Hide();
                        }
                    }, null);

                    Thread.Sleep(50);
                }
            }
            catch (Exception)
            {
            }
        }

        private void Enque(ALARMINFO alarm)
        {
            try
            {
                if (dic_alarm.ContainsKey(alarm.Code))
                {
                    if (alarm.Category != ALARMINFO.CATEGORY.SAFE)
                        return;
                }
                else
                {
                    dic_alarm.Clear();
                    dic_alarm.Add(alarm.Code, alarm);

                    syncContext.Send(
                    delegate
                    {
                        var prev_alaram = new ALARMINFO();
                        queue.Enqueue(alarm);
                        while (queue.Count > MaxQueueSize && queue.TryDequeue(out prev_alaram)) ;
                    }, null);
                }

                showDialog = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return;
            }
        }

        private void ShowDialog()
        {
            alarmDlg.Show();
            showDialog = false;
        }

        private void WriteLog(ALARMINFO alarm)
        {
            try
            {
                string message;
                message = $"[{alarm.Date.ToString("HH:mm:ss.fff")}][{Enum.GetName(typeof(ALARMINFO.CATEGORY), alarm.Category)}]";
                message += $"[{Enum.GetName(typeof(ALARMINFO.TYPE), alarm.Type)}] {alarm.Message}";

                FileVersionInfo fileVersion = FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly().Location);
                string logfilepath = filepath + @"\" + $"{alarm.Date.ToString("yyyy-MM-dd")}.txt";
                if (Directory.Exists(filepath) == false)
                    Directory.CreateDirectory(filepath);

                if (alarm.WriteLog)
                {
                    using (StreamWriter sw = new StreamWriter(logfilepath, true, Encoding.UTF8))
                    {
                        sw.WriteLine(message);
                        sw.Close();
                    }

                    Action action = delegate
                    {
                        if (AlarmLogEvent != null)
                            AlarmLogEvent(alarm.Code, message);
                    };
                    Task.Run(action);
                }
                if (alarm.Message != null)
                    LogUtil.Instance.Log(LOG_TYPE.ALARM, alarm.Message.ToString(), CONTENT_TYPE.INFO);
                alarmDlg.Clear();
                alarmDlg.Add(alarm);
                ShowDialog();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
                return;
            }
        }

        public void DeleteAsScheduled(string folderpath)
        {
            DirectoryInfo di = new DirectoryInfo(folderpath);

            if (di.Exists)
            {
                DeleteLogFile(folderpath);
                DirectoryInfo[] subDirs = di.GetDirectories();

                foreach (DirectoryInfo directoryInfo in subDirs)
                {
                    DeleteAsScheduled(folderpath + @"\" + directoryInfo.Name);
                }
            }
        }

        private void DeleteLogFile(string folderpath)
        {
            try
            {
                syncContext.Post(
                delegate
                {
                    DirectoryInfo directoryInfo = new DirectoryInfo(folderpath);
                    string removeFileDate = DateTime.Today.AddDays(-MaxStoreDays).ToString("yyyyMMdd");

                    if (directoryInfo.Exists)
                    {
                        foreach (var file in directoryInfo.GetFiles())
                        {
                            if (removeFileDate.CompareTo(file.LastWriteTime.ToString("yyyyMMdd")) > 0)
                            {
                                file.IsReadOnly = false;
                                file.Delete();
                            }
                        }
                    }
                }, null);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
                return;
            }
        }

        private void AlarmSoundOn()
        {
            if (AlarmSoundOnEvent != null)
                AlarmSoundOnEvent();
        }

        private void AlarmMute()
        {
            if (AlarmMuteEvent != null)
                AlarmMuteEvent();
        }

        public SubForm_Alarm2 GetAlarmDlg()
        {
            return alarmDlg;
        }
    }

    public class ALARMINFO
    {
        public enum TYPE
        {
            UNKNOWN = 0,
            WARNNING = 1,
            CRITICAL = 2,
        }

        public enum CATEGORY
        {
            NONE = -1,
            SAFE,
            SYSTEM,
            PROCESS,
            VISION,
            DATA,
            EXCEPTION,
        }
        public enum UNIT
        {
            UNKNOWN,
            TRAYIN,
            TRAYINBUF,
            TRAYWORK,
            TRAYOUTBUF,
            TRAYOUT,
            LD_PICKER,
            ULD_PICKER,
            LEFTBUF,
            RIGHTBUF,
            JIGINBUF,
            JIGWORK,
            JIGOUTBUF,
            JIGCARRIER,
            TAPE_ROLL1,
            TAPE_ROLL2,
            MAXUNIT,
        }

        public enum LEVEL
        {
            UNKNOWN,
            OP,
            EN
        }

        public DateTime Date { get; set; }
        public int Code { get; set; }
        public string Tag { get; set; }
        public TYPE Type { get; set; }
        public CATEGORY Category { get; set; } = CATEGORY.NONE;
        public string Message { get; set; }
        public int SoundTime { get; set; }
        public bool WriteLog { get; set; } = true;
        public UNIT Unit { get; set; } = UNIT.UNKNOWN;
        public LEVEL ErrorLevel { get; set; } = LEVEL.UNKNOWN;

        public ALARMINFO()
        {
        }

        public ALARMINFO(DateTime date, int code, TYPE type, CATEGORY category, string message, bool writelog, LEVEL errorlevel)
        {
            this.Date = date;
            this.Code = code;
            this.Type = type;
            this.Category = category;
            this.Message = message;
            this.WriteLog = writelog;
            this.ErrorLevel = errorlevel;
        }
    }

    public class AlarmList
    {
        private List<ALARMINFO> alarmlist = new List<ALARMINFO>();
        public Dictionary<int, ALARMINFO> alarm = new Dictionary<int, ALARMINFO>();

        public AlarmList()
        {
        }

        public ALARMINFO GetAlarm(int code)
        {
            if (!alarm.ContainsKey(code))
                return null;
            return alarm[code];
        }

        public bool Read(string filepath)
        {
            if (File.Exists(filepath))
            {
                string streamJson = File.ReadAllText(filepath);
                streamJson = Regex.Unescape(streamJson);
                alarmlist = JsonSerializer.Deserialize<List<ALARMINFO>>(streamJson);
            }
            else
            {
                MessageBox.Show($"Failed to read file a {filepath}");
                return false;
            }

            bool isMismatchFromEnum = false;
            for (int i = 0; i < alarmlist.Count; ++i)
            {
                if (((ECODE)alarmlist[i].Code).ToString() != alarmlist[i].Tag)
                {
                    isMismatchFromEnum = true;
                }
            }       

            alarm.Clear();

            try
            {
                for (int i = 0; i < alarmlist.Count; i++)
                    alarm.Add(alarmlist[i].Code, alarmlist[i]);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return false;
            }

            return true;
        }
    }
}
