using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public interface IGunProjectileBehavior
{
    void SpawnProjectileAnimation();
    void AddModifications(Dictionary<ElementEnum,int> elements);
    void DestroyManally();
    void MoveProjectile(Vector2 Direction);
    void Initialize(ObjectPoolManager objectPoolManager);
}
