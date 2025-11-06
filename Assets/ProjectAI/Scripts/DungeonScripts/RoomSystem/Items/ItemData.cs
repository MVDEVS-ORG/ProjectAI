using Assets.ProjectAI.Scripts.HelperClasses;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.ProjectAI.Scripts.DungeonScripts.RoomSystem.Items
{
    [CreateAssetMenu(fileName = "NewItemData", menuName = "Dungeon/Item Data")]
    public class ItemData : ScriptableObject
    {
        [Header("Basic Info")]
        public string itemId; // Addressable key or prefab name
        public Sprite sprite;

        [Header("Size & Type")]
        [Tooltip("Width and Height in Tile Units (e.g., 3x3 Table)")]
        public Vector2Int size = new Vector2Int(1, 1);
        public PlacementType placementType;

        [Header("Placement Settings")]
        [Tooltip("If true, offset placement slightly to center it visually.")]
        public bool addOffset = false;

        public bool PlaceAsGroup;
        public int GroupMinCount = 2;
        public int GroupMaxCount = 4;

        public bool Corner;
        public bool NearWallUp;
        public bool NearWallDown;
        public bool NearWallLeft;
        public bool NearWallRight;
        public bool Inner;


        [Header("Stats")]
        public int health = 1;
        public int maxHealth = 3;
        public bool nonDestructible = false;
        public bool litObject = false;

        /// <summary>
        /// Returns all occupied tiles for this item given a bottom-left origin.
        /// </summary>
        public HashSet<Vector2Int> GetOccupiedTiles(Vector2Int origin, Vector2Int itemSize)
        {
            HashSet<Vector2Int> tiles = new();
            for (int x = 0; x < itemSize.x; x++)
            {
                for (int y = 0; y < itemSize.y; y++)
                {
                    tiles.Add(origin + new Vector2Int(x, y));
                }
            }
            return tiles;
        }


        /// <summary>
        /// Checks if item can be placed on given floor positions.
        /// </summary>
        public bool CanBePlaced(Vector2Int origin, HashSet<Vector2Int> floorPositions, HashSet<Vector2Int> occupiedPositions, Vector2Int itemSize)
        {

            var occupied = GetOccupiedTiles(origin, itemSize);

            foreach (var pos in occupied)
            {
                if (!floorPositions.Contains(pos) || occupiedPositions.Contains(pos))
                    return false;
            }

            return true;
        }
    }
}
