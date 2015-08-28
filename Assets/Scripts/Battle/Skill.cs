using System;
using UnityEngine;

[Serializable]
public class Skill
{
    public int SlotID;
    public Unit Caster;
    public SkillInfo Info;

    public float CDRemaining;
    public float Amount;

    public void Reset(Unit caster, int slotID)
    {
        Caster = caster;
        SlotID = slotID;
        CDRemaining = Info.CD;
    }

    public void Update(float dt)
    {
        CDRemaining -= dt;
    }


    public void Start(Vector3 displacement)
    {
        CDRemaining = Info.CD;
        Amount--;
        OnAttackFrame(displacement);
    }

    public void OnAttackFrame(Vector3 displacement)
    {
        Caster.CmdCreateProjectile1(displacement);
    }

    public void Finish()
    {
        if (Caster.CurrentCastingSkill == this)
        {
            Caster.CurrentCastingSkill = null;
        }
    }

    public void Interrupt()
    {
        
    }
}