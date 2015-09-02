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

    public LeftControlPad LeftControlPad;
    public JoystickFeedbackGizmo LeftFeedbackGizmo;
    public RightControlPad RightControlPad;
    public JoystickFeedbackGizmo RightFeedbackGizmo;
    public StraightWeaponGizmo StraightWeaponGizmo;

    void Awake()
    {
        Instance = this;
    }

    public void Init(Unit unit, int camp)
    {
        FocusedUnit = unit;
        CameraFollow.SetTarget(unit.transform, camp == 2);
        LeftControlPad.Init(unit, Camera, LeftFeedbackGizmo);
        RightControlPad.Init(unit, Camera, RightFeedbackGizmo, StraightWeaponGizmo);
    }


    public BunkerGizmoContainer BunkerGizmoContainer;

    public void SwitchSkill(int slotID)
    {
        Debug.LogFormat("SwitchSkill({0})", slotID);
        if (FocusedUnit)
        {
            FocusedUnit.SwitchWeapon(slotID);
            RightControlPad.DidSwitchWeapon(slotID);
        }
    }

    public void OnFocusedUnitEnterBunker()
    {
        LeftControlPad.SortNearbySites();
        var allBunkerList = BattleEngine.Instance.BunkerList;
        foreach (var bunker in allBunkerList)
        {
            bunker.Gizmo.HideAndDehighlightAllSiteButtons();
        }
        for (int i = 0; i < LeftControlPad.SortedCount; i++)
        {
            var e = LeftControlPad.SortArray[i];
            allBunkerList[e.BunkerIndex].Gizmo.ShowHideSiteButton(e.SiteIndex, true);
        }
    }
}