using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.IO.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor.EventSystems;
using UnityEngine.EventSystems;

public class PurchaseButton : MonoBehaviour, IPointerEnterHandler
{
    public GameObject purchaseFab;
    public GameObject previewFab;
    public InterfaceManager ui;
    public TMP_Text costText;
    public Image icon;

    protected Button button;

    protected virtual void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
    }

    protected virtual void Update()
    {
        button.interactable = GameManager.instance.gold >= purchaseFab.GetComponent<Entity>().cost;
        int cost = previewFab.GetComponent<PlacementPreview>().cost;
        costText.text = $"{cost}";
    }
    
    private void OnClick()
    {
        // Instantiate the prefab and allow the player to place it
        Debug.Log($"Purchased {purchaseFab.name}");
        Purchase();

    }

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        ui.menuTooltip.SetButton(this);
    }

    protected virtual void Purchase()
    {
        if (ui.currentPreview == null)
        {
            GameObject prev = Instantiate(previewFab);
            PlacementPreview pp = prev.GetComponent<PlacementPreview>();
            pp.placement = purchaseFab;
            pp.buttonRef = gameObject;
            ui.currentPreview = pp;

        }
    }
}
