using UnityEngine;
using TMPro;

public class TooltipController : MonoBehaviour
{
    public static TooltipController Instance;

    public RectTransform tooltipTransform;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI conditionText;
    public TextMeshProUGUI descriptionText;
    public Canvas canvas;

    // Tooltip will appear top-right from cursor
    private Vector2 offset = new Vector2(20f, -20f);

    private void Awake()
    {
        Instance = this;
        tooltipTransform.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (!tooltipTransform.gameObject.activeSelf) return;

        Vector2 screenPos = Input.mousePosition;
        Vector2 localPos;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            screenPos,
            canvas.worldCamera,
            out localPos
        );

        // Apply offset
        tooltipTransform.anchoredPosition = localPos + offset;
    }

    public void ShowTooltip(string title, string condition, string description)
{
    titleText.text = title;
    conditionText.text = condition;
    descriptionText.text = description;
    tooltipTransform.gameObject.SetActive(true);
}  


    public void HideTooltip()
    {
        tooltipTransform.gameObject.SetActive(false);
    }
}
