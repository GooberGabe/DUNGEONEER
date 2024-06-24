using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;

public class CollapsibleMenu : MonoBehaviour
{
    public RectTransform menu;
    [HideInInspector]
    public float size;
    public bool tilePlacement;

    private bool _isExpanded;
    public bool isExpanded
    {
        get { return _isExpanded; }
        set 
        { 
            _isExpanded = value; 
            

        }
    }


    public void Interact()
    {
        bool inverseState = !isExpanded;
        transform.parent.GetComponent<CollapsibleManager>().CollapseAll();
        isExpanded = inverseState;
    }

    private void Update()
    {
        menu.sizeDelta = new Vector2(menu.sizeDelta.x, Mathf.Lerp(menu.sizeDelta.y, _isExpanded ? size : 0, 0.08f));
    }
}
