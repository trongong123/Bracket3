using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Threading;
using System.IO;
using System.Windows.Forms;

namespace TopEng.Utils
{
    public class RECDATA
    {
        public DateTime datetime { get; set; }
        public string desc { get; set; }

        public RECDATA()
        {

        }

        public RECDATA(DateTime datetime, string desc)
        {
            this.datetime = datetime;
            this.desc = desc;
        }
    }

    public class RecordUtil
    {
        public const int MAX_QUEUE_SIZE = 50;
        public int deleteimageperiod = 1;

        public string folderpath;
        private object LockControl = new object();
        private Thread m_thread;

        private SynchronizationContext syncContext;
        private ConcurrentQueue<RECDATA> queue = new ConcurrentQueue<RECDATA>();
        private Object queueLock = new object();

        private Dictionary<string, DateTime> records = new Dictionary<string, DateTime>();

        private bool firstWrite = true;

        public RecordUtil(string folderpath, int delete_period_day)
        {
            this.folderpath = folderpath;
            this.deleteimageperiod = delete_period_day;
            syncContext = SynchronizationContext.Current;

            records.Clear();
            bool duplicate = false;

            try
            {
                string filePath = folderpath + @"\" + DateTime.Now.ToString("yyyy-MM-dd") + ".csv";
                if (File.Exists(filePath))
                {
                    StreamReader stream = new StreamReader(filePath, false);

                    while (!stream.EndOfStream)
                    {
                        string strLine = stream.ReadLine();
                        string[] split = strLine.Split(',');

                        if (split[0] == "") continue;

                        if (!records.ContainsKey(split[0]))
                            records.Add(split[0], DateTime.Now);
                        else
                            duplicate = true;
                    }

                    if (duplicate)
                    {
                        WriteRecords();
                        //MessageBox.Show("동일한 키가 검색되었습니다. 백업파일이 저장됩니다");
                    }
                }
            }
            catch (Exception e)
            {
            }

            m_thread = new Thread(this.OnThread);
            m_thread.IsBackground = true;
            m_thread.Start();
        }

        public bool Record(string desc)
        {
            lock (LockControl)
            {
                if (records.ContainsKey(desc))
                    return false;

                RECDATA data = new RECDATA(DateTime.Now, desc);

                Enque(data);
                DeleteOldFiles(folderpath);

                List<string> RemoveKeys = new List<string>();

                foreach (var rec in records)
                {
                    TimeSpan timeDiff = DateTime.Now - rec.Value;
                    if (timeDiff.Minutes > 30)
                        RemoveKeys.Add(rec.Key);
                }

                foreach (var key in RemoveKeys)
                    records.Remove(key);

                return true;
            }
        }

        public void WriteRecords()
        {
            foreach (var rec in records)
            {
                RECDATA data = new RECDATA(DateTime.Now, rec.Key);
                WriteLog(data, true);
            }
        }

        ~RecordUtil()
        {
            RECDATA data = new RECDATA();

            while (!queue.IsEmpty)
            {
                queue.TryDequeue(out data);
                WriteLog(data);
            }
        }

        private void Enque(RECDATA data)
        {
            try
            {
                syncContext.Send(
                delegate
                {
                    RECDATA oldData = new RECDATA(); ;
                    queue.Enqueue(data);
                    //while (queue.Count > MAX_QUEUE_SIZE && queue.TryDequeue(out oldData)) ;
                }, null);
            }
            catch
            {
                return;
            }
        }

        public void OnThread()
        {
            try
            {
                //RECDATA[] data = new RECDATA[MAX_QUEUE_SIZE];
                //for (int i = 0; i < MAX_QUEUE_SIZE; i++)
                //    data[i] = new RECDATA();

                while (true)
                {
                    syncContext.Send(
                    delegate
                    {
                        if (!queue.IsEmpty)
                        {
                            lock (queueLock)
                            {
                                RECDATA data;
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

        public void DeleteOldFiles(string path)
        {
            DirectoryInfo di = new DirectoryInfo(path);

            if (di.Exists)
            {
                DeleteImageFile(path);
                DirectoryInfo[] subDirs = di.GetDirectories();

                foreach (DirectoryInfo directoryInfo in subDirs)
                {
                    DeleteOldFiles(path + @"\" + directoryInfo.Name);
                }
            }
        }

        private void WriteLog(RECDATA data, bool backup = false)
        {
            try
            {
                string folderPath = folderpath + @"\";
                string filePath = folderPath + @"\" + DateTime.Now.ToString("yyyy-MM-dd");
                
                if (backup) 
                    filePath += ".bak";
                else
                    filePath += ".csv";

                if (Directory.Exists(folderPath) == false)
                {
                    Directory.CreateDirectory(folderPath);
                }

                bool append = backup && firstWrite ? false : true;
                using (StreamWriter sw = new StreamWriter(filePath, append, Encoding.UTF8))
                {
                    sw.WriteLine($"{data.desc}");
                    sw.Close();
                }

                firstWrite = false;

                DeleteOldFiles(folderPath);
            }
            catch (Exception e)
            {
                return;
            }
        }

        private void DeleteImageFile(string folderPath)
        {
            try
            {
                DateTime now = DateTime.Now;

                syncContext.Post(
                delegate
                {
                    DirectoryInfo directoryInfo = new DirectoryInfo(folderPath);

                    if (directoryInfo.Exists)
                    {
                        foreach (var file in directoryInfo.GetFiles())
                        {
                            DateTime writtern = file.LastWriteTime.ToLocalTime();

                            TimeSpan period = now - writtern;

                            if (period.Days > deleteimageperiod)
                            {
                                file.IsReadOnly = false;
                                file.Delete();
                            }
                        }

                        if (directoryInfo.GetDirectories().Length == 0 && directoryInfo.GetFiles().Length == 0)
                            directoryInfo.Delete();
                    }
                }, null);
            }
            catch
            {
                return;
            }
        }
    }
}
