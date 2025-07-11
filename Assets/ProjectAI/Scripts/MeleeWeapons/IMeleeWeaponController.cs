using UnityEngine;

public interface IMeleeWeaponController
{
    void Initialize(Transform playerTransform, Transform cursorTransform);
    void SetupWeapon(MeleeWeaponView view);
    void MeleeAttack();
    bool Initialized { get; }
}
