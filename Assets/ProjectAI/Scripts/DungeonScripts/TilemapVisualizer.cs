using Assets.ProjectAI.Scripts.HelperClass;
using Assets.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
        [SerializeField]
        private TileBase _floorTile, wallTop, wallSideRight, wallSideLeft, wallBottom, wallFull,
            wallInnerCornerDownLeft, wallInnerCornerDownRight, 
            wallDiagonalCornerDownRight, wallDiagonalCornerDownLeft, wallDiagonalCornerUpRight, wallDiagonalCornerUpLeft;

        public async void PaintFloorTiles(IEnumerable<Vector2Int> floorPositions)
        {
            if (_assetService == null)
            {
                Debug.LogError("asset service is null");
            }
            var tileBase = await _assetService.LoadAssetAsync<TileBase>(AddressableIds.Floor);
            PaintTiles(floorPositions, _floorTilemap, tileBase);
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

        public async Awaitable PaintSingleBasicWall(Vector2Int position, string binaryType)
        {
            Debug.LogWarning("Generated");
            int typeAsInt = Convert.ToInt32(binaryType, 2);
            string tileBase = null;
            if (WallTypesHelper.wallTop.Contains(typeAsInt))
            {
                tileBase = AddressableIds.Wall_Top;
            }
            else if (WallTypesHelper.wallSideRight.Contains(typeAsInt))
            {
                tileBase = AddressableIds.Wall_Side_Right;
            }
            else if (WallTypesHelper.wallSideLeft.Contains(typeAsInt))
            {
                tileBase = AddressableIds.Wall_Side_Left;
            }
            else if (WallTypesHelper.wallBottm.Contains(typeAsInt))
            {
                tileBase = AddressableIds.Wall_Bottom;
            }
            else if (WallTypesHelper.wallFull.Contains(typeAsInt))
            {
                tileBase = AddressableIds.Wall_Full;
            }
            if (tileBase == null)
                return;
            try
            {
                TileBase tile = await _assetService.LoadAssetAsync<TileBase>(tileBase);
                if (tile != null)
                {
                    PaintSingleTile(_wallTilemap, tile, position);
                }
                else
                {
                    Debug.LogError($"tile is null addressable Id: {tileBase}");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"tile is null addressable Id: {tileBase} \n {ex}");
            }
        }

        public async Awaitable PaintSingleCornerWall(Vector2Int position, string binaryType)
        {
            Debug.LogWarning("Generated");
            int typeAsInt = Convert.ToInt32(binaryType, 2);
            string tileBase = null;
            if (WallTypesHelper.wallInnerCornerDownLeft.Contains(typeAsInt))
            {
                tileBase = AddressableIds.Wall_Inner_Corner_Down_Left;
            }
            else if (WallTypesHelper.wallInnerCornerDownRight.Contains(typeAsInt))
            {
                tileBase = AddressableIds.Wall_Inner_Corner_Down_Right;
            }
            else if (WallTypesHelper.wallDiagonalCornerDownLeft.Contains(typeAsInt))
            {
                tileBase = AddressableIds.Wall_Diagonal_Corner_Down_Left;
            }
            else if (WallTypesHelper.wallDiagonalCornerDownRight.Contains(typeAsInt))
            {
                tileBase = AddressableIds.Wall_Diagonal_Corner_Down_Right;
            }
            else if (WallTypesHelper.wallDiagonalCornerUpRight.Contains(typeAsInt))
            {
                tileBase = AddressableIds.Wall_Diagonal_Corner_Up_Right;
            }
            else if (WallTypesHelper.wallDiagonalCornerUpLeft.Contains(typeAsInt))
            {
                tileBase = AddressableIds.Wall_Diagonal_Corner_Up_Left;
            }
            else if (WallTypesHelper.wallFullEightDirections.Contains(typeAsInt))
            {
                tileBase = AddressableIds.Wall_Full;
            }
            else if (WallTypesHelper.wallBottmEightDirections.Contains(typeAsInt))
            {
                tileBase = AddressableIds.Wall_Bottom;
            }
            if (string.IsNullOrEmpty(tileBase))
            {
                return;
            }
            try
            {
                TileBase tile = await _assetService.LoadAssetAsync<TileBase>(tileBase);
                if (tile != null)
                {
                    PaintSingleTile(_wallTilemap, tile, position);
                }
                else
                {
                    Debug.LogError($"tile is null addressable Id: {tileBase}");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"error in loading addressable Id: {tileBase} \n {ex}");
                return;
            }
        }
    }
}