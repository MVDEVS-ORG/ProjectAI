using UnityEngine;

public interface IGunUI
{
    void Initialize(GunsModel model, Transform playerTransform);
    void UpdateCoolDown();
    void SpecialEffects();
}
