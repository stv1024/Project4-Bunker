using System;
using Fairwood.Math;
using UnityEngine;

/// <summary>
/// 摇杆反馈Gizmo
/// </summary>
public class JoystickFeedbackGizmo : MonoBehaviour
{
    public ScreenJoystick Joystick;
    public Transform CenterObject;
    public GameObject MinCircle;
    public GameObject FallSpot;
    public GameObject DragSpot;

    private ScreenJoystick.StateEnum _lastState;

    void Awake()
    {
        Hide();
    }

    public void Init(ScreenJoystick joystick, Transform centerObject)
    {
        Joystick = joystick;
        CenterObject = centerObject;
    }

    void Update()
    {
        if (!CenterObject || !Joystick) return;
        transform.position = CenterObject.position.SetV3Y(0.01f);
        if (Joystick.State != _lastState)
        {
            StateTransition(_lastState, Joystick.State);
            _lastState = Joystick.State;
        }
        if (Joystick.State != ScreenJoystick.StateEnum.Idle)
        {
            FallSpot.SetActive(Joystick.IsValidDrag);
            FallSpot.transform.position = transform.position + Joystick.WorldActualDisplacement;
            DragSpot.transform.position = transform.position + Joystick.WorldAimingDisplacement;
        }
    }

    void StateTransition(ScreenJoystick.StateEnum lastState, ScreenJoystick.StateEnum curState)
    {
        switch (curState)
        {
            case ScreenJoystick.StateEnum.Idle:
                Hide();
                break;
            case ScreenJoystick.StateEnum.InvalidDragging:
            case ScreenJoystick.StateEnum.ValidDragging:
                MinCircle.SetActive(true);
                FallSpot.SetActive(true);
                DragSpot.SetActive(true);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    void Hide()
    {
        MinCircle.SetActive(false);
        FallSpot.SetActive(false);
        DragSpot.SetActive(false);
    }
}