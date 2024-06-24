using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndModule : FlexModule
{
    public GameObject dragonFab;
    public Monster dragonRef;

    protected override void Start()
    {
        base.Start();
        preventPlacement = true;
        dragonRef = (Monster)GameManager.instance.Spawn(dragonFab, transform.position);
        GameManager.instance.GetGrid().GetComponent<DungeonBuilder>().BakeNavMesh();
    }
}
