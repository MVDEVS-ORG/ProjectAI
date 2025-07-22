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
    private bool _isInitialized = false;
    private bool _active = false;

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
        _isInitialized = true;
    }

    private void ApplyCameraEffect(CamEffectsSignal camEffect)
    {
        if (!_isInitialized || !_active)
        {
            return;
        }
        if (_currentCamEffect != null)
        {
            _camShakeComponent.StopCoroutine(_currentCamEffect);
            _currentCamEffect = null;
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
        _camShakeComponent.FrequencyGain = signal.Frequency;
        _camShakeComponent.AmplitudeGain = signal.Amplitude;
        float timer = 0f;
        while (timer < 1)
        {
            timer += Time.deltaTime/signal.Duration;
            yield return Awaitable.EndOfFrameAsync();
        }
        _camShakeComponent.AmplitudeGain = 0f;
    }
}
