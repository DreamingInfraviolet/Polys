using System;

namespace Polys.Util
{
    class DoubleBuffer<T>
    {
        public T back, front;

        public void flip()
        {
            var tmp = front;
            back = front;
            front = tmp;
        }

        public DoubleBuffer(T front, T back)
        {
            this.front = front;
            this.back = back;
        }
    }
}
