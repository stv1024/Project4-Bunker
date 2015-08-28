using System;
using Fairwood.Math;
using UnityEngine;

/// <summary>
/// 摇杆反馈Gizmo
/// </summary>
public class JoystickFeedbackGizmo : MonoBehaviour
{
    public AttackControlPad Joystick;
    public Transform CenterObject;
    public GameObject MinCircle;
    public GameObject Spot;

    private AttackControlPad.StateEnum _lastState;

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
        if (Joystick.State != AttackControlPad.StateEnum.Idle)
        {
            Spot.transform.position = transform.position + Joystick.GeodesicDisplacement;
        }
    }

    void StateTransition(AttackControlPad.StateEnum lastState, AttackControlPad.StateEnum curState)
    {
        switch (curState)
        {
            case AttackControlPad.StateEnum.Idle:
                Hide();
                break;
            case AttackControlPad.StateEnum.InvalidDragging:
            case AttackControlPad.StateEnum.ValidDragging:
                MinCircle.SetActive(true);
                Spot.SetActive(true);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    void Hide()
    {
        MinCircle.SetActive(false);
        Spot.SetActive(false);
    }
}