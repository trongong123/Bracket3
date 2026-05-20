using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Threading;
using System.IO;
using Cognex.VisionPro;
using Cognex.VisionPro.ImageFile;
using TopEng.Utils;
namespace TopEng.Vision
{
    public class CaptureData
    {
        public DateTime datetime { get; set; }
        public string name { get; set; }
        public CaptureUtil.CAPTURETYPE type { get; set; }
        public ICogImage image { get; set; }
        public string filepath { get; set; }

        public CaptureData()
        {

        }

        public CaptureData(string name, CaptureUtil.CAPTURETYPE type, DateTime datetime, ICogImage image, string filepath)
        {
            this.type = type;
            this.name = name;
            this.datetime = datetime;
            this.image = image;
            this.filepath = filepath;
        }
    }

    public class CaptureUtil
    {
        public const int MAX_QUEUE_SIZE = 1000;
        public int deleteimageperiod = 1;

        public string filepath;
        private object LockControl = new object();
        private Thread m_thread;
        public bool autoDelete = true;
        public bool pauseSave = false;

        public enum CAPTURETYPE
        {
            NONE,
            RAW,
            RESULT,
            USER,
            SUCCESS,
            EMPTY,
            FAIL,
            RETRY_FAIL,
            WARNING,
            TRAIN,
            CLASSIFY_OK,
            CLASSIFY_NG,
            LOW_SCORE_OK,
            LOW_SCORE_NG,

            UNDER_LEFT,
            UNDER_RIGHT,
        }

        //public delegate void Capture

        private static CaptureUtil instance;
        private SynchronizationContext syncContext;
        private ConcurrentQueue<CaptureData> queue = new ConcurrentQueue<CaptureData>();
        private Object queueLock = new object();

        public CaptureUtil(string filepath, int delete_period_day)
        {
            this.filepath = filepath;
            this.deleteimageperiod = delete_period_day;
            syncContext = SynchronizationContext.Current;

            m_thread = new Thread(this.OnThread);
            m_thread.IsBackground = true;
            m_thread.Start();
        }

        public void Capture(string name, CAPTURETYPE type, ICogImage image, bool LowQuality = false, [CallerFilePath] string file = "")
        {
            lock (LockControl)
            { 
                if (image == null)
                    return;
                if (LowQuality)
                    image = image.ScaleImage(640, 480);
                CaptureData data = new CaptureData(name, type, DateTime.Now, image, file);
                Enque(data);

                if (autoDelete)
                    DeleteOldFiles(filepath);
            }
        }

        public void Clear()
        {
            CaptureData data = new CaptureData();

            while (!queue.IsEmpty)
            {
                queue.TryDequeue(out data);
            }
        }

        ~CaptureUtil()
        {
            CaptureData data = new CaptureData();

            while (!queue.IsEmpty)
            {
                queue.TryDequeue(out data);
                StoreImage(data);
                Thread.Sleep(20);
            }
        }

        private void Enque(CaptureData data)
        {
            try
            {
                syncContext.Send(
                delegate
                {
                    //CaptureData oldData = new CaptureData();
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
                //CaptureData[] data = new CaptureData[MAX_QUEUE_SIZE];
                //for (int i = 0; i < MAX_QUEUE_SIZE; i++)
                //    data[i] = new CaptureData();

                while (true)
                {
                    syncContext.Send(
                    delegate
                    {
                        if (!queue.IsEmpty && !pauseSave)
                        {
                            lock (queueLock)
                            {
                                CaptureData data;
                                queue.TryDequeue(out data);
                                //queue.CopyTo(data, 0);
                                //for (int i = 0; i < queue.Count; i++)
                                //{
                                StoreImage(data);
                                //    queue.TryDequeue(data[i]);
                                //}
                                //queue = null;
                                //queue = new ConcurrentQueue<CaptureData>();
                            }

                        }
                    }, null);

                    Thread.Sleep(50);
                }
            }
            catch (Exception) { }
        }

        public void DeleteOldFiles()
        {
            DeleteOldFiles(filepath);
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

        private void StoreImage(CaptureData data)
        {
            try
            {
                string folderPath = filepath + @"\";
                folderPath += DateTime.Now.ToString("yyyy-MM-dd") + @"\";
                folderPath += data.name + @"\";
                if (data.type != CAPTURETYPE.NONE)
                    folderPath += Enum.GetName(typeof(CAPTURETYPE), data.type);
                string filePath = folderPath + @"\" + DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss-ffff") + ".bmp";

                if (Directory.Exists(folderPath) == false)
                {
                    Directory.CreateDirectory(folderPath);
                }

                CogImageFileBMP bmp = new CogImageFileBMP();
                bmp.Open(filePath, CogImageFileModeConstants.Write);
                bmp.Append(data.image);
                bmp.Close();
                bmp.Dispose();

                if (autoDelete)
                    DeleteOldFiles(folderPath);
            }
            catch
            {
                return;
            }
        }

        public void SetDeleteImagePeriod(int period) => this.deleteimageperiod = period;

        private void DeleteImageFile(string folderPath)
        {
            try
            {
                DateTime now = DateTime.Now;

                syncContext.Post(
                delegate
                {
                    DirectoryInfo directoryInfo = new DirectoryInfo(folderPath);
                    if (IOFunctionUtil.CheckDirectoryExist(folderPath))
                    {
                        DateTime written = directoryInfo.LastWriteTime.ToLocalTime();
                        TimeSpan period = now - written;
                        if(period.Days > deleteimageperiod)
                        {
                            IOFunctionUtil.DeleteDirectory(folderPath);
                        }
                    }
                }, null);
            }
            catch (Exception ex)
            {
                return;
            }
        }
    }
}
