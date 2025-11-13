using UnityEngine;

public class Wizard : Hero
{
    public override void SetStatusEffect(StatusEffect effect, float duration)
    {
     
        if (effect == StatusEffect.Burning || effect == StatusEffect.Slowed)
        {
            return;
        }
        base.SetStatusEffect(effect, duration);
    }

    public override void Hit()
    {
        base.Hit();
        if (GetTarget() != null)
        {
            Creature target = GetTarget().GetComponent<Creature>();
            Instantiate(projectile, target.transform.position + (Vector3.up * 15), projectile.transform.rotation);
        }
    }
}
