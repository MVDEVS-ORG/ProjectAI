using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.ProjectAI.Scripts.EnemyScripts.Bosses
{
    public class BossHealthUI : MonoBehaviour
    {

        [SerializeField] private Image _health;
        [SerializeField] private Image _rallyHealth;
        [SerializeField] private Animator _coreAnimator;
        [Range(0.1f, 3f)]
        [SerializeField] private float _healthDropTime;
        private Coroutine _healthChangeCoroutine = null;

        private int _cachedHealth;
        private HealthModelsSO _healthModel;
        private float _timer;


        public void Initialize(HealthModelsSO model)
        {
            _healthModel = model;
            _health.fillAmount = _healthModel.Health / _healthModel.MaxHealth;
            _rallyHealth.fillAmount = _healthModel.Health / _healthModel.MaxHealth;
            _cachedHealth = _healthModel.Health;
            _coreAnimator.SetFloat("Health", _healthModel.Health * 100 / _healthModel.MaxHealth);
        }

        public void AlterHealthBar()
        {
            if(_healthChangeCoroutine != null)
            {
                _rallyHealth.fillAmount = (float) _cachedHealth / _healthModel.MaxHealth;
                StopCoroutine(_healthChangeCoroutine);
                _healthChangeCoroutine = null;
            }
            _health.fillAmount = (float) _healthModel.Health / _healthModel.MaxHealth;
            _healthChangeCoroutine = StartCoroutine(ChangeRallyHealth(_cachedHealth));
            _cachedHealth = _healthModel.Health;
            _coreAnimator.SetFloat("Health", _healthModel.Health * 100 / _healthModel.MaxHealth);
        }

        IEnumerator ChangeRallyHealth(int startRallyHealth)
        {
            _timer = 0f;
            while(_timer <= 1)
            {
                _rallyHealth.fillAmount = (float)((float)Mathf.Lerp(startRallyHealth, _healthModel.Health, _timer) / _healthModel.MaxHealth);
                _timer += Time.deltaTime / _healthDropTime;
                yield return new WaitForEndOfFrame();
            }
            _timer = 1f;
            _rallyHealth.fillAmount = Mathf.Lerp(startRallyHealth, _healthModel.Health, 1) / _healthModel.MaxHealth;
        }
    }
}