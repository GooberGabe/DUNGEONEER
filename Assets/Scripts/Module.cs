using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Module : MonoBehaviour
{
    public Transform walls;
    public Transform arches;
    public Transform bases;
    public Transform columns;

    public GridTile gridTile;
    public Hazard hazard;

    protected bool wasChecked = false;
    public GridTile[] adjacents;
    public bool preventPlacement = false;

    /// <summary>
    /// Build this Module based on adjacencies with other Modules.
    /// </summary>
    public void Assemble()
    {
        wasChecked = true;
        adjacents = new GridTile[4]
        { GetAdjacentTile(0,1), GetAdjacentTile(1,0), GetAdjacentTile(0,-1), GetAdjacentTile(-1,0) };

        for (int i = 0; i < 4; i++)
        {
            if (adjacents[i] != null)
            {
                if (adjacents[i].module != null)
                {
                    walls.GetChild(i).gameObject.SetActive(false);

                    if (!adjacents[i].module.wasChecked) adjacents[i].module.Assemble();

                }
            }
                
        }

        GridTile[] diagonals = new GridTile[4]
        { GetAdjacentTile(-1,1), GetAdjacentTile(1,1), GetAdjacentTile(1,-1), GetAdjacentTile(-1,-1) };

        for (int i = 0; i < 4; i++)
        {
            int next = i < 3 ? i + 1 : 0;
            int prev = i > 0 ? i - 1 : 3;

            if (adjacents[i] != null && diagonals[i] != null && adjacents[prev] != null)
            {
                if ((diagonals[i].module != null || adjacents[i].module != null || adjacents[prev].module != null) && !preventPlacement)
                {
                    columns.GetChild(i).gameObject.SetActive(false);
                }
                else
                {
                    columns.GetChild(i).gameObject.SetActive(true);
                }
            }
        }
    }
    /// <summary>
    /// Returns a tile adjacent to this one, based on a direction.
    /// </summary>
    /// <param name="x">x, Relative to our position</param>
    /// <param name="z">z, Relative to our position</param>
    /// <returns></returns>
    GridTile GetAdjacentTile(int x, int z)
    {
        try
        {
            return GameManager.instance.GetGrid().GetTile((int)(gridTile.coordinates.x + x), (int)(gridTile.coordinates.z + z)).GetComponent<GridTile>();
        } 
        catch
        {
            return null;
        }
    }

    GridTile GetAdjacentWall(int x, int z)
    {
        GridTile tile = GameManager.instance.GetGrid().GetTile((int)(gridTile.coordinates.x + x), (int)(gridTile.coordinates.z + z)).GetComponent<GridTile>();
        if (tile == null) return null;
        if (tile.module == null) return null;

        for (int i = 0; i < 4; i++)
        {
            if (tile.module.adjacents[i] == gridTile)
            {
                if (walls.GetChild(i).gameObject.activeSelf) return tile;
                return null;
            }
        }
        Debug.Log("Adjacency error!");
        return null;
    }

    public void LateUpdate()
    {
        wasChecked = false;
    }

}
