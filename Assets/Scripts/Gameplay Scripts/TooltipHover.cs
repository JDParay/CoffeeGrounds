using UnityEngine;

public class HoverTooltip : MonoBehaviour
{
    [TextArea]
    public string title;
    public string condition;
    public string description;

    private void OnMouseEnter()
{
    TooltipController.Instance.ShowTooltip(
        title,
        condition,
        description
    );
}

    private void OnMouseExit()
    {
        TooltipController.Instance.HideTooltip();
    }
}
