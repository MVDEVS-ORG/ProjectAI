using Assets.ProjectAI.Scripts.DungeonScripts.DecisionSystem;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.ProjectAI.Scripts.DungeonScripts
{
    public class RoomContentGeneration : MonoBehaviour
    {
        [SerializeField]
        private RoomGenerator playerRoom, defaultRoom;

        List<GameObject> spawnedObjects = new List<GameObject>();
        [SerializeField]
        private GraphTest graphTest;

        public Transform itemParent;

        public void GenerateRoomContent(DungeonData dungeonData)
        {
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
            spawnedObjects.AddRange( placedPrefabs );
            dungeonData.roomsDictionary.Remove(playerSpawnPoint);
        }
    }

}
public abstract class RoomGenerator : MonoBehaviour
{
    public abstract List<GameObject> ProcessRoom(
        Vector2Int roomCenter,
        HashSet<Vector2Int> roomFloor,
        HashSet<Vector2Int> corridors);
}