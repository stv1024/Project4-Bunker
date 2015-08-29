using UnityEngine;

/// <summary>
/// 临时的掩体的墙，用于传递伤害事件，以后优化
/// </summary>
public class BunkerWall : MonoBehaviour, IAnnihilable
{
    public Bunker Bunker;
    public float TakeDamage(Unit caster, float power)
    {
        return Bunker.TakeDamage(caster, power);
    }
    public Transform GetTransform()
    {
        return transform;
    }
}