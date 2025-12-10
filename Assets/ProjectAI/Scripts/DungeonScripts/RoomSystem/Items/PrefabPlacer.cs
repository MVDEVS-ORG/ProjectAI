using Assets.ProjectAI.Scripts.EnemyScripts;
using Assets.ProjectAI.Scripts.HelperClasses;
using Assets.Services;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.ProjectAI.Scripts.DungeonScripts.RoomSystem.Items
{
    public class PrefabPlacer : MonoBehaviour
    {
        public static async Awaitable<GameObject> CreateObject(string prefabAddress, Vector2 placementPosition, IAssetService assetService)
        {
            if(prefabAddress == null)
            {
                return null;
            }
            GameObject newItem;
            newItem = await assetService.InstantiateWithPRAsync(prefabAddress, placementPosition, Quaternion.identity);
            return newItem;
        }

        public static async Awaitable<List<GameObject>> PlaceAllItems(List<ItemPlacementData> itemData, ItemPlacementHelper itemPlacementHelper, IAssetService assetService)
        {
            if (itemData == null) return null;
            List<GameObject> placedObjects = new List<GameObject>();

            IEnumerable<ItemPlacementData> sortedList = new List<ItemPlacementData>(itemData).OrderByDescending(placementData=>
            placementData.itemData.size.x * placementData.itemData.size.y);

            foreach (var placementData in sortedList)
            {
                // Normalize chance: allow either 0..1 values
                float chance = placementData.spawnChance;
                chance = Mathf.Clamp01(chance);

                // Per-instance roll (recommended): each of the Quantity attempts has its own chance
                for (int i = 0; i < placementData.Quantity; i++)
                {
                    if (Random.value < chance) // Random.value returns [0.0, 1.0)
                    {
                        Vector2? possiblePlacementSpot = itemPlacementHelper.GetItemPlacementPosition(placementData.itemData);
                        if (possiblePlacementSpot.HasValue)
                        {
                            placedObjects.Add(
                                await PlaceItem(placementData.itemData, possiblePlacementSpot.Value, assetService));
                        }
                    }
                }
            }
            return placedObjects;
        }

        public static async Awaitable<List<GameObject>> PlaceEnemies(ObjectPoolManager opManager, List<EnemyPlacementData> enemyPlacementData, ItemPlacementHelper itemPlacementHelper, Transform characterView, Transform spawnerTransform)
        {
            List<GameObject> placedObjects = new List<GameObject>();

            foreach (var placementData in enemyPlacementData)
            {
                for (int i = 0; i < placementData.Quantity; i++)
                {
                    Vector2? possiblePlacementSpot = itemPlacementHelper.GetPlacementNearPosition(spawnerTransform.position, searchRadius: 8);

                    if (possiblePlacementSpot.HasValue)
                    {
                        Vector2 position = possiblePlacementSpot.Value + new Vector2(0.5f, 0.5f);
                        var go = await opManager.SpawnObjectAsync(
                            placementData.enemyData.enemyId,
                            position,
                            Quaternion.identity,
                            ObjectPoolManager.PoolType.Enemies
                        );

                        placedObjects.Add(go);
                        go.GetComponent<EnemyAI>().InitializeEnemy(characterView, opManager);
                        EnemyManager.spawnedEnemies.Add(go);
                    }
                }
            }

            return placedObjects;
        }

        private static async Awaitable<GameObject> PlaceItem(ItemData itemData, Vector2 value, IAssetService assetService)
        {
            GameObject newItem = await CreateObject(itemData.itemId, value, assetService);
            var itemDataClass = newItem.GetComponent<Item>();
            itemDataClass.InitializeItemData(itemData);
            return newItem;
        }
    }
}