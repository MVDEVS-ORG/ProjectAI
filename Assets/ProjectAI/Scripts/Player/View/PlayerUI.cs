using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] private Image _heartShape;
    [SerializeField] private Image _health;
    private PlayerModel _model;
    private float _healthFill;
    private float _timer;
    [Range(0.1f,1f)][SerializeField] private float _healthDropTime;
    private Coroutine _healthDropCoroutine = null;

    public void Initialize(PlayerModel model)
    {
        _model = model;
        _healthFill = model.Health / model.MaxHealth;
        _health.fillAmount = _healthFill;

    }

    public void DisplayDamage(int damage)
    {
        if(_healthDropCoroutine!=null)
        {
            StopCoroutine(_healthDropCoroutine);
            _healthDropCoroutine = null;
        }
        _healthDropCoroutine = StartCoroutine(drainHealth(damage));
    }

    IEnumerator drainHealth(int damage)
    {
        _timer = 0f;
        while(_timer<=1)
        {
            var temp = Mathf.Lerp(_model.Health, Mathf.Max(_model.Health - damage, 0), _timer);
            _healthFill = temp / _model.MaxHealth;
            _health.fillAmount = _healthFill;
            _timer += _timer + Time.deltaTime/ _healthDropTime;
            yield return Awaitable.EndOfFrameAsync();
        }
        var t = Mathf.Lerp(_model.Health, Mathf.Max(_model.Health - damage, 0), 1);
        _healthFill = t / _model.MaxHealth;
        _timer += _timer + Time.deltaTime / _healthDropTime;
    }
}
