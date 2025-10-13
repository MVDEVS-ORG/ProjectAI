using UnityEngine;
using UnityEngine.Rendering.Universal;

public class SimpleLightSwitch : MonoBehaviour
{
    [SerializeField] private GameObject _light;

    private void enable()
    {
        _light.SetActive(true);
    }

    private void disable()
    {
        _light.SetActive(false);
    }
}
