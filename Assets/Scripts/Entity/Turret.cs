using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Turret : DynamicEntity
{
    protected int cooldownCounter;
    public int cooldownTime;
    public override EntityType entityType => EntityType.Turret;

    public override string TextDisplay()
    {
        return "Range:  " + GetComponent<CapsuleCollider>().radius.ToString() +
             "\nDamage: " + projectile.GetComponent<Projectile>().damage;
    }


    public override void Fire()
    {
        base.Fire();
        cooldownCounter = cooldownTime;
    }

    protected List<Entity> GetValidTargets()
    {
        return entitiesInRange.Where(x => detectedTypes.Contains(x.entityType)).ToList().OrderBy(
            x => -Vector3.Distance(transform.position, x.transform.position)).ToList();
    }

    private void LockTarget()
    {
        List<Entity> validTargets = GetValidTargets();

        if (validTargets.Count > 0)
        {
            // Check all entities in range, starting with the first one that was detected, to see if any of them are eligible to be targeted
            for (int i = 0; i < validTargets.Count; i++)
            {
                if (IsTargetInLineOfSight(validTargets[i].transform) && validTargets[i].hitPoints > 0)
                {
                    mainDestination = validTargets[i].transform;
                    Face(mainDestination);
                }

            }
        }
    }

    private bool CanFire()
    {
        return cooldownCounter == 0;
    }

    protected override void Update()
    {
        base.Update();
        if (cooldownCounter > 0)
        {
            cooldownCounter--;
        }

        LockTarget();

        if (CanFire())
        {
            if (GetTarget() != null)
            {
                Fire();
            }
        }
    }
}
