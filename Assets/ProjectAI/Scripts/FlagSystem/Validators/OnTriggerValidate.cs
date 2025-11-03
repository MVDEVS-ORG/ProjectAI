using UnityEngine;

public class OnTriggerValidate : FlagValidator
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        InvokeOnValidation(flagStorage.GetFlags());
    }
}
