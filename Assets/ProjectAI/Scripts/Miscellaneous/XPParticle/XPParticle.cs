using System.Collections;
using UnityEngine;

public class XPParticle : MonoBehaviour
{
    public bool MoveToPlayer = false;
    private Transform _playerTransform;
    private Vector2 _initialPos;
    private ObjectPoolManager _objectPool;

    public void Initialize(ObjectPoolManager poolManager)
    {
        _objectPool = poolManager;
    }

    public void CollectParticle(Transform player)
    {
        _playerTransform = player;
        MoveToPlayer = true;
        _initialPos = transform.position;
        StartCoroutine(MoveTowardsPlayer());
    }

    IEnumerator MoveTowardsPlayer()
    {
        float timer = 0;
        while(timer<1)
        {
            transform.position = Vector2.Lerp(_initialPos, _playerTransform.position, timer);
            timer += Time.deltaTime;
            yield return Awaitable.EndOfFrameAsync();
        }
        transform.position = Vector2.Lerp(_initialPos, _playerTransform.position,1);
        _objectPool.ReleaseGameObject(gameObject,ObjectPoolManager.PoolType.ParticleSystems);
    }
}
