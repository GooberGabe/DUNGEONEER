using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UpgradeButton : PurchaseButton
{
    public Entity entityToUpgrade;
    public GameObject outline;
    public bool available = false;

    protected override void Start()
    {
        base.Start();
        outline.SetActive(false);
    }

    protected override void Update()
    {
        //base.Update();
        if (!available) button.interactable = false;

    }

    protected override void Purchase()
    {
        Debug.Log("Upgrade selected!");
        if (entityToUpgrade != null)
        {
            InterfaceManager.instance.selectedUpgrade = this;
        }
    }

    public override (string head, string body) TextDisplay()
    {
        Entity entity = purchaseFab.GetComponent<Entity>();
        return (entity.upgradeName, entity.TextDisplay());
    }
}
