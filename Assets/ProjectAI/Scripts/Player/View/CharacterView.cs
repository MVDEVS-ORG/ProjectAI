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

    private IGamePauseController _gamePauseController;

    private GameObject _BulletCursor = null;
    private GameObject _bulletCursorUI = null;
    private PlayerInput _playerInput;
    private Vector3 _lastValidDirection = Vector3.right;
    private Vector2 _lastDamageTickDirection = Vector2.zero;
    private Vector2 _meleeDashDirection = Vector2.zero;

    private Vector2 _rollDirection;

    private SpriteRenderer _spriteRenderer;

    private List<GameObject> _interactableObjects = new();

    private FlashFeedback _flashFeedback;

    private SignalBus _signalBus;

    private float _kickBackSpeed = 1f;
    private Vector2 _kickBackDirection = Vector2.one;

    private float _temporarySpeedHolder = 0f;
    private Coroutine _KickbackBoostcoroutine = null;


    public void Initialize(IPlayerController playerController, PlayerModel playerModel, GameObject bulletCursor, GameObject bulletCursorUI, SignalBus signalBus, IGamePauseController gamePauseController)
    {
        _gamePauseController = gamePauseController;
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

    private void OnDestroy()
    {
        InputSystem.onDeviceChange -= OnDeviceChange;
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

                case State.MeleeDash:
                    _rigidBody.linearVelocity = _meleeDashDirection * _playerModel.MeleeDashSpeed;
                    break;

                case State.KickBack:
                    _rigidBody.linearVelocity = _kickBackDirection * _kickBackSpeed;
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
        if (_playerModel != null)
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, _playerModel.XPCollectionRadius);
            foreach (Collider2D collider in colliders)
            {
                if (collider.TryGetComponent<XPParticle>(out XPParticle particle))
                {
                    if (!particle.MoveToPlayer )
                    {
                        particle.CollectParticle(this);
                    }
                    else
                    {
                        particle.MoveToPlayer = true;
                    }
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
            else
            {
                if (Time.timeScale != 0)
                {
                    Cursor.lockState = CursorLockMode.Confined;
                    Cursor.visible = false;
                }
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
                _playerController.PickUpNewPlayerGun(gun);
            }
            else if (_interactableObjects[0].TryGetComponent(out IInteractable interaction))
            {
                interaction.Interact(transform);
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

    public void SetMeleeDashDirection(Vector2 Direction)
    {
        _meleeDashDirection = Direction;
    }

    public void SwapWeapons(InputAction.CallbackContext context)
    {
        if (!_playerController.Initialized) return;
        if (context.performed)
        {
            float val = context.ReadValue<float>();
            _playerController.SwapWeapons((int)val);
        }
    }

    public void ActivateAbility(InputAction.CallbackContext context)
    {
        if (!_playerController.Initialized) return;
        if (context.performed)
        {
            _playerController.ActivateAbility();
        }
    }

    public void PauseGame(InputAction.CallbackContext context)
    {
        if (!_playerController.Initialized) return;
        if(context.performed && !_gamePauseController.IsPaused)
        {
            _gamePauseController.PauseGame();
        }
    }

    #endregion

    public void TakeDamage(int damage,Vector2 damagePosition, float kickbackMultiplier = 0f)
    {
        if (!_playerController.Initialized || _playerController.IsInvincible) return;
        _playerController.TakeDamage(damage);
        _lastDamageTickDirection = (new Vector2(transform.position.x,transform.position.y) - damagePosition).normalized * kickbackMultiplier;
        _flashFeedback.Flash(_playerModel.InvincibilityTime);
    }

    public void SetKickBackStrength(float strength, Vector2 direction)
    {
        _kickBackSpeed = strength;
        _kickBackDirection = direction;
    }

    public void ExternalKickBack(float strength, Vector2 sourceOfKickback, float duration)
    {
        Vector2 direction = (new Vector2(transform.position.x, transform.position.y) - sourceOfKickback).normalized;
        if (Vector2.Dot(direction, _moveInput) < 0.3)
        {
            _playerController.KickBack(strength, duration, direction);
        }
        else
        {
            if (_KickbackBoostcoroutine != null)
            {
                StopCoroutine(_KickbackBoostcoroutine);
                _playerModel.Speed = _temporarySpeedHolder;
                _KickbackBoostcoroutine = null;
            }
            _temporarySpeedHolder = _playerModel.Speed;
            _KickbackBoostcoroutine = StartCoroutine(AddSpeed(duration, strength));
        }
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
                if (Time.timeScale != 0)
                {
                    Cursor.lockState = CursorLockMode.Confined;
                    Cursor.visible = false;
                }
            }
        }
        catch (Exception exception)
        {
            Debug.Log(exception);
            _playerInput.SwitchCurrentControlScheme("KBM", Keyboard.current, Mouse.current);
            if (Time.timeScale != 0)
            {
                Cursor.lockState = CursorLockMode.Confined;
                Cursor.visible = false;
            }
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
            if (Time.timeScale != 0)
            {
                Cursor.lockState = CursorLockMode.Confined;
                Cursor.visible = false;
            }
        }
    }

    #endregion

    public void AddXP(int xp)
    {
        _playerController.AddXP(xp);
    }

    IEnumerator AddSpeed(float duration, float strength)
    {
        _playerModel.Speed += strength;
        yield return Awaitable.WaitForSecondsAsync(duration);
        _playerModel.Speed = _temporarySpeedHolder;
    }

}
