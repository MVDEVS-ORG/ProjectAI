using Assets.ProjectAI.Scripts.DungeonScripts.RoomSystem.Items;
using Assets.ProjectAI.Scripts.HelperClasses;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.ProjectAI.Scripts.DungeonScripts.RoomSystem
{
    public class PlayerRoom : RoomGenerator
    {
        public GameObject player;

        public List<ItemPlacementData> itemData;

        [SerializeField]
        private PrefabPlacer _prefabPlacer;


        private Vector2 _playerSpawnPoint = Vector2.zero;

        public override List<GameObject> ProcessRoom(Vector2Int roomCenter, HashSet<Vector2Int> roomFloor, HashSet<Vector2Int> roomFloorNoCorridors)
        {
            ItemPlacementHelper itemPlacementHelper = new ItemPlacementHelper(roomFloor,roomFloorNoCorridors);
            Debug.LogError($"ItemPlacementDatas: {itemData.Count}");
            List<GameObject> placedObjects = _prefabPlacer.PlaceAllItems(itemData, itemPlacementHelper);

            Vector2Int playerSpawnPoint = roomCenter;
            _playerSpawnPoint = roomCenter;
            //GameObject playerObject =
                //_prefabPlacer.CreateObject(player, playerSpawnPoint + new Vector2(0.5f, 0.5f));
            //placedObjects.Add(playerObject);
            return placedObjects;
        }

        public Vector3 GetPlayerSpawnLocation()
        {
            if(_playerSpawnPoint!=Vector2.zero)
            {
                return _playerSpawnPoint;
            }
            Debug.LogError("Player spawn point is not set for some reason.");
            return _playerSpawnPoint;
        }
    }

    public abstract class PlacementData
    {
        [Min(0)]
        public int minQuantity = 0;
        [Min(0)]
        [Tooltip("Max is Inclusive")]
        public int maxQuantity = 0;
        public int Quantity => UnityEngine.Random.Range(minQuantity, maxQuantity + 1);
    } 
    [Serializable]
    public class ItemPlacementData : PlacementData
    {
        public ItemData itemData;
    }

    [Serializable]
    public class EnemyPlacementData: PlacementData
    {
        public GameObject enemyPrefab;
        public Vector2Int enemySize = Vector2Int.one;
    }
}