using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] private Image _heartShape;
    [SerializeField] private Image _health;
    [SerializeField] private Image _rallyHealth;
    private int _cachedHealth;
    private PlayerModel _model;
    private float _timer;
    [Range(0.1f,3f)][SerializeField] private float _healthDropTime;
    private Coroutine _healthChangeCoroutine = null;

    public void Initialize(PlayerModel model)
    {
        _model = model;
        _health.fillAmount = _model.Health / _model.MaxHealth;
        _rallyHealth.fillAmount = _model.Health / _model.MaxHealth;
        _cachedHealth = model.Health;
    }

    public void AlterHealthBar()
    {
        if(_healthChangeCoroutine!=null)
        {
            _rallyHealth.fillAmount =(float) _cachedHealth / _model.MaxHealth;
            StopCoroutine( _healthChangeCoroutine );
            _healthChangeCoroutine = null;
        }
        _health.fillAmount = (float)_model.Health / _model.MaxHealth;
        _healthChangeCoroutine = StartCoroutine(ChangeRallyHealth(_cachedHealth));
        _cachedHealth = _model.Health;
    }

    IEnumerator ChangeRallyHealth(int startRallyHealth)
    {
        _timer = 0f;
        while (_timer <= 1)
        {
            _rallyHealth.fillAmount = (float)((float)Mathf.Lerp(startRallyHealth, _model.Health, _timer) / _model.MaxHealth);
            _timer += Time.deltaTime / _healthDropTime;
            yield return new WaitForEndOfFrame();
        }
        _timer = 1f;
        _rallyHealth.fillAmount = Mathf.Lerp(startRallyHealth, _model.Health, 1) / _model.MaxHealth;
    }
}
