using System.Collections;
using UnityEngine;

namespace Assets.ProjectAI.Scripts.EnemyScripts.Bosses
{
    public class LightningAttack : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float _riseHeight = 5f;         // how high it rises before disappearing
        [SerializeField] private float _riseSpeed = 10f;         // speed of upward motion
        [SerializeField] private int damage = 20;                // damage dealt by spawned lightning
        [SerializeField] private float _destroyDelay = 0.5f;     // delay before pooling back
        [SerializeField] private float _warningDuration = 1.5f;  // delay before lightning spawns
        [SerializeField] private TrailRenderer _trailRenderer;

        private Vector3 _origin;
        private Vector3 _targetPosition;
        private ObjectPoolManager _poolManager;
        private GameObject _warningObject;

        public async Awaitable Fire(Vector3 target, ObjectPoolManager poolManager)
        {
            _trailRenderer.enabled = false;
            _origin = transform.position;
            _targetPosition = target;
            _poolManager = poolManager;

            // spawn a warning indicator
            _warningObject = await _poolManager.SpawnObjectAsync(
                AddressableIds.Warning_Indicator,
                _targetPosition,
                Quaternion.identity,
                ObjectPoolManager.PoolType.GameObjects
            );

            _ = LightningRoutine();
        }

        private async Awaitable LightningRoutine()
        {
            // Phase 1: rise upwards
            Vector3 riseTarget = new Vector3(transform.position.x, transform.position.y + _riseHeight, transform.position.z);
            _trailRenderer.enabled = true;
            while (Mathf.Abs(transform.position.y - riseTarget.y) > 0.1f)
            {
                transform.position = Vector3.MoveTowards(
                    transform.position,
                    riseTarget,
                    _riseSpeed * Time.deltaTime
                );
                await Awaitable.EndOfFrameAsync();
            }

            // Wait before strike (warning shown)
            await Awaitable.WaitForSecondsAsync(_warningDuration);

            // Spawn lightning at target
            var lightning = await _poolManager.SpawnObjectAsync(
                AddressableIds.Lightning_Strike,
                _targetPosition,
                Quaternion.identity,
                ObjectPoolManager.PoolType.GameObjects
            );

            // Setup lightning damage logic (if it has its own script)
            var lightningScript = lightning.GetComponent<SpawnedLightning>();
            if (lightningScript != null)
            {
                lightningScript.Initialize(damage, _poolManager, _warningObject);
            }
            else
            {
                // fallback: release warning immediately
                _poolManager.ReleaseGameObject(_warningObject, ObjectPoolManager.PoolType.GameObjects);
            }

            // Reset and return this projectile to pool
            Invoke(nameof(DestroySelf), _destroyDelay);
        }

        private void DestroySelf()
        {
            _trailRenderer.enabled = false;
            _poolManager.ReleaseGameObject(gameObject, ObjectPoolManager.PoolType.ParticleSystems);
        }
    }
}
