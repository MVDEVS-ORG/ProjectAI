using UnityEngine;

public class SimpleGun : GunsView
{
    public async override Awaitable Fire()
    {
        await Awaitable.WaitForSecondsAsync(2f);
        Debug.Log("SimpleGunFire");
        //Call signal here
    }
}
