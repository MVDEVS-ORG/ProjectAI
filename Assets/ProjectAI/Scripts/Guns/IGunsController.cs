using UnityEngine;

public interface IGunsController
{
    Awaitable SetCurrentActiveGun(GunsView gun, Transform playerTransform, Transform playerCursor);
    void Fire(bool firing);
    Awaitable SwapGuns(GunsView gun, Transform playerTransform, Transform playerCursor);
}
