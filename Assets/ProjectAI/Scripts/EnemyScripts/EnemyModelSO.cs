using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyModelSO", menuName = "Scriptable Objects/EnemyModelSO")]
public class EnemyModelSO : ScriptableObject
{
    public float moveSpeed;
    public float attackRange;
    public float detectionRange;
    public List<IEnemyState> enemyStates;
}
