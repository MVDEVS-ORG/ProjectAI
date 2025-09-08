using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;
using Zenject;

[RequireComponent(typeof(ZenAutoInjecter))]
public class ControllerNavigation : MonoBehaviour
{
    [Inject] IUniversalDeviceController _universalDeviceController;

    public static List<ControllerNavigation> _activeNavigators = new();

    [SerializeField] private GameObject _firstSelectedObject;
    [SerializeField] private Button _primaryBackButton;

    InputActionAsset _inputAction;

    private void OnDeviceChanged(ControllerType controller)
    {
        if (controller == ControllerType.GamePad)
        {
            _universalDeviceController.SetGameObjectUI(_firstSelectedObject);
        }
        else
        {
            _universalDeviceController.SetGameObjectUI(null);
        }
    }

    public void ForcedSelectionAfterInitialization()
    {
        ControllerType controller = _universalDeviceController.GetCurrentActiveDevice();
        if (controller == ControllerType.GamePad)
        {
            EventSystem.current.SetSelectedGameObject(_firstSelectedObject);
        }
        else
        {
            EventSystem.current.SetSelectedGameObject(_firstSelectedObject);
        }
    }

    private async void OnEnable()
    {
        await Awaitable.EndOfFrameAsync();
        _activeNavigators.Add(this);
        _universalDeviceController.OnGamePadSetUI(_firstSelectedObject);
        _universalDeviceController.OnDeviceChanged += OnDeviceChanged;
        _inputAction = EventSystem.current.GetComponent<InputSystemUIInputModule>().actionsAsset;
        _inputAction.FindActionMap("UI").FindAction("Back").performed += OnBackAction;
        _inputAction.FindActionMap("UI").FindAction("Back").Enable();
    }

    private void OnBackAction(InputAction.CallbackContext context)
    {
        _primaryBackButton?.onClick.Invoke();
    }

    private void OnApplicationFocus(bool focus)
    {
        if (focus)
        {
            _universalDeviceController.OnGamePadSetUI(_firstSelectedObject);
        }
    }

    private void OnDisable()
    {
        _activeNavigators.Remove(this);
        if (_activeNavigators != null && _activeNavigators.Count > 0)
        {
            _activeNavigators[_activeNavigators.Count - 1].ForcedSelectionAfterInitialization();
        }
        _universalDeviceController.OnDeviceChanged -= OnDeviceChanged;
        _inputAction.FindActionMap("UI").FindAction("Back").performed -= OnBackAction;
        _inputAction.FindActionMap("UI").FindAction("Back").Disable();
    }
}
