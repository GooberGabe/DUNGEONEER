using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Cleric : Hero
{
    protected override void NavigationBehavior()
    {
        if (GetValidTargets().Count > 0 && cooldownCounter == 0)
        {
            cooldownCounter = cooldownTime;
            GetComponent<Animator>().SetBool("Attacking", true);
        }
        base.NavigationBehavior();
    }

    protected override List<Entity> GetValidTargets()
    {
        return entitiesInRange.Where(x => ((Creature)x).hitPoints < maxHitPoints).ToList();
    }

    protected override void Arrive(Transform target)
    {
        //base.Arrive(target);
    }

    public override void Hit()
    {
        foreach (Creature creature in GetValidTargets())
        {
            creature.Heal(strength*2);
            Transform particle = Instantiate(projectile, creature.transform.position, Quaternion.identity).transform;
            particle.parent = creature.transform;
            particle.eulerAngles = new Vector3(-90, 0, 0);

        }
    }
}
