
using UnityEngine;

public interface IEnemyState
{
    void Enter(EnemyAI enemy, Transform player);
    void Update();
    void Exit();
}