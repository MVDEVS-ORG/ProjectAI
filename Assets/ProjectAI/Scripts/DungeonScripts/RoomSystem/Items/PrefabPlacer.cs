using Assets.ProjectAI.Scripts.HelperClasses;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Properties;
using UnityEngine;
using Zenject;

namespace Assets.ProjectAI.Scripts.DungeonScripts.RoomSystem.Items
{
    public class PrefabPlacer : MonoBehaviour
    {
        [SerializeField]
        private GameObject _itemPrefab;

        public GameObject CreateObject(GameObject prefab, Vector2 placementPosition)
        {
            if(prefab == null)
            {
                return null;
            }
            GameObject newItem;

            newItem = Instantiate(prefab, placementPosition, Quaternion.identity);

            return newItem;
        }

        public List<GameObject> PlaceAllItems(List<ItemPlacementData> itemData, ItemPlacementHelper itemPlacementHelper)
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
                        placedObjects.Add(PlaceItem(placementData.itemData, possiblePlacementSpot.Value));
                    }
                }
            }
            return placedObjects;
        }

        public List<GameObject> PlaceEnemies(List<EnemyPlacementData> enemyPlacementData, ItemPlacementHelper itemPlacementHelper)
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

                        placedObjects.Add(CreateObject(placementData.enemyPrefab, possiblePlacementSpot.Value + new Vector2(0.5f, 0.5f))); //Instantiate(placementData.enemyPrefab,possiblePlacementSpot.Value + new Vector2(0.5f, 0.5f), Quaternion.identity)
                    }
                }
            }
            return placedObjects;
        }

        private GameObject PlaceItem(ItemData itemData, Vector2 value)
        {
            GameObject newItem = CreateObject(_itemPrefab, value);
            newItem.GetComponent<Item>().Initialize(itemData);
            return newItem;
        }
    }
}