using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class CollapsibleManager : MonoBehaviour
{
    public CollapsibleMenu[] menus;

    private void Start()
    {
        menus = GetComponentsInChildren<CollapsibleMenu>();
    }

    float ComputeVerticalSize(RectTransform rect, float tiers)
    {
        return (tiers * (rect.sizeDelta.y + 2)) + 10;
    }

    private void Update()
    {
        float y = 0;
        float padding = 5;
        float interval = menus[0].GetComponent<RectTransform>().rect.height + padding;
        for (int i = 0; i < menus.Length; i++) 
        {
            RectTransform rt = menus[i].GetComponent<RectTransform>();
            if (menus[i].tilePlacement) 
            {
                menus[i].size = 51;
            }
            else if (menus[i].menu.transform.GetChild(0).childCount > 0)
            {
                PurchaseButton[] children = menus[i].menu.transform.GetChild(0).GetComponentsInChildren<PurchaseButton>();
                float levels = Mathf.Floor((children.Length + 1) / 2);
                float levelCap = Mathf.Min(levels, menus[i].maxRows);
                menus[i].size = ComputeVerticalSize(children[0].GetComponent<RectTransform>(), levelCap);
                menus[i].content.sizeDelta = new Vector2(menus[i].content.sizeDelta.x, ComputeVerticalSize(children[0].GetComponent<RectTransform>(), levels) + 10);
            }
            else
            {
                menus[i].size = 10;
            }
            

            rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, Mathf.Lerp(rt.anchoredPosition.y, y, 0.08f));
            if (menus[i].isExpanded)
            {
                y -= menus[i].size - 5; 
            }
            y -= interval;

        }
    }

    public void CollapseAll()
    {
        foreach (CollapsibleMenu menu in menus)
        {
            menu.isExpanded = false;
        }
    }
}
