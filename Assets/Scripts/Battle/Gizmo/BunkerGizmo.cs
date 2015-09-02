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
            go.transform.position = bunker.SiteList[i].Position.SetV3Y(0.1f);
            var button = go.GetComponent<Button>();
            var siteID = i;
            button.onClick.AddListener(bunker.SiteList[i].OnSiteClick);
            SiteButtonList[i] = button;
        }
    }

    void Update()
    {
//        var unit = UnitController.Instance.FocusedUnit;
//        foreach (var button in SiteButtonList)
//        {
//            if (unit && unit.State != Unit.StateEnum.Running && Vector3.Distance(unit.Position, button.transform.position.SetV3Y(0)) < 15)
//            {
//                button.gameObject.SetActive(true);
//            }
//            else
//            {
//                button.gameObject.SetActive(false);
//            }
//        }
    }

    private readonly Color DehighlightColor = new Color(1, 1, 1, 0.5f);
    private readonly Color HighlightColor = new Color(0, 1, 0, 0.8f);
    public void HideAndDehighlightAllSiteButtons()
    {
        foreach (var button in SiteButtonList)
        {
            button.gameObject.SetActive(false);
            button.GetComponent<Image>().color = DehighlightColor;
        }
    }

    public void ShowHideSiteButton(int siteIndex, bool show)
    {
        SiteButtonList[siteIndex].gameObject.SetActive(show);
    }

    public void HighlightAllSiteButtons(bool highlight)
    {
        foreach (var button in SiteButtonList)
        {
            button.GetComponent<Image>().color = highlight ? HighlightColor : DehighlightColor;
        }
    }
    public void HighlightSiteButton(int siteIndex, bool highlight)
    {
        SiteButtonList[siteIndex].GetComponent<Image>().color = highlight ? HighlightColor : DehighlightColor;
    }
}