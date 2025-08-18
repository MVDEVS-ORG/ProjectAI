using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.ProjectAI.Scripts.EnemyScripts.Bosses
{
    public class ORBReactor : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Animator _bossAnimator;

        [Header("Attack Settings")]
        [SerializeField] private float _delayBetweenAttacks = 2f;


        private bool _isInPhase1 = true;
        // Use this for initialization
        void Start()
        {

        }

        public void EnterIdle()
        {
            _bossAnimator.SetTrigger("Idle");
        }

        public void SelectAndPlayAttack()
        {
            StartCoroutine(PlayRandomAttack());
        }

        private IEnumerator PlayRandomAttack()
        {
            yield return new WaitForSeconds(_delayBetweenAttacks);
            string chosenAttack = GetRandomAttack();
            _bossAnimator.SetTrigger(chosenAttack);
        }

        private string GetRandomAttack()
        {
            List<string> attacks = new List<string>()
            {
                "UpwardAttack",
                "Nova",
                "LaserAttack"
            };

            int index = UnityEngine.Random.Range(0, attacks.Count);
            return attacks[index];
        }

        public void AttackFinished()
        {
            _bossAnimator.SetTrigger("AttackComplete");
        }

        public void SummonLightning()
        {
            Debug.LogError("Summoning Lightning");
            StartCoroutine(Nova());
        }

        public void LaserAttack()
        {
            Debug.LogError("Attacking With Laser");
            StartCoroutine(Nova());
        }

        public void BossWakeUp()
        {
            _bossAnimator.SetTrigger("WakeUp");
        }

        public void NovaAttack()
        {
            StartCoroutine(Nova());
        }

        private IEnumerator Nova()
        {
            yield return new WaitForSeconds(2f);
            AttackFinished();
        }
    }
}