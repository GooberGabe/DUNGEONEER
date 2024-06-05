using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEngine.GraphicsBuffer;

public class MapGrid : MonoBehaviour
{
    public GameObject highlightPrefab;
    public GameObject modulePrefab;
    public GameObject startModulePrefab;
    public GameObject endModulePrefab;
    public int gridSize = 10; // Set the size of the grid
    private GameObject[,] tiles; // 2D array to store tile objects

    private GameObject selectedTile;
    public StartModule startModule;
    public EndModule endModule;

    private Color highlightColor = Color.white;

    void Start()
    {
        CreateGrid();
        HideTiles();
        SizeOffset();
        SetEndpoints();
    }

    void SizeOffset()
    {
        transform.position = new Vector3(-gridSize/2, 0, -gridSize/2);
    }

    void SetEndpoints()
    {
        startModule = (StartModule)GetTile(6, 6).GetComponent<GridTile>().AddStartModule(0);
        endModule = (EndModule)GetTile(12, 12).GetComponent<GridTile>().AddEndModule();
    }

    void CreateGrid()
    {
        tiles = new GameObject[gridSize, gridSize];

        for (int x = 0; x < gridSize; x++)
        {
            for (int z = 0; z < gridSize; z++)
            {
                Vector3 position = new Vector3(x, 0, z); // Calculate the position for each tile
                GameObject tile = Instantiate(highlightPrefab, position, Quaternion.identity, transform); // Instantiate the tile at the calculated position
                tiles[x, z] = tile;
                tile.SetActive(true);
                tile.GetComponent<GridTile>().coordinates = position;
                tile.GetComponent<GridTile>().SetVisibility(false); // Set the tile to inactive (hidden) initially
            }
        }
    }

    void HideTiles()
    {
        foreach (GameObject tile in tiles)
        {
            if (tile != null)
                tile.GetComponent<GridTile>().SetVisibility(false);
        }
    }

    /// <summary>
    /// Get a tile based on spatial coordinates.
    /// </summary>
    /// <param name="position">Real-world coordinates</param>
    /// <returns></returns>
    public GameObject GetTile(Vector3 position)
    {
        try
        {
            return tiles[(int)(position.x + (gridSize / 2)), (int)(position.z + (gridSize / 2))];
        }
        catch 
        {
            return null;
        }
    }

    /// <summary>
    /// Get a tile based on grid coordinates
    /// </summary>
    /// <param name="x"></param>
    /// <param name="z"></param>
    /// <returns></returns>
    public GameObject GetTile(int x, int z)
    {
        if (x > 0 && x < gridSize && z > 0 && z < gridSize)
        {
            return tiles[x, z];
        }
        else
        {
            return null;
        }
    }

    public List<Module> GetAllModules()
    {
        List<Module> modules = new List<Module>();
        foreach (GameObject tile in tiles)
        {
            if (tile != null)
                if (tile.GetComponent<GridTile>().module != null) modules.Add(tile.GetComponent<GridTile>().module);
        }
        return modules;
    }

    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        HideTiles();

        if (GameManager.instance.isAlive) {
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                if (Input.GetMouseButtonDown(0))
                {
                    InterfaceManager.instance.selectedEntity = null;
                    InterfaceManager.instance.selectedUpgrade = null;
                }

                // Raycast from the cursor to layer 3 only
                if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << 3))
                {
                    int x = Mathf.RoundToInt(hit.point.x);
                    int z = Mathf.RoundToInt(hit.point.z);
                    GameObject hoverTile = GetTile(new Vector3(x, 0, z));

                    PlacementPreview prev = InterfaceManager.instance.currentPreview;
                    if (hoverTile != null)
                    {
                        if (prev != null)
                        {
                            prev.isValid = true;
                            Module module = hoverTile.GetComponent<GridTile>().module;

                            if (module != null)
                            {
                                if (module.hazard != null || module.preventPlacement) prev.isValid = false;
                            }
                            else
                            {
                                prev.isValid = false;
                            }

                            if (prev.snapToGrid)
                            {
                                // Grid-aligned hazards
                                prev.transform.position = hoverTile.transform.position + (Vector3.up * 0.03f);
                            }
                            else
                            {
                                // Free placement
                                prev.transform.position = hit.point;
                                selectedTile = null;
                            }

                            if (Input.GetMouseButtonDown(0))
                            {
                                prev.Place(prev.transform.position);
                            }
                        }
                        else
                        {
                            // Tile placement
                            selectedTile = hoverTile;

                            if (selectedTile.layer == 3 && GameManager.instance.tilePlacement)
                            {
                                GridTile selectedTileSquare = selectedTile.GetComponent<GridTile>();
                                if (selectedTileSquare.module == null) Highlight(selectedTileSquare);

                                if (Input.GetMouseButton(0) && selectedTileSquare.module == null && selectedTileSquare.validPlacement && !startModule.playRound)
                                {

                                    int price = GameManager.instance.GetTilePrice();
                                    
                                    if (GameManager.instance.TryBuy(price)) selectedTileSquare.AddModule();
                                    
                                }
                            }
                            else
                            {
                                GameManager.instance.tilePlacement = false;
                            }
                        }
                    }
                    else
                    {
                        selectedTile = null;
                    }
                }

                List<RaycastHit> hits = Physics.RaycastAll(ray, Mathf.Infinity).ToList();
                
                for (int i = 0; i < hits.Count; i++)
                {
                    RaycastHit _hit = hits[i];

                    float size = 0;
                    if (_hit.collider.GetType() == typeof(CapsuleCollider)) size = ((CapsuleCollider)_hit.collider).radius;
                    if (_hit.collider.GetType() == typeof(BoxCollider)) size = Mathf.Max(((BoxCollider)_hit.collider).size.x, ((BoxCollider)_hit.collider).size.z);
                    bool colliderIsValid = _hit.collider.isTrigger ? size <= 1 : true;

                    if (_hit.transform.GetComponent<Entity>() != null && colliderIsValid)
                    {
                        EntityInterface display = _hit.transform.GetComponentInChildren<EntityInterface>();
                        if (display != null)
                        {
                            display.OnHover();
                            if (Input.GetMouseButton(0))
                            {
                                display.OnClick();
                            }
                        }

                    }

                }
                
            }
            else
            {
                if (Input.GetMouseButton(0))
                {
                    if (InterfaceManager.instance.currentPreview != null)
                    {
                        Destroy(InterfaceManager.instance.currentPreview.gameObject);
                    }
                    GameManager.instance.tilePlacement = false;
                }
            }
        }
    }

    void Highlight(GridTile selectedTileSquare)
    {
        selectedTileSquare.SetVisibility(true); // Unhide the tile at the hovered position

        if (!selectedTileSquare.validPlacement) selectedTileSquare.SetHighlightColor(Color.red);
        else selectedTileSquare.SetHighlightColor(Color.white);
    }
}
