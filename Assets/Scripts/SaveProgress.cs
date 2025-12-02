using System.IO;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public bool story1Completed = false;
    public bool story2Unlocked = false;
    public bool minigameUnlocked = false;
}

public class SaveProgress : MonoBehaviour
{
    public static SaveProgress Instance; // Singleton for easy access

    private string savePath;
    public PlayerData data = new PlayerData();

    private void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        savePath = Application.persistentDataPath + "/save.json";
        LoadData();
    }

    // ======== Local Save/Load ========
    public void SaveData()
    {
        string json = JsonUtility.ToJson(data);
        File.WriteAllText(savePath, json);
        Debug.Log("Progress saved locally!");

        // Optional: send json to cloud here
        CloudSave(json);
    }

    public void LoadData()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            data = JsonUtility.FromJson<PlayerData>(json);
            Debug.Log("Progress loaded!");
        }
        else
        {
            Debug.Log("No save file found, starting fresh.");
        }
    }

    // ======== Progress Tracking ========
    public void CompleteStory1()
    {
        data.story1Completed = true;
        data.story2Unlocked = true; // unlock story 2 automatically
        SaveData();
    }

    public void UnlockMinigame()
    {
        data.minigameUnlocked = true;
        SaveData();
    }

    public bool IsStory2Unlocked()
    {
        return data.story2Unlocked;
    }

    public bool IsMinigameUnlocked()
    {
        return data.minigameUnlocked;
    }

    // ======== Cloud Save Placeholder ========
    private void CloudSave(string jsonData)
    {
        // TODO: Add code to send jsonData to a cloud server or itch.io API
        // Example: upload jsonData to your database when the player is online
    }

    public void CloudLoad(string jsonData)
    {
        // TODO: Add code to load jsonData from cloud and overwrite local data
        data = JsonUtility.FromJson<PlayerData>(jsonData);
        SaveData(); // update local save as backup
    }
}
