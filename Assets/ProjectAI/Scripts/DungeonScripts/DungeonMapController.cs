using Assets.ProjectAI.Scripts.PathFinding;
using System.Collections;
using UnityEngine;

namespace Assets.ProjectAI.Scripts.DungeonScripts
{
    public class DungeonMapController : MonoBehaviour
    {
        [SerializeField]
        private RoomFirstDungeonGenerator roomFirstDungeonGenerator;

        // Use this for initialization
        public async Awaitable Initialize()
        {
            await roomFirstDungeonGenerator.GenerateDungeon();
            var isMapBaked = await PathFindingManager.Instance.BakeMap();
            if (isMapBaked)
            {
                Debug.LogError("Baking Complete");
            }
        }
    }
}