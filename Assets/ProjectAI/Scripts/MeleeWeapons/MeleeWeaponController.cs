using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class MeleeWeaponController : IMeleeWeaponController
{
    [Inject] private IUpgradeController _upgradeController;
    [Inject] private IGunsController _gunController;
    [Inject] private GamepadRumble _rumbleController;
    private MeleeWeaponModel _model;
    private MeleeWeaponView _view;

    private Transform _playerTransform;
    private Transform _cursorTransform;

    private bool _initialized = false;

    private Coroutine _attackCoroutine;
    private CharacterView _playerView;
    private IPlayerController _playerController;

    public bool Initialized => _initialized;

    void IMeleeWeaponController.Initialize(Transform playerTransform, Transform cursorTransform, IPlayerController controller)
    {
        _playerController = controller;
        _playerView = playerTransform.GetComponent<CharacterView>();
        _playerTransform = playerTransform;
        _cursorTransform = cursorTransform;
    }



    void IMeleeWeaponController.SetupWeapon(MeleeWeaponView view)
    {
        _view = view;
        _model = view.SetupAndActivate(_playerTransform,_cursorTransform , this);
        _upgradeController.OnUpgrade += UpgradeMeleeWeapon;
        _initialized = true;
        _view.gameObject.SetActive(false);
    }

    void IMeleeWeaponController.MeleeAttack()
    {
        if(_view.gameObject.activeSelf)
        {
            return;
        }
        else if(_model.Attacks<_model.AttackChainLimit)
        {
            _gunController.View.gameObject.SetActive(false); 
            Attack();
            _model.Attacks = _model.Attacks+1;
            _playerView.StartCoroutine(AttackCoolDown());
        }
    }

    private void UpgradeMeleeWeapon(List<UpgradeSO> upgrades)
    {
        foreach (UpgradeSO upgrade in upgrades)
        {
            if (upgrade.UpgradeType == UpgradeType.MeleeWeapon)
            {
                switch (upgrade.Type)
                {
                    case StatType.Additive:
                        _model = _model + upgrade.meleeWeaponModel;
                        break;

                    case StatType.Multiplicative:
                        _model = _model * upgrade.meleeWeaponModel;
                        break;

                    case StatType.Set:
                        _model = _model % upgrade.meleeWeaponModel;
                        break;
                }
            }
        }
    }

    IEnumerator AttackCoolDown()
    {
        yield return Awaitable.WaitForSecondsAsync(_model.AttackRechargeDelay);
        _model.Attacks--;
    }

    private void Attack()
    {
        if (_playerTransform != null && _cursorTransform != null && !_view.gameObject.activeSelf)
        {
            _rumbleController.Rumble(0.1f, 0.2f, 0.3f);
            Vector2 direction = (_cursorTransform.position - _playerTransform.position).normalized;
            _playerController.MeleeDash(direction);
            _view.gameObject.SetActive(true);
            _view.transform.position = new Vector2(_playerTransform.position.x, _playerTransform.position.y) + direction * _model.DistanceFromPlayer;
            _view.transform.right = direction;
            _view.AttackAnimation();
        }
    }

    void IMeleeWeaponController.MeleeAttackDone()
    {
        _gunController.View.gameObject.SetActive(true);
        _gunController.View.OrbitalMotion();
    }
}
