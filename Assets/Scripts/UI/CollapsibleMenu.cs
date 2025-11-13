using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollapsibleMenu : MonoBehaviour
{
    public RectTransform menu;
    public RectTransform scrollArea;
    public RectTransform content;
    [HideInInspector]
    public float size;
    public int maxRows;
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
        Vector2 newSize = new Vector2(menu.sizeDelta.x, Mathf.Lerp(menu.sizeDelta.y, _isExpanded ? size : 0, 0.08f));
        menu.sizeDelta = newSize;
        scrollArea.sizeDelta = newSize;
        scrollArea.gameObject.SetActive(menu.sizeDelta.y > 10);
    }
}
