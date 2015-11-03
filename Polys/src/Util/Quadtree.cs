﻿using System;
using System.Collections.Generic;
using OpenGL;

namespace Polys.Util
{
    /**A region quadtree designed for optimising many tasks!*/
    public class Quadtree
    {
        Node root;
        
        /** The constructor takes a rect representing the quadtree.
          * Note that the width and height must be power of 2 */
        public Quadtree(Rect rect)
        {
            if ((rect.w & (rect.w - 1)) != 0 || (rect.h & (rect.h - 1)) != 0 || rect.w == 0 || rect.h == 0)
                throw new Exception("Attempting to create a quadtree with non power-of-two size.");

            root = new Node(rect);
        }

        /** Deletes all the nodes. */
        public void clear()
        {
            root.clear();
        }

        /** Inserts an item into the quadtree. */
        public void insert(Video.Transformable t)
        {
            root.insert(t);
        }

        /** Finds all objects that intersect with the rect. */
        public List<Video.Transformable> findIntersecting(Rect rect)
        {
            List<Video.Transformable> answer = new List<Video.Transformable>();
            root.findIntersecting(rect, answer);
            return answer;
        }

        /** Allows one to quickly iterate through the internal objects. */
        public System.Collections.IEnumerable GetEnnumerable()
        {
            foreach (var v in root.GetEnnumerable())
                yield return v;
        }

        /** Writes this quadtree to a file as a directed graph in the GML format. */
        public void dumpGmlToFile(string path)
        {
            string text = "graph\n[\n";

            root.fillGml(ref text);

            text += "\n]\n"; //close graph definition

            System.IO.File.WriteAllText(path, text);
        }

        class Node
        {
            //A node will not attempt to split until this number of object is contained in it.
            //It may still refuse to split if the object being inserted doesn't fit into a subnode.
            static int minObjectsBeforeSplit = 3;

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
                int halfW = rect.w / 2;
                int halfH = rect.h / 2;

                //If we have not yet reached the max objects, or if it doesn't fit into any of subsquares, insert into this.
                if (objects.Count < minObjectsBeforeSplit || t.rect.w > halfW || t.rect.h > halfH || halfW == 1 || halfH == 1)
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
                bool left, right, bottom, top;
                if ((left = (r.right < rect.x + halfNodeWidth)) ^ (right = (r.left >= rect.x + halfNodeWidth)) &&
                   (bottom = (r.top < rect.y + halfNodeHeight)) ^ (top = (r.bottom >= rect.y + halfNodeHeight)))
                {
                    split();
                    if (left)
                        if (bottom)
                            return bottomLeft;
                        else
                            return topLeft;
                    else if (bottom)
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

                    topRight = new Node(new Rect(rect.x + halfW, rect.y + halfH, halfW, halfH));
                    topLeft = new Node(new Rect(rect.x, rect.y + halfH, halfW, halfH));
                    bottomLeft = new Node(new Rect(rect.x + halfW, rect.y, halfW, halfH));
                    bottomRight = new Node(new Rect(rect.x + halfW, rect.y, halfW, halfH));
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
                    next = splitAndFindChildWithPerfectFit(r, rect.w / 2, rect.h / 2);

                if (next != null)
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

            public void fillGml(ref string str, int id = 0, int parentId = 0, int type = 0)
            {
                //Document the node
                string[] typeLabelMapping = new string[] { "Root", "TopRight", "TopLeft", "BottomLeft", "BottomRight" };

                str += "node\n[\n id " + id + "\n label \"" + typeLabelMapping[type] + "(" + objects.Count + ")\"\n ]\n";

                //If it has a parent, add an edge.
                if (id != 0)
                    str += "edge\n[\n source " + parentId + "\n target " + id + "\n]\n";

                if (hasChildren)
                {
                    topRight.fillGml(ref str, id + 1, id, 1);
                    topLeft.fillGml(ref str, id + 2, id, 2);
                    bottomLeft.fillGml(ref str, id + 3, id, 3);
                    bottomRight.fillGml(ref str, id + 4, id, 4);
                }
            }
        }
    }
}
