using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using Zenject;


public class UniversalDeviceController : IUniversalDeviceController
{
    [Inject] ISceneManager _sceneManager;
    public event Action<ControllerType> OnDeviceChanged;
    private ControllerType _currentControllerType;

    [Inject]
    public void Initialize()
    {
        _sceneManager.BeforeChangeScene += RemoveListners;
        InputSystem.onDeviceChange += InputSystemOnDeviceChange;
        OnDeviceChanged?.Invoke((this as IUniversalDeviceController).GetCurrentActiveDevice());
    }

    private void InputSystemOnDeviceChange(InputDevice device, InputDeviceChange change)
    {
        try
        {
            if (device is Gamepad && !(change == InputDeviceChange.Disconnected || change == InputDeviceChange.Disabled))
            {
                SwitchToController();
                
            }
            else
            {
                SwitchToKBM();
                
            }
            OnDeviceChanged?.Invoke(_currentControllerType);
        }
        catch (Exception exception)
        {
            Debug.Log(exception);
            SwitchToKBM();
            OnDeviceChanged?.Invoke(_currentControllerType);
        }
    }

    ControllerType IUniversalDeviceController.GetCurrentActiveDevice()
    {
        if(Gamepad.all.Count>0)
        {
            SwitchToController();
            return ControllerType.GamePad;
        }
        else
        {
            SwitchToKBM();
            return ControllerType.KBM;
        }
    }

    private void SwitchToController()
    {
        _currentControllerType = ControllerType.GamePad;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void SwitchToKBM()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = UIController.LookingAtUI; 
        _currentControllerType = ControllerType.KBM;
    }

    async Awaitable IUniversalDeviceController.SetGameObjectUI(GameObject obj)
    {
        await Awaitable.EndOfFrameAsync();
        EventSystem.current.SetSelectedGameObject(obj);
    }

    void IUniversalDeviceController.OnGamePadSetUI(GameObject obj)
    {
        (this as IUniversalDeviceController).GetCurrentActiveDevice();
        if (_currentControllerType == ControllerType.GamePad)
        {
            EventSystem.current.SetSelectedGameObject(obj);
        }
        else
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
    }

    private void RemoveListners()
    {
        OnDeviceChanged = null;
    }
}
