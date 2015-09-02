using Fairwood.Math;
using UnityEngine;

/// <summary>
/// BunkerGizmo的容器
/// </summary>
public class BunkerGizmoContainer : MonoBehaviour
{
    public GameObject BunkerGizmoPrefab;


    public void CreateBunkerGizmo(Bunker bunker)
    {
        var go = PrefabHelper.InstantiateAndReset(BunkerGizmoPrefab, transform);
        go.transform.position = bunker.transform.position.SetV3Y(0.1f);
        var gizmo = go.GetComponent<BunkerGizmo>();
        bunker.Gizmo = gizmo;
        gizmo.Init(bunker);
    }
}