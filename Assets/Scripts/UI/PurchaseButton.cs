using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PurchaseButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public bool acceptsTooltips = true;
    public GameObject purchaseFab;
    public GameObject previewFab;
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
        Purchase();

    }

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        if (acceptsTooltips) InterfaceManager.instance.menuTooltip.SetButton(this);
    }

    public void OnPointerExit(PointerEventData pointerEventData)
    {
        if (acceptsTooltips) InterfaceManager.instance.menuTooltip.SetButton(this);
    }

    protected virtual void Purchase()
    {
        if (InterfaceManager.instance.currentPreview == null && purchaseFab != null)
        {
            GameObject prev = Instantiate(previewFab);
            PlacementPreview pp = prev.GetComponent<PlacementPreview>();
            pp.placement = purchaseFab;
            pp.buttonRef = gameObject;
            InterfaceManager.instance.currentPreview = pp;

        }
    }

    public virtual (string head, string body) TextDisplay()
    {
        Entity entity = purchaseFab.GetComponent<Entity>();
        return (entity.entityName, (entity.description.Length > 0 ? entity.description + "\n" : "") + entity.TextDisplay());
    }
}
