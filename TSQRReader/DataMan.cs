using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopEng.Utils;
using System.IO;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.IO.Ports;
using System.Windows.Forms;

namespace TopEng.Device
{
    public class DataMan
    {
        public List<SerialComm> QRReader = new List<SerialComm>();
        public List<SERIALCOMMINFO> CommInfo = new List<SERIALCOMMINFO>();

        public string filepath;
        private object RunLockObject = new object();

        public event EventHandler<SerialDataEventArgs> SerialDataRecieved = null;


        public bool Open(string filepath)
        {
            try
            {
                this.filepath = filepath;

                // LIGHT INFO
                if (File.Exists(filepath))
                {
                    string streamJson = File.ReadAllText(filepath);
                    streamJson = Regex.Unescape(streamJson);
                    CommInfo = JsonSerializer.Deserialize<List<SERIALCOMMINFO>>(streamJson);
                }
                else
                {
                    string msg = $"Failed to read file a {filepath}";
                    MessageBox.Show(msg);
                    return false;
                }

                for (int i = 0; i < CommInfo.Count; i++)
                {
                    QRReader.Add(new SerialComm(CommInfo[i].Port, CommInfo[i].Baudrate, CommInfo[i].DataBits,
                        (StopBits)CommInfo[i].StopBits, (Parity)CommInfo[i].ParityBit));
                    QRReader[i].Open();
                    QRReader[i].SerialDataRecieved += new EventHandler<SerialDataEventArgs>(OnDataReceiveEvent);
                }

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public void Close()
        {
            foreach (var dev in QRReader)
                dev.Close();
        }

        public void Write(string filepath)
        {
            var options = new JsonSerializerOptions { WriteIndented = true };

            // CAMERA INFO
            string streamCam = JsonSerializer.Serialize(CommInfo, options);
            streamCam = Regex.Unescape(streamCam);
            if (!IOFunctionUtil.IsDiskSpaceAvailable(filepath, streamCam)) return;
            File.WriteAllText(filepath, streamCam);
        }

        private void OnDataReceiveEvent(object sender, SerialDataEventArgs e)
        {
            SerialDataRecieved?.Invoke(this, e);
        }

        //private void OnDataReceiveEvent(byte[] data)
        //{
        //    try
        //    {
        //        lock (RunLockObject)
        //        {
        //            receivedData = Convert.ToBase64String(data);
        //            //int dataLength = ComPort.BytesToRead;
        //            //byte[] data = new byte[dataLength];
        //            //int numberOfByteRead = ComPort.Read(data, 0, dataLength);

        //            //if (numberOfByteRead != 0)
        //            //{
        //            //    SerialDataRecieved?.Invoke(this, new SerialDataEventArgs(data));
        //            //}
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        public void Send(int dev, string trig)
        {
            if (dev < QRReader.Count)
            {
                if (!QRReader[dev].Isopenned())
                    return;
            }
            else
                return;

            try
            {
                lock (RunLockObject)
                {
                    QRReader[dev].Send(trig);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
