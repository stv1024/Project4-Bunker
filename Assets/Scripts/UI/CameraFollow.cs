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
    public bool Reverse;
    void Awake()
    {
        Offset = transform.position;
    }

    public void SetTarget(Transform target, bool reverse)
    {
        Target = target;
        Reverse = reverse;
        if (reverse)
        {
            transform.Rotate(Vector3.up, 180, Space.World);
        }
    }

    void Update()
    {
        if (Target)
        {
            var offset = Offset;
            if (Reverse)
            {
                offset.x = -offset.x;
                offset.z = -offset.z;
            }
            transform.position = Vector3.Lerp(transform.position, Target.position.SetV3Y(0) + offset, FollowSpeed);
        }
    }
}