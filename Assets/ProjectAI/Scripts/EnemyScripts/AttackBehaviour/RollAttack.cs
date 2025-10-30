using System;
using System.Collections;
using UnityEngine;

namespace Assets.ProjectAI.Scripts.EnemyScripts.AttackBehaviour
{
    public class RollAttack : AbstractAttackState
    {
        private bool _isWaitingForAttackEnd = false;
        private bool _hasPerformedAttack = true;
        private Rigidbody2D _rb;
        private float _lastAttackTime = -Mathf.Infinity;
        private Coroutine _attackCoroutine;
        private bool _inAttack;

        private float _slowDownTime = 0.5f;

        private float _attackMovementSpeed = 5f;
        private int _attackDamage = 10;
        public override void Enter(EnemyAI enemy, Transform player, ObjectPoolManager op)
        {
            _rb = enemy.GetComponent<Rigidbody2D>();
            base.Enter(enemy, player, op);

            if (_characterView == null)
            {
                _characterView = _player.GetComponent<CharacterView>();
            }
            _attackCooldown = 5f;
        }

        private bool CanExecute()
        {
            return _enemy.IsPlayerInAttackRange() && !_isWaitingForAttackEnd;
        }

        private void Execute()
        {
            Vector3 StartPos = _enemy.transform.position;
            Vector3 Direction = (_player.position - _enemy.transform.position).normalized;
            if (_attackCoroutine != null)
            {
                _enemy.StopCoroutine(_attackCoroutine);
                _attackCoroutine = null;
            }
            _attackCoroutine = _enemy.StartCoroutine(RollAttackExecution(StartPos, Direction));
        }

        public override void Update()
        {
            if (_hasPerformedAttack && CanExecute() && !_enemy.enemyModel.Stunned)
            {
                if (Time.time >= _lastAttackTime + _attackCooldown)
                {
                    _lastAttackTime = Time.time;
                    Execute();
                }
            }
            if (!_enemy.IsPlayerInAttackRange())
            {
                _enemy.TransitionToState(_enemy.GetNextStateFromMap(EnemyStateTypes.Search));
            }
        }

        private IEnumerator RollAttackExecution(Vector3 StartPos, Vector3 Direction)
        {
            float timer = 0f;
            _enemy.animator?.Play("AttackStart");
            //before attack
            yield return new WaitForSeconds(1f);
            _enemy.animator?.Play("Attack");
            _rb.linearVelocity = Direction * 10f;
            while (_inAttack)
            {
                timer += Time.deltaTime;
                if (timer > 2f)
                {
                    _rb.linearVelocity = Vector2.zero;
                    _inAttack = false;
                    _hasPerformedAttack = true;
                }
                yield return Awaitable.EndOfFrameAsync();
            }
        }

        public override void Exit()
        {
            _rb.linearVelocity = Vector2.zero;
            _enemy.animator?.SetBool("AttackEnd", true);
            if (_attackCoroutine != null)
            {
                _enemy.StopCoroutine(_attackCoroutine);
                _attackCoroutine = null;
            }
            base.Exit();
        }
    }
}