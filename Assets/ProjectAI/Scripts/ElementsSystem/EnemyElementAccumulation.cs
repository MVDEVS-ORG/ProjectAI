using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemyElementAccumulation : MonoBehaviour
{
    private Dictionary<ElementsEnum, int> AccumulationValues = new();
    [SerializeField] private int FireAccumulationLimit;
    [SerializeField] private int IceAccumulationLimit;
    [SerializeField] private int ElectricityAccumulationLimit;

    private Dictionary<ElementsEnum, bool> CurrentActiveAfflictions = new();

    void Start()
    {
        
    }

    private void FixedUpdate()
    {
        
    }

    public void AddAccumulation(Dictionary<ElementsEnum,int> values)
    {
        foreach(var element in values)
        {
            AccumulationValues[element.Key] += element.Value;
            int AfflictionLimit = 0;
            switch (element.Key)
            {
                case ElementsEnum.Fire:
                    AfflictionLimit = FireAccumulationLimit;
                    break;

                case ElementsEnum.Ice:
                    AfflictionLimit = IceAccumulationLimit;
                    break;

                case ElementsEnum.Electricity:
                    AfflictionLimit = ElectricityAccumulationLimit;
                    break;
            }
            if(AccumulationValues[element.Key]>AfflictionLimit)
            {
                Debug.Log($"Trigger or refresh {element.Key} affliction");
            }
            AccumulationValues[element.Key] %= AfflictionLimit;
        }
    }

    private void TriggerAffliction(ElementsEnum element)
    {
        switch (element)
        {
            case ElementsEnum.Fire:
                break;

            case ElementsEnum.Ice:
                break;

            case ElementsEnum.Electricity:
                break;
        }
    }
}

public class Element
{
    public ElementsEnum AfflictionElement;
    public int AccumulationLimit;
    public int AccumulationCoolOffLimit;
    public int AccumulationAfflictionTimer;
}

