using System;

namespace Polys.Game.States
{
    public class StateManager : System.IDisposable
    {
        public enum UpdateType
        {
            BeforeInput,
            AfterInput,
            AfterFrame
        }

        public enum StateUpdateResult
        {
            Finish,
            Quit,
            UpdateBelow
        }

        public enum StateRenderResult
        {
            Continue,
            StopDrawing
        }

        System.Collections.Generic.List<State> stack = new System.Collections.Generic.List<State>();
        
        public StateManager(State start)
        {
            push(start);
        }

        public State top
        {
            get
            {
                if (stack.Count == 0)
                    return null;
                else
                    return stack[stack.Count - 1];
            }
        }

        public void push(State state)
        {
            stack.Add(state);
        }

        public State pop()
        {
            State popped = top;
            stack.RemoveAt(stack.Count - 1);
            return popped;

        }

        /** Executes the state stack.
          * @return True if the state manager wants to continue to run the game, or false to quit. */
        public bool update(UpdateType type)
        {
            if (stack.Count == 0)
                return true;
            else
            {
                for (int i = stack.Count - 1; i > -1; --i)
                {
                    StateUpdateResult result;

                    switch (type)
                    {
                        case StateManager.UpdateType.BeforeInput:
                            result = stack[i].updateBeforeInput();
                            break;
                        case StateManager.UpdateType.AfterInput:
                            result = stack[i].updateAfterInput();
                            break;
                        case StateManager.UpdateType.AfterFrame:
                            result = stack[i].updateAfterFrame();
                            break;
                        default:
                            throw new System.NotImplementedException();
                    }
                    
                    switch (result)
                    {
                        case StateUpdateResult.Finish:
                            return true;
                        case StateUpdateResult.Quit:
                            return false;
                        case StateUpdateResult.UpdateBelow:
                            if (i == 0)
                            {
                                System.Console.WriteLine("Warning: attempting to update non-existent state - ignoring.");
                                return true;
                            }
                            break;
                        default:
                            throw new System.NotImplementedException();
                    }
                }
                throw new System.Exception("SYSTEM LOGIC ERROR IN STATE LOOP");
            }
        }

        /** Differs from Update by drawing things at the bottom first, climbing up the stack if needed. */
        public void draw()
        {
            if (stack.Count == 0)
                return;
            else
            {
                for (int i = 0; i < stack.Count; ++i)
                {
                    StateRenderResult result = stack[i].draw();
                    switch (result)
                    {
                        case StateRenderResult.Continue:
                            break;
                        case StateRenderResult.StopDrawing:
                            return;
                        default:
                            throw new System.NotImplementedException();
                    }
                }
            }
        }

        public void Dispose()
        {
            foreach(State state in stack)
            {
                state.Dispose();
            }
        }
    }
}
