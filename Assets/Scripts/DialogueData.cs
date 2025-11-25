using UnityEngine;

[System.Serializable]
public class DialogueLine
{
    public string speaker;
    [TextArea(3, 10)]
    public string text;
    public bool useAltBox;
    public AudioClip voiceOver; 
}

[CreateAssetMenu(fileName = "Dialogue", menuName = "VN/Dialogue")]
public class DialogueData : ScriptableObject
{
    public DialogueLine[] lines;
    public string nextSceneName;   // where to go after VN
}
