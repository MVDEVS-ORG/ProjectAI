using System.Collections;
using Assets.ProjectAI.Scripts.EnemyScripts.AttackBehaviour;
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

        public override void TransitionToState(IEnemyState newState)
        {
            base.TransitionToState(newState);
            _rb.bodyType = RigidbodyType2D.Kinematic;
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            _rb.linearVelocity = Vector2.zero;
            _rb.bodyType = RigidbodyType2D.Static;
            animator?.Play("Stunned");
            if (collision.collider.TryGetComponent(out CharacterView player))
            {
                player.TakeDamage(10, transform.position, 1f);
            }
        }
    }
}