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
    public enum LOG_TYPE
    {
        DEVELOPMENT = 1,
        ALARM = 2,
        INSPECTION = 4,
        EDM = 8,
        PROFILE = 16,
        SYSTEM = 32,
        DATA = 64,
        PROCESS = 128,
        UI = 256,
        BARCODE = 512
    }

    public enum CONTENT_TYPE
    {
        NONE = -1,
        DEBUG,
        INFO,
        WARNING,
        ERROR,
        EXCEPTION,
        EDM,
    }

    public class LogUtil
    {
        public static int MAX_QUEUE_SIZE = 500 * 1024; // 500k
        public const int DELETE_LOG_PERIOD = 7; //days

        public delegate void EDMLogDelegate(string msg);
        public static event EDMLogDelegate EDMLogEvent;
        public delegate void InspectionLogDelegate(string msg);
        public static event InspectionLogDelegate InspectionLogEvent;
        public delegate void SystemLogDelegate(string msg);
        public static event SystemLogDelegate SystemLogEvent;
        public delegate void DataLogDelegate(string msg);
        public static event DataLogDelegate DataLogEvent;
        public delegate void ProcessLogDelegate(string msg);
        public static event ProcessLogDelegate ProcessLogEvent;
        public delegate void UILogDelegate(string msg);
        public static event UILogDelegate UILogEvent;

        public delegate void AlarmLogDelegate(string msg);
        public delegate void DevelLogDelegate(string msg);
        private static LogUtil instance;
        private SynchronizationContext syncContext;
        private ConcurrentQueue<LogData> queue = new ConcurrentQueue<LogData>();
        private Object queueLock = new object();
        private Object enqueLock = new object();

        public static event AlarmLogDelegate AlarmLogEvent;
        public static event DevelLogDelegate DevelLogEvent;

        public delegate void BarcodeLogDelegate(string msg);
        public static event BarcodeLogDelegate BarcodeLogEvent;

        private LogUtil()
        {
            syncContext = SynchronizationContext.Current;
        }

        ~LogUtil()
        {
            LogData data = new LogData();
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
                                LogData data;
                                queue.TryDequeue(out data);
                                WriteLog(data);
                                DeleteOldFiles(SystemDefine.logPath);
                            }
                        }
                    }, null);

                    Thread.Sleep(30);
                }
            }
            catch (Exception) { }
        }

        public static LogUtil Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new LogUtil();
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
        private void Enque(LogData data)
        {
            try
            {
                //lock (enqueLock)
                //{
                    syncContext.Send(
                    delegate
                    {
                        LogData oldData = new LogData(); ;
                        queue.Enqueue(data);
                        //while (queue.Count > MAX_QUEUE_SIZE && queue.TryDequeue(out oldData)) ;
                    }, null);
                //}
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
                        foreach (var folder in directoryInfo.GetDirectories())
                        {
                            if (removeFileDate.CompareTo(folder.LastWriteTime.ToString("yyyyMMdd")) > 0)
                            {
                                IOFunctionUtil.DeleteDirectory(folder.FullName);
                            }
                        }
                        foreach (var file in directoryInfo.GetFiles())
                        {
                            if (removeFileDate.CompareTo(file.LastWriteTime.ToString("yyyyMMdd")) > 0)
                            {
                                IOFunctionUtil.DeleteFile(file.FullName);
                            }
                        }
                    }
                }, null);
            }
            catch (Exception e)
            {
                LogUtil.Instance.Log(LOG_TYPE.DEVELOPMENT, e.ToString(), CONTENT_TYPE.EXCEPTION);
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
        public void Log(LOG_TYPE logType, string message, CONTENT_TYPE type, string code = "", [CallerLineNumber] int line = 0)
        {
            LogData data = new LogData(logType, DateTime.Now, message, type, code, line);
            Enque(data);
        }

        /**
         * @brief 로그를 파일 Write 하는 함수
         * @param[in] data LogData 타입
         */
        private void WriteLog(LogData data)
        {
            try
            {
                foreach (LOG_TYPE logType in Enum.GetValues(typeof(LOG_TYPE)))
                {
                    if (data.tagFile.HasFlag(logType))
                    {
                        string filePath = SystemDefine.logPath;
                        string filName;

                        if (data.tagFile == LOG_TYPE.PROCESS)
                        {
                            filePath = SystemDefine.logPath + $"\\{Enum.GetName(typeof(LOG_TYPE), logType).ToString()}_{DateTime.Now.ToString("yyyy-MM-dd")}";
                            filName = filePath + @"\" + DateTime.Now.ToString("HH") + ".txt";
                        }
                        else
                        {
                            filName = filePath + @"\" + Enum.GetName(typeof(LOG_TYPE), logType).ToString() + "_" + DateTime.Now.ToString("yyyy-MM-dd") + ".txt";
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

                Action action = delegate
                {
                    UpdateLogEvent(data);
                };
                Task.Run(action);
            }
            catch (Exception e)
            {
                LogUtil.Instance.Log(LOG_TYPE.DEVELOPMENT, e.ToString(), CONTENT_TYPE.EXCEPTION);
                return;
            }
        }
        private void UpdateLogEvent(LogData data)
        {
            if (data.tagFile.HasFlag(LOG_TYPE.DATA))
            {
                DataLogEvent?.Invoke($"[{data.logTime.ToString("yyyy-MM-dd HH:mm:ss.fff")}] {data.msg}");
            }
            if (data.tagFile.HasFlag(LOG_TYPE.ALARM))
            {
                AlarmLogEvent?.Invoke($"[{data.logTime.ToString("yyyy-MM-dd HH:mm:ss.fff")}] {data.msg}");
            }
            if (data.tagFile.HasFlag(LOG_TYPE.BARCODE))
            {
                BarcodeLogEvent?.Invoke($"[{data.logTime.ToString("yyyy-MM-dd HH:mm:ss.fff")}] {data.msg}");
            }
            Util.Delay(200);
        }

        /**
         * @brief 파일과 라인을 출력하는 함수
         * @param[in] file 소스코드 파일 이름
         * @param[in] line 소스코드 라인 
         * @return 만들어진 file:line 스트링 셋
         */
        private static string BuildTag(string file, int line)
        {
            return string.Intern($"{Path.GetFileName(file)}:{line.ToString()}");
        }
    }

    /**
     * @brief LogData 클래스
     */
    public class LogData
    {
        public DateTime logTime { get; set; }
        public string eventCode { get; set; }
        public LOG_TYPE tagFile { get; set; }
        public string msg { get; set; }
        public CONTENT_TYPE type { get; set; }
        public int line { get; set; }

        /**
         * @brief 생성자
         */
        public LogData()
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
        public LogData(LOG_TYPE tagFile, DateTime dateTime, string msg, CONTENT_TYPE type, string Code = "", int line = 0)
        {
            this.tagFile = tagFile;
            this.type = type;
            this.msg = msg;
            this.logTime = dateTime;
            this.eventCode = Code;
            this.line = line;
        }
    }
}
