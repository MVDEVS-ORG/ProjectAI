using Assets.ProjectAI.Scripts.PathFinding;
using System.Collections.Generic;
using UnityEngine;

public class SearchState : IEnemyState
{
    private EnemyAI enemy;
    private float searchTime = 3f;
    private float timer;
    private Vector3 lastKnownPos;

    public void Enter(EnemyAI enemy)
    {
        this.enemy = enemy;
        lastKnownPos = enemy.player.position;
        Vector3Int start = enemy.floorTilemap.WorldToCell(enemy.transform.position);
        Vector3Int goal = enemy.floorTilemap.WorldToCell(lastKnownPos);
        List<Vector3Int> path = PathFindingManager.Instance.FindPath(start, goal);
        enemy.StartPathMovement(path);
        timer = 0f;
    }

    public void Update()
    {
        timer += Time.deltaTime;

        if (enemy.IsPlayerVisible())
        {
            enemy.TransitionToState(new ChaseState());
            return;
        }

        if (timer > searchTime)
        {
            enemy.TransitionToState(new IdleState());
        }
    }

    public void Exit() { }
}
