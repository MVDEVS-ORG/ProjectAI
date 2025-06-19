using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.ProjectAI.Scripts.DungeonScripts
{
    public class CorridorFirstDungeonGenerator : SimpleRandomWalkDungeonGenerator
    {
        [SerializeField]
        private int corridorLength = 14, corridorCount = 5;
        [SerializeField]
        [Range(0.1f, 1)]
        private float roomPercent = 0.8f;
        protected override async Awaitable RunProceduralGeneration()
        {
            await CorridorFirstDungeonGeneration();
        }

        private async Awaitable CorridorFirstDungeonGeneration()
        {
            HashSet<Vector2Int> floorPositions = new HashSet<Vector2Int>();
            HashSet<Vector2Int> potentialRoomPositions = new HashSet<Vector2Int>();

            List<List<Vector2Int>> corridors = CreateCorridors(floorPositions, potentialRoomPositions);

            HashSet<Vector2Int> roomPositions = CreateRooms(potentialRoomPositions);
            List<Vector2Int> deadEnds = FindAllDeadEnds(floorPositions);
            CreateRoomsAtDeadEnd(deadEnds, roomPositions);

            floorPositions.UnionWith(roomPositions);

            for (int i = 0; i < corridors.Count; i++)
            {
                corridors[i] = IncreaseCorridorSizeByOne(corridors[i]);
                floorPositions.UnionWith(corridors[i]);
            }

            tilemapVisualizer.PaintFloorTiles(floorPositions);
            await WallGenerator.CreateWalls(floorPositions, tilemapVisualizer);
        }

        private List<Vector2Int> IncreaseCorridorSizeByOne(List<Vector2Int> corridor)
        {
            List<Vector2Int> newCorridor = new List<Vector2Int>();
            Vector2Int previousDirection = Vector2Int.zero;
            for (int i = 1; i < corridor.Count; i++)
            {
                Vector2Int directionFromCell = corridor[i] - corridor[i - 1];
                if (previousDirection != Vector2Int.zero && directionFromCell != previousDirection)
                {
                    //HandleCorner
                    for (int x = -1; x < 2; x++)
                    {
                        for (int y = -1; y < 2; y++)
                        {
                            newCorridor.Add(corridor[i - 1] + new Vector2Int(x, y));
                        }
                    }
                    previousDirection = directionFromCell;
                }
                else
                {
                    //Add a Single Cell in the Direction + 90 degrees
                    Vector2Int newCorrdorTileOffset = GetDirection90From(directionFromCell);
                    newCorridor.Add(corridor[i - 1]);
                    newCorridor.Add(corridor[i - 1] + newCorrdorTileOffset);
                }
            }
            return newCorridor;
        }

        private Vector2Int GetDirection90From(Vector2Int direction)
        {
            if (direction == Vector2Int.up)
                return Vector2Int.right;
            if (direction == Vector2Int.right)
                return Vector2Int.down;
            if (direction == Vector2Int.down)
                return Vector2Int.left;
            if (direction == Vector2Int.left)
                return Vector2Int.up;
            return Vector2Int.zero;
        }

        private void CreateRoomsAtDeadEnd(List<Vector2Int> deadEnds, HashSet<Vector2Int> roomFloors)
        {
            foreach (var position in deadEnds)
            {
                if (roomFloors.Contains(position) == false)
                {
                    var room = RunRandomWalk(randomWalkParameters, position);
                    roomFloors.UnionWith(room);
                }
            }
        }

        private List<Vector2Int> FindAllDeadEnds(HashSet<Vector2Int> floorPositions)
        {
            List<Vector2Int> deadEnds = new List<Vector2Int>();
            foreach (Vector2Int pos in floorPositions)
            {
                int neighborsCount = 0;
                foreach (var direction in Direction2D.cardinalDirectionList)
                {
                    if (floorPositions.Contains(pos + direction))
                        neighborsCount++;

                }
                if (neighborsCount == 1)
                    deadEnds.Add(pos);
            }
            return deadEnds;
        }

        private HashSet<Vector2Int> CreateRooms(HashSet<Vector2Int> potentialRoomPositions)
        {
            HashSet<Vector2Int> roomPositions = new HashSet<Vector2Int>();
            int roomToCreateCount = Mathf.RoundToInt(potentialRoomPositions.Count * roomPercent);

            List<Vector2Int> roomToCreate = potentialRoomPositions.OrderBy(x => Guid.NewGuid()).Take(roomToCreateCount).ToList();

            foreach (var roomPosition in roomToCreate)
            {
                var roomFloor = RunRandomWalk(randomWalkParameters, roomPosition);
                roomPositions.UnionWith(roomFloor);
            }
            return roomPositions;
        }

        private List<List<Vector2Int>> CreateCorridors(HashSet<Vector2Int> floorPositions, HashSet<Vector2Int> potentialRoomPositions)
        {
            var currentPosition = startPosition;
            potentialRoomPositions.Add(currentPosition);
            List<List<Vector2Int>> corridors = new List<List<Vector2Int>>();
            for (int i = 0; i < corridorCount; i++)
            {
                var corridor = ProceduralGenerationAlgorithms.RandomWalkCorridor(currentPosition, corridorLength);
                corridors.Add(corridor);
                currentPosition = corridor[corridor.Count - 1];
                potentialRoomPositions.Add(currentPosition);
                floorPositions.UnionWith(corridor);
            }
            return corridors;
        }
    }
}