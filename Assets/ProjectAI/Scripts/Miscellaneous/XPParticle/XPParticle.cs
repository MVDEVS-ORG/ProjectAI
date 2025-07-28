using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class XPParticle : MonoBehaviour
{
    public bool MoveToPlayer = false;
    private CharacterView _playerView;
    private Vector2 _initialPos;
    private ObjectPoolManager _objectPool;
    private Rigidbody2D _rb;
    private int _xp;

    public void Initialize(ObjectPoolManager poolManager, int xp)
    {
        _xp = xp;
        _objectPool = poolManager;
        _rb = GetComponent<Rigidbody2D>();
        _rb.linearDamping = Random.Range(0.5f,0.8f);
        _rb.AddForce(Random.insideUnitCircle * Random.Range(5, 10));
        _rb.gravityScale = 0f;
    }

    public void CollectParticle(CharacterView player)
    {
        _playerView = player;
        MoveToPlayer = true;
        _initialPos = transform.position;
        StartCoroutine(MoveTowardsPlayer());
    }

    IEnumerator MoveTowardsPlayer()
    {
        float timer = 0;
        while(timer<1)
        {
            transform.position = Vector2.Lerp(_initialPos, _playerView.transform.position, timer);
            timer += Time.deltaTime;
            yield return Awaitable.EndOfFrameAsync();
        }
        transform.position = Vector2.Lerp(_initialPos, _playerView.transform.position,1);
        _playerView.AddXP(_xp);
        ResetParticle();
        _objectPool.ReleaseGameObject(gameObject,ObjectPoolManager.PoolType.ParticleSystems);
    }

    private void ResetParticle()
    {
        MoveToPlayer = false;
        _initialPos = Vector2.zero;
        _xp = 0;
    }
}
