using UnityEngine;

public class ArcherPost : AnimatedTurret
{
    protected override void Face(Transform target)
    {
        //base.Face(target);
        animatedElement.LookAt(target);
        animatedElement.transform.eulerAngles = new Vector3(0, animatedElement.transform.eulerAngles.y, 0);
    }

    protected override void Update()
    {
        base.Update();
    }

    private void LateUpdate()
    {
        animatedElement.GetComponent<Animator>().SetBool("Fire", false);
    }

    public void ReleaseArrow()
    {
        cooldownCounter = cooldownTime;
        SpawnProjectile();
    }
}
