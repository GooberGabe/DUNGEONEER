using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hazard : Entity
{
    public float cooldownTime;

    protected float cooldownCounter;

    public override EntityType entityType => throw new NotImplementedException();

    public virtual void Engage()
    {
        
    }

    public virtual void Disengage()
    {

    }

    protected override void Update()
    {
        base.Update();
        if (cooldownCounter > 0)
        {
            cooldownCounter -= Time.deltaTime;
        }
    }

    protected void StartCooldown()
    {
        cooldownCounter = cooldownTime;
    }

    protected bool CanEngage()
    {
        return cooldownCounter <= 0;
    }

}
