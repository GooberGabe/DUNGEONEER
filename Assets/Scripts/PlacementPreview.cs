using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementPreview : MonoBehaviour
{
    public GameObject placement;
    public bool snapToGrid;
    public GameObject buttonRef;
    public Vector3 offset;

    private GridTile gridTile;

    public int cost { get { return GetCost(); } }

    private bool _isValid;
    public bool isValid
    {
        get { return _isValid; }
        set
        {
            _isValid = value;
            foreach (Renderer r in GetComponentsInChildren<Renderer>())
            {
                for (int i = 0; i < r.materials.Length; i++)
                {
                    r.materials[i].color = value ? ((Material)Resources.Load("Materials/Preview")).color : new Color(1, 0, 0, 0.4f);
                }
            }
            
        }
    }

    public void SetGridTile(GridTile tile)
    {
        gridTile = tile;
    }

    protected virtual void Start()
    {
        transform.eulerAngles = new Vector3(0, 180, 0);
    }

    protected virtual int GetCost()
    {
        Entity entity = placement.GetComponent<Entity>();
        if (entity) return entity.cost;
        else return 0;
    }

    public void Place(Vector3 pos)
    {
        if (_isValid)
        {
            if (GameManager.instance.TryBuy(cost))
            {
                _Place(pos);
                Destroy(gameObject);
            }
        }
    }

    protected virtual void _Place(Vector3 pos)
    {
        if (!placement.GetComponent<Entity>())
        {
            Instantiate(placement, pos, placement.transform.rotation);
        }
        else if (placement.GetComponent<Entity>().entityType == EntityType.Monster)
        {
            GameManager.instance.Spawn(placement, pos);
        }
        else
        {
            GameObject tower = Instantiate(placement, pos, Quaternion.identity);
            gridTile.module.hazard = tower.GetComponent<Entity>();
        }
    }

    public virtual bool CheckValidity(Module module)
    {
        bool v = true;
        if (module != null)
        {
            if (module.hazard != null || module.preventPlacement) v = false;
        }
        else
        {
            v = false;
        }
        return v;
    }
}
