using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.EventSystems.EventTrigger;

public class Projectile : MonoBehaviour
{
    public float damage;
    public float speed;
    public bool autoMove = true;
    public List<EntityType> affectedTypes;
    public GameObject[] afterEffects;
    public StatusEffect[] statusEffects;
    public float effectDuration;
    public Entity source;
    public Transform effectOrigin;
    public bool destroyObject = true;
    public bool passThroughUnaffectedTypes = false;
    public bool nonentityCollisions = true;
    public float knockback = 0;

    private void Start()
    {
        if (effectOrigin == null)
        {
            effectOrigin = transform;
        }
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (autoMove) transform.position += transform.forward * speed * Time.deltaTime;
    }
    
    protected void IgnoreCollisionPhysics(Entity entity)
    {
        foreach (Collider col in GetComponents<Collider>())
        {
            Physics.IgnoreCollision(col, entity.GetComponent<Collider>());
        }
        
    }

    protected void IgnoreCollisionPhysics(GameObject obj)
    {
        foreach (Collider col in GetComponents<Collider>())
        {
            Physics.IgnoreCollision(col, obj.GetComponent<Collider>());
        }
    }

    protected void DealDamage(Entity entity)
    {
        Debug.Log("C");
        entity.TakeDamage(damage);
        if (knockback > 0)
        {
            Vector3 direction = entity.transform.position - transform.position;

            float repulsionStrength = knockback;

            float maxRepulsionDistance = 5f;

            float minDistance = .3f;

            float randomnessFactor = 0.3f;

            float maxRandomAngle = 45f;

            // Normalize the direction vector
            direction.Normalize();

            Debug.Log("A");
            NavMeshAgent agent = entity.transform.GetComponent<NavMeshAgent>();
            if (agent != null)
            {
                // Calculate the distance between agent and projectile
                Vector3 toAgent = agent.transform.position - transform.position;
                float distance = toAgent.magnitude;

                // Check if within repulsion range
                if (distance <= maxRepulsionDistance)
                {
                    Vector3 repulsionDirection = -entity.transform.forward;

                    // Add randomness if desired
                    if (randomnessFactor > 0)
                    {
                        repulsionDirection = Entity.AddRandomnessToDirection(repulsionDirection, randomnessFactor, maxRandomAngle);
                    }

                    // Calculate repulsion strength - stronger when closer
                    float repulsionMagnitude = repulsionStrength * (1 - (distance / maxRepulsionDistance));
                    repulsionMagnitude = Mathf.Clamp(repulsionMagnitude, 0, repulsionStrength);

                    // Calculate the repulsion velocity
                    Vector3 repulsionVelocity = repulsionDirection * repulsionMagnitude;

                    // If agent is already moving, blend the velocities
                    if (agent.hasPath)
                    {
                        // Get the agent's current desired velocity
                        Vector3 currentVelocity = agent.velocity;

                        // Blend current velocity with repulsion
                        // Higher weight to repulsion when closer to projectile
                        float blendFactor = Mathf.Clamp01(1 - (distance / maxRepulsionDistance));
                        Vector3 blendedVelocity = Vector3.Lerp(currentVelocity, repulsionVelocity, blendFactor);

                        // Apply the blended velocity
                        agent.velocity = blendedVelocity;

                        // If very close to the projectile, calculate new destination
                        if (distance < minDistance * 1.5f)
                        {
                            // Calculate a new destination in the repulsion direction
                            Vector3 newDestination = agent.transform.position + repulsionDirection * minDistance * 3f;

                            // Make sure the destination is on the NavMesh
                            NavMeshHit hit;
                            if (NavMesh.SamplePosition(newDestination, out hit, 5f, agent.areaMask))
                            {
                                agent.SetDestination(hit.position);
                            }
                        }
                    }
                    else
                    {
                        // Set a new destination in the repulsion direction
                        Vector3 newDestination = agent.transform.position + repulsionDirection * minDistance * 3f;

                        // Make sure the destination is on the NavMesh
                        NavMeshHit hit;
                        if (NavMesh.SamplePosition(newDestination, out hit, 5f, agent.areaMask))
                        {
                            agent.SetDestination(hit.position);
                        }
                    }
                
                }
            }
        
        }
        for (int i = 0; i < statusEffects.Length; i++)
        {
            ((Creature)entity).SetStatusEffect(statusEffects[i], effectDuration);
        }
    }

    protected virtual void CollideSurface(GameObject collision)
    {
        foreach (GameObject afterEffect in afterEffects)
        {
            Transform fx = Instantiate(afterEffect, effectOrigin.transform.position, Quaternion.identity).transform;
            fx.transform.eulerAngles = Vector3.zero;
        }
        Delete();
    }

    protected virtual void CollideEntity(Entity entity)
    {
        DealDamage(entity);
        foreach (GameObject afterEffect in afterEffects)
        {
            Instantiate(afterEffect, effectOrigin.transform.position, Quaternion.identity);
        }
        Delete();
    }

    public void Delete()
    {
        if (destroyObject)
        {
            Destroy(gameObject);
        }
        else
        {
            GetComponent<Renderer>().enabled = false;
            GetComponent<Collider>().enabled = false;
            if (GetComponent<ParticleSystem>())
            {
                ParticleSystem.MainModule mainSys = GetComponent<ParticleSystem>().main;
                mainSys.loop = false;
            }
            //GetComponent<ParticleSystem>().loop = false;
            //Rigidbody rb = GetComponent<Rigidbody>();
            //rb.velocity = Vector3.zero;
            //rb.isKinematic = true;
            this.enabled = false;
        }
    }

    protected virtual void Collide(GameObject collision)
    {
        Entity entity = collision.GetComponent<Entity>();
        if (collision.layer == 7 || (collision.layer == 9 && source != entity))
        {
            CollideSurface(collision);
        }
        else if (entity == null)
        {
            if (nonentityCollisions)
            {
                CollideSurface(collision);
            }
        }
        else
        {
            if (affectedTypes.Contains(entity.entityType))
            {
                CollideEntity(entity);
            }
            else if (passThroughUnaffectedTypes)
            {
                IgnoreCollisionPhysics(entity);
                return;
            }
            
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Collide(collision.gameObject);
    }

}
