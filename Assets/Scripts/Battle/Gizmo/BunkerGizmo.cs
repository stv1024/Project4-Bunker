using Fairwood.Math;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 掩体的Gizmo
/// </summary>
public class BunkerGizmo : MonoBehaviour
{
    public GameObject SiteButtonPrefab;
    public Bunker Bunker;
    public Button[] SiteButtonList;

    public void Init(Bunker bunker)
    {
        Bunker = bunker;
        var length = bunker.SiteList.Length;
        SiteButtonList = new Button[length];
        for (var i = 0; i < length; i++)
        {
            var go = PrefabHelper.InstantiateAndReset(SiteButtonPrefab, transform);
            go.name = SiteButtonPrefab.name + i;
            go.transform.position = bunker.SiteList[i].Position.SetV3Y(0.01f);
            var button = go.GetComponent<Button>();
            var siteID = i;
            button.onClick.AddListener(bunker.SiteList[i].OnSiteClick);
            SiteButtonList[i] = button;
        }
    }
}