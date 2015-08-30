using UnityEngine;
using System.Collections;
using Fairwood.Math;
using UnityEngine.Assertions;
using UnityEngine.Networking;

public class Unit : NetworkBehaviour, IAnnihilable
{
    public enum StateEnum
    {
        Idle,
        Running,
        Aiming,
        Firing,
        Hurt
    }

    public UnitWalker Walker;
    public UnitData Data;

    public UnitInfoCanvas UnitInfoCanvas;
    public Animator Animator;
    public Renderer ActorRenderer;

    public Transform LaunchPoint;
    public GameObject ArrowPrefab;
    public GameObject BombPrefab;

    public StateEnum State = StateEnum.Idle;

    public Vector3 Position
    {
        get { return transform.position.SetV3Y(0); }
    }

    public Weapon[] SkillList;
    public Weapon CurrentSelectedWeapon;

    //public float[] SkillDragToDisplacementRatioList;

    //public int[] SkillOriginalAvailableCountList = { 3, int.MaxValue, 1 };

    //public float[] SkillCDList = {0f, 3f, 3f};

    private float _redFlashLeftTime;

    private Vector3 _rebirthPosition;

    public Transform GetTransform()
    {
        return transform;
    }

    #region 信息同步
    public override void OnStartServer()
    {
        Debug.LogFormat("OnStartServer {0}", Position);
        base.OnStartServer();
        BattleEngine.IsHost = true;
    }
    public override void OnStartClient()
    {
        Debug.LogFormat("OnStartClient {0}", Position);
        if (Data.Camp == 0 && !BattleEngine.IsHost)
        {
            Destroy(GetComponent<AI>());
            enabled = false;
        }
    }

    void Awake()
    {
        var traUnitInfoCanvas = transform.FindChild("Canvas-Unit");
        if (traUnitInfoCanvas)
        {
            UnitInfoCanvas = traUnitInfoCanvas.GetComponent<UnitInfoCanvas>();
            if (UnitInfoCanvas != null) UnitInfoCanvas.Owner = this;
        }
    }

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        Debug.LogFormat("Unit.OnStartLocalPlayer {0};{1} {2}", GetComponent<NetworkIdentity>().hasAuthority, GetComponent<NetworkIdentity>().isLocalPlayer, netId);
        if (isLocalPlayer)
        {
            UnitController.Instance.Init(this);
            BattleEngine.Instance.WeaponControlJoystick.Init(this);
            CmdRequestCampInfo();
        }

        NetworkManager.singleton.GetComponent<UManagerHUG>().showGUI = false;

