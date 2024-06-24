using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Module : MonoBehaviour
{
    public Transform walls;

    public GridTile gridTile;
    public Entity hazard;

    [HideInInspector]
    public bool wasChecked = false;

    public bool[] openings;
    public bool preventPlacement = false;

    public virtual void Assemble()
    {
        wasChecked = true;
        gridTile.GetAdjacentTiles();
    }

    public void Delete()
    {
        if (hazard != null)
        {
            Destroy(hazard.gameObject);
            hazard = null;
        }
        gridTile.module = null;
        foreach (GridTile tile in gridTile.adjacents)
        {
            if (tile != null)
            {
                if (tile.module != null)
                {
                    tile.module.Assemble();
                }
            }
        }
        GameManager.instance.tilesPlaced--;
        gridTile = null;
        Destroy(gameObject);
    }


    protected GridTile GetAdjacentWall(int x, int z)
    {
        GridTile tile = GameManager.instance.GetGrid().GetTile((int)(gridTile.coordinates.x + x), (int)(gridTile.coordinates.z + z)).GetComponent<GridTile>();
        if (tile == null) return null;
        if (tile.module == null) return null;

        for (int i = 0; i < 4; i++)
        {
            if (tile.adjacents[i] == gridTile)
            {
                if (walls.GetChild(i).gameObject.activeSelf) return tile;
                return null;
            }
        }
        Debug.Log("Adjacency error!");
        return null;
    }

    public bool IsOpen(int dir)
    {
        return openings[dir] && gridTile.IsOpen(dir);
    }

    public virtual void LateUpdate()
    {
        wasChecked = false;
    }

}
