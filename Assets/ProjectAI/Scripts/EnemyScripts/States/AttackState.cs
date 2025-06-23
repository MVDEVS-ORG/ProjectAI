using UnityEngine;

public class AttackState : IEnemyState
{
    private EnemyAI enemy;
    private float attackCooldown = 1.5f;
    private float timer;

    public void Enter(EnemyAI enemy)
    {
        this.enemy = enemy;
        timer = attackCooldown;
        enemy.StopMovement();
    }

    public void Update()
    {
        if (!enemy.IsPlayerInAttackRange())
        {
            enemy.TransitionToState(new ChaseState());
            return;
        }

        timer += Time.deltaTime;
        if (timer >= attackCooldown)
        {
            Attack();
            timer = 0f;
        }
    }

    public void Exit() { }

    private void Attack()
    {
        Debug.Log("Enemy attacks!");
        // You can play animation, shoot bullet, reduce health here
    }
}
