using System;
using System.Collections.Generic;
using OpenGL;

namespace Polys.Util
{
    /*
    public class Quadtree
    {
        Node root;
        int maxObjects, maxLevels;
        int level;

        public Vector2 origin { get; private set; }

        public Quadtree(int maxObjects = 5, int maxLevels = 5)
        {
            root = new Node();
            this.maxObjects = maxObjects;
            this.maxLevels = maxLevels;
        }
        

        public Node closest(Video.Transformable t)
        {
            if (!root.hasChildren())
                return null;

            return root.find(x, y);
        }
    }

    public class Node
    {
        Video.Transformable obj;

        Node topRight, topLeft, bottomLeft, bottomRight;

        public Node(bool valid = false) {}

 
        public bool hasChildren()
        {
            return topLeft != null || bottomLeft != null || topRight != null || bottomRight != null;
        }
    }
    */
}
