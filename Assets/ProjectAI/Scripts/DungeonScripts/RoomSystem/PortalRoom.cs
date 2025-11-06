using Assets.ProjectAI.Scripts.DungeonScripts.RoomSystem.Items;
using Assets.ProjectAI.Scripts.EnemyScripts;
using Assets.ProjectAI.Scripts.HelperClasses;
using Assets.Services;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.ProjectAI.Scripts.DungeonScripts.RoomSystem
{
    public class PortalRoom : RoomGenerator
    {

        [SerializeField]
        public List<PlacementDataList> placementData;

        public async override Awaitable<List<GameObject>> ProcessRoom(Vector2Int roomCenter, HashSet<Vector2Int> roomFloor, HashSet<Vector2Int> roomFloorNoCorridors, IAssetService assetService, DungeonData dungeonData)
        {
            return await ProcessRoom(roomCenter, roomFloor, roomFloorNoCorridors, assetService, null, null, dungeonData);
        }

        public async override Awaitable<List<GameObject>> ProcessRoom(Vector2Int roomCenter, HashSet<Vector2Int> roomFloor, HashSet<Vector2Int> roomFloorNoCorridors, IAssetService assetService, Transform playerTransform, ObjectPoolManager opManager, DungeonData dungeonData)
        {
            Room room = new Room(roomCenter, roomFloor, dungeonData.doorPositions);
            room.ProcessRooms();
            var itemPlacementHelper = new ItemPlacementHelper(room);
            dungeonData.Rooms.Add(room);
            Debug.LogError($"Processing Portal Room: {placementData[dungeonData.currentDungeonLevel - 1].items.Count}");
            var placedObjects = await PrefabPlacer.PlaceAllItems(placementData[dungeonData.currentDungeonLevel - 1].items, itemPlacementHelper, assetService);
            var enemySpawnerObj = await assetService.InstantiateWithPRAsync(AddressableIds.Enemy_Spawner, room.RoomCenterPos, Quaternion.identity);
            placedObjects.Add(enemySpawnerObj);
            var enemySpawner = enemySpawnerObj.GetComponent<EnemySpawner>();
            enemySpawner.InitializeSpawner(opManager, placementData[dungeonData.currentDungeonLevel -  1].enemies, itemPlacementHelper, playerTransform);

            return placedObjects;
        }
    }
}