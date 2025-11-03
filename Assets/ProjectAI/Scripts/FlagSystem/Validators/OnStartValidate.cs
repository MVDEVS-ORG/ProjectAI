using UnityEngine;

public class OnStartValidate :FlagValidator
{
    private void Start()
    {
        InvokeOnValidation(flagStorage.GetFlags());
    }
}
