using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using Unity.Android.Gradle;
using UnityEngine;

public class EnemyElementAccumulation : MonoBehaviour
{
    private EnemyModel _model;
    private Dictionary<ElementEnum, Coroutine> AfflictionCooldowns = new();
    private int Tick = 0;
    private const int TickRate = 10;
    private EnemyAI _enemyAI;

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
            if (cuurentAfflictionData.AfflictionAccumulation>= cuurentAfflictionData.AfflictionLimit)
            {
                cuurentAfflictionData.AfflictionAccumulation %= cuurentAfflictionData.AfflictionLimit;
                cuurentAfflictionData.Afflicted = true;
                InflictAffliction(element.Key);
            }
            if(_model.EnemyAfflictionData[element.Key].Afflicted && _model.EnemyAfflictionData[cuurentAfflictionData.OpposingElement].Afflicted)
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
        Debug.LogError(_model.GetHashCode() + "duration" + duration);
        Debug.LogError($"speed is {_enemyAI.enemyModel.MoveSpeed}");
        yield return new WaitForSeconds(duration);
        Debug.LogError($"speed is {_enemyAI.enemyModel.MoveSpeed}");
        DisableAffliction(ElementEnum.Ice);
    }

    private IEnumerator LightningAffliction(float duration)
    {
        _model.Stunned = true;
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
        Debug.LogError($"{_model.GetHashCode()} is clearing {element}");
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
            if (Tick < TickRate)
            {
                Tick += 1;
            }
            else
            {
                Tick = 0;
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

