using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] private Image _heartShape;
    [SerializeField] private Image _health;
    [SerializeField] private Image _rallyHealth;
    [SerializeField] private Animator _heartAnimator;
    [SerializeField] private Image _xpBar;
    [SerializeField] private TMP_Text _levelText;
    [SerializeField] private Image _xpOutline;
    private int _cachedHealth;
    private PlayerModel _model;
    private float _timer;
    [Range(0.1f,3f)][SerializeField] private float _healthDropTime;
    private Coroutine _healthChangeCoroutine = null;
    private List<int> _xpLevelMap = new List<int>();

    public void Initialize(PlayerModel model, List<int> levelMap)
    {
        _model = model;
        _health.fillAmount = _model.Health / _model.MaxHealth;
        _rallyHealth.fillAmount = _model.Health / _model.MaxHealth;
        _cachedHealth = model.Health;
        _heartAnimator.SetFloat("Health", _model.Health * 100 / _model.MaxHealth );
        _xpLevelMap = levelMap;
        UpdateXpBar();
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
        _heartAnimator.SetFloat("Health", _model.Health * 100 / _model.MaxHealth );
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

    public void UpdateXpBar()
    {
        _xpBar.fillAmount = (float)_model.Experience/_xpLevelMap[_model.PlayerLevel];
        _levelText.text = $"{_model.PlayerLevel + 1}";
    }
}
