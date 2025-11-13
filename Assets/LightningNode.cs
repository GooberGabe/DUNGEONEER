using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LightningNode : EffectZone
{
    public Entity origin;
    public LightningNode connection;
    public int numJumps;
    public LightningEffect lightningEffect;

    // We deal damage to Origin, if it exists.
    // We spawn a new LightningNode at Connection.


    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
        if (origin) entitiesInRange.Remove(origin);
        if (connection == null && entitiesInRange.Count > 0 && numJumps > 0)
        {
            IEnumerable<Entity> available = entitiesInRange.Where(target => !((Creature)target).HasStatusEffect(StatusEffect.Shocked));
            Entity nearest = available.OrderBy(target => Vector3.Distance(transform.position, target.transform.position)).FirstOrDefault();
            
            if (nearest != null && nearest != origin)
            {
                Propagate(nearest);
            }
        }
    }

    private void OnDestroy()
    {
        if (origin != null)
        {
            ((Creature)origin).SetStatusEffect(StatusEffect.Shocked, 0);
        }
        if (connection != null)
        {
            Destroy(connection);
        }
    }

    void Propagate(Entity target)
    {
        HandleStatusEffects(target);
        GameObject newNode = Instantiate(gameObject);
        newNode.transform.position = target.transform.position;
        LightningNode lightning = newNode.GetComponent<LightningNode>();
        connection = lightning;
        lightning.numJumps = numJumps - 1; 
        lightning.origin = target;
        Vector3 endpoint = target.transform.position + (target.transform.forward * 0.25f);
        lightningEffect.endObject.position = new Vector3(endpoint.x, lightningEffect.endObject.position.y, endpoint.z);
        lightning.source = source;
    }

    public override bool DealDamage(int amount)
    {

        if (origin != null)
        {
            origin.TakeDamage(amount);
            HandleStatusEffects(origin);
            return true;
        }
        return false;
            
    }

}
