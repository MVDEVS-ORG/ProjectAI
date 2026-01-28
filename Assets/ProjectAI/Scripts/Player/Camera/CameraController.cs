using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using Zenject;

public class CameraController
{
    [Inject] private SignalBus _signalBus;

    private CinemachineCamera _cam;
    private CinemachineBasicMultiChannelPerlin _camShakeComponent;
    private Coroutine _currentCamEffect;
    private Coroutine _camScaleCoroutine;
    private bool _isInitialized = false;
    private bool _active = false;
    private float _camSize = 5;

    public void InitializeCamera(CinemachineCamera cam)
    {
        _cam = cam;
        _camShakeComponent = cam.GetComponent<CinemachineBasicMultiChannelPerlin>();
        _signalBus.Subscribe<CamEffectsSignal>(ApplyCameraEffect);
        _active = true;
    }

    public void Initialize(Transform player)
    {
        _cam.Target.TrackingTarget = player;
        if (_camScaleCoroutine != null)
        {
            _cam.StopCoroutine(_camScaleCoroutine);
            _camScaleCoroutine = null;
        }
        _camScaleCoroutine = _cam.StartCoroutine(CamScaling(_camSize));
        _isInitialized = true;
    }

    private void ApplyCameraEffect(CamEffectsSignal camEffect)
    {
        if (!_isInitialized || !_active)
        {
            return;
        }

        switch (camEffect.CamEffect)
        {
            case CamEffect.CamShakeConstant:
                _currentCamEffect = _camShakeComponent.StartCoroutine(CamShakeConstant(camEffect));
                break;
            case CamEffect.CamWobble:
                Debug.Log("Cam wobble");
                //_currentCamEffect = _camShakeComponent.StartCoroutine(CamWobble());
                break;
        }
    }

    IEnumerator CamShakeConstant(CamEffectsSignal signal)
    {
        float timer = 0f;
        while (timer < 1)
        {
            timer += Time.deltaTime / signal.Duration;
            _camShakeComponent.FrequencyGain = signal.Frequency;
            _camShakeComponent.AmplitudeGain = signal.Amplitude;
            yield return Awaitable.EndOfFrameAsync();
        }
        float fadeTimer = 0f;
        while (fadeTimer < 1)
        {
            fadeTimer += Time.deltaTime / signal.FadeDuration;
            _camShakeComponent.AmplitudeGain = Mathf.Lerp(signal.Amplitude, 0, fadeTimer);
            yield return Awaitable.EndOfFrameAsync();
        }
        _camShakeComponent.AmplitudeGain = 0f;
        _currentCamEffect = null;
    }

    public void DetachCamera(Transform holdPosition, float camSize)
    {
        _cam.Target.TrackingTarget = holdPosition;
        if (_camScaleCoroutine != null)
        {
            _cam.StopCoroutine(_camScaleCoroutine);
            _camScaleCoroutine = null;
        }
        _camScaleCoroutine = _cam.StartCoroutine(CamScaling(camSize));
    }

    IEnumerator CamScaling(float value)
    {
        float timer = 0f;
        float camStartSize = _cam.Lens.OrthographicSize;
        while (timer < 1)
        {
            _cam.Lens.OrthographicSize = Mathf.Lerp(camStartSize, value, timer);
            timer += Time.deltaTime;
            yield return Awaitable.EndOfFrameAsync();
        }
        _cam.Lens.OrthographicSize = value;
    }

}
