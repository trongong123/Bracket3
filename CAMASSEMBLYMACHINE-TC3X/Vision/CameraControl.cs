using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using TopEng.Utils;
using TopEng.Vision;
using TopEng.Vision.Camera;
using CAMASSEMBLYMACHINE.Define;

namespace CAMASSEMBLYMACHINE
{
    public class CAMERAINFO
    {
        public string Name { get; set; } = "";
        public string Ip { get; set; } = "192.168.0.10";
        public double Exposure { get; set; } = 35;
        public double Gain { get; set; } = 1;
        public int ResolutionX { get; set; } = 0;
        public int ResolutionY { get; set; } = 0;
        public double PixelSizeX { get; set; } = 1;
        public double PixelSizeY { get; set; } = 1;
        public double RatioLeftX { get; set; } = 1.0;
        public double RatioLeftY { get; set; } = 1.0;
        public double RatioRightX { get; set; } = 1.0;
        public double RatioRightY { get; set; } = 1.0;
        public string Calibration { get; set; } = "Axis";
        public double Angle { get; set; } = 0;
        public double Slope { get; set; } = 0;
        public string SlopeBase { get; set; } = "X"; // X, Y
        public double OriginX { get; set; } = 0;
        public double OriginY { get; set; } = 0;
        public string FlapX { get; set; } = "False";
        public string FlapY { get; set; } = "False";
        public string Rotation { get; set; } = "DEG0";
        public double ExtReferenceX { get; set; } = 0;
        public double ExtReferenceY { get; set; } = 0;

        public int lightDev { get; set; } = 0;
        public LIGHTDATA targetLight { get; set; }
        public LIGHTDATA lightSet { get; set; }
        public bool lightPowerControl { get; set; }
        public string InterferenceCam { get; set; } = "";
        public int CheckerBoardCal { get; set; } = 0;
    }

    public class CameraControl
    {
        public List<CAMERAINFO> cameraInfo = new List<CAMERAINFO>();
        public Dictionary<string, int> cameraIndex = new Dictionary<string, int>();

        public HikrobotControl grabber = new HikrobotControl();
        private DaraeLightControl light = new DaraeLightControl();

        object RunLockObject = new object();

        public CameraControl()
        {

        }

        public int CameraCount()
        {
            return grabber.cameras.Count;
        }

        public void Open()
        {
            string filepath = SystemDefine.systemPath + @"\CameraInfo.json";
            bool returnValue = true;

            if (File.Exists(filepath))
            {
                string streamJson = File.ReadAllText(filepath);
                streamJson = Regex.Unescape(streamJson);
                cameraInfo = JsonSerializer.Deserialize<List<CAMERAINFO>>(streamJson);
            }
            else
            {
                string msg = $"Failed to read file a {filepath}";
                LogUtil.Instance.Log(LOG_TYPE.ALARM, msg, CONTENT_TYPE.INFO);
                returnValue = false;
            }

            cameraIndex.Clear();

            if (!SystemDefine.IS_NOTEBOOK_MODE)
            {
                for (int i = 0; i < cameraInfo.Count; i++)
                    cameraIndex.Add(cameraInfo[i].Name, i);

                if (!light.Open(SystemDefine.systemPath + @"\LightInfo.json"))
                    returnValue = false;

                GrabberOpen();
            }

            if (!returnValue)
            {
                throw new Exception("The program can not be executed." + Environment.NewLine + "Please check the vision setting file.");
            }
        }

