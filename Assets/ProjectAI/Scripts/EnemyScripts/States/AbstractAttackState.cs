using UnityEngine;

public abstract class AbstractAttackState : IEnemyState
{
    protected EnemyAI _enemy;
    protected Transform _player;
    protected ObjectPoolManager _poolManager;
    private float attackCooldown = 1.5f;
    private float timer;
    private CharacterView _characterView;

    public virtual void Enter(EnemyAI enemy, Transform player, ObjectPoolManager op)
    {
        _poolManager = op;
        _enemy = enemy;
        _player = player;
        timer = attackCooldown;
        enemy.StopMovement();
    }

    public virtual void Update()
    {
        if (!_enemy.IsPlayerInAttackRange())
        {
            IEnemyState state = new ChaseState();
            _enemy.TransitionToState(state);
            return;
        }

        timer += Time.deltaTime;
        if (timer >= attackCooldown)
        {
            Attack();
            timer = 0f;
        }
    }

    public virtual void Exit() { }

    public virtual void Attack()
    {
        //Debug.LogError("Attacking");
    }
}
