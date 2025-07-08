using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class MeleeWeaponController : IMeleeWeaponController
{
    private MeleeWeaponModel _model;
    private MeleeWeaponView _view;

    private Transform _playerTransform;
    private Transform _cursorTransform;

    private bool _initialized = false;

    public bool Initialized => _initialized;

    void IMeleeWeaponController.Initialize(Transform playerTransform, Transform cursorTransform)
    {
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

        if(_view.IsAttacking())
        {
            return;
        }
        else
        {
            Debug.LogError("Melee Attack called fomr Melee Weapons controller");
            _view.Attack(_model.Attacks);
            _model.Attacks = Mathf.Clamp(_model.Attacks++,0,_model.AttackChainLimit);
            _view.StartCoroutine(AttackCoolDown());
        }
    }

    IEnumerator AttackCoolDown()
    {
        yield return Awaitable.WaitForSecondsAsync(_model.AttackRechargeDelay);
        _model.Attacks++;
    }
}
