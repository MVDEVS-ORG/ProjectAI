using Assets.ProjectAI.Scripts.DungeonScripts.RoomSystem;
using Assets.ProjectAI.Scripts.DungeonScripts.RoomSystem.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.ProjectAI.Scripts.HelperClasses
{
    public class ItemPlacementHelper
    {
        private Room _room;
        public HashSet<Vector2Int> OccupiedPositions { get; private set; } = new();

        public ItemPlacementHelper(Room room)
        {
            this._room = room;
        }
        /// <summary>
        /// Tries to find a valid position near an origin within a given radius.
        /// </summary>
        public Vector2? GetPlacementNearPosition(Vector2 origin, float searchRadius)
        {
            // Convert the origin to grid coordinates
            Vector2Int originInt = Vector2Int.RoundToInt(origin);

            // Collect all valid nearby tiles
            List<Vector2Int> nearbyTiles = new();

            int intRadius = Mathf.CeilToInt(searchRadius);
            for (int x = -intRadius; x <= intRadius; x++)
            {
                for (int y = -intRadius; y <= intRadius; y++)
                {
                    Vector2Int testPos = originInt + new Vector2Int(x, y);

                    // Check if within circular radius
                    if (Vector2.Distance(originInt, testPos) > searchRadius)
                        continue;

                    // Must be part of the room floor and not occupied
                    if (_room.FloorTiles.Contains(testPos) && !OccupiedPositions.Contains(testPos))
                    {
                        nearbyTiles.Add(testPos);
                    }
                }
            }

            // If no valid positions found, return null
            if (nearbyTiles.Count == 0)
            {
                Debug.LogError("No valid nearby positions found for placement.");
                return null;
            }
                

            // Pick a random one for variation
            var selected = nearbyTiles[UnityEngine.Random.Range(0, nearbyTiles.Count)];
            return selected;
        }


        /// <summary>
        /// Randomly finds a placement position of given type.
        /// </summary>
        public Vector2? GetItemPlacementPosition(ItemData itemData)
        {
            switch (itemData)
            {
                case { Corner: true }:
                    return PlaceCornerItem(itemData);
                case { NearWallDown: true }:
                    return PlaceNearWallVerticleItem(itemData, Vector2Int.up);
                case { NearWallUp: true }:
                    return PlaceNearWallVerticleItem(itemData, Vector2Int.down);
                case { NearWallLeft: true }:
                    return PlaceNearWallHorizontalItem(itemData, Vector2Int.right);
                case { NearWallRight: true }:
                    return PlaceNearWallHorizontalItem(itemData, Vector2Int.left);
                case { Inner: true }:
                    return PlaceInnerItem(itemData);
                default:
                    Debug.LogError($"ItemData {itemData.name} has no valid placement type set.");
                    return null;
            }
        }
        private Vector2? PlaceInnerItem(ItemData itemData)
        {
            // Get all available inner tiles that aren't occupied
            HashSet<Vector2Int> availableTiles = _room.InnerTiles.Except(OccupiedPositions).ToHashSet();
            if (availableTiles.Count == 0)
            {
                Debug.LogError("No Inner positions available for placement.");
                return null;
            }

            // Shuffle list for randomized placement
            var shuffledTiles = availableTiles.OrderBy(_ => UnityEngine.Random.value).ToList();

            foreach (var baseTile in shuffledTiles)
            {
                // Try placing directly or near this base tile
                // Small random offsets in four directions for natural variation
                List<Vector2Int> offsets = new()
                {
                    Vector2Int.zero,
                    Vector2Int.up,
                    Vector2Int.down,
                    Vector2Int.left,
                    Vector2Int.right
                };

                foreach (var offset in offsets)
                {
                    Vector2Int testTile = baseTile + offset;

                    if (!itemData.CanBePlaced(testTile, _room.FloorTiles, OccupiedPositions, itemData.size))
                        continue;

                    // Get all tiles the item would occupy
                    var occupiedTiles = itemData.GetOccupiedTiles(testTile, itemData.size);

                    // Check if any occupied tile is adjacent to a door
                    bool nearDoor = occupiedTiles.Any(tile =>
                        Direction2D.cardinalDirectionList.Any(neighbor => _room.DoorPositons.Contains(neighbor + tile))
                    );

                    if (nearDoor)
                        continue; // Skip placement near door

                    // Mark tiles as occupied and return
                    MarkOccupied(itemData, testTile);
                    return testTile;
                }
            }

            Debug.LogError($"No valid Inner position found for item {itemData.name}");
            return null;
        }

        private Vector2? PlaceNearWallHorizontalItem(ItemData itemData, Vector2Int direction)
        {
            // Get unoccupied tiles near the left or right wall
            HashSet<Vector2Int> availableTiles = _room.NearWallTilesLeft.Except(OccupiedPositions).ToHashSet();
            if (availableTiles.Count == 0)
            {
                Debug.LogError("No NearWallLeft positions available for placement.");
                return null;
            }

            // Shuffle list to randomize placement
            var shuffledTiles = availableTiles.OrderBy(_ => UnityEngine.Random.value).ToList();

            foreach (var baseTile in shuffledTiles)
            {
                Vector2Int moveDir = direction;
                int maxOffset = Mathf.Clamp(itemData.size.x, 1, 5);

                for (int offsetX = 0; offsetX <= maxOffset; offsetX++)
                {
                    Vector2Int testTile = baseTile + moveDir * offsetX;

                    if (!itemData.CanBePlaced(testTile, _room.FloorTiles, OccupiedPositions, itemData.size))
                        continue;

                    // Get all tiles the item would occupy
                    var occupiedTiles = itemData.GetOccupiedTiles(testTile, itemData.size);

                    // Check if any occupied tile is adjacent to a door
                    bool nearDoor = occupiedTiles.Any(tile =>
                        Direction2D.cardinalDirectionList.Any(neighbor => _room.DoorPositons.Contains(neighbor + tile))
                    );

                    if (nearDoor)
                        continue; // Skip placement near door

                    // Mark tiles as occupied and return
                    MarkOccupied(itemData, testTile);
                    return testTile;
                }
            }

            Debug.LogError($"No valid NearWallLeft position found for item {itemData.name}");
            return null;
        }



        private Vector2? PlaceNearWallVerticleItem(ItemData itemData, Vector2Int direction)
        {
            // Get unoccupied tiles near the top or bottom wall
            HashSet<Vector2Int> availableTiles = _room.NearWallTilesUp.Except(OccupiedPositions).ToHashSet();
            if (availableTiles.Count == 0)
            {
                Debug.LogError("No NearWallUp positions available for placement.");
                return null;
            }

            // Shuffle list for random placement variety
            var shuffledTiles = availableTiles.OrderBy(_ => UnityEngine.Random.value).ToList();

            foreach (var baseTile in shuffledTiles)
            {
                Vector2Int moveDir = direction;
                int maxOffset = Mathf.Clamp(itemData.size.y, 1, 5);

                for (int offsetY = 0; offsetY <= maxOffset; offsetY++)
                {
                    Vector2Int testTile = baseTile + moveDir * offsetY;

                    if (!itemData.CanBePlaced(testTile, _room.FloorTiles, OccupiedPositions, itemData.size))
                        continue;

                    var occupiedTiles = itemData.GetOccupiedTiles(testTile, itemData.size);

                    // Check for any door adjacent to these tiles
                    bool nearDoor = occupiedTiles.Any(tile =>
                        Direction2D.cardinalDirectionList.Any(neighbor => _room.DoorPositons.Contains(neighbor + tile))
                    );

                    if (nearDoor)
                        continue; // Avoid placing near door

                    // Mark tiles as occupied
                    MarkOccupied(itemData, testTile);
                    return testTile;
                }
            }

            Debug.LogError($"No valid NearWall position found for item {itemData.name}");
            return null;
        }


        private Vector2? PlaceCornerItem(ItemData itemData)
        {
            HashSet<Vector2Int> availableCorners = _room.CornerTiles.Except(OccupiedPositions).ToHashSet();
            if (availableCorners.Count == 0)
            {
                Debug.LogError($"No corner positions available for placement. Item: {itemData.name}");
                return null;
            }

            // Shuffle corner list to make random selection fair
            var shuffledCorners = availableCorners.OrderBy(_ => UnityEngine.Random.value).ToList();

            foreach (var cornerTile in shuffledCorners)
            {
                // Determine binary neighbor pattern
                string neighborBinary = "";
                foreach (var dir in Direction2D.eightDirectionList)
                {
                    var neighbor = cornerTile + dir;
                    neighborBinary += _room.FloorTiles.Contains(neighbor) ? "1" : "0";
                }

                // Determine which corner type this tile likely represents
                Vector2Int moveDir1 = Vector2Int.zero;
                Vector2Int moveDir2 = Vector2Int.zero;

                if (neighborBinary == "11100000") // Bottom Left
                {
                    moveDir1 = Vector2Int.right;
                    moveDir2 = Vector2Int.up;
                }
                else if (neighborBinary == "00000111") // Bottom Right
                {
                    moveDir1 = Vector2Int.left;
                    moveDir2 = Vector2Int.up;
                }
                else if (neighborBinary == "00111000") // Top Left
                {
                    moveDir1 = Vector2Int.right;
                    moveDir2 = Vector2Int.down;
                }
                else if (neighborBinary == "00001110") // Top Right
                {
                    moveDir1 = Vector2Int.left;
                    moveDir2 = Vector2Int.down;
                }
                else
                {
                    // Not a clear corner pattern — skip this one
                    continue;
                }

                // Try to place at this corner or adjusted positions
                Vector2Int currentTile = cornerTile;

                // Try a few possible offset moves if the item doesn’t fit perfectly
                // Try multiple inward moves depending on item size
                int maxOffsetX = Mathf.Clamp(itemData.size.x, 1, 5);
                int maxOffsetY = Mathf.Clamp(itemData.size.y, 1, 5);

                for (int dx = 0; dx <= maxOffsetX; dx++)
                {
                    for (int dy = 0; dy <= maxOffsetY; dy++)
                    {
                        Vector2Int testTile = currentTile + moveDir1 * dx + moveDir2 * dy;

                        if (itemData.CanBePlaced(testTile, _room.FloorTiles, OccupiedPositions, itemData.size))
                        {
                            // Mark tiles as occupied
                            MarkOccupied(itemData, testTile);

                            return testTile; // Found a valid placement
                        }
                    }
                }
            }

            Debug.LogError($"No valid corner found for {itemData.name} after trying all corners.");
            return null;
        }

        /// <summary>
        /// Mark tiles as occupied for a given ItemData (used by PrefabPlacer).
        /// </summary>
        public void MarkOccupied(ItemData item, Vector2Int origin)
        {
            foreach (var pos in item.GetOccupiedTiles(origin, item.size))
                OccupiedPositions.Add(pos);
        }
    }

    public enum PlacementType
    {
        Corner,
        OpenSpace,
        NearWall,
        Enemy
    }
}
