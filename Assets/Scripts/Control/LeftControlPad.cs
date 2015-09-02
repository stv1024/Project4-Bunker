using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 左手控制区
/// </summary>
[RequireComponent(typeof(ScreenJoystick))]
public class LeftControlPad : MonoBehaviour
{
    private ScreenJoystick _joystick;
    private JoystickFeedbackGizmo _feedbackGizmo;

    private Unit _focusedUnit;

    public void Init(Unit unit, Camera controlCamera, JoystickFeedbackGizmo feedbackGizmo)
    {
        _focusedUnit = unit;
        _feedbackGizmo = feedbackGizmo;
        _joystick = GetComponent<ScreenJoystick>();
        _joystick.Init(controlCamera);
        _joystick.GetWorldDragStartPointMethod = () => unit.Position;
        _joystick.OnPointerDownEvent = OnPointerDown;
        _joystick.OnPointerStayEvent = OnPointerStay;
        _joystick.OnPointerUpEvent = OnPointerUp;
        _feedbackGizmo.Init(_joystick, unit.transform);
    }

    private BunkerSite _lastHighlightSite;
    void OnPointerDown(ScreenJoystick joystick)
    {
        Debug.Log("OnPointerDown");
        _lastHighlightSite = null;
        SortNearbySites();
        var allBunkerList = BattleEngine.Instance.BunkerList;
        foreach (var bunker in allBunkerList)
        {
            bunker.Gizmo.HideAndDehighlightAllSiteButtons();
        }
        for (int i = 0; i < SortedCount; i++)
        {
            var e = SortArray[i];
            allBunkerList[e.BunkerIndex].Gizmo.ShowHideSiteButton(e.SiteIndex, true);
        }
    }
    void OnPointerStay(ScreenJoystick joystick)
    {
        if (joystick.IsValidDrag)
        {
            //肯定已经SortNearbySites()过了。当然如果在移动中，可能需要重新计算
            var index = PickupSelectedSite(joystick.WorldActualDisplacement);
            if (index >= 0)
            {
                var site = SortArray[index].GetBunkerSite(BattleEngine.Instance.BunkerList);
                if (site != _lastHighlightSite)
                {
                    var allBunkerList = BattleEngine.Instance.BunkerList;
                    for (int i = 0; i < SortedCount; i++)
                    {
                        var e = SortArray[i];
                        allBunkerList[e.BunkerIndex].Gizmo.HighlightAllSiteButtons(false);
                    }

                    _lastHighlightSite = site;
                    var gizmo = site.Bunker.Gizmo;
                    gizmo.HighlightSiteButton(site.ID, true);
                }
            }
        }
        else
        {
            if (_lastHighlightSite)
            {
                var allBunkerList = BattleEngine.Instance.BunkerList;
                for (int i = 0; i < SortedCount; i++)
                {
                    var e = SortArray[i];
                    allBunkerList[e.BunkerIndex].Gizmo.HighlightAllSiteButtons(false);
                }
                _lastHighlightSite = null;
            }
        }
    }

    
    void OnPointerUp(ScreenJoystick joystick)
    {
        Debug.Log("OnPointerUp");
        if (joystick.IsValidDrag)
        {
            var index = PickupSelectedSite(joystick.WorldActualDisplacement);
            if (index >= 0)
            {
                var site = SortArray[index].GetBunkerSite(BattleEngine.Instance.BunkerList);
                _focusedUnit.SetDestination(site);
            }
        }
    }

    /// <summary>
    /// 需要排序多少个
    /// </summary>
    public int SortCount = 6;
    public float MaxRunDistance = 15;
    public float MaxTurnAngle = 120;//一定≥90°
    public float AngleFactor = 0.1f;

    public struct SortElement
    {
        public int BunkerIndex;
        public int SiteIndex;
        public float Value;

        public BunkerSite GetBunkerSite(List<Bunker> bunkerList)
        {
            return bunkerList[BunkerIndex].SiteList[SiteIndex];
        }
    }
    public readonly SortElement[] SortArray = new SortElement[256];
    /// <summary>
    /// 最终排出来多少个，≤SortCount
    /// </summary>
    public int SortedCount;
    private SortElement _tempElement;
    public void SortNearbySites()
    {
        var count = 0;
        var unitPosition = _focusedUnit.Position;
        var allBunkerList = BattleEngine.Instance.BunkerList;
        for (int i = 0; i < allBunkerList.Count; i++)
        {
            var bunker = allBunkerList[i];
            var displacement = bunker.Position - unitPosition;
            var distance = displacement.magnitude;
            if (distance > MaxRunDistance) continue;
            for (int j = 0; j < bunker.SiteList.Length; j++)
            {
                var site = bunker.SiteList[j];
                if (_focusedUnit.CurrentBunkerSite == site) continue;
                var theta = Vector3.Angle(-displacement, site.FaceDirection);
                var isTheSameBunker = _focusedUnit.CurrentBunkerSite && site.Bunker == _focusedUnit.CurrentBunkerSite.Bunker;
                if (!isTheSameBunker && theta > MaxTurnAngle) continue;
                SortArray[count].BunkerIndex = i;
                SortArray[count].SiteIndex = j;
                float value;
                if (isTheSameBunker) value = 999;
                else value = 30 - distance - (theta - 90)*AngleFactor;
                SortArray[count].Value = value;
                count++;
            }
        }
        //从大到小排序，只需要前6个
        SortedCount = 0;
        for (int i = 0; i < SortCount && i < count; i++)
        {
            for (int j = i + 1; j < count; j++)
            {
                if (SortArray[j].Value > SortArray[i].Value)
                {
                    _tempElement = SortArray[i];
                    SortArray[i] = SortArray[j];
                    SortArray[j] = _tempElement;
                }
            }
            SortedCount ++;
            Debug.DrawLine(allBunkerList[SortArray[i].BunkerIndex].SiteList[SortArray[i].SiteIndex].Position,
                unitPosition, Color.blue, 10);
        }
        if (SortedCount <= 0)
        {
            Debug.LogError("近距离内竟然没有掩体，这还怎么玩儿");
        }
    }

    struct SortAngleElement
    {
        public int AfterSortIndex;
        /// <summary>
        /// 摇杆操作方向和Unit到Site方向的角度差，越小越好
        /// </summary>
        public float AngleOffset;
    }
    readonly SortAngleElement[] _sortAngleArray = new SortAngleElement[32];
    private SortAngleElement _tempAngleElement;
    int PickupSelectedSite(Vector3 actualDisplacement)
    {
        var allBunkerList = BattleEngine.Instance.BunkerList;
        var bestIndex = -1;
        var bestAngleOffset = float.PositiveInfinity;
        for (int i = 0; i < SortedCount; i++)
        {
            var site = SortArray[i].GetBunkerSite(allBunkerList);
            var unitToSite = site.Position - _focusedUnit.Position;
            var angleOffset = Vector3.Angle(actualDisplacement, unitToSite);
            if (angleOffset < bestAngleOffset)
            {
                bestIndex = i;
                bestAngleOffset = angleOffset;
            }
        }
        return bestIndex;
    }
}