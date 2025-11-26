using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnterGameTransition : MonoBehaviour
{
    public Animator transition;   // Drag reference in Inspector
    public float transitionTime = 1f;    // Change scene index here

    public void PlayStart()
    {
        StartCoroutine(LoadLevel(SceneManager.GetActiveScene().buildIndex + 1));
    }

    IEnumerator LoadLevel(int levelIndex)
    {
        transition.SetTrigger("Start");

        yield return new WaitForSeconds(transitionTime);

        SceneManager.LoadScene(levelIndex);
    }
}
