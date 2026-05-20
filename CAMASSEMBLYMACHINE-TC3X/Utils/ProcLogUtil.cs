using CAMASSEMBLYMACHINE.Define;
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
    public enum PROCLOG_TYPE
    {
        PROCESS = 1,
        TACT = 2
    }

    public class ProcLogUtil
    {
        public static int MAX_QUEUE_SIZE = 500 * 1024; // 500k
        public const int DELETE_LOG_PERIOD = 10; //days

        private static ProcLogUtil instance;
        private SynchronizationContext syncContext;
        private ConcurrentQueue<ProcLogData> queue = new ConcurrentQueue<ProcLogData>();
        private Object queueLock = new object();
        private Object enqueLock = new object();        

        private ProcLogUtil()
        {
            syncContext = SynchronizationContext.Current;
        }

        ~ProcLogUtil()
        {
            ProcLogData data = new ProcLogData();
            while (!queue.IsEmpty)
            {
                queue.TryDequeue(out data);
                WriteLog(data);
            }
        }

        /**
         * @brief Thread 수행하면서 Log을 Queue에서 꺼내서 File write 하는 함수
         */
        public void ThreadRun()
        {
            try
            {
                //LogData[] data = new LogData[MAX_QUEUE_SIZE];
                //for (int i = 0; i < MAX_QUEUE_SIZE; i++)
                //    data[i] = new LogData();
                
                while (true)
                {
                    syncContext.Send(
                    delegate
                    {
                        if (!queue.IsEmpty)
                        {
                            lock (queueLock)
                            {
                                ProcLogData data;
                                queue.TryDequeue(out data);
                                WriteLog(data);
                            }
                            
                        }
                    }, null);
                    
                    Thread.Sleep(10);
                }
            }
            catch (Exception) { }
        }

        public static ProcLogUtil Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ProcLogUtil();
                }
                return instance;
            }
        }

        /**
         * @brief 기간이 지난 로그들을 삭제하는 함수
         * @param[in] logPath 로그 삭제 대상 경로
         */
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

        /**
         * @brief Log data를 메모리 상에 임시로 저장하기 위한 함수
         * @param[in] data LogData
         */
        private void Enque(ProcLogData data)
        {
            try
            {
                lock (enqueLock)
                {
                    syncContext.Send(
                    delegate
                    {
                        ProcLogUtil oldData = new ProcLogUtil(); ;
                        queue.Enqueue(data);
                        //while (queue.Count > MAX_QUEUE_SIZE && queue.TryDequeue(out oldData)) ;
                    }, null);
                }
            }
            catch (Exception ex)
            {
                LogUtil.Instance.Log(LOG_TYPE.DEVELOPMENT, ex.ToString(), CONTENT_TYPE.EXCEPTION);
                return;
            }
        }

        /**
         * @brief 기간보다 초과한 로그 파일을 삭제하는 함수
         * @param[in] fileDir 어떤 디렉토리를 기준으로 하위 파일들을 삭제할 것인지 정하는 인자 (string)
         */
        private void DeleteLogFile(string fileDir)
        {
            try
            {
                syncContext.Post(
                delegate
                {
                    DirectoryInfo directoryInfo = new DirectoryInfo(fileDir);
                    string removeFileDate = DateTime.Today.AddDays(-SystemDefine.deletelogperiod).ToString("yyyyMMdd");

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
                return;
            }
        }

        /**
         * @brief User가 Logging을 위해 호출 하는 함수
         * @param[in] logType LOG_TYPE 타입으로 로깅 파일 선택
         * @param[in] message 로깅 내용
         * @param[in] type 로깅 레벨
         * @param[in] file CallerFilePath로 아무것도 적어주지 않는 것이 좋다
         * @param[in] line CallerLineNumber로 아무것도 적어주지 않는 것이 좋다
         */
        public void Log(PROCLOG_TYPE logType, string message)
        {
            ProcLogData data = new ProcLogData(logType, DateTime.Now, message);
            Enque(data);
        }

        /**
         * @brief 로그를 파일 Write 하는 함수
         * @param[in] data LogData 타입
         */
        private void WriteLog(ProcLogData data)
        {
            try
            {
                foreach (PROCLOG_TYPE logType in Enum.GetValues(typeof(PROCLOG_TYPE)))
                {
                    if (data.tagFile.HasFlag(logType))
                    {
                        string filePath = SystemDefine.logPath;
                        string filName;

                        if (data.tagFile == PROCLOG_TYPE.PROCESS)
                        {
                            filePath = SystemDefine.logPath + $"\\{Enum.GetName(typeof(PROCLOG_TYPE), logType).ToString()}_{DateTime.Now.ToString("yyyy-MM-dd")}";
                            filName = filePath + @"\" + DateTime.Now.ToString("HH") + ".txt";
                        }
                        else
                        {
                            filName = filePath + @"\" + Enum.GetName(typeof(PROCLOG_TYPE), logType).ToString() + "_" + DateTime.Now.ToString("yyyy-MM-dd") + ".txt";
                        }

                        if (Directory.Exists(filePath) == false)
                            Directory.CreateDirectory(filePath);

                        using (StreamWriter sw = new StreamWriter(filName, true, Encoding.UTF8))
                        {
                            sw.WriteLine($"[{data.logTime.ToString("yyyy-MM-dd HH:mm:ss.fff")}] {data.msg}");
                            sw.Close();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                return;
            }
        }
    }

    /**
     * @brief LogData 클래스
     */
    public class ProcLogData
    {
        public DateTime logTime { get; set; }
        public PROCLOG_TYPE tagFile { get; set; }
        public string msg { get; set; }

        /**
         * @brief 생성자
         */
        public ProcLogData()
        {
        }

        /**
         * @brief 생성자
         * @param[in] tagFile 로깅 파일 이름 
         * @param[in] dateTime 로깅 시간
         * @param[in] msg 로깅 메시지
         * @param[in] type 로깅 레벨
         * @param[in] excepFile exception file
         * @param[in] line 소스코드의 라인 수
         */
        public ProcLogData(PROCLOG_TYPE tagFile, DateTime dateTime, string msg)
        {
            this.tagFile = tagFile;
            this.msg = msg;
            this.logTime = dateTime;
        }
    }
}
