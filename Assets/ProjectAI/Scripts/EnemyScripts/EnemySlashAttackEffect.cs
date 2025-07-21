using System.Collections;
using UnityEngine;

namespace Assets.ProjectAI.Scripts.EnemyScripts
{
    public class EnemySlashAttackEffect : MonoBehaviour
    {
        public ObjectPoolManager poolManager;
        public int slashDamage = 5;
        public LayerMask playerMask;
        public Transform EnemyTransform;
        bool canDamagePlayer = false;
        private CharacterView playerHealthSystem;
        public void GiveDamage()
        {
            if (canDamagePlayer && playerHealthSystem != null)
            {
                playerHealthSystem.TakeDamage(slashDamage,EnemyTransform.position);
            }
        }

        public void RemoveSlashEffect()
        {
            poolManager.ReleaseGameObject(gameObject, ObjectPoolManager.PoolType.ParticleSystems);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                canDamagePlayer = true;
                if (collision.transform.TryGetComponent(out CharacterView health))
                {
                    playerHealthSystem = health;
                }
            }
        }
        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                canDamagePlayer = false;
                playerHealthSystem = null;
            }
        }
    }
}