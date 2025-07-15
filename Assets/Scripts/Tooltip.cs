using UnityEngine;
using TMPro;
using UnityEngine.UI;

public abstract class Tooltip : MonoBehaviour
{
    [Header("Base Settings")]
    [SerializeField] protected float hoverDelay = 0.5f;
    [SerializeField] protected Vector2 cursorOffset = new Vector2(15f, 15f);
    [SerializeField] protected CanvasGroup canvasGroup;

    protected float hoverTimer;
    protected Vector2 lastMousePos;
    protected bool isVisible;
    protected Camera mainCamera;

    protected virtual void Awake()
    {
        mainCamera = Camera.main;
        Hide();
    }

    protected virtual void Update()
    {
        if (!HasTarget()) return;

        // Check if mouse has moved
        if (Vector2.Distance(lastMousePos, Input.mousePosition) > 1f)
        {
            ResetHover();
        }
        else
        {
            hoverTimer += Time.deltaTime;
            if (hoverTimer >= hoverDelay && !isVisible)
            {
                Show();
            }
        }

        if (isVisible)
        {
            UpdatePosition();
        }

        lastMousePos = Input.mousePosition;
    }

    protected void ResetHover()
    {
        hoverTimer = 0;
        if (isVisible) Hide();
    }

    protected virtual void UpdatePosition()
    {
        float screenHalfWidth = Screen.width / 2f;

        Vector3 position = Input.mousePosition;
        position += new Vector3(position.x > screenHalfWidth ? cursorOffset.x : -cursorOffset.x, cursorOffset.y, 0);

        // Keep tooltip on screen
        RectTransform rect = transform as RectTransform;
        Vector3 size = rect.rect.size;
        position.x = Mathf.Min(position.x, Screen.width - size.x);
        position.y = Mathf.Min(position.y, Screen.height - size.y);

        transform.position = position;
    }

    protected virtual void Show()
    {
        isVisible = true;
        canvasGroup.alpha = 1;
        canvasGroup.blocksRaycasts = true;
        LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
    }

    protected virtual void Hide()
    {
        isVisible = false;
        canvasGroup.alpha = 0;
        canvasGroup.blocksRaycasts = false;
    }

    // Abstract methods that derived classes must implement
    protected abstract bool HasTarget();
    protected abstract void UpdateContent();
}