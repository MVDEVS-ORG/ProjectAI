using UnityEngine;

public class OnFunctionCallValidate : FlagValidator
{
    public void Validate()
    {
        InvokeOnValidation(flagStorage.GetFlags());
    }
}
