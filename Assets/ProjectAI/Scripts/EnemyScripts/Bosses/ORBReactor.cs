using Assets.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.ProjectAI.Scripts.EnemyScripts.Bosses
{
    // TODO: add halo range where player can take damage
    public class ORBReactor : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Animator _bossAnimator;
        [SerializeField] private Transform _lightningAttackPosition;
        [SerializeField] private Transform _laserAttackPosition;
        [SerializeField] private GameObject _summoningLight;
        [SerializeField] private GameObject _LaserSmoke;

        [Header("Attack Settings")]
        [SerializeField] private float _delayBetweenAttacks = 2f;
        [SerializeField] private float _lightningDuration = 3f;
        [SerializeField] private float _laserDuration = 4f;
        [SerializeField] private float _novaChargeTime = 2f;
        [SerializeField] private float _novaExplosionDuration = 3f;
        [SerializeField] private float _startingDistanceFormPlayer = 5f;
        [SerializeField] private int _noOfLightningAttack = 3;

        public GameObject EmergencyWalls { get; set; }


        private bool _isInPhase1 = true;
        private ObjectPoolManager _poolManager;
        private IAssetService _assetService;
        private IPlayerController _target;
        // Use this for initialization
        public async void Initilaize(ObjectPoolManager poolManager, IAssetService assetService, IPlayerController playerController)
        {
            _poolManager = poolManager;
            _assetService = assetService;
            _target = playerController;
            await WaitForPlayer();
        }

        async Awaitable WaitForPlayer()
        {
            while(_target != null)
            {
                Transform target = await _target.GetPlayerTransform();
                if(target != null && Vector2.Distance(transform.position, target.position)<= _startingDistanceFormPlayer)
                {
                    BossWakeUp();
                    return;
                }
                await Awaitable.EndOfFrameAsync();
            }
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

        public async void SummonLightning()
        {
            Debug.LogError("Summoning Lightning");
            _summoningLight.SetActive(true);
            // Fire lightning _noOfLightningAttack times in a row
            for (int i = 0; i < _noOfLightningAttack; i++)
            {
                try
                {
                    await LightningStrike();
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }
            }

            Debug.Log("All Lightning Strikes Finished.");
            _summoningLight.SetActive(false);
            AttackFinished();
        }

        private async Awaitable LightningStrike()
        {
            Debug.LogError("Summoning Lightning on the player...");
            Vector3 origin = _lightningAttackPosition.position;
            var target = await _target.GetPlayerTransform();

            // Spawn lightning prefab
            GameObject lightningGO = await _poolManager.SpawnObjectAsync(
                AddressableIds.Enemy_Lightning,
                origin,
                Quaternion.identity,
                ObjectPoolManager.PoolType.ParticleSystems
            );

            var lightning = lightningGO.GetComponent<LightningAttack>();
            if (lightning != null)
            {
                await lightning.Fire(target.position, _poolManager); // lock on to player's position
            }

            // Wait for duration before next strike
            await Awaitable.WaitForSecondsAsync(_lightningDuration);

            Debug.Log("Lightning strike finished.");
        }


        public async void LaserAttack()
        {
            // laser is fired (Laser sweep) towards player
            Debug.LogError("Attacking With Laser");
            _LaserSmoke.SetActive(true);
            // wait for attack to finish and Call AttackFinished()
            await LaserSweep();
            _LaserSmoke.SetActive(false);
        }

        private async Awaitable LaserSweep()
        {
            // Start Laser Sweep
            Debug.LogError(" Attacking With Laser Sweep...");
            Vector3 origin = _laserAttackPosition.position;

            GameObject beamGO = await _poolManager.SpawnObjectAsync(
                AddressableIds.Enemy_Laser_Beam,
                origin,
                Quaternion.identity,
                ObjectPoolManager.PoolType.ParticleSystems
            );
            var beam = beamGO.GetComponent<EnemyLaserBeam>();
            if (beam != null)
            {
                var target = await _target.GetPlayerTransform();
                beam.FireSweep(origin, _poolManager);
            }

            await Awaitable.WaitForSecondsAsync(_laserDuration);

            beam.Interrupt();

            Debug.Log(" Laser sweep finished.");
            AttackFinished();
        }

        public void BossWakeUp()
        {
            _bossAnimator.SetTrigger("WakeUp");
        }

        public async void PerformNovaAttack()
        {
            Debug.LogError("Performing Nova Attack...");

            GameObject novaGO = await _poolManager.SpawnObjectAsync(
                AddressableIds.Enemy_Nova,
                transform.position,             // spawn at boss position
                Quaternion.identity,
                ObjectPoolManager.PoolType.GameObjects
            );

            var nova = novaGO.GetComponent<NovaAttack>();
            if (nova != null)
            {
                var playerTransform = await _target.GetPlayerTransform();
                EmergencyWalls?.SetActive(true);
                await nova.PlayNova(_novaChargeTime, _novaExplosionDuration, playerTransform, _poolManager);
            }
            EmergencyWalls?.SetActive(false);
            Debug.Log(" Nova attack finished.");
            AttackFinished();
        }

    }
}