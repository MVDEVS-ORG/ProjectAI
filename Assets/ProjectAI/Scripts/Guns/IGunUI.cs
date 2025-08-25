using UnityEngine;

public interface IGunUI
{
    Awaitable Initialize(GunsModel model, Transform playerTransform);
    void UpdateCoolDown();
    Awaitable AddGun(GunsModel model);
    void RemoveGun(GunsModel model, GunsModel switchTo);
    void SwapGun(GunsModel model);
}
