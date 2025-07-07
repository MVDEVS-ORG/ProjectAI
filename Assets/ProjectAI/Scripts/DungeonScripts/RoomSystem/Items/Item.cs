using Assets.ProjectAI.Scripts.PathFinding;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.Universal;

namespace Assets.ProjectAI.Scripts.DungeonScripts.RoomSystem.Items
{
    public class Item : MonoBehaviour, IHealthSystem
    {
        [SerializeField]
        private SpriteRenderer _spriteRenderer;
        [SerializeField]
        private BoxCollider2D _itemCollider;

        [SerializeField]
        int _health = 3;
        int _maxHealth = 3;
        [SerializeField]
        bool _nonDestructible;

        [SerializeField]
        private GameObject _hitFeedback, _destroyFeedback;

        [SerializeField]
        private Light2D _light;
        [SerializeField]
        private ShadowCaster2D _shadowCaster;

        public int Health => _health;

        public int MaxHealth => _maxHealth;

        public void InitializeItemData(ItemData itemData)
        {
            _spriteRenderer.sprite = itemData.sprite;
            _spriteRenderer.transform.localPosition = new Vector2(0.5f * itemData.size.x, 0.5f * itemData.size.y);
            _itemCollider.size = itemData.size;
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
            _health = itemData.health;
            _maxHealth = itemData.maxHealth;
        }

        public void TakeDamage(int damage)
        {
            if (_nonDestructible)
            {
                return;
            }
            _health = Mathf.Clamp(_health - damage, 0, _maxHealth);
            if (_health <= 0)
            {
                Debug.LogError("Item destroyed");
                PathFindingManager.Instance.UnblockItemArea(this);
                Destroy(gameObject);
            }
        }

        public void Heal(int healing)
        {
            //throw new NotImplementedException();
        }

        public void Initialize(HealthModels model)
        {
            
            //_health = MaxHealth;
        }

        public void ResetHealth()
        {
            _health = _maxHealth;
        }
    }
}