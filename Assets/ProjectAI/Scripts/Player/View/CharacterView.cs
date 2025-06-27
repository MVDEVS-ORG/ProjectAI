using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterView : MonoBehaviour
{
    private IPlayerController _playerController;
    private PlayerModel _playerModel;

    private Rigidbody2D _rigidBody;
    private Vector2 _moveInput;

    private GameObject _BulletCursor = null;
    private GameObject _bulletCursorUI = null;
    private PlayerInput _playerInput;
    private Vector3 _lastValidDirection = Vector3.right;

    private bool _isControllerInUse = false;
    private Vector2 _rollDirection;

    private SpriteRenderer _spriteRenderer;

    public void Initialize(IPlayerController playerController, PlayerModel playerModel, GameObject bulletCursor, GameObject bulletCursorUI)
    {
        _playerController = playerController;
        _playerModel = playerModel;
        _BulletCursor = bulletCursor;
        _bulletCursorUI = bulletCursorUI;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;
        _playerInput = GetComponent<PlayerInput>();
        InputSystem.onDeviceChange += OnDeviceChange;
        CheckInitialControlSchema();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_playerController != null && _playerController.Initialized)
        {
            if (_playerController.MoveState == State.Moving)
            {
                _moveInput = _playerInput.actions.FindAction("Move").ReadValue<Vector2>();
            }
            switch (_playerController.MoveState)
            {
                case State.Moving:
                    _rigidBody.linearVelocity = _moveInput * _playerModel.Speed;
                    break;

                case State.RollDash:
                    _rigidBody.linearVelocity = _rollDirection * _playerModel.RollSpeed;
                    break;
            }
            TurnCharacter();
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
        if (_BulletCursor != null && _playerInput.currentControlScheme == "KBM")
        {
            Vector2 position = _playerInput.actions.FindAction("LookMouse").ReadValue<Vector2>();
            Vector3 posInWorldSpace = Camera.main.ScreenToWorldPoint(position);
            posInWorldSpace.z = 0;
            _BulletCursor.transform.position = posInWorldSpace;
        }
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
            _playerController.Shoot(true);
        }
        if (context.canceled)
        {
            _playerController.Shoot(false);
        }
    }

    public void Dash(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            _rollDirection = _playerController.Dash(_moveInput);
        }
    }

    

    #region Control schema
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

    private void CheckInitialControlSchema()
    {
        if(_playerInput.currentControlScheme == "KBM")
        {
            if(Gamepad.all.Count > 0)
            {
                _playerInput.SwitchCurrentControlScheme("Controller", Gamepad.current);
            }
        }
        else if(Gamepad.all.Count == 0)
        {
            _playerInput.SwitchCurrentControlScheme("KBM", Keyboard.current, Mouse.current);
        }
    }

    #endregion

    #region to be deleted or improved
    public void TurnCharacter()
    {
        if (_bulletCursorUI!=null && _spriteRenderer!=null)
        {
            float angle = Mathf.Atan2(_bulletCursorUI.transform.position.y - transform.position.y, _bulletCursorUI.transform.position.x - transform.position.x);
            if (MathF.Abs(angle) > (MathF.PI / 4f) && MathF.Abs(angle) < (MathF.PI * 3f / 4f))
            {
                if (angle > 0)
                {
                    _spriteRenderer.sprite = _playerModel.UpSprite;
                }
                else
                {
                    _spriteRenderer.sprite = _playerModel.DownSprite;
                }
            }
            else
            {
                if (MathF.Abs(angle) > MathF.PI / 2f)
                {
                    _spriteRenderer.sprite = _playerModel.LeftSprite;
                }
                else
                {
                    _spriteRenderer.sprite = _playerModel.RightSprite;
                }
            }
        }
    }
    #endregion
}
