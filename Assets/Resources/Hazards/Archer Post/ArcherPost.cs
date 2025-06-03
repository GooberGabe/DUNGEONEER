using UnityEngine;

public class ArcherPost : Turret
{
    public Transform skeleton;
    public Vector3 spawnOffset;
    protected override void Face(Transform target)
    {
        //base.Face(target);
        skeleton.LookAt(target);
        skeleton.transform.eulerAngles = new Vector3(0, skeleton.transform.eulerAngles.y, 0);
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
            skeleton.GetComponent<Animator>().SetBool("Fire", true);

        }
    }

    public void ReleaseArrow()
    {
        cooldownCounter = cooldownTime;
        SpawnProjectile();
    }
}
