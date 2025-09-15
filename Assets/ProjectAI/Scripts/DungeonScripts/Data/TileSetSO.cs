using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Assets.ProjectAI.Scripts.DungeonScripts.Data
{
    [CreateAssetMenu(fileName = "TileSet", menuName = "Dungeon/TileSet")]
    public class TileSetSO : ScriptableObject
    {
        [Header("Floor")]
        public TileBase floorTile;

        [Header("Walls - Basic")]
        public TileBase wallTop;
        public TileBase wallSideRight;
        public TileBase wallSideLeft;
        public TileBase wallBottom;
        public TileBase wallFull;

        [Header("Walls - Corners")]
        public TileBase wallInnerCornerDownLeft;
        public TileBase wallInnerCornerDownRight;
        public TileBase wallDiagonalCornerDownRight;
        public TileBase wallDiagonalCornerDownLeft;
        public TileBase wallDiagonalCornerUpRight;
        public TileBase wallDiagonalCornerUpLeft;
    }
}