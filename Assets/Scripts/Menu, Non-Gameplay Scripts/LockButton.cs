using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LockButton : MonoBehaviour
{
    public Button button; 
    public bool isUnlocked = false; 

    [Header("UI Feedback")]
    public TMP_Text messageText;
    public string lockedMessage = "Once the game jam officially ends!";
    public string successMessage = "Button pressed successfully!";

    private void Start()
    {
        // Add listener to the button
        button.onClick.AddListener(OnClick);

        // Clear message at start
        if (messageText != null)
            messageText.text = "";
    }

    private void OnClick()
    {
        if (isUnlocked)
        {
            if (messageText != null) messageText.text = successMessage;

        }
        else
        {
            // Button is locked
            if (messageText != null) messageText.text = lockedMessage;
        }
    }

    public void UnlockButton() => isUnlocked = true;
    public void LockedButton() => isUnlocked = false;
}
