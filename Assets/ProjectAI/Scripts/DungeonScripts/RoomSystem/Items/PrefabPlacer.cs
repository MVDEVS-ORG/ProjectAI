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
        public async Awaitable<GameObject> CreateObject(string prefabAddress, Vector2 placementPosition, IAssetService assetService)
        {
            if(prefabAddress == null)
            {
                return null;
            }
            GameObject newItem;
            newItem = await assetService.InstantiateWithPRAsync(prefabAddress, placementPosition, Quaternion.identity);
            return newItem;
        }

        public async Awaitable<List<GameObject>> PlaceAllItems(List<ItemPlacementData> itemData, ItemPlacementHelper itemPlacementHelper, IAssetService assetService)
        {
            List<GameObject> placedObjects = new List<GameObject>();

            IEnumerable<ItemPlacementData> sortedList = new List<ItemPlacementData>(itemData).OrderByDescending(placementData=>
            placementData.itemData.size.x * placementData.itemData.size.y);

            foreach(var placementData in sortedList)
            {
                for(int i = 0; i< placementData.Quantity; i++)
                {
                    Vector2? possiblePlacementSpot = itemPlacementHelper.GetItemPlacementPosition(
                        placementData.itemData.placementType,
                        100,
                        placementData.itemData.size,
                        placementData.itemData.addOffset
                        );
                    if( possiblePlacementSpot.HasValue )
                    {
                        placedObjects.Add(await PlaceItem(placementData.itemData, possiblePlacementSpot.Value, assetService));
                    }
                }
            }
            return placedObjects;
        }

        public async Awaitable<List<GameObject>> PlaceEnemies(ObjectPoolManager opManager, List<EnemyPlacementData> enemyPlacementData, ItemPlacementHelper itemPlacementHelper, Transform characterView)
        {
            List<GameObject> placedObjects = new List<GameObject>();

            foreach (var placementData in enemyPlacementData)
            {
                for (int i = 0; i < placementData.Quantity; i++)
                {
                    Vector2? possiblePlacementSpot = itemPlacementHelper.GetItemPlacementPosition(
                        PlacementType.OpenSpace,
                        100,
                        placementData.enemySize,
                        false
                        );
                    if (possiblePlacementSpot.HasValue)
                    {
                        var go = await opManager.SpawnObjectAsync(placementData.enemyPrefabAddress, possiblePlacementSpot.Value + new Vector2(0.5f, 0.5f), Quaternion.identity, ObjectPoolManager.PoolType.Enemies);
                        placedObjects.Add(go);
                        go.GetComponent<EnemyAI>().InitializeEnemy(characterView, opManager);
                        EnemyManager.spawnedEnemies.Add(go);
                        //Instantiate(placementData.enemyPrefab,possiblePlacementSpot.Value + new Vector2(0.5f, 0.5f), Quaternion.identity)
                    }
                }
            }
            return placedObjects;
        }

        private async Awaitable<GameObject> PlaceItem(ItemData itemData, Vector2 value, IAssetService assetService)
        {
            GameObject newItem = await CreateObject(AddressableIds.Item, value, assetService);
            newItem.GetComponent<Item>().Initialize(itemData);
            return newItem;
        }
    }
}