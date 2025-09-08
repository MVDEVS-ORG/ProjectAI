using System.Collections;
using TMPro;
using UnityEngine;

public class PopUpUI : MonoBehaviour
{
    [SerializeField] private TMP_Text text;
    Coroutine DestroyTextCoroutine = null;

    public void DisplayText(string Text)
    {
        text.text = Text;
    }

    public void DisplayTextWithDuration(string Text, float duration)
    {
        text.text = Text;
        Destroy(gameObject, duration);
    }
}
