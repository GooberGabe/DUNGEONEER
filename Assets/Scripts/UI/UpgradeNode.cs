
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Node", menuName = "Upgrades/Node")]
public class UpgradeNode : ScriptableObject
{
    public Entity currentEntity;
    public List<UpgradeNode> upgrades = new List<UpgradeNode>();
    public string description;

    public override string ToString()
    {
        return currentEntity.entityName + ", " + upgrades.Count.ToString() + " upgrades.";
    }
}