using Assets.ProjectAI.Scripts.DungeonScripts.RoomSystem.Items;
using Assets.ProjectAI.Scripts.HelperClasses;
using Assets.ProjectAI.Scripts.PathFinding;
using Assets.Services;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.ProjectAI.Scripts.DungeonScripts.RoomSystem
{
    public class PlayerRoom : RoomGenerator
    {
        public GameObject player;

        public List<PlacementDataList> placementData;

        private Vector2 _playerSpawnPoint = Vector2.zero;

        public override async Awaitable<List<GameObject>> ProcessRoom(Vector2Int roomCenter, HashSet<Vector2Int> roomFloor, HashSet<Vector2Int> roomFloorNoCorridors, IAssetService assetService, DungeonData dungeonData)

        {
            return await ProcessRoom(roomCenter, roomFloor, roomFloorNoCorridors, assetService, null, null, dungeonData);
        }

        public Vector3 GetPlayerSpawnLocation()
        {
            if (_playerSpawnPoint != Vector2.zero)
            {
                Vector2Int spawnCell = Vector2Int.RoundToInt(_playerSpawnPoint);
                Vector2Int validCell = PathFindingManager.Instance.GetNearestValidWalkableTile(spawnCell);
                var resultPos = PathFindingManager.Instance.GetCellCenterWorld((Vector3Int)validCell);
                return resultPos;
            }

            Debug.LogError("Player spawn point is not set for some reason.");
            return Vector3.zero;
        }

        public override async Awaitable<List<GameObject>> ProcessRoom(
            Vector2Int roomCenter,
            HashSet<Vector2Int> roomFloor,
            HashSet<Vector2Int> roomFloorNoCorridors,
            IAssetService assetService,
            Transform playerTransform,
            ObjectPoolManager opManager,
            DungeonData dungeonData
        )
        {
            ItemPlacementHelper itemPlacementHelper = new ItemPlacementHelper(roomFloor, roomFloorNoCorridors, dungeonData);
            List<GameObject> placedObjects = await PrefabPlacer.PlaceAllItems(placementData[dungeonData.currentDungeonLevel].items, itemPlacementHelper, assetService);

            Vector2Int playerSpawnPoint = roomCenter;
            _playerSpawnPoint = roomCenter;
            return placedObjects;
        }
    }

    public abstract class PlacementData
    {
        [Min(0)]
        public int minQuantity = 0;
        [Min(0)]
        [Tooltip("Max is Inclusive")]
        public int maxQuantity = 0;
        public float spawnChance = 1f; // 0 to 1
        public int Quantity => UnityEngine.Random.Range(minQuantity, maxQuantity + 1);
    } 
    [Serializable]
    public class ItemPlacementData : PlacementData
    {
        public ItemData itemData;
    }

    [Serializable]
    public class PlacementDataList
    {
        public List<ItemPlacementData> items = new List<ItemPlacementData>();
        public List<EnemyPlacementData> enemies = new List<EnemyPlacementData>();
    }

    [Serializable]
    public class EnemyPlacementData: PlacementData
    {
        public EnemyDataSO enemyData;
        public Vector2Int enemySize = Vector2Int.one;
    }
}