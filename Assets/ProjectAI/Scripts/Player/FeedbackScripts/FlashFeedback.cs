using System.Collections;
using UnityEngine;

public class FlashFeedback : MonoBehaviour
{
    [SerializeField] private AnimationCurve _flashFeedbackCurve;
    private Material _flashMaterial;

    private Coroutine _flashCoroutine;

    private void Start()
    {
        _flashMaterial = GetComponent<SpriteRenderer>().material;
    }

    public void Flash(float duration)
    {
        if (_flashCoroutine!=null)
        {
            StopCoroutine(_flashCoroutine);
            _flashCoroutine = null;
        }
        _flashCoroutine = StartCoroutine(CauseFlash(duration));
    }

    IEnumerator CauseFlash(float duration)
    {
        float timer = 0f;
        while(timer<=duration)
        {
            _flashMaterial.SetFloat("_FlashAmount", _flashFeedbackCurve.Evaluate(timer/duration));
            timer += Time.deltaTime;
            yield return Awaitable.EndOfFrameAsync();
        }
        timer = duration;
        _flashMaterial.SetFloat("_FlashAmount", _flashFeedbackCurve.Evaluate(timer / duration));
    }

}
