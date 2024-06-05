using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Monster : Creature
{
    public override EntityType entityType { get; } = EntityType.Monster;
    public Spawner spawnOrigin;

    protected override void Start()
    {
        base.Start();
        if (spawnOrigin != null) mainDestination = spawnOrigin.transform;
    }

    protected override void Update()
    {
        base.Update();
        NavigationBehavior();
    }

    protected override void NavigationBehavior()
    {
        // Filter out non-creatures and sort the remaining entities based on distance and numFoes
        List<Entity> validTargets = entitiesInRange.Where(x => x.entityType == EntityType.Hero || x.entityType == EntityType.Monster).ToList().OrderBy(
            x => -Vector3.Distance(transform.position,x.transform.position) - ((Creature)x).foes.Count).ToList();

        if (validTargets.Count > 0)
        {
            // Check all entities in range, starting with the first one that was detected, to see if any of them are eligible to be targeted
            for (int i = 0; i < validTargets.Count; i++)
            {
                if (spawnOrigin != null)
                {
                    if (Vector3.Distance(transform.position, spawnOrigin.transform.position) > spawnOrigin.gameObject.GetComponent<CapsuleCollider>().radius * 1.1f)
                    {
                        Debug.Log("Too far!");
                        continue;
                    }
                }
                if (IsTargetInLineOfSight(validTargets[i].transform))
                {
                    subDestination = validTargets[i].transform;
                    Face(validTargets[i].transform);
                }

            }
        }
        base.NavigationBehavior();
        
    }

    protected override void Arrive(Transform target)
    {
        base.Arrive(target);
        if (target.GetComponent<Hero>() != null)
        {
            Hero hero = target.GetComponent<Hero>();
            Attack(hero);
        }
    }
}
