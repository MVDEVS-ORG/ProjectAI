using UnityEngine;

public interface IGunsController
{
    void SetCurrentActiveGun(GunsView gun, Transform playerTransform, Transform playerCursor);
    void Fire(bool firing);
    void SwapGuns(GunsView gun);
}
