using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EntityInterface : MonoBehaviour
{
    public Entity target;
    public GameObject spotLight;
    public GameObject rangeDisplay;

    private bool _showRange;
    private bool lightOff;

    private void Start()
    {
        target = transform.parent.GetComponent<Entity>();
        lightOff = true;
    }

    private void Update()
    {
        bool preview = transform.parent.GetComponent<PlacementPreview>() != null;
        ShowRange(preview || InterfaceManager.instance.selectedEntity == target);
        if (lightOff || preview)
        {
            ShowSpotlight(false);
        }
        lightOff = true;
    }

    private void ShowRange(bool show)
    {
        if (_showRange != show)
        {
            _showRange = show;
            rangeDisplay.GetComponent<Renderer>().enabled = show;
            if (show)
            {
                if (target.GetComponent<CapsuleCollider>() != null)
                {
                    float r = target.transform.GetComponent<CapsuleCollider>().radius * 2;
                    rangeDisplay.transform.localScale = new Vector3(r, rangeDisplay.transform.localScale.y, r);
                }
                if (target.GetComponent<BoxCollider>() != null)
                {
                    Debug.Log(GetComponent<Collider>().GetType());
                    BoxCollider col = target.transform.GetComponent<BoxCollider>();
                    rangeDisplay.transform.localScale = new Vector3(col.size.x, col.size.y, col.size.z);
                }
                    
            }
            else
            {
                rangeDisplay.transform.localScale = new Vector3(0, rangeDisplay.transform.localScale.y, 0);
            }
        }
    }

    private void ShowSpotlight(bool show)
    {
        spotLight.GetComponent<Light>().enabled = show;
        lightOff = false;
    }

    public void OnHover()
    {
        ShowSpotlight(true);
    }

    public void OnClick()
    {
        InterfaceManager.instance.selectedEntity = target;
        if (target != null)
        {
            InterfaceManager.instance.DisplayUpgradeTree(target);
        }
    }

}
