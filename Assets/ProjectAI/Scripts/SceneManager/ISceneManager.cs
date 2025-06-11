using System;
using System.Threading.Tasks;
using UnityEngine;

public interface ISceneManager
{
    Task LoadSceneAsync(string sceneName);
    event Action BeforeChangeScene;
    event Action AfterChangeScene;
}
