using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class CamEffectsSignal
{
    public Vector3 Direction;
    public Vector3 Position;
    public CamEffect CamEffect;

    public CamEffectsSignal(SignalEffect build)
    {
        Direction = build.Direction;
        Position = build.Position;
        CamEffect = build.CamEffect;
    }

    public class SignalEffect
    {
        public Vector3 Direction = Vector3.zero;
        public Vector3 Position = Vector3.zero;
        public CamEffect CamEffect = CamEffect.CamShake;

        public SignalEffect WithDirection(Vector3 direction)
        {
            this.Direction = direction;
            return this;
        }

        public SignalEffect WithPosition(Vector3 position)
        {
            this.Position = position;
            return this;
        }

        public SignalEffect WithEffect(CamEffect camEffect)
        {
            this.CamEffect = camEffect;
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
    CamShake,
    CamWobble
}

