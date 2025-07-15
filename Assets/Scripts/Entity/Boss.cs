using UnityEngine;

public class Boss : Monster
{
    public AbilityCard[] abilityUnlock;

    protected override void Start()
    {
        base.Start();
        if (abilityUnlock.Length > 0)
        {
            for (int i = 0; i < abilityUnlock.Length; i++)
            {
                GameManager.instance.AddAbility(abilityUnlock[i]);
            }
        }
        GameManager.instance.GetGrid().endModule.dragonRef = this;
    }
}
