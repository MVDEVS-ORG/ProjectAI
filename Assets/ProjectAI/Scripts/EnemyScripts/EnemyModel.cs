using UnityEngine;

public class EnemyModel
{
    public float moveSpeed;
    public float attackRange;
    public float detectionRange;
    public float attackOffset;
    public float attackCooldown;
    public int xp;

    public EnemyModel(EnemyDataSO enemyData)
    {
        moveSpeed = enemyData.moveSpeed;
        attackRange = enemyData.attackRange;
        detectionRange = enemyData.detectionRange;
        attackOffset = enemyData.attackOffset;
        attackCooldown = enemyData.attackCooldown;
        xp = enemyData.xp;
    }
}
