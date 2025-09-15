using System.Collections;
using System.Threading;
using UnityEngine;

namespace Assets.ProjectAI.Scripts.EnemyScripts.AttackBehaviour
{
    public class RangedAttack : AbstractAttackState
    {
        private EnemyLaserBeam _activeLaserBeam;
        private Coroutine _exitCoroutine;
        private bool _isAttacking;
        private bool _transitioning = false;

        private CancellationTokenSource _cancellationTokenSource;

        public override void Enter(EnemyAI enemy, Transform player, ObjectPoolManager op)
        {
            base.Enter(enemy, player, op);
            _isAttacking = true;
            _transitioning = false;

            _cancellationTokenSource = new CancellationTokenSource();

            _enemy.animator?.SetBool("Attack", true);
            _enemy.animator?.SetBool("AttackEnd", false);
        }

        public override void Update()
        {
            if (!_enemy.IsPlayerInAttackRange() && _isAttacking)
            {
                if (_activeLaserBeam != null && _activeLaserBeam.gameObject.activeSelf)
                {
                    if (_activeLaserBeam.CurrentPhase == BeamPhase.Tracking)
                    {
                        _isAttacking = false;
                        _enemy.animator?.SetBool("Attack", false);
                        _enemy.animator?.SetBool("AttackEnd", true);
                        _activeLaserBeam.Interrupt();
                        _activeLaserBeam = null;
                    }
                    else if (_activeLaserBeam.CurrentPhase == BeamPhase.Growing ||
                        _activeLaserBeam.CurrentPhase == BeamPhase.Locked)
                    {
                        return;
                    }
                }

                _exitCoroutine = _enemy?.StartCoroutine(WaitForAttackEndAndTransition(_cancellationTokenSource.Token));
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
                beam.OnBeamFinished = () =>
                {
                    if (!_enemy.IsPlayerInAttackRange())
                    {
                        _transitioning = true;
                        _enemy.TransitionToState(_enemy.GetNextStateFromMap(EnemyStateTypes.Chase));
                    }
                        
                };
                beam.Fire(origin, _enemy.Target, _poolManager);

            }
        }

        private IEnumerator WaitForAttackEndAndTransition(CancellationToken cancellationToken)
        {
            if (_transitioning) yield break;
            if (_enemy == null || _enemy.animator == null) yield break;

            AnimatorStateInfo stateInfo = _enemy.animator.GetCurrentAnimatorStateInfo(0);

            // Wait until we are actually in AttackEnd animation
            while (!cancellationToken.IsCancellationRequested)
            {
                if (_enemy == null || _enemy.animator == null) yield break;
                if (stateInfo.IsName("AttackEnd")) break;

                yield return null;
                if (cancellationToken.IsCancellationRequested) yield break;

                stateInfo = _enemy.animator.GetCurrentAnimatorStateInfo(0);
            }

            // Wait until AttackEnd finishes
            while (!cancellationToken.IsCancellationRequested)
            {
                if (_enemy == null || _enemy.animator == null) yield break;
                if (stateInfo.normalizedTime >= 1f) break;

                yield return null;
                if (cancellationToken.IsCancellationRequested) yield break;

                stateInfo = _enemy.animator.GetCurrentAnimatorStateInfo(0);
            }

            if (!_transitioning && _enemy != null)
            {
                _transitioning = true;
                _enemy.TransitionToState(_enemy.GetNextStateFromMap(EnemyStateTypes.Chase));
            }
        }


        public override void Exit()
        {
            _transitioning = false;
            _isAttacking = false;
            _enemy.animator?.SetBool("Attack", false);
            _enemy.animator?.SetBool("AttackEnd", true);

            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
            _cancellationTokenSource = null;

            if (_exitCoroutine != null)
            {
                _enemy.StopCoroutine(_exitCoroutine);
                _exitCoroutine = null;
            }

            if (_activeLaserBeam != null)
            {
                _activeLaserBeam.OnBeamFinished = null;
                if (_activeLaserBeam.gameObject.activeSelf)
                {
                    _activeLaserBeam.Interrupt();
                }
                _activeLaserBeam = null;
            }

            base.Exit();
        }
    }
}
