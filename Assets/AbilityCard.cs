using UnityEngine;

public class AbilityCard : PurchaseButton
{
    public string abilityName;
    public string description;
    public float cooldownTime;
    private float cooldownCounter;

    public RectTransform cooldownIndicator;

    protected override void Start()
    {
        base.Start();
        cooldownCounter = 0;
    }

    protected override void Update()
    {
        cooldownCounter -= Time.deltaTime;
        button.interactable = cooldownCounter <= 0;
        float progress = ((cooldownCounter / cooldownTime) * 80) - 80;
        cooldownIndicator.anchoredPosition = new Vector2(cooldownIndicator.anchoredPosition.x, progress);
    }

    protected override void Purchase()
    {
        base.Purchase();
        cooldownCounter = cooldownTime;
    }

    public override (string head, string body) TextDisplay()
    {
        return (abilityName, description);
    }
}
