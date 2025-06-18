using System.Collections;
using UnityEngine;

namespace Assets.ProjectAI.Scripts.DungeonScripts
{
    public class DungeonMapController : MonoBehaviour
    {
        [SerializeField]
        private RoomFirstDungeonGenerator roomFirstDungeonGenerator;
        // Use this for initialization
        void Start()
        {
            roomFirstDungeonGenerator.GenerateDungeon();
        }

    }
}