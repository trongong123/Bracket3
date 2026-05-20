using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TopEng
{
    namespace Utils
    {
        public class StopWatch
        {
            public DateTime now = DateTime.Now;
            public bool run = true;
            public double laptime = 0;

            public StopWatch()
            {
                Start();
            }

            public void Start()
            {
                now = DateTime.Now;
                run = true;
                laptime = 0;
            }

            public void Reset()
            {
                now = DateTime.Now;
                run = true;
                laptime = 0;
            }

            public bool Run
            {
                get
                {
                    return run;
                }
                set
                {
                    if (run && !value)
                    {
                        TimeSpan timeSpan = DateTime.Now - now;
                        laptime = (double)timeSpan.TotalMilliseconds + (double)laptime;
                        run = false;
                    }
                    else if (!run && value)
                    {
                        now = DateTime.Now;
                        run = true;
                    }
                }
            }

            public int Elapsed
            {
                get
                {
                    TimeSpan timeSpan = DateTime.Now - now;
                    return (int)timeSpan.TotalMilliseconds + (int)laptime;
                }
            }
        }
    }
}
