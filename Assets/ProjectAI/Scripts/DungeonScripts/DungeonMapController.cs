using Assets.ProjectAI.Scripts.DungeonScripts.Data;
using Assets.ProjectAI.Scripts.GameController;
using Assets.ProjectAI.Scripts.HelperClasses;
using Assets.ProjectAI.Scripts.PathFinding;
using Assets.Services;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

namespace Assets.ProjectAI.Scripts.DungeonScripts
{
    public class DungeonMapController : MonoBehaviour
    {
        [SerializeField]
        private RoomFirstDungeonGenerator _roomFirstDungeonGenerator;
        [SerializeField] private TilemapVisualizer _tileMapVisualizer;
        [Inject] private RoomContentGenerator _roomContentGenerator;
        [Inject] private IAssetService _assetService;
        [Inject] private LevelManager _levelManager;

        // Use this for initialization
        public async Awaitable Initialize()
        {
            var tileSet = await _assetService.LoadAssetAsync<TileSetSO>(AddressableIds.TileSet + _levelManager.CurrentLevel.ToString());
            _tileMapVisualizer.SetTileSet(tileSet);
            DungeonData data = await _roomFirstDungeonGenerator.GenerateDungeon();
            data.currentDungeonLevel = _levelManager.CurrentLevel;
            data = await PathFindingManager.Instance.InitialBakeAsync(data);
            data =  _roomFirstDungeonGenerator.DetectDoorPositions(data);
            var roomFloor = new HashSet<Vector2Int>(data.floorPositions.Except(data.corridorPositions));

            var items = await _roomContentGenerator.GenerateRoomContent(data);
            foreach (var item in items)
            {
                data.items.Add(item);
            }
            var isMapBaked = await PathFindingManager.Instance.BakeItemsAsync(data);
            //PathFindingManager.Instance.BakeFromTilemap(data.occupiedPosition);
            if (isMapBaked)
            {
                Debug.Log("Baking Complete");
            }
            else
            {
                Debug.LogError("Baking Failed");
            }
        }

        public List<GameObject> GetAllSpawnedEnemies()
        {
            return _roomContentGenerator.GetSpawnedGameObjects<EnemyAI>();
        }
    }
}