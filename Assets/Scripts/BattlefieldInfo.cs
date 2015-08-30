using UnityEngine;

/// <summary>
/// Summary
/// </summary>
public class BattlefieldInfo : MonoBehaviour
{
    //public Transform[] RespawnPositionList;
    public Bunker[] BaseBunkerList;

    void Awake()
    {
        var umanager = FindObjectOfType<UManager>();
        if (umanager) umanager.BattlefieldInfo = this;

        for (int i = 0; i < BaseBunkerList.Length; i++)
        {
            if (BaseBunkerList[i]) BaseBunkerList[i].GetComponent<HomeBunker>().Camp = i;
        }
    }
}