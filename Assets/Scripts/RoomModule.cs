using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RoomModule : Module
{

    public override bool persistent => false;
    public GameObject buttonRef;

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

    public override void Delete()
    {
        buttonRef.gameObject.SetActive(true);
        base.Delete();
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
                    if (!tile.module.wasChecked) tile.module.Assemble();
                }
            }
        }
        
    }
}
