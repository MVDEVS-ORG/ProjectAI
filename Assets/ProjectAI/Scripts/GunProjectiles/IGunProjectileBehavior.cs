using UnityEngine;

public interface IGunProjectileBehavior
{
    void SpawnProjectileAnimation();
    void AddModifications();
    void DestroyManally();
    void MoveProjectile(Vector2 Direction);
    void Initialize(ObjectPoolManager objectPoolManager);
}
