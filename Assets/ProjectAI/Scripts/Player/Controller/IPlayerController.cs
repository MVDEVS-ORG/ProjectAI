using UnityEngine;

public interface IPlayerController
{
    Awaitable SpawnPlayer(Vector3 pos, PlayerCharactersSO playerCharcter);
    bool Initialized { get; }
    bool MovementPossible { get; }
    bool IsInvincible { get; }
    void TakeDamage(int damage);
    void RestoreHealth(int health);
    void Shoot(bool firing);
    State MoveState { get; }
    Vector2 Dash(Vector2 MoveInput);
    Awaitable<Transform> GetPlayerTransform();
    void SwapPlayerGuns(GunsView gun);
    void EnableController(bool enable);
    void MeleeAttack();
    void Test();
}

public enum State
{
    Moving,
    RollDash,
    TakeDamage
}

