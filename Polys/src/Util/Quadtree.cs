using System;
using System.Collections.Generic;
using OpenGL;

namespace Polys.Util
{
    public class Quadtree
    {
        Node root;
        int maxObjects;

        public Vector2 origin { get; private set; }

        public Quadtree(Rect rect)
        {
            if ((rect.w & (rect.w - 1)) == 0 || (rect.h & (rect.h - 1)) == 0 || rect.w == 0 || rect.h == 0)
                throw new Exception("Attempting to create a quadtree with non power-of-two size.");

            root = new Node(rect);
        }

        public void clear()
        {
            root.clear();
        }

        public void insert(Video.Transformable t)
        {
            root.insert(t);
        }

        public List<Video.Transformable> findIntersecting(Rect rect)
        {
            List<Video.Transformable> answer = new List<Video.Transformable>();
            root.findIntersecting(rect, answer);
            return answer;
        }


    }

    public class Node
    {
        List<Video.Transformable> objects = new List<Video.Transformable>();

        Node topRight, topLeft, bottomLeft, bottomRight;
        Rect rect;
        
        bool hasChildren = false;

        public Node(Rect r)
        {
            rect = r;
        }

        public void insert(Video.Transformable t)
        {
            //If the object is bigger than half the size of the node, insert it into this node.
            //Else, insert it into a subnode.
            int halfW = rect.w/2;
            int halfH = rect.h/2;

            if (t.rect.w > halfW || t.rect.h > halfH || halfW == 1 || halfH == 1)
                objects.Add(t);
            else
            {
                //There is a chance that it will fit neatly into one of the subchildren. If so, split and insert.
                Node n = splitAndFindChildWithPerfectFit(t.rect, halfW, halfH);
                if (n != null)
                    n.insert(t);
                else //If does not fit neatly into subquad
                    objects.Add(t);
            }
        }

        private Node splitAndFindChildWithPerfectFit(Rect r, int halfNodeWidth, int halfNodeHeight)
        {
            bool b1, b2, b3, b4;
            if ((b1 = (r.right < r.x + halfNodeWidth)) ^ (b2 = (r.left >= r.x + halfNodeWidth)) &&
               (b3 = (r.top < r.y + halfNodeHeight)) ^ (b4 = (r.bottom >= r.y + halfNodeHeight)))
            {
                split();
                if (b1)
                    if (b3)
                        return bottomLeft;
                    else
                        return topLeft;
                else
                    if (b3)
                        return bottomRight;
                    else
                        return topRight;
            }
            else
                return null;
        }

        public void split()
        {
            if (!hasChildren)
            {
                if (rect.w == 1 || rect.h == 1)
                    throw new Exception("Attempting to split a quadtree of size 1.");

                int halfW = rect.w / 2;
                int halfH = rect.h / 2;

                topRight = new Node(new Rect(rect.x+ halfW, rect.y, halfW, halfH));
                topLeft = new Node(new Rect(rect.x, rect.y, halfW, halfH));
                bottomLeft = new Node(new Rect(rect.x, rect.y + halfH, halfW, halfH));
                bottomRight = new Node(new Rect(rect.x+ halfW, rect.h+halfH, halfW, halfH));
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

            objects.Clear();
        }

        public void findIntersecting(Rect r, List<Video.Transformable> answer)
        {
            //Check if any objects in this node intersect.
            foreach (Video.Transformable t in objects)
                if (t.overlaps(r))
                    answer.Add(t);

            //Ask the children for any additional shapes
            Node next = null;
            if (hasChildren)
                next = splitAndFindChildWithPerfectFit(r, rect.w/2, rect.h/2);

            if(next!=null)
                next.findIntersecting(r, answer);
        }

        public System.Collections.IEnumerable GetEnnumerable()
        {
            //Return all objects in this node
            foreach (Video.Transformable t in objects)
                yield return t;

            //Return all objects in child nodes
            if (hasChildren)
            {
                foreach (var x in topRight.GetEnnumerable())
                    yield return x;
                foreach (var x in topLeft.GetEnnumerable())
                    yield return x;
                foreach (var x in bottomRight.GetEnnumerable())
                    yield return x;
                foreach (var x in bottomLeft.GetEnnumerable())
                    yield return x;
            }
        }
    }
}
