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

    [Header("Scene Transition Panels")]
    public GameObject vsPanel;           // shows after final dialogue
    public GameObject sceneTransition;   // transition before scene loads
    public float vsDisplayDuration = 3f; // how long VS stays before switch
    public float transitionDuration = 1.5f; // wait for animation
    
    [Header("Scene Transition")]
    public EnterGameTransition transitionManager;

    private int index = 0;
    private bool isTyping = false;
    private Coroutine typingCoroutine;


    void Start()
    {
        if (altBox != null) altBox.SetActive(false);
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

        public void ShowFirstLine()
    {
        index = 0;
        ShowLine();
    }

        void NextLine()
    {
        index++;
        if (index >= dialogue.lines.Length)
        {
            StartCoroutine(EndDialogueSequence());
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

        // CHARACTER A — Slide In Support
        if (line.leftCharacter != null)
            {
                if (line.slideCharacterA && leftCharacterImage.sprite != line.leftCharacter)
                    StartCoroutine(SlideInCharacterA(line.leftCharacter, line.slideADistance, line.slideASpeed));
                else
                    leftCharacterImage.sprite = line.leftCharacter;
            }

        if (line.rightCharacter != null)
            {
                if (line.slideCharacterB && rightCharacterImage.sprite != line.rightCharacter)
                    StartCoroutine(SlideInCharacterB(line.rightCharacter, line.slideDistance, line.slideSpeed));
                else
                    rightCharacterImage.sprite = line.rightCharacter;
            }


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

    IEnumerator SlideInCharacterA(Sprite newSprite, float distance, float speed)
    {
        RectTransform rt = leftCharacterImage.rectTransform;
        Vector3 originalPos = rt.localPosition;
        Vector3 offscreen = originalPos + new Vector3(-distance, 0, 0); // slide left side out

        float t = 0;
        while (t < 1f)
        {
            rt.localPosition = Vector3.Lerp(originalPos, offscreen, t);
            t += Time.deltaTime / speed;
            yield return null;
        }

        rt.localPosition = offscreen;
        leftCharacterImage.sprite = newSprite;

        t = 0;
        while (t < 1f)
        {
            rt.localPosition = Vector3.Lerp(offscreen, originalPos, t);
            t += Time.deltaTime / speed;
            yield return null;
        }

        rt.localPosition = originalPos;
    }


    IEnumerator SlideInCharacterB(Sprite newSprite, float distance, float speed)
    {
        RectTransform rt = rightCharacterImage.rectTransform;
        Vector3 originalPos = rt.localPosition;
        Vector3 offscreen = originalPos + new Vector3(distance, 0, 0);

        // slide OUT
        float t = 0;
        while (t < 1f)
        {
            rt.localPosition = Vector3.Lerp(originalPos, offscreen, t);
            t += Time.deltaTime / speed;
            yield return null;
        }

        rt.localPosition = offscreen;
        rightCharacterImage.sprite = newSprite; // swap after exit

        // slide IN
        t = 0;
        while (t < 1f)
        {
            rt.localPosition = Vector3.Lerp(offscreen, originalPos, t);
            t += Time.deltaTime / speed;
            yield return null;
        }

        rt.localPosition = originalPos;
    }

    IEnumerator EndDialogueSequence()
    {
        if (vsPanel != null) 
            vsPanel.SetActive(true);

        yield return new WaitForSeconds(vsDisplayDuration);

        if (sceneTransition != null)
            sceneTransition.SetActive(true);

        transitionManager.PlayStart();
    }

}
