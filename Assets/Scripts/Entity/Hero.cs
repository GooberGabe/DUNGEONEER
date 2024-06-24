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
        Skin();
    }

    private void Skin()
    {
        foreach (Material mat in gameObject.GetComponentInChildren<SkinnedMeshRenderer>().materials)
        {
            if (mat.name.Contains("Skin"))
            {
                mat.color = GameManager.instance.skinTones[Random.Range(0, GameManager.instance.skinTones.Length - 1)];
            }
        }
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
