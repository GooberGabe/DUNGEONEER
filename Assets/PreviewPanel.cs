using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PreviewPanel : SlidePanel
{
    public TMP_Text infoText;
    public TMP_Text nameText;
    public Image icon;
    private Hero focusHero;

    protected override void Start()
    {
        base.Start();
        rectTransform.localPosition = hiddenPos;
    }

    public void SetHero(Hero hero)
    {
        focusHero = hero;
        nameText.text = focusHero.name;
        infoText.text = focusHero.TextDisplay();
        icon.sprite = focusHero.icon;

        Show();
    }

    public void Close()
    {
        Hide();
    }

    protected override void Update()
    {
        base.Update();
        if (!show) Destroy(gameObject,10);
    }

}
