using System.Collections;
using UnityEngine;

namespace Assets.ProjectAI.Scripts.EnemyScripts.Bosses
{
    public class NearDistanceDamageDealer : MonoBehaviour
    {
        public int damage = 10;
        private void OnTriggerEnter2D(Collider2D collision)
        {
            DealDamageToTarget(collision);
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            DealDamageToTarget(collision);
        }

        private void DealDamageToTarget(Collider2D collision)
        {
            if (collision.transform.CompareTag("Player"))
            {
                var playerHealth = collision.GetComponent<CharacterView>();
                playerHealth?.TakeDamage(damage, transform.position, 1.2f);
            }
        }
    }
}