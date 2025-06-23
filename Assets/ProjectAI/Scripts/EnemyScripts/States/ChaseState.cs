using UnityEngine;
using System.Collections.Generic;
using Assets.ProjectAI.Scripts.PathFinding;

public class ChaseState : IEnemyState
{
    private EnemyAI enemy;
    private float pathRefreshTime = 1f;
    private float timer;

    public void Enter(EnemyAI enemy)
    {
        this.enemy = enemy;
        RequestPath();
        timer = 0f;
    }

    public void Update()
    {
        if (!enemy.IsPlayerVisible())
        {
            enemy.TransitionToState(new SearchState());
            return;
        }

        if (enemy.IsPlayerInAttackRange())
        {
            enemy.TransitionToState(new AttackState());
            return;
        }

        timer += Time.deltaTime;
        if (timer > pathRefreshTime)
        {
            RequestPath();
            timer = 0f;
        }
    }

    public void Exit() { }

    private void RequestPath()
    {
        Vector3Int start = enemy.floorTilemap.WorldToCell(enemy.transform.position);
        Vector3Int goal = enemy.floorTilemap.WorldToCell(enemy.player.position);
        List<Vector3Int> path = PathFindingManager.Instance.FindPath(start, goal);
        enemy.StartPathMovement(path);
    }
}
