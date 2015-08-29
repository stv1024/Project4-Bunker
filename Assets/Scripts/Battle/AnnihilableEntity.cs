using UnityEngine;

/// <summary>
/// 可以被毁坏的。包括Unit、Bunker等
/// </summary>
public interface IAnnihilable
{
    float TakeDamage(Unit caster, float power);

    Transform GetTransform();
}