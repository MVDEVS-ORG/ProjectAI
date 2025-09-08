using System.Collections;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.Rendering.Universal;
using Zenject; // For Light2D

namespace Assets.ProjectAI.Scripts.EnemyScripts.Bosses
{
    public class NovaAttack : MonoBehaviour
    {
        [Header("Nova Settings")]
        [SerializeField] private float _damage = 9999f;
        [SerializeField] private LayerMask _obstacleMask;  // to check if player is behind wall

        [Header("Light Settings")]
        [SerializeField] private Light2D _light2D;
        [SerializeField] private float _maxIntensity = 3f;
        [SerializeField] private float _minIntensity = 1f;
        [SerializeField] private float _maxInnerAngle = 175f;
        [SerializeField] private float _maxOuterRadius = 80f;
        [SerializeField] private float _minOuterRadius = 45f;

        [Header("Collider Settings")]
        [SerializeField] private CircleCollider2D _circleCollider;
        [SerializeField] private float _maxRadius = 60f;
        [SerializeField] private float _minRadius = 1f;

        private Transform _target;
        private SignalBus _signalBus;
        private ObjectPoolManager _poolManager;

        public async Awaitable PlayNova(float chargeTime, float explosionDuration, Transform target, ObjectPoolManager poolManager, SignalBus signalBus)
        {
            _target = target;
            _poolManager = poolManager;
            _signalBus = signalBus;
            // reset
            _light2D.pointLightInnerAngle = 0f;
            _light2D.intensity = _minIntensity;
            _circleCollider.radius = _minRadius;

            //Charging
            float elapsed = 0f;
            while (elapsed < chargeTime)
            {
                elapsed += Time.deltaTime;
                _light2D.pointLightInnerAngle = Mathf.Lerp(0f, _maxInnerAngle, elapsed / chargeTime);
                _light2D.pointLightOuterRadius = Mathf.Lerp(_minOuterRadius, _maxOuterRadius, elapsed / chargeTime);
                await Awaitable.EndOfFrameAsync();
            }

            //Explosion
            _light2D.intensity = _maxIntensity;
            _circleCollider.radius = _maxRadius;
            _signalBus.Fire(new CamEffectsSignal(new CamEffectsSignal.SignalEffect().WithEffect(CamEffect.CamShakeConstant).WithFrequency(1f).WithAmplitude(5f).WithDuration(explosionDuration).WithFadeDuration(1f)));
            float explosionElapsed = 0f;
            while (explosionElapsed < explosionDuration)
            {
                explosionElapsed += Time.deltaTime;

                if (_target != null)
                {
                    Vector2 dir = (_target.position - transform.position).normalized;
                    float dist = Vector2.Distance(transform.position, _target.position);

                    if (dist <= _maxRadius)
                    {
                        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, dist, _obstacleMask);
                        if (!hit) // not behind obstacle
                        {
                            var playerHealth = _target.GetComponent<CharacterView>();
                            if (playerHealth != null)
                            {
                                // Continuous damage every frame (scaled by deltaTime)
                                playerHealth.TakeDamage((int)(_damage * Time.deltaTime), transform.position);
                            }
                        }
                    }
                }

                await Awaitable.EndOfFrameAsync();
            }

            //Reset before returning to pool
            _light2D.pointLightInnerAngle = 0f;
            _light2D.intensity = _minIntensity;
            _circleCollider.radius = _minRadius;

            _poolManager.ReleaseGameObject(gameObject, ObjectPoolManager.PoolType.GameObjects);
        }
    }
}
