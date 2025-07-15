using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Turret : DynamicEntity
{
    public override EntityType entityType => EntityType.Turret;
    public PriorityType priority = PriorityType.First;

    public override string TextDisplay()
    {
        return
               "Range:  " + GetRange() +
               "\nDamage: " + GetDamage() * firePosition.Length +
               "\nReload Time: " + GetDamageRate() + "s";
    }

    public void SetPriority(PriorityType newPriority)
    {
        priority = newPriority;
        InterfaceManager.instance.UpdateDropdownForCurrentTurret();
    }

    public override float GetDamage()
    {
        return projectile.GetComponent<Projectile>().damage;
    }

    public override void Fire()
    {
        base.Fire();
        cooldownCounter = cooldownTime;
    }

    protected override List<Entity> GetValidTargets()
    {
        return base.GetValidTargets().Where(x => detectedTypes.Contains(x.entityType)).ToList().OrderBy(
            x => Vector3.Distance(transform.position, x.transform.position) * (targetClose ? -1 : 1)).ToList();
    }

    private void LockTarget()
    {
        List<Entity> validTargets = GetValidTargets();
        if (validTargets.Count == 0)
        {
            mainDestination = null;
            return;
        }

        List<Entity> targetsInLineOfSight = validTargets.Where(target =>
            IsTargetInLineOfSight(target.transform) && target.hitPoints > 0).ToList();

        if (targetsInLineOfSight.Count == 0)
        {
            mainDestination = null;
            return;
        }

        Entity selectedTarget = SelectTargetByPriority(targetsInLineOfSight);
        if (selectedTarget != null)
        {
            mainDestination = selectedTarget.GetTrackingTarget();
            Face(mainDestination);
        }
    }

    private Entity SelectTargetByPriority(List<Entity> targets)
    {
        switch (priority)
        {
            case PriorityType.First:
                // Return the first target that entered range
                return targets.LastOrDefault();

            case PriorityType.Last:
                // Return the last target that entered range
                return targets.FirstOrDefault();

            case PriorityType.Close:
                // Return the closest target
                return targets.OrderBy(target =>
                    Vector3.Distance(transform.position, target.transform.position)).FirstOrDefault();

            case PriorityType.Far:
                // Return the farthest target
                return targets.OrderByDescending(target =>
                    Vector3.Distance(transform.position, target.transform.position)).FirstOrDefault();

            case PriorityType.Strong:
                // Return the target with the highest health
                return targets.OrderByDescending(target => target.hitPoints).FirstOrDefault();

            default:
                return targets.FirstOrDefault();
        }
    }

    public override void Upgrade(Entity upgradedVersion)
    {
        ((Turret)upgradedVersion).SetPriority(priority);
        base.Upgrade(upgradedVersion);
    }

    // Helper method to get priority as string (for UI integration)
    public string GetPriorityString()
    {
        switch (priority)
        {
            case PriorityType.First: return "First";
            case PriorityType.Last: return "Last";
            case PriorityType.Close: return "Closest";
            case PriorityType.Far: return "Farthest";
            case PriorityType.Strong: return "Strongest";
            default: return "First";
        }
    }

    // Helper method to set priority from string (for UI integration)
    public void SetPriorityFromString(string priorityString)
    {
        switch (priorityString)
        {
            case "First": priority = PriorityType.First; break;
            case "Last": priority = PriorityType.Last; break;
            case "Closest": priority = PriorityType.Close; break;
            case "Farthest": priority = PriorityType.Far; break;
            case "Strongest": priority = PriorityType.Strong; break;
            default: priority = PriorityType.First; break;
        }
    }

    private bool CanFire()
    {
        return cooldownCounter <= 0;
    }

    protected override void Update()
    {
        base.Update();

        LockTarget();

        if (CanFire())
        {
            if (GetTarget() != null)
            {
                Fire();
            }
        }
    }
}

public enum PriorityType
{
    First,
    Last,
    Close,
    Far,
    Strong,
}
