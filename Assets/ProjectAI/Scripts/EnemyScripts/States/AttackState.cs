using System.Collections;
using UnityEngine;

namespace Assets.ProjectAI.Scripts.EnemyScripts.States
{
    public class AttackState : AbstractAttackState //TODO: Rename this to other Attack Method when adding different type of enemy attack
    {
        /*public override void Attack()
        {
            base.Attack();
            foreach (var attack in _enemy.attackBehaviors)
            {
                if (attack.CanExecute(_enemy))
                {
                    attack.Execute(_enemy, _poolManager);
                    break;
                }
            }
        }*/
    }
}