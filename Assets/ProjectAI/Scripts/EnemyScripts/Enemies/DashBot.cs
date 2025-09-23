using Assets.ProjectAI.Scripts.EnemyScripts.AttackBehaviour;
using System.Collections;
using UnityEngine;

namespace Assets.ProjectAI.Scripts.EnemyScripts.Enemies
{
    public class DashBot : EnemyAI
    {
        public Rigidbody2D _rb;

        public override void InitialState()
        {
            StartStateType = EnemyStateTypes.Patrol;
        }
        public override void InitializeStates()
        {
            stateMap.Add(new PatrolState(), EnemyStateTypes.Idle);
            stateMap.Add(new SearchState(), EnemyStateTypes.Search);
            stateMap.Add(new ChaseState(), EnemyStateTypes.Chase);
            stateMap.Add(new DeadState(), EnemyStateTypes.Dead);
            stateMap.Add(new PatrolState(), EnemyStateTypes.Patrol);
            stateMap.Add(new RollAttack(), EnemyStateTypes.Attack);
            _rb = GetComponent<Rigidbody2D>();
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            _rb.linearVelocity = Vector2.zero;
        }
    }
}