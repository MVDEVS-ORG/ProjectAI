using System.Collections;
using UnityEngine;

namespace Assets.ProjectAI.Scripts.EnemyScripts.AttackBehaviour
{
    public class MeleeAttack : AbstractAttackState
    {
        private bool _isWaitingForAttackEnd = false;
        private bool _hasPerformedAttack = false;
        private float _lastAttackTime = -Mathf.Infinity;

        public override void Enter(EnemyAI enemy, Transform player, ObjectPoolManager op)
        {
            base.Enter(enemy, player, op);
            _enemy.animator.SetBool("Attack", true);
            _enemy.animator.SetBool("AttackEnd", false);
        }

        private bool CanExecute()
        {
            return _enemy.IsPlayerInAttackRange() && !_isWaitingForAttackEnd;
        }

        private async void Execute()
        {
            Debug.LogError("Melee Slash!");
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
            effect.slashDamage = 10;
            effect.poolManager = _poolManager;
        }

        public override void Update()
        {
            if (_hasPerformedAttack && CanExecute())
            {
                if (Time.time >= _lastAttackTime + _attackCooldown)
                {
                    _lastAttackTime = Time.time;
                    Execute();
                }
            }

            if (!_enemy.IsPlayerInAttackRange() && !_isWaitingForAttackEnd)
            {
                _isWaitingForAttackEnd = true;
                _hasPerformedAttack = false;
                _enemy.animator.SetBool("Attack", false);
                _enemy.animator.SetBool("AttackEnd", true);
                _enemy.StartCoroutine(WaitForAttackEndAndTransition());
            }
        }

        private IEnumerator WaitForAttackEndAndTransition()
        {
            AnimatorStateInfo stateInfo = _enemy.animator.GetCurrentAnimatorStateInfo(0);
            while (!stateInfo.IsName("AttackEnd"))
            {
                yield return null;
                stateInfo = _enemy.animator.GetCurrentAnimatorStateInfo(0);
            }

            while (stateInfo.normalizedTime < 1f)
            {
                yield return null;
                stateInfo = _enemy.animator.GetCurrentAnimatorStateInfo(0);
            }

            _enemy.TransitionToState(_enemy.GetNextStateFromMap(EnemyStateTypes.Chase));
            _isWaitingForAttackEnd = false;
        }

        public override void Attack()
        {
            _hasPerformedAttack = true;
        }

        public override void Exit()
        {
            _enemy.animator.SetBool("AttackEnd", true);
            base.Exit();
        }
    }

}