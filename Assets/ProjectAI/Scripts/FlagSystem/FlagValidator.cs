using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

[RequireComponent(typeof(ZenAutoInjecter))]
public abstract class FlagValidator : MonoBehaviour
{
    [Inject] public UniversalFlagStorage flagStorage;
    public UnityEvent OnInvoke;
    public Validation validation = new();

    protected virtual void Invoke()
    {
        OnInvoke?.Invoke();
    }

    protected virtual bool ValidateFlag(List<Flag> flags)
    {
        return validation.Validate(flags);
    }

    protected virtual void InvokeOnValidation(List<Flag> flags)
    {
        if(validation.Validate(flags))
        {
            Invoke();
        }
    }

    protected virtual bool ValidateFromUniversalFlags()
    {
        return validation.reverseValidate(validation.logic,flagStorage.GetFlags());
    }
}

public enum ValidationsBooleanLogic
{
    And,
    Or
}

[Serializable]
public class Validation
{
    public ValidationsBooleanLogic logic;
    public List<Flag> flag;
    public bool negate = false;

    public bool Validate(List<Flag> testFlag)
    {
        bool output = validateEach(logic, testFlag);
        return negate? !output : output;
    }

    public bool validateEach(ValidationsBooleanLogic logic, List<Flag> testflags)
    {
        bool test = false;
        switch (logic)
        {
            case ValidationsBooleanLogic.And:
                foreach (Flag flags in testflags)
                {
                    test = true;
                    test = test && flag.Contains(flags);
                }
                break;

            case ValidationsBooleanLogic.Or:
                foreach (Flag flags in testflags)
                {
                    test = false;
                    test = test || flag.Contains(flags);
                }
                break;
        }
        return test;
    }

    public bool reverseValidate(ValidationsBooleanLogic logic, List<Flag> testflags)
    {
        bool test = false;
        switch (logic)
        {
            case ValidationsBooleanLogic.And:
                foreach (Flag flags in flag)
                {
                    test = true;
                    test = test && testflags.Contains(flags);
                }
                break;

            case ValidationsBooleanLogic.Or:
                foreach (Flag flags in flag)
                {
                    test = false;
                    test = test || testflags.Contains(flags);
                }
                break;
        }
        return test;
    }

    
}



