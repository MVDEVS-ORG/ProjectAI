using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class CamEffectsSignal
{
    public float Amplitude;
    public float Frequency;
    public float Duration;
    public CamEffect CamEffect;

    public CamEffectsSignal(SignalEffect build)
    {
        Amplitude = build.Amplitude;
        Frequency = build.Frequency;
        CamEffect = build.CamEffect;
        Duration = build.Duration;
    }

    public class SignalEffect
    {
        public float Amplitude = 1f;
        public float Frequency = 1f;
        public float Duration = 1f; 
        public CamEffect CamEffect = CamEffect.CamShakeConstant;

        public SignalEffect WithAmplitude(float amplitude)
        {
            this.Amplitude = amplitude;
            return this;
        }

        public SignalEffect WithFrequency(float frequency)
        {
            this.Frequency = frequency;
            return this;
        }

        public SignalEffect WithEffect(CamEffect camEffect)
        {
            this.CamEffect = camEffect;
            return this;
        }

        public SignalEffect WithDuration(float duration)
        {
            this.Duration = Mathf.Abs(duration);
            return this;
        }

        public CamEffectsSignal Build()
        {
            return new CamEffectsSignal(this);
        }
    }
}

public enum CamEffect
{
    CamShakeConstant,
    CamShakeDecreasing,
    CamWobble
}

