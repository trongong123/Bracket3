using CAMASSEMBLYMACHINE.Define;
using CAMASSEMBLYMACHINE.UI;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using TopEng.Utils;

namespace CAMASSEMBLYMACHINE.Process
{
    public class ProcessExternalEquip : IProcess
    {
        public enum STEP
        {
            ERROR = -1,
            IDLE,
            STOP,

            SEND_CHECK_PO_MESSAGE,
            CHECK_PO_RESULT,

        }

        public enum MSG
        {
            MSG_CHECK_PO,
        }

        public enum AUTOSTEP
        {
            ERROR, 
            IDLE,
            STOP,
            INIT,
            INIT_CHECK
        }

        #region PROCESS_STEP_VARIABLES
        public List<STEP> StepList = new List<STEP>();
        public STEP Step = STEP.IDLE;
        public int StepIndex = 0;
        public AUTOSTEP AutoStep = AUTOSTEP.IDLE;
        public override Type StepEnumType => typeof(STEP);
        public override Type MsgEnumType => typeof(MSG);
        #endregion

        private int checkPORetryCount;
        private Ethernet externalEquipEthernet;
        private object dataSendLockObject;
        private object dataReceiveLockObject;
        private bool isConnected;
        private ConcurrentQueue<string> receiveDataQueue;
        private EquipMessage_SetPO setPOReceiver;
        private EquipMessage_CheckPO checkPOReceiver;
        public ProcessExternalEquip()
        {
            externalEquipEthernet = new Ethernet(EthernetMode.eClientMode, SystemDefine.externalEquipServerIP, SystemDefine.externalEquipServerPort);
            dataSendLockObject = new object();
            dataReceiveLockObject = new object();
            receiveDataQueue = new ConcurrentQueue<string>();
            setPOReceiver = new EquipMessage_SetPO();
            checkPOReceiver = new EquipMessage_CheckPO();
            Connect();
            InitCallbackEvent();
        }

        #region CONNECTION_RELATED_FUNCTION
        private void InitCallbackEvent()
        {
            externalEquipEthernet.ConnectCallbackEventHandler += ConnectCallback;
            externalEquipEthernet.ReceiveDataCallbackEventHandler += ReceivedDataCallback;
            externalEquipEthernet.DisconnectedCallbackEventHandler += DisconnectCallback;
        }

        public void Connect()
        {
            if (SystemDefine.IS_NOTEBOOK_MODE) return;
            this.externalEquipEthernet.Connect();
        }
        public void Disconnect()
        {
            if (SystemDefine.IS_NOTEBOOK_MODE) return;
            this.externalEquipEthernet.DisConnect();
            isConnected = false;
        }
        private void ConnectCallback(EthernetResult result, string remoteEndPoint)
        {
            if (remoteEndPoint == null) return;
            if (remoteEndPoint.Contains(SystemDefine.externalEquipServerIP))
            {
                if (result == EthernetResult.SUCCESS)
                {
                    isConnected = true;
                    var IPConnection = IPGlobalProperties.GetIPGlobalProperties().GetActiveTcpConnections().Where
                        (c => c.RemoteEndPoint.Address.ToString() == SystemDefine.externalEquipServerIP).ToList();
                    LogUtil.Instance.Log(LOG_TYPE.DEVELOPMENT, $"Number of client connect to Rear EQP Server: {IPConnection.Count()}", CONTENT_TYPE.INFO);
                }
                else
                {
                    isConnected = false;
                }
            }
        }

        private void DisconnectCallback(string remoteEndPoint)
        {
            if (remoteEndPoint == null || remoteEndPoint.Contains(SystemDefine.externalEquipServerIP))
            {
                isConnected = false;
            }
        }

        private void AutoRecoveryConnection()
        {
            try
            {
                if (!isConnected)
                {
                    Disconnect();
                    Util.Delay(1000);
                    Connect();
                    Util.Delay(1500);
                    if (!isConnected) Disconnect();
                    Util.Delay(1500);
                }
            }
            catch (Exception e)
            {
                LogUtil.Instance.Log(LOG_TYPE.DEVELOPMENT, e.Message, CONTENT_TYPE.EXCEPTION);
            }
            GC.Collect();
        }
        #endregion

        #region DATA_PARSING
        public class EquipDataParse_Base
        {
            public string strEquipName;
            public EXT_EQUIP_ACTION rearEquipAction;
            public MESSAGE_TYPE messageType;

