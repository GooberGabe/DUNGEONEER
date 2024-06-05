using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.UI;

public class PurchaseButton : MonoBehaviour
{
    public GameObject purchaseFab;
    public GameObject previewFab;
    public InterfaceManager ui;

    protected Button button;

    protected virtual void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
    }

    protected virtual void Update()
    {
        button.interactable = GameManager.instance.gold >= purchaseFab.GetComponent<Entity>().cost;
    }

    private void OnClick()
    {
        // Instantiate the prefab and allow the player to place it
        Debug.Log($"Purchased {purchaseFab.name}");
        Purchase();

    }

    protected virtual void Purchase()
    {
        if (ui.currentPreview == null)
        {
            GameObject prev = Instantiate(previewFab);
            PlacementPreview pp = prev.GetComponent<PlacementPreview>();
            pp.placement = purchaseFab;
            ui.currentPreview = pp;
        }
    }
}
