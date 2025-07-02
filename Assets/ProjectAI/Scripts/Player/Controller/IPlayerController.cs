using UnityEngine;

public interface IPlayerController
{
    Awaitable SpawnPlayer(Vector3 pos, PlayerCharactersSO playerCharcter);
    bool Initialized { get; }
    bool MovementPossible { get; }
    void TakeDamage(int damage);
    void RestoreHealth(int health);
    void Shoot(bool firing);
    State MoveState { get; }
    Vector2 Dash(Vector2 MoveInput);
    Awaitable<Transform> GetPlayerTransform();
    void SwapPlayerGuns(GunsView gun);
}

public enum State
{
    Moving,
    RollDash
}
