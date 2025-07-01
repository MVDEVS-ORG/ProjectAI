using Assets.ProjectAI.Scripts.DungeonScripts.RoomSystem.Items;
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
            return await ProcessRoom(roomCenter, roomFloor, roomFloorNoCorridors, assetService, null);
        }

        public override async Awaitable<List<GameObject>> ProcessRoom(
               Vector2Int roomCenter,
               HashSet<Vector2Int> roomFloor,
               HashSet<Vector2Int> roomFloorNoCorridors,
               IAssetService assetService,
               Transform playerTransform
           )
        {
            var itemPlacementHelper = new ItemPlacementHelper(roomFloor, roomFloorNoCorridors);

            Transform characterView = playerTransform;

            var placedObjects = await prefabPlacer.PlaceAllItems(itemData, itemPlacementHelper, assetService);
            while (!PathFindingManager.Instance.isBaked)
            {
                await Awaitable.EndOfFrameAsync();
            }
            placedObjects.AddRange(await prefabPlacer.PlaceEnemies(enemyPlacementData, itemPlacementHelper, assetService, characterView));

            return placedObjects;
        }
    }
}