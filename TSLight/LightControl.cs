using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using TopEng.Utils;

namespace TopEng.Vision
{
    public class LIGHTDATA
    {
        public int a { get; set; }
        public int b { get; set; }
        public int c { get; set; }
        public int d { get; set; }
        
        

    }

    public class LightControl
    {
        public List<SerialComm> light = new List<SerialComm>();
        public List<SERIALCOMMINFO> lightInfo = new List<SERIALCOMMINFO>();

        public string filepath;
        private object RunLockObject = new object();

        private List<List<int>> dataList = new List<List<int>>();

        public bool Open(string filepath)
        {
            this.filepath = filepath;

            // LIGHT INFO
            if (File.Exists(filepath))
            {
                string streamJson = File.ReadAllText(filepath);
                streamJson = Regex.Unescape(streamJson);
                lightInfo = JsonSerializer.Deserialize<List<SERIALCOMMINFO>>(streamJson);
            }
            else
            {
                string msg = $"Failed to read file a {filepath}";
                MessageBox.Show(msg);
                return false;
            }

            for (int i = 0; i < lightInfo.Count; i++)
            {
                light.Add(new SerialComm(lightInfo[i].Port, lightInfo[i].Baudrate, lightInfo[i].DataBits,
                    (StopBits)lightInfo[i].StopBits, (Parity)lightInfo[i].ParityBit));
                light[i].Open();

                dataList.Add(new List<int>());
                for (int n = 0; n < lightInfo[i].Channels; n++)
                    dataList[i].Add(0);
            }

            return true;
        }

        public void Write(string filepath)
        {
            var options = new JsonSerializerOptions { WriteIndented = true };

            // CAMERA INFO
            string streamCam = JsonSerializer.Serialize(lightInfo, options);
            streamCam = Regex.Unescape(streamCam);
            if (!IOFunctionUtil.IsDiskSpaceAvailable(filepath, streamCam)) return;
            File.WriteAllText(filepath, streamCam);
        }

        public bool LightSet(int dev, int Channel, int Level)
        {
            lock (RunLockObject)
            {
                if (dev < light.Count)
                {
                    if (!light[dev].Isopenned() || Channel < 0 || Channel >= lightInfo[dev].Channels)
                        return false;
                }
                else
                    return false;

                if (Level <= 0) Level = 0;
                if (Level >= 255) Level = 255;

                try
                {
                    byte checkSum = (byte)(0x12 ^ Channel ^ Level);

                    byte[] packet = new byte[7];
                    packet[0] = 0x4c;
                    packet[1] = 0x12;
                    packet[2] = (byte)Channel;
                    packet[3] = (byte)Level;
                    packet[4] = checkSum;
                    packet[5] = 0x0D;
                    packet[6] = 0x0A;

                    light[dev].Send(packet);
                    Thread.Sleep(50);

                    return true;
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }

        public bool LightSet(int dev, List<int> Levels)
        {
            lock (RunLockObject)
            {
                if (dev < light.Count)
                {
                    if (!light[dev].Isopenned() || Levels.Count == 0)
                        return false;
                }
                else
                    return false;

                try
                {
                    int channels = lightInfo[dev].Channels;
                    int checkSum = 0x15;

                    byte[] packet = new byte[channels + 5];
                    packet[0] = 0x4c;
                    packet[1] = 0x15;

                    for (int i = 0; i < channels; i++)
                    {
                        int Level = i < Levels.Count ? Levels[i] : 0;
                        if (Level < 0) Level = dataList[dev][i];
                        if (Level >= 255) Level = 255;
                        dataList[dev][i] = Level;

                        packet[2 + i] = (byte)Level;
                        checkSum = checkSum ^ Level;
                    }

                    packet[2 + channels] = (byte)checkSum;
                    packet[3 + channels] = 0x0D;
                    packet[4 + channels] = 0x0A;

                    light[dev].Send(packet);
                    Thread.Sleep(50);

                    return true;
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }

        public void Close()
        {
            foreach (var dev in light)
                dev.Close();
        }

        public void ManualOpen(int ID)
        {
            if (ID >= light.Count) return;
            light[ID].Open();
        }

        public void ManualClose(int ID)
        {
            if (ID >= light.Count) return;
            light[ID].Close();
        }

        public bool IsConnected(int Id) => lightInfo.Count > Id && light[Id].Isopenned();
    }
}
