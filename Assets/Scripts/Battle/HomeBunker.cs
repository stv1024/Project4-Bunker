using UnityEngine;

/// <summary>
/// 基地
/// </summary>
public class HomeBunker : MonoBehaviour
{
    private Bunker _bunker;
    [HideInInspector] public int Camp;

    void Awake()
    {
        _bunker = GetComponent<Bunker>();
    }

    void Update()
    {
        if (_bunker._hp <= 0)
        {
            var win = Camp != UnitController.Instance.FocusedUnit.Data.Camp;
            
        }
    }
}