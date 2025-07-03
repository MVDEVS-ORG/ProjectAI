using System.Collections;
using UnityEngine;

public class SimpleBullet : MonoBehaviour , IGunProjectileBehavior
{

    private bool _motion = false;
    private Vector3 _direction = Vector3.zero;
    public GunProjectileSO ProjectileProperties;
    private ObjectPoolManager _objectPoolManager;

    void IGunProjectileBehavior.AddModifications()
    {
        //nothing for this
    }

    void IGunProjectileBehavior.DestroyManally()
    {
        //not required
    }

    void IGunProjectileBehavior.MoveProjectile(Vector2 direction)
    {
        _direction = direction;
        StartCoroutine(DestroyProjectile());
        _motion = true;
    }

    void IGunProjectileBehavior.SpawnProjectileAnimation()
    {
        //nothing here
        gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if(_motion)
        {
            transform.position = transform.position + _direction * ProjectileProperties.ProjectileSpeed * Time.deltaTime;
        }
    }

    IEnumerator DestroyProjectile()
    {
        yield return Awaitable.WaitForSecondsAsync(ProjectileProperties.ProjectileDuration);
        _objectPoolManager.ReleaseGameObject(gameObject, ObjectPoolManager.PoolType.GameObjects);
    }

    void IGunProjectileBehavior.Initialize(ObjectPoolManager objectPoolManager)
    {
        _objectPoolManager = objectPoolManager;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //create system to deal damage to enemies
        if(collision.transform.TryGetComponent<IHealthSystem>(out IHealthSystem health))
        {
            health.TakeDamage(ProjectileProperties.Damage);
        }
        _objectPoolManager.ReleaseGameObject(gameObject,ObjectPoolManager.PoolType.GameObjects);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //create system to deal damage to enemies
        if (collision.transform.TryGetComponent<IHealthSystem>(out IHealthSystem health))
        {
            health.TakeDamage(ProjectileProperties.Damage);
        }
        _objectPoolManager.ReleaseGameObject(gameObject, ObjectPoolManager.PoolType.GameObjects);
    }
}
