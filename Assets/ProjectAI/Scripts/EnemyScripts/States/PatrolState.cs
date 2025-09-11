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
        patrolTarget = PathFindingManager.Instance.GetRandomWalkableTile();
        var startPos = PathFindingManager.Instance.floorTilemap.WorldToCell(_enemy.transform.position);
        var targetPos = PathFindingManager.Instance.floorTilemap.WorldToCell(patrolTarget);
        path = PathFindingManager.Instance.FindPath(startPos, targetPos);
        if (path == null)
        {
            Debug.LogError($"{_enemy.name} no path found s-{startPos}, e- {targetPos}");
            return;
        }
        _enemy.StartPathMovement(path);
    }
}
