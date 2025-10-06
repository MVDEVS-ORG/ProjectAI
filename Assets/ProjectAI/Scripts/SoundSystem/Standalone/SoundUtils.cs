using FMODUnity;
using UnityEngine;

public class SoundUtils
{
    public static void PlayStandaloneSFX(EventReference sound, Vector3? worldPos)
    {
        if(worldPos.HasValue)
        {
            RuntimeManager.PlayOneShot(sound, worldPos.Value);
        }
        else
        {
            RuntimeManager.PlayOneShot(sound);
        }
    }
}
