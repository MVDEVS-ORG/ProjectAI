using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemyModel
{
    public float MoveSpeed;
    public float SlowedSpeed;
    public float NormalSpeed;
    public float AttackRange;
    public float DetectionRange;
    public float AttackOffset;
    public float AttackCooldown;
    public int Xp;
    public bool Stunned;
    public float DamageTakenMultiplier;

    public Dictionary<ElementEnum,ElementAfflictionData> EnemyAfflictionData =  new();

    public EnemyModel(EnemyDataSO enemyData)
    {
        MoveSpeed = enemyData.MoveSpeed;
        NormalSpeed = MoveSpeed;
        SlowedSpeed = enemyData.SlowedSpeed;
        AttackRange = enemyData.AttackRange;
        DetectionRange = enemyData.DetectionRange;
        AttackOffset = enemyData.AttackOffset;
        AttackCooldown = enemyData.AttackCooldown;
        Xp = enemyData.Xp;
        Stunned = false;
        DamageTakenMultiplier = 1f;
        foreach (var data in enemyData.EnemyAfflictionData)
        {
            switch (data.Element)
            {
                case ElementEnum.Ice:
                    data.OpposingElement = ElementEnum.Fire;
                    break;

                case ElementEnum.Fire:
                    data.OpposingElement = ElementEnum.Ice;
                    break;

                case ElementEnum.Lightning:
                    data.OpposingElement = ElementEnum.Resin;
                    break;

                case ElementEnum.Resin:
                    data.OpposingElement = ElementEnum.Lightning;
                    break;
            }
            EnemyAfflictionData[data.Element] = data;
            break;
        }
    }
        
}
