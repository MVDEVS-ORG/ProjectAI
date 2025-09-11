using System.Collections;
using System.Net;
using System.Threading;
using UnityEngine;

namespace Assets.ProjectAI.Scripts.EnemyScripts
{
    [RequireComponent(typeof(LineRenderer), typeof(BoxCollider2D))]
    public class EnemyLaserBeam : MonoBehaviour
    {
        [Header("General Settings")]
        [SerializeField] private float _maxWidth = 0.5f;
        [SerializeField] private float _growSpeed = 2f;
        [SerializeField] private float _beamDuration = 0.3f;
        [SerializeField] private float _lockOnendDuration = 0.5f;
        [SerializeField] private LayerMask _collisionMask;
        [SerializeField] private int _damage = 10;

        [Header("References")]
        [SerializeField] private LineRenderer _lineRenderer;
        [SerializeField]  private BoxCollider2D _boxCollider;

        [Header("Gradients")]
        [SerializeField] private Gradient _followGradient;
        [SerializeField] private Gradient _lockGradient;
        [SerializeField] private Gradient _sweepGradient;

        private ObjectPoolManager _pool;
        private Vector3 _origin;
        private Vector3 _direction;
        private bool _damageApplied;
        public bool lockOn = false;
        private Coroutine _trackingCoroutine;
        private Coroutine _growCoroutine;
        private Coroutine _sweepCoroutine;

        #region Lock-On Fire
        public void Fire(Vector3 origin, Transform playerTransform, ObjectPoolManager pool)
        {
            _boxCollider.enabled = false;
            _pool = pool;
            _origin = origin;
            _trackingCoroutine = StartCoroutine(TrackPlayer(playerTransform));
            _growCoroutine = StartCoroutine(LockOn());
        }
        
        private IEnumerator TrackPlayer(Transform playerTansform)
        {
            while(!lockOn)
            {
                _direction = (playerTansform.position - _origin).normalized;

                Vector3 endPoint =_origin + (_direction * 100f);

                _lineRenderer.SetPosition(0, _origin);
                _lineRenderer.SetPosition(1, endPoint);

                _lineRenderer.startWidth = 0.05f;
                _lineRenderer.endWidth = 0.05f;
                _lineRenderer.colorGradient = _followGradient;
               
                transform.position = _origin;
                transform.right = _direction;

                gameObject.SetActive(true);
                yield return Awaitable.EndOfFrameAsync();
            }
            if (lockOn)
            {
                _lineRenderer.colorGradient = _lockGradient;
                yield return Awaitable.WaitForSecondsAsync(_lockOnendDuration);
                _growCoroutine = StartCoroutine(GrowBeamWidth());
            }
        }

        private IEnumerator LockOn()
        {
            yield return new WaitForSeconds(2f);
            lockOn = true;
        }
        private IEnumerator GrowBeamWidth()
        {
            //yield return new WaitForSeconds(_beamDuration);
            _boxCollider.enabled = true;
            float width = 0.05f;
            while (width < _maxWidth)
            {
                width += Time.deltaTime * _growSpeed;
                _lineRenderer.startWidth = width;
                _lineRenderer.endWidth = width;
                _lineRenderer.colorGradient = _lockGradient;
                float length = Vector3.Distance(_origin, _lineRenderer.GetPosition(1));
                Vector2 size = _boxCollider.size;
                size.y = width;
                size.x = length;
                _boxCollider.size = size;
                Vector2 offset = Vector2.zero;
                offset.x = length / 2f;
                _boxCollider.offset = offset;


                yield return null;
            }

            yield return new WaitForSeconds(_beamDuration);
            lockOn = false;
            ResetObject();
        }

        #endregion

        #region Sweep Fire
        public void FireSweep(Vector3 origin, ObjectPoolManager pool, bool leftToRight, bool isPhase2,float sweepDuration, CancellationToken cancellationToken)
        {
            _pool = pool;
            _origin = origin;
            _damageApplied = false;
            gameObject.SetActive(true);

            // Start sweeping coroutine
            _sweepCoroutine = StartCoroutine(SweepLaser(sweepDuration, leftToRight, isPhase2, cancellationToken));
        }

        private IEnumerator SweepLaser(float sweepDuration, bool leftToRight, bool isPhase2, CancellationToken cancellationToken)
        {
            _boxCollider.enabled = true;
            _lineRenderer.colorGradient = _sweepGradient;

            float elapsed = 0f;

            float startAngle;
            float endAngle;

            if (isPhase2)
            {
                // Only sweep halfway (to center 0°)
                sweepDuration /= 2f;

                if (leftToRight)
                {
                    startAngle = 75f;
                    endAngle = 0f;
                }
                else
                {
                    startAngle = -75f;
                    endAngle = 0f;
                }
            }
            else
            {
                // Full sweep
                startAngle = leftToRight ? 75f : -75f;
                endAngle = leftToRight ? -75f : 75f;
            }

            while (elapsed < sweepDuration)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    Interrupt();
                    yield return null;
                }
                elapsed += Time.deltaTime;
                float t = elapsed / sweepDuration;
                float angle = Mathf.Lerp(startAngle, endAngle, t);

                // Direction from angle
                _direction = Quaternion.Euler(0f, 0f, angle) * Vector3.down;
                Vector3 endPoint = _origin + (_direction * 100f);

                // Update line
                _lineRenderer.SetPosition(0, _origin);
                _lineRenderer.SetPosition(1, endPoint);

                _lineRenderer.startWidth = _maxWidth;
                _lineRenderer.endWidth = _maxWidth;

                // Update collider to match beam
                float length = Vector3.Distance(_origin, endPoint);
                Vector2 size = _boxCollider.size;
                size.y = _maxWidth;
                size.x = length;
                _boxCollider.size = size;

                Vector2 offset = Vector2.zero;
                offset.x = length / 2f;
                _boxCollider.offset = offset;

                transform.position = _origin;
                transform.right = _direction;

                yield return null;
            }

            yield return new WaitForSeconds(_beamDuration);

            ResetObject();
        }

        #endregion
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (_damageApplied) return;
            if (other.CompareTag("Player"))
            {
                var playerHealth = other.GetComponent<CharacterView>();
                playerHealth?.TakeDamage(_damage,_origin , 0.2f);
                _damageApplied = true;
            }
        }

        private void OnEnable()
        {
            _damageApplied = false;
        }

        void ResetObject()
        {
            lockOn = false;
            _boxCollider.size = Vector2.one;
            _boxCollider.offset = Vector2.zero;
            _boxCollider.enabled = false;
            _pool.ReleaseGameObject(gameObject, ObjectPoolManager.PoolType.GameObjects);
        }

        public void Interrupt()
        {
            if (_trackingCoroutine != null) StopCoroutine(_trackingCoroutine);
            if (_growCoroutine != null) StopCoroutine(_growCoroutine);
            if (_sweepCoroutine != null) StopCoroutine(_sweepCoroutine);
            ResetObject();
        }
    }


}