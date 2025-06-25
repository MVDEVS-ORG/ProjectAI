using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GunUI : MonoBehaviour, IGunUI
{
    [SerializeField] private Image _healthBarFill;
    [SerializeField] private GunsModel _gun;
    [SerializeField] private GameObject _gunBar;

    private bool _initialized = false;
    private Transform _playerTransform;
    private void Start()
    {
        _gunBar.SetActive(false);
    }

    void IGunUI.Initialize(GunsModel model, Transform playerTransform)
    {
        _gun = model;
        _playerTransform = playerTransform;
        _initialized = true;
        _gunBar.SetActive(true);
    }

    void IGunUI.SpecialEffects()
    {
        //nothing for simple gun
    }

    void IGunUI.UpdateCoolDown()
    {
        (this as IGunUI).SpecialEffects();
        _healthBarFill.fillAmount = _gun.OverHeatValue / _gun.OverHeatLimit; 
    }

    void Update()
    {
        if (_initialized)
        {
            transform.position = _playerTransform.position;
        }
    }
}
