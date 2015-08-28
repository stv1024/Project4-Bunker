using UnityEngine;

/// <summary>
/// 走路模块
/// </summary>
public class UnitWalker : MonoBehaviour
{
    private Unit _unit;
    private NavMeshAgent _navMeshAgent;
    void Awake()
    {
        _unit = GetComponent<Unit>();
        _navMeshAgent = GetComponent<NavMeshAgent>();
    }

    public float k = 0.25f;
    void Update()
    {
        _unit.Animator.SetFloat("Speed", _navMeshAgent.velocity.magnitude*k+0.001f);
    }

    public void WalkTo(Vector3 position)
    {
        var nma = GetComponent<NavMeshAgent>();
        nma.SetDestination(position);
    }
}