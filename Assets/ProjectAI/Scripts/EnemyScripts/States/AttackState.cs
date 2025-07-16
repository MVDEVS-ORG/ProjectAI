using System.Collections;
using UnityEngine;

namespace Assets.ProjectAI.Scripts.EnemyScripts.States
{
    public class AttackState : AbstractAttackState
    {
        public override void Attack()
        {
            base.Attack();
            Debug.LogError("attacking different attacks");
        }
    }
}