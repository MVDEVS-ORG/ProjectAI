
using System.Collections.Generic;
using UnityEngine;

public interface IEnemyState
{
    void Enter(EnemyAI enemy, Transform player, ObjectPoolManager op);
    void Update();
    void Exit();
}