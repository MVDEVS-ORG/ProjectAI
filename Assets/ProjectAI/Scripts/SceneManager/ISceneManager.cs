using System;
using System.Threading.Tasks;
using UnityEngine;

public interface ISceneManager
{
    Awaitable LoadSceneAsync(string sceneName);
    event Action BeforeChangeScene;
    event Action AfterChangeScene;
    Awaitable FadeToBlack();
    Awaitable FadeBack();
}
