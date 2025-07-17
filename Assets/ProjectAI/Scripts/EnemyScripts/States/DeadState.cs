using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadState : IEnemyState
{
    private EnemyAI _enemy;
    private ObjectPoolManager _objectPoolmanager;
    public void Enter(EnemyAI enemy, Transform player, ObjectPoolManager op)
    {
        _enemy = enemy;
        _objectPoolmanager = op;
        Debug.LogError("Enemy Dead");
        _objectPoolmanager.ReleaseGameObject(_enemy.gameObject, ObjectPoolManager.PoolType.Enemies);
        //_ = DeathAnimation();
        //Add Object pooling
    }

    async Awaitable DeathAnimation()
    {
        await Awaitable.WaitForSecondsAsync(1000);
        _objectPoolmanager.ReleaseGameObject(_enemy.gameObject, ObjectPoolManager.PoolType.Enemies);
    }

    public void Update() { }
    public void Exit() { }
}
