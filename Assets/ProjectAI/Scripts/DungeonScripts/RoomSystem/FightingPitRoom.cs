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
        [SerializeField]
        private PrefabPlacer prefabPlacer;

        public List<EnemyPlacementData> enemyPlacementData;
        public List<ItemPlacementData> itemData;

        public override async Awaitable<List<GameObject>> ProcessRoom(
            Vector2Int roomCenter,
            HashSet<Vector2Int> roomFloor,
            HashSet<Vector2Int> roomFloorNoCorridors,
            IAssetService assetService)
        {
            return await ProcessRoom(roomCenter, roomFloor, roomFloorNoCorridors, assetService, null, null);
        }

        public override async Awaitable<List<GameObject>> ProcessRoom(
               Vector2Int roomCenter,
               HashSet<Vector2Int> roomFloor,
               HashSet<Vector2Int> roomFloorNoCorridors,
               IAssetService assetService,
               Transform playerTransform,
                ObjectPoolManager opManager
           )
        {
            var itemPlacementHelper = new ItemPlacementHelper(roomFloor, roomFloorNoCorridors);

            Transform characterView = playerTransform;

            var placedObjects = await prefabPlacer.PlaceAllItems(itemData, itemPlacementHelper, assetService);
            /*placedObjects.AddRange(await prefabPlacer.PlaceEnemies(enemyPlacementData, itemPlacementHelper, assetService, characterView));*/
            var enemySpawnerObj = await assetService.InstantiateWithPRAsync(AddressableIds.EnemySpawner, (Vector3)itemPlacementHelper.GetItemPlacementPosition(PlacementType.OpenSpace, 1, Vector2Int.one, false), Quaternion.identity);
            placedObjects.Add(enemySpawnerObj);
            var enemySpawner = enemySpawnerObj.GetComponent<EnemySpawner>();
            enemySpawner.InitializeSpawner(opManager, enemyPlacementData, itemPlacementHelper, playerTransform);

            return placedObjects;
        }
    }
}