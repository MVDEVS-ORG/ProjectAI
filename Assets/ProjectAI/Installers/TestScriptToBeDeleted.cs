using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

public class TestScriptToBeDeleted 
{
    [Inject]
    public void Initialize()
    {
        Debug.LogError("Working");

    }

    public void test()
    {
        Debug.LogError("Testing");
    }
}
