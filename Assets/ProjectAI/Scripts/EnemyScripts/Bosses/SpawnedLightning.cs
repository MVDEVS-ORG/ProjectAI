using UnityEngine;

namespace Assets.ProjectAI.Scripts.EnemyScripts.Bosses
{
    public class SpawnedLightning : MonoBehaviour
    {
        [SerializeField] private float _lifeTime = 0.5f;
        private int _damage;
        private ObjectPoolManager _poolManager;
        private GameObject _warningObject;

        public void Initialize(int damage, ObjectPoolManager poolManager, GameObject warningObj)
        {
            _damage = damage;
            _poolManager = poolManager;
            _warningObject = warningObj;

            // destroy after lifetime
            Invoke(nameof(Disappear), _lifeTime);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                var playerHealth = other.GetComponent<CharacterView>();
                playerHealth?.TakeDamage(_damage, transform.position, 0.2f);
            }
        }

        private void Disappear()
        {
            if (_warningObject != null)
                _poolManager.ReleaseGameObject(_warningObject, ObjectPoolManager.PoolType.GameObjects);

            _poolManager.ReleaseGameObject(gameObject, ObjectPoolManager.PoolType.GameObjects);
        }
    }
}
