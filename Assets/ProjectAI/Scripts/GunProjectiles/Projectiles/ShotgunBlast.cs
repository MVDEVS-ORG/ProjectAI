using System.Collections.Generic;
using UnityEngine;

public class ShotgunBlast : MonoBehaviour, IGunProjectileBehavior
{
    private ObjectPoolManager _objectPoolManager;
    [SerializeField] private GunProjectileSO _projectileProperties;
    [SerializeField] private Animator _animator;
    private Dictionary<ElementEnum, int> ElementDamage;

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
        //create system to deal _damage to enemies
        if (collision.transform.TryGetComponent<IHealthSystem>(out IHealthSystem health))
        {
            health.TakeDamage(_projectileProperties.Damage);
        }
        if (collision.transform.TryGetComponent<EnemyElementAccumulation>(out EnemyElementAccumulation elementAccumulation))
        {
            elementAccumulation.TakeElementAccumulation(ElementDamage);
        }
    }

    public void DisableBlast()
    {
        _objectPoolManager.ReleaseGameObject(gameObject, ObjectPoolManager.PoolType.GameObjects);
    }

    void IGunProjectileBehavior.AddModifications(Dictionary<ElementEnum, int> elements)
    {
        ElementDamage = elements;
    }
}
