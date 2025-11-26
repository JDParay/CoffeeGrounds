using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneTransition : MonoBehaviour
{
    public static Vector2 lastClickPos;   // Stores click location between scenes
    public static bool playReverse = false;

    public Image[] squares; // 3 circles
    public float expandSpeed = 4f;
    public float maxScale = 6f;

    void Start()
    {
        if (playReverse) StartCoroutine(ReverseTransition());
    }

    public void PlayTransition(int sceneIndex)
    {
        Vector2 mousePos = Input.mousePosition;
        lastClickPos = mousePos;
        StartCoroutine(Expand(sceneIndex));
    }

    IEnumerator Expand(int sceneIndex)
    {
        foreach (var square in squares)
        {
            square.transform.position = lastClickPos;
            square.transform.localScale = Vector3.zero;
            square.gameObject.SetActive(true);
        }

        while (squares[0].transform.localScale.x < maxScale)
        {
            foreach (var square in squares)
                square.transform.localScale += Vector3.one * expandSpeed * Time.deltaTime;

            yield return null;
        }

        playReverse = true;
        SceneManager.LoadScene(sceneIndex);
    }

    IEnumerator ReverseTransition()
    {
        foreach (var square in squares)
        {
            square.transform.position = lastClickPos;
            square.gameObject.SetActive(true);
            square.transform.localScale = Vector3.one * maxScale;
        }

        while (squares[0].transform.localScale.x > 0.05f)
        {
            foreach (var square in squares)
                square.transform.localScale -= Vector3.one * expandSpeed * Time.deltaTime;

            yield return null;
        }

        foreach (var square in squares) square.gameObject.SetActive(false);
        playReverse = false;
    }
}
