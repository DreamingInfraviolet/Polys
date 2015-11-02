using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Polys.Game.States;
using Polys.Util;

namespace Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestUtils()
        {
            //Clamp
            Assert.AreEqual(Util.Clamp(-1, -1, -1), -1);
            Assert.AreEqual(Util.Clamp(-2, -1, 2), -1);
            Assert.AreEqual(Util.Clamp(4, -4, -2), -2);
            Assert.AreEqual(Util.Clamp(1, -1, 2), 1);

            //Memset
            byte[] arr1 = new byte[123];
            byte[] arr2 = new byte[1];
            byte[] arr3 = null;

            Util.MemSet(arr1, 3);
            Util.MemSet(arr2, 3);
            Util.MemSet(arr3, 3);

            foreach (byte b in arr1)
                Assert.AreEqual(b, 3);
            foreach (byte b in arr2)
                Assert.AreEqual(b, 3);

            //Is Rect Visible
            Assert.AreEqual(Maths.isRectVisible(0, 0, 0, 0, 100, 100), true);
            Assert.AreEqual(Maths.isRectVisible(99, 99, 2333, 400, 100, 100), true);
            Assert.AreEqual(Maths.isRectVisible(-2, -3, 1, 1, 100, 100), false);
            Assert.AreEqual(Maths.isRectVisible(0, 0, 1, 1, 1, 1), true);
            Assert.AreEqual(Maths.isRectVisible(200, 330, 1, 5, 100, 100), false);

            //Fit rectangle inside rectangle projection
            OpenGL.Vector4 bottomLeftVec = new OpenGL.Vector4(-1, -1, 0, 1);
            OpenGL.Vector4 projected = bottomLeftVec * Maths.matrixFitRectIntoScreen(100, 200, 50, 60);
            Assert.IsTrue(bottomLeftVec.x >= -1 && bottomLeftVec.x <= 1 && bottomLeftVec.y >= -1 && bottomLeftVec.y <= 1);
            projected = bottomLeftVec * Maths.matrixFitRectIntoScreen(100, 200, 50, 60);
            Assert.IsTrue(bottomLeftVec.x >= -1 && bottomLeftVec.x <= 1 && bottomLeftVec.y >= -1 && bottomLeftVec.y <= 1);
            projected = bottomLeftVec * Maths.matrixFitRectIntoScreen(200, 65, 50, 60);
            Assert.IsTrue(bottomLeftVec.x >= -1 && bottomLeftVec.x <= 1 && bottomLeftVec.y >= -1 && bottomLeftVec.y <= 1);
            projected = bottomLeftVec * Maths.matrixFitRectIntoScreen(100, 200, 80, 20);
            Assert.IsTrue(bottomLeftVec.x >= -1 && bottomLeftVec.x <= 1 && bottomLeftVec.y >= -1 && bottomLeftVec.y <= 1);
            projected = bottomLeftVec * Maths.matrixFitRectIntoScreen(500, 200, 70, 2);
            Assert.IsTrue(bottomLeftVec.x >= -1 && bottomLeftVec.x <= 1 && bottomLeftVec.y >= -1 && bottomLeftVec.y <= 1);
            projected = bottomLeftVec * Maths.matrixFitRectIntoScreen(123, 123, 123, 123);
            Assert.IsTrue(bottomLeftVec.x >= -1 && bottomLeftVec.x <= 1 && bottomLeftVec.y >= -1 && bottomLeftVec.y <= 1);
            projected = bottomLeftVec * Maths.matrixFitRectIntoScreen(35, 2, 1, 60);
            Assert.IsTrue(bottomLeftVec.x >= -1 && bottomLeftVec.x <= 1 && bottomLeftVec.y >= -1 && bottomLeftVec.y <= 1);
        }

        //A state for testing purposes.
        class TestState : Polys.Game.States.State
        {
            public static List<int> stateUpdateOrder = new List<int>();
            public static List<int> stateDrawOrder = new List<int>();

            public int value;

            public TestState(int v) { value = v; }

            public void Dispose() { }

            public StateManager.StateRenderResult draw()
            {
                stateDrawOrder.Add(value);
                if (value > 5)
                    return StateManager.StateRenderResult.StopDrawing;
                else
                    return StateManager.StateRenderResult.Continue;
            }

            public StateManager.StateUpdateResult updateAfterFrame()
            {
                stateUpdateOrder.Add(value);
                return StateManager.StateUpdateResult.UpdateBelow;
            }

            public StateManager.StateUpdateResult updateAfterInput()
            {
                stateUpdateOrder.Add(value);
                if (value > 2)
                    return StateManager.StateUpdateResult.UpdateBelow;
                else if (value > 1)
                    return StateManager.StateUpdateResult.Finish;
                else
                    return StateManager.StateUpdateResult.Quit;
            }

            public StateManager.StateUpdateResult updateBeforeInput()
            {
                stateUpdateOrder.Add(value);
                if (value > 4)
                    return StateManager.StateUpdateResult.UpdateBelow;
                else if (value < 2)
                    return StateManager.StateUpdateResult.Finish;
                else
                    return StateManager.StateUpdateResult.Quit;
            }
        }

        [TestMethod]
        public void TestStates()
        {
            StateManager manager = new StateManager(new TestState(0));
            manager.push(new TestState(1));
            Assert.AreEqual(((TestState)manager.pop()).value, 1);
            Assert.AreEqual(((TestState)manager.top).value, 0);
            manager.push(new TestState(1));
            manager.push(new TestState(2));
            manager.push(new TestState(3));
            manager.push(new TestState(4));
            manager.push(new TestState(5));
            manager.push(new TestState(6));
            manager.push(new TestState(7));

            Assert.AreEqual(manager.update(StateManager.UpdateType.BeforeInput), false);
            Assert.IsTrue(Enumerable.SequenceEqual(TestState.stateUpdateOrder, new List<int>() { 7, 6, 5, 4 }));
            TestState.stateUpdateOrder = new List<int>();

            Assert.AreEqual(manager.update(StateManager.UpdateType.AfterInput), true);
            Assert.IsTrue(Enumerable.SequenceEqual(TestState.stateUpdateOrder, new List<int>() { 7, 6, 5, 4, 3, 2 }));
            TestState.stateUpdateOrder = new List<int>();

            Assert.AreEqual(manager.update(StateManager.UpdateType.AfterFrame), true);
            Assert.IsTrue(Enumerable.SequenceEqual(TestState.stateUpdateOrder, new List<int>() { 7, 6, 5, 4, 3, 2, 1, 0 }));
            TestState.stateUpdateOrder = new List<int>();

            manager.draw();
            Assert.IsTrue(Enumerable.SequenceEqual(TestState.stateDrawOrder, new List<int>() { 0, 1, 2, 3, 4, 5, 6 }));
            TestState.stateDrawOrder = new List<int>();

            manager.pop();
            manager.pop();
            manager.pop();
            manager.draw();
            Assert.IsTrue(Enumerable.SequenceEqual(TestState.stateDrawOrder, new List<int>() { 0, 1, 2, 3, 4 }));
            TestState.stateDrawOrder = new List<int>();
        }
    }
}
