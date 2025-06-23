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

        public void GenerateRoomContent(DungeonData dungeonData)
        {
            Debug.LogError("Generating Room Content");
            foreach (GameObject obj in spawnedObjects)
            {
                DestroyImmediate(obj);
            }
            spawnedObjects.Clear();
            SelectPlayerSpawnPoint(dungeonData);
            SelectEnemySpawnPoint(dungeonData);

            foreach (GameObject obj in spawnedObjects)
            {
                if (obj != null)
                    obj.transform.SetParent(itemParent, false);
            }
        }

        private void SelectEnemySpawnPoint(DungeonData dungeonData)
        {
            foreach (KeyValuePair<Vector2Int, HashSet<Vector2Int>> roomData in dungeonData.roomsDictionary)
            {
                spawnedObjects.AddRange(
                    defaultRoom.ProcessRoom(
                        roomData.Key,
                        roomData.Value,
                        dungeonData.GetRoomFloorwithoutCorridors(roomData.Key)
                        )
                );

            }
        }

        private void SelectPlayerSpawnPoint(DungeonData dungeonData)
        {
            int randomRoomIndex = Random.Range(0, dungeonData.roomsDictionary.Count);
            Vector2Int playerSpawnPoint = dungeonData.roomsDictionary.Keys.ElementAt(randomRoomIndex);

            graphTest.RunDijkstraAlgorithm(playerSpawnPoint, dungeonData.floorPositions);

            Vector2Int roomIndex = dungeonData.roomsDictionary.Keys.ElementAt(randomRoomIndex);
            List<GameObject> placedPrefabs = playerRoom.ProcessRoom(
                playerSpawnPoint,
                dungeonData.roomsDictionary.Values.ElementAt(randomRoomIndex),
                dungeonData.GetRoomFloorwithoutCorridors(roomIndex)
                );
             Vector2 spawnPosition = (playerRoom as PlayerRoom).GetPlayerSpawnLocation();

            _playerController.SpawnPlayer(spawnPosition, _playerPicker.PickPlayer());
            spawnedObjects.AddRange( placedPrefabs );
            dungeonData.roomsDictionary.Remove(playerSpawnPoint);
        }
    }

}