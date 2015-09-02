using UnityEngine;

/// <summary>
/// Summary
/// </summary>
public class RightControlPad : MonoBehaviour
{
    private ScreenJoystick _joystick;
    private JoystickFeedbackGizmo _feedbackGizmo;

    private Unit _focusedUnit;

    public void Init(Unit unit, Camera controlCamera, JoystickFeedbackGizmo feedbackGizmo, StraightWeaponGizmo straightWeaponGizmo)
    {
        _focusedUnit = unit;
        _feedbackGizmo = feedbackGizmo;
        _joystick = GetComponent<ScreenJoystick>();
        _joystick.Init(controlCamera);
        _joystick.GetWorldDragStartPointMethod = () => unit.Position;
        _joystick.OnPointerStayEvent = OnPointerStay;
        _joystick.OnPointerUpEvent = OnPointerUp;
        _feedbackGizmo.Init(_joystick, unit.transform);
        straightWeaponGizmo.Init(_joystick, _focusedUnit);
    }

    void OnPointerStay(ScreenJoystick joystick)
    {
        var worldAimingDisplacement = joystick.WorldAimingDisplacement;
        if (worldAimingDisplacement != Vector3.zero) _focusedUnit.transform.forward = worldAimingDisplacement;
        if (joystick.State == ScreenJoystick.StateEnum.ValidDragging)
        {
            if (_focusedUnit.CurrentSelectedWeapon && _focusedUnit.CurrentSelectedWeapon.Info.Automatic)
            {
                _focusedUnit.Fire(joystick.WorldActualDisplacement);
            }
        }
    }
    void OnPointerUp(ScreenJoystick joystick)
    {
        Debug.Log("OnPointerUp");
        if (joystick.IsValidDrag)
        {
            if (_focusedUnit.CurrentSelectedWeapon && !_focusedUnit.CurrentSelectedWeapon.Info.Automatic)
            {
                _focusedUnit.Fire(joystick.WorldActualDisplacement);
            }
        }
    }

    public void DidSwitchWeapon(int slotID)
    {
        _joystick.ViewportDragToWorldAimingRatio =
            _focusedUnit.CurrentSelectedWeapon.Info.ViewportDragToWorldRatio;
        _joystick.WorldActualDisplacementMaxLimit =
            _focusedUnit.CurrentSelectedWeapon.Info.WorldActualDisplacementMaxLimit;
        _joystick.WorldAimingDisplacementThreshold =
            _focusedUnit.CurrentSelectedWeapon.Info.WorldAimingDisplacementThreshold;
    }
}