using System.Collections.Generic;
using UnityEngine;

public class OnAwakeUnlock : FlagValidator
{
    [SerializeField] bool grantFlagsAfter = false;
    [SerializeField] private List<Flag> flags;

    private void Awake()
    {
        if(ValidateFromUniversalFlags())
        {
            if (grantFlagsAfter)
            {
                flagStorage.AddFlags(flags);
            }
            Invoke();
        }
    }
}
