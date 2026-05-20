using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;

namespace TopEng.Utils
{
    public class EDMUtil
    {
        public enum RUNMODE
        {
            AUTO,
            BYPASS,
            DRYRUN
        }

        public static int MAX_QUEUE_SIZE = 500;
        public const int DELETE_LOG_PERIOD = 1; //days

        public const string RESERVED_LOG_MACHINE_START = "9000";
        public const string RESERVED_LOG_MACHINE_STOP = "9001";
        public const string RESERVED_LOG_UNLOAD_GOOD = "9002";
        public const string RESERVED_LOG_UNLOAD_NG = "9003";
        public const string RESERVED_LOG_LOAD_NEW = "9004";
        public const string RESERVED_LOG_LAMP_STATE = "9009";
        public const string RESERVED_LOG_NGTRAY_COUNT = "9010";
        public const string RESERVED_LOG_PARTS_MONITORING = "9210";
        public const string RESERVED_LOG_SETS_MONITORING = "9200";
        public const string RESERVED_LOG_SETS_LOADING = "9004";
        public const string RESERVED_LOG_SETS_UNLOADING_GOOD = "9002";
        public const string RESERVED_LOG_SETS_UNLOADING_NG = "9003";

        public const string TOP_CODE = "TOPS81";

        public delegate void EDMLogDelegate(string msg);
        public static event EDMLogDelegate EDMLogEvent;

        private static EDMUtil instance;
        private SynchronizationContext syncContext;
        private ConcurrentQueue<EDMDATA> queue = new ConcurrentQueue<EDMDATA>();
        private Object queueLock = new object();

        private string filepath;

        private EDMUtil(string filepath)
        {
            syncContext = SynchronizationContext.Current;
            this.filepath = filepath;
        }

        ~EDMUtil()
        {
            EDMDATA data = new EDMDATA();
            while (!queue.IsEmpty)
            {
                queue.TryDequeue(out data);
                WriteLog(data);
            }
        }

        public static EDMUtil Instance(string filepath)
        {
            if (instance == null)
            {
                instance = new EDMUtil(filepath);
            }
            return instance;
        }

        public void Start(string idf, string version, string jig_stat, RUNMODE mode)
        {
            string runmode = ((int)mode).ToString();

            EDMDATA data = new EDMDATA(idf, DateTime.Now, RESERVED_LOG_MACHINE_START, jig_stat, version, "", runmode, "", "");
            Enque(data);
        }

        public void Stop(string idf, string version, string jig_stat)
        {
            EDMDATA data = new EDMDATA(idf, DateTime.Now, RESERVED_LOG_MACHINE_STOP, jig_stat, version, "", "", "", "");
            Enque(data);
        }

        public void TowerLamp(string idf, string version, string jig_stat, uint red, uint yellow, uint green)
        {
            string stat = green.ToString();
            stat += yellow.ToString();
            stat += red.ToString();

            EDMDATA data = new EDMDATA(idf, DateTime.Now, RESERVED_LOG_LAMP_STATE, jig_stat, version, "", stat, "", "");
            Enque(data);
        }

        public void PartsCount(string idf, string version, string jig_stat, uint inputCount, uint totalCount, uint alarmLimit, string id)
        {
            string extra_value = totalCount.ToString() + "-" + alarmLimit.ToString();
            string alarm_bit = totalCount - inputCount > alarmLimit ? "0" : "1";

            EDMDATA data = new EDMDATA(idf, DateTime.Now, RESERVED_LOG_PARTS_MONITORING, jig_stat, version, inputCount.ToString(), extra_value, alarm_bit, id);
            Enque(data);
        }

        public void SetsCount(string idf, string version, string jig_stat, uint setCount)
        {
            EDMDATA data = new EDMDATA(idf, DateTime.Now, RESERVED_LOG_SETS_MONITORING, jig_stat, version, "", setCount.ToString(), "", "");
            Enque(data);
        }

        public void LoadingSets(string idf, string version, string jig_stat, bool wait)
        {
            string loading = wait ? "1" : "0";
            EDMDATA data = new EDMDATA(idf, DateTime.Now, RESERVED_LOG_SETS_LOADING, jig_stat, version, loading, "", "", "");
            Enque(data);
        }

        public void UnloadingSetsGood(string idf, string version, string jig_stat, bool wait)
        {
            string unloading = wait ? "1" : "0";
            EDMDATA data = new EDMDATA(idf, DateTime.Now, RESERVED_LOG_SETS_UNLOADING_GOOD, jig_stat, version, unloading, "", "", "");
            Enque(data);
        }

        public void UnloadingSetsNG(string idf, string version, string jig_stat, string NGreason, bool wait)
        {
            string unloading = wait ? "1" : "0";
            EDMDATA data = new EDMDATA(idf, DateTime.Now, RESERVED_LOG_SETS_UNLOADING_NG, jig_stat, version, "", NGreason, TOP_CODE, "","");
            Enque(data);
        }

