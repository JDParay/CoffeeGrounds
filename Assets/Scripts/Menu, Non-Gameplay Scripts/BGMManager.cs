using UnityEngine;

public class BGMManager : MonoBehaviour
{
    public static BGMManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject); // prevents duplicates if returning to previous scene
        }
    }
}
