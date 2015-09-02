using Fairwood.Math;
using UnityEngine;

/// <summary>
/// 掩体
/// </summary>
public class Bunker : MonoBehaviour, IAnnihilable
{
    public BunkerSite[] SiteList;
    public const int SeatsPerSite = 2;
    public float HP = 300;
    public float _hp;

    public Transform Entity;
    private Vector3 _oriScale;

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

        _hp = HP;
        _oriScale = transform.localScale;

        BattleEngine.Instance.BunkerList.Add(this);
    }

    public void OnSiteClick(int siteID)
    {
        Debug.LogFormat("OnSiteClick");
        UnitController.Instance.FocusedUnit.Walker.WalkTo(SiteList[siteID].Position);
    }

    public float TakeDamage(Unit caster, float power)
    {
        var dmg = power*0;//0.5f;

        _hp -= dmg;

        if (_hp <= 0)
        {
            Destroy(Entity.gameObject);
            enabled = false;
        }
        else
        {
            var scale = transform.localScale;
            var ratio = _hp/HP*0.8f + 0.2f;
            scale.x = _oriScale.x * ratio;
            scale.z = _oriScale.z * ratio;
            transform.localScale = scale;
        }

        return dmg;
    }
    public Transform GetTransform()
    {
        return transform;
    }
}