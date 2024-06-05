using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthDisplay : Display
{
    public Slider healthSlider;
    public Image fill;

    protected override void Update()
    {
        if (subject == null || subject.hitPoints <= 0) Destroy(gameObject);
        else
        {
            healthSlider.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 0.36f + (.004f * subject.maxHitPoints));
            healthSlider.value = subject.hitPoints / subject.maxHitPoints;
            base.Update();
        }
        
    }
}
