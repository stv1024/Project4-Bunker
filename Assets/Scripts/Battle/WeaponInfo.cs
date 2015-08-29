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
    public float ViewportDragToWorldRatio = 20f;//屏幕拖拽像素数/屏幕高像素数*此值=世界坐标的距离

    public bool Automatic;
    public float FiringRate = 1;


    public bool IsSpeedFixed;
    public float InitialSpeed;

    public float WorldAimingDisplacementThreshold = 2;//低于此值，不是武器射不了那么近，而是玩家放弃射击
    public float WorldActualDisplacementMaxLimit = float.PositiveInfinity;

    public GameObject ProjectilePrefab;
}