using Fairwood.Math;
using UnityEngine;

/// <summary>
/// 掩体上可躲人的地点，每个地点有若干席位
/// </summary>
public class BunkerSite : MonoBehaviour
{
    /// <summary>
    /// 席位数
    /// </summary>
    public int SeatCount = 2;

    public Bunker Bunker;
    public int ID;
    public Vector3 Position
    {
        get { return transform.position.SetV3Y(0); }
    }

    public void Init(Bunker bunker, int id)
    {
        Bunker = bunker;
        ID = id;
    }
    public void OnSiteClick()
    {
        Debug.LogFormat("OnSiteClick");
        UnitController.Instance.FocusedUnit.Walker.WalkTo(Position);
    }
    public void OnUnitEnter(Unit unit)
    {

    }

    public void OnUnitExit(Unit unit)
    {

    }
}