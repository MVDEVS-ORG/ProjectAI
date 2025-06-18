using Assets.ProjectAI.Scripts.HelperClass;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.ProjectAI.Scripts.DungeonScripts.DecisionSystem
{
    public class DijkstraAlgorithm
    {
        public static Dictionary<Vector2Int, int> Dijkstra(Graph graph, Vector2Int startPosition)
        {
            Queue<Vector2Int> unfinishedVertices = new Queue<Vector2Int>();

            Dictionary<Vector2Int, int> distanceDictionary = new Dictionary<Vector2Int, int>();
            Dictionary<Vector2Int, Vector2Int> parentDictionary = new Dictionary<Vector2Int, Vector2Int>();

            distanceDictionary[startPosition] = 0;
            parentDictionary[startPosition] = startPosition;

            foreach(Vector2Int vertex in graph.GetNeighbors4Directions(startPosition))
            {
                unfinishedVertices.Enqueue(vertex);
                parentDictionary[vertex] = startPosition;
            }

            while(unfinishedVertices.Count > 0)
            {
                Vector2Int vertex = unfinishedVertices.Dequeue();
                int newDistance = distanceDictionary[parentDictionary[vertex]] + 1;
                if (distanceDictionary.ContainsKey(vertex) && distanceDictionary[vertex] <= newDistance)
                    continue;
                distanceDictionary[vertex] = newDistance;
                foreach(Vector2Int neighbor in graph.GetNeighbors4Directions(vertex))
                {
                    if (distanceDictionary.ContainsKey(neighbor))
                        continue;
                    unfinishedVertices.Enqueue(neighbor);
                    parentDictionary[neighbor] = vertex;
                }
            }
            return distanceDictionary;
        }
    }
}