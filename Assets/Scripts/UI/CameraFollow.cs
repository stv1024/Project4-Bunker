using Fairwood.Math;
using UnityEngine;

/// <summary>
/// Summary
/// </summary>
public class CameraFollow : MonoBehaviour
{
    public Vector3 Offset;
    public Transform Target;
    public float FollowSpeed = 0.1f;
    void Awake()
    {
        Offset = transform.position;
    }

    public void SetTarget(Transform target)
    {
        Target = target;
    }

    void Update()
    {
        if (Target) transform.position = Vector3.Lerp(transform.position, Target.position.SetV3Y(0) + Offset, FollowSpeed);
    }
}