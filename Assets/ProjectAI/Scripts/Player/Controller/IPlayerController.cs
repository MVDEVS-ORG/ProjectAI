using UnityEngine;

public interface IPlayerController
{
    Awaitable SpawnPlayer(Vector3 pos, PlayerCharactersSO playerCharcter);
    bool Initialized { get; }
    bool MovementPossible { get; }
    bool IsInvincible { get; }
    bool GunEnabled { get; set; }
    bool IsAbilityInUse { get; set; }
    void TakeDamage(int damage);
    void RestoreHealth(int health);
    void Shoot(bool firing);
    State MoveState { get; }
    Vector2 Dash(Vector2 MoveInput);
    Awaitable<Transform> GetPlayerTransform();
    void PickUpNewPlayerGun(GunsView gun);
    void EnableController(bool enable);
    void MeleeAttack();
    void Test();
    void AddXP(int xp);
    void MeleeDash(Vector2 Direction);
    void KickBack(float strength, float duration, Vector2 direction);
    void SwapWeapons(int value);
    void ActivateAbility();
}

public enum State
{
    Moving,
    RollDash,
    TakeDamage,
    MeleeDash,
    KickBack
}

