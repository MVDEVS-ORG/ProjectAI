using UnityEngine;
using UnityEngine.UI;

public interface IGunsController
{
    Awaitable InitializeOnSceneLoad(string gunAddress, Transform playerTransform, Transform playerCursor);
    void SetCurrentActiveGun(GunsView gun);
    void Fire(bool firing);
    Awaitable ReplaceGuns(GunsView gun);
    GunsView View { get; }
    Awaitable AddGun(GunsView gun);
    void ChangeGunLimit(int limit);
    void SwapGuns(int updown);
    void FireAllGuns(bool toggle, bool alternateAbility);
}
