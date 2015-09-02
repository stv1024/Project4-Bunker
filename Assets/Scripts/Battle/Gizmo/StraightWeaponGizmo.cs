using Fairwood.Math;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 直线平飞武器Gizmo
/// </summary>
public class StraightWeaponGizmo : MonoBehaviour
{
    public Unit _focusUnit;
    public ScreenJoystick Joystick;
    public GameObject Arrow;

    private Color _shootArrowOriginalColor;
    void Awake()
    {
        _shootArrowOriginalColor = Arrow.GetComponent<Image>().color;
    }

    public void Init(ScreenJoystick joystick, Unit unit)
    {
        Joystick = joystick;
        _focusUnit = unit;
    }
    void Update()
    {
        if (!Joystick) return;
        var isDragging = Joystick.State == ScreenJoystick.StateEnum.ValidDragging ||
                         Joystick.State == ScreenJoystick.StateEnum.InvalidDragging;
        if (_focusUnit && isDragging && _focusUnit.CurrentSelectedWeapon.Info.ShowStraightGizmo && Joystick.WorldActualDisplacement != Vector3.zero)
        {
            Arrow.SetActive(true);
//            Arrow.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 0);
            var arrow = Arrow.GetComponent<RectTransform>();
//            var img = arrow.GetComponent<Image>();
            var eA = arrow.localEulerAngles;
            eA.z = Quaternion.FromToRotation(Vector3.right, Joystick.WorldActualDisplacement).eulerAngles.y;
            arrow.localEulerAngles = -eA;
            arrow.position = _focusUnit.transform.position.SetV3Y(0.01f);
            arrow.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, _focusUnit.CurrentSelectedWeapon.Info.StraightGizmoLength);
        }
        else
        {
            Arrow.SetActive(false);
        }
    }
}