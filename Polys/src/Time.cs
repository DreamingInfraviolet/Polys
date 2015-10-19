using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Polys
{
    /** A class to help handle time, providing functions to get the current time as well as the delta time. */
    class Time
    {
        static System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();

        /** The time (in milliseconds) that the last frame took to complete. */
        public static float deltaTime { get; private set; }

        /** The current time. */
        public static long currentTime
        {
            get { return System.Diagnostics.Stopwatch.GetTimestamp();  }
        }

        /** Marks the beginning of a frame. */
        public static void startFrame()
        {
            stopwatch.Start();
        }

        /** Marks the end of a frame, computing detlaTime. */
        public static void endFrame()
        {
            stopwatch.Stop();
            deltaTime = stopwatch.ElapsedMilliseconds;
            stopwatch.Reset();
        }
    }
}
