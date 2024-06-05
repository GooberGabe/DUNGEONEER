using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float damage;
    public float speed;
    public bool autoMove = true;
    public List<EntityType> affectedTypes;
    public GameObject afterEffect;
    public Transform effectOrigin;

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

    protected virtual void CollideEntity(Entity entity)
    {
        DealDamage(entity);
        Instantiate(afterEffect, effectOrigin.transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    protected void DealDamage(Entity entity)
    {
        entity.TakeDamage(damage);
    }

    protected virtual void CollideSurface(GameObject collision)
    {
        Transform fx = Instantiate(afterEffect, effectOrigin.transform.position, Quaternion.identity).transform;
        fx.transform.eulerAngles = Vector3.zero;
        Destroy(gameObject);
    }

    protected virtual void Collide(GameObject collision)
    {
        Entity entity = collision.GetComponent<Entity>();
        if (entity != null)
        {
            if (affectedTypes.Contains(entity.entityType))
            {
                CollideEntity(entity);
            }
        }
        else
        {
            CollideSurface(collision);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Collide(collision.gameObject);
    }

}
