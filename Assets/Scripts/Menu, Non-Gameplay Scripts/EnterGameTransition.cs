using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnterGameTransition : MonoBehaviour
{
    public Animator transition;   // Drag reference in Inspector
    public float transitionTime = 1f;
    [Header("Go to Scene (buildIndex + input)")]
    public int Index = 1;
    public void PlayStart()
    {
        StartCoroutine(LoadLevel(SceneManager.GetActiveScene().buildIndex + Index));
    }

    IEnumerator LoadLevel(int levelIndex)
    {
        transition.SetTrigger("Start");

        yield return new WaitForSeconds(transitionTime);

        SceneManager.LoadScene(levelIndex);
    }
}
