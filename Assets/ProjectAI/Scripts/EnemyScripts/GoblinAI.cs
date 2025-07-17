using Assets.ProjectAI.Scripts.EnemyScripts.AttackBehaviour;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.ProjectAI.Scripts.EnemyScripts
{
    public class GoblinAI : EnemyAI
    {
        public override void Initialize(HealthModels model)
        {
            base.Initialize(model);
            attackBehaviors.Add(new MeleeAttack());
            attackBehaviors.Add(new RangedAttack());
        }
    }
}