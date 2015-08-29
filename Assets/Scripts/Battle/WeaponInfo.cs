using System;
using UnityEditor;
using UnityEngine;

public class WeaponInfo : ScriptableObject
{
    [MenuItem("Assets/Create/WeaponInfo")]
    public static void CreateAsset()
    {
        string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath("Assets/Data/NewWeaponInfo.asset");
        AssetDatabase.CreateAsset(CreateInstance<WeaponInfo>(), assetPathAndName);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
    public float DragToDisplacementRatio = 0.05f;

    public bool Automatic;
    public float FiringRate = 1;


    public bool IsSpeedFixed;
    public float InitialSpeed;

    public GameObject ProjectilePrefab;
}