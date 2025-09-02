using Assets.ProjectAI.Scripts.EnemyScripts.AttackBehaviour;
using Assets.ProjectAI.Scripts.EnemyScripts.States;
using System.Collections;
using UnityEngine;

namespace Assets.ProjectAI.Scripts.EnemyScripts
{
    public class RangedRoomba : EnemyAI
    {

        public override void Initialize(HealthModelsSO model)
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
            stateMap.Add(new RangedAttack(), EnemyStateTypes.Attack);

        }

        public void Attack()
        {
            (currentState as RangedAttack)?.Attack();
        }
    }
}