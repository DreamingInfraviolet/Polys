namespace Polys.Game
{
    /** Represents any object that can represent a state */
    interface State
    {
        /** Execute a frame of the state
          * @return Whether the game should stop. */
        StateManager.StateUpdateResult update(float dt);

        StateManager.StateRenderResult draw();
    }
}