        public void SetNGBoxStatus(string idf, string version, string jig_stat, int currentCount, int maxCount, bool wait)
        {
            string extra_value = currentCount.ToString() + "-" + maxCount.ToString();
            string unloading = wait ? "1" : "0";
            EDMDATA data = new EDMDATA(idf, DateTime.Now, RESERVED_LOG_NGTRAY_COUNT, jig_stat, version, unloading, extra_value, "", "");
            Enque(data);
        }

        public void Alarm(string idf, string version, string jig_stat, uint code)
        {
            EDMDATA data = new EDMDATA(idf, DateTime.Now, code.ToString(), jig_stat, version, "", "", "", "");
            Enque(data);
        }

        public void ThreadRun()
        {
            try
            {
                EDMDATA[] data = new EDMDATA[MAX_QUEUE_SIZE];
                for (int i = 0; i < MAX_QUEUE_SIZE; i++)
                    data[i] = new EDMDATA();

                while (true)
                {
                    syncContext.Send(
                    delegate
                    {
                        if (!queue.IsEmpty)
                        {
                            lock (queueLock)
                            {
                                queue.CopyTo(data, 0);
                                for (int i = 0; i < queue.Count; i++)
                                    WriteLog(data[i]);
                                queue = null;
                                queue = new ConcurrentQueue<EDMDATA>();
                            }

                        }
                    }, null);

                    Thread.Sleep(50);
                }
            }
            catch (Exception) { }
        }

        private void Enque(EDMDATA data)
        {
            try
            {
                syncContext.Send(
                delegate
                {
                    EDMDATA oldData = new EDMDATA(); ;
                    queue.Enqueue(data);
                    while (queue.Count > MAX_QUEUE_SIZE && queue.TryDequeue(out oldData)) ;
                }, null);
            }
            catch (Exception e)
            {
                WriteException(e.Message);
            }
        }

        private void WriteLog(EDMDATA data)
        {
            try
            {
                string filename = $"{data.identifier},";
                filename += $"{data.logTime.ToString("yyyyMMddHHmmssfff")},";
                filename += $"{data.eventCode},";
                filename += $"{data.jigState},";
                filename += $"{data.version},";
                filename += $"{data.args[0]},";
                filename += $"{data.args[1]},";
                filename += $"{data.args[2]},";
                filename += $"{data.args[3]},";
                if (data.args.Count() == 5) filename += $"{data.args[4]},";
                string fullpath = filepath + "\\" + filename + ".txt";

                if (Directory.Exists(filepath) == false)
                    Directory.CreateDirectory(filepath);

                using (StreamWriter sw = new StreamWriter(fullpath, true, Encoding.UTF8))
                {
                    sw.WriteLine($"{filename}");
                    sw.Close();
                }

                Action action = delegate
                {
                    UpdateLogEvent(data.logTime, filename);
                };
                Task.Run(action);
            }
            catch (Exception e)
            {
                WriteException(e.Message);
            }
        }

        private void UpdateLogEvent(DateTime datetime, string message)
        {
            EDMLogEvent?.Invoke($"[{datetime.ToString("yyyy-MM-dd HH:mm:ss.fff")}] {message}");
            timeUtil.Delay(200);
        }

        public void WriteException(string message)
        {
            try
            {
                string filename = DateTime.Now.ToString("yyyyMMdd") + ".txt";
                string fullpath = filepath + "\\Exception\\" + filename;

                if (Directory.Exists(filepath) == false)
                    Directory.CreateDirectory(filepath);

                using (StreamWriter sw = new StreamWriter(fullpath, true, Encoding.UTF8))
                {
                    sw.WriteLine($"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}] {message}");
                    sw.Close();
                }
            }
            catch
            {
            }
        }

        public void DeleteOldFiles(string path)
        {
            DirectoryInfo di = new DirectoryInfo(path);

            if (di.Exists)
            {
                DeleteLogFile(path);
                DirectoryInfo[] subDirs = di.GetDirectories();

                foreach (DirectoryInfo directoryInfo in subDirs)
                {
                    DeleteOldFiles(path + @"\" + directoryInfo.Name);
                }
            }
        }
        private void DeleteLogFile(string fileDir)
        {
            try
            {
                syncContext.Post(
                delegate
                {
                    DirectoryInfo directoryInfo = new DirectoryInfo(fileDir);
                    string removeFileDate = DateTime.Today.AddDays(-DELETE_LOG_PERIOD).ToString("yyyyMMdd");

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
                WriteException(e.Message);
            }
        }
    }

    public class EDMDATA
    {
        public string identifier { get; set; }
        public DateTime logTime { get; set; }
        public string eventCode { get; set; }
        public string jigState { get; set; } = "00000000";
        public string version { get; set; }
        public string[] args = new string[4];

        public EDMDATA()
        {
        }

        public EDMDATA(string idf, DateTime time, string eventCode, string jig_state, string version, string f6, string f7, string f8, string f9, string f10 = null)
        {
            this.identifier = idf;
            this.logTime = time;
            this.eventCode = eventCode;
            this.jigState = jig_state;
            this.version = version;
            if (f10 != null)
            {
                string[] argument2 = { f6, f7, f8, f9, f10 };
                this.args = argument2;
            }
            else
            {
                string[] argument = { f6, f7, f8, f9 };
                this.args = argument;
            }
        }
    }
}
