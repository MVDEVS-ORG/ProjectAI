using System.Collections;
using UnityEngine;

namespace Assets.ProjectAI.Scripts.PathFinding
{
    public class PathNode
    {
        public Vector3Int position;
        public bool walkable;

        public int gCost;
        public int hCost;
        public int fCost => gCost + hCost;

        public PathNode parent;
        public bool closed;

        public void Reset()
        {
            gCost = int.MaxValue;
            hCost = 0;
            parent = null;
            closed = false;
        }
    }
}