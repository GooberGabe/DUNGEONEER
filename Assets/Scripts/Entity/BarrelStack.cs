  using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class BarrelStack : AnimatedTurret
{

    protected override void Face(Transform target)
    {
        //base.Face(target);
        animatedElement.LookAt(target);
        animatedElement.transform.eulerAngles = new Vector3(0, NormalizeAngle(animatedElement.transform.eulerAngles.y), 0);
    }

    protected override void Update()
    {
        base.Update();
    }

    public override float GetRange()
    {
        return rangeColliders[0].bounds.size.z;
    }

    public void Throw()
    {
        cooldownCounter = cooldownTime;
        Vector3 directionToTarget = animatedElement.forward;

        GameObject p = Instantiate(projectile, firePosition[0].position + spawnOffset, Quaternion.identity);
        p.GetComponent<Projectile>().source = this;
        p.GetComponent<Rigidbody>().linearVelocity = animatedElement.transform.forward * 4.5f;
        p.transform.forward = -animatedElement.transform.right;
        Debug.DrawRay(animatedElement.position + Vector3.up, directionToTarget, Color.red);
        //p.transform.forward = new Vector3(0, directionToTarget.normalized.y, 0);
    }
}
