using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EffectZone : Entity
{
    public int counter;
    public bool areaOn = true;
    public int damage;
    public bool destroyObject = true;

    // Other possible effects: Knockback, status effect, etc

    public override EntityType entityType { get; } = EntityType.Zone;

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        if (areaOn)
        {
            DealDamage(damage);
        }
        if (counter == 0)
        {
            if (destroyObject) Destroy(gameObject);
            else Destroy(this);
        }
        counter--;
    }

    public void DealDamage(int amount)
    {
        foreach (Entity entity in entitiesInRange)
        {
            if (entity != null)
            {
                entity.TakeDamage(amount);
            }
        }
    }
}
