using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : Creature
{
    public override EntityType entityType { get; } = EntityType.Hero;

    protected override void Update()
    {
        base.Update();
        NavigationBehavior();
    }

    protected override void Start()
    {
        base.Start();
        GameManager.instance.totalHeroes++;
    }

    protected override void NavigationBehavior()
    {
        base.NavigationBehavior();
    }

    protected override void Die()
    {
        if (isAlive)
        {
            GameManager.instance.gold += cost;
            GameManager.instance.totalHeroes--;
        }
        base.Die();

    }

    protected override void Arrive(Transform target)
    {
        base.Arrive(target);
        if (target.GetComponent<Monster>() != null)
        {
            Monster monster = target.GetComponent<Monster>();
            Attack(monster);
        }
    }
}
