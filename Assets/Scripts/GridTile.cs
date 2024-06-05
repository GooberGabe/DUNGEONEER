using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GridTile : MonoBehaviour
{
    public Module module = null;
    public bool validPlacement = true;

    public Vector3 coordinates;

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

    public Module AddModule()
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

        module.GetComponent<StartModule>().Assemble();
        Endpoint(archway);
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

    void Endpoint(int archway)
    {
        for (int i = 0; i < 4; i++)
        {
            if (i != archway)
            {
                module.adjacents[i].validPlacement = false;
            }
            else
            {
                module.walls.GetChild(i).gameObject.SetActive(false);
                module.arches.GetChild(i).gameObject.SetActive(true);
            }
        }
    }

    public void SetHighlightColor(Color color)
    {
        transform.GetChild(0).GetComponent<Renderer>().material.color = color;
    }
}
