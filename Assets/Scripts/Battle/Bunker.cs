using Fairwood.Math;
using UnityEngine;

/// <summary>
/// 掩体
/// </summary>
public class Bunker : MonoBehaviour
{
    public BunkerSite[] SiteList;
    public const int SeatsPerSite = 2;

    public Vector3 Position
    {
        get { return transform.position.SetV3Y(0); }
    }
    
    public BunkerGizmo Gizmo;

    void Awake()
    {
        UnitController.Instance.BunkerGizmoContainer.CreateBunkerGizmo(this);
        for (int i = 0; i < SiteList.Length; i++)
        {
            SiteList[i].Init(this, i);
        }
    }

    public void OnSiteClick(int siteID)
    {
        Debug.LogFormat("OnSiteClick");
        UnitController.Instance.FocusedUnit.Walker.WalkTo(SiteList[siteID].Position);
    }

}