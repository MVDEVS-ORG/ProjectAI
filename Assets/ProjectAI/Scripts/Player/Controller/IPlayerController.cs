using UnityEngine;

public interface IPlayerController
{
    Awaitable SpawnPlayer(Vector3 pos, PlayerCharactersSO playerCharcter);
    bool Initialized { get; }
    bool MovementPossible { get; }
    void TakeDamage(int damage);
    void RestoreHealth(int health);
}
