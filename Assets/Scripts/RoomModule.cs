using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomModule : Module
{
    void Start()
    {
        SetOpenings();
    }

    public void Rotate()
    {
        Collider[] wallsList = walls.GetComponentsInChildren<Collider>(true);
        //int x = 3;
        //foreach (Collider wall in wallsList)
        //{
        //    wall.transform.SetSiblingIndex(x);
        //    x++;
        //    if (x > 3) x = 0;
        //    
        //}
        wallsList[3].transform.SetAsFirstSibling();
        
    }

    public void SetOpenings()
    {

        for (int i = 0; i < 4; i++)
        {
            openings[i] = (!walls.GetComponentsInChildren<BoxCollider>(true)[i].gameObject.activeSelf);
        }
    }

    public override void Assemble()
    {
        base.Assemble();
        foreach (GridTile tile in gridTile.adjacents)
        {
            if (tile != null)
            {
                if (tile.module != null)
                {
                    Debug.Log(tile.module.name, tile.module);
                    if (!tile.module.wasChecked) tile.module.Assemble();
                }
            }
        }
        
    }
}
