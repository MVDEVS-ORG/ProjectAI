using System;
using System.Collections.Generic;
using UnityEngine;

public interface IGunsController
{
    Awaitable IntializeOnSceneLoad(Transform playerTransform, Transform playerCursor);
    Awaitable InitializeGunsOnSceneLoad(string defaultGunAddress);
    void SetCurrentActiveGun(GunsView gun);
    void Fire(bool firing);
    Awaitable ReplaceGuns(GunsView gun);
    GunsView View { get; }
    Awaitable AddGun(GunsView gun);
    void ChangeGunLimit(int limit);
    void SwapGuns(int updown);
    void FireAllGuns(bool toggle, bool alternateAbility);
    void SetGunElements(Dictionary<ElementEnum, int> ElementalBuffs);
    event Action<GunsView> OnGunSwap;
    public void SavePlayerGuns();
    public (string, List<string>) LoadPlayerGuns();
}