        public void Write()
        {
            try
            {
                string filepath = SystemDefine.systemPath + @"\CameraInfo.json";
                var options = new JsonSerializerOptions { WriteIndented = true };

                // CAMERA INFO
                string streamCam = JsonSerializer.Serialize(cameraInfo, options);
                streamCam = Regex.Unescape(streamCam);
                if (!IOFunctionUtil.IsDiskSpaceAvailable(filepath, streamCam)) return;
                File.WriteAllText(filepath, streamCam);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public bool IsCameraConnected(int Id)
        {
            try
            {
                return grabber.IsConnected(Id);
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public bool IsLightConnected(int Id)
        {
            try
            {
                if (Id >= cameraInfo.Count) return false;
                return light.IsConnected(cameraInfo[Id].lightDev);
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public void CameraManualOpen(int Id)
        {
            try
            {
                grabber.ManualOpen(Id);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Exception Occurred! ({ex})", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public void CameraManualClose(int Id)
        {
            try
            {
                grabber.ManualClose(Id);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Exception Occurred! ({ex})", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public void LightManualOpen(int Id)
        {
            try
            {
                if (Id >= cameraInfo.Count) return;
                light.ManualOpen(cameraInfo[Id].lightDev);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Exception Occurred! ({ex})", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public void LightManualClose(int Id)
        {
            try
            {
                if (Id >= cameraInfo.Count) return;
                light.ManualClose(cameraInfo[Id].lightDev);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Exception Occurred! ({ex})", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void GrabberOpen()
        {
            if (SystemDefine.IS_NOTEBOOK_MODE) return;
            grabber.Open();

            if (grabber.cameras.Count > cameraInfo.Count)
            {
                throw new Exception("There are fewer settings than active cameras." + Environment.NewLine + "Please check the vision setting file.");
            }

            for (int i = 0; i < grabber.cameras.Count; i++)
            {
                if (grabber.Width(i) != ResolutionX(i))
                {
                    if (DialogResult.Yes == MessageBox.Show($"The set camera[{i}] width and actual width are different. Would you like to renew?", "Warning", MessageBoxButtons.YesNo))
                        ResolutionX(i, grabber.Width(i));
                }
                if (grabber.Height(i) != ResolutionY(i))
                {
                    if (DialogResult.Yes == MessageBox.Show($"The set camera[{i}] height and actual height are different. Would you like to renew?", "Warning", MessageBoxButtons.YesNo))
                        ResolutionY(i, grabber.Height(i));
                }
                if (grabber.ExposureTime(i) != Exposure(i))
                {
                    if (SystemDefine.UpdateCameraInfo)
                    {
                        if (DialogResult.Yes == MessageBox.Show($"The set camera[{i}] exposure time and actual value are different. Would you like to renew?", "Warning", MessageBoxButtons.YesNo))
                            Exposure(i, grabber.ExposureTime(i));
                    }
                    else
                    {
                        if (DialogResult.Yes == MessageBox.Show($"The set camera[{i}] exposure time and actual value are different. Would you like to update camera information?", "Warning", MessageBoxButtons.YesNo))
                            grabber.ExposureTime(i, Exposure(i));
                    }
                }
                if (grabber.Gain(i) != Gain(i))
                {
                    if (SystemDefine.UpdateCameraInfo)
                    {
                        if (DialogResult.Yes == MessageBox.Show($"The set camera[{i}] gain and actual value are different. Would you like to renew?", "Warning", MessageBoxButtons.YesNo))
                            Gain(i, grabber.Gain(i));
                    }
                    else
                    {
                        if (DialogResult.Yes == MessageBox.Show($"The set camera[{i}] gain and actual value are different. Would you like to update camera information?", "Warning", MessageBoxButtons.YesNo))
                            grabber.Gain(i, Gain(i));
                    }
                }
                if (grabber.FlapX(i) != FlapX(i))
                {
                    if (SystemDefine.UpdateCameraInfo)
                    {
                        if (DialogResult.Yes == MessageBox.Show($"The set camera[{i}] gain and actual value are different. Would you like to renew?", "Warning", MessageBoxButtons.YesNo))
                            FlapX(i, grabber.FlapX(i));
                    }
                    else
                    {
                        if (DialogResult.Yes == MessageBox.Show($"The set camera[{i}] gain and actual value are different. Would you like to update camera information?", "Warning", MessageBoxButtons.YesNo))
                            grabber.FlapX(i, (FlapX(i) == "True" ? true : false));
                    }
                }
                if (grabber.FlapY(i) != FlapY(i))
                {
                    if (SystemDefine.UpdateCameraInfo)
                    {
                        if (DialogResult.Yes == MessageBox.Show($"The set camera[{i}] gain and actual value are different. Would you like to renew?", "Warning", MessageBoxButtons.YesNo))
                            FlapY(i, grabber.FlapY(i));
                    }
                    else
                    {
                        if (DialogResult.Yes == MessageBox.Show($"The set camera[{i}] gain and actual value are different. Would you like to update camera information?", "Warning", MessageBoxButtons.YesNo))
                            grabber.FlapY(i, (FlapY(i) == "True" ? true : false));
                    }
                }
            }

            Write();
        }

        public void SetExposure(int nID, double dVal)
        {
            grabber.ExposureTime(nID, dVal);
        }

        public double GetExposure(int nID)
        {
            double dExposure = 0.0;
            dExposure = grabber.ExposureTime(nID);

            return dExposure;
        }

        public int GetIndex(string name)
        {
            return cameraIndex.ContainsKey(name) ? cameraIndex[name] : -1;
        }

        public void LightSet(int dev, int Channel, int Level)
        {
            light.LightSet(dev, Channel, Level);
        }

        public void CloseLight()
        {
            light.Close();
        }

        public void LightOn(int targetCam, bool On)
        {
            if (SystemDefine.IS_NOTEBOOK_MODE) return;
            LIGHTDATA targetLight = cameraInfo[targetCam].targetLight;
            int targetDev = cameraInfo[targetCam].lightDev;

            int[] channelList = { targetLight.a, targetLight.b, targetLight.c, targetLight.d };
            int channels = light.lightInfo[targetDev].Channels;
            var targetChannels = new List<int>();
            for (int i = 0; i < channels; i++)
            {
                if (i == targetLight.a) targetChannels.Add(i);
                else if (i == targetLight.b) targetChannels.Add(i);
                else if (i == targetLight.c) targetChannels.Add(i);
                else if (i == targetLight.d) targetChannels.Add(i);
                else
                    targetChannels.Add(-1);
            }

            light.LightOn(targetDev, targetChannels, On);
        }

        public void LightSet(int targetCam, LIGHTDATA data)
        {
            if (SystemDefine.IS_NOTEBOOK_MODE) return;
            LIGHTDATA targetLight = cameraInfo[targetCam].targetLight;
            int interferenceCam = GetIndex(cameraInfo[targetCam].InterferenceCam);
            int targetLightDev = cameraInfo[targetCam].lightDev;

            // interference Light Turn Off
            if (interferenceCam >= 0)
            {
                int interferenceLightDev = cameraInfo[interferenceCam].lightDev;
                light.LightSet(interferenceLightDev, new List<int>(), cameraInfo[targetCam].lightPowerControl);
            }

            int[] tarLightList = { targetLight.a, targetLight.b, targetLight.c, targetLight.d };
            int channels = light.lightInfo[targetLightDev].Channels;
            var Levels = new List<int>();
            for (int i = 0; i < channels; i++)
            {
                if (i == targetLight.a) Levels.Add(data.a);
                else if (i == targetLight.b) Levels.Add(data.b);
                else if (i == targetLight.c) Levels.Add(data.c);
                else if (i == targetLight.d) Levels.Add(data.d);
                else
                    Levels.Add(-1);
            }

            light.LightSet(targetLightDev, Levels, cameraInfo[targetCam].lightPowerControl);
        }

        public void Grab(int Id)
        {
            if (Id < 0 || Id >= grabber.cameras.Count)
                return;
            grabber.Grab(Id);
        }

        public bool GrabCompleted(int Id)
        {
            if (Id < 0 || Id >= grabber.cameras.Count)
                return false;

            StopWatch stopWatch = new StopWatch();
            stopWatch.Reset();
            bool ret = true;

            while (true)
            {
                if (stopWatch.Elapsed > 1000) return false;
                if (grabber.GrabCompleted(Id)) return true;
                Util.Delay(100);
            }
        }

        public void LiveStart(int Id)
        {
            if (Id < 0 || Id >= grabber.cameras.Count)
                return;

            grabber.LiveStart(Id);
        }

        public void LiveStop(int Id)
        {
            if (Id < 0 || Id >= grabber.cameras.Count)
                return;
            grabber.LiveStop(Id);
        }

        public string Name(int Id)
        {
            if (cameraInfo.Count > Id)
                return cameraInfo[Id].Name;
            return "unnamed";
        }

        public string IP(int Id)
        {
            if (cameraInfo.Count > Id)
                return cameraInfo[Id].Ip;
            return "192.168.0.010";
        }

        public double Exposure(int Id)
        {
            if (cameraInfo.Count > Id)
                return cameraInfo[Id].Exposure;
            return 35;
        }

        public void Exposure(int Id, double time)
        {
            if (cameraInfo.Count > Id)
                cameraInfo[Id].Exposure = time;
        }

        public double Gain(int Id)
        {
            if (cameraInfo.Count > Id)
                return cameraInfo[Id].Gain;
            return 1;
        }

        public void Gain(int Id, double value)
        {
            if (cameraInfo.Count > Id)
                cameraInfo[Id].Gain = value;
        }

        public double ResolutionX(int Id)
        {
            if (cameraInfo.Count > Id)
                return cameraInfo[Id].ResolutionX;
            return 0;
        }

        public void ResolutionX(int Id, int Width)
        {
            if (cameraInfo.Count > Id)
                cameraInfo[Id].ResolutionX = Width;
        }

        public double ResolutionY(int Id)
        {
            if (cameraInfo.Count > Id)
                return cameraInfo[Id].ResolutionY;
            return 0;
        }

        public void ResolutionY(int Id, int Height)
        {
            if (cameraInfo.Count > Id)
                cameraInfo[Id].ResolutionY = Height;
        }

        public double PixelSizeX(int Id)
        {
            if (cameraInfo.Count > Id)
                return cameraInfo[Id].PixelSizeX;
            return 0;
        }

        public void PixelSizeX(int Id, double size)
        {
            if (cameraInfo.Count > Id)
                cameraInfo[Id].PixelSizeX = size;
        }

        public double PixelSizeY(int Id)
        {
            if (cameraInfo.Count > Id)
                return cameraInfo[Id].PixelSizeY;
            return 0;
        }

        public void PixelSizeY(int Id, double size)
        {
            if (cameraInfo.Count > Id)
                cameraInfo[Id].PixelSizeY = size;
        }

        public void RatioLeftX(int Id, double value)
        {
            if (cameraInfo.Count > Id)
                cameraInfo[Id].RatioLeftX = value;
        }

        public double RatioLeftX(int Id)
        {
            if (cameraInfo.Count > Id)
                return cameraInfo[Id].RatioLeftX;
            return 0;
        }

        public void RatioLeftY(int Id, double value)
        {
            if (cameraInfo.Count > Id)
                cameraInfo[Id].RatioLeftY = value;
        }

        public double RatioLeftY(int Id)
        {
            if (cameraInfo.Count > Id)
                return cameraInfo[Id].RatioLeftY;
            return 0;
        }

        public void RatioRightX(int Id, double value)
        {
            if (cameraInfo.Count > Id)
                cameraInfo[Id].RatioRightX = value;
        }

        public double RatioRightX(int Id)
        {
            if (cameraInfo.Count > Id)
                return cameraInfo[Id].RatioRightX;
            return 0;
        }

        public void RatioRightY(int Id, double value)
        {
            if (cameraInfo.Count > Id)
                cameraInfo[Id].RatioRightY = value;
        }

        public double RatioRightY(int Id)
        {
            if (cameraInfo.Count > Id)
                return cameraInfo[Id].RatioRightY;
            return 0;
        }

        public double Angle(int Id)
        {
            if (cameraInfo.Count > Id)
                return cameraInfo[Id].Angle;
            return 0;
        }

        public void Angle(int Id, double angle)
        {
            if (cameraInfo.Count > Id)
                cameraInfo[Id].Angle = angle;
        }

        public double Slope(int Id)
        {
            if (cameraInfo.Count > Id)
                return cameraInfo[Id].Slope;
            return 0;
        }

        public void Slope(int Id, double angle)
        {
            if (cameraInfo.Count > Id)
                cameraInfo[Id].Slope = angle;
        }

        public double OriginX(int Id)
        {
            if (cameraInfo.Count > Id)
                return cameraInfo[Id].OriginX;
            return 0;
        }

        public double OriginY(int Id)
        {
            if (cameraInfo.Count > Id)
                return cameraInfo[Id].OriginY;
            return 0;
        }

        public string FlapX(int Id)
        {
            if (cameraInfo.Count > Id)
                return cameraInfo[Id].FlapX;
            return "False";
        }

        public void FlapX(int Id, string reverse)
        {
            if (cameraInfo.Count > Id)
                cameraInfo[Id].FlapX = reverse;
        }

        public string FlapY(int Id)
        {
            if (cameraInfo.Count > Id)
                return cameraInfo[Id].FlapY;
            return "False";
        }

        public void FlapY(int Id, string reverse)
        {
            if (cameraInfo.Count > Id)
                cameraInfo[Id].FlapY = reverse;
        }

        public string RotateType(int Id)
        {
            if (cameraInfo.Count > Id)
                return cameraInfo[Id].Rotation;
            return "DEG0";
        }

        public double ExtRefX(int Id)
        {
            if (cameraInfo.Count > Id)
                return cameraInfo[Id].ExtReferenceX;
            return 0.0;
        }

        public void ExtRefX(int Id, double RefX)
        {
            if (cameraInfo.Count > Id)
                cameraInfo[Id].ExtReferenceX = RefX;
        }

        public double ExtRefY(int Id)
        {
            if (cameraInfo.Count > Id)
                return cameraInfo[Id].ExtReferenceY;
            return 0.0;
        }

        public void ExtRefY(int Id, double RefY)
        {
            if (cameraInfo.Count > Id)
                cameraInfo[Id].ExtReferenceY = RefY;
        }

        public int UseCheckBoardCal(int Id)
        {
            if (cameraInfo.Count > Id)
                return cameraInfo[Id].CheckerBoardCal;
            return 0;
        }
    }
}
