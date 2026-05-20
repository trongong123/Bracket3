using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MvCameraControl;

namespace TopEng.Vision.Camera
{
    public class HikrobotControl
    {
        public Dictionary<int, HikrobotGrab> cameras = new Dictionary<int, HikrobotGrab>();

        public HikrobotControl()
        {

        }

        public void Open()
        {
            try
            {
                foreach (var cam in cameras)
                    cam.Value.Close();
                cameras.Clear();

                List<IDeviceInfo> allDeviceInfos = new List<IDeviceInfo>();
                int ret = DeviceEnumerator.EnumDevices(DeviceTLayerType.MvUsbDevice, out allDeviceInfos);
                if (ret != MvError.MV_OK)
                    return;

                if (allDeviceInfos.Count == 0)
                {
                    throw new ApplicationException("No USB cameras present.");
                }

                foreach (var info in allDeviceInfos)
                {
                    HikrobotGrab grab = new HikrobotGrab(info);
                    grab.Open();
                    if (grab.grabId >= 0)
                        cameras.Add(grab.grabId, grab);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void Grab(int Id)
        {
            if (cameras.Count > Id)
                cameras[Id].Grab();
        }

        public bool GrabCompleted(int Id)
        {
            if (cameras.Count > Id)
                return cameras[Id].grabCompleted;
            return false;
        }

        public void LiveStart(int Id)
        {
            if (cameras.Count > Id)
                cameras[Id].LiveSart();
        }

        public void LiveStop(int Id)
        {
            if (cameras.Count > Id)
                cameras[Id].LiveStop();
        }

        public int Width(int Id)
        {
            if (cameras.Count > Id)
                return cameras[Id].bitmap.Width;
            return 0;
        }

        public int Height(int Id)
        {
            if (cameras.Count > Id)
                return cameras[Id].bitmap.Height;
            return 0;
        }

        public double ExposureTime(int Id)
        {
            if (cameras.Count > Id)
                return cameras[Id].exposureTime;
            return 0;
        }

        public void ExposureTime(int Id, double time)
        {
            if (cameras.Count > Id)
                cameras[Id].ExposureTime(time);
        }

        public double Gain(int Id)
        {
            if (cameras.Count > Id)
                return cameras[Id].gain;
            return 0;
        }

        public void Gain(int Id, double value)
        {
            if (cameras.Count > Id)
                cameras[Id].Gain(value);
        }

        public string FlapX(int Id)
        {
            if (cameras.Count > Id)
                return cameras[Id].flapX ? "True" : "False";
            return "False";
        }

        public void FlapX(int Id, bool reverse)
        {
            if (cameras.Count > Id)
                cameras[Id].FlapX(reverse);
        }

        public string FlapY(int Id)
        {
            if (cameras.Count > Id)
                return cameras[Id].flapY ? "True" : "False";
            return "False";
        }

        public void FlapY(int Id, bool reverse)
        {
            if (cameras.Count > Id)
                cameras[Id].FlapY(reverse);
        }

        public void ManualClose(int Id)
        {
            if (Id >= cameras.Count) return;
            cameras[Id].Close();
        }

        public void ManualOpen(int Id)
        {
            if (Id >= cameras.Count) return;
            cameras[Id].Open();
        }
        public bool IsConnected(int Id) => cameras.Count > Id && cameras[Id].IsConnected();
    }

    public class HikrobotGrab
    {
        IDevice usbCamera = null;
        public Bitmap bitmap;
        public double exposureTime;
        public double gain;
        public bool flapX;
        public bool flapY;
        public bool grabCompleted = false;

        private Thread LiveThread = null;
        private bool LiveOn = false;
        public int grabId = -1;

        public static string targetname = "";
        public static string action = "";

        public delegate void GrabbedImageEvent(int grabId, Bitmap bitmap);
        public event GrabbedImageEvent m_cbGrabbedImageEventFunc;
        public delegate void GrabbedMessageEvent(int grabId, string msg);
        public event GrabbedMessageEvent m_cbGrabbedMessageEventFunc;

        public HikrobotGrab(IDeviceInfo info)
        {
            usbCamera = DeviceFactory.CreateDevice(info);
        }

        void FrameGrabedEventHandler(object sender, FrameGrabbedEventArgs e)
        {
            string msg;

            if(e.FrameOut.Image != null)
            {
                byte[] pixelData = (byte[])e.FrameOut.Image.PixelData;

                BitmapData bmpData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, bitmap.PixelFormat);
                IntPtr ptrBmp = bmpData.Scan0;
                int bpp = e.FrameOut.Image.PixelType == MvGvspPixelType.PixelType_Gvsp_Mono8 ? 8 : 24;
                Marshal.Copy(pixelData, 0, ptrBmp, bitmap.Width * bitmap.Height * bpp / 8);
                bitmap.UnlockBits(bmpData);

                if (m_cbGrabbedImageEventFunc != null)
                    m_cbGrabbedImageEventFunc(grabId, bitmap);
                grabCompleted = true;

            }
            else
            {
                msg = $"Error Code: {MvError.MV_E_HANDLE} Error Description: Error or Invalid handle";
                if (m_cbGrabbedMessageEventFunc != null)
                    m_cbGrabbedMessageEventFunc(grabId, msg);
            }
        }

        public void Open()
        {
            if (usbCamera == null)
                return;

            string msg;

            try
            {
                msg = $"Using camera {usbCamera.DeviceInfo.ModelName}.";
                
                DeviceTLayerType deviceType = usbCamera.DeviceInfo.TLayerType;

                int ret = usbCamera.Open();
                if (ret != MvError.MV_OK)
                    return;

                usbCamera.Parameters.SetEnumValueByString("AcquisitionMode", "Continuous");
                usbCamera.Parameters.SetEnumValueByString("TriggerMode", "On");
                usbCamera.Parameters.SetEnumValueByString("TriggerSource", "Software");

                grabId = Convert.ToInt32(usbCamera.DeviceInfo.UserDefinedName);

                usbCamera.StreamGrabber.FrameGrabedEvent += FrameGrabedEventHandler;

                IIntValue ivalue;
                IFloatValue fvalue;
                bool bvalue;
                IEnumValue evalue;

                ret = usbCamera.Parameters.GetIntValue("Width", out ivalue);
                if (ret != MvError.MV_OK)
                    return;
                int w = (int)ivalue.CurValue;

                ret = usbCamera.Parameters.GetIntValue("Height", out ivalue);
                if (ret != MvError.MV_OK)
                    return;
                int h = (int)ivalue.CurValue;

                ret = usbCamera.Parameters.GetFloatValue("ExposureTime", out fvalue);
                if (ret != MvError.MV_OK)
                    return;
                exposureTime = (double)fvalue.CurValue;

                ret = usbCamera.Parameters.GetFloatValue("Gain", out fvalue);
                if (ret != MvError.MV_OK)
                    return;
                gain = (double)fvalue.CurValue;

                ret = usbCamera.Parameters.GetBoolValue("ReverseX", out bvalue);
                if (ret != MvError.MV_OK)
                    return;
                flapX = bvalue;

                ret = usbCamera.Parameters.GetBoolValue("ReverseY", out bvalue);
                if (ret != MvError.MV_OK)
                    return;
                flapY = bvalue;

                ret = usbCamera.Parameters.GetEnumValue("PixelFormat", out evalue);
                if (ret != MvError.MV_OK)
                    return;
                string pixelFormat = evalue.CurEnumEntry.Symbolic;
                PixelFormat format = pixelFormat == "Mono8" ? PixelFormat.Format8bppIndexed : PixelFormat.Format24bppRgb;
                bitmap = new Bitmap(w, h, format);

                if (bitmap.PixelFormat == PixelFormat.Format8bppIndexed)
                {
                    ColorPalette colorPalette = bitmap.Palette;
                    for (int p = 0; p < 256; p++)
                    {
                        colorPalette.Entries[p] = Color.FromArgb(p, p, p);
                    }
                    bitmap.Palette = colorPalette;
                }

                ret = usbCamera.StreamGrabber.StartGrabbing(StreamGrabStrategy.OneByOne);
                if (ret != MvError.MV_OK)
                    return;
            }
            catch (Exception e)
            {
                Console.Error.WriteLine("\nException: {0}", e.Message);
            }
        }

        public bool IsConnected() => usbCamera != null && usbCamera.IsConnected;

        public void Close()
        {
            int ret = usbCamera.StreamGrabber.StopGrabbing();
            if (ret != MvError.MV_OK)
                return;

            usbCamera.Close();
        }

        public void ExposureTime(double time)
        {
            try
            {
                exposureTime = time;
                int ret = usbCamera.Parameters.SetEnumValue("ExposureAuto", 0);
                if (ret != MvError.MV_OK)
                    return;

                ret = usbCamera.Parameters.SetFloatValue("ExposureTime", (float)time);
                if (ret != MvError.MV_OK)
                    return;
            }
            catch
            {

            }
        }

        public void Gain(double value)
        {
            try
            {
                gain = value;
                int ret = usbCamera.Parameters.SetEnumValue("GainAuto", 0);
                if (ret != MvError.MV_OK)
                    return;

                ret = usbCamera.Parameters.SetFloatValue("Gain", (float)value);
                if (ret != MvError.MV_OK)
                    return;
            }
            catch
            {

            }
        }

        public void FlapX(bool Reverse)
        {
            try
            {
                int ret = usbCamera.Parameters.SetBoolValue("ReverseX", Reverse);
                if (ret != MvError.MV_OK)
                    return;
            }
            catch
            {

            }
        }

        public void FlapY(bool Reverse)
        {
            try
            {
                int ret = usbCamera.Parameters.SetBoolValue("ReverseY", Reverse);
                if (ret != MvError.MV_OK)
                    return;
            }
            catch
            {

            }
        }

        public void Grab()
        {
            LiveStop();

            grabCompleted = false;

            if (usbCamera.IsConnected && usbCamera != null)
            {
                int ret = usbCamera.Parameters.SetCommandValue("TriggerSoftware");
                if (ret != MvError.MV_OK)
                    return;
            }
        }

        public void LiveSart()
        {
            if (LiveOn)
                return;

            try
            {
                LiveOn = true;

                LiveThread = new Thread(new ParameterizedThreadStart(OnThreadLive));
                LiveThread.Priority = ThreadPriority.Normal;
                LiveThread.Start(new object[] { });

                foreach (ProcessThread proc in Process.GetCurrentProcess().Threads)
                    proc.ProcessorAffinity = Process.GetCurrentProcess().ProcessorAffinity;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void LiveStop()
        {
            LiveOn = false;
        }

        private void OnThreadLive(object pObject)
        {
            while(LiveOn)
            {
                if (usbCamera.IsConnected && usbCamera != null)
                {
                    if (!grabCompleted)
                        continue;

                    grabCompleted = false;

                    int ret = usbCamera.Parameters.SetCommandValue("TriggerSoftware");
                    if (ret != MvError.MV_OK)
                        return;
                }

                Thread.Sleep(1);
            }

            GC.Collect();
            LiveThread = null;
            LiveOn = false;
        }
    }
}
