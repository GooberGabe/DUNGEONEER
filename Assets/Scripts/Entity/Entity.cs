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

    public float hitPoints = 1;
    public float maxHitPoints = 1;
    public string entityName;
    public string upgradeName;
    public int cost;
    abstract public EntityType entityType { get; }
    public EntityType[] detectedTypes;
    public UpgradeTree upgradeTree;


    protected List<Entity> entitiesInRange = new List<Entity>();

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
            if (entity == null || !entity.enabled) entitiesInRange.Remove(entity);
            
        }
    }

    public virtual string TextDisplay()
    {
        return "";
    }

    public float GetColliderSize()
    {
        Collider col = GetComponent<Collider>();
        if (col.GetType() == typeof(CapsuleCollider)) return ((CapsuleCollider)col).radius * 2;
        if (col.GetType() == typeof(BoxCollider)) return Mathf.Max(((BoxCollider)col).size.x, ((BoxCollider)col).size.z);
        return 0;
    }

    public void Poof(float delay = 0.1f)
    {
        GameObject g = Instantiate((GameObject)Resources.Load("Particles/Poof"), transform.position + (transform.up * 0.5f), Quaternion.identity);
        Destroy(gameObject, delay);
    }

    protected virtual void Die()
    {
        Destroy(gameObject); // If alternative behavior is desired, override behavior without calling base
    }

    public virtual void Upgrade()
    {
        Poof(0);
    }

    public virtual void TakeDamage(float amount)
    {
        hitPoints -= amount;
        if (hitPoints <= 0)
        {
            hitPoints = 0;
            Die();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Entity entity = other.gameObject.GetComponent<Entity>();
        if (entity != null)
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
        if (entity != null)
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
