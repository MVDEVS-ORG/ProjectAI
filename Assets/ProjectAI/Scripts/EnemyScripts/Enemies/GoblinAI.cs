using Assets.ProjectAI.Scripts.EnemyScripts.AttackBehaviour;
using Assets.ProjectAI.Scripts.EnemyScripts.States;

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
        }
        public override void InitializeStates()
        {
            stateMap.Add(new IdleState(), EnemyStateTypes.Idle);
            stateMap.Add(new SearchState(), EnemyStateTypes.Search);
            stateMap.Add(new ChaseState(), EnemyStateTypes.Chase);
            stateMap.Add(new DeadState(), EnemyStateTypes.Dead);
            stateMap.Add(new PatrolState(), EnemyStateTypes.Patrol);
            stateMap.Add(new MeleeAttack(), EnemyStateTypes.Attack);

        }

        public void Attack()
        {
            Debug.LogError(currentState);
            (currentState as MeleeAttack)?.Attack();
        }
    }
}