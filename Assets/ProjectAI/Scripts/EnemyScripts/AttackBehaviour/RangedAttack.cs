using System.Collections;
using UnityEngine;

namespace Assets.ProjectAI.Scripts.EnemyScripts.AttackBehaviour
{
    public class RangedAttack : IAttackBehavior
    {
        public bool CanExecute(EnemyAI enemy)
        {
            float dist = Vector3.Distance(enemy.transform.position, enemy.Target.position);
            return dist >= 2f && dist <= 6f;
        }

        public void Execute(EnemyAI enemy, ObjectPoolManager op)
        {
            Debug.LogError("Firing projectile!");
            //Fire projectile here!
        }

        public void ResetState()
        {
            Debug.LogError("resetting state");
        }
    }
}