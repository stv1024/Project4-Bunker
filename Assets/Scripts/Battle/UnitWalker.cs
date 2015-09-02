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
        _unit.State = _navMeshAgent.velocity.magnitude > 0 ? Unit.StateEnum.Running : Unit.StateEnum.Idle;
    }

    public void WalkTo(Vector3 position)
    {
        _navMeshAgent.Resume();
        _navMeshAgent.SetDestination(position);
    }

    public void Stop()
    {
        _navMeshAgent.Stop();
    }
}