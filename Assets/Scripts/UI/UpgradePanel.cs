using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradePanel : MonoBehaviour
{
    public TextMeshProUGUI title;
    public TextMeshProUGUI description;
    public TextMeshProUGUI details;
    public Button sellButton;
    public GameObject upgradeContent;
    public float transitionSpeed = 0.035f;

    private RectTransform rectTransform;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }
    public void Show()
    {
        rectTransform.anchoredPosition = Vector2.Lerp(rectTransform.anchoredPosition, Vector2.zero, transitionSpeed);
    }

    public void Hide()
    {
        Vector2 hiddenPos = new Vector2(-140, 0);
        rectTransform.anchoredPosition = Vector2.Lerp(rectTransform.anchoredPosition, hiddenPos, transitionSpeed);
    }
}
