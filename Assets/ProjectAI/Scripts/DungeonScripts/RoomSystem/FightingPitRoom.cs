using Assets.ProjectAI.Scripts.DungeonScripts.RoomSystem.Items;
using Assets.ProjectAI.Scripts.EnemyScripts;
using Assets.ProjectAI.Scripts.HelperClasses;
using Assets.ProjectAI.Scripts.PathFinding;
using Assets.Services;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.ProjectAI.Scripts.DungeonScripts.RoomSystem
{
    public class FightingPitRoom : RoomGenerator
    {
        public List<PlacementDataList> placementData;

        public override async Awaitable<List<GameObject>> ProcessRoom(
            Vector2Int roomCenter,
            HashSet<Vector2Int> roomFloor,
            HashSet<Vector2Int> roomFloorNoCorridors,
            IAssetService assetService,
            DungeonData dungeonData)
        {
            return await ProcessRoom(roomCenter, roomFloor, roomFloorNoCorridors, assetService, null, null, dungeonData);
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
            var itemPlacementHelper = new ItemPlacementHelper(roomFloor, roomFloorNoCorridors, dungeonData);

            Transform characterView = playerTransform;

            var placedObjects = await PrefabPlacer.PlaceAllItems(placementData[dungeonData.currentDungeonLevel].items, itemPlacementHelper, assetService);
            /*placedObjects.AddRange(await prefabPlacer.PlaceEnemies(enemyPlacementData, itemPlacementHelper, assetService, characterView));*/
            var enemySpawnerObj = await assetService.InstantiateWithPRAsync(AddressableIds.Enemy_Spawner, (Vector3)itemPlacementHelper.GetItemPlacementPosition(PlacementType.OpenSpace, 1, Vector2Int.one, false), Quaternion.identity);
            placedObjects.Add(enemySpawnerObj);
            var enemySpawner = enemySpawnerObj.GetComponent<EnemySpawner>();
            enemySpawner.InitializeSpawner(opManager, placementData[dungeonData.currentDungeonLevel].enemies, itemPlacementHelper, playerTransform);

            return placedObjects;
        }
    }
}