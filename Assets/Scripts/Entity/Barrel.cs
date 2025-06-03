using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrel : Projectile
{
    private float counter;
    public float lifespan;
    
    protected override void Update()
    {
        base.Update();
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z - 1);

        counter += Time.deltaTime;
        if (counter >= lifespan) Destroy(gameObject);
    }

    protected override void CollideSurface(GameObject collision)
    {
        base.CollideSurface(collision);

    }

    protected override void CollideEntity(Entity entity)
    {
        DealDamage(entity);
        IgnoreCollisionPhysics(entity);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.isTrigger) Collide(other.gameObject);
    }

}
