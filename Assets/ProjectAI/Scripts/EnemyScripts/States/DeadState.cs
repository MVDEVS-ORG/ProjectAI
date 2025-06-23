using UnityEngine;

public class DeadState : IEnemyState
{
    public void Enter(EnemyAI enemy)
    {
        GameObject.Destroy(enemy.gameObject);
    }

    public void Update() { }
    public void Exit() { }
}
