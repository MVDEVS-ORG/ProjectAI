using Assets.ProjectAI.Scripts.HelperClass;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.ProjectAI.Scripts.HelperClasses
{
    public class ItemPlacementHelper
    {
        Dictionary<PlacementType, HashSet<Vector2Int>> tileByType = new Dictionary<PlacementType, HashSet<Vector2Int>>();
        HashSet<Vector2Int> roomFloorNoCorridor;

        public ItemPlacementHelper(HashSet<Vector2Int> roomFloor, HashSet<Vector2Int> roomFloorNoCorridor)
        {
            Graph graph = new Graph(roomFloor);
            this.roomFloorNoCorridor = roomFloorNoCorridor;

            foreach(var position in roomFloorNoCorridor)
            {
                int neighborsCount8Dir = graph.GetNeighbors8Directions(position).Count;
                PlacementType type = neighborsCount8Dir < 8 ? PlacementType.NearWall : PlacementType.OpenSpace;

                if(tileByType.ContainsKey(type) == false)
                {
                    tileByType[type] = new HashSet<Vector2Int>();
                } 
                if(type == PlacementType.NearWall && graph.GetNeighbors4Directions(position).Count == 4)
                {
                    continue;
                }
                tileByType[type].Add(position);
            }
        }

        public Vector2? GetItemPlacementPosition(PlacementType placementType, int iterationsMax, Vector2Int size, bool addoffset)
        {
            int itemArea = size.x * size.y;
            if (tileByType[placementType].Count < itemArea)
            {
                return null;
            }
            int iteration = 0;
            while(iteration < iterationsMax)
            {
                iteration++;
                int index = Random.Range(0, tileByType[placementType].Count);
                Vector2Int position = tileByType[placementType].ElementAt(index);

                if(itemArea > 1)
                {
                    var (result, placementPosition) = PlaceBigItem(position, size, addoffset);

                    if(result == false)
                    {
                        continue;
                    }
                    tileByType[placementType].ExceptWith(placementPosition);
                }
                else
                {
                    tileByType[placementType].Remove(position);
                }
                return position;
            }
            return null;
        }

        private (bool, List<Vector2Int>) PlaceBigItem(Vector2Int originPosition, Vector2Int size, bool addOffset)
        {
            List<Vector2Int> positions = new();

            // Adjust offset if required
            int offsetX = addOffset ? -1 : 0;
            int offsetY = addOffset ? -1 : 0;
            int width = size.x + (addOffset ? 2 : 0);
            int height = size.y + (addOffset ? 2 : 0);

            // Treat originPosition as bottom-left corner
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Vector2Int pos = new Vector2Int(originPosition.x + offsetX + x, originPosition.y + offsetY + y);

                    if (!roomFloorNoCorridor.Contains(pos))
                    {
                        return (false, positions); // early exit on failure
                    }

                    positions.Add(pos);
                }
            }

            return (true, positions);
        }

    }

    public enum PlacementType
    {
        OpenSpace,
        NearWall
    }
}