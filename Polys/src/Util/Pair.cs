namespace Polys.Util
    /** A generic pair class */
{
    public struct Pair<First, Second>
    {
        public First first;
        public Second second;

        /** Initialises the class with the given arguments */
        public Pair(First f, Second s)
        {
            first = f;
            second = s;
        }
    }
}
