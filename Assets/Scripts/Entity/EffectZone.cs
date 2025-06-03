using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EffectZone : Entity
{
    private int frameTick;
    private float tick;
    public float counter;
    public bool areaOn = true;
    public int damage;
    public float damageInterval = 0;
    public bool destroyObject = true;
    public StatusEffect[] statusEffects;
    public float effectDuration = 0;

    // Other possible effects: Knockback, status effect, etc

    public override EntityType entityType { get; } = EntityType.Zone;

    protected override void Start()
    {   
        base.Start();
    }

    protected override void Update()
    {
        bool dealtDamage = false;
        if (areaOn && (damageInterval == 0 || tick >= damageInterval))
        {
            dealtDamage = DealDamage(damage);
            tick = 0;
        }
        if (counter <= 0 && (frameTick >= 2 || dealtDamage) && counter > -99)
        {
            if (destroyObject) Destroy(gameObject);
            else Destroy(this);
        }
        counter -= Time.deltaTime;
        tick += Time.deltaTime;
        frameTick++;
    }

    public bool DealDamage(int amount)
    {
        bool ret = false;
        Debug.Log("--A: Deal Damage");
        foreach (Entity entity in entitiesInRange)
        {
            Debug.Log("--B: Detect Entity");
            if (entity != null)
            {
                if (damage > 0) entity.TakeDamage(amount);
                if (entity.entityType == EntityType.Hero || entity.entityType == EntityType.Monster) 
                {
                    for (int i = 0; i < statusEffects.Length; i++)
                    {
                        ((Creature)entity).SetStatusEffect(statusEffects[i], effectDuration);
                    }
                }
                ret = true;

            }
        }
        return ret;
    }
}

public enum StatusEffect
{
    Burning,
    Slowed,
}
