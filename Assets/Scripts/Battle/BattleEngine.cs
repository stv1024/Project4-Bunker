using Fairwood.Math;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

/// <summary>
/// 战斗控制器
/// </summary>
public class BattleEngine : MonoBehaviour
{
    public static BattleEngine Instance { get; private set; }

    //public AttackControlPad AttackControlPad;
    public WeaponControlJoystick WeaponControlJoystick;

    public static bool IsHost;

    public Parameters Parameters;
    public GameObject PlayerUnitPrefab;

    public AnimationCurve OnDamagedRedFlashCurve;
    public float OnDamagedRedFlashDuraion = 0.5f;

    void Awake()
    {
        Instance = this;
        Parameters.JumpingGravity = -8 * Parameters.JumpingHeight / (Parameters.JumpingTime * Parameters.JumpingTime);
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        if (UManager.Instance)//联机模式
        {
            IsHost = false;
        }
        else//单机测试模式
        {
            IsHost = true;
            var go = PrefabHelper.InstantiateAndReset(PlayerUnitPrefab, null);
        }
    }

    public RectTransform Arrow;
    public RectTransform ThinArrow;


    public void OnUnitDie(Unit unit, Unit killer)
    {
        //if (killer && killer != unit) killer.Data.KillCount += 1;
    }

    public void OnAvatarClick()
    {
        var hud = NetworkManager.singleton.GetComponent<UManagerHUG>();
        hud.showGUI = !hud.showGUI;
    }
}