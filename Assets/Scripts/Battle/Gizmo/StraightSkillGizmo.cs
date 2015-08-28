using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 直线平飞技能Gizmo
/// </summary>
public class StraightSkillGizmo : MonoBehaviour
{
    public GameObject Arrow;

    private Color _shootArrowOriginalColor;
    void Awake()
    {
        _shootArrowOriginalColor = Arrow.GetComponent<Image>().color;
    }
    void Update()
    {
        Arrow.SetActive(true);
        Arrow.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 0);
        //var img = arrow.GetComponent<Image>();
        //img.color = img.color.SetAlpha(_assistAimingTarget ? 1f : 1);
        //var eA = arrow.localEulerAngles;
        //eA.z = Quaternion.FromToRotation(Vector3.right, geodesicDisplacement).eulerAngles.y;
        //arrow.localEulerAngles = -eA;
        //arrow.position = unit.transform.position.SetV3Y(0.01f);
        //arrow.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,
        //    CurrentAttackSkillID != 1
        //        ? geodesicDisplacement.magnitude
        //        : Mathf.Min(arrow.rect.width + 60 * Time.deltaTime, geodesicDisplacement.magnitude));
        //arrow.GetComponent<Image>().color = unit.GetSkillTotalCDRemaining(CurrentAttackSkillID) <= 0 && unit.GetSkillCountEnough(CurrentAttackSkillID) ? _shootArrowOriginalColor : Color.grey;


        //var thinArrow = BattleEngine.Instance.ThinArrow;
        ////thinArrow.gameObject.SetActive(_assistAimingTarget);
        //if (_assistAimingTarget) //找到辅助瞄准的目标了
        //{
        //    var me2mostMiddleUnitVector = _assistAimingTarget.Position - unit.Position;
        //    eA = thinArrow.localEulerAngles;
        //    eA.z = Quaternion.FromToRotation(Vector3.right, me2mostMiddleUnitVector).eulerAngles.y;
        //    thinArrow.localEulerAngles = -eA;
        //    thinArrow.position =
        //        unit.transform.position.SetV3Y(0.01f);
        //    thinArrow.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, geodesicDisplacement.magnitude);
        //}
    }
}