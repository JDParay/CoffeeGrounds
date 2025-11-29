using UnityEngine;
using System.Collections;

public class VNSceneTransition : MonoBehaviour
{
    [Header("UI Groups")]
    public CanvasGroup blackPanel;       // Fullscreen black panel
    public CanvasGroup vnUI;             // Dialogue UI group

    [Header("Timings")]
    public float blackFadeDuration = 1.2f;
    public float uiFadeDuration = 1f;
    public float dialogueDelay = 0.5f;

    [Header("References")]
    public DialogueManager dialogueManager;  // call dialogue start

    void Start()
    {
        StartCoroutine(PlaySceneEntry());
    }

    IEnumerator PlaySceneEntry()
    {
        yield return new WaitForSeconds(1f);
        // ============ STEP 1: Fade black to reveal background ============
        yield return FadeCanvasGroup(blackPanel, 1, 0, blackFadeDuration);

        // ============ STEP 2: Fade in VN UI elements =====================
        yield return FadeCanvasGroup(vnUI, 0, 1, uiFadeDuration);

        // ============ STEP 3: Start dialogue after delay =================
        yield return new WaitForSeconds(dialogueDelay);
        dialogueManager.ShowFirstLine(); 
    }

    IEnumerator FadeCanvasGroup(CanvasGroup cg, float from, float to, float time)
    {
        float t = 0;
        cg.alpha = from;
        cg.blocksRaycasts = true;

        while (t < time)
        {
            t += Time.deltaTime;
            cg.alpha = Mathf.Lerp(from, to, t / time);
            yield return null;
        }

        cg.alpha = to;
        cg.blocksRaycasts = to > 0.9f; // enable input only when visible
    }
}
