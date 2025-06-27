using Assets.ProjectAI.Scripts.PathFinding;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Assets.ProjectAI.Scripts.DungeonScripts
{
    public class DungeonMapController : MonoBehaviour
    {
        [SerializeField]
        private RoomFirstDungeonGenerator roomFirstDungeonGenerator;
        [Inject] private RoomContentGenerator roomContentGenerator;

        // Use this for initialization
        public async Awaitable Initialize()
        {
            DungeonData data = await roomFirstDungeonGenerator.GenerateDungeon();
            var isMapBaked = await PathFindingManager.Instance.BakeMap(data);
            if (isMapBaked)
            {
                Debug.LogError("Baking 1 Complete");
                isMapBaked = false;
            }
            var items = await roomContentGenerator.GenerateRoomContent(data);
            foreach (var item in items)
            {
                data.items.Add(item);
            }
            isMapBaked = await PathFindingManager.Instance.BakeMap(data);
            if (isMapBaked)
            {
                Debug.LogError("Baking Complete");
            }
        }

        public List<GameObject> GetAllSpawnedEnemies()
        {
            return roomContentGenerator.GetSpawnedGameObjects<EnemyAI>();
        }
    }
}