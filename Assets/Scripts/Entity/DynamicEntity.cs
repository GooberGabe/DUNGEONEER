using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DynamicEntity : Entity
{
    public override EntityType entityType => throw new System.NotImplementedException();

    public Transform mainDestination;
    public bool detectInvisibility = false;
    public bool targetClose = true;
    public Transform holdPosition;
    public Transform[] firePosition;
    protected Transform trackingPosition;
    public GameObject projectile;
    protected float cooldownCounter;
    public float cooldownTime;

    public static float NormalizeAngle(float degrees)
    {
        // Add the input degrees and take modulo 360 to get a value between -359 and 359
        float normalizedAngle = (degrees + 360) % 360;

        // If the result is negative, add 360 to bring it into the positive range
        if (normalizedAngle < 0)
            normalizedAngle += 360;

        // Now we have a normalized angle between 0 and 359
        // You can further map it to the desired set of values (90, 180, 270, or 360)
        if (normalizedAngle <= 45)
            return 0;
        else if (normalizedAngle <= 135)
            return 90;
        else if (normalizedAngle <= 225)
            return 180;
        else if (normalizedAngle <= 315)
            return 270;
        else
            return 360;
    }
    protected virtual Transform GetTarget()
    {
        return mainDestination;
    }

    protected virtual List<Entity> GetValidTargets()
    {
        if (detectInvisibility) return entitiesInRange;
        else return entitiesInRange.Where(x => !x.invisible).ToList();
    }

    protected virtual void Face(Transform target)
    {
        transform.LookAt(target);
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
    }

    protected override void Update()
    {
        base.Update();
        if (cooldownCounter > 0)
        {
            cooldownCounter -= Time.deltaTime;
        }
    }

    protected override void Start()
    {
        base.Start();
        if (trackingPosition == null)
        {
            trackingPosition = transform;
        }
    }

    public override float GetDamageRate()
    {
        return cooldownTime;
    }

    public virtual void Fire()
    {
        SpawnProjectile();
    }

    protected void SpawnProjectile()
    {
        if (GetTarget() != null)
        {
            foreach (Transform barrel in firePosition)
            {
                Vector3 raycastOrigin = barrel.position;
                Vector3 directionToTarget = ((GetTarget().position + Vector3.up * 0.5f) - raycastOrigin).normalized;

                GameObject p = Instantiate(projectile, barrel.position, Quaternion.identity);
                p.transform.forward = directionToTarget.normalized;
                Projectile proj = p.GetComponent<Projectile>();
                if (proj != null) proj.source = this;
            }

        }
    }
}
