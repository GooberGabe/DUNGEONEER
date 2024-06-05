using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrel : Projectile
{
    private int counter;
    public int lifespan;
    
    protected override void Update()
    {
        base.Update();
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z - 1);

        counter++;
        if (counter > lifespan) Destroy(gameObject);
    }

    protected override void CollideSurface(GameObject collision)
    {
        if (collision.gameObject.layer == 7) 
        {
            base.CollideSurface(collision);
        }

    }

    protected override void CollideEntity(Entity entity)
    {
        DealDamage(entity);
        Physics.IgnoreCollision(GetComponent<Collider>(), entity.GetComponent<Collider>());
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.isTrigger) Collide(other.gameObject);
    }

}
