using UnityEngine;

public class IdleState : IEnemyState
{
    private EnemyAI _enemy;
    private float waitTime = 2f;
    private float timer;

    public void Enter(EnemyAI enemy, Transform player)
    {
        this._enemy = enemy;
        timer = 0f;
    }

    public void Update()
    {
        if (_enemy.IsPlayerVisible())
        {
            _enemy.TransitionToState(new ChaseState());
            return;
        }

        timer += Time.deltaTime;
        if (timer > waitTime)
        {
            _enemy.TransitionToState(new PatrolState());
        }
    }

    public void Exit() { }
}
