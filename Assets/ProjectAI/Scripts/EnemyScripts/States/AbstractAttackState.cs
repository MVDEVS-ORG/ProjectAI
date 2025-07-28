using System.Collections.Generic;
using UnityEngine;
public abstract class AbstractAttackState : IEnemyState
{
    protected EnemyAI _enemy;
    protected Transform _player;
    protected ObjectPoolManager _poolManager;
    protected float _attackCooldown = 1.5f;
    protected float timer;
    protected CharacterView _characterView;


    public virtual void Enter(EnemyAI enemy, Transform player, ObjectPoolManager op)
    {
        _poolManager = op;
        _enemy = enemy;
        _player = player;
        _attackCooldown = _enemy.attackCooldown;
        timer = _attackCooldown;
        enemy.StopMovement();
    }

    public virtual void Update()
    {
        
    }

    public virtual void Exit() 
    {
        _enemy = null;
        _player = null;
        _characterView = null;
        timer = 0f;
    }

    public virtual void Attack()
    {
        Debug.LogError("Attacking using virtual Function");
    }
}
