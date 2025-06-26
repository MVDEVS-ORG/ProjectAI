using UnityEngine;

public class IdleState : IEnemyState
{
    private EnemyAI enemy;
    private float waitTime = 2f;
    private float timer;

    public void Enter(EnemyAI enemy)
    {
        this.enemy = enemy;
        timer = 0f;
    }

    public void Update()
    {
        if (enemy.IsPlayerVisible())
        {
            enemy.TransitionToState(new ChaseState());
            return;
        }

        timer += Time.deltaTime;
        if (timer > waitTime)
        {
            enemy.TransitionToState(new PatrolState());
        }
    }

    public void Exit() { }
}
