using System;

namespace Polys
{
    /** A class to help handle time, providing functions to get the current time as well as the delta time. */
    public class Time
    {
        static System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();

        static float[] deltaTimeHistory = new float[5];

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

            //Push delta time history down:
            for (int i = 0; i < deltaTimeHistory.Length-1; ++i)
                deltaTimeHistory[i] = deltaTimeHistory[i+1];

            //Add new dt to history
            deltaTimeHistory[deltaTimeHistory.Length - 1] = (float)(stopwatch.ElapsedTicks / (double)System.Diagnostics.Stopwatch.Frequency);

            //Get mode thingy
            deltaTime = Util.Util.findClosestToAllOthers(deltaTimeHistory);

            stopwatch.Reset();
        }

    }
}
