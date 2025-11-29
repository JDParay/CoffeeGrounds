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
    public Sprite leftCharacter; 
    public Sprite rightCharacter; 

    [Header("Character A Slide Transition")]
    public bool slideCharacterA = false; 
    public float slideADistance = 600f;  
    public float slideASpeed = 0.4f;     


    [Header("Character B Slide Transition")]
    public bool slideCharacterB = false;     
    public float slideDistance = 600f;          
    public float slideSpeed = 0.4f;     

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
