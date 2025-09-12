using System.Collections.Generic;
using UnityEngine;

public class IdleState : IEnemyState
{
    private EnemyAI _enemy;
    private float waitTime = 2f;
    private float timer;

    public void Enter(EnemyAI enemy, Transform player, ObjectPoolManager op)
    {
        this._enemy = enemy;
        timer = 0f;
        enemy.animator?.SetBool("Idle", true);
    }

    public void Update()
    {
        if (_enemy.IsPlayerVisible())
        {
            _enemy.TransitionToState(_enemy.GetNextStateFromMap(EnemyStateTypes.Chase));
            return;
        }
        

        timer += Time.deltaTime;
        if (timer > waitTime)
        {
            _enemy.TransitionToState(_enemy.GetNextStateFromMap(EnemyStateTypes.Patrol));
        }
    }

    public void Exit() 
    {
        _enemy.animator?.SetBool("Idle", false);
        _enemy = null;
        timer = 0f;
    }
}
