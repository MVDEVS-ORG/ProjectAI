using Assets.ProjectAI.Scripts.DungeonScripts.RoomSystem.Items;
using Assets.ProjectAI.Scripts.HelperClasses;
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
        public override List<GameObject> ProcessRoom(Vector2Int roomCenter, HashSet<Vector2Int> roomFloor, HashSet<Vector2Int> roomFloorNoCorridors)
        {
            ItemPlacementHelper itemPlacementHelper =
            new ItemPlacementHelper(roomFloor, roomFloorNoCorridors);
            List<GameObject> placedObjects =
                prefabPlacer.PlaceAllItems(itemData, itemPlacementHelper);

            placedObjects.AddRange(prefabPlacer.PlaceEnemies(enemyPlacementData, itemPlacementHelper));

            return placedObjects;
        }
    }
}