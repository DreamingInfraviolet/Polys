using System;
using System.Collections.Generic;
using OpenGL;

namespace Polys.Util
{
    public class Quadtree
    {
        Node root;
        int maxObjects;
        int level;

        public Vector2 origin { get; private set; }

        public Quadtree(Rect rect, int maxObjects = 15)
        {
            if ((rect.w & (rect.w - 1)) == 0 || (rect.h & (rect.h - 1)) == 0 || rect.w == 0 || rect.h == 0)
                throw new Exception("Attempting to create a quadtree with non power-of-two size.");

            root = new Node(rect, maxObjects);
            this.maxObjects = maxObjects;
        }

        public void clear()
        {
            root.clear();
        }

        public void insert(Video.Transformable t)
        {

        }
    }

    public class Node
    {
        Video.Transformable[] objects;

        Node topRight, topLeft, bottomLeft, bottomRight;
        Rect rect;

        int objectCount = 0;
        bool hasChildren = false;

        public Node(Rect r, int maxObjects)
        {
            rect = r;
            objects = new Video.Transformable[maxObjects];
        }

        public void insert(Video.Transformable t)
        {
            //If the object is bigger than half the size of the node, insert it into this node.
            //Else, insert it into a subnode.
        }

        public void split()
        {
            if (!hasChildren)
            {
                if (rect.w == 1 || rect.h == 1)
                    throw new Exception("Attempting to split a quadtree of size 1.");

                int halfW = rect.w / 2;
                int halfH = rect.h / 2;

                topRight = new Node(new Rect(rect.x+ halfW, rect.y, halfW, halfH), objects.Length);
                topLeft = new Node(new Rect(rect.x, rect.y, halfW, halfH), objects.Length);
                bottomLeft = new Node(new Rect(rect.x, rect.y + halfH, halfW, halfH), objects.Length);
                bottomRight = new Node(new Rect(rect.x+ halfW, rect.h+halfH, halfW, halfH), objects.Length);
                hasChildren = true;
            }
        }

        public void clear()
        {
            if (hasChildren)
            {
                topRight.clear();
                topLeft.clear();
                bottomLeft.clear();
                bottomRight.clear();

                topRight = topLeft = bottomLeft = bottomRight = null;

                hasChildren = false;
            }

            for (int i = 0; i < objects.Length; ++i)
                objects[i] = null;

            objectCount = 0;
        }
    }
}
