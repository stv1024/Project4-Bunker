using System;
using UnityEngine;

[Serializable]
public class Skill
{
    public int SlotID;
    public string Name;
    public Unit Caster;
    public SkillInfo Info;

    public float CDRemaining;
    public float Amount;

    public void Reset(Unit caster, int slotID)
    {
        Debug.LogFormat("Skill.Reset");
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
        switch (SlotID)
        {
            case 0:
                Caster.CmdCreateProjectile1(displacement);
                break;
            case 1:
                Caster.CmdCreateProjectile2(displacement);
                break;
        }
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