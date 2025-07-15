using TMPro;
using UnityEngine;

public class MenuTooltip : Tooltip
{
    [Header("Hex Tooltip References")]
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI contentText;

    private PurchaseButton currentButton;

    public void SetButton(PurchaseButton button)
    {
        if (button == currentButton) return;

        currentButton = button;
        ResetHover();
        lastMousePos = Input.mousePosition;

        if (button != null)
        {
            UpdateContent();
        }
    }

    protected override bool HasTarget()
    {
        return currentButton != null;
    }

    protected override void UpdateContent()
    {
        if (currentButton == null) return;

        (string, string) textContent = currentButton.TextDisplay();

        titleText.text = textContent.Item1;
        contentText.text = textContent.Item2;
    }
}