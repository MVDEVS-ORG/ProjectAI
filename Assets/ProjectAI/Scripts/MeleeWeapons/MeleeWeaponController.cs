using Unity.VisualScripting;
using UnityEngine;

public class MeleeWeaponController : IMeleeWeaponController
{
    private MeleeWeaponModel _model;
    private MeleeWeaponView _view;

    private Transform _playerTransform;
    private Transform _cursorTransform;

    void Initialize(Transform playerTransform, Transform cursorTransform)
    {
        _playerTransform = playerTransform;
        _cursorTransform = cursorTransform;
    }
    void IMeleeWeaponController.SetupWeapon(MeleeWeaponView view)
    {
        _view = view;
        _model = view.SetupAndActivate(_playerTransform,_cursorTransform);
    }

    void IMeleeWeaponController.MeleeAttack()
    {

    }
}
