using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public abstract class Entity : MonoBehaviour
{

    public Sprite icon;
    public float hitPoints = 1;
    public float maxHitPoints = 1;
    public string entityName;
    public string upgradeName;
    public int cost;
    abstract public EntityType entityType { get; }
    public EntityType[] detectedTypes;
    [SerializeField]
    protected Collider[] rangeColliders;
    public UpgradeTree upgradeTree;
    protected bool isAlive = true;

    protected List<Entity> entitiesInRange = new List<Entity>();
    protected Transform anchorPoint;

    protected virtual void Start()
    {
        
    }

    protected virtual void Update()
    {
        if (hitPoints > maxHitPoints)
        {
            hitPoints = maxHitPoints;
        }

        List<Entity> entitiesCopy = new List<Entity>(entitiesInRange);
        foreach (Entity entity in entitiesCopy) 
        {
            if (entity == null || !entity.enabled || !entity.IsAlive()) entitiesInRange.Remove(entity);
            
        }
    }

    public bool IsAlive()
    {
        return isAlive;
    }

    public virtual string TextDisplay()
    {
        return "";
    }

    public virtual float GetRange()
    {
        return 0; // TODO
    }

    public Collider[] GetRangeColliders()
    {
        return rangeColliders;
    }

    public void Poof(float delay = 0.1f)
    {
        GameObject g = Instantiate((GameObject)Resources.Load("Particles/Poof"), transform.position + (transform.up * 0.5f), Quaternion.identity);
        Destroy(gameObject, delay);
    }

    protected virtual void Die()
    {
        isAlive = false;
        Destroy(gameObject); // If alternative behavior is desired, override behavior without calling base
    }

    public virtual void Upgrade()
    {
        Poof(0);
    }

    public virtual void Sell()
    {
        Destroy(gameObject);
        GameManager.instance.gold += cost - 10;
    }

    public virtual void TakeDamage(float amount, bool hidden = false)
    {
        hitPoints -= amount;
        if (hitPoints <= 0)
        {
            hitPoints = 0;
            Die();
        }
    }

    public virtual void Heal(float amount)
    {
        hitPoints += amount;
        if (hitPoints > maxHitPoints)
        {
            hitPoints = maxHitPoints;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Entity entity = other.gameObject.GetComponent<Entity>();
        if (entity != null && !other.isTrigger)
        {
            if (detectedTypes.Contains(entity.entityType))
            {
                entitiesInRange.Add(entity);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Entity entity = other.gameObject.GetComponent<Entity>();
        if (entity != null && !other.isTrigger)
        {
            if (detectedTypes.Contains(entity.entityType))
            {
                entitiesInRange.Remove(entity);
            }
        }
    }

    protected bool IsTargetInLineOfSight(Transform target)
    {

        if (Vector3.Distance(target.position, transform.position) <= 0.4f) return true;

        Vector3 raycastOrigin = transform.position + Vector3.up * 0.4f;
        Vector3 directionToTarget = ((target.position + Vector3.up * 0.4f) - raycastOrigin).normalized;

        List<RaycastHit> hits = Physics.RaycastAll(raycastOrigin, directionToTarget, Mathf.Infinity).OrderBy(o => Vector3.Distance(o.point, transform.position)).ToList();

        for (int i = 0; i < hits.Count; i++)
        {
            RaycastHit hit = hits[i];

            if (hit.transform == target)
            {
                return true;
            }

            // Pass through entities
            if (hit.transform.gameObject.layer == 7) return false;

        }

        return false;
    }
    
    public virtual Transform GetTrackingTarget()
    {
        return transform;
    }

    public static Vector3 AddRandomnessToDirection(Vector3 baseDirection, float randomnessFactor, float maxRandomAngle)
    {
        if (randomnessFactor <= 0)
            return baseDirection;

        // Calculate random deviation angle based on randomness factor
        float randomAngle = UnityEngine.Random.Range(-maxRandomAngle, maxRandomAngle) * randomnessFactor;

        // Create a random rotation around a random axis
        Vector3 perpendicular = Vector3.Cross(baseDirection, UnityEngine.Random.onUnitSphere);
        if (perpendicular.magnitude < 0.001f)  // In case they're parallel
            perpendicular = Vector3.Cross(baseDirection, UnityEngine.Random.onUnitSphere);

        perpendicular.Normalize();

        // Create rotation quaternion
        Quaternion randomRotation = Quaternion.AngleAxis(randomAngle, perpendicular);

        // Apply rotation to the base direction
        Vector3 randomizedDirection = randomRotation * baseDirection;

        // Make sure the result is normalized
        return randomizedDirection.normalized;
    }
}

public enum EntityType
{
    Zone,
    Trap,
    Spawner,
    Hero,
    Monster,
    Turret
}

/// <summary>
/// 1-3, determines maximum number of creatures that can engage this one.
/// </summary>
public enum Size
{
    Small,
    Medium,
    Large
}
