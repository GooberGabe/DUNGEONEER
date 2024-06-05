using UnityEngine;
using System.Collections.Generic;
using TreeEditor;

[CreateAssetMenu(fileName = "New Upgrade Tree", menuName = "Upgrades/UpgradeTree")]
public class UpgradeTree : ScriptableObject
{
    public List<UpgradeNode> upgradeNodes = new List<UpgradeNode>();

    // Helper method to find all possible upgrades for a given entity
    public List<UpgradeNode> GetPossibleUpgrades(Entity entity)
    {
        List<UpgradeNode> possibleUpgrades = new List<UpgradeNode>();
        UpgradeNode node = FindNodeWithEntity(entity);
        if (node != null)
        {
            possibleUpgrades.AddRange(node.upgrades);
        }
        return possibleUpgrades;
    }

    // Helper method to find the UpgradeNode containing a given entity
    public UpgradeNode FindNodeWithEntity(Entity entity)
    {
        return FindNodeWithEntityRecursive(entity, entity.upgradeTree.upgradeNodes[0]);
    }

    private UpgradeNode FindNodeWithEntityRecursive(Entity lookupEntity, UpgradeNode node)
    {
        if (lookupEntity.upgradeName == node.currentEntity.upgradeName)
        {
            return node;
        }
        foreach (UpgradeNode node2 in node.upgrades)
        {
            UpgradeNode foundNode = FindNodeWithEntityRecursive(lookupEntity, node2);
            if (foundNode != null)
            {
                return foundNode;
            }
        }

        return null;
    }

    public static bool IsEntityAbove(UpgradeNode root, UpgradeNode selectedNode, UpgradeNode entityToCheck)
    {
        if (root == null || selectedNode == null || entityToCheck == null)
            return false;

        if (selectedNode == entityToCheck)
            return false;

        return IsEntityAboveRecursive(root, selectedNode, entityToCheck);
    }

    private static bool IsEntityAboveRecursive(UpgradeNode currentNode, UpgradeNode selectedNode, UpgradeNode entityToCheck)
    {
        if (currentNode == null)
            return false;

        if (currentNode == entityToCheck)
        {
            // entityToCheck is above selectedNode in the tree
            return true;
        }

        foreach (UpgradeNode child in currentNode.upgrades)
        {
            if (child == selectedNode)
            {
                // Check if entityToCheck is in the path from currentNode to selectedNode
                if (IsEntityAboveRecursive(currentNode, entityToCheck, entityToCheck))
                    return true;
            }
            else
            {
                // Recursively check for entityToCheck in the subtree of the child node
                bool isAbove = IsEntityAboveRecursive(child, selectedNode, entityToCheck);
                if (isAbove)
                    return true;
            }
        }

        return false;
    }
}
