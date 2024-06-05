using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementPreview : MonoBehaviour
{
    public GameObject placement;
    public bool snapToGrid;

    public int cost { get { return placement.GetComponent<Entity>().cost; } }

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

    private void Start()
    {
        transform.eulerAngles = new Vector3(0, 180, 0);
    }

    public void Place(Vector3 pos)
    {
        if (_isValid)
        {
            if (GameManager.instance.TryBuy(cost))
            {
                if (placement.GetComponent<Entity>().entityType == EntityType.Monster) GameManager.instance.Spawn(placement, pos);
                else Instantiate(placement, pos, Quaternion.identity);
                Destroy(gameObject);
            }
        }
    }
}
