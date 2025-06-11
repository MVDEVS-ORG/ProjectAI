using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManager : ISceneManager
{
    public event Action BeforeChangeScene;
    public event Action AfterChangeScene;

    async Task ISceneManager.LoadSceneAsync(string sceneName)
    {
        if (BeforeChangeScene != null)
        {
            BeforeChangeScene.Invoke();
        }
        await UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName);
        if (AfterChangeScene != null)
        {
            AfterChangeScene.Invoke();
        }
    }
}
