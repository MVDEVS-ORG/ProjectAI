using UnityEngine;

public class FollowScript : MonoBehaviour
{
    private Transform _target;
    private bool _initialized;
    private Vector3 _velocity = Vector3.zero;

    public void Initialize(Transform target)
    {
        _target = target;
        _initialized = true;
    }

    void Update()
    {
        if (_initialized)
        {
            transform.position = Vector3.SmoothDamp(transform.position, _target.position, ref _velocity, 0.08f);
        }
    }
}
