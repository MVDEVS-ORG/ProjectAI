using Assets.ProjectAI.Scripts.DungeonScripts.Data;
using Assets.ProjectAI.Scripts.HelperClass;
using Assets.Services;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Zenject;

namespace Assets.ProjectAI.Scripts.DungeonScripts
{
    public class TilemapVisualizer : MonoBehaviour
    {
        [Inject] IAssetService _assetService;
        [SerializeField]
        private Tilemap _floorTilemap, _wallTilemap;
        [SerializeField] private TileSetSO _currentTileSet;

        public void SetTileSet(TileSetSO tileSet)
        {
            _currentTileSet = tileSet;
            Debug.Log($"Tile Set changed at runTime to: {_currentTileSet.name}");
        }

        public void PaintFloorTiles(IEnumerable<Vector2Int> floorPositions)
        {
            if (_currentTileSet == null || _currentTileSet.floorTile == null)
            {
                Debug.LogError("TileSet or floor tile not assigned!");
                return;
            }

            PaintTiles(floorPositions, _floorTilemap, _currentTileSet.floorTile);
        }

        private void PaintTiles(IEnumerable<Vector2Int> positions, Tilemap tilemap, TileBase tile)
        {
            foreach (var position in positions)
            {
                PaintSingleTile(tilemap, tile, position);
            }
        }

        private void PaintSingleTile(Tilemap tilemap, TileBase tile, Vector2Int position)
        {
            var tilePosition = tilemap.WorldToCell((Vector3Int)position);
            tilemap.SetTile(tilePosition, tile);
        }

        public void Clear()
        {
            _floorTilemap.ClearAllTiles();
            _wallTilemap.ClearAllTiles();
        }

        public void PaintSingleBasicWall(Vector2Int position, string binaryType)
        {
            if (_currentTileSet == null)
            {
                Debug.LogError("TileSet not assigned!");
                return;
            }

            int typeAsInt = Convert.ToInt32(binaryType, 2);
            TileBase tile = null;

            if (WallTypesHelper.wallTop.Contains(typeAsInt))
                tile = _currentTileSet.wallTop;
            else if (WallTypesHelper.wallSideRight.Contains(typeAsInt))
                tile = _currentTileSet.wallSideRight;
            else if (WallTypesHelper.wallSideLeft.Contains(typeAsInt))
                tile = _currentTileSet.wallSideLeft;
            else if (WallTypesHelper.wallBottm.Contains(typeAsInt))
                tile = _currentTileSet.wallBottom;
            else if (WallTypesHelper.wallFull.Contains(typeAsInt))
                tile = _currentTileSet.wallFull;

            if (tile != null)
                PaintSingleTile(_wallTilemap, tile, position);
        }

        public void PaintSingleCornerWall(Vector2Int position, string binaryType)
        {
            if (_currentTileSet == null)
            {
                Debug.LogError("TileSet not assigned!");
                return;
            }

            int typeAsInt = Convert.ToInt32(binaryType, 2);
            TileBase tile = null;

            if (WallTypesHelper.wallInnerCornerDownLeft.Contains(typeAsInt))
                tile = _currentTileSet.wallInnerCornerDownLeft;
            else if (WallTypesHelper.wallInnerCornerDownRight.Contains(typeAsInt))
                tile = _currentTileSet.wallInnerCornerDownRight;
            else if (WallTypesHelper.wallDiagonalCornerDownLeft.Contains(typeAsInt))
                tile = _currentTileSet.wallDiagonalCornerDownLeft;
            else if (WallTypesHelper.wallDiagonalCornerDownRight.Contains(typeAsInt))
                tile = _currentTileSet.wallDiagonalCornerDownRight;
            else if (WallTypesHelper.wallDiagonalCornerUpRight.Contains(typeAsInt))
                tile = _currentTileSet.wallDiagonalCornerUpRight;
            else if (WallTypesHelper.wallDiagonalCornerUpLeft.Contains(typeAsInt))
                tile = _currentTileSet.wallDiagonalCornerUpLeft;
            else if (WallTypesHelper.wallFullEightDirections.Contains(typeAsInt))
                tile = _currentTileSet.wallFull;
            else if (WallTypesHelper.wallBottmEightDirections.Contains(typeAsInt))
                tile = _currentTileSet.wallBottom;

            if (tile != null)
                PaintSingleTile(_wallTilemap, tile, position);
        }
    }
}