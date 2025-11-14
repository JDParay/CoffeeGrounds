using UnityEngine;

public class HoverTooltip : MonoBehaviour
{
    [TextArea]
    public string description;

    private void OnMouseEnter()
    {
        TooltipController.Instance.ShowTooltip(description);
    }

    private void OnMouseExit()
    {
        TooltipController.Instance.HideTooltip();
    }
}
