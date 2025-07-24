using System.Collections;
using UnityEngine;

namespace Assets.ProjectAI.Scripts.EnemyScripts.AttackBehaviour
{
    public class RangedAttack : AbstractAttackState
    {
        private bool _isWaitingForAttackEnd = false;
        private bool _hasPerformedAttack = false;
        private float _lastAttackTime = -Mathf.Infinity;
        private EnemyLaserBeam _activeLaserBeam;
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
            Debug.LogError("Ranged Attack");
            Vector3 origin = _enemy.attackSpawnPos.position;

            GameObject beamGO = await _poolManager.SpawnObjectAsync(
                AddressableIds.Enemy_Laser_Beam,
                origin,
                Quaternion.identity,
                ObjectPoolManager.PoolType.ParticleSystems
            );

            var beam = beamGO.GetComponent<EnemyLaserBeam>();
            if (beam != null)
            {
                _activeLaserBeam = beam;
                beam.Fire(origin, _enemy.Target, _poolManager);
            }
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
            if (_activeLaserBeam != null && _activeLaserBeam.gameObject.activeSelf)
            {
                _activeLaserBeam.Interrupt();
                _activeLaserBeam = null;
            }
            _isWaitingForAttackEnd = false;
            base.Exit();
            
        }
    }
}