using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;
using static UnityEngine.UIElements.UxmlAttributeDescription;

public class Spawner : Hazard
{
    public override EntityType entityType { get; } = EntityType.Spawner;

    public GameObject creatureToSpawn;
    public float spawnRadius;
    public int maxSpawns;
    public bool alwaysSpawn = true;
    public bool respawnDuringRound = false;
    public bool respawnRoundStart = true;

    protected List<Entity> monsters = new List<Entity>();
    
    protected override void Start()
    {
        base.Start();
        if (respawnRoundStart)
        {
            for (int i = 0; i < maxSpawns; i++)
            {
                Engage();
            }
        }
    }

    public override string TextDisplay()
    {
        return "Spawn Count: " + monsters.Count + "/" + maxSpawns;
    }

    public override void Engage()
    {
        base.Engage();
        int x = -1;
        int z = -1;
        Vector3 pos = Vector3.one;

        for (int i = 0; i < 100; i++)
        {
            pos = new Vector3(transform.position.x + Random.Range(-spawnRadius, spawnRadius), transform.position.y, transform.position.z + Random.Range(-spawnRadius, spawnRadius));
            
            x = Mathf.RoundToInt(pos.x);
            z = Mathf.RoundToInt(pos.z);
            if (GameManager.instance.grid.GetTile(x, z) != null) break;
        }

        Monster g = (Monster)GameManager.instance.Spawn(creatureToSpawn, pos);
        g.spawnOrigin = this;
        monsters.Add(g);
        StartCooldown();
    }

    public override void Upgrade()
    {
        DestroyMonsters();
        base.Upgrade();
        Debug.Log("A");
    }

    public override void Sell()
    {
        DestroyMonsters();
        base.Sell();
    }

    private void DestroyMonsters()
    {
        foreach (Entity m in monsters)
        {
            m.Poof();
        }
    }

    protected override void Update()
    {
        base.Update();
        Cleanup();
        CheckEngage();
            
    }

    protected void Cleanup()
    {
        List<Entity> entitiesCopy = new List<Entity>(monsters);
        foreach (Entity entity in entitiesCopy)
        {
            if (entity == null || !entity.enabled) monsters.Remove(entity);
        }
    }

    protected virtual void CheckEngage()
    {
        if (monsters.Count >= maxSpawns) StartCooldown();
        if (CanEngage() && respawnDuringRound && GameManager.instance.playRound)
        {
            if ((entitiesInRange.Count > 0 || alwaysSpawn) && monsters.Count < maxSpawns)
            {
                Engage();
            }

        }

        if (respawnRoundStart)
        {
            while (!GameManager.instance.playRound && monsters.Count < maxSpawns)
            {
                Engage();
            }
        }
        
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position + (Vector3.up * 0.5f), spawnRadius);
    }
}
