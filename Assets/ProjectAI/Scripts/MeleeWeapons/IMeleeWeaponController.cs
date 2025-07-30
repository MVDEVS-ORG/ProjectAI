using UnityEngine;

public interface IMeleeWeaponController
{
    void Initialize(Transform playerTransform, Transform cursorTransform, IPlayerController controller);
    void SetupWeapon(MeleeWeaponView view);
    void MeleeAttack();
    bool Initialized { get; }
    void MeleeAttackDone();
}
