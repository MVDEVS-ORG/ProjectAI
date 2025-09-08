using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyElementAccumulation : MonoBehaviour
{
    private EnemyModel _model;
    private Dictionary<ElementEnum, Coroutine> AfflictionCooldowns = new();
    private int _tick = 0;
    private const int _tickRate = 10;
    private EnemyAI _enemyAI;
    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private async void OnEnable()
    {
        _enemyAI = transform.GetComponent<EnemyAI>();
        while(!_enemyAI.EnemyModelInitialized)
        {
            await Awaitable.EndOfFrameAsync();
        }
        _model = _enemyAI.enemyModel;
        AfflictionCooldowns[ElementEnum.Ice] = null;
        AfflictionCooldowns[ElementEnum.Fire] = null;
        AfflictionCooldowns[ElementEnum.Lightning] = null;
        AfflictionCooldowns[ElementEnum.Resin] = null;
    }
    public void TakeElementAccumulation(Dictionary<ElementEnum,int> elements)
    {
        foreach (var element in elements)
        {
            var cuurentAfflictionData = _model.EnemyAfflictionData[element.Key];
            cuurentAfflictionData.AfflictionAccumulation += element.Value;
            if (cuurentAfflictionData.AfflictionAccumulation> cuurentAfflictionData.AfflictionLimit)
            {
                cuurentAfflictionData.AfflictionAccumulation %= cuurentAfflictionData.AfflictionLimit;
                cuurentAfflictionData.Afflicted = true;
                InflictAffliction(element.Key);
            }
            if(_model.EnemyAfflictionData[cuurentAfflictionData.OpposingElement].Afflicted)
            {
                DisableAffliction(cuurentAfflictionData.OpposingElement);
            }
        }
    }

    private void InflictAffliction(ElementEnum element)
    {
        float duration = _model.EnemyAfflictionData[element].AfflictionDuration;
        if (AfflictionCooldowns[element] != null)
        {
            StopCoroutine(AfflictionCooldowns[element]);
            AfflictionCooldowns[element] = null;
        }
        switch (element)
        {
            case ElementEnum.Ice:
                AfflictionCooldowns[element] = StartCoroutine(FrostAffliction(duration));
                break;

            case ElementEnum.Fire:
                AfflictionCooldowns[element] = StartCoroutine(OverheatAffliction(duration));
                break;

            case ElementEnum.Lightning:
                AfflictionCooldowns[element] = StartCoroutine(LightningAffliction(duration));
                break;

            case ElementEnum.Resin:
                AfflictionCooldowns[element] = StartCoroutine(BrittleAffliction(duration));
                break;
        }
    }

    private IEnumerator OverheatAffliction(float duration)
    {
        yield return new WaitForSeconds(duration);
        DisableAffliction(ElementEnum.Fire);
    }

    private IEnumerator FrostAffliction(float duration)
    {
        _model.MoveSpeed = _model.SlowedSpeed;
        yield return new WaitForSeconds(duration);
        DisableAffliction(ElementEnum.Ice);
    }

    private IEnumerator LightningAffliction(float duration)
    {
        _model.Stunned = true;
        _animator.speed = 0;
        yield return new WaitForSeconds(duration);
        DisableAffliction(ElementEnum.Lightning);
    }

    private IEnumerator BrittleAffliction(float duration)
    {
        _model.DamageTakenMultiplier = _model.EnemyAfflictionData[ElementEnum.Lightning].EffectValue;
        yield return new WaitForSeconds(duration);
        DisableAffliction(ElementEnum.Resin);
    }

    private void DisableAffliction(ElementEnum element)
    {
        switch (element)
        {
            case ElementEnum.Ice:
                _model.EnemyAfflictionData[ElementEnum.Ice].Afflicted = false;
                _model.MoveSpeed = _model.NormalSpeed;
                break;

            case ElementEnum.Fire:
                _model.EnemyAfflictionData[ElementEnum.Fire].Afflicted = false;
                break;

            case ElementEnum.Lightning:
                _model.Stunned = false;
                _animator.speed = 1f;
                _model.EnemyAfflictionData[ElementEnum.Lightning].Afflicted = false;
                break;

            case ElementEnum.Resin:
                _model.DamageTakenMultiplier = 1f;
                _model.EnemyAfflictionData[ElementEnum.Resin].Afflicted = false;
                break;
        }
    }

    private void FixedUpdate()
    {
        if (_model != null)
        {
            if (_tick < _tickRate)
            {
                _tick += 1;
            }
            else
            {
                _tick = 0;
                if (_model.EnemyAfflictionData[ElementEnum.Fire].Afflicted)
                {
                    _enemyAI.TakeDamage((int)_model.EnemyAfflictionData[ElementEnum.Fire].EffectValue);
                }
            }
            foreach (var element in _model.EnemyAfflictionData)
            {
                if (element.Value.AfflictionAccumulation > 0)
                {
                    element.Value.AfflictionAccumulation = Mathf.Max(element.Value.AfflictionAccumulation - element.Value.AfflictionCDRate * Time.fixedDeltaTime, 0f);
                }
            }
        }
    }
}


public enum ElementEnum
{
    Ice,
    Fire,
    Lightning,
    Resin
}

