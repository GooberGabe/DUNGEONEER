using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonDummy : MonoBehaviour
{
    public void Trigger()
    {
        transform.parent.GetComponent<BarrelStack>().Throw();
    }

    public void Fire()
    {
        transform.parent.GetComponent<ArcherPost>().ReleaseArrow();
    }
}
