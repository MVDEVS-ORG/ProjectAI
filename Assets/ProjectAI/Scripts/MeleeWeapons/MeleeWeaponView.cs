using System.Collections;
using UnityEngine;

public class MeleeWeaponView : MonoBehaviour
{
    [SerializeField] private MeleeWeaponSO _meleeData;
    private float _lastAngle = 0;
    private MeleeWeaponModel _model;
    private Transform _playerTransform;
    private Transform _cursorTransform;
    private Coroutine _attackCoroutine;

    public MeleeWeaponModel SetupAndActivate(Transform playerTransform, Transform cursorTransform)
    {
        _playerTransform = playerTransform;
        _cursorTransform = cursorTransform;
        _model = new MeleeWeaponModel(_meleeData);
        return _model;
    }

    public void Attack(int attackNo)
    {
        if (_playerTransform != null && _cursorTransform != null && _attackCoroutine==null)
        {
            float startingAngle= 0;
            float endAngle = 0;
            Vector2 direction = (_cursorTransform.position - _playerTransform.position).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x);
            if (attackNo % 2 == 0)
            {
                startingAngle = angle - Mathf.PI / 2;
                endAngle = angle + Mathf.PI / 2;
            }
            else
            {
                startingAngle = angle + Mathf.PI / 2;
                endAngle = angle - Mathf.PI / 2;
            }
            _attackCoroutine = StartCoroutine(MeleeMotion(startingAngle, endAngle));
        }
    }

    public bool IsAttacking()
    {
        return _attackCoroutine != null;
    }

    IEnumerator MeleeMotion(float startAngle,float endAngle)
    {
        Debug.LogError("Triggered melee motion ");
        float angle = startAngle;
        Debug.LogError(startAngle < endAngle ? "true" : "falseeer");
        while(startAngle < endAngle ? angle < endAngle : angle > endAngle)
        {
            angle = angle + Time.deltaTime / (_model.AttackTime);
            transform.position = _playerTransform.position + new Vector3(_model.DistanceFromPlayer * Mathf.Cos(angle), _model.DistanceFromPlayer * Mathf.Sin(angle),0);
            transform.right = new Vector3(Mathf.Cos(angle),  Mathf.Sin(angle),0); 
            yield return Awaitable.EndOfFrameAsync();
        }
        _attackCoroutine = null;
    }    

}
