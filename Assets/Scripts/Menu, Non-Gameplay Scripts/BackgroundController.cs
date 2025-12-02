using UnityEngine;
using UnityEngine.SceneManagement;

public class BackgroundController : MonoBehaviour
{
    [Header("Scenes where background should be removed")]
    public string[] sceneBlacklist;

    private static BackgroundController instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            // Make sure this canvas stays behind UI layers
            Canvas canvas = GetComponent<Canvas>();
            if (canvas != null) canvas.sortingOrder = -10;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        foreach (var s in sceneBlacklist)
        {
            if (scene.name == s)
            {
                Destroy(gameObject);
                return;
            }
        }
    }
}
