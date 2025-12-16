using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Assets.Services;
using UnityEngine;
using Zenject;

namespace Assets.ProjectAI.Scripts.EnemyScripts.Bosses
{
    public class ORBReactor : MonoBehaviour, IHealthSystem
    {
        [SerializeField] private string _bossName;
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
        [SerializeField] private NearDistanceDamageDealer _bossAura;
        [Range(0.0f, 3f)][SerializeField] private float _sweepDuration = 1.5f;

        [Header("Health Settings")]
        [SerializeField] private HealthModelsSO _healthModel;

        public GameObject EmergencyWalls { get; set; }
        public GameObject BossRoomDoor { get; set; }

        private int _health;
        private int _maxHealth;
        public int Health => _health;

        public int MaxHealth => _maxHealth;

        private bool _isInPhase1 = true;
        private bool _isPhase2Active = true;
        private bool _bossInitialized = false;

        //Private References
        private ObjectPoolManager _poolManager;
        private IAssetService _assetService;
        private IPlayerController _target;
        private BossHealthUI _bossHealthUI;
        private CameraController _camController;
        private Transform _camTransform;
        private SignalBus _signalBus;
        private CancellationTokenSource _cancellationTokenSource;
        private CancellationToken _cancellationToken;

        private string _lastAttack = null;
        private bool _isFirstAttack = true;
        // Use this for initialization
        public async void InitializeBoss(ObjectPoolManager poolManager, IAssetService assetService, IPlayerController playerController, CameraController camController, Transform camTransform, SignalBus signalBus)
        {
            _isFirstAttack = true;
            _isPhase2Active = false;
            _signalBus = signalBus;
            _poolManager = poolManager;
            _assetService = assetService;
            _target = playerController;
            _camController = camController;
            _camTransform = camTransform;
            await WaitForPlayer();
        }

        async Awaitable WaitForPlayer()
        {
            while (!destroyCancellationToken.IsCancellationRequested && _target != null)
            {
                Transform target = await _target.GetPlayerTransform();
                if (target != null && Vector2.Distance(transform.position, target.position) <= _startingDistanceFormPlayer)
                {
                    BossRoomDoor?.SetActive(true);
                    BossWakeUp();
                    _camController.DetachCamera(_camTransform, 16.5f);
                    var bossHpCanvas = await _assetService.InstantiateAsync(AddressableIds.Boss_HP_Canvas);
                    _bossHealthUI = bossHpCanvas.GetComponent<BossHealthUI>();
                    Initialize(_healthModel);
                    _bossHealthUI.Initialize(_healthModel, _bossName);
                    _bossInitialized = true;
                    return;
                }
                await Awaitable.EndOfFrameAsync();
            }
        }

        public void EnterIdle()
        {
            _bossAnimator.SetTrigger("Idle");
            _bossAura.TurnOnAura();
        }

        public void SelectAndPlayAttack()
        {
            if (_isInPhase1)
            {
                StartCoroutine(PlayRandomAttack());
            }
            else if (_isPhase2Active && _healthModel.Health > 0)
            {
                //Phase 2 starts
                StartCoroutine(PlayPhaseTwoAttacks(false));
            }
            else if (_healthModel.Health <= 0)
            {
                StartCoroutine(PlayPhaseTwoAttacks(true));
            }

        }

        private IEnumerator PlayRandomAttack()
        {
            yield return new WaitForSeconds(_delayBetweenAttacks);
            string chosenAttack = GetRandomAttack();
            _bossAnimator.SetTrigger(chosenAttack);
        }

        private IEnumerator PlayPhaseTwoAttacks(bool isDead)
        {
            yield return new WaitForSeconds(_delayBetweenAttacks);
            _bossAnimator.SetTrigger($"{(isDead ? "Nova" : "LaserAttack")}");
        }

        private string GetRandomAttack()
        {
            List<string> weightedAttacks = new List<string>()
            {
                "UpwardAttack", "UpwardAttack", "UpwardAttack",
                "LaserAttack", "LaserAttack", "LaserAttack",
                "Nova"
            };
            string chosenAttack;

            // Handle first attack: cannot be "Nova"
            if (_isFirstAttack)
            {
                List<string> noNovaList = new List<string>()
                {
                    "UpwardAttack", "LaserAttack"
                };

                chosenAttack = noNovaList[UnityEngine.Random.Range(0, noNovaList.Count)];
                _isFirstAttack = false;
            }
            else
            {
                chosenAttack = weightedAttacks[UnityEngine.Random.Range(0, weightedAttacks.Count)];
            }

            _lastAttack = chosenAttack;
            return chosenAttack;
        }

