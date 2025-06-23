using UnityEngine;
using System.Collections.Generic;
using Assets.ProjectAI.Scripts.PathFinding;

public class PatrolState : IEnemyState
{
    private EnemyAI enemy;
    private Vector3Int patrolTarget;
    private float waitTimeAtPoint = 2f;
    private float waitTimer;
    private bool waiting;

    public void Enter(EnemyAI enemy)
    {
        this.enemy = enemy;
        waiting = false;
        SetNewPatrolTarget();
    }

    public void Update()
    {
        if (enemy.IsPlayerVisible())
        {
            enemy.TransitionToState(new ChaseState());
            return;
        }

        if (waiting)
        {
            waitTimer += Time.deltaTime;
            if (waitTimer >= waitTimeAtPoint)
            {
                waiting = false;
                SetNewPatrolTarget();
            }
        }
        else if (enemy.currentPath == null || enemy.currentPathIndex >= enemy.currentPath.Count)
        {
            // Reached target, start waiting
            waiting = true;
            waitTimer = 0f;
        }
    }

    public void Exit() { }

    private void SetNewPatrolTarget()
    {
        patrolTarget = enemy.GetRandomWalkableTile();
        Vector3Int start = enemy.floorTilemap.WorldToCell(enemy.transform.position);
        List<Vector3Int> path = PathFindingManager.Instance.FindPath(start, patrolTarget);
        enemy.StartPathMovement(path);
    }
}
