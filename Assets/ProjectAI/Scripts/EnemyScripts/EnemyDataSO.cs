using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyDataSO", menuName = "Scriptable Objects/EnemyDataSO")]
public class EnemyDataSO : ScriptableObject
{
    public float MoveSpeed;
    public float SlowedSpeed;
    public float AttackRange;
    public float DetectionRange;
    public float AttackOffset;
    public float AttackCooldown; 
    public int Xp;

    [Header("ElementSystem")]
    public List<ElementAfflictionData> EnemyAfflictionData;
}

[Serializable]
public class ElementAfflictionData
{
    public ElementAfflictionData(ElementAfflictionData data)
    {
        Element = data.Element;
        OpposingElement = data.OpposingElement;
        AfflictionLimit = data.AfflictionLimit;
        AfflictionCDRate = data.AfflictionCDRate;
        AfflictionDuration = data.AfflictionDuration;
        EffectValue = data.EffectValue;
        AfflictionAccumulation = data.AfflictionAccumulation;
        Afflicted = data.Afflicted;
    }

    public ElementEnum Element;
    [HideInInspector] public ElementEnum OpposingElement;
    public float AfflictionLimit;
    public float AfflictionCDRate;
    public float AfflictionDuration;
    public float EffectValue;
    [HideInInspector]public float AfflictionAccumulation;
    [HideInInspector]public bool Afflicted = false;
}

