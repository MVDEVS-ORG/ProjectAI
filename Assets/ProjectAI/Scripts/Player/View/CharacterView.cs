using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterView : MonoBehaviour
{
    private IPlayerController _playerController;
    private PlayerModel _playerModel;

    private Rigidbody2D _rigidBody;
    private Vector2 _moveInput;

    private GameObject _BulletCursor = null;
    private PlayerInput _playerInput;
    private Vector3 _lastValidDirection = Vector3.right;

    private bool _isControllerInUse = false;

    public void Initialize(IPlayerController playerController, PlayerModel playerModel, GameObject bulletCursor)
    {
        _playerController = playerController;
        _playerModel = playerModel;
        _BulletCursor = bulletCursor;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;
        _playerInput = GetComponent<PlayerInput>();
        InputSystem.onDeviceChange += OnDeviceChange;
    }

    // Update is called once per frame
    void Update()
    {
        if (_playerController != null && _playerController.Initialized)
        {
            _rigidBody.linearVelocity = _moveInput * _playerModel.Speed;
        }
        if (_playerInput.currentControlScheme == "Controller" && _BulletCursor != null)
        {
            Vector2 direction = _playerInput.actions.FindAction("Look").ReadValue<Vector2>();
            Vector3 dir = direction;
            if (dir.magnitude >= 0.25f)
            {
                _lastValidDirection = dir / Vector3.Magnitude(dir);
            }
            _BulletCursor.transform.position = transform.position + (_lastValidDirection * _playerModel.CursorDistance);
        }
    }

    public void Move(InputAction.CallbackContext context)
    {
        _moveInput = context.ReadValue<Vector2>();
    }

    public void OnApplicationFocus(bool focus)
    {
        if (focus == true)
        {
            if (Gamepad.all.Count > 0)
            {
                _playerInput.SwitchCurrentControlScheme("Controller", Gamepad.current);
            }
        }
    }

    public void Shoot(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("Performed");
        }
        if (context.canceled)
        {
            Debug.Log("Cancelled");
        }
    }

    public void PointerCursorMouse(InputAction.CallbackContext context)
    {
        if (_BulletCursor != null)
        {
            Vector2 position = context.ReadValue<Vector2>();
            Vector3 posInWorldSpace = Camera.main.ScreenToWorldPoint(position);
            posInWorldSpace.z = 0;
            _BulletCursor.transform.position = posInWorldSpace;
        }
    }

    public void InputChange(PlayerInput controller)
    {
        Debug.Log("Changed Input");
        Debug.Log(controller.currentControlScheme);
    }

    private void OnDeviceChange(InputDevice device, InputDeviceChange change)
    {
        try
        {
            if (device is Gamepad && !(change == InputDeviceChange.Disconnected || change == InputDeviceChange.Disabled))
            {
                _playerInput.SwitchCurrentControlScheme("Controller", Gamepad.current);
            }
            else
            {
                _playerInput.SwitchCurrentControlScheme("KBM", Keyboard.current, Mouse.current);
            }
        }
        catch (Exception exception)
        {
            Debug.Log(exception);
            _playerInput.SwitchCurrentControlScheme("KBM", Keyboard.current, Mouse.current);
        }
    }
}
