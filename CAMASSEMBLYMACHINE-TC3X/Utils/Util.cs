using CAMASSEMBLYMACHINE;
using CAMASSEMBLYMACHINE.Define;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static CAMASSEMBLYMACHINE.Define.ParameterDefine;
using static CAMASSEMBLYMACHINE.Define.SystemDefine;

namespace TopEng.Utils
{
    /**
     * @brief Util을 위해 제공하는 클래스
     */
    public static class Util
    {
        public static bool IsAxisInPos(params (AXIS axis, Enum pos)[] items)
        {
            try
            {
                double actualPos = 0;
                foreach (var (axis, posEnum) in items)
                {
                    Machine.motion.GetAxisActualPos((int)axis, ref actualPos);

                    double paramPos =
                        posEnum is CALIBRATION c ? 1000 * Machine.param.Calibration(c) :
                        posEnum is ParameterDefine.POSITION p ? 1000 * Machine.param.Position(p) :
                        posEnum is RecipeDefine.POSITION p2 ? 1000 * Machine.recipe.Position(p2) :
                        throw new InvalidOperationException();

                    if (Math.Abs(paramPos - actualPos) > 5)
                        return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static bool IsAxisInPos(params (AXIS axis, double param)[] items)
        {
            try
            {
                double actualPos = 0;
                foreach (var (axis, paramPos) in items)
                {
                    Machine.motion.GetAxisActualPos((int)axis, ref actualPos);

                    if (Math.Abs(paramPos - actualPos) > 5)
                        return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /**
         * @brief 동작중에 잠깐의 딜레이를 추가하기 위한 함수 
         * @param[in] delayTime (ms)
         */
        public static void Delay(int delayTime)
        {
            Stopwatch stopwatch = new Stopwatch();
            long interval = 0;

            stopwatch.Restart();

            while (delayTime > interval)
            {
                System.Windows.Forms.Application.DoEvents();
                Thread.Sleep(1);
                interval = stopwatch.ElapsedMilliseconds;
            }

            stopwatch.Reset();
            return;
        }

        /**
         * @brief 열려 있는 Form 찾는 함수
         * @param[in] formNameArray 찾고자 하는 폼 이름들 (array)
         * @return 찾고하는 이름의 Form이 떠 있는지 여부
         */
        public static bool IsOpenForm(params string[] formNameArray)
        {
            bool ret = false;

            if (formNameArray.Length > 0)
            {
                foreach (string formName in formNameArray)
                {
                    if (Application.OpenForms[formName] != null)
                    {
                        ret = true;
                        break;
                    }
                }
            }
            return ret;
        }

        /**
         * @brief Process alarm 이 떠 있는지 확인하는 함수
         * @return Process alarm 이 떠있는지 여부
         */
        public static bool IsProcessAlarmOccur(int errCode)
        {
            return ((errCode != SystemDefine.ERRCODE_NO_ERROR) && (errCode != SystemDefine.ERRCODE_STOP_YELLOW));
        }

        /**
         * @brief UI 버벅임을 줄이고자 Double buffering 수행하는 함수
         * @param[in] control 버퍼링 적용 대상
         * @param[in] enabled 버퍼링 적용 여부 true / false
         */
        public static void DoubleBuffered(this Control control, bool enabled)
        {
            var prop = control.GetType().GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            prop.SetValue(control, enabled, null);
        }

        /**
         * @brief Array 타입의 인자 중 최대값 반환하는 함수
         * @param[in] arr Array 타입
         * @return 최대값 
         */
        public static object Max(Array arr)
        {
            object ret = -1;
            for (int i = 0; i < arr.Length; i++)
            {
                if ((int)ret < (int)arr.GetValue(i))
                    ret = arr.GetValue(i);
            }

            return ret;
        }

        internal static string GetLocalIP()
        {
            string localIP;
            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    localIP = ip.ToString();
                    if (localIP.Contains("192.168.0.100"))
                    {
                        return localIP;
                    }
                }
            }
            throw new NotImplementedException();
        }
    }
}
