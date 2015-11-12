namespace Polys.Game.States
{
    /** Represents any object that can represent a state */
    public interface State : System.IDisposable
    {
        /** Execute a frame of the state
          * @return Whether the game should stop. */
        StateManager.StateUpdateResult updateBeforeInput();

        StateManager.StateUpdateResult updateAfterInput();

        StateManager.StateUpdateResult updateAfterFrame();

        StateManager.StateRenderResult draw();

        void setStateManager(StateManager m);
    }
}
