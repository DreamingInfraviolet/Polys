namespace Polys.Game
{
    /** Represents any object that can represent a state */
    abstract class State
    {
        //StateManager parent;

        //public State(StateManager parent)
        //{
        //    this.parent = parent;
        //}

        /** Execute a frame of the state */
        public virtual void execute()
        {

        }
    }
}
