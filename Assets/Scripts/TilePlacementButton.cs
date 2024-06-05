using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TilePlacementButton : MonoBehaviour
{
    public void Interact()
    {
        GameManager.instance.tilePlacement = true;
    }

    private void Update()
    {
        GetComponent<Button>().interactable = GameManager.instance.CanPlaceTiles();
    }
}
