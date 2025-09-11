using System.Collections;
using UnityEngine;

namespace Assets.ProjectAI.Scripts.EnemyScripts.AttackBehaviour
{
    public class RangedAttack : AbstractAttackState
    {
        private EnemyLaserBeam _activeLaserBeam;
        private Coroutine _exitCoroutine;
        private bool _isAttacking;

        public override void Enter(EnemyAI enemy, Transform player, ObjectPoolManager op)
        {
            base.Enter(enemy, player, op);
            _isAttacking = true;
            _enemy.animator?.SetBool("Attack", true);
            _enemy.animator?.SetBool("AttackEnd", false);
        }

        public override void Update()
        {
            if (!_enemy.IsPlayerInAttackRange() && _isAttacking)
            {
                // Player left range → stop attacking immediately
                _isAttacking = false;
                _enemy.animator?.SetBool("Attack", false);
                _enemy.animator?.SetBool("AttackEnd", true);

                if (_activeLaserBeam != null && _activeLaserBeam.gameObject.activeSelf)
                {
                    _activeLaserBeam.Interrupt();
                    _activeLaserBeam = null;
                }

                _exitCoroutine = _enemy.StartCoroutine(WaitForAttackEndAndTransition());
            }
        }

        // Called by Animation Event
        public override void Attack()
        {
            if (!_isAttacking) return;

            Execute();
        }

        private async void Execute()
        {
            // If there is already a beam active, don’t fire another
            if (_activeLaserBeam != null && _activeLaserBeam.gameObject.activeSelf) return;

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

                // Wait until beam finishes before next attack
                _enemy.StartCoroutine(WaitForBeamToFinish());
            }
        }

        private IEnumerator WaitForBeamToFinish()
        {
            // Wait while beam is alive
            while (_activeLaserBeam != null && _activeLaserBeam.lockOn)
            {
                yield return null;
            }

            // Beam finished → if still in attack range, restart attack
            if (_enemy.IsPlayerInAttackRange() && _isAttacking)
            {
                _enemy.animator?.SetBool("Attack", false);
                yield return null; // force animator reset
                _enemy.animator?.SetBool("Attack", true);
            }
            else if (!_enemy.IsPlayerInAttackRange())
            {
                _enemy.TransitionToState(_enemy.GetNextStateFromMap(EnemyStateTypes.Chase));
            }
        }

        private IEnumerator WaitForAttackEndAndTransition()
        {
            while (_activeLaserBeam != null && _activeLaserBeam.lockOn)
            {
                yield return null;
            }
            AnimatorStateInfo stateInfo = _enemy.animator.GetCurrentAnimatorStateInfo(0);

            // Wait until we are actually in AttackEnd animation
            while (!stateInfo.IsName("AttackEnd"))
            {
                yield return null;
                stateInfo = _enemy.animator.GetCurrentAnimatorStateInfo(0);
            }

            // Wait until AttackEnd finishes
            while (stateInfo.normalizedTime < 1f)
            {
                yield return null;
                stateInfo = _enemy.animator.GetCurrentAnimatorStateInfo(0);
            }
/*            while (_activeLaserBeam!= null && !_activeLaserBeam.lockOn)
            {
                yield return null;
            }*/
            _enemy.TransitionToState(_enemy.GetNextStateFromMap(EnemyStateTypes.Chase));
        }

        public override void Exit()
        {
            _isAttacking = false;
            _enemy.animator?.SetBool("Attack", false);
            _enemy.animator?.SetBool("AttackEnd", true);

            if (_exitCoroutine != null)
            {
                _enemy.StopCoroutine(_exitCoroutine);
                _exitCoroutine = null;
            }

            if (_activeLaserBeam != null && _activeLaserBeam.gameObject.activeSelf)
            {
                _activeLaserBeam.Interrupt();
                _activeLaserBeam = null;
            }

            base.Exit();
        }
    }
}
