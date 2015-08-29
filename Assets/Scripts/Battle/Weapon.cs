using System;
using Fairwood.Math;
using UnityEngine;
using UnityEngine.Networking;

[Serializable]
public class Weapon : MonoBehaviour
{
    public int SlotID;

    public string Name
    {
        get { return Info.name; }
    }
    public Unit Caster;
    public WeaponInfo Info;

    public float CDRemaining;
    public float Amount;

    public Transform LaunchPoint;

    public void Reset(Unit caster, int slotID)
    {
        Debug.LogFormat("Skill.Reset");
        Caster = caster;
        SlotID = slotID;
        CDRemaining = 0;
    }

    public void Update(float dt)
    {
        CDRemaining = Mathf.Max(0, CDRemaining - dt);
    }


    public void Fire(Vector3 displacement)
    {
        CDRemaining += 1f/Info.FiringRate;
        Amount--;
        Caster.CmdFireWeapon(SlotID, displacement);
    }

    public void OnAttackFrame(Vector3 displacement)
    {
        switch (SlotID)
        {
            case 1:
                Caster.CmdCreateProjectile2(displacement);
                break;
        }
    }
    //[Server]
    //只在Server执行
    public void CreateProjectile(Vector3 displacement)
    {
        var ratio = 0.9f;

        //var go = Network.Instantiate(ArrowPrefab, Vector3.zero, Quaternion.identity, 0);
        var go = PrefabHelper.InstantiateAndReset(Info.ProjectilePrefab, null);
        go.transform.position = LaunchPoint.position;
        go.transform.forward = displacement;
        var rigid = go.GetComponent<Rigidbody>();
        var projectile = go.GetComponent<Projectile>();
        if (Info.IsSpeedFixed)
        {
            rigid.velocity = Info.InitialSpeed*displacement.normalized;
        }
        else
        {
            var actualDisplacement = displacement.normalized*
                                     (displacement.magnitude - LaunchPoint.localPosition.z);
            const float tanTheta = 1;
            var vx = Mathf.Sqrt(actualDisplacement.magnitude*projectile.Gravity/2/tanTheta);
            var vy = vx*tanTheta;
            rigid.velocity = vx*actualDisplacement.normalized + vy*Vector3.up;
        }
        projectile.Launcher = Caster;
        NetworkServer.Spawn(go);
    }
}