using UnityEngine;
using DG.Tweening;
using DG.Tweening.Plugins.Core.PathCore;

public class TestCurve : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Vector3[] path = new Vector3[3];
        path[0] = new Vector2(1,0);
        path[1] = new Vector2(0, 1);
        path[2] = new Vector2(1,1);
        /*path[2] = new Vector2(-1, 0);
        path[3] = new Vector2(1, 0);
        path[4] = new Vector2(1, 0);
        path[5] = new Vector2(0, -1);*/
        transform.DOPath(path, 2, PathType.CubicBezier, PathMode.TopDown2D, 10);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
