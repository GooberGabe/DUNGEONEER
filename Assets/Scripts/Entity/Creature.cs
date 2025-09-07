using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;

public class Creature : DynamicEntity
{
    public int strength;
    public int speed;
    public Size size;
    public Behavior behavior;
    private float[] statusEffectDurations;
    private GameObject[] effectDisplays;

    public Transform subDestination;

    private NavMeshAgent navMeshAgent;
    private Animator animator;
    private bool isNavigating;
    public bool isMoving = false;

    public List<Creature> foes;

    public override EntityType entityType => throw new System.NotImplementedException();

    protected override void Start()
    {
        base.Start();
        statusEffectDurations = new float[3];
        effectDisplays = new GameObject[2];

        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        GameObject h = (GameObject)Instantiate(Resources.Load("UI/HealthDisplay"), transform.position + Vector3.up, Quaternion.identity);
        h.GetComponent<HealthDisplay>().subject = this;

        GameObject b = (GameObject)Instantiate(Resources.Load("Particles/StatusBurning"), transform.position, Quaternion.identity, transform);
        b.transform.localScale = Vector3.one;
        b.SetActive(false);
        effectDisplays[0] = b;

        GameObject s = (GameObject)Instantiate(Resources.Load("Particles/StatusShock"), transform.position, Quaternion.identity, transform);
        s.transform.localScale = Vector3.one;
        s.SetActive(false);
        effectDisplays[1] = s;

        GameObject t = new GameObject();
        t.transform.parent = transform;
        t.name = "Tracking Position";
        trackingPosition = t.transform;
        SetTrackingPosition(0);
    }

    protected override void Update()
    {
        base.Update();
        float effectiveSpeed = speed;
        animator.SetBool("Shock", false);

        for (int i = 0; i < statusEffectDurations.Length; i++)
        {
            bool statusOn = false;
            if (statusEffectDurations[i] > 0)
            {
                statusEffectDurations[i] -= Time.deltaTime;
                statusOn = true;
                if (i == (int)StatusEffect.Burning)
                {
                    TakeDamage(Time.deltaTime * 3, true);
                }
                if (i == (int)StatusEffect.Shocked)
                {
                    effectiveSpeed = 0f;
                    animator.SetBool("Shock", true);
                }
                if (i == (int)StatusEffect.Slowed)
                {
                    effectiveSpeed *= .75f;
                }
            }
            if (i < effectDisplays.Length) effectDisplays[i].SetActive(statusOn);
        }

        if (isNavigating)
        {
            animator.SetFloat("Speed", Mathf.Max((float)(speed * speed) / 100f, 0.1f));
            SetTrackingPosition(isMoving ? effectiveSpeed : 0);
        }
        else
        {
            animator.SetFloat("Speed", 0);
            SetTrackingPosition(0);
        }

        navMeshAgent.speed = isMoving ? effectiveSpeed / 7f : 0;

        List<Creature> entitiesCopy = new List<Creature>(foes);
        foreach (Creature entity in entitiesCopy)
        {
            if (entity == null || !entity.enabled) foes.Remove(entity);

        }

    }

    public bool HasStatusEffect(StatusEffect status)
    {
        return (statusEffectDurations[(int)status] > 0);
    }

    public override float GetDamage()
    {
        return strength;
    }

    public override string TextDisplay()
    {
        return maxHitPoints.ToString() + " HP" +
               "\nDamage: " + GetDamage() +
               "\nSpeed:  " + speed.ToString() +
               description != "" ? "\n" + description : "";
    }

    public override string StatusDisplay()
    {
        return hitPoints.ToString() + "/" + maxHitPoints.ToString() + " HP";
    }

    public float GetStatusEffect(StatusEffect effect)
    {
        return statusEffectDurations[(int)effect];
    }

    public virtual void SetStatusEffect(StatusEffect effect, float duration)
    {
        statusEffectDurations[(int)effect] = duration;
    }

