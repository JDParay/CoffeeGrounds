using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class DialogueManager : MonoBehaviour
{
    public DialogueData dialogue;

    [Header("UI - Main Dialogue Box")]
    public GameObject mainBox;
    public TMP_Text nameText;
    public TMP_Text mainDialogueText;

    [Header("UI - Alternate Dialogue Box")]
    public GameObject altBox;
    public TMP_Text altDialogueText;

    [Header("Character Slots")]
    public Image leftCharacterImage;
    public Image rightCharacterImage;

    [Header("Visuals")]
    public Image backgroundImage;

    [Header("Audio")]
    public AudioSource voiceSource;

    [Header("Typing")]
    public float typeSpeed = 0.03f;

    [Header("Speaker Highlight")]
    public Color activeColor = Color.white;                          // speaking = normal
    public Color fadedColor = new Color(0.55f, 0.55f, 0.55f, 1f);     // gray when NOT speaking

    private int index = 0;
    private bool isTyping = false;
    private Coroutine typingCoroutine;


    void Start()
    {
        if (altBox != null) altBox.SetActive(false);
        ShowLine();
    }

    public void OnScreenTap()
    {
        if (isTyping)
        {
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

        // BACKGROUND UPDATE
        if (line.newBackground != null)
            backgroundImage.sprite = line.newBackground;

        // CHARACTER SPRITES
        if (line.leftCharacter != null) leftCharacterImage.sprite = line.leftCharacter;
        if (line.rightCharacter != null) rightCharacterImage.sprite = line.rightCharacter;

        // 🔥 NEW → SPEAKER GRAY LOGIC
        if (line.speakerType == SpeakerType.CharacterA)
        {
            leftCharacterImage.color = activeColor;
            rightCharacterImage.color = fadedColor;
        }
        else if (line.speakerType == SpeakerType.CharacterB)
        {
            leftCharacterImage.color = fadedColor;
            rightCharacterImage.color = activeColor;
        }
        else // narration
        {
            leftCharacterImage.color = fadedColor;
            rightCharacterImage.color = fadedColor;
        }

        // SHAKE EFFECT
        if (line.shakeBackground)
            StartCoroutine(ShakeBackground(line.shakeIntensity, line.shakeDuration));

        // AUDIO
        if (voiceSource != null)
        {
            voiceSource.Stop();
            if (line.voiceOver != null) voiceSource.PlayOneShot(line.voiceOver);
        }

        // TEXT DISPLAY
        if (line.useAltBox)
        {
            mainBox.SetActive(false);
            altBox.SetActive(true);
            nameText.text = line.speaker;

            if (typingCoroutine != null) StopCoroutine(typingCoroutine);
            typingCoroutine = StartCoroutine(TypeLine_Alt(line.text));
        }
        else
        {
            altBox.SetActive(false);
            mainBox.SetActive(true);
            nameText.text = line.speaker;

            if (typingCoroutine != null) StopCoroutine(typingCoroutine);
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
        if (line.useAltBox) altDialogueText.text = line.text;
        else mainDialogueText.text = line.text;
        isTyping = false;
    }

    IEnumerator ShakeBackground(float intensity, float duration)
    {
        Vector3 originalPos = backgroundImage.rectTransform.localPosition;
        float time = 0;

        while (time < duration)
        {
            float x = Random.Range(-intensity, intensity);
            float y = Random.Range(-intensity, intensity);

            backgroundImage.rectTransform.localPosition = originalPos + new Vector3(x, y, 0);

            time += Time.deltaTime;
            yield return null;
        }

        backgroundImage.rectTransform.localPosition = originalPos;
    }
}
