using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndModule : Module
{
    public GameObject dragonFab;
    public Monster dragonRef;

    private void Start()
    {
        preventPlacement = true;
        dragonRef = (Monster)GameManager.instance.Spawn(dragonFab, transform.position);
        GameManager.instance.GetGrid().GetComponent<DungeonBuilder>().BakeNavMesh();
    }
}
