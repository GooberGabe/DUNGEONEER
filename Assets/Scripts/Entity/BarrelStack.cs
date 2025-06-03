  using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class BarrelStack : Turret
{
    public Transform skeleton;
    public Vector3 spawnOffset;
    protected override void Face(Transform target)
    {
        //base.Face(target);
        skeleton.LookAt(target);
        skeleton.transform.eulerAngles = new Vector3(0, NormalizeAngle(skeleton.transform.eulerAngles.y), 0);
    }

    protected override void Update()
    {
        base.Update();
    }

    private void LateUpdate()
    {
        skeleton.GetComponent<Animator>().SetBool("Fire", false);
    }

    public override void Fire()
    {
         // Freeze cooldown until the animation has finished.
        if (GetTarget() != null)
        {
            cooldownCounter = -99;
            skeleton.GetComponent<Animator>().SetBool("Fire",true);

        }
    }

    public void Throw()
    {
        cooldownCounter = cooldownTime;
        Vector3 directionToTarget = skeleton.forward;

        GameObject p = Instantiate(projectile, firePosition[0].position + spawnOffset, Quaternion.identity);
        p.GetComponent<Projectile>().source = this;
        p.GetComponent<Rigidbody>().linearVelocity = skeleton.transform.forward * 4.5f;
        p.transform.forward = -skeleton.transform.right;
        Debug.DrawRay(skeleton.position + Vector3.up, directionToTarget, Color.red);
        //p.transform.forward = new Vector3(0, directionToTarget.normalized.y, 0);
    }
}
