namespace Polys.Util
    /** A generic pair class */
{
    public class Pair<First, Second>
    {
        public First first;
        public Second second;

        /** Trivial constructor */
        public Pair()
        {

        }

        /** Initialises the class with the given arguments */
        public Pair(First f, Second s)
        {
            first = f;
            second = s;
        }
    }
}
