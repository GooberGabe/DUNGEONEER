using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomPreview : PlacementPreview
{
    private int rotations = 0;
    protected override int GetCost()
    {
        return 0;
    }

    protected override void _Place(Vector3 pos)
    {
        GameObject room = Instantiate(placement, pos, Quaternion.identity);
        room.transform.eulerAngles = transform.eulerAngles;
        room.transform.localScale = transform.localScale;

        foreach (RoomModule child in room.GetComponentsInChildren<RoomModule>())
        {
            for (int i = 0; i < rotations; i++) child.Rotate();
            child.SetOpenings();
            child.buttonRef = buttonRef;
            
        }

        buttonRef.SetActive(false);

    }

    protected override void Start()
    {

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R)) // Rotate
        {
            transform.eulerAngles += new Vector3(0, 90, 0);
            foreach(RoomModule child in GetComponentsInChildren<RoomModule>(true))
            {
                child.Rotate();
                child.SetOpenings();
            }
            rotations++;
        }
        if (Input.GetKeyDown(KeyCode.M)) // Mirror
        {
            transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
        }
    }

    public override bool CheckValidity(Module module)
    {
        MapGrid grid = GameManager.instance.GetGrid();
        bool connected = false;

        foreach (Module child in GetComponentsInChildren<Module>(true))
        {
            GameObject tileObject = grid.GetTile(new Vector3(child.transform.position.x, 0, child.transform.position.z));
            if (tileObject != null)
            {
                GridTile tile = tileObject.GetComponent<GridTile>();
                if (tile.module == null)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        if (tile.adjacents[i] != null) 
                        {
                            ((RoomModule)child).SetOpenings();
                            if (tile.IsOpen(i) && child.openings[i]) connected = true;
                            
                        } 
                    }
                }
                else
                {
                    return false;
                }
            } 
            else
            {
                return false;
            }
        }
        return connected;
        
    }
}
