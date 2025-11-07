using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.ProjectAI.Scripts.DungeonScripts.RoomSystem
{
    public class Room
    {
        public Vector2 RoomCenterPos { get; set; }
        public HashSet<Vector2Int> FloorTiles { get; private set; } = new HashSet<Vector2Int>();

        public HashSet<Vector2Int> NearWallTilesUp { get; set; } = new HashSet<Vector2Int>();
        public HashSet<Vector2Int> NearWallTilesDown { get; set; } = new HashSet<Vector2Int>();
        public HashSet<Vector2Int> NearWallTilesLeft { get; set; } = new HashSet<Vector2Int>();
        public HashSet<Vector2Int> NearWallTilesRight { get; set; } = new HashSet<Vector2Int>();
        public HashSet<Vector2Int> CornerTiles { get; set; } = new HashSet<Vector2Int>();
        public HashSet<Vector2Int> InnerTiles { get; set; } = new HashSet<Vector2Int>();
        public HashSet<Vector2Int> ItemPositions { get; set; } = new HashSet<Vector2Int>();
        public HashSet<Vector2Int> DoorPositons { get; set; } = new HashSet<Vector2Int>();

        public Room(Vector2 roomCenterPos, HashSet<Vector2Int> floorTiles, HashSet<Vector2Int> doorPos)
        {
            this.RoomCenterPos = roomCenterPos;
            this.FloorTiles = floorTiles;
            this.DoorPositons = FindAdjacentDoors(floorTiles, doorPos);
        }
        private HashSet<Vector2Int> FindAdjacentDoors(HashSet<Vector2Int> floorTiles, HashSet<Vector2Int> allDoorPositions)
        {
            HashSet<Vector2Int> adjacentDoors = new();

            foreach (var door in allDoorPositions)
            {
                // Check if the door is next to any floor tile (4-directionally)
                bool isAdjacent = Direction2D.cardinalDirectionList.Any(neighbor => floorTiles.Contains(neighbor + door));

                if (isAdjacent)
                    adjacentDoors.Add(door);
            }
            Debug.LogError($"Found {adjacentDoors.Count} adjacent doors for room at {RoomCenterPos}");
            return adjacentDoors;
        }

        public void ProcessRooms()
        {
            foreach (Vector2Int tilePosition in this.FloorTiles)
            {
                int neightboursCount = 4;
                if (this.FloorTiles.Contains(tilePosition + Vector2Int.up) == false)
                {
                    neightboursCount--;
                    this.NearWallTilesUp.Add(tilePosition);
                }
                if (this.FloorTiles.Contains(tilePosition + Vector2Int.down) == false)
                {
                    neightboursCount--;
                    this.NearWallTilesDown.Add(tilePosition);
                }
                if (this.FloorTiles.Contains(tilePosition + Vector2Int.right) == false)
                {
                    neightboursCount--;
                    this.NearWallTilesRight.Add(tilePosition);
                }
                if (this.FloorTiles.Contains(tilePosition + Vector2Int.left) == false)
                {
                    neightboursCount--;
                    this.NearWallTilesLeft.Add(tilePosition);
                }

                //FindCorners
                if (neightboursCount <= 2)
                {
                    this.CornerTiles.Add(tilePosition);
                }

                if (neightboursCount == 4)
                {
                    this.InnerTiles.Add(tilePosition);
                }
            }
            this.NearWallTilesDown.ExceptWith(this.CornerTiles);
            this.NearWallTilesUp.ExceptWith(this.CornerTiles);
            this.NearWallTilesLeft.ExceptWith(this.CornerTiles);
            this.NearWallTilesRight.ExceptWith(this.CornerTiles);
        }

    }
}