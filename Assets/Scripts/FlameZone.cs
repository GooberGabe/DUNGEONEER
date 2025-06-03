using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlameZone : EffectZone
{

    protected override void Start()
    {
        base.Start();
        transform.position = GameManager.instance.GetGrid().GetTile(new Vector3(transform.position.x + .5f, 0, transform.position.z + .5f)).transform.position;
    }
    protected override void Update()
    {
        if (counter == 0)
        {
            foreach (ParticleSystem flame in GetComponentsInChildren<ParticleSystem>())
            {
                flame.transform.parent = null;
                flame.loop = false;
            }
        }
        base.Update();

    }
}
