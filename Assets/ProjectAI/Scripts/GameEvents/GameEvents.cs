using System;
using System.Collections;
using UnityEngine;

namespace Assets.ProjectAI.Scripts.GameEvent
{
    public static class GameEvents
    {
        public static event Action OnPortalKeyAcquired;

        public static void PortalkeyAcquired()=> OnPortalKeyAcquired?.Invoke();
    }
}