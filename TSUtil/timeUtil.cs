using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace TopEng.Utils
{
    public class timeUtil
    {
        public static void Delay(int delayTime)
        {
            StopWatch stopwatch = new StopWatch();
            stopwatch.Reset();

            while (delayTime > stopwatch.Elapsed)
            {
                System.Windows.Forms.Application.DoEvents();
                Thread.Sleep(1);
            }
        }
    }
}
