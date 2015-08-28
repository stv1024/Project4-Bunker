using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// 加在玩家Unit Prefab上
/// </summary>
public class PlayerUnit : MonoBehaviour
{
    public Unit Owner;

    public UnitData Data
    {
        get { return Owner.Data; }
    }

    void Awake()
    {
        Debug.LogFormat("b:" + GetComponent<NetworkIdentity>().hasAuthority);
        if (!GetComponent<NetworkIdentity>().hasAuthority)
        {
            Destroy(this);
        }
    }
    void OnEnable()
    {
        Owner = GetComponent<Unit>();
    }

    void OnStartClient()
    {
        Debug.LogFormat("OnStartClient");
    }
}