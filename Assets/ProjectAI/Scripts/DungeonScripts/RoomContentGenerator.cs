using Assets.ProjectAI.Scripts.DungeonScripts.DecisionSystem;
using Assets.ProjectAI.Scripts.DungeonScripts.RoomSystem;
using Assets.Services;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

namespace Assets.ProjectAI.Scripts.DungeonScripts
{
    public class RoomContentGenerator : MonoBehaviour
    {
        [Inject] private IAssetService _assetService;
        [Inject] private IPlayerController _playerController;
        [Inject] private PlayerPicker _playerPicker;
        [SerializeField]
        private RoomGenerator playerRoom, defaultRoom;

        List<GameObject> spawnedObjects = new List<GameObject>();
        [SerializeField]
        private GraphTest graphTest;

        public Transform itemParent;
        public async Awaitable GenerateRoomContent(DungeonData dungeonData)
        {
            foreach (GameObject obj in spawnedObjects)
            {
                DestroyImmediate(obj);
            }
            spawnedObjects.Clear();
            await SelectPlayerSpawnPoint(dungeonData);
            await SelectEnemySpawnPoint(dungeonData);

            foreach (GameObject obj in spawnedObjects)
            {
                if (obj != null)
                    obj.transform.SetParent(itemParent, false);
            }
        }

        private async Awaitable SelectEnemySpawnPoint(DungeonData dungeonData)
        {
            var playerTransform = await _playerController.GetPlayerTransform();
            foreach (KeyValuePair<Vector2Int, HashSet<Vector2Int>> roomData in dungeonData.roomsDictionary)
            {
                var roomObjects = await defaultRoom.ProcessRoom(
                    roomData.Key,
                    roomData.Value,
                    dungeonData.GetRoomFloorwithoutCorridors(roomData.Key),
                    _assetService,
                    playerTransform
                );
                spawnedObjects.AddRange(
                    roomObjects
                );

            }
        }

        public List<GameObject> GetSpawnedGameObjects<T>()
        {
            var gameObjects = spawnedObjects.FindAll(go => go.GetComponent<T>() != null);
            return gameObjects;
        }

        private async Awaitable SelectPlayerSpawnPoint(DungeonData dungeonData)
        {
            int randomRoomIndex = Random.Range(0, dungeonData.roomsDictionary.Count);
            Vector2Int playerSpawnPoint = dungeonData.roomsDictionary.Keys.ElementAt(randomRoomIndex);

            graphTest.RunDijkstraAlgorithm(playerSpawnPoint, dungeonData.floorPositions);

            Vector2Int roomIndex = dungeonData.roomsDictionary.Keys.ElementAt(randomRoomIndex);
            List<GameObject> placedPrefabs = await playerRoom.ProcessRoom(
                playerSpawnPoint,
                dungeonData.roomsDictionary.Values.ElementAt(randomRoomIndex),
                dungeonData.GetRoomFloorwithoutCorridors(roomIndex),
                _assetService
                );
             Vector2 spawnPosition = (playerRoom as PlayerRoom).GetPlayerSpawnLocation();

            _playerController.SpawnPlayer(spawnPosition, _playerPicker.PickPlayer());
            spawnedObjects.AddRange( placedPrefabs );
            dungeonData.roomsDictionary.Remove(playerSpawnPoint);
        }
    }

}