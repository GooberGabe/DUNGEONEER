using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InterfaceManager : MonoBehaviour
{
    public PlacementPreview currentPreview;
    public Entity selectedEntity;
    public UpgradeButton selectedUpgrade;
    public UpgradePanel leftPanel;
    public ConstructionPanel constructionPanel;
    public TextMeshProUGUI roundCount;
    public TextMeshProUGUI goldCount;
    public TextMeshProUGUI costCount;
    public TextMeshProUGUI messageText;
    public TextMeshProUGUI tilePlacementText;
    public CollapsibleManager rightPanel;
    public Button playRoundButton;
    public MenuTooltip menuTooltip;

    public Color sellColor;
    public Color upgradeColor;

    private List<UpgradeTier> tiers = new List<UpgradeTier>();

    public static InterfaceManager instance;

    private StartModule startModule;

    private int counter;
    private int messageTime;

    private void Start()
    {
        instance = this;
        startModule = GameManager.instance.GetGrid().startModules[0];
        messageText.gameObject.SetActive(false);
    }

    private void Update()
    {
        // -- Round count & Gold count
        roundCount.text = (GameManager.instance.round).ToString();
        goldCount.text = GameManager.instance.gold.ToString();

        // -- Defeat message
        counter++;
        if (!GameManager.instance.isAlive)
        {
            messageText.text = "DEFEAT!";
            messageText.gameObject.SetActive(true);
        }
        else if (counter > messageTime)
        {
            messageText.gameObject.SetActive(false);
        }

        // -- LeftPanel


        if (selectedEntity != null)
        {
            leftPanel.gameObject.SetActive(true);
            constructionPanel.gameObject.SetActive(false);
            leftPanel.Show();

            leftPanel.title.text = selectedEntity.entityName;
            leftPanel.description.text = selectedEntity.TextDisplay();

            if (selectedUpgrade == null)
            {
                leftPanel.sellButton.gameObject.SetActive(selectedEntity.entityType != EntityType.Hero && selectedEntity.cost > 0);
                leftPanel.sellButton.interactable = !GameManager.instance.playRound;
                leftPanel.sellButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Sell (" + (selectedEntity.cost - 10).ToString() + " Gold)";
                leftPanel.sellButton.GetComponent<Image>().color = sellColor;
                leftPanel.details.text = "";
            }
            else
            {
                Entity upgrade = selectedUpgrade.purchaseFab.GetComponent<Entity>();
                leftPanel.sellButton.gameObject.SetActive(selectedEntity.entityType != EntityType.Hero && selectedEntity.cost > 0);
                int cost = upgrade.cost;
                leftPanel.sellButton.interactable = GameManager.instance.gold >= cost;
                leftPanel.sellButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = upgrade.upgradeName+" (" + (cost).ToString() + " Gold)";
                leftPanel.sellButton.GetComponent<Image>().color = upgradeColor;
                leftPanel.details.text = upgrade.upgradeTree.FindNodeWithEntity(upgrade).description;
            }

        }
        else if (GameManager.instance.round == 0)
        {
            constructionPanel.gameObject.SetActive(true);
            leftPanel.gameObject.SetActive(false);
            leftPanel.Show();

        }
        else
        {
            leftPanel.Hide();
        }

        RectTransform rectTransform = leftPanel.GetComponent<RectTransform>();
        constructionPanel.transform.GetComponent<RectTransform>().anchoredPosition = rectTransform.anchoredPosition;

        // -- Tile Placement
        int numTiles = GameManager.instance.tilesPlaced;
        int maxTiles = GameManager.instance.maxTiles;
        int price = GameManager.instance.GetTilePrice();
        tilePlacementText.text =  numTiles+ "/" + maxTiles + "\nCost: "+price;
        if (price > 0) tilePlacementText.color = Color.red;
        else tilePlacementText.color = Color.white;

        // -- Play Button
        playRoundButton.interactable = GameManager.instance.isValidPath;

    }

    public void DisplayUpgradeTree(Entity selectedEntity)
    {
        // Clear the previous upgrade tree display
        ClearUpgradeTreeDisplay();
        try
        {
            DisplayUpgradeTreeRecursive(null, selectedEntity.upgradeTree.upgradeNodes[0], leftPanel.upgradeContent.transform, 0);
        }
        catch
        {
            Debug.Log(selectedEntity);
        }
    }

    private void DisplayUpgradeTreeRecursive(UpgradeNode previousNode, UpgradeNode node, Transform parentTransform, int depth)
    {
        // Create a new Tier object for the current entity
        GameObject tierObject;
        if (tiers.Count < depth+1)
        {
            tierObject = Instantiate((GameObject)Resources.Load("UI/Tier"), parentTransform);
            tiers.Add(tierObject.GetComponent<UpgradeTier>());
        }
        else
        {
            tierObject = tiers[depth].gameObject;
        }

        UpgradeTier tier = tierObject.GetComponent<UpgradeTier>();

        // Add the current entity's button to the tier
        Button button = tier.AddEntityButton(node.currentEntity);

        // Get the possible upgrades for the current entity
        List<UpgradeNode> possibleUpgrades = selectedEntity.upgradeTree.GetPossibleUpgrades(node.currentEntity);
        button.GetComponent<UpgradeButton>().entityToUpgrade = selectedEntity;
        button.GetComponent<UpgradeButton>().purchaseFab = node.currentEntity.gameObject;

        if (previousNode != null) 
        {
            button.GetComponent<UpgradeButton>().available = (previousNode.currentEntity.upgradeName == selectedEntity.upgradeName);
        } 
        else 
        {
            button.GetComponent<UpgradeButton>().available = false;
        }
        

        // Recursively display the upgrade tree for each possible upgrade
        foreach (UpgradeNode upgrade in possibleUpgrades)
        {
            DisplayUpgradeTreeRecursive(node, upgrade, parentTransform, depth+1);
        }
    }

    private void ClearUpgradeTreeDisplay()
    {
        // Destroy all child objects of the "Content" panel
        foreach (Transform child in leftPanel.upgradeContent.transform)
        {
            Destroy(child.gameObject);
        }
        selectedUpgrade = null;
        tiers.Clear();
    }

    public void LeftPanelConfirm()
    {
        if (selectedUpgrade == null)
        {
            Sell();
        }
        else
        {
            Upgrade();
        }
    }

    private void Upgrade()
    {
        GameManager.instance.gold -= selectedUpgrade.purchaseFab.GetComponent<Entity>().cost;
        if (selectedUpgrade.entityToUpgrade.entityType != EntityType.Monster)
        {
            Entity newEntity = Instantiate(selectedUpgrade.purchaseFab).GetComponent<Entity>();
            newEntity.transform.position = selectedUpgrade.entityToUpgrade.transform.position;
            newEntity.hitPoints = selectedUpgrade.entityToUpgrade.hitPoints;

            selectedUpgrade.entityToUpgrade.Upgrade();
            selectedUpgrade = null;
            selectedEntity = newEntity;

            DisplayUpgradeTree(newEntity);
        }
        else
        {

        }
    }
    private void Sell()
    {
        if (selectedEntity != null)
        {
            selectedEntity.Sell();
        }
    }

    public void StartRound()
    {
        GameManager.instance.NewRound();
    }

    public void Message(string message, int time) 
    {
        messageText.gameObject.SetActive(true);
        messageText.text = message;
        messageTime = time;
        counter = 0;
    }

    public void CancelPreview()
    {
        if (currentPreview != null)
        {
            Destroy(currentPreview.gameObject);
        }
        GameManager.instance.tilePlacement = false;
    }

}
