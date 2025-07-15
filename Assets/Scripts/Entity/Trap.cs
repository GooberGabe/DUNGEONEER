using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Trap : Hazard
{
    public override EntityType entityType { get; } = EntityType.Trap;

    /// <summary>
    /// Can be a reference to either a prefab, or an existing object in the hierarchy
    /// </summary>
    public GameObject hitBox;
    public Vector3 effectOffset;
    public int maxUses;
    public float disengageTime = 10;
    private int uses;

    public override string TextDisplay()
    {
        return
              "Damage: " + GetDamage() +
              (maxUses > 0 ? "\nCharges: " + maxUses : "") +
              (cooldownTime > 0 ? "\nCooldown Time: " + cooldownTime + "s" : "");
    }

    public override string StatusDisplay()
    {
        return (maxUses > 0 ? "\nCharges: " + uses + "/" + maxUses : "");
    }

    public override void Engage()
    {
        base.Engage();
        StartCooldown();
        GetComponent<Animator>().SetBool("engaged",true);
        uses++;
    }

    public override void Disengage()
    {
        base.Disengage();
        GetComponent<Animator>().SetBool("engaged", false);
    }

    public void Hit()
    {
        EffectZone ez = hitBox.GetComponent<EffectZone>();
        ez.DealDamage(ez.damage);
        if (uses >= maxUses && maxUses > 0)
        {
            Destroy(gameObject, 0.1f);
            Poof();
        }
    }

    public override float GetDamage()
    {
        return hitBox.GetComponent<EffectZone>().damage;
    }

    public void Disjoint()
    {
        GameObject newEffect = Instantiate(hitBox, transform.position, Quaternion.identity);
    }

    protected override void Update()
    {

        base.Update();
        if (CanEngage())
        {
            if (entitiesInRange.Count > 0)
            {
                Engage();
            }
        } 
        else
        {
            if (cooldownCounter >= disengageTime) Disengage();
        }

    }
}
