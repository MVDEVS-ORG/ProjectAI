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

        public async void Execute(EnemyAI enemy, ObjectPoolManager op)
        {
            //Debug.LogError("Melee Slash!");
            var attackDirection = (enemy.Target.position - enemy.transform.position).normalized;
            Quaternion attackRotation = Quaternion.FromToRotation(Vector3.right, attackDirection); 
            Vector3 spawnPosition = enemy.attackSpawnPos.position + attackDirection * enemy.attackOffset;
            GameObject go = await op.SpawnObjectAsync(
                AddressableIds.Enemy_Melee_Attack,
                spawnPosition,
                attackRotation,
                ObjectPoolManager.PoolType.ParticleSystems
            );
            var effect = go.GetComponent<EnemySlashAttackEffect>();
            effect.EnemyTransform = enemy.transform;
            effect.slashDamage = 10;
            effect.poolManager = op;
        }

        public void ResetState()
        {
            Debug.LogError("resetting state");
        }
    }
}