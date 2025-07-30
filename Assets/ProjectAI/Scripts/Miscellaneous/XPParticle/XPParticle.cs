using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class XPParticle : MonoBehaviour
{
    [HideInInspector]public bool MoveToPlayer = false;
    private CharacterView _playerView;
    private Vector2 _initialPos;
    private ObjectPoolManager _objectPool;
    private Rigidbody2D _rb;
    private int _xp;

    [Range(0, 1)][SerializeField] private float _linearDampingMin;
    [Range(0, 1)][SerializeField] private float _linearDampingMax;
    [Range(0, 10)][SerializeField] private float _initialForceMin;
    [Range(0, 10)][SerializeField] private float _initialForceMax;

    public void Initialize(ObjectPoolManager poolManager, int xp)
    {
        _xp = xp;
        _objectPool = poolManager;
        _rb = GetComponent<Rigidbody2D>();
        _rb.linearDamping = Random.Range(_linearDampingMin, _linearDampingMax); // 0.5,0.8 tested values
        _rb.AddForce(Random.insideUnitCircle * Random.Range(_initialForceMin, _initialForceMax)); //5,10 tested values
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
