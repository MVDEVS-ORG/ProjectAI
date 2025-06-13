using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterView : MonoBehaviour
{
    private IPlayerController _playerController;
    private PlayerModel _playerModel;

    private Rigidbody2D _rigidBody;
    private Vector2 _moveInput;

    public void Initialize(IPlayerController playerController, PlayerModel playerModel)
    {
        _playerController = playerController;
        _playerModel = playerModel;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        _rigidBody.linearVelocity = _moveInput * _playerModel.Speed;
    }

    public void Move(InputAction.CallbackContext context)
    {
        _moveInput = context.ReadValue<Vector2>();
    }
}
