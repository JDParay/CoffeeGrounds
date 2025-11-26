using UnityEngine;

[System.Serializable]
public enum SpeakerType { None, CharacterA, CharacterB }

[System.Serializable]
public class DialogueLine
{
    public string speaker;
    public SpeakerType speakerType = SpeakerType.CharacterA;

    [TextArea(3, 10)]
    public string text;
    public bool useAltBox;

    public AudioClip voiceOver;

    [Header("Visuals")]
    public Sprite newBackground;     

    [Header("Character Sprites")]
    public Sprite leftCharacter;      // Character A
    public Sprite rightCharacter;     // Character B

    [Header("Character B Slide Transition")]
    public bool slideCharacterB = false;        // enable slide
    public float slideDistance = 600f;          // how far offscreen
    public float slideSpeed = 0.35f;            // animation speed

    [Header("Effects")]
    public bool shakeBackground;
    public float shakeIntensity = 20f;
    public float shakeDuration = 0.25f;
}

[CreateAssetMenu(fileName = "Dialogue", menuName = "VN/Dialogue")]
public class DialogueData : ScriptableObject
{
    public DialogueLine[] lines;
    public string nextSceneName;
}