            public bool ParsingValueBase(string[] sValue)
            {
                try
                {
                    this.strEquipName = sValue[EXT_EQUIP_STRUCTURE.DATA_MACHINE_NAME];
                    Enum.TryParse(sValue[EXT_EQUIP_STRUCTURE.DATA_ACTION], out EXT_EQUIP_ACTION rearEquipAction);
                    this.rearEquipAction = rearEquipAction;
                    Enum.TryParse(sValue[EXT_EQUIP_STRUCTURE.DATA_MESSAGE_TYPE], out MESSAGE_TYPE inputMessageType);
                    this.messageType = inputMessageType;
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }

        public class EquipMessage_SetPO : EquipDataParse_Base
        {
            public static int DATA_PO_TYPE = 0;
            public static int DATA_NUMBER = 1;
            public static int DATA_DATA = 2;
            public string poType;
            public int dataNumber;
            public string data;
            public EquipMessage_SetPO() { }
            public bool ParsingValue(string[] value)
            {
                try
                {
                    base.ParsingValueBase(value);

                    string[] sSplitBodyData = value[EXT_EQUIP_STRUCTURE.DATA_MESSAGE_CONTENT].Split(new string[] { "}{" }, StringSplitOptions.None);
                    for (int i = 0; i < sSplitBodyData.Length; i++)
                    {
                        sSplitBodyData[i] = sSplitBodyData[i].Trim(new char[] { '{', '}' });
                    }

                    poType = sSplitBodyData[DATA_PO_TYPE].Replace($"{PO_STRUCTURE.PO_TYPE}:", string.Empty);
                    dataNumber = int.Parse(sSplitBodyData[DATA_NUMBER].Replace($"{PO_STRUCTURE.DATA_NUMBER}:", string.Empty));
                    data = sSplitBodyData[DATA_DATA].Replace($"{PO_STRUCTURE.DATA}:", string.Empty);
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
        }

        public class EquipMessage_CheckPO : EquipDataParse_Base
        {
            public static int DATA_PO_RESULT = 0;
            public PO_RESULT result;
            public EquipMessage_CheckPO() { }
            public bool ParsingValue(string[] value)
            {
                try
                {
                    base.ParsingValueBase(value);

                    string[] sSplitBodyData = value[EXT_EQUIP_STRUCTURE.DATA_MESSAGE_CONTENT].Split(new string[] { "}{" }, StringSplitOptions.None);
                    for (int i = 0; i < sSplitBodyData.Length; i++)
                    {
                        sSplitBodyData[i] = sSplitBodyData[i].Trim(new char[] { '{', '}' });
                    }

                    string returnData = sSplitBodyData[DATA_PO_RESULT].Replace($"{CHECK_PO_RESPONSE_STRUCTURE.RESULT}:", string.Empty);
                    Enum.TryParse(returnData, out PO_RESULT inputResult);
                    result = inputResult;
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
        }
        #endregion

        #region DATA_SEND/RECEIVE
        private void ReceivedDataCallback(string readData, string remoteEndPoint)
        {
            lock (dataReceiveLockObject)
            {
                try
                {
                    receiveDataQueue.Enqueue(readData);
                    Task.Run(ParseDataInQueue);
                }
                catch (Exception e)
                {
                    LogUtil.Instance.Log(LOG_TYPE.DEVELOPMENT, $"{e}", CONTENT_TYPE.EXCEPTION);
                }
            }
        }

        private void ParseDataInQueue()
        {
            while (receiveDataQueue.TryDequeue(out string strContent))
            {
                strContent = strContent.Replace(" ", string.Empty);
                string[] splitReadData = strContent.Split(new string[] { "><" }, StringSplitOptions.None);
                for (int i = 0; i < splitReadData.Length; i++)
                {
                    splitReadData[i] = splitReadData[i].Trim(new char[] { '<', '>' });
                }
                if (splitReadData.Length != EXT_EQUIP_STRUCTURE.DATA_MAX)
                {
                    throw new Exception("Received Data Format Error!");
                }

                Enum.TryParse(splitReadData[EXT_EQUIP_STRUCTURE.DATA_ACTION], out EXT_EQUIP_ACTION equipAction);
                Enum.TryParse(splitReadData[EXT_EQUIP_STRUCTURE.DATA_MESSAGE_TYPE], out MESSAGE_TYPE messageType);

                switch (equipAction)
                {
                    case EXT_EQUIP_ACTION.SET_PO:
                        if (messageType == MESSAGE_TYPE.REQUEST)
                        {
                            setPOReceiver.ParsingValue(splitReadData);
                            Vision.inspection.recipeInfo.PO_TYPE = setPOReceiver.poType;
                            Vision.inspection.recipeInfo.PO_DATA_NUMBER = setPOReceiver.dataNumber;
                            Vision.inspection.recipeInfo.PO_DATA = setPOReceiver.data;
                            Vision.inspection.WriteRecipeInfo();
                        }
                        break;
                    case EXT_EQUIP_ACTION.CHECK_PO:
                        if (messageType == MESSAGE_TYPE.RESPONSE)
                        {
                            checkPOReceiver.ParsingValue(splitReadData);
                        }
                        break;
                }
                Util.Delay(100);
            }
        }
        public bool SendMessage(params object[] contents)
        {
            lock (dataSendLockObject)
            {
                string sendPacket;
                try
                {
                    sendPacket = SendMessageParsing(contents);
                    externalEquipEthernet.SendData((string)sendPacket);
                    return true;
                }
                catch (Exception ex)
                {
                    LogUtil.Instance.Log(LOG_TYPE.DEVELOPMENT, ex.ToString(), CONTENT_TYPE.EXCEPTION);
                    //error
                    return false;
                }
            }
        }

        private string SendMessageParsing(params object[] contents)
        {
            string[] strPacketField = new string[6];
            strPacketField[EXT_EQUIP_STRUCTURE.DATA_MACHINE_NAME] = MACHINE_NAME.WC.ToString();
            strPacketField[EXT_EQUIP_STRUCTURE.DATA_ACTION] = ((EXT_EQUIP_ACTION)contents[0]).ToString();
            strPacketField[EXT_EQUIP_STRUCTURE.DATA_MESSAGE_TYPE] = ((MESSAGE_TYPE)contents[1]).ToString();
            strPacketField[EXT_EQUIP_STRUCTURE.DATA_MESSAGE_CONTENT] = string.Empty;
            switch ((EXT_EQUIP_ACTION)contents[0])
            {
                case EXT_EQUIP_ACTION.SET_PO:
                    if ((MESSAGE_TYPE)contents[1] == MESSAGE_TYPE.RESPONSE)
                    {
                        strPacketField[EXT_EQUIP_STRUCTURE.DATA_MESSAGE_CONTENT] += "{" + $"{PO_STRUCTURE.PO_TYPE}:{contents[2]}" + "}";
                    }
                    break;
                case EXT_EQUIP_ACTION.CHECK_PO:
                    if ((MESSAGE_TYPE)contents[1] == MESSAGE_TYPE.REQUEST)
                    {
                        strPacketField[EXT_EQUIP_STRUCTURE.DATA_MESSAGE_CONTENT] += "{" + $"{PO_STRUCTURE.PO_TYPE}:{contents[2]}" + "}";
                        strPacketField[EXT_EQUIP_STRUCTURE.DATA_MESSAGE_CONTENT] += "{" + $"{PO_STRUCTURE.DATA_NUMBER}:{contents[3]}" + "}";
                        strPacketField[EXT_EQUIP_STRUCTURE.DATA_MESSAGE_CONTENT] += "{" + $"{PO_STRUCTURE.DATA}:{contents[4]}" + "}";
                    }
                    break;
            }
            return $"<{strPacketField[EXT_EQUIP_STRUCTURE.DATA_MACHINE_NAME]}>" +
                $"<{strPacketField[EXT_EQUIP_STRUCTURE.DATA_ACTION]}>" +
                $"<{strPacketField[EXT_EQUIP_STRUCTURE.DATA_MESSAGE_TYPE]}>" +
                $"<{strPacketField[EXT_EQUIP_STRUCTURE.DATA_MESSAGE_CONTENT]}>";
        }
        #endregion

        #region PROCESS_STEP_RELATED_FUNCTION
        private void SetError(ECODE error)
        {
            if (error_proc)
                return;
            error_proc = true;

            AutoStep = AUTOSTEP.STOP;
            Machine.Alarm(error);
        }

        public bool CheckStopBit()
        {
            if (stopBit)
            {
                Step = STEP.IDLE;
                AutoStep = AUTOSTEP.IDLE;
                return true;
            }
            return false;
        }

        public override int GetStep() { return (int)Step; }
        public override int GetAutoStep() { return 0; }


        public override void TimeOut() { }

        public override void SetMessage(int message, int step = 0)
        {
            if (Busy() || Error())
            {
                SubForm_Warning dlg = new SubForm_Warning("The equipment is running or in an error state.");
                dlg.ShowDialog();
                return;
            }

            switch ((MSG)message)
            {
                case MSG.MSG_CHECK_PO:
                    checkPORetryCount = 0;
                    var checkPO = new List<STEP>()
                    {
                        STEP.SEND_CHECK_PO_MESSAGE,
                        STEP.CHECK_PO_RESULT,
                        STEP.IDLE
                    };
                    StepList = checkPO;
                    Step = StepList[StepIndex = 0];
                    break;
            }

        }

        public override void SetHeadTarget(int iTarget) { }

        public override void AutoStart()
        {
            stopBit = false;
            AutoStep = AUTOSTEP.INIT;
        }

        public override void EStop(bool pause)
        {
            stopBit = true;

            for (int i = 0; i < StepList.Count; i++)
                StepList[i] = STEP.IDLE;
            AutoStep = AUTOSTEP.IDLE;
            Step = STEP.IDLE;
        }

        public override void Stop()
        {
            stopBit = true;
        }


        public override bool Busy()
        {
            return Step != STEP.IDLE && Step != STEP.ERROR;
        }

        public override bool Ready()
        {
            return Step == STEP.IDLE;
        }

        public override bool Stopped()
        {
            return (Step == STEP.STOP);
        }

        public override bool Error()
        {
            return Step == STEP.ERROR;
        }

        public STEP NextStep()
        {
            if (++StepIndex < StepList.Count())
                Step = StepList[StepIndex];
            else
                Step = STEP.IDLE;
            return Step;
        }

        private int GetStepIndex(List<STEP> stepList, STEP eStep)
        {
            for (int i = 0; i < stepList.Count; i++)
            {
                if (stepList[i].ToString() == eStep.ToString())
                {
                    return i;
                }
            }
            return -1;
        }
        #endregion

        protected override void OnProcessing()
        {
            //if (!isConnected) AutoRecoveryConnection();
            OnProcessOfAutoRun();
            OnProcessOfComm();
            Util.Delay(100);
        }

        private void OnProcessOfAutoRun()
        {
            switch ((AUTOSTEP)AutoStep)
            {
                case AUTOSTEP.ERROR:
                    break;

                case AUTOSTEP.IDLE:
                    stopBit = false;
                    break;

                case AUTOSTEP.STOP:
                    break;

                case AUTOSTEP.INIT:
                    initCompl = false;
                    stopBit = false;

                    //SetMessage((int)MSG.MSG_CHECK_PO);
                    AutoStep = AUTOSTEP.INIT_CHECK;
                    break;

                case AUTOSTEP.INIT_CHECK:
                    if (CheckStopBit() || !Ready() || Error())
                        break;

                    initCompl = true;
                    AutoStep = AUTOSTEP.IDLE;
                    break;
            }

        }

        private void OnProcessOfComm()
        {
            switch ((STEP)Step)
            {
                case STEP.ERROR:
                    error_proc = false;
                    AutoStep = AUTOSTEP.ERROR;
                    timeWait[(int)TIMER.TIMEOUT].Reset();
                    break;

                case STEP.IDLE:
                    error_proc = false;
                    stopBit = false;
                    timeWait[(int)TIMER.TIMEOUT].Reset();
                    break;

                case STEP.STOP:
                    timeWait[(int)TIMER.TIMEOUT].Reset();
                    break;

                case STEP.SEND_CHECK_PO_MESSAGE:

                    checkPOReceiver.result = PO_RESULT.NONE;
                    bool isCompleted = SendMessage(EXT_EQUIP_ACTION.CHECK_PO, MESSAGE_TYPE.REQUEST,
                        Vision.inspection.recipeInfo.PO_TYPE,
                        Vision.inspection.recipeInfo.PO_DATA_NUMBER,
                        Vision.inspection.recipeInfo.PO_DATA);

                    if (!isCompleted)
                        SetError(ECODE.ETHERNET_SEND_MESSAGE_TO_SERVER_FAIL);
                    else
                    {
                        NextStep();
                        timeWait[(int)TIMER.TIMEOUT].Start();
                    }
                    break;

                case STEP.CHECK_PO_RESULT:
                    if (CheckStopBit())
                        break;
                    /* bool isTimeout = timeWait[(int)TIMER.TIMEOUT].Elapsed > 5000;

                     if (checkPOReceiver.result == PO_RESULT.TRUE)
                         NextStep();
                     else if (checkPOReceiver.result == PO_RESULT.FALSE || isTimeout)
                     {
                         if (checkPORetryCount >= 3) SetError(ECODE.ETHERNET_CHECK_PO_FAIL);
                         else
                         {
                             checkPORetryCount++;
                             int nIndex = GetStepIndex(StepList, STEP.SEND_CHECK_PO_MESSAGE);
                             if (nIndex >= 0)
                                 Step = StepList[StepIndex = nIndex];
                         }
                     }*/
                    NextStep();
                    break;
            }
        }

        public bool GetConnected()
        {
            return isConnected;
        }
    }
}
