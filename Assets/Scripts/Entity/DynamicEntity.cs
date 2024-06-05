using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicEntity : Entity
{
    public override EntityType entityType => throw new System.NotImplementedException();

    public Transform mainDestination;
    public Transform holdPosition;
    public Transform[] firePosition;
    public GameObject projectile;

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

    protected virtual void Face(Transform target)
    {
        transform.LookAt(target);
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
    }

    public virtual void Fire()
    {
        
        if (GetTarget() != null)
        {
            foreach (Transform barrel in firePosition)
            {
                Vector3 raycastOrigin = barrel.position;
                Vector3 directionToTarget = ((GetTarget().position + Vector3.up * 0.5f) - raycastOrigin).normalized;

                GameObject p = Instantiate(projectile, barrel.position, Quaternion.identity);
                p.transform.forward = directionToTarget.normalized;
            }
            
        }
    }
}
