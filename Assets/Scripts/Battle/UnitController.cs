using UnityEngine;

/// <summary>
/// 单位控制器
/// </summary>
public class UnitController : MonoBehaviour
{
    public static UnitController Instance;

    public Unit FocusedUnit;
    public Camera Camera;
    public CameraFollow CameraFollow;

    void Awake()
    {
        Instance = this;
    }

    public void Init(Unit unit)
    {
        FocusedUnit = unit;
        CameraFollow.Target = unit.transform;
    }

    public BunkerGizmoContainer BunkerGizmoContainer;

}