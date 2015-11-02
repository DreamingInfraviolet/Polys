using System;
using System.Collections.Generic;
using OpenGL;

namespace Polys.Util
{
    /*
    public class Quadtree<T> where T : new()
    {
        public class Node
        {
            public T posX, posY;
            Node topLeft, bottomLeft, topRight, bottomRight;
            bool valid;

            public Node(bool valid = false) { posX = posY = new T(); this.valid = valid; }

            public Node(T posX, T posY, bool valid)
            {
                this.posX = posX;
                this.posY = posY;
                this.valid = valid;
            }

            public Node find(T x, T y)
            {
                if(x < posX)
                {

                }
                else if (x> posX)
                {

                }
                else
                {

                }
            }

            public bool hasChildren()
            {
                return topLeft != null || bottomLeft != null || topRight != null || bottomRight != null;
            }
        }

        Node root;

        public Vector2 origin { get; private set; }

        public Quadtree()
        {
            root = new Node();
        }

        public Quadtree(T posX, T posY)
        {
            root = new Node(posX, posY, false);
        }

        public Node closest(T x, T y)
        {
            if (!root.hasChildren())
                return null;

            return root.find(x, y);
        }
    }
    */
}
