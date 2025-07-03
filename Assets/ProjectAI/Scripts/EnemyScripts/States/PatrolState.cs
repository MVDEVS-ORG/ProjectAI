using Assets.ProjectAI.Scripts.PathFinding;
using System.Collections.Generic;
using UnityEngine;

public class PatrolState : IEnemyState
{
    private EnemyAI _enemy;
    private Transform _player;
    private Vector3Int patrolTarget;
    private float waitTimeAtPoint = 2f;
    private float waitTimer;
    private bool waiting;

    public void Enter(EnemyAI enemy, Transform player)
    {
        _enemy = enemy;
        _player = player;
        waiting = false;
        SetNewPatrolTarget();
    }

    public void Update()
    {
        if (_enemy.IsPlayerVisible())
        {
            _enemy.TransitionToState(new ChaseState());
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
        else if (_enemy.currentPath == null || _enemy.currentPathIndex >= _enemy.currentPath.Count)
        {
            // Reached target, start waiting
            waiting = true;
            waitTimer = 0f;
        }
    }

    public void Exit() { }

    private void SetNewPatrolTarget()
    {
        patrolTarget = _enemy.GetRandomWalkableTile();
        Vector3Int start = _enemy.floorTilemap.WorldToCell(_enemy.transform.position);
        List<Vector3Int> path = PathFindingManager.Instance.FindPath(start, patrolTarget);
        _enemy.StartPathMovement(path);
    }
}
