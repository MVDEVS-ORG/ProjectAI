using System.Collections;
using UnityEngine;

public class MeleeWeaponView : MonoBehaviour
{
    [SerializeField] private MeleeWeaponSO _meleeData;
    private MeleeWeaponModel _model;
    private Transform _playerTransform;
    private Transform _cursorTransform;
    private Coroutine _attackCoroutine;

    public MeleeWeaponModel SetupAndActivate(Transform playerTransform, Transform cursorTransform)
    {
        _playerTransform = playerTransform;
        _cursorTransform = cursorTransform;
        transform.parent = _playerTransform;
        _model = new MeleeWeaponModel(_meleeData);
        return _model;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent(out IHealthSystem health))
        {
            health.TakeDamage(_model.Damage);
        }
    }

}
