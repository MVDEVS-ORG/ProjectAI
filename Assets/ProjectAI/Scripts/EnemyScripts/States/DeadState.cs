﻿using UnityEngine;

public class DeadState : IEnemyState
{
    public void Enter(EnemyAI enemy, Transform player)
    {
        GameObject.Destroy(enemy.gameObject);
        //Add Object pooling
    }

    public void Update() { }
    public void Exit() { }
}
