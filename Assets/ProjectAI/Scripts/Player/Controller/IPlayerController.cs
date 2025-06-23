using UnityEngine;

public interface IPlayerController
{
    Awaitable SpawnPlayer(Vector3 pos, PlayerCharactersSO playerCharcter);
    bool Initialized { get; }
    bool MovementPossible { get; }
    void TakeDamage(int damage);
    void RestoreHealth(int health);
    void Shoot(Vector2 direction);
    State MoveState { get; }
    Vector2 Dash(Vector2 MoveInput);
    Transform GetPlayerTransform();
}

public enum State
{
    Moving,
    RollDash
}
