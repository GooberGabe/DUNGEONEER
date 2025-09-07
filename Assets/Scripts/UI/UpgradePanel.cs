using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradePanel : SlidePanel
{
    public TextMeshProUGUI title;
    public TextMeshProUGUI description;
    public TextMeshProUGUI details;
    public Button sellButton;
    public GameObject upgradeContent;
    
}

public class SlidePanel : MonoBehaviour
{
    public float transitionSpeed = 0.035f;
    public Vector2 hiddenPos = new Vector2(-140, 0);
    public Vector2 shownPos = Vector2.zero;

    protected RectTransform rectTransform;
    protected bool show;

    protected virtual void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }
    public void Show()
    {
        if (!gameObject.activeSelf) MoveToShow();
        show = true;
    }
    
    public void Hide()
    {
        show = false;
        if (!gameObject.activeSelf) MoveToHide();
    }

    private void MoveToShow()
    {
        rectTransform.anchoredPosition = Vector2.Lerp(rectTransform.anchoredPosition, shownPos, transitionSpeed);
    }

    private void MoveToHide()
    {
        rectTransform.anchoredPosition = Vector2.Lerp(rectTransform.anchoredPosition, hiddenPos, transitionSpeed);
    }

    protected virtual void Update()
    {
        if (show)
        {
            MoveToShow();
            return;
        }
        MoveToHide();
        
    }
}