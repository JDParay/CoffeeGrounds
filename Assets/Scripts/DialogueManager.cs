using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class DialogueManager : MonoBehaviour
{
    [Header("Dialogue Data")]
    public DialogueData dialogue;

    [Header("UI - Main Dialogue Box")]
    public GameObject mainBox;
    public TMP_Text nameText;
    public TMP_Text mainDialogueText;

    [Header("UI - Alternate Dialogue Box")]
    public GameObject altBox;
    public TMP_Text altDialogueText;

    [Header("Audio")]
    public AudioSource voiceSource;

    [Header("Typing")]
    public float typeSpeed = 0.03f;

    private int index = 0;
    private bool isTyping = false;
    private Coroutine typingCoroutine;

    public Image backgroundImage;
    public Image characterImage;


    void Start()
    {
        if (altBox != null)
            altBox.SetActive(false);

        ShowLine();
    }

    public void OnScreenTap()
    {
        if (isTyping)
        {
            // Skip to full text
            StopCoroutine(typingCoroutine);
            FinishLineInstantly();
            return;
        }

        NextLine();
    }

    void NextLine()
    {
        index++;

        if (index >= dialogue.lines.Length)
        {
            SceneManager.LoadScene(dialogue.nextSceneName);
            return;
        }

        ShowLine();
    }

    void ShowLine()
    {
        var line = dialogue.lines[index];

        // BACKGROUND CHANGE
    if (line.newBackground != null)
    {
        backgroundImage.sprite = line.newBackground;
    }

    // CHARACTER SPRITE CHANGE
    if (line.newCharacterSprite != null)
    {
        characterImage.sprite = line.newCharacterSprite;
    }

        // Play voice
        if (voiceSource != null)
        {
            voiceSource.Stop();
            if (line.voiceOver != null)
                voiceSource.PlayOneShot(line.voiceOver);
        }

        if (line.useAltBox)
        {
            mainBox.SetActive(false);
            altBox.SetActive(true);

            nameText.text = line.speaker;

            if (typingCoroutine != null)
                StopCoroutine(typingCoroutine);

            typingCoroutine = StartCoroutine(TypeLine_Alt(line.text));
        }
        else
        {
            altBox.SetActive(false);
            mainBox.SetActive(true);

            nameText.text = line.speaker;

            if (typingCoroutine != null)
                StopCoroutine(typingCoroutine);

            typingCoroutine = StartCoroutine(TypeLine_Main(line.text));
        }
    }

    IEnumerator TypeLine_Main(string text)
    {
        isTyping = true;
        mainDialogueText.text = "";

        foreach (char c in text)
        {
            mainDialogueText.text += c;
            yield return new WaitForSeconds(typeSpeed);
        }

        isTyping = false;
    }

    IEnumerator TypeLine_Alt(string text)
    {
        isTyping = true;
        altDialogueText.text = "";

        foreach (char c in text)
        {
            altDialogueText.text += c;
            yield return new WaitForSeconds(typeSpeed);
        }

        isTyping = false;
    }

    void FinishLineInstantly()
    {
        var line = dialogue.lines[index];

        if (line.useAltBox)
        {
            altDialogueText.text = line.text;
        }
        else
        {
            mainDialogueText.text = line.text;
        }

        isTyping = false;
    }
}
