using System;
using System.Collections.Generic;

namespace Polys.Util
{
    /** A miscallaneous utility class. */
    public class Util
    {
        /** Clamps a number into a specific range. */
        public static T Clamp<T>(T x, T min, T max) where T : IComparable<T>
        {
            if (x.CompareTo(min) < 0) return min;
            else if (x.CompareTo(max) > 0) return max;
            else return x;
        }

        /** Fast memset algorithm. */
        public static void MemSet(byte[] array, byte value, int block = 32)
        {
            if (array == null)
                return;

            int index = 0;
            int length = Math.Min(block, array.Length);

            //Fill the initial array
            while (index < length)
            {
                array[index] = value;
                ++index;
            }

            length = array.Length;
            while (index < length)
            {
                Buffer.BlockCopy(array, 0, array, index, Math.Min(block, length - index));
                index += block;
                block *= 2;
            }
        }

        public static void CheckGl()
        {
            OpenGL.ErrorCode err = OpenGL.Gl.GetError();
            if (err != OpenGL.ErrorCode.NoError)
                Console.WriteLine("Open GL Error: " + err);
        }

        public static void insertionSort<T,S>(List<T> objects) where T :IComparable<S>, S
        {
            for (int iOuterPos = 1; iOuterPos < objects.Count; ++iOuterPos)
            {
                T tmp = objects[iOuterPos];

                int iInnerPos = iOuterPos - 1;
                for (; iInnerPos > -1 && objects[iInnerPos].CompareTo(tmp) > 0; --iInnerPos)
                    objects[iInnerPos + 1] = objects[iInnerPos];

                objects[iInnerPos + 1] = tmp;
            }
        }

        //Returns the value most like the others. Use dumb algorithm for now.
        public static float findClosestToAllOthers(float[] x)
        {
            if (x.Length == 1)
                return x[0];
            else if (x.Length == 0)
                return 0;

            float[] distances = new float[x.Length];
            for (int i = 0; i < x.Length; ++i)
                for (int j = 0; j < x.Length; ++j)
                    distances[i] += Math.Abs(x[j] - x[i]);

            float min = distances[0];
            int minIndex = 0;
            for(int i = 1; i < distances.Length; ++i)
                if(distances[i]< min)
                {
                    min = distances[i];
                    minIndex = i;
                }

            return x[minIndex];
        }

        /*   Same as above, but not tested.     public static void insertionSort<T>(T[] objects) where T :IComparable
        {
            for (int iOuterPos = 1; iOuterPos < objects.Length; ++iOuterPos)
            {
                T tmp = objects[iOuterPos];

                int iInnerPos = iOuterPos - 1;
                for (; iInnerPos > -1 && objects[iInnerPos].CompareTo(tmp) > 0; --iInnerPos)
                    objects[iInnerPos + 1] = objects[iInnerPos];

                objects[iInnerPos + 1] = tmp;
            }
        }*/
    }
}
