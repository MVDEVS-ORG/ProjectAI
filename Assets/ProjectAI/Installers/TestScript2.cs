using UnityEngine;
using Zenject;

public class TestScript2
{
    [Inject]
    private TestScriptToBeDeleted t;
    [Inject]
    public async void Initialize()
    {
        t.test();
    }
}
