using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.IO;
using System.Text.Json;
using System.Text.RegularExpressions;
using TopEng.Utils;
using System.Threading;

namespace TopEng.Utils
{
    public class SERIALCOMMINFO
    {
        public int Port { get; set; } = 1;
        public int Baudrate { get; set; } = 9600;
        public int DataBits { get; set; } = 8;
        public int StopBits { get; set; } = 1;
        public int ParityBit { get; set; } = 0;
        public int Channels { get; set; } = 1;
    }

    public class SerialDataEventArgs : EventArgs
    {
        public string data = "";

        /**
         * @brief Serial 통신을 통해 전달되는 매개변수 클래스
         * @param[in] 매개변수 배열
         */
        public SerialDataEventArgs(byte[] dataInByteArray)
        {
            data = Encoding.Default.GetString(dataInByteArray);
        }
    }

    public class SerialComm
    {
        public SerialPort ComPort = new SerialPort();

        public int PortNo = 1;
        public int Baudrate = 9600;
        public int Databits = 8;
        public StopBits Stopbits = StopBits.One;
        public Parity Paritybit = Parity.None;

        public event EventHandler<SerialDataEventArgs> SerialDataRecieved = null;
        private event SerialDataReceivedEventHandler DataReceivedHandler = null;

        private object lockOject = new object();

        public SerialComm()
        {

        }

        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                lock (lockOject)
                {
                    Thread.Sleep(100);
                    
                    int dataLength = ComPort.BytesToRead;
                    byte[] data = new byte[dataLength];
                    int numberOfByteRead = ComPort.Read(data, 0, dataLength);

                    if (numberOfByteRead != 0)
                    {
                        SerialDataRecieved?.Invoke(this, new SerialDataEventArgs(data));
                    }
                }
            }
            catch (Exception ex) 
            {
                byte[] data = new byte[1];
                data = Encoding.ASCII.GetBytes("N");
                SerialDataRecieved?.Invoke(this, new SerialDataEventArgs(data));
                //throw ex;
            }
        }

        public SerialComm(int port, int baudrate, int databits, StopBits stopbits, Parity paritybit)
        {
            PortNo = port;
            Baudrate = baudrate;
            Databits = databits;
            Stopbits = stopbits;
            Paritybit = paritybit;

            DataReceivedHandler = new SerialDataReceivedEventHandler(SerialPort_DataReceived);
        }

        public bool Open()
        {
            try
            {
                ComPort.PortName = "COM" + PortNo.ToString();
                ComPort.BaudRate = Baudrate;
                ComPort.DataBits = Databits;
                ComPort.StopBits = Stopbits;
                ComPort.Parity = Paritybit;
                ComPort.RtsEnable = true;
                ComPort.Handshake = Handshake.None;
                ComPort.DtrEnable = true;

                ComPort.DataReceived += DataReceivedHandler;

                if (ComPort.IsOpen)
                    ComPort.Close();

                ComPort.Open();
                return ComPort.IsOpen;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void Close()
        {
            try
            {
                ComPort.Close();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public bool Isopenned()
        {
            return ComPort.IsOpen;
        }

        public void Send(string command)
        {
            try
            {
                lock (lockOject)
                {
                    ComPort.Write(command);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public void Send(byte[] buffer)
        {
            try
            {
                lock (lockOject)
                {
                    ComPort.Write(buffer, 0, buffer.Length);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