        for (int i = 0; i < SkillList.Length; i++)
        {
            SkillList[i].Reset(this, i);
        }
        CmdSwitchWeapon(0);
    }

    [Command]
    void CmdRequestCampInfo()
    {
        Debug.LogFormat("CmdRequestCampInfo({0})", name);
        UManager.Instance.DidAddPlayer(this);
    }

    [Server]
    public void ResetCampInfo(int playerID, int camp)
    {
        Debug.LogFormat("Unit{1}.SResetCampInfo({0})", camp, playerID);
        Data.PlayerID = playerID;
        Data.Camp = camp;
        RpcResetCampInfo(playerID, camp);
    }
    [ClientRpc]
    public void RpcResetCampInfo(int playerID, int camp)
    {
        var position = UManager.Instance.BattlefieldInfo.BaseBunkerList[camp].SiteList[0].Position.SetV3Y(1.01f);
        Debug.LogFormat("Unit{2}.RpcResetCampInfo({0},{1})", camp, position, playerID);
        name = "Player Unit " + playerID;
        if (isLocalPlayer)
        {
            transform.position = position;
            _rebirthPosition = Position;
        }
        if (ActorRenderer) ActorRenderer.material.mainTexture = Parameters.Instance.CampTextureList[Data.Camp];
    }


    [Server]
    public override void OnNetworkDestroy()
    {
        base.OnNetworkDestroy();
        if (Data.IsPlayer)
        {
            UManager.Instance.DidRemovePlayer(this);
        }
    }

    #endregion

    #region 移动

    public BunkerSite CurrentBunkerSite;
    public BunkerSite DestinationBunkerSite;
    public void SetDestination(BunkerSite destinationBunkerSite)
    {
        DestinationBunkerSite = destinationBunkerSite;
    }

    void UpdateMove()
    {
        if (CurrentBunkerSite && Vector3.Distance(Position, CurrentBunkerSite.Position) > 0.1f)
        {
            CurrentBunkerSite.OnUnitExit(this);
            CurrentBunkerSite = null;
        }
        if (DestinationBunkerSite && Vector3.Distance(Position, DestinationBunkerSite.Position) <= 0.1f)
        {
            CurrentBunkerSite = DestinationBunkerSite;
            DestinationBunkerSite.OnUnitEnter(this);
            DestinationBunkerSite = null;
        }
    }

    #endregion

    //[Command]
    public void CmdSwitchWeapon(int skillSlot)
    {
        if (skillSlot < SkillList.Length) CurrentSelectedWeapon = SkillList[skillSlot];
    }

    public void Fire(Vector3 displacement)
    {
        FireWeapon(CurrentSelectedWeapon, displacement);
    }
    public void FireWeapon(Weapon weapon, Vector3 displacement)
    {
        if (weapon.CDRemaining > 0 || weapon.Amount <= 0)
        {
            return;
        }
        //CurrentCastingSkill = skill;
        weapon.Fire(displacement);
        if (Animator)
        {
            Animator.SetTrigger("CastSkill1");
        }
        CmdCastAtkSkill(displacement);
    }

    [Command]
    public void CmdCastAtkSkill(Vector3 displacement)
    {
        if (Animator)
        {
            Animator.SetTrigger("CastSkill1");
        }
    }

    void Update()
    {
        var dt = Time.deltaTime;
        if (Data.IsAlive)
        {
            UpdateMove();
            Data._globalCDRemaining -= dt;
            for (int i = 0; i < SkillList.Length; i++)
            {
                SkillList[i].Update(dt);
            }

            Sethp(Data.hp + Data.HPR * dt);

            if (_redFlashLeftTime > 0)
            {
                _redFlashLeftTime -= Time.deltaTime;

                var color = (Color.white *
                             BattleEngine.Instance.OnDamagedRedFlashCurve.Evaluate(1 -
                                                                                     _redFlashLeftTime /
                                                                                     BattleEngine.Instance
                                                                                         .OnDamagedRedFlashDuraion));
                color.r = 1;
                GetComponent<Renderer>().material.color = color;
            }
        }

        if (UnitInfoCanvas)
        {
            UnitInfoCanvas.transform.rotation = UnitController.Instance.Camera.transform.rotation;
        }
    }

    public void PushBack(float force, Vector3 explodePosition, float explodeRadius)
    {
        var rgd = GetComponent<Rigidbody>();
        rgd.AddExplosionForce(force, explodePosition, explodeRadius);
        rgd.AddForce(Vector3.up / ((explodePosition - transform.position).magnitude + 1) * 2000);
    }
    public float TakeDamage(Unit caster, float power)
    {
        if (!Data.IsAlive) return 0;

        var dmg = power / (1 + 0.05f * Data.ARM);
        var validDmg = Mathf.Min(Data.hp, dmg);
        if (caster && caster.Data.Camp != Data.Camp) caster.Data.CausedDamage += validDmg;
        Sethp(Data.hp - dmg);
        if (Data.hp <= 0)
        {
            Die(caster);
            return dmg;
        }
        _redFlashLeftTime = BattleEngine.Instance.OnDamagedRedFlashDuraion;


        return dmg;
    }

    public void Sethp(float newhp)
    {
        Data.hp = Mathf.Clamp(newhp, 0, Data.HP);
    }
    public void Setmp(float newmp)
    {
        Data.mp = Mathf.Clamp(newmp, 0, Data.MP);
    }

    [Server]
    public void Die(Unit killer)
    {
        if (!Data.IsAlive) return;
        Data.IsAlive = false;

        var rgd = GetComponent<Rigidbody>();
        if (rgd)
        {
            rgd.constraints = RigidbodyConstraints.None;
        }

        Data.DieCount ++;
        if (Data.Rebirthable)
        {
            UnitInfoCanvas.gameObject.SetActive(false);
            CoroutineManager.StartCoroutine(new CoroutineManager.Coroutine(3, Rebirth));
        }
        else
        {
            Destroy(UnitInfoCanvas.gameObject);
            Destroy(gameObject, 3);
        }

        BattleEngine.Instance.OnUnitDie(this, killer);
    }


    [Command]
    public void CmdFireWeapon(int weaponSlot, Vector3 displacement)
    {
        SkillList[weaponSlot].CreateProjectile(displacement);
    }

    [Command]
    public void CmdCreateProjectile2(Vector3 displacement)
    {
        var go = PrefabHelper.InstantiateAndReset(BombPrefab, null);
        go.transform.position = transform.TransformPoint(new Vector3(0, 1.1f, 0));
        go.transform.forward = displacement;
        var rigid = go.GetComponent<Rigidbody>();
        var ratio = 0.9f;
        var xzSpeed = Parameters.Instance.BombXZSpeed;
        var t = displacement.magnitude / xzSpeed;
        var vy = Parameters.Instance.BombGravity * 0.5f * t;
        rigid.velocity = (displacement.normalized * xzSpeed + Vector3.up * vy) * ratio;
        var projectile = go.GetComponent<Projectile>();
        projectile.Launcher = this;
        //projectile.Lifespan = t;
        go.GetComponent<ConstantForce>().force = Vector3.down * rigid.mass * Parameters.Instance.BombGravity;

        NetworkServer.Spawn(go);
    }

    public float GetSkillMaxGeodesicDistance(int skillID)
    {
        return Mathf.Infinity;
    }


    [Server]
    public void Rebirth()
    {
        Data.IsAlive = true;
        Sethp(Data.HP);
        Setmp(Data.MP);
        UnitInfoCanvas.gameObject.SetActive(true);
        RpcRebirth();

        CmdSwitchWeapon(0);
    }

    [ClientRpc]
    public void RpcRebirth()
    {
        transform.position = _rebirthPosition.AddV3Y(1.01f);
        transform.rotation = Quaternion.identity;
        var rgd = GetComponent<Rigidbody>();
        if (rgd)
        {
            rgd.velocity = Vector3.zero;
            rgd.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        }
    }

}
