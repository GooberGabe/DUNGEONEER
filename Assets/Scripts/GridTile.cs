using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Build.Content;
using UnityEngine;
using UnityEngine.UIElements;

public class GridTile : MonoBehaviour
{
    public Module module = null;
    public GridTile[] adjacents;
    public GridTile[] diagonals;
    public bool validPlacement = true;

    public Vector3 coordinates;

    private void Start()
    {
        GetAdjacentTiles();
    }

    public void SetVisibility(bool visible)
    {
        Renderer renderer = transform.GetChild(0).GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.enabled = visible;
        }
    }

    private void BuildNavMesh()
    {
        GameManager.instance.GetGrid().GetComponent<DungeonBuilder>().BakeNavMesh();
    }

    public void GetAdjacentTiles()
    {
        adjacents = new GridTile[4]
        { GetAdjacentTile(0,1), GetAdjacentTile(1,0), GetAdjacentTile(0,-1), GetAdjacentTile(-1,0) };

        diagonals = new GridTile[4]
        { GetAdjacentTile(-1,1), GetAdjacentTile(1,1), GetAdjacentTile(1,-1), GetAdjacentTile(-1,-1) };
    }

    /// <summary>
    /// Returns a tile adjacent to this one, based on a direction.
    /// </summary>
    /// <param name="x">x, Relative to our position</param>
    /// <param name="z">z, Relative to our position</param>
    /// <returns></returns>
    protected GridTile GetAdjacentTile(int x, int z)
    {
        try
        {
            return GameManager.instance.GetGrid().GetTile((int)(coordinates.x + x), (int)(coordinates.z + z)).GetComponent<GridTile>();
        }
        catch
        {
            return null;
        }
    }

    public bool IsOpen(int dir)
    {
        //Debug.Log(dir + ", " + ((dir + 2) % 4) + ", size:" + adjacents);
        if (adjacents[dir].module == null) return false;
        return adjacents[dir].module.openings[(dir + 2) % 4];
    }

    public Module AddRoomModule(GameObject moduleObject)
    {
        module = moduleObject.GetComponent<Module>();
        moduleObject.transform.parent = transform;
        //module.transform.localPosition = Vector3.zero;
        module.gridTile = this;

        module.Assemble();
        BuildNavMesh();
        return module;
    }

    public Module AddFlexModule()
    {
        GameObject m = Instantiate(GameManager.instance.GetGrid().modulePrefab, transform.position, Quaternion.identity, transform);
        module = m.GetComponent<Module>();
        module.gridTile = this;

        module.GetComponent<Module>().Assemble();
        BuildNavMesh();
        GameManager.instance.tilesPlaced++;
        return module;
    }

    public Module AddStartModule(int archway)
    {
        GameObject m = Instantiate(GameManager.instance.GetGrid().startModulePrefab, transform.position, Quaternion.identity, transform);
        module = m.GetComponent<StartModule>();
        module.gridTile = this;
        ((StartModule)module).archway = 0;
        module.Assemble();
        BuildNavMesh();
        return module;
        
    }

    public Module AddEndModule()
    {
        GameObject m = Instantiate(GameManager.instance.GetGrid().endModulePrefab, transform.position, Quaternion.identity, transform);
        module = m.GetComponent<EndModule>();
        module.gridTile = this;

        module.GetComponent<EndModule>().Assemble();
        BuildNavMesh();
        return module;

    }

    public void SetHighlightColor(Color color)
    {
        transform.GetChild(0).GetComponent<Renderer>().material.color = color;
    }
}
