using UnityEngine;

public interface IGunsController
{
    void SetCurrentActiveGun(GunsView gun, Transform playerTransform);
    void Fire(Vector2 direction);
    void SwapGuns(GunsView gun);
}
