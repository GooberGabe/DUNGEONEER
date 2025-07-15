using UnityEngine;

public class AnimatedTurret : Turret
{
    public Transform animatedElement;
    public Vector3 spawnOffset;
    public AnimationClip fireAnimation;
    public float animationSpeed = 1;

    private Animator animator;

    protected override void Start()
    {
        base.Start();
        animator = animatedElement.GetComponent<Animator>();
        animator.SetFloat("Modifier", animationSpeed);
    }

    private void LateUpdate()
    {
        animator.SetBool("Fire", false);
    }

    public override float GetDamageRate()
    {
        float f = base.GetDamageRate() + (fireAnimation.length / animationSpeed);
        return Mathf.Round(f * 10.0f) * 0.1f;
    }

    public override void Fire()
    {
        // Freeze cooldown until the animation has finished.
        if (GetTarget() != null)
        {
            cooldownCounter = -99;
            animator.SetBool("Fire", true);

        }
    }
}
