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

    [SerializeField] private GameObject _firstSelectedObject;
    [SerializeField] private Button _primaryBackButton;

    private GameObject _lastSelectedObject;
    InputActionAsset _inputAction;

    private void OnDeviceChanged(ControllerType controller)
    {
        if (controller == ControllerType.GamePad)
        {
            _universalDeviceController.SetGameObjectUI(_lastSelectedObject!=null?_lastSelectedObject:_firstSelectedObject);
        }
        else
        {
            _universalDeviceController.SetGameObjectUI(null);
        }
    }

    private async void OnEnable()
    {
        await Awaitable.EndOfFrameAsync();
        _universalDeviceController.OnDeviceChanged += OnDeviceChanged;
        _ = _universalDeviceController.OnGamePadSetUI(_lastSelectedObject != null ? _lastSelectedObject : _firstSelectedObject);
        _inputAction = EventSystem.current.GetComponent<InputSystemUIInputModule>().actionsAsset;
        Debug.Log(_inputAction.FindActionMap("UI").FindAction("Back") != null);
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
            _universalDeviceController.OnGamePadSetUI(_lastSelectedObject != null ? _lastSelectedObject : _firstSelectedObject);
        }
    }

    private void OnDisable()
    {
        _lastSelectedObject = EventSystem.current.currentSelectedGameObject??null;
        _universalDeviceController.OnDeviceChanged -= OnDeviceChanged;
        _inputAction.FindActionMap("UI").FindAction("Back").performed -= OnBackAction;
        _inputAction.FindActionMap("UI").FindAction("Back").Disable();
    }
}
