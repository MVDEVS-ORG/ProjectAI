using UnityEngine;

public class ShotgunBlast : MonoBehaviour, IGunProjectileBehavior
{
    private ObjectPoolManager _objectPoolManager;
    [SerializeField] private GunProjectileSO _projectileProperties;
    [SerializeField] private Animator _animator;

    void IGunProjectileBehavior.AddModifications()
    { 

    }

    void IGunProjectileBehavior.DestroyManally()
    {

    }

    void IGunProjectileBehavior.Initialize(ObjectPoolManager objectPoolManager)
    {
        _objectPoolManager = objectPoolManager;
    }

    void IGunProjectileBehavior.MoveProjectile(Vector2 Direction)
    {

    }

    void IGunProjectileBehavior.SpawnProjectileAnimation()
    {
        _animator.Play("Blast");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //create system to deal damage to enemies
        if (collision.transform.TryGetComponent<IHealthSystem>(out IHealthSystem health))
        {
            health.TakeDamage(_projectileProperties.Damage);
        }
    }

    public void DisableBlast()
    {
        _objectPoolManager.ReleaseGameObject(gameObject, ObjectPoolManager.PoolType.GameObjects);
    }
}
