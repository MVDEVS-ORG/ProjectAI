using Assets.ProjectAI.Scripts.HelperClasses;
using System.Collections;
using UnityEngine;

namespace Assets.ProjectAI.Scripts.DungeonScripts.RoomSystem.Items
{
    [CreateAssetMenu]
    public class ItemData : ScriptableObject
    {
        public Sprite sprite;
        public Vector2Int size = new Vector2Int(1, 1);
        public PlacementType placementType;
        public bool addOffset;
        public int health = 1;
        public bool nonDestructible;
    }
}