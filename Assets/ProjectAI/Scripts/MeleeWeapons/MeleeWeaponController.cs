using System.Collections;
using UnityEngine;

public class MeleeWeaponController : IMeleeWeaponController
{
    private MeleeWeaponModel _model;
    private MeleeWeaponView _view;

    private Transform _playerTransform;
    private Transform _cursorTransform;

    private bool _initialized = false;

    private Coroutine _attackCoroutine;
    private bool _isLeftSwing = false;
    private CharacterView _playerView;

    public bool Initialized => _initialized;

    void IMeleeWeaponController.Initialize(Transform playerTransform, Transform cursorTransform)
    {
        _playerView = playerTransform.GetComponent<CharacterView>();
        _playerTransform = playerTransform;
        _cursorTransform = cursorTransform;
    }
    void IMeleeWeaponController.SetupWeapon(MeleeWeaponView view)
    {
        _view = view;
        _model = view.SetupAndActivate(_playerTransform,_cursorTransform);
        _initialized = true;
    }

    void IMeleeWeaponController.MeleeAttack()
    {
        if(IsAttacking())
        {
            return;
        }
        else if(_model.Attacks<_model.AttackChainLimit)
        {
            Attack();
            _model.Attacks = _model.Attacks+1;
            _playerView.StartCoroutine(AttackCoolDown());
        }
    }

    IEnumerator AttackCoolDown()
    {
        yield return Awaitable.WaitForSecondsAsync(_model.AttackRechargeDelay);
        _model.Attacks--;
    }

    private void Attack()
    {
        if (_playerTransform != null && _cursorTransform != null && _attackCoroutine == null)
        {
            _isLeftSwing = !_isLeftSwing;
            float startingAngle = 0;
            float endAngle = 0;
            Vector2 direction = (_cursorTransform.position - _playerTransform.position).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x);
            startingAngle = _isLeftSwing ? angle - Mathf.PI / 2 : angle + Mathf.PI / 2;
            endAngle = _isLeftSwing ? angle + Mathf.PI / 2 : angle - Mathf.PI / 2;
            _view.gameObject.SetActive(true);
            _attackCoroutine = _view.StartCoroutine(MeleeMotion(_isLeftSwing, startingAngle, endAngle));
        }
    }

    IEnumerator MeleeMotion(bool isLeftSwing, float startAngle, float endAngle)
    {
        int directionOfMotion = 0;
        directionOfMotion = isLeftSwing ? 1 : -1;
        float angle = startAngle;
        while (directionOfMotion * angle < directionOfMotion * endAngle)
        {
            angle = angle + (directionOfMotion * Time.deltaTime) / (_model.AttackTime);
            _view.transform.position = _playerTransform.position + new Vector3(_model.DistanceFromPlayer * Mathf.Cos(angle), _model.DistanceFromPlayer * Mathf.Sin(angle), 0);
            _view.transform.right = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0);
            yield return Awaitable.EndOfFrameAsync();
        }
        //_view.transform.position = -100 * Vector3.one;
        _attackCoroutine = null;
        _view.gameObject.SetActive(false);
    }

    private bool IsAttacking()
    {
        return _attackCoroutine != null;
    }
}
