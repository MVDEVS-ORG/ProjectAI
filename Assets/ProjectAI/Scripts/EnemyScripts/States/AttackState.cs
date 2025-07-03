using UnityEngine;

public class AttackState : IEnemyState
{
    private EnemyAI _enemy;
    private Transform _player;
    private float attackCooldown = 1.5f;
    private float timer;

    public void Enter(EnemyAI enemy, Transform player)
    {
        _enemy = enemy;
        _player = player;
        timer = attackCooldown;
        enemy.StopMovement();
    }

    public void Update()
    {
        if (!_enemy.IsPlayerInAttackRange())
        {
            _enemy.TransitionToState(new ChaseState());
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
        // You can play animation, shoot bullet, reduce _health here
    }
}
