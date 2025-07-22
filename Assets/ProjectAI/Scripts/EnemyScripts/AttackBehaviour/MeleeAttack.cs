using System.Collections;
using UnityEngine;

namespace Assets.ProjectAI.Scripts.EnemyScripts.AttackBehaviour
{
    public class MeleeAttack : AbstractAttackState
    {
        private bool CanExecute()
        {
            return _enemy.IsPlayerInAttackRange();
        }

        private async void Execute()
        {
            //Debug.LogError("Melee Slash!");
            var attackDirection = (_enemy.Target.position - _enemy.transform.position).normalized;
            Quaternion attackRotation = Quaternion.FromToRotation(Vector3.right, attackDirection); 
            Vector3 spawnPosition = _enemy.attackSpawnPos.position + attackDirection * _enemy.attackOffset;
            GameObject go = await _poolManager.SpawnObjectAsync(
                AddressableIds.Enemy_Melee_Attack,
                spawnPosition,
                attackRotation,
                ObjectPoolManager.PoolType.ParticleSystems
            );
            var effect = go.GetComponent<EnemySlashAttackEffect>();
            effect.EnemyTransform = _enemy.transform;
            effect.slashDamage = 10;
            effect.poolManager = _poolManager;
        }

        public override void Update()
        {
            if (!_enemy.IsPlayerInAttackRange())
            {
                IEnemyState state = new ChaseState();
                _enemy.TransitionToState(state);
                return;
            }

            timer += Time.deltaTime;
            if (timer >= _attackCooldown)
            {
                Attack();
                timer = 0f;
            }
        }
        public override void Attack()
        {
            Debug.Log($"This is being called! but value is {CanExecute()}");
            if (CanExecute())
            {
                Execute();
            }
        }
    }
}