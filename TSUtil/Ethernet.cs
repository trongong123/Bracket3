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
using System.Net.Sockets;
using System.Net;
using System.Net.NetworkInformation;

namespace TopEng.Utils
{
    public class EthernetInfoStruct
    {
        public string IP { get; set; } = string.Empty;
        public int portNumber { get; set; } = 0;
    }
    /**
     * @brief Ethernet 모드
     */
    public enum EthernetMode
    {
        eNone,
        eServerMode,
        eClientMode
    };

    /**
     * @brief Ethernet 결과
     */
    public enum EthernetResult
    {
        SUCCESS,
        SERVER_ACCEPT_ERROR,
        CLIENT_CONNECT_ERROR,
        SEND_DATA_ERROR,
        SEND_DATA_CONNECT_CLOSE_ERROR,
    };

    /**
     * @brief 비동기 통신 상태 정보 클래스
     */
    public class ConnectedSocketStateObject
    {
        // Client  socket.  
        private Socket workSocket = null;
        // Size of receive buffer.  
        private const int bufferSize = 1024 * 12;
        // Receive buffer.  
        private byte[] receiveBuffer = new byte[bufferSize];
        // Receive buffer.  
        private int receiveDataSize;
        // Received data string.  
        private StringBuilder receiveDataStringBuilder = new StringBuilder();

        public Socket WorkSocket
        {
            get { return workSocket; }
            set { workSocket = value; }
        }
        public int BufferSize
        {
            get { return bufferSize; }
        }
        public byte[] ReceiveBuffer
        {
            get { return receiveBuffer; }
            set { receiveBuffer = value; }
        }
        public int ReceiveDataSize
        {
            get { return receiveDataSize; }
            set { receiveDataSize = value; }
        }
        public StringBuilder ReceiveDataStringBuilder
        {
            get { return receiveDataStringBuilder; }
            set { receiveDataStringBuilder = value; }
        }

        public void ClearBuffer()
        {
            Array.Clear(receiveBuffer, 0, BufferSize);
        }
    }

    /**
     * @brief Ethernet 통신 클래스
     */
    public class Ethernet
    {
        private static string CLASS_NAME = "Ethernet";

        private const int MinimumPortNum = 0;
        private const int InvalidPortNum = -1;

        private EthernetMode mode;
        private IPAddress ipAddress;
        private int portNum;
        private Socket mainSocket;
        private IPEndPoint endPoint;
        private object ReceiveLockObject;
        private object SendLockObject;
        private List<ConnectedSocketStateObject> connectedSocketList;

        public delegate void ConnectCallbackEvent(EthernetResult result, string remoteEndPoint);
        public event ConnectCallbackEvent ConnectCallbackEventHandler;
        public delegate void SendDataCallbackEvent(EthernetResult result, string remoteEndPoint, string sendData = null);
        public event SendDataCallbackEvent SendDataCallbackEventHandler;
        public delegate void ReceiveDataCallbackEvent(string readData, string remoteEndPoint);
        public event ReceiveDataCallbackEvent ReceiveDataCallbackEventHandler;
        public delegate void DisconnectedCallbackEvent(string remoteEndPoint);
        public event DisconnectedCallbackEvent DisconnectedCallbackEventHandler;

        /**
         * @brief Ethernet 클래스 생성자
         * @param[in] mode EthernetMode 타입의 모드
         * @param[in] ipAddress Target IP address (string)
         * @param[in] portNum Target port number
         */
        public Ethernet(EthernetMode mode = EthernetMode.eNone, string ipAddress = null, int portNum = InvalidPortNum)
        {
            this.mode = mode;
            this.ipAddress = IPAddress.Parse(ipAddress);
            this.portNum = portNum;

            mainSocket = null;
            endPoint = null;
            this.ReceiveLockObject = new object();
            this.SendLockObject = new object();
        }

        /**
         * @brief Ethernet 클래스 생성자
         * @param[in] mode EthernetMode 타입의 모드
         * @param[in] ipAddress Target IP address (IPAddress)
         * @param[in] portNum Target port number
         */
        public Ethernet(EthernetMode mode = EthernetMode.eNone, IPAddress ipAddress = null, int portNum = InvalidPortNum)
        {
            this.mode = mode;
            this.ipAddress = ipAddress;
            this.portNum = portNum;

            mainSocket = null;
            endPoint = null;
        }

        /**
         * @brief Ethernet 연결 함수
         * @details 생성자로 전달 된 EthernetMode에 맞는 연결 요청
         */
        public void Connect()
        {
            //ProcLogUtil.Instance.Log(LOG_TYPE.DEVELOPMENT, "Ethernet Connected", CONTENT_TYPE.DEBUG);
            try
            {
                switch (mode)
                {
                    case EthernetMode.eServerMode:
                        if (mainSocket != null) return;
                        startServer();
                        break;

                    case EthernetMode.eClientMode:
                        startClient();
                        break;

                    default:
                        //LogUtil.Instance.Log(LOG_TYPE.DEVELOPMENT, "Ethernet Wrong Mode", CONTENT_TYPE.DEBUG);
                        break;
                }
            }
            catch (Exception ex)
            {
                //LogUtil.Instance.Log(LOG_TYPE.DEVELOPMENT, ex.ToString(), CONTENT_TYPE.DEBUG);
                throw ex;
            }
        }

        /**
         * @brief Ethernet 연결 종료 함수
         * @details 생성자로 전달 된 EthernetMode에 맞는 연결 종료 처리
         */
        public void DisConnect()
        {
            //LogUtil.Instance.Log(LOG_TYPE.DEVELOPMENT, "Ethernet Disconnected", CONTENT_TYPE.DEBUG);
            try
            {
                switch (mode)
                {
                    case EthernetMode.eServerMode:
                        CloseAllSocket();
                        break;
                    case EthernetMode.eClientMode:
                        CloseAllSocket();
                        break;
                }
            }
            catch (Exception ex)
            {
                //LogUtil.Instance.Log(LOG_TYPE.DEVELOPMENT, ex.ToString(), CONTENT_TYPE.DEBUG);
                throw ex;
            }

        }

        /**
         * @brief 서버 시작 함수
         * @details EthernetMode 중 server mode 일때 해당
         */
        private void startServer()
        {
            //LogUtil.Instance.Log(LOG_TYPE.DEVELOPMENT, "Ethernet Start server", CONTENT_TYPE.DEBUG);
            if ((ipAddress == null) || (portNum < MinimumPortNum))
            {
                //LogUtil.Instance.Log(LOG_TYPE.DEVELOPMENT, "Ethernet wrong parameter", CONTENT_TYPE.DEBUG);
                throw new Exception("Wrong parameter");
            }

            endPoint = new IPEndPoint(ipAddress, portNum);
            mainSocket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            connectedSocketList = new List<ConnectedSocketStateObject>();

            try
            {
                mainSocket.Bind(endPoint);
                mainSocket.Listen(10);
                mainSocket.BeginAccept(new AsyncCallback(AcceptCallback), mainSocket);
                //LogUtil.Instance.Log(LOG_TYPE.DEVELOPMENT, "Ethernet wrong BeginAccept", CONTENT_TYPE.DEBUG);

            }
            catch (Exception ex)
            {
                //LogUtil.Instance.Log(LOG_TYPE.DEVELOPMENT, ex.ToString(), CONTENT_TYPE.DEBUG);
                throw ex;
            }
        }

        /**
         * @brief 연결 허가 callback 함수
         * @param[in] ar IAsyncResult 타입으로 ascync state 등의 정보를 가지고 있는 매개변수
         */
        private void AcceptCallback(IAsyncResult ar)
        {
            //LogUtil.Instance.Log(LOG_TYPE.DEVELOPMENT, "Ethernet Accept Callback", CONTENT_TYPE.DEBUG);
            Socket client = null;
            try
            {
                // Get the socket that handles the client request.
                client = ((Socket)ar.AsyncState).EndAccept(ar);
                mainSocket.BeginAccept(AcceptCallback, mainSocket);

                // Create the state object.
                ConnectedSocketStateObject connSocketState = new ConnectedSocketStateObject();
                connSocketState.WorkSocket = client;
                connectedSocketList.Add(connSocketState);

                ConnectCallbackEventHandler?.Invoke(EthernetResult.SUCCESS, client.RemoteEndPoint.ToString());

                client.BeginReceive(connSocketState.ReceiveBuffer, 0, connSocketState.BufferSize, 0, new AsyncCallback(ReceiveDataCallback), connSocketState);
                //LogUtil.Instance.Log(LOG_TYPE.DEVELOPMENT, "Ethernet Accept success", CONTENT_TYPE.DEBUG);

            }
            catch (Exception ex)
            {
                //LogUtil.Instance.Log(LOG_TYPE.DEVELOPMENT, ex.ToString(), CONTENT_TYPE.DEBUG);
                string endPoint = null;

                if ((client != null) && (client.RemoteEndPoint != null))
                {
                    endPoint = client.RemoteEndPoint.ToString();
                }
                ConnectCallbackEventHandler?.Invoke(EthernetResult.SERVER_ACCEPT_ERROR, endPoint);
            }
        }

        /**
         * @brief Client 시작 함수
         * @details EthernetMode 중 client mode 일때 해당
         */
        private void startClient()
        {
            try
            {
                //LogUtil.Instance.Log(LOG_TYPE.DEVELOPMENT, "Ethernet start client", CONTENT_TYPE.DEBUG);
                if ((ipAddress == null) || (portNum < MinimumPortNum))
                {
                    //LogUtil.Instance.Log(LOG_TYPE.DEVELOPMENT, "Ethernet Wrong parameter", CONTENT_TYPE.DEBUG);
                    throw new Exception("Wrong parameter");
                }

                endPoint = new IPEndPoint(ipAddress, portNum);
                mainSocket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                if (mainSocket != null)
                {
                    if (!mainSocket.Connected)
                    {
                        try
                        {
                            mainSocket.BeginConnect(endPoint, new AsyncCallback(ConnectCallback), mainSocket);
                            //LogUtil.Instance.Log(LOG_TYPE.DEVELOPMENT, "Ethernet Wrong parameter", CONTENT_TYPE.DEBUG);
                        }
                        catch (Exception ex)
                        {
                            //LogUtil.Instance.Log(Util.GetCommLogTypeByIPAddress(this.ipAddress.ToString()), $"{ex}", CONTENT_TYPE.EXC, Util.GetCommMainUnitTypeByIPAddress(this.ipAddress.ToString()), Util.GetCommSubUnitTypeByIPAddress(this.ipAddress.ToString(), this.portNum));
                            throw new Exception("Start client error!");
                        }
                    }
                }
                else
                {
                    //LogUtil.Instance.Log(Util.GetCommLogTypeByIPAddress(this.ipAddress.ToString()), "mainSocket is null", CONTENT_TYPE.EXC, Util.GetCommMainUnitTypeByIPAddress(this.ipAddress.ToString()), Util.GetCommSubUnitTypeByIPAddress(this.ipAddress.ToString(), this.portNum));
                }
            }
            catch (Exception e)
            {
                //LogUtil.Instance.Log(Util.GetCommLogTypeByIPAddress(this.ipAddress.ToString()), e.ToString(), CONTENT_TYPE.EXC, Util.GetCommMainUnitTypeByIPAddress(this.ipAddress.ToString()), Util.GetCommSubUnitTypeByIPAddress(this.ipAddress.ToString(), this.portNum));
            }
        }

        /**
         * @brief Connection에 대한 응답이 이루어지는 함수
         * @param[in] ar IAsyncResult 타입으로 ascync state 등의 정보를 가지고 있는 매개변수
         */
        private void ConnectCallback(IAsyncResult ar)
        {
            //LogUtil.Instance.Log(Util.GetCommLogTypeByIPAddress(this.ipAddress.ToString()), "ConnectCallback", CONTENT_TYPE.DBG, Util.GetCommMainUnitTypeByIPAddress(this.ipAddress.ToString()), Util.GetCommSubUnitTypeByIPAddress(this.ipAddress.ToString(), this.portNum));
            Socket client = null;
            try
            {
                // Retrieve the socket from the state object.  
                client = (Socket)ar.AsyncState;

                // Complete the connection.  
                client.EndConnect(ar);

                ConnectedSocketStateObject connSocketState = new ConnectedSocketStateObject();
                connSocketState.WorkSocket = mainSocket;

                ConnectCallbackEventHandler?.Invoke(EthernetResult.SUCCESS, ipAddress.ToString());

                client.BeginReceive(connSocketState.ReceiveBuffer, 0, connSocketState.BufferSize, 0, new AsyncCallback(ReceiveDataCallback), connSocketState);
                //LogUtil.Instance.Log(Util.GetCommLogTypeByIPAddress(this.ipAddress.ToString()), "BeginReceive", CONTENT_TYPE.INF, Util.GetCommMainUnitTypeByIPAddress(this.ipAddress.ToString()), Util.GetCommSubUnitTypeByIPAddress(this.ipAddress.ToString(), this.portNum));


            }
            catch (Exception ex)
            {
                //LogUtil.Instance.Log(Util.GetCommLogTypeByIPAddress(this.ipAddress.ToString()), $"{ex}", CONTENT_TYPE.EXC, Util.GetCommMainUnitTypeByIPAddress(this.ipAddress.ToString()), Util.GetCommSubUnitTypeByIPAddress(this.ipAddress.ToString(), this.portNum));
                ConnectCallbackEventHandler?.Invoke(EthernetResult.CLIENT_CONNECT_ERROR, ipAddress.ToString());
                CloseAllSocket();
            }
        }

        /**
         * @brief Target으로 부터 receive 받게 되면 불리는 함수
         * @param[in] ar IAsyncResult 타입으로 ascync state 등의 정보를 가지고 있는 매개변수
         */
        private void ReceiveDataCallback(IAsyncResult ar)
        {
            lock (ReceiveLockObject)
            {
                //LogUtil.Instance.Log(Util.GetCommLogTypeByIPAddress(this.ipAddress.ToString()), "ReceiveDataCallback", CONTENT_TYPE.DBG, Util.GetCommMainUnitTypeByIPAddress(this.ipAddress.ToString()), Util.GetCommSubUnitTypeByIPAddress(this.ipAddress.ToString(), this.portNum));
                string content = String.Empty;

                // Retrieve the state object and the handler socket  
                // from the asynchronous state object.  
                ConnectedSocketStateObject connSocketState = (ar.AsyncState) as ConnectedSocketStateObject;

                try
                {
                    // Read data from the remote socket.
                    connSocketState.ReceiveDataSize = connSocketState.WorkSocket.EndReceive(ar);

                    if (connSocketState.ReceiveDataSize > 0)
                    {
                        // There  might be more data, so store the data received so far.  
                        connSocketState.ReceiveDataStringBuilder.Append(Encoding.UTF8.GetString(connSocketState.ReceiveBuffer, 0, connSocketState.ReceiveDataSize));
                        content = connSocketState.ReceiveDataStringBuilder.ToString();
                        connSocketState.ClearBuffer();
                        connSocketState.ReceiveDataStringBuilder.Clear();
                        connSocketState.ReceiveDataSize = 0;
                        ReceiveDataCallbackEventHandler?.Invoke(content, connSocketState.WorkSocket.RemoteEndPoint.ToString());

                        connSocketState.WorkSocket.BeginReceive(connSocketState.ReceiveBuffer, 0, connSocketState.BufferSize, 0, new AsyncCallback(ReceiveDataCallback), connSocketState);
                        //LogUtil.Instance.Log(Util.GetCommLogTypeByIPAddress(this.ipAddress.ToString()), $"[{connSocketState.WorkSocket.RemoteEndPoint}]{content}", CONTENT_TYPE.DBG, Util.GetCommMainUnitTypeByIPAddress(this.ipAddress.ToString()), Util.GetCommSubUnitTypeByIPAddress(this.ipAddress.ToString(), this.portNum));
                    }
                    else
                    {
                        throw new Exception("ReceivedDataSize Error!");
                    }
                }
                catch (ObjectDisposedException objectDisposedException)
                {
                    //LogUtil.Instance.Log(Util.GetCommLogTypeByIPAddress(this.ipAddress.ToString()), $"{objectDisposedException}", CONTENT_TYPE.EXC, Util.GetCommMainUnitTypeByIPAddress(this.ipAddress.ToString()), Util.GetCommSubUnitTypeByIPAddress(this.ipAddress.ToString(), this.portNum));
                    DisconnectedCallbackEventHandler?.Invoke(null);
                }
                catch (Exception ex)
                {
                    //LogUtil.Instance.Log(Util.GetCommLogTypeByIPAddress(this.ipAddress.ToString()), $"{ex}", CONTENT_TYPE.EXC, Util.GetCommMainUnitTypeByIPAddress(this.ipAddress.ToString()), Util.GetCommSubUnitTypeByIPAddress(this.ipAddress.ToString(), this.portNum));

                    string endPoint = null;

                    if (mode == EthernetMode.eServerMode)
                    {
                        if ((connSocketState != null) && (connSocketState.WorkSocket != null))
                        {
                            if (connSocketState.WorkSocket.RemoteEndPoint != null)
                            {
                                endPoint = connSocketState.WorkSocket.RemoteEndPoint.ToString();
                            }
                            removeDisconnectedSocket(connSocketState.WorkSocket);
                        }
                    }
                    else
                    {
                        endPoint = ipAddress.ToString();
                    }

                    DisconnectedCallbackEventHandler?.Invoke(endPoint);
                }
            }
        }

        /**
         * @brief 연결이 끊긴 socket을 메모리에서 제거하는 함수
         * @param[in] socket 연결 끊긴 socket
         */
        private void removeDisconnectedSocket(Socket socket)
        {
            if (connectedSocketList != null)
            {
                for (int connectedClientsCount = connectedSocketList.Count - 1; connectedClientsCount >= 0; connectedClientsCount--)
                {
                    ConnectedSocketStateObject connSocketState = connectedSocketList[connectedClientsCount];
                    if (socket.RemoteEndPoint.Equals(connSocketState.WorkSocket.RemoteEndPoint) &&
                        socket.LocalEndPoint.Equals(connSocketState.WorkSocket.LocalEndPoint))
                    {
                        connSocketState.WorkSocket.Close();
                        connectedSocketList.RemoveAt(connectedClientsCount);
                        break;
                    }
                }
            }
        }

        /**
         * @brief 데이터 전송 함수
         * @param[in] data 전송할 데이터
         * @param[in] remoteIpAddress Target의 IP address (string)
         * @param[in] remotePortNum Target의 Port number
         */
        public void SendData(string data, string remoteIpAddress = null, int remotePortNum = InvalidPortNum)
        {
            lock (SendLockObject)
            {
                //LogUtil.Instance.Log(Util.GetCommLogTypeByIPAddress(this.ipAddress.ToString()), "SendData", CONTENT_TYPE.INF, Util.GetCommMainUnitTypeByIPAddress(this.ipAddress.ToString()), Util.GetCommSubUnitTypeByIPAddress(this.ipAddress.ToString(), this.portNum));
                if (mode == EthernetMode.eClientMode)
                {
                    if (isSocketConnected(mainSocket))
                    {
                        //LogUtil.Instance.Log(Util.GetCommLogTypeByIPAddress(this.ipAddress.ToString()), $"[{mainSocket.RemoteEndPoint}]{data}", CONTENT_TYPE.DBG, Util.GetCommMainUnitTypeByIPAddress(this.ipAddress.ToString()), Util.GetCommSubUnitTypeByIPAddress(this.ipAddress.ToString(), this.portNum));
                        SendData(mainSocket, data);
                    }
                    else
                    {
                        if (mainSocket != null)
                        {
                            SendDataCallbackEventHandler?.Invoke(EthernetResult.SEND_DATA_CONNECT_CLOSE_ERROR, mainSocket.RemoteEndPoint.ToString());
                        }
                        CloseAllSocket();
                    }
                }
                else
                {
                    Socket client = null;
                    for (int connectedClientsCount = connectedSocketList.Count - 1; connectedClientsCount >= 0; connectedClientsCount--)
                    {
                        ConnectedSocketStateObject connSocketState = connectedSocketList[connectedClientsCount];
                        if ((remoteIpAddress == null)
                            || ((remoteIpAddress != null) && IsSameDestination(remoteIpAddress, remotePortNum, connSocketState.WorkSocket)))
                        {
                            client = connSocketState.WorkSocket;
                            if (isSocketConnected(client))
                            {
                                //LogUtil.Instance.Log(Util.GetCommLogTypeByIPAddress(this.ipAddress.ToString()), $"[{client.RemoteEndPoint}]{data}", CONTENT_TYPE.DBG, Util.GetCommMainUnitTypeByIPAddress(this.ipAddress.ToString()), Util.GetCommSubUnitTypeByIPAddress(this.ipAddress.ToString(), this.portNum));
                                SendData(client, data);
                            }
                            else
                            {
                                string endPoint = null;

                                if (client != null && client.RemoteEndPoint != null)
                                {
                                    endPoint = client.RemoteEndPoint.ToString();
                                }
                                SendDataCallbackEventHandler?.Invoke(EthernetResult.SEND_DATA_CONNECT_CLOSE_ERROR, endPoint);
                                DisconnectedCallbackEventHandler?.Invoke(endPoint);

                                client.Close();
                                connectedSocketList.RemoveAt(connectedClientsCount);
                            }
                        }
                    }
                }
            }
        }

        /**
         * @brief 데이터 전송 함수
         * @param[in] handler 전송할 Target의 socket
         * @param[in] data 전송할 데이터
         */
        private void SendData(Socket handler, string data)
        {
            // Convert the string data to byte data using ASCII encoding.  
            byte[] byteData = Encoding.ASCII.GetBytes(data);
            //byte[] byteData = Encoding.ASCII.GetBytes(data);

            string endPoint = null;

            if (handler != null && handler.RemoteEndPoint != null)
            {
                endPoint = handler.RemoteEndPoint.ToString();
            }

            try
            {
                if (handler != null)
                {
                    // Begin sending the data to the remote device.  
                    handler.BeginSend(byteData, 0, byteData.Length, 0, new AsyncCallback(SendDataCallback), handler);
                    SendDataCallbackEventHandler?.Invoke(EthernetResult.SUCCESS, endPoint, data);
                }
            }
            catch (Exception ex)
            {
                //LogUtil.Instance.Log(Util.GetCommLogTypeByIPAddress(this.ipAddress.ToString()), $"{ex}", CONTENT_TYPE.EXC, Util.GetCommMainUnitTypeByIPAddress(this.ipAddress.ToString()), Util.GetCommSubUnitTypeByIPAddress(this.ipAddress.ToString(), this.portNum));
                SendDataCallbackEventHandler?.Invoke(EthernetResult.SEND_DATA_ERROR, endPoint);

            }
        }

        /**
         * @brief 데이터 전송 후 불리는 callback 함수
         * @param[in] ar IAsyncResult 타입으로 ascync state 등의 정보를 가지고 있는 매개변수
         */
        private void SendDataCallback(IAsyncResult ar)
        {
            Socket handler = null;

            try
            {
                // Retrieve the socket from the state object.  
                handler = (Socket)ar.AsyncState;

                // Complete sending the data to the remote device.  
                int bytesSent = handler.EndSend(ar);

                if (handler != null)
                {
                    SendDataCallbackEventHandler?.Invoke(EthernetResult.SUCCESS, handler.RemoteEndPoint.ToString());
                }
            }
            catch (Exception ex)
            {
                //LogUtil.Instance.Log(Util.GetCommLogTypeByIPAddress(this.ipAddress.ToString()), $"{ex}", CONTENT_TYPE.EXC, Util.GetCommMainUnitTypeByIPAddress(this.ipAddress.ToString()), Util.GetCommSubUnitTypeByIPAddress(this.ipAddress.ToString(), this.portNum));

                string endPoint = null;

                if (handler != null && handler.RemoteEndPoint != null)
                {
                    endPoint = handler.RemoteEndPoint.ToString();
                }
                SendDataCallbackEventHandler?.Invoke(EthernetResult.SEND_DATA_ERROR, endPoint);
            }
        }

        /**
         * @brief 연결 된 모든 socket 연결 종료 함수
         * @details ServerMode 일 경우에만 사용 
         */
        private void CloseAllSocket()
        {
            try
            {
                if (mainSocket != null)
                {
                    mainSocket.Close();
                }

                if ((mode == EthernetMode.eServerMode) && (connectedSocketList != null))
                {
                    for (int connectedClientsCount = connectedSocketList.Count - 1; connectedClientsCount >= 0; connectedClientsCount--)
                    {
                        ConnectedSocketStateObject connSocketState = connectedSocketList[connectedClientsCount];
                        if ((connSocketState != null) && (connSocketState.WorkSocket != null))
                        {
                            connSocketState.WorkSocket.Close();
                        }
                        connectedSocketList.RemoveAt(connectedClientsCount);
                    }
                }

                mainSocket = null;
                connectedSocketList = null;
            }
            catch (Exception ex)
            {
                //LogUtil.Instance.Log(Util.GetCommLogTypeByIPAddress(this.ipAddress.ToString()), $"{ex}", CONTENT_TYPE.EXC, Util.GetCommMainUnitTypeByIPAddress(this.ipAddress.ToString()), Util.GetCommSubUnitTypeByIPAddress(this.ipAddress.ToString(), this.portNum));
                throw ex;
            }
        }

        /**
         * @brief 소켓이 연결 되어있는지 확인하는 함수
         * @param[in] socket Target socket
         */
        private bool isSocketConnected(Socket socket)
        {
            bool result = false;
            IPGlobalProperties ipProperties = IPGlobalProperties.GetIPGlobalProperties();

            TcpConnectionInformation[] tcpConnections;

            try
            {
                if (socket == null)
                {
                    throw new Exception("Socket is null");
                }
                else
                {
                    tcpConnections = ipProperties.GetActiveTcpConnections().Where(
                        x => x.LocalEndPoint.Equals(socket.LocalEndPoint) && x.RemoteEndPoint.Equals(socket.RemoteEndPoint)).ToArray();
                }
            }
            catch (Exception ex)
            {
                //LogUtil.Instance.Log(Util.GetCommLogTypeByIPAddress(this.ipAddress.ToString()), $"{ex}", CONTENT_TYPE.EXC, Util.GetCommMainUnitTypeByIPAddress(this.ipAddress.ToString()), Util.GetCommSubUnitTypeByIPAddress(this.ipAddress.ToString(), this.portNum));
                return false;
            }

            if ((tcpConnections != null) && (tcpConnections.Length > 0))
            {
                TcpState stateOfConnection = tcpConnections.First().State;

                if (stateOfConnection == TcpState.Established)
                {
                    result = true; ;
                }
            }

            return result;
        }

        /**
         * @brief End point가 동일한 목적지인지 확인하는 함수 (동일 Target인지)
         * @param[in] remoteIpAddress Target IP address (string)
         * @param[in] remotePortNum Target port number
         * @param[in] socket Target socket
         * @return 동일한지 여부
         */
        private bool IsSameDestination(string remoteIpAddress, int remotePortNum, Socket socket)
        {
            bool result = false;

            try
            {
                IPEndPoint endPoint = (IPEndPoint)socket.RemoteEndPoint;
                string socketIpAddress = endPoint.Address.ToString();
                if (remoteIpAddress.Equals(socketIpAddress))
                {
                    if (remotePortNum != InvalidPortNum)
                    {
                        //if (remotePortNum == endPoint.Port)
                        {
                            result = true;
                        }
                    }
                    else
                    {
                        result = true;
                    }
                }
            }
            catch (Exception ex)
            {
                //LogUtil.Instance.Log(Util.GetCommLogTypeByIPAddress(this.ipAddress.ToString()), $"{ex}", CONTENT_TYPE.EXC, Util.GetCommMainUnitTypeByIPAddress(this.ipAddress.ToString()), Util.GetCommSubUnitTypeByIPAddress(this.ipAddress.ToString(), this.portNum));
                throw ex;
            }

            return result;
        }

        /**
         * @brief 현재 나의 IPAdress 얻는 클래스
         * @return IPAddress의 루프백
         */
        private IPAddress GetDefaultIpAddress()
        {
            IPHostEntry hostEntry = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress address in hostEntry.AddressList)
            {
                if (address.AddressFamily == AddressFamily.InterNetwork)
                {
                    return address;
                }
            }

            return IPAddress.Loopback;
        }
    }
}
