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
        public double Exposure { get; set; } = 1000;
    }

    public class CHANNELDATA
    {
        public int channel;
        public int on;
        public int level;
    }

    public class DaraeLightControl
    {
        public List<SerialComm> light = new List<SerialComm>();
        public List<SERIALCOMMINFO> lightInfo = new List<SERIALCOMMINFO>();

        public string filepath;
        private object RunLockObject = new object();

        private int[] Levels = new int[16];
        private int[] OnFlag = new int[16];
        private List<List<int>> dataList = new List<List<int>>();

        public bool Open(string filepath)
        {
            this.filepath = filepath;

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

            string streamCam = JsonSerializer.Serialize(lightInfo, options);
            streamCam = Regex.Unescape(streamCam);
            if (!IOFunctionUtil.IsDiskSpaceAvailable(filepath, streamCam)) return;
            File.WriteAllText(filepath, streamCam);
        }

        public bool LightOn(int dev, List<int> Channel, bool On)
        {
            lock (RunLockObject)
            {
                if (dev < light.Count)
                {
                    if (!light[dev].Isopenned() || Channel.Count == 0)
                        return false;
                }
                else
                    return false;

                try
                {
                    foreach (var ch in Channel)
                    {
                        if (ch >= 0 && ch < 16)
                            OnFlag[ch] = On ? 1 : 0;
                    }

                    int value = 0;

                    for (int i = 0; i < OnFlag.Length; i++)
                    {
                        value += OnFlag[i] << i;
                    }

                    light[dev].Send($"[PW,{value}#");
                    Thread.Sleep(10);

                    return true;
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }

        public bool LightSet(int dev, int Channel, int Level)
        {
            lock (RunLockObject)
            {
                if (dev < light.Count)
                {
                    if (!light[dev].Isopenned() || Channel < 0 || Channel > lightInfo[dev].Channels)
                        return false;
                }
                else
                    return false;

                try
                {
                    string ch_str = Channel.ToString("x");
                    // Light Value Set
                    int value = Math.Max(Math.Min(255, Level), 0);
                    string valueData = $"{value:000}";

                    light[dev].Send($"[CD{ch_str},{valueData}#");
                    Thread.Sleep(10);

                    return true;
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }

        public bool LightSet(int dev, List<int> Levels, bool lightPowerControl)
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

                    for (int i = 0; i < channels; i++)
                    {
                        if (Levels[i] != -1)
                        {
                            LightSet(dev, (i + 1), Levels[i]);
                        }
                    }

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
