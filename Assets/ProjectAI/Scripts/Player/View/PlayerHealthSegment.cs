using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthSegment : MonoBehaviour
{
    [SerializeField] private Image _rallyFill;
    [SerializeField] private Image _healthFill;
    public void SetImmediateFill(float normalized)
    {
        _healthFill.fillAmount = Mathf.Clamp01(normalized);
    }

    public void SetRallyFill(float normalized)
    {
        _rallyFill.fillAmount = Mathf.Clamp01(normalized);
    }
}