        public void AttackFinished()
        {
            _bossAnimator.SetTrigger("AttackComplete");
            _bossAura.TurnOnAura();
        }

        public async void SummonLightning()
        {
            _bossAura.TurnOffAura();
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
            _bossAura.TurnOffAura();
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
            _bossAura.TurnOffAura();
            if (_isInPhase1)
            {
                // laser is fired (Laser sweep) towards player
                _LaserSmoke.SetActive(true);
                bool leftToRight = UnityEngine.Random.value > 0.5f;
                // wait for attack to finish and Call AttackFinished()
                await LaserSweep(leftToRight);
                Debug.Log(" Laser sweep finished.");
                AttackFinished();
                _LaserSmoke.SetActive(false);
            }
            else
            {
                await Phase2AttackLoop();
            }

        }


        private async Awaitable LaserSweep(bool leftToRight)
        {
            _ = FireSweepNoWait(leftToRight);
            await Awaitable.WaitForSecondsAsync(_laserDuration);
        }

        private void OnEnable()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _cancellationToken = _cancellationTokenSource.Token;
        }

        private void OnDestroy()
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource = null;
        }
        private void OnDisable()
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource = null;
        }

        private async Awaitable FireSweepNoWait(bool leftToRight)
        {
            Vector3 origin = _laserAttackPosition.position;

            GameObject beamGO = await _poolManager.SpawnObjectAsync(
                AddressableIds.Enemy_Laser_Beam,
                origin,
                Quaternion.identity,
                ObjectPoolManager.PoolType.GameObjects
            );

            var beam = beamGO.GetComponent<EnemyLaserBeam>();
            if (beam != null)
            {
                var target = await _target.GetPlayerTransform();
                beam.FireSweep(origin, _poolManager, leftToRight, _isPhase2Active, _sweepDuration, _cancellationToken);

                // stop beam after duration
                await Awaitable.WaitForSecondsAsync(_laserDuration);
                beam.Interrupt();
            }
        }

        private async Awaitable Phase2AttackLoop()
        {
            _bossAura.TurnOffAura();
            _LaserSmoke.SetActive(true);
            while (_healthModel.Health > 0) // keep going until boss dies
            {
                // Left to Right
                await LaserSweep(true);

                // Right to Left
                await LaserSweep(false);

                //both at at the Same Time
                _ = FireSweepNoWait(true);
                await Awaitable.EndOfFrameAsync();
                _ = FireSweepNoWait(false);
                await Awaitable.WaitForSecondsAsync(_laserDuration);

            }
            _LaserSmoke.SetActive(false);
            AttackFinished();
        }

        public void BossWakeUp()
        {
            _bossAnimator.SetTrigger("WakeUp");
            _bossAura.TurnOnAura();
        }

        public async void PerformNovaAttack()
        {
            _bossAura.TurnOffAura();
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
                await nova.PlayNova(_novaChargeTime, _novaExplosionDuration, playerTransform, _poolManager, _signalBus);
            }
            EmergencyWalls?.SetActive(false);
            Debug.Log(" Nova attack finished.");
            AttackFinished();
        }

        public void TakeDamage(int damage)
        {
            if (!_bossInitialized) return;
            _healthModel.Health = Mathf.Max(0, _healthModel.Health - damage);
            _bossHealthUI.AlterHealthBar();
            if (_healthModel.Health * 100 / _healthModel.MaxHealth <= 40)
            {
                _isInPhase1 = false;
                if (!_isPhase2Active)
                {
                    _isPhase2Active = true;
                    StopAllCoroutines();
                    _bossAnimator.SetBool("Phase2", true);
                }
            }
            if (_healthModel.Health <= 0)
            {
                _cancellationTokenSource?.Cancel();
                _cancellationTokenSource = null;
            }
        }

        public void Heal(int healing)
        {
            // No Healing Required
        }

        public void Initialize(HealthModelsSO model)
        {
            model.Health = model.MaxHealth;
            _health = model.MaxHealth;
            _maxHealth = model.MaxHealth;

        }

        public void ResetHealth()
        {
            // Not required
        }

    }
}