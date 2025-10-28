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
    [Range(0.1f, 3f)][SerializeField] private float _healthDropTime;
    private Coroutine _healthChangeCoroutine = null;
    private List<int> _xpLevelMap = new List<int>();

    //segmented health configs
    [SerializeField] private RectTransform _segmentContainer;
    [SerializeField] private GameObject _segmentPrefab;
    [SerializeField] private int _minSegmentCount = 5;
    [SerializeField] private int _maxSegmentCount = 50;
    private List<PlayerHealthSegment> _segments = new();
    private int _currentSegmentCount = 0;
    private int _initialMaxHp;


    public void Initialize(PlayerModel model, List<int> levelMap)
    {
        _model = model;

        _cachedHealth = model.Health;
        _initialMaxHp = model.MaxHealth;
        RebuildSegmentsForMaxHealth(_model.MaxHealth);

        SetImmediateHealth(_model.Health);
        SetRallyHealth(_model.Health);

        _heartAnimator.SetFloat("Health", _model.Health * 100 / _model.MaxHealth);
        _xpLevelMap = levelMap;
        UpdateXpBar();
    }

    public void AlterHealthBar()
    {
        int newSegmentCount = Mathf.CeilToInt(_model.MaxHealth / ((float)_initialMaxHp / _minSegmentCount));
        if (newSegmentCount != _currentSegmentCount)
        {
            RebuildSegmentsForMaxHealth(_model.MaxHealth);
        }

        if (_healthChangeCoroutine != null)
        {
            SetRallyHealth(_cachedHealth);
            StopCoroutine(_healthChangeCoroutine);
            _healthChangeCoroutine = null;
        }

        SetImmediateHealth(_model.Health);
        _healthChangeCoroutine = StartCoroutine(ChangeRallyHealthSegments(_cachedHealth, _model.Health));
        _cachedHealth = _model.Health;
        _heartAnimator.SetFloat("Health", _model.Health * 100 / _model.MaxHealth);
    }

    private void RebuildSegmentsForMaxHealth(int maxHp)
    {
        //maxhp/(hp per segment) 
        int newSegmentCount = Mathf.Clamp(Mathf.CeilToInt(maxHp / ((float)_initialMaxHp / _minSegmentCount)), _minSegmentCount, _maxSegmentCount);

        if (newSegmentCount == _currentSegmentCount) return;

        while (_segments.Count < newSegmentCount)
        {
            var go = Instantiate(_segmentPrefab, _segmentContainer);
            go.name = "HealthSegment_" + _segments.Count;
            var ctrl = go.GetComponent<PlayerHealthSegment>();
            _segments.Add(ctrl);
        }

        for (int i = 0; i < _segments.Count; i++)
        {
            _segments[i].gameObject.SetActive(i < newSegmentCount);
        }

        _currentSegmentCount = _segments.Count;
        SetImmediateHealth(_model.Health);
        SetRallyHealth(_model.Health);
    }

    private void SetImmediateHealth(int health)
    {
        if (_segments.Count == 0) return;
        float segmentHp = _model.MaxHealth / _segments.Count;

        for (int i = 0; i < _segments.Count; i++)
        {
            float segStart = i * segmentHp;
            float segFill = Mathf.Clamp01((health - segStart) / segmentHp);
            _segments[i].SetImmediateFill(segFill);
        }

        //old code
        // _health.fillAmount = health / _model.MaxHealth;
    }


    private void SetRallyHealth(int health)
    {
        if (_segments.Count == 0) return;
        float segmentHp = _model.MaxHealth / _segments.Count;

        for (int i = 0; i < _segments.Count; i++)
        {
            float segStart = i * segmentHp;
            float segFill = Mathf.Clamp01((health - segStart) / segmentHp);
            _segments[i].SetRallyFill(segFill);
        }
    }


    IEnumerator ChangeRallyHealthSegments(int startHealth, int endHealth)
    {
        float timer = 0f;
        while (timer < 1f)
        {
            float t = timer;
            float interp = Mathf.Lerp(startHealth, endHealth, t);
            SetRallyHealth(Mathf.RoundToInt(interp));
            timer += Time.deltaTime / _healthDropTime;
            yield return null;
        }

        SetRallyHealth(endHealth);
        _healthChangeCoroutine = null;
    }

    public void UpdateXpBar()
    {
        _xpBar.fillAmount = (float)_model.Experience / _xpLevelMap[_model.PlayerLevel];
        _levelText.text = $"{_model.PlayerLevel + 1}";
    }
}
