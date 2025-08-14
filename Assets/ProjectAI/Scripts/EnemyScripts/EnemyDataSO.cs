using UnityEngine;

[CreateAssetMenu(fileName = "EnemyDataSO", menuName = "Scriptable Objects/EnemyDataSO")]
public class EnemyDataSO : ScriptableObject
{
    public float moveSpeed;
    public float attackRange;
    public float detectionRange;
    public float attackOffset;
    public float attackCooldown; 
    public int xp;
}
