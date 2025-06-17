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
        Cursor.visible = true;
        _playerInput = GetComponent<PlayerInput>();
        //_playerInput.SwitchCurrentActionMap("Controller");
    }

    // Update is called once per frame
    void Update()
    {
        if (_playerController!=null && _playerController.Initialized)
        {
            _rigidBody.linearVelocity = _moveInput * _playerModel.Speed;
        }
        if(_playerInput.currentActionMap == _playerInput.actions.FindActionMap("Controller") && _BulletCursor!=null)
        {
            Vector2 direction = _playerInput.actions.FindAction("Look").ReadValue<Vector2>();
            Vector3 dir = direction;
            if (dir.magnitude >= 0.25f)
            {
                _lastValidDirection = dir/Vector3.Magnitude(dir);
            }
            _BulletCursor.transform.position = transform.position + (_lastValidDirection * _playerModel.CursorDistance);
        }
    }

    public void Move(InputAction.CallbackContext context)
    {
        _moveInput = context.ReadValue<Vector2>();
    }

    public void Shoot(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            Debug.Log("Performed");
        }
        if(context.canceled)
        {
            Debug.Log("Cancelled");
        }
    }

    public void PointCursor(InputAction.CallbackContext context)
    {
        if (_BulletCursor != null)
        {
            /*Vector2 direction = context.ReadValue<Vector2>();
            Debug.LogError("Raw"+ direction);
            Vector3 dir = new Vector3(direction.x, direction.y, 0);
            Vector3 lastValidDirection = Vector3.zero;
            if (direction.magnitude>0.5f)
            {
                lastValidDirection = dir / Vector3.Magnitude(dir);
            }
            _BulletCursor.transform.position = transform.position + (dir * _playerModel.CursorDistance);*/
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
}
