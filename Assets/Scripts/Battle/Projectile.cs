using System;
using Fairwood.Math;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// 飞行道具
/// </summary>
public class Projectile : NetworkBehaviour
{
    public enum AfterHitBehaviorEnum
    {
        DoNothing,
        Remain,
        Terminate,
    }
    public AfterHitBehaviorEnum AfterHitBehavior;
    public enum AfterLifespanBehaviorEnum
    {
        Terminate,
        Fall,
    }
    public AfterLifespanBehaviorEnum AfterLifespanBehavior;
    
    public bool ForwardToVelocity;
    public float Lifespan;
    public float Gravity;
    public float DamagePower = 400;
    public float ExplodeForce = 10;
    public float ExplodeRadius = 10;

    [HideInInspector] public Unit Launcher;
    private float _remainingLifespan;
    private bool _hasEnbledHittingLauncher;

    public GameObject ExplodePrefab;

    private Vector3 _lastPosition;

    public override void OnStartServer()
    {
        _remainingLifespan = Lifespan;
        var constForce = GetComponent<ConstantForce>();
        if (constForce)
        {
            if (Math.Abs(Gravity) > 0.0001f)
            {
                constForce.force = Vector3.down*GetComponent<Rigidbody>().mass*Gravity;
                constForce.enabled = true;
            }
            else
            {
                constForce.enabled = false;
            }
        }

        var myCollider = GetComponent<Collider>();
        if (Launcher && myCollider && myCollider.enabled) Physics.IgnoreCollision(GetComponent<Collider>(), Launcher.GetComponent<Collider>());

        _lastPosition = transform.position;
    }

    [ServerCallback]
    protected virtual void Update()
    {
        if (ForwardToVelocity)
        {
            var rigid = GetComponent<Rigidbody>();
            if (rigid && rigid.velocity.sqrMagnitude > 0)
            {
                transform.forward = rigid.velocity;
            }
        }
        if (!_hasEnbledHittingLauncher && Lifespan - _remainingLifespan > 0.3f)
        {
            var myCollider = GetComponent<Collider>();
            if (Launcher && myCollider && myCollider.enabled) Physics.IgnoreCollision(myCollider, Launcher.GetComponent<Collider>(), false);//处理和发射者的碰撞
            _hasEnbledHittingLauncher = true;
        }
        if (Lifespan > 0 && _remainingLifespan > 0)
        {
            _remainingLifespan -= Time.deltaTime;
            if (_remainingLifespan <= 0)
            {
                switch (AfterLifespanBehavior)
                {
                    case AfterLifespanBehaviorEnum.Terminate:
                        Terminate();
                        break;
                    case AfterLifespanBehaviorEnum.Fall:
                        var rigid = GetComponent<Rigidbody>();
                        if (rigid)
                        {
                            rigid.velocity *= 0.6f;
                        }
                        var constForce = GetComponent<ConstantForce>();
                        if (constForce)
                        {
                            constForce.enabled = true;
                        }
                        break;
                }
            }
        }
    }


    [ServerCallback]
    protected virtual void FixedUpdate()
    {
        var myCollider = GetComponent<Collider>();
        if (!myCollider || !myCollider.enabled)
        {
            RaycastHit hitInfo;
            var justDisplacement = transform.position - _lastPosition;
            var isHit = Physics.Raycast(new Ray(_lastPosition, justDisplacement), out hitInfo,
                justDisplacement.magnitude);
            if (isHit)
            {
                transform.position = hitInfo.point;
                var other = hitInfo.collider;

                OnTriggerEnter(other);
            }
        }
        _lastPosition = transform.position;
    }
    protected virtual void OnTriggerEnter(Collider other)
    {
        var unit = other.GetComponent<Unit>();
        TakeEffectAt(unit, transform.position);
        switch (AfterHitBehavior)
        {
            case AfterHitBehaviorEnum.DoNothing:
                break;
            case AfterHitBehaviorEnum.Remain:
                if (other.transform.lossyScale == Vector3.one)
                {
                    transform.parent = other.transform;
                }
                Destroy(GetComponent<ConstantForce>());
                var rigid = GetComponent<Rigidbody>();
                var otherRgd = other.GetComponent<Rigidbody>();
                if (otherRgd)
                {
                    var impulse = rigid.mass * rigid.velocity;
                    otherRgd.AddForceAtPosition(impulse, rigid.worldCenterOfMass, ForceMode.Impulse);
                }
                Destroy(rigid);
                GetComponent<NetworkTransform>().transformSyncMode =
                    NetworkTransform.TransformSyncMode.SyncTransform;
                GetComponent<Collider>().enabled = false;
                enabled = false;

                Destroy(gameObject);
                NetworkServer.Destroy(gameObject);
                //Destroy(gameObject, 5);
                break;
            case AfterHitBehaviorEnum.Terminate:
                Terminate();
                break;
        }
    }

    [Server]
    protected virtual void Hit(Unit target)
    {
        TakeEffectOn(target);
    }

    [Server]
    protected virtual void TakeEffectAt(Unit target, Vector3 location)
    {
        if (ExplodeRadius > 0)
        {
            var colliders = Physics.OverlapSphere(location, ExplodeRadius);
            foreach (var cldr in colliders)
            {
                var unit = cldr.GetComponent<Unit>();
                if (unit && unit.Data.IsAlive)
                {
                    var toCurUnitVector = (unit.transform.position - location);
                    var hasBlock = Physics.Raycast(location, toCurUnitVector, toCurUnitVector.magnitude,
                        LayerManager.Mask.Ground);
                    if (!hasBlock)
                    {
                        TakeEffectOn(unit);
                        unit.PushBack(ExplodeForce, location, ExplodeRadius);
                    }
                }
            }
            RpcPlayPointEffect(location);
        }
        else
        {
            if (target)
            {
                Hit(target);
            }
        }

        if (ExplodePrefab)
        {
            var go = PrefabHelper.InstantiateAndReset(ExplodePrefab, null);
            go.transform.position = location;
            Destroy(go, 10);
        }
    }

    [Server]
    protected virtual void TakeEffectOn(Unit target)
    {
        target.TakeDamage(Launcher, DamagePower);
    }

    [Server]
    protected virtual void Terminate()
    {
        Destroy(gameObject);
        NetworkServer.Destroy(gameObject);
    }

    [ClientRpc]
    public void RpcPlayPointEffect(Vector3 location)//TODO:和Destroy同时故无法在客户端生效
    {
        var go = PrefabHelper.InstantiateAndReset(ExplodePrefab, null);
        go.transform.position = location;
        Destroy(go, 10);
    }
}