using System.Collections;
using UnityEngine;

namespace Assets.ProjectAI.Scripts.EnemyScripts.Bosses
{
    public class LightningAttack : MonoBehaviour
    {

        [Header("Settings")]
        [SerializeField] private float _riseHeight = 5f;          // how high it rises before striking down
        [SerializeField] private float _riseSpeed = 10f;          // speed of upward motion
        [SerializeField] private float _strikeSpeed = 20f;        // speed of downward strike
        [SerializeField] private int damage = 20;                 // damage dealt
        [SerializeField] private float _destroyDelay = 0.5f;      // delay before destroying after hit
        [SerializeField] private float _warningDuration = 1.5f;   // how long the warning is shown

        [Header("Prefabs")]
        [SerializeField] private GameObject _warningIndicatorPrefab;  // warning effect prefab

        private Vector3 _origin;                                  // start position
        private Vector3 _targetPosition;                          // locked player position
        private bool _damageApplied = false;

        public void Fire(Vector3 target)
        {
            _origin = transform.position;
            _targetPosition = target;

            StartCoroutine(LightningRoutine());
        }

        private IEnumerator LightningRoutine()
        {
            // Spawn warning indicator at the target position
            if (_warningIndicatorPrefab)
            {
                GameObject warning = Instantiate(_warningIndicatorPrefab, _targetPosition, Quaternion.identity);
                Destroy(warning, _warningDuration); // auto remove after duration
            }

            // Wait for warning duration before lightning starts
            yield return new WaitForSeconds(_warningDuration);

            // --- Phase 1: rise upward (lock X to target, only Y moves) ---
            Vector3 riseTarget = new Vector3(_targetPosition.x, _origin.y + _riseHeight, _origin.z);
            while (Mathf.Abs(transform.position.y - riseTarget.y) > 0.1f)
            {
                transform.position = Vector3.MoveTowards(
                    transform.position,
                    new Vector3(_targetPosition.x, riseTarget.y, transform.position.z),
                    _riseSpeed * Time.deltaTime
                );
                yield return null;
            }

            // Phase 2: strike down (again lock X to target, move Y down)
            Vector3 strikeTarget = new Vector3(_targetPosition.x, _targetPosition.y, _targetPosition.z);
            while (Mathf.Abs(transform.position.y - strikeTarget.y) > 0.1f)
            {
                transform.position = Vector3.MoveTowards(
                    transform.position,
                    strikeTarget,
                    _strikeSpeed * Time.deltaTime
                );
                yield return null;
            }

            // If it didn’t hit the player, destroy after delay
            Destroy(gameObject, _destroyDelay);
        }



        private void OnTriggerEnter2D(Collider2D other)
        {
            if (_damageApplied) return;

            if (other.CompareTag("Player"))
            {
                var playerHealth = other.GetComponent<CharacterView>();
                playerHealth?.TakeDamage(damage, _origin, 0.2f);
                _damageApplied = true;

                Destroy(gameObject, _destroyDelay); // clean up after hit
            }
        }
    }
}