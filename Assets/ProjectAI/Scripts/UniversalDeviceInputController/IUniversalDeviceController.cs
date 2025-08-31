using System;
using UnityEngine;

public interface IUniversalDeviceController
{
    event Action<ControllerType> OnDeviceChanged;
    ControllerType GetCurrentActiveDevice();
    Awaitable SetGameObjectUI(GameObject obj);
    Awaitable OnGamePadSetUI(GameObject obj);
}

public enum ControllerType
{
    KBM,
    GamePad
}
