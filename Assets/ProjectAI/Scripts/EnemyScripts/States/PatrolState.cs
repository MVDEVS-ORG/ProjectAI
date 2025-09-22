using Assets.ProjectAI.Scripts.PathFinding;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PatrolState : IEnemyState
{
    private EnemyAI _enemy;
    private Transform _player;
    private Vector3Int patrolTarget;
    private float waitTimeAtPoint = 2f;
    private float waitTimer;
    private bool waiting;

    public void Enter(EnemyAI enemy, Transform player, ObjectPoolManager op)
    {
        _enemy = enemy;
        _player = player;
        waiting = false;
        SetNewPatrolTarget();
        _enemy.animator?.SetBool("Walking", true);
    }

    public void Update()
    {
        if (_enemy.IsPlayerVisible())
        {
            _enemy.TransitionToState(_enemy.GetNextStateFromMap(EnemyStateTypes.Chase));
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

    public void Exit() 
    {
        _enemy.animator?.SetBool("Walking", false);

        _enemy = null;
        waiting = false;
        _player = null;
        waitTimer = 0;
    }

    private void SetNewPatrolTarget()
    {
        List<Vector3Int> path = new();

        // Get a random valid tile from the dungeon
        Vector2Int patrolTargetCell = PathFindingManager.Instance.GetRandomWalkableTile();

        // Convert enemy world position to cell
        Vector3Int startPos = PathFindingManager.Instance.WorldToCell(_enemy.transform.position);

        // If enemy somehow is outside walkable area, snap to nearest valid tile
        if (!PathFindingManager.Instance.IsWalkable((Vector2Int)startPos))
        {
            startPos = (Vector3Int)PathFindingManager.Instance.GetNearestValidWalkableTile((Vector2Int)startPos);
        }

        // Find a path to the patrol target
        path = PathFindingManager.Instance.FindPath(startPos, (Vector3Int)patrolTargetCell);

        if (path == null || path.Count == 0)
        {
            Debug.LogWarning($"{_enemy.name} no path found for patrol s-{startPos}, e-{patrolTargetCell}, retrying...");
            // Retry once with a different target
            patrolTargetCell = PathFindingManager.Instance.GetRandomWalkableTile();
            path = PathFindingManager.Instance.FindPath(startPos, (Vector3Int)patrolTargetCell);

            if (path == null || path.Count == 0)
                return; // give up this frame
        }

        _enemy.StartPathMovement(path);
    }

}
