using System;
using Fairwood.Math;
using UnityEngine;

/// <summary>
/// 摇杆反馈Gizmo
/// </summary>
public class JoystickFeedbackGizmo : MonoBehaviour
{
    public WeaponControlJoystick Joystick;
    public Transform CenterObject;
    public GameObject MinCircle;
    public GameObject FallSpot;
    public GameObject DragSpot;

    private WeaponControlJoystick.StateEnum _lastState;

    void Awake()
    {
        Hide();
    }

    void Update()
    {
        if (!CenterObject)
        {
            var unit = UnitController.Instance.FocusedUnit;
            if (unit) CenterObject = unit.transform;
            if (!CenterObject) return;
        }
        transform.position = CenterObject.position.SetV3Y(0.01f);
        if (Joystick.State != _lastState)
        {
            StateTransition(_lastState, Joystick.State);
            _lastState = Joystick.State;
        }
        if (Joystick.State != WeaponControlJoystick.StateEnum.Idle)
        {
            FallSpot.SetActive(Joystick.IsValidDrag);
            FallSpot.transform.position = transform.position + Joystick.WorldActualDisplacement;
            DragSpot.transform.position = transform.position + Joystick.WorldAimingDisplacement;
        }
    }

    void StateTransition(WeaponControlJoystick.StateEnum lastState, WeaponControlJoystick.StateEnum curState)
    {
        switch (curState)
        {
            case WeaponControlJoystick.StateEnum.Idle:
                Hide();
                break;
            case WeaponControlJoystick.StateEnum.InvalidDragging:
            case WeaponControlJoystick.StateEnum.ValidDragging:
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