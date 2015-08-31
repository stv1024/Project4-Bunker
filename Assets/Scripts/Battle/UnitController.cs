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

    public void Init(Unit unit, int camp)
    {
        FocusedUnit = unit;
        CameraFollow.SetTarget(unit.transform, camp == 2);
    }

    public BunkerGizmoContainer BunkerGizmoContainer;

    public void SwitchSkill(int slotID)
    {
        Debug.LogFormat("SwitchSkill({0})", slotID);
        if (FocusedUnit) FocusedUnit.CmdSwitchWeapon(slotID);
    }
}