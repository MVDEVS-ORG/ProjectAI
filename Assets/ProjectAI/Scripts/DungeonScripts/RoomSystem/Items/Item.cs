using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.Universal;

namespace Assets.ProjectAI.Scripts.DungeonScripts.RoomSystem.Items
{
    public class Item : MonoBehaviour
    {
        [SerializeField]
        private SpriteRenderer _spriteRenderer;
        [SerializeField]
        private BoxCollider2D _itemCollider;

        [SerializeField]
        int _health = 3;
        [SerializeField]
        bool _nonDestructible;

        [SerializeField]
        private GameObject _hitFeedback, _destroyFeedback;

        [SerializeField]
        private Light2D _light;
        [SerializeField]
        private ShadowCaster2D _shadowCaster;

        public UnityEvent OnGetHit {  get; private set; }

        public void Initialize(ItemData itemData)
        {
            _spriteRenderer.sprite = itemData.sprite;
            _spriteRenderer.transform.localPosition = new Vector2(0.5f * itemData.size.x, 0.5f * itemData.size.y);
            //_itemCollider.size = itemData.size;
            _itemCollider.offset = _spriteRenderer.transform.localPosition;
            if(itemData.litObject)
            {
                _light.enabled = true;
            }
            else
            {
                _shadowCaster.enabled = true;
            }
            if (itemData.nonDestructible)
            {
                _nonDestructible = true;
            }
            this._health = itemData.health;
        }

        public void GetHit(int damage, GameObject damageDealer)
        {
            if (_nonDestructible)
            {
                return;
            }
            if (_health > 1)
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
            _health--;
            if (_health <= 0)
            {
                //Show Effects
                Destroy(gameObject);
            }
        }
    }
}