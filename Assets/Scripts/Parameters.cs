using System.IO;
using UnityEngine;

/// <summary>
/// 参数们
/// </summary>
public class Parameters : ScriptableObject
{
    public static Parameters Instance
    {
        get { return BattleEngine.Instance.Parameters; }
    }
    //[MenuItem("Assets/Create/Parameters")]
    //public static void CreateAsset()
    //{
    //    string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath("Assets/Parameters.asset");
    //    AssetDatabase.CreateAsset(CreateInstance<Parameters>(), assetPathAndName);
    //    AssetDatabase.SaveAssets();
    //    AssetDatabase.Refresh();
    //}

    public float JumpingHeight = 1;
    public float JumpingTime = 0.5f;
    public float JumpingGravity;

    public float BombXZSpeed = 5;
    public float BombGravity = -2f;
}