using System;
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
        private BoxCollider2D _itemCollider;

        [SerializeField]
        int health = 3;
        [SerializeField]
        bool nonDestructible;

        [SerializeField]
        private GameObject hitFeedback, destroyFeedback;

        public UnityEvent OnGetHit {  get; private set; }

        public void Initialize(ItemData itemData)
        {
            _spriteRenderer.sprite = itemData.sprite;
            _spriteRenderer.transform.localPosition = new Vector2(0.5f * itemData.size.x, 0.5f * itemData.size.y);
            _itemCollider.size = itemData.size;
            _itemCollider.offset = _spriteRenderer.transform.localPosition;

            if (itemData.nonDestructible)
            {
                nonDestructible = true;
            }
            this.health = itemData.health;
        }

        public void GetHit(int damage, GameObject damageDealer)
        {
            if (nonDestructible)
            {
                return;
            }
            if (health > 1)
            {
                //Instantiate hit feedback
                //Instantiate(hitFeedback, spriteRenderer.transform.position, Quaternion.identity);
            }
            else
            {
                //Instantiate(destoyFeedback, spriteRenderer.transform.position, Quaternion.identity);
            }
            ReduceHealth();
        }

        private void ReduceHealth()
        {
            health--;
            if (health <= 0)
            {
                //Show Effects
                Destroy(gameObject);
            }
        }
    }
}