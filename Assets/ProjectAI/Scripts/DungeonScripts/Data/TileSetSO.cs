using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Assets.ProjectAI.Scripts.DungeonScripts.Data
{
    [CreateAssetMenu(fileName = "TileSet", menuName = "Dungeon/TileSet")]
    public class TileSetSO : ScriptableObject
    {
        [Header("Dungeon-Tile")]
        public TileBase dungeonRuleTile;

        [Header("BackgroundTiles")]
        public TileBase backgroundTile;
    }
}