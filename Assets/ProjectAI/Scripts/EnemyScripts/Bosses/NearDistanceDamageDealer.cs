using System.Collections;
using UnityEngine;

namespace Assets.ProjectAI.Scripts.EnemyScripts.Bosses
{
    public class NearDistanceDamageDealer : MonoBehaviour
    {
        public int damage = 10;
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.transform.CompareTag("Player"))
            {
                var playerHealth = collision.GetComponent<CharacterView>();
                var direction = (collision.transform.position - transform.position).normalized;
                playerHealth?.TakeDamage(damage, direction, 0.2f);
            }
        }
    }
}