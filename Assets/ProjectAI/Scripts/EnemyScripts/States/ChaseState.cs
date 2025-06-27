using UnityEngine;
using System.Collections.Generic;
using Assets.ProjectAI.Scripts.PathFinding;

public class ChaseState : IEnemyState
{
    private EnemyAI enemy;
    private float pathRefreshTime = Random.Range(0.5f, 1f);
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
        Vector3 targetPos = GetOffsetAroundPlayer();
        Vector3Int start = enemy.floorTilemap.WorldToCell(enemy.transform.position);
        Vector3Int goal = enemy.floorTilemap.WorldToCell(targetPos);
        List<Vector3Int> path = PathFindingManager.Instance.FindPath(start, goal);
        enemy.StartPathMovement(path);
    }
    private Vector3 GetOffsetAroundPlayer()
    {
        int nearbyEnemyCount = 0;
        var allEnemies = GameObject.FindGameObjectsWithTag("Enemy"); // Make sure your enemies are tagged
        foreach (var e in allEnemies)
        {
            if (Vector3.Distance(e.transform.position, enemy.player.position) < 6f)
                nearbyEnemyCount++;
        }

        int index = System.Array.IndexOf(allEnemies, enemy.gameObject);
        float angle = (360f / Mathf.Max(nearbyEnemyCount, 1)) * index;
        float radius = 1.5f; // Distance from the player

        Vector3 offset = new Vector3(
            Mathf.Cos(angle * Mathf.Deg2Rad),
            Mathf.Sin(angle * Mathf.Deg2Rad),
            0
        ) * radius;

        return enemy.player.position + offset;
    }
}
