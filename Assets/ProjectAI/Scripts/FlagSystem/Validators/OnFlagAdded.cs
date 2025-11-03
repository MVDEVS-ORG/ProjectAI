using System.Collections.Generic;
using UnityEngine;

public class OnFlagAdded : FlagValidator
{
    private void Start()
    {
        //flagStorage.
    }

    private void OnEnable()
    {
        if(flagStorage!=null)
        {
            flagStorage.OnFlagsAdded += InvokeOnFlagSet;
            flagStorage.OnSingleFlagAdded += InvokeOnFlagSet;
        }
    }

    private void InvokeOnFlagSet(List<Flag> flags)
    {
        InvokeOnValidation(flags);
    }

    private void InvokeOnFlagSet(Flag flag)
    {
        var flags = new List<Flag>() { flag };
        InvokeOnValidation(flags);
    }

    private void OnDisable()
    {
        if (flagStorage != null)
        {
            flagStorage.OnFlagsAdded += InvokeOnFlagSet;
            flagStorage.OnSingleFlagAdded += InvokeOnFlagSet;
        }
    }
}
