using System.Collections;
using UnityEngine;

public class StaticSpawner : Spawner
{
    public override EntityType entityType { get; } = EntityType.Turret; // This tower should be treated like a turret even though it has the behavior of a Spawner.
    public override void Engage()
    {
        for (int i = 0; i < maxSpawns; i++)
        {
            
            // Calculate angle for even distribution
            float angle = i * (360f / maxSpawns) * Mathf.Deg2Rad;

            // Calculate position on circle
            Vector3 pos = transform.position + new Vector3(
                Mathf.Sin(angle) * spawnRadius,
                0,
                Mathf.Cos(angle) * spawnRadius
            );
            

            // Check if the tile is valid
            if (GameManager.instance.grid.GetTile(pos) != null)
            {
                // Adjust the rounded position back to the center of the tile
                pos = new Vector3(pos.x, transform.position.y, pos.z);

                Entity g = GameManager.instance.Spawn(creatureToSpawn, pos);
                monsters.Add(g);
            }
        }
        GetComponentInChildren<Animator>()?.SetTrigger("engaged");

        StartCooldown();
    }

    protected override void CheckEngage()
    {
        if (monsters.Count > 0 && cooldownCounter < cooldownTime - 0.1f)
        {
            StartCooldown();
        }

        if (GameManager.instance.playRound)
        {
            if (CanEngage() && respawnDuringRound)
            {
                if ((entitiesInRange.Count > 0 || alwaysSpawn) && monsters.Count == 0)
                {
                    Engage();
                }

            }
        }
        else if (CanEngage() && respawnRoundStart && monsters.Count == 0)
        {
            Engage();
        }


    }
}
