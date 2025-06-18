using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.ProjectAI.Scripts.HelperClass
{
    public class Graph
    {
        private static List<Vector2Int> neighbors4directions = new List<Vector2Int>
        {
            new Vector2Int(0,1), //UP
            new Vector2Int(1,0), //Right
            new Vector2Int(0,-1), //Down
            new Vector2Int(-1,0) //Left
        };
        private static List<Vector2Int> neighbors8directions = new List<Vector2Int>
        {
            new Vector2Int(0,1),
            new Vector2Int(1,0),
            new Vector2Int(0,-1),
            new Vector2Int(-1,0),
            new Vector2Int(1,1),
            new Vector2Int(1,-1),
            new Vector2Int(-1,1),
            new Vector2Int(-1,-1)
        };

        List<Vector2Int> graph;

        public Graph(IEnumerable<Vector2Int> vertices)
        {
            graph = new List<Vector2Int>(vertices);
        }

        public List<Vector2Int> GetNeighbors4Directions(Vector2Int startPosition)
        {
            return GetNeighbors(startPosition, neighbors4directions);
        }

        public List<Vector2Int> GetNeighbors8Directions(Vector2Int startPosition)
        {
            return GetNeighbors(startPosition, neighbors8directions);
        }

        private List<Vector2Int> GetNeighbors(Vector2Int startPosition, List<Vector2Int> neighborsOffsetList)
        {
            List<Vector2Int> neighbors = new List<Vector2Int>();
            foreach (var neighborDirection in neighborsOffsetList)
            {
                Vector2Int potentialNeighbor = startPosition + neighborDirection;
                if(graph.Contains(potentialNeighbor))
                    neighbors.Add(potentialNeighbor);
            }
            return neighbors;
        }
    }
}