﻿using Fairwood.Math;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 覆盖屏幕右半边的攻击控制器
/// </summary>
public class AttackControlPad : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    public enum StateEnum
    {
        Idle,
        InvalidDragging,
        ValidDragging,
    }

    public StateEnum State = StateEnum.Idle;
    public MainUI MainUI;

    public Transform MainCameraTra;
    public Transform AssistPlane;
    public RectTransform TouchCircle;
    public RectTransform TouchSpot;
    public RectTransform DragDrop;

    public Vector2 PressPosition;//屏幕坐标
    public Vector2 CurrentPosition;//屏幕坐标
    public Vector3 GeodesicDisplacement;//战场坐标

    private Unit _assistAimingTarget;
    public float AssistAimingWidth = 1;

    public float WorldDisplacementThreshold = 5;


    //private const float UnitColliderDiameter = 1f;
    //private const int AssistAimingTestRayCount = 3;
    //private readonly float[] AssistAimingTestRayOffsetList = {-0.5f, 0, 0.5f};

    void Awake()
    {
        enabled = false;
    }

    public void Init(Unit playerUnit)
    {
        State = StateEnum.Idle;
        HideUI();
        enabled = true;
        //gameObject.SetActive(false);
    }

    public void SwitchOnOff()
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }

    void Update()
    {
        var focusedUnit = UnitController.Instance.FocusedUnit;
        if (!focusedUnit || !focusedUnit.Data.IsAlive) return;
        if (State == StateEnum.InvalidDragging || State == StateEnum.ValidDragging)
        {
            ResetAssistPlaneRotation();//坐标转换时使用的辅助平面，更新状态
            var dragDisplacement = CurrentPosition - PressPosition;//屏幕坐标
            var dragMagnitude = dragDisplacement.magnitude;


            GeodesicDisplacement = DragDisplacementToGeodesicDisplacement(dragDisplacement);//战场坐标
            //检测是否有效Drag
            var geodesicMagnitude = GeodesicDisplacement.magnitude;
            if (geodesicMagnitude > WorldDisplacementThreshold)//有效拖动
            {
                if (State == StateEnum.InvalidDragging)//状态切换
                {
                    State = StateEnum.ValidDragging;
                    HideUI();
                }

                TouchSpot.localPosition = dragDisplacement * (1800f / Screen.width);
                DragDrop.parent.right = dragDisplacement;
                DragDrop.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, dragDisplacement.magnitude + 71 + 20);
                
                #region 辅助瞄准
                _assistAimingTarget = null;
                //if (false)
                //{
                //    var mostMiddleDistance = float.MaxValue;
                //    var right = Vector3.Cross(geodesicDisplacement, Vector3.up).normalized;
                //    for (int i = 0; i < AssistAimingTestRayOffsetList.Length; i++)
                //    {
                //        var offset = AssistAimingTestRayOffsetList[i];
                //        var castedHits = Physics.RaycastAll(unit.Position.SetV3Y(1) + offset * right, geodesicDisplacement,
                //            geodesicDisplacement.magnitude,
                //            LayerManager.Mask.Unit);
                //        foreach (var hit in castedHits)
                //        {
                //            var castedUnit = hit.collider.GetComponent<Unit>();
                //            castedUnit.MustNotBeNull();
                //            castedUnit.MustNotBeEqual(unit);
                //            var me2targetVector = castedUnit.Position - unit.Position;
                //            var toMiddleLineDistance = Mathf.Abs(Vector3.Dot(me2targetVector, right));
                //            if (toMiddleLineDistance < mostMiddleDistance)
                //            {
                //                _assistAimingTarget = castedUnit;
                //                mostMiddleDistance = toMiddleLineDistance;
                //            }
                //        }
                //    }
                //}
                #endregion

                focusedUnit.transform.forward = GeodesicDisplacement;
            }
            else
            {
                if (State == StateEnum.ValidDragging)//状态切换
                {
                    State = StateEnum.InvalidDragging;
                    HideUI();
                }
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        var focusedUnit = UnitController.Instance.FocusedUnit;
        if (!focusedUnit || !focusedUnit.Data.IsAlive) return;
        State = StateEnum.InvalidDragging;
        PressPosition = eventData.pressPosition;
        CurrentPosition = eventData.position;

        var pos = PressPosition * 1800f/Screen.width;
        TouchCircle.localPosition = pos;
        TouchSpot.localPosition = Vector3.zero;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        var focusedUnit = UnitController.Instance.FocusedUnit;
        if (!focusedUnit || !focusedUnit.Data.IsAlive) return;
        CurrentPosition = eventData.position;
        ResetAssistPlaneRotation();
        if (State == StateEnum.ValidDragging)
        {
            var dragDisplacement = CurrentPosition - PressPosition;
            var dragMagnitude = dragDisplacement.magnitude;
            if (dragMagnitude > WorldDisplacementThreshold) //有效拖动
            {
                var geodesicDisplacement = DragDisplacementToGeodesicDisplacement(dragDisplacement);
                Vector3 castSkillDisplacement;
                if (false && _assistAimingTarget)
                {
                    //castSkillDisplacement = (_assistAimingTarget.Position - f.Position).normalized*
                    //                        geodesicDisplacement.magnitude;
                }
                else
                {
                    castSkillDisplacement = geodesicDisplacement;
                }
                focusedUnit.CastSkill(castSkillDisplacement);
            }
        }

        State = StateEnum.Idle;
        HideUI();
    }

    public void OnDrag(PointerEventData eventData)
    {
        var focusedUnit = UnitController.Instance.FocusedUnit;
        if (!focusedUnit || !focusedUnit.Data.IsAlive) return;
        CurrentPosition = eventData.position;
    }

    void ResetAssistPlaneRotation()
    {
        AssistPlane.right = MainCameraTra.right;
    }

    Vector3 DragDisplacementToGeodesicDisplacement(Vector2 dragDisplacement)
    {
        var focusedUnit = UnitController.Instance.FocusedUnit;
        //方案2
        var cam = MainCameraTra.GetComponent<Camera>();
        var screenStartPos = cam.WorldToScreenPoint(focusedUnit.Position.SetV3Y(0));

        var ray0 = cam.ScreenPointToRay(screenStartPos);
        var startPos = ray0.GetPoint(-ray0.origin.y/ray0.direction.y);
        var ray1 = cam.ScreenPointToRay(screenStartPos.ToVector2() + dragDisplacement);
        var endPos = ray1.GetPoint(-ray1.origin.y / ray1.direction.y);
        var geodesicDisplacement = (endPos - startPos).normalized*dragDisplacement.magnitude*0.03f;
                                   //focusedUnit.SkillDragToDisplacementRatioList[CurrentAttackSkillID];
        return geodesicDisplacement;
    }

    void ShowUI()
    {
        TouchCircle.gameObject.SetActive(true);
        TouchSpot.gameObject.SetActive(true);
        DragDrop.gameObject.SetActive(true);
    }
    void HideUI()
    {
        TouchCircle.gameObject.SetActive(false);
        TouchSpot.gameObject.SetActive(false);
        DragDrop.gameObject.SetActive(false);
    }
}