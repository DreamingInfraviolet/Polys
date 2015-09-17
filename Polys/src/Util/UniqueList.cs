using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Polys.Util
{
    /** Allows registering and deregistering of an object in such a
        way as to help avoid duplication of resources. */
        
    class UniqueList<First, Second>
    {
        static Dictionary<First, Pair<int, Second> > mList = new Dictionary<First, Pair<int, Second>>();

        //Must create a Second object given First as parameter.
        public delegate Second Creator(First x);

        /**If the object already exists as determined by the parameter, return it.
          * otherwise, create it using the creator and return. */
        public Second register(First fst, Creator creator)
        {
            Pair<int, Second> correspondingList;
            Second snd;
            
            //if key exists:
            if (mList.TryGetValue(fst, out correspondingList))
            {
                //Increment count
                correspondingList.first++;

                //Reference that element
                snd = correspondingList.second;
            }
            else //if key does not exist
            {
                //Create the resource
                snd = creator(fst);
                mList.Add(fst, new Pair<int, Second>(1, snd));
            }
            return snd;
        }

        /** Deregisters the object, freeing memory if needed */
        public void deregister(First fst)
        {
            //if it exists,
            Pair<int, Second> correspondingList;
            if(mList.TryGetValue(fst, out correspondingList))
            {
                //Reduce the counter
                correspondingList.first--;
                //remove if none left
                if (correspondingList.first == 0)
                    mList.Remove(fst);
            }
        }
    }
}
