using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class FlexModule : Module
{
    public Transform columns;
    public Transform arches;
    public Transform bases;
    public Transform overlay;

    private Color initialColor;
    public Color hoverColor;
    public bool hover;

    public override bool persistent => false;

    protected virtual void Start()
    {
        if (overlay != null) initialColor = overlay.GetComponent<Renderer>().material.color;
        //openings = new bool[4] { true, true, true, true };
    }

    private void Update()
    {

        if (overlay != null)
        {
            overlay.gameObject.SetActive(GameManager.instance.tilePlacement);
            if (hover) overlay.GetComponent<Renderer>().material.color = hoverColor;
            else overlay.GetComponent<Renderer>().material.color = initialColor;

        }
    }

    public override void Delete()
    {
        GameManager.instance.tilesPlaced--;
        base.Delete();
        GameManager.instance.gold += GameManager.instance.GetTilePrice();
    }

    /// <summary>
    /// Build this Module based on adjacencies with other Modules.
    /// </summary>
    public override void Assemble()
    {
        base.Assemble();

        for (int i = 0; i < 4; i++)
        {
            GridTile adj = gridTile.adjacents[i];
            if (adj != null) // Make sure we haven't checked an out-of-bounds position
            {
                if (adj.module != null) // If there's a module here
                {
                    
                    if (IsOpen(i))  // If this side and the adjacent side are both open
                    {
                        walls.GetChild(i).gameObject.SetActive(false);

                        if (!gridTile.adjacents[i].module.wasChecked) adj.module.Assemble();
                    }
                        

                }
            }

        }

        for (int i = 0; i < 4; i++)
        {
            int next = i < 3 ? i + 1 : 0;
            int prev = i > 0 ? i - 1 : 3;

            if (gridTile.adjacents[i] != null && gridTile.diagonals[i] != null && gridTile.adjacents[prev] != null)
            {
                if ((gridTile.diagonals[i].module != null || gridTile.adjacents[i].module != null || gridTile.adjacents[prev].module != null) && !preventPlacement)
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

    public override void LateUpdate()
    {
        base.LateUpdate();
        hover = false;

    }
}
