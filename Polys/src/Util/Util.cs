using System;

namespace Polys.Util
{
    /** A miscallaneous utility class. */
    class Util
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
            {
                throw new ArgumentNullException("array");
            }

            int index = 0;
            int length = Math.Min(block, array.Length);

            //Fill the initial array
            while (index < length)
            {
                array[index++] = value;
            }

            length = array.Length;
            while (index < length)
            {
                Buffer.BlockCopy(array, 0, array, index, Math.Min(block, length - index));
                index += block;
                block *= 2;
            }
        }
    }
}
