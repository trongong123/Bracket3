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
    public class AGVUtil
    {
        public static int MAX_QUEUE_SIZE = 500;
        public const int DELETE_LOG_PERIOD = 1; //days

        public delegate void AGVLogDelegate(string msg);
        public static event AGVLogDelegate AGVLogEvent;

        private static AGVUtil instance;
        private SynchronizationContext syncContext;
        private ConcurrentQueue<AGVDATA> queue = new ConcurrentQueue<AGVDATA>();
        private Object queueLock = new object();

        private string filepath;

        private AGVUtil(string filepath)
        {
            syncContext = SynchronizationContext.Current;
            this.filepath = filepath;
        }

        ~AGVUtil()
        {
            AGVDATA data = new AGVDATA();
            while (!queue.IsEmpty)
            {
                queue.TryDequeue(out data);
                WriteLog(data);
            }
        }

        public static AGVUtil Instance(string filepath)
        {
            if (instance == null)
            {
                instance = new AGVUtil(filepath);
            }
            return instance;
        }

        public void LoadingExist(bool in_buf)
        {
            string message = "";
            if (in_buf) message = "1";

            AGVDATA data = new AGVDATA(DateTime.Now, message);
            Enque(data);
        }

        public void Log(string message)
        {
            AGVDATA data = new AGVDATA(DateTime.Now, message);
            Enque(data);
        }

        public void ThreadRun()
        {
            try
            {
                //AGVDATA[] data = new AGVDATA[MAX_QUEUE_SIZE];
                //for (int i = 0; i < MAX_QUEUE_SIZE; i++)
                //    data[i] = new AGVDATA();

                while (true)
                {
                    syncContext.Send(
                    delegate
                    {
                        if (!queue.IsEmpty)
                        {
                            lock (queueLock)
                            {
                                AGVDATA data;
                                queue.TryDequeue(out data);
                                WriteLog(data);
                            }
                        }
                    }, null);

                    Thread.Sleep(50);
                }
            }
            catch (Exception) { }
        }

        private void Enque(AGVDATA data)
        {
            try
            {
                syncContext.Send(
                delegate
                {
                    //AGVDATA oldData = new AGVDATA(); ;
                    queue.Enqueue(data);
                    //while (queue.Count > MAX_QUEUE_SIZE && queue.TryDequeue(out oldData)) ;
                }, null);
            }
            catch (Exception e)
            {
                WriteException(e.Message);
            }
        }

        private void WriteLog(AGVDATA data)
        {
            try
            {
                string filename = DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt";
                string fullpath = filepath + "\\" + filename;

                if (Directory.Exists(filepath) == false)
                    Directory.CreateDirectory(filepath);

                using (StreamWriter sw = new StreamWriter(fullpath, true, Encoding.UTF8))
                {
                    sw.WriteLine($"{data.message}");
                    sw.Close();
                }



                Action action = delegate
                {
                    UpdateLogEvent(data);
                };
                Task.Run(action);
            }
            catch (Exception e)
            {
                WriteException(e.Message);
            }
        }

        private void UpdateLogEvent(AGVDATA data)
        {
            AGVLogEvent?.Invoke($"[{data.logTime.ToString("yyyy-MM-dd HH:mm:ss.fff")}] {data.message}");
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

    public class AGVDATA
    {
        public DateTime logTime { get; set; }
        public string message { get; set; }

        public AGVDATA()
        {
        }

        public AGVDATA(DateTime time, string message)
        {
            this.message = message;
            this.logTime = time;
        }
    }
}