    protected override Transform GetTarget()
    {
        if (subDestination != null)
        {
            return subDestination;
        }
        if (mainDestination != null)
        {
            return mainDestination;
            
        }
        return null;
    }

    protected virtual void NavigationBehavior()
    {
        Transform target = GetTarget();

        if (target != null) SetDestination(target.position);
        if (navMeshAgent.enabled)
        {
            if (target != null && navMeshAgent.isOnNavMesh && navMeshAgent.hasPath)
            {
                //navMeshAgent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
                bool visible = true;
                Entity entity = target.GetComponent<Entity>();
                if (entity != null)
                {
                    if (entity.entityType == EntityType.Monster || entity.entityType == EntityType.Hero) 
                    {
                        visible = IsTargetInLineOfSight(target) || Vector3.Distance(transform.position, target.transform.position) < 0.1f;
                        // We only care about line of sight if the target is a creature
                    }
                }
                if (navMeshAgent.remainingDistance > navMeshAgent.stoppingDistance)
                {
                    isNavigating = true;
                }
                else if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance && visible && Vector3.Distance(target.position, transform.position) <= navMeshAgent.stoppingDistance)
                {
                    Arrive(target);
                    isNavigating = false;
                }
                else
                {
                    Debug.LogWarning("Agent has stopped but did not reach its destination.");
                }

            }
            else
            {
                isNavigating = false;
            }
        }
        else
        {
            isNavigating = false;
            if (target != null)
            {
                if (IsTargetInLineOfSight(target) && Vector3.Distance(target.position, transform.position) <= navMeshAgent.stoppingDistance) Arrive(target);
            }
        }
            

    }

    protected virtual void Arrive(Transform target)
    {
        if (isNavigating) // First frame only
        {

        }
        Face(target);   
    }

    protected virtual void Attack(Entity target)
    {
        if (target.entityType == EntityType.Monster || target.entityType == EntityType.Hero)
        {
            if (!foes.Contains((Creature)target)) foes.Add((Creature)target);
        }
        animator.SetBool("Attacking", true);
    }

    public virtual void Hit()
    {
        if (GetTarget() != null)
        {
            Entity target = GetTarget().GetComponent<Entity>();
            if (target != null)
            {
                target.TakeDamage(strength);
                if (target.entityType == EntityType.Hero || target.entityType == EntityType.Monster)
                {
                    ((Creature)target).OnAttacked(this);
                }
            }
        }
    }

    public virtual void OnAttacked(Creature assailant)
    {
        if (!foes.Contains(assailant)) foes.Add(assailant);
        subDestination = assailant.transform;
    }

    protected override void Die()
    {
        //base.Die();
        animator.SetBool("Alive", false);
        animator.SetBool("Shock", false);
        for (int i = 0; i < statusEffectDurations.Length; i++) 
        {
            statusEffectDurations[i] = 0;
        }

        if (isAlive)
        {
            foreach (Collider col in GetComponents<Collider>())
            {
                col.enabled = false;
            }
            navMeshAgent.isStopped = true;
            isAlive = false;
            this.enabled = false;
        }
    }

    public override void TakeDamage(float amount, bool hidden = false)
    {
        if (isAlive)
        {
            base.TakeDamage(amount);
            if (!hidden) Instantiate((GameObject)Resources.Load("Particles/BloodSplat"), transform.position + (transform.up * 0.5f), Quaternion.identity);
        }
            
    }

    void SetDestination(Vector3 destination)
    {
        if (navMeshAgent.isOnNavMesh)
        {
            navMeshAgent.SetDestination(destination);
            isNavigating = true;
        }
        else
        {
            Debug.LogWarning("Creature is not on the NavMesh!");
        }
    }

    public void LateUpdate()
    {
        animator.SetBool("Attacking", false);
    }


    public override Transform GetTrackingTarget()
    {
        return trackingPosition;
    }
    
    private void SetTrackingPosition(float _speed)
    {
        trackingPosition.position = transform.position + (transform.forward * (_speed * 0.03f));
    }

}

public enum Behavior
{
    Seek,
    Fight,
    Flee
}
