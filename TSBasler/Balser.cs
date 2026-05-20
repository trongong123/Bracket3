using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Basler.Pylon;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Threading;
using System.Diagnostics;

namespace TopEng.Vision.Camera
{
    public class BaslerControl
    {
        public Dictionary<int, BaslerGrab> cameras = new Dictionary<int, BaslerGrab>();

        public BaslerControl()
        {

        }

        public void Open()
        {
            try
            {
                foreach (var cam in cameras)
                    cam.Value.Close();
                cameras.Clear();

                // Ask the camera finder for a list of all GigE camera devices.
                // Note that this sample only works with GigE camera devices.
                List<ICameraInfo> allDeviceInfos = CameraFinder.Enumerate(DeviceType.Usb);

                if (allDeviceInfos.Count == 0)
                {
                    throw new ApplicationException("No USB cameras present.");
                }

                // Open all cameras to fulfill preconditions for Configure(ICamera())
                foreach (var info in allDeviceInfos)
                {
                    BaslerGrab grab = new BaslerGrab(info);
                    grab.Open();
                    if (grab.grabId >= 0)
                        cameras.Add(grab.grabId, grab);
                }
            }
            catch (Exception e)
            {
                throw e;;
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

    public class BaslerGrab
    {
        const UInt32 countOfImagesToGrab = 100;
        Basler.Pylon.Camera usbCamera = null;
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

        public BaslerGrab(ICameraInfo info)
        {
            usbCamera = new Basler.Pylon.Camera(info);
        }

        // OnImageGrabbed is used to print the image information like Width, Height etc.
        // Can be used to implement other functionality for image grab event.
        void OnImageGrabbed(Object sender, ImageGrabbedEventArgs e)
        {
            string msg;

            if (e.GrabResult.GrabSucceeded)
            {
                //msg = $"SizeX:{e.GrabResult.Width}, SizeY:{e.GrabResult.Height}";
                //LogUtil.Instance.Log(LogUtil.LOG_TYPE.DEVELOPMENT, msg, LogUtil.CONTENT_TYPE.INFO);
                byte[] pixelData = (byte[])e.GrabResult.PixelData;
                //msg = $"Gray value of first pixel:{pixelData[0]}";
                //LogUtil.Instance.Log(LogUtil.LOG_TYPE.DEVELOPMENT, msg, LogUtil.CONTENT_TYPE.INFO);

                BitmapData bmpData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, bitmap.PixelFormat);
                IntPtr ptrBmp = bmpData.Scan0;
                int bpp = e.GrabResult.PixelTypeValue == PixelType.BGR8packed ? 24 : 8;
                Marshal.Copy(pixelData, 0, ptrBmp, bitmap.Width * bitmap.Height * bpp / 8);
                bitmap.UnlockBits(bmpData);

                if (m_cbGrabbedImageEventFunc != null)
                    m_cbGrabbedImageEventFunc(grabId, bitmap);

                grabCompleted = true;
            }
            else
            {
                msg = $"Error Code: {e.GrabResult.ErrorCode} Error Description: {e.GrabResult.ErrorDescription}";
                if (m_cbGrabbedMessageEventFunc != null)
                    m_cbGrabbedMessageEventFunc(grabId, msg);
            }

            grabCompleted = true;
        }

        // This method is called if one or more images have been skipped during
        // image acquisition.
        void OnImageSkipped(Object sender, ImageGrabbedEventArgs e)
        {
            string msg = $"[OnImageSkipped Event] Number Of skipped images {e.GrabResult.SkippedImageCount}";
            if (m_cbGrabbedMessageEventFunc != null)
                m_cbGrabbedMessageEventFunc(grabId, msg);
        }

        public void Open()
        {
            if (usbCamera == null)
                return;

            // The exit code of the sample application.
            //int exitCode = 0;
            string msg;

            try
            {
                // Create a camera object that selects the first camera device found.
                // More constructors are available for selecting a specific camera device.
                // For multicast only look for GigE cameras here.
                // Print the model name of the camera.
                msg = $"Using camera {usbCamera.CameraInfo[CameraInfoKey.ModelName]}.";
                //LogUtil.Instance.Log(LogUtil.LOG_TYPE.DEVELOPMENT, msg, LogUtil.CONTENT_TYPE.INFO);
                String deviceType = usbCamera.CameraInfo[CameraInfoKey.DeviceType];

                //LogUtil.Instance.Log(LogUtil.LOG_TYPE.DEVELOPMENT, "==========", LogUtil.CONTENT_TYPE.INFO);
                //LogUtil.Instance.Log(LogUtil.LOG_TYPE.DEVELOPMENT, $"{deviceType} Camera", LogUtil.CONTENT_TYPE.INFO);
                //LogUtil.Instance.Log(LogUtil.LOG_TYPE.DEVELOPMENT, "==========", LogUtil.CONTENT_TYPE.INFO);

                // Set the acquisition mode to software triggered continuous acquisition when the camera is opened.
                usbCamera.CameraOpened += Configuration.SoftwareTrigger;

                // CONTROL MODE
                //LogUtil.Instance.Log(LogUtil.LOG_TYPE.DEVELOPMENT, "Control mode", LogUtil.CONTENT_TYPE.INFO);
                usbCamera.Open(); // Open the camera.

                //usbCamera.Parameters[PLUsbCamera.PixelFormat].SetValue(PLUsbCamera.PixelFormat.Mono8);
                grabId = Convert.ToInt32(usbCamera.Parameters[PLCamera.DeviceUserID].GetValue());

                //Check if camera supports waiting for trigger ready
                if (usbCamera.CanWaitForFrameTriggerReady)
                {
                    usbCamera.StreamGrabber.ImageGrabbed += OnImageGrabbed;
                    usbCamera.StreamGrabber.ImageGrabbed += OnImageSkipped;

                    // Start grabbing using the grab loop thread. This is done by setting the grabLoopType parameter
                    // to GrabLoop.ProvidedByStreamGrabber. The grab results are delivered to the image event handler OnImageGrabbed.
                    // The default grab strategy (GrabStrategy_OneByOne) is used.
                    usbCamera.StreamGrabber.Start(GrabStrategy.OneByOne, GrabLoop.ProvidedByStreamGrabber);
                }

                // IMAGE BUFFER
                long w = usbCamera.Parameters[PLCamera.Width].GetValue();
                long h = usbCamera.Parameters[PLCamera.Height].GetValue();
                exposureTime = usbCamera.Parameters[PLCamera.ExposureTime].GetValue();
                gain = usbCamera.Parameters[PLCamera.Gain].GetValue();
                flapX = usbCamera.Parameters[PLCamera.ReverseX].GetValue();
                flapY = usbCamera.Parameters[PLCamera.ReverseY].GetValue();
                string pixelFormat = usbCamera.Parameters[PLCamera.PixelFormat].GetValue();
                PixelFormat format = pixelFormat == "BGR8" ? PixelFormat.Format24bppRgb : PixelFormat.Format8bppIndexed;
                bitmap = new Bitmap((int)w, (int)h, format);

                if (bitmap.PixelFormat == PixelFormat.Format8bppIndexed)
                {
                    ColorPalette colorPalette = bitmap.Palette;
                    for (int p = 0; p < 256; p++)
                    {
                        colorPalette.Entries[p] = Color.FromArgb(p, p, p);
                    }
                    bitmap.Palette = colorPalette;
                }

                usbCamera.StreamGrabber.Start();
            }
            catch (Exception e)
            {
                // Error handling
                Console.Error.WriteLine("\nException: {0}", e.Message);
                //exitCode = 1;
            }
        }

        public bool IsConnected() => usbCamera != null && usbCamera.IsOpen && usbCamera.IsConnected;

        public void Close()
        {
            usbCamera.StreamGrabber.Stop();
            usbCamera.Close();
        }

        public void ExposureTime(double time)
        {
            try
            {
                exposureTime = time;
                usbCamera.Parameters[PLCamera.ExposureTime].SetValue(time);
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
                usbCamera.Parameters[PLCamera.Gain].SetValue(value);
            }
            catch
            {

            }
        }

        public void FlapX(bool Reverse)
        {
            try
            {
                usbCamera.Parameters[PLCamera.ReverseX].SetValue(Reverse);
            }
            catch
            {

            }
        }

        public void FlapY(bool Reverse)
        {
            try
            {
                usbCamera.Parameters[PLCamera.ReverseY].SetValue(Reverse);
            }
            catch
            {

            }
        }

        public void Grab()
        {
            LiveStop();

            grabCompleted = false;

            if (usbCamera.IsOpen && usbCamera.IsConnected)
            {
                // Execute the software trigger. Wait up to 1000 ms until the camera is ready for trigger.
                if (usbCamera.WaitForFrameTriggerReady(100, TimeoutHandling.ThrowException))
                {
                    usbCamera.ExecuteSoftwareTrigger();
                }
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
                throw e;;
            }
        }

        public void LiveStop()
        {
            LiveOn = false;
        }

        private void OnThreadLive(object pObject)
        {
            while (LiveOn)
            {
                if (usbCamera.IsOpen && usbCamera.IsConnected)
                {
                    if (!grabCompleted)
                        continue;

                    grabCompleted = false;
                    // Execute the software trigger. Wait up to 1000 ms until the camera is ready for trigger.
                    if (usbCamera.WaitForFrameTriggerReady(1000, TimeoutHandling.ThrowException))
                    {
                        usbCamera.ExecuteSoftwareTrigger();
                    }
                }

                Thread.Sleep(1);
            }

            GC.Collect();
            LiveThread = null;
            LiveOn = false;
        }
    }
}
