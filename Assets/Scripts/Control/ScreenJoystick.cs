using Fairwood.Math;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 屏幕摇杆，基于UI
/// </summary>
public class ScreenJoystick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    public enum StateEnum
    {
        Idle,
        InvalidDragging,
        ValidDragging,
    }

    public StateEnum State = StateEnum.Idle;

    Camera _controlCamera;
    public RectTransform TouchCircle;
    public RectTransform TouchSpot;
    public RectTransform DragDrop;

    public Vector2 PressPosition;//屏幕坐标
    public Vector2 CurrentPosition;//屏幕坐标
    public Vector2 ScreenDragDisplacement;//屏幕拖拽位移
    public Vector3 WorldDragDisplacement;//世界拖拽位移（与屏幕拖拽距离1:1）
    public Vector3 WorldAimingDisplacement;//世界瞄准位移，白点位置（拖拽位移*ratio，不考虑上下限）
    public Vector3 WorldActualDisplacement;//世界实际位移（瞄准位移的基础上，考虑上限，无下限）
    /// <summary>
    /// 是否超过下限
    /// </summary>
    public bool IsValidDrag;

    public delegate void JoystickEventHandler(ScreenJoystick joystick);
    public JoystickEventHandler OnPointerDownEvent;
    public JoystickEventHandler OnPointerUpEvent;
    public JoystickEventHandler OnPointerStayEvent;
    public float ViewportDragToWorldAimingRatio = 10f;
    public float WorldActualDisplacementMaxLimit = 20f;
    public float WorldAimingDisplacementThreshold = 1f;

    public delegate Vector3 GetWorldDragStartPointHandler();
    /// <summary>
    /// 告诉我世界位移的起点在哪
    /// </summary>
    public GetWorldDragStartPointHandler GetWorldDragStartPointMethod;

    void Awake()
    {
        enabled = false;
    }

    public void Init(Camera controlCamera)
    {
        State = StateEnum.Idle;
        HideUI();
        _controlCamera = controlCamera;
        enabled = true;
    }

    public void SwitchOnOff()
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        State = StateEnum.InvalidDragging;
        PressPosition = eventData.pressPosition;
        CurrentPosition = eventData.position;
        Debug.LogFormat("OnPointerDown(CurrentPosition={0})@{1}", CurrentPosition, Time.frameCount);
        var pos = PressPosition * 1800f / Screen.width;
        if (TouchCircle) TouchCircle.localPosition = pos;
        if (TouchSpot) TouchSpot.localPosition = Vector3.zero;


        if (OnPointerDownEvent != null) OnPointerDownEvent(this);
    }

    void Update()
    {
        //Debug.LogFormat("Update(CurrentPosition={0})@{1}", CurrentPosition, Time.frameCount);
        if (State == StateEnum.InvalidDragging || State == StateEnum.ValidDragging)
        {
            CalcDisplacements(WorldAimingDisplacementThreshold);
            if (IsValidDrag)//有效拖动
            {
                if (State == StateEnum.InvalidDragging)//状态切换
                {
                    State = StateEnum.ValidDragging;
                    HideUI();
                }

                if (TouchSpot) TouchSpot.localPosition = ScreenDragDisplacement * (1800f / Screen.width);
                if (DragDrop)
                {
                    DragDrop.parent.right = ScreenDragDisplacement;
                    DragDrop.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,
                        ScreenDragDisplacement.magnitude + 71 + 20);
                }
            }
            else
            {
                if (State == StateEnum.ValidDragging)//状态切换
                {
                    State = StateEnum.InvalidDragging;
                    HideUI();
                }
            }

            if (OnPointerStayEvent != null) OnPointerStayEvent(this);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        CurrentPosition = eventData.position;
        if (State == StateEnum.ValidDragging)
        {
            CalcDisplacements(WorldAimingDisplacementThreshold);
        }

        State = StateEnum.Idle;
        HideUI();

        if (OnPointerUpEvent != null) OnPointerUpEvent(this);
    }

    void CalcDisplacements(float worldAimingDisplacementThreshold)
    {
        //ScreenDragDisplacement
        ScreenDragDisplacement = CurrentPosition - PressPosition;//屏幕坐标

        //WorldDragDisplacement。方案2——ScreenDrag和WorldDrag在玩家看来是平行的，magnitude另算
        var screenStartPos = _controlCamera.WorldToScreenPoint(GetWorldDragStartPointMethod().SetV3Y(0));

        var ray0 = _controlCamera.ScreenPointToRay(screenStartPos);
        var startPos = ray0.GetPoint(-ray0.origin.y / ray0.direction.y);
        var ray1 = _controlCamera.ScreenPointToRay(screenStartPos.ToVector2() + ScreenDragDisplacement);
        var endPos = ray1.GetPoint(-ray1.origin.y / ray1.direction.y);
        var worldDragDirection = (endPos - startPos).normalized;
        WorldDragDisplacement = worldDragDirection * ScreenDragDisplacement.magnitude;
        //WorldAimingDisplacement
        WorldAimingDisplacement = worldDragDirection * (ScreenDragDisplacement.magnitude / Screen.height * ViewportDragToWorldAimingRatio);

        //WorldActualDisplacement
        WorldActualDisplacement = Vector3.ClampMagnitude(WorldAimingDisplacement, WorldActualDisplacementMaxLimit);
        var worldAimingMagnitude = WorldAimingDisplacement.magnitude;
        IsValidDrag = worldAimingMagnitude > worldAimingDisplacementThreshold;
    }

    public void OnDrag(PointerEventData eventData)
    {
        CurrentPosition = eventData.position;
    }

    void ShowUI()
    {
        if (TouchCircle) TouchCircle.gameObject.SetActive(true);
        if (TouchSpot) TouchSpot.gameObject.SetActive(true);
        if (DragDrop) DragDrop.gameObject.SetActive(true);
    }
    void HideUI()
    {
        if (TouchCircle) TouchCircle.gameObject.SetActive(false);
        if (TouchSpot) TouchSpot.gameObject.SetActive(false);
        if (DragDrop) DragDrop.gameObject.SetActive(false);
    }
}