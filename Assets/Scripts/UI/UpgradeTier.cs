using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeTier : MonoBehaviour
{
    public GameObject buttonPrefab; // Prefab for the entity buttons

    public Button AddEntityButton(Entity entity)
    {
        // Instantiate a new button for the entity
        GameObject buttonObject = Instantiate(buttonPrefab, transform);
        Button button = buttonObject.GetComponent<Button>();
        button.GetComponent<UpgradeButton>().purchaseFab = entity.gameObject;

        // Set the button text
        button.GetComponentInChildren<TextMeshProUGUI>().text = entity.upgradeName.Length > 0 ? entity.upgradeName : entity.entityName;
        return button;
    }
}