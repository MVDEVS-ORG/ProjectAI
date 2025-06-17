using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.ProjectAI.Scripts.DungeonScripts.RoomSystem.Items
{
    public class Item : MonoBehaviour
    {
        [SerializeField]
        private SpriteRenderer _spriteRenderer;
        [SerializeField]
        private Collider2D _itemCollider;

        [SerializeField]
        int health = 3;
        [SerializeField]
        bool nonDestructible;

        [SerializeField]
        private GameObject hitFeedback, destroyFeedback;

        public UnityEvent OnGetHit {  get; private set; }

        public void Initialize(ItemData itemData)
        {

        }
    }
}