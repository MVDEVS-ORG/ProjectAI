using System.Collections.Generic;
using UnityEngine;

public class FlagMono : MonoBehaviour
{
    [SerializeField] private List<Flag> flags;

    public void stuff()
    {
        UniversalFlagStorage storage = new();
        storage.SetUpFlags();
        storage.AddFlags(flags);
    }
}
