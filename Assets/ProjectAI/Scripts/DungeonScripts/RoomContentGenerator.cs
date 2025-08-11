using Assets.ProjectAI.Scripts.DungeonScripts.DecisionSystem;
using Assets.ProjectAI.Scripts.DungeonScripts.RoomSystem;
using Assets.ProjectAI.Scripts.DungeonScripts.RoomSystem.Items;
using Assets.ProjectAI.Scripts.HelperClasses;
using Assets.ProjectAI.Scripts.Player;
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
        [Inject] private ObjectPoolManager _objectPoolManager;
        [Inject] private PlayerSelectionService _playerSelectionService;

        [SerializeField]
        private RoomGenerator _playerRoom, _defaultRoom, _treasureRoom, _portalRoom;
        [SerializeField] GameObject _doorPrefab;
        List<GameObject> spawnedObjects = new List<GameObject>();
        [SerializeField]
        private GraphTest graphTest;

        public Transform itemParent;
        private Vector2Int _playerSpawnPoint;
        public async Awaitable<List<Item>> GenerateRoomContent(DungeonData dungeonData)
        {
            foreach (GameObject obj in spawnedObjects)
            {
                DestroyImmediate(obj);
            }
            spawnedObjects.Clear();
            await SelectPlayerSpawnPoint(dungeonData);
            await SelectEnemySpawnPoint(dungeonData);
            await SpawnSpecialRoomContent(dungeonData);
            /*foreach (var doorPos in dungeonData.doorPositions)
            {
                if (_doorPrefab != null)
                {
                    Vector3 spawnWorldPos = new Vector3(doorPos.x + 0.5f, doorPos.y + 0.5f, 0); // center on tile
                    GameObject doorInstance = GameObject.Instantiate(_doorPrefab, spawnWorldPos, Quaternion.identity);
                    spawnedObjects.Add(doorInstance);
                }
            }*/

            foreach (GameObject obj in spawnedObjects)
            {
                if (obj != null)
                    obj.transform.SetParent(itemParent, false);
            }
            List<Item> spawnedItem = new List<Item>();
            spawnedObjects.ForEach(
                item =>
                {
                    var itemComponent = item.GetComponent<Item>();
                    if ( itemComponent!= null)
                    {
                        spawnedItem.Add(itemComponent);
                    }
                }
                );
            return spawnedItem;
        }

        private async Awaitable SelectEnemySpawnPoint(DungeonData dungeonData)
        {
            var playerTransform = await _playerController.GetPlayerTransform();
            var roomDictionary = new Dictionary<Vector2Int, HashSet<Vector2Int>>(dungeonData.roomsDictionary);
            if(dungeonData.treasureRoomCenter.HasValue) roomDictionary.Remove(dungeonData.treasureRoomCenter.Value);
            if(dungeonData.bossRoomCenter.HasValue) roomDictionary.Remove(dungeonData.bossRoomCenter.Value);
            roomDictionary.Remove(_playerSpawnPoint);
            foreach (KeyValuePair<Vector2Int, HashSet<Vector2Int>> roomData in roomDictionary)
            {
                var roomObjects = await _defaultRoom.ProcessRoom(
                    roomData.Key,
                    roomData.Value,
                    dungeonData.GetRoomFloorwithoutCorridors(roomData.Key),
                    _assetService,
                    playerTransform,
                    _objectPoolManager
                );
                spawnedObjects.AddRange(
                    roomObjects
                );

            }
        }

        private async Awaitable SpawnSpecialRoomContent(DungeonData dungeonData)
        {
            var playerTransform = await _playerController.GetPlayerTransform();
            if (dungeonData.bossRoomCenter.HasValue)
            {
                Debug.Log("Spawning boss room");
                var bossCenter = dungeonData.bossRoomCenter.Value;
                var bossRoomFloor = dungeonData.GetRoomFloorwithoutCorridors(bossCenter);
                var bossRoom = dungeonData.roomsDictionary[bossCenter];

                var bossRoomObjects = await _portalRoom.ProcessRoom(bossCenter, bossRoom, bossRoomFloor, _assetService, playerTransform, _objectPoolManager);
                spawnedObjects.AddRange(bossRoomObjects);
            }

            if (dungeonData.treasureRoomCenter.HasValue)
            {
                Debug.Log("Spawning treasure room");
                var treasureCenter = dungeonData.treasureRoomCenter.Value;
                var treasureRoomFloor = dungeonData.GetRoomFloorwithoutCorridors(treasureCenter);
                var treasureRoom = dungeonData.roomsDictionary[treasureCenter];

                var treasureRoomObjects = await _treasureRoom.ProcessRoom(treasureCenter, treasureRoom, treasureRoomFloor, _assetService, playerTransform, _objectPoolManager);
                spawnedObjects.AddRange(treasureRoomObjects);
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

            // Run Dijkstra from player room
            var dijkstraFromPlayer = graphTest.RunDijkstraAlgorithm(playerSpawnPoint, dungeonData.floorPositions);

            // Find boss room (farthest from player room)
            var roomDistances = new List<(Vector2Int center, int distance)>();
            foreach (var room in dungeonData.roomsDictionary)
            {
                int minDistance = int.MaxValue;
                foreach (var tile in room.Value)
                {
                    if (dijkstraFromPlayer.TryGetValue(tile, out int dist))
                        minDistance = Mathf.Min(minDistance, dist);
                }
                roomDistances.Add((room.Key, minDistance));
            }

            roomDistances.Sort((a, b) => b.distance.CompareTo(a.distance));
            dungeonData.bossRoomCenter = roomDistances[0].center;

            // Now run Dijkstra from boss room
            var dijkstraFromBoss = graphTest.RunDijkstraAlgorithm(dungeonData.bossRoomCenter.Value, dungeonData.floorPositions);

            // Find treasure room farthest from both player room and boss room
            Vector2Int? bestTreasureRoom = null;
            int maxCombinedDistance = int.MinValue;

            foreach (var room in dungeonData.roomsDictionary)
            {
                if (room.Key == playerSpawnPoint || room.Key == dungeonData.bossRoomCenter.Value)
                    continue;

                int minDistToPlayer = int.MaxValue;
                int minDistToBoss = int.MaxValue;

                foreach (var tile in room.Value)
                {
                    if (dijkstraFromPlayer.TryGetValue(tile, out int distToPlayer))
                        minDistToPlayer = Mathf.Min(minDistToPlayer, distToPlayer);

                    if (dijkstraFromBoss.TryGetValue(tile, out int distToBoss))
                        minDistToBoss = Mathf.Min(minDistToBoss, distToBoss);
                }

                int combined = minDistToPlayer + minDistToBoss;
                if (combined > maxCombinedDistance)
                {
                    maxCombinedDistance = combined;
                    bestTreasureRoom = room.Key;
                }
            }

            if (bestTreasureRoom.HasValue)
            {
                dungeonData.treasureRoomCenter = bestTreasureRoom.Value;
            }

            // Finalize player room content
            Vector2Int roomIndex = playerSpawnPoint;
            List<GameObject> placedPrefabs = await _playerRoom.ProcessRoom(
                playerSpawnPoint,
                dungeonData.roomsDictionary[roomIndex],
                dungeonData.GetRoomFloorwithoutCorridors(roomIndex),
                _assetService
            );

            Vector2 spawnPosition = (_playerRoom as PlayerRoom).GetPlayerSpawnLocation();

            Character selected = _playerSelectionService.SelectedCharacter;
            await _playerController.SpawnPlayer(spawnPosition, _playerPicker.SelectPlayer(selected));
            spawnedObjects.AddRange(placedPrefabs);
            _playerSpawnPoint = playerSpawnPoint;
        }

    }

}