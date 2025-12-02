using UnityEngine;

public class OpenCloseButton : MonoBehaviour
{
    [Header("UI References")]
    public GameObject Panel;

    void Start()
    {
        Panel.SetActive(false);
    }

    public void OpenHowToPlay()
    {
        Panel.SetActive(true);
    }
    public void CloseHowToPlay()
    {
        Panel.SetActive(false);
    }
}
