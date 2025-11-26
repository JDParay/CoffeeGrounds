using UnityEngine;
using UnityEngine.SceneManagement;

public class StopBGMOnScene : MonoBehaviour
{
    public int stopSceneIndex = 4;

    void Start()
    {
        if (SceneManager.GetActiveScene().buildIndex == stopSceneIndex)
        {
            if (BGMManager.instance != null)
                Destroy(BGMManager.instance.gameObject);
        }
    }
}
