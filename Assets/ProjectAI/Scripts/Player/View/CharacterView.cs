using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

[RequireComponent(typeof(FlashFeedback))]
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
    private Vector2 _lastDamageTickDirection = Vector2.zero;

    private Vector2 _rollDirection;

    private SpriteRenderer _spriteRenderer;

    private List<GameObject> _interactableObjects = new();

    private FlashFeedback _flashFeedback;

    private SignalBus _signalBus;


    public void Initialize(IPlayerController playerController, PlayerModel playerModel, GameObject bulletCursor, GameObject bulletCursorUI, SignalBus signalBus)
    {
        _playerController = playerController;
        _playerModel = playerModel;
        _BulletCursor = bulletCursor;
        _bulletCursorUI = bulletCursorUI;
        _signalBus = signalBus;
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
        _flashFeedback = GetComponent<FlashFeedback>();
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

                case State.TakeDamage:
                    _rigidBody.linearVelocity = _lastDamageTickDirection * _playerModel.DamageKickBackSpeed;
                    break;
            }
            //TurnCharacter();
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

        #region check overlay colliders for particles
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, _playerModel.XPCollectionRadius);
        foreach (Collider2D collider in colliders)
        {
            if(collider.TryGetComponent<XPParticle>(out XPParticle particle))
            {
                if(particle.MoveToPlayer!=false)
                {
                    particle.CollectParticle(transform);
                }
            }
        }
        #endregion
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

    #region button inputs
    public void Shoot(InputAction.CallbackContext context)
    {
        if (!_playerController.Initialized) return;
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
        if (!_playerController.Initialized) return;
        if (context.performed)
        {
            Debug.LogError(_playerModel.GetHashCode());
            _rollDirection = _playerController.Dash(_moveInput);
        }
    }

    public void Interact(InputAction.CallbackContext context)
    {
        if (!_playerController.Initialized) return;
        if (context.performed && _interactableObjects.Count>0)
        {
            if (_interactableObjects[0].TryGetComponent<GunsView>(out GunsView gun))
            {
                _playerController.SwapPlayerGuns(gun);
            }
            else if (_interactableObjects[0].TryGetComponent(out IInteractable interaction))
            {
                interaction.Interact();
            }
        }
    }
     
    public void MeleeAttack(InputAction.CallbackContext context)
    {
        if (!_playerController.Initialized) return;
        if(context.performed)
        {
            _playerController.MeleeAttack();
        }
    }

    public void TestThings(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _playerController.Test();
        }
    }

    #endregion

    public void TakeDamage(int damage,Vector2 direction, float kickbackMultiplier = 0f)
    {
        if (!_playerController.Initialized || _playerController.IsInvincible) return;
        _playerController.TakeDamage(damage);
        _lastDamageTickDirection = (new Vector2(transform.position.x,transform.position.y) - direction).normalized * kickbackMultiplier;
        _flashFeedback.Flash(_playerModel.InvincibilityTime);
    }

    #region collision
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.TryGetComponent(out IInteractable interactableObject))
        {
            _interactableObjects.Add(collision.gameObject);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (_interactableObjects.Contains(collision.gameObject))
        {
            _interactableObjects.Remove(collision.gameObject);
        }
    }
    #endregion

    #region Control schema
    public void InputChange(PlayerInput controller)
    {
        Debug.Log($"Changed Input:  + {controller.currentControlScheme}");
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

}
