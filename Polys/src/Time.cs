using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Polys
{
    class Time
    {
        static System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
        public static float deltaTime { get; private set; }
        public static long currentTime
        {
            get { return System.Diagnostics.Stopwatch.GetTimestamp();  }
        }

        public static void startFrame()
        {
            stopwatch.Start();
        }

        public static void endFrame()
        {
            stopwatch.Stop();
            deltaTime = stopwatch.ElapsedMilliseconds;
            stopwatch.Reset();
        }
    }
}
