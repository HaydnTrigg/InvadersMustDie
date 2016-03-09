using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
//http://alopex.li/wiki/QuadTree

#region "Imports"

//System Imports

//OpenTK Imports
using OpenTK;

#endregion

namespace Isotope.Library
{
    public class QuadTree<T>
    {
        #region "Variables & Properties"

        public Vector2 Loc;
        public T Item;
        public QuadTree<T> UpperLeft;
        public QuadTree<T> UpperRight;
        public QuadTree<T> LowerLeft;

        public QuadTree<T> LowerRight;
        #endregion
        #region "Initializers"


        public QuadTree(Vector2 loc__1, T i)
        {
            Item = i;
            Loc = loc__1;
            UpperLeft = null;
            UpperRight = null;
            LowerLeft = null;
            LowerRight = null;
        }

        #endregion
        #region "Main Functions"

        public void Insert(Vector2 loc__1, T item)
        {
            if (loc__1.X > Loc.X)
            {
                if (loc__1.Y > Loc.Y)
                {
                    if (UpperRight == null)
                    {
                        UpperRight = new QuadTree<T>(loc__1, item);
                    }
                    else
                    {
                        UpperRight.Insert(loc__1, item);
                    }
                }
                else
                {
                    // loc.Y <= Loc.Y
                    if (LowerRight == null)
                    {
                        LowerRight = new QuadTree<T>(loc__1, item);
                    }
                    else
                    {
                        LowerRight.Insert(loc__1, item);
                    }
                }
            }
            else
            {
                // loc.X <= Loc.X
                if (loc__1.Y > Loc.Y)
                {
                    if (UpperLeft == null)
                    {
                        UpperLeft = new QuadTree<T>(loc__1, item);
                    }
                    else
                    {
                        UpperLeft.Insert(loc__1, item);
                    }
                }
                else
                {
                    // loc.Y <= Loc.Y
                    if (LowerLeft == null)
                    {
                        LowerLeft = new QuadTree<T>(loc__1, item);
                    }
                    else
                    {
                        LowerLeft.Insert(loc__1, item);
                    }
                }
            }
        }

        public List<T> GetWithin(Vector2 target, double d)
        {
            return GetWithin(target, d, new List<T>());
        }

        // Returns a list of items within d of the target
        // Upon consideration, it feels weird not making this tail-recursive.
        public List<T> GetWithin(Vector2 target, double d, List<T> ret)
        {
            double dsquared = d * d;
            // First, we check and see if the current item is in range
            if (GameMath.Vector2DistanceSquared(Loc, target) < dsquared)
            {
                ret.Add(Item);
            }

            if (target.X + d > Loc.X || target.X > Loc.X)
            {
                if (target.Y + d > Loc.Y || target.Y > Loc.Y)
                {
                    if (UpperRight != null)
                    {
                        UpperRight.GetWithin(target, d, ret);
                    }
                }
                if (target.Y - d <= Loc.Y || target.Y <= Loc.Y)
                {
                    if (LowerRight != null)
                    {
                        LowerRight.GetWithin(target, d, ret);
                    }
                }
            }
            if (target.X - d <= Loc.X || target.X <= Loc.X)
            {
                if (target.Y + d > Loc.Y || target.Y > Loc.Y)
                {
                    if (UpperLeft != null)
                    {
                        UpperLeft.GetWithin(target, d, ret);
                    }
                }
                if (target.Y - d <= Loc.Y || target.Y <= Loc.Y)
                {
                    if (LowerLeft != null)
                    {
                        LowerLeft.GetWithin(target, d, ret);
                    }
                }
            }

            return ret;
        }

        #endregion
    }
}
