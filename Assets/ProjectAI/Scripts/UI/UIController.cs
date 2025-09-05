using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public static class UIController
{
    public static bool LookingAtUI { get { return _lookingAtUI; } }
    private static bool _lookingAtUI = false;

    private static List<GameObject> openUIs = new();

    public static void LookAtUI(bool lookingAtUI, GameObject obj)
    {
        if (!openUIs.Contains(obj))
        {
            if (lookingAtUI)
            {
                openUIs.Add(obj);
            }
        }
        else
        {
            openUIs.Remove(obj);
        }

        if (openUIs.Count > 0)
        {
            _lookingAtUI = true;
        }
        else
        {
            _lookingAtUI = false;
        }
        if (Gamepad.all.Count == 0)
        {
            Cursor.visible = lookingAtUI;
        }
    }
}
