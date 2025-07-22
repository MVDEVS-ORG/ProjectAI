using System.Collections;
using System.Net;
using UnityEngine;

namespace Assets.ProjectAI.Scripts.EnemyScripts
{
    [RequireComponent(typeof(LineRenderer), typeof(BoxCollider2D))]
    public class EnemyLaserBeam : MonoBehaviour
    {
        [SerializeField] private float maxWidth = 0.5f;
        [SerializeField] private float growSpeed = 2f;
        [SerializeField] private float beamDuration = 0.3f;
        [SerializeField] private LayerMask collisionMask;
        [SerializeField] private int damage = 10;

        [SerializeField] private LineRenderer _lineRenderer;
        [SerializeField]  private BoxCollider2D _boxCollider;
        [SerializeField] private Gradient _followGradient;
        [SerializeField] private Gradient _lockGradient;
        private ObjectPoolManager _pool;
        private Vector3 _origin;
        private Vector3 _direction;
        private bool _damageApplied;
        private bool _lockOn = false;
        private Coroutine _trackingCoroutine;
        private Coroutine _growCoroutine;
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
            while(!_lockOn)
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
            if (_lockOn)
            {
                _growCoroutine = StartCoroutine(GrowBeamWidth());
            }
        }

        private IEnumerator LockOn()
        {
            yield return new WaitForSeconds(2f);
            _lockOn = true;
        }
        private IEnumerator GrowBeamWidth()
        {
            //yield return new WaitForSeconds(beamDuration);
            _boxCollider.enabled = true;
            float width = 0.05f;
            while (width < maxWidth)
            {
                width += Time.deltaTime * growSpeed;
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

            yield return new WaitForSeconds(beamDuration);
            _lockOn = false;
            ResetObject();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (_damageApplied) return;
            if (other.CompareTag("Player"))
            {
                var playerHealth = other.GetComponent<CharacterView>();
                playerHealth?.TakeDamage(damage);
                _damageApplied = true;
            }
        }

        private void OnEnable()
        {
            _damageApplied = false;
        }

        void ResetObject()
        {
            _lockOn = false;
            _boxCollider.size = Vector2.one;
            _boxCollider.offset = Vector2.zero;
            _boxCollider.enabled = false;
            _pool.ReleaseGameObject(gameObject, ObjectPoolManager.PoolType.ParticleSystems);
        }

        public void Interrupt()
        {
            if (_trackingCoroutine != null) StopCoroutine(_trackingCoroutine);
            if (_growCoroutine != null) StopCoroutine(_growCoroutine);
            ResetObject();
        }
    }


}