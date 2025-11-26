using UnityEngine;
using UnityEngine.EventSystems;

public class OpenLinkOnClick : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private string url = "";

    public void OnPointerClick(PointerEventData eventData)
    {
        Application.OpenURL(url);
    }
}
