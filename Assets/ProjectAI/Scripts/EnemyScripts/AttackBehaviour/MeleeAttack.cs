using System.Collections;
using UnityEngine;

namespace Assets.ProjectAI.Scripts.EnemyScripts.AttackBehaviour
{
    public class MeleeAttack : IAttackBehavior
    {
        public bool CanExecute(EnemyAI enemy)
        {
            return enemy.IsPlayerInAttackRange();
        }

        public void Execute(EnemyAI enemy, ObjectPoolManager op)
        {
            Debug.LogError("Melee Slash!");
        }

        public void ResetState()
        {
            Debug.LogError("resetting state");
        }
    }
}