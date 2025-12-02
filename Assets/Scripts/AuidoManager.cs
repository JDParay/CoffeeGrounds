using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Sliders")]
    public Slider bgmSlider;
    public Slider sfxSlider;
    public Slider voiceSlider;

    private AudioSource bgmSource;
    private AudioSource[] sfxSources;
    private AudioSource[] voiceSources;

    private void Awake()
    {
        // Singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Initialize slider values from PlayerPrefs
        bgmSlider.value = PlayerPrefs.GetFloat("BGMVolume", 1f);
        sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume", 1f);
        voiceSlider.value = PlayerPrefs.GetFloat("VoiceVolume", 1f);

        // Add listeners
        bgmSlider.onValueChanged.AddListener(SetBGMVolume);
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);
        voiceSlider.onValueChanged.AddListener(SetVoiceVolume);

        // Find audio sources in the scene
        FindAudioSources();
    }

    private void OnEnable()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
    {
        // Re-find audio sources in new scene
        FindAudioSources();
    }

    private void FindAudioSources()
    {
        // Auto-detect BGM
        var bgmObj = GameObject.FindGameObjectsWithTag("BGM").FirstOrDefault();
        if (bgmObj != null)
            bgmSource = bgmObj.GetComponent<AudioSource>();

        // Auto-detect SFX
        sfxSources = GameObject.FindGameObjectsWithTag("SFX")
                               .Select(go => go.GetComponent<AudioSource>())
                               .Where(a => a != null)
                               .ToArray();

        // Auto-detect Voice
        voiceSources = GameObject.FindGameObjectsWithTag("Voice")
                                 .Select(go => go.GetComponent<AudioSource>())
                                 .Where(a => a != null)
                                 .ToArray();

        // Apply saved volumes immediately
        if (bgmSource != null) bgmSource.volume = bgmSlider.value;
        foreach (var sfx in sfxSources) sfx.volume = sfxSlider.value;
        foreach (var voice in voiceSources) voice.volume = voiceSlider.value;
    }

    // Slider functions
    public void SetBGMVolume(float value)
    {
        if (bgmSource != null) bgmSource.volume = value;
        PlayerPrefs.SetFloat("BGMVolume", value);
    }

    public void SetSFXVolume(float value)
    {
        foreach (var sfx in sfxSources) sfx.volume = value;
        PlayerPrefs.SetFloat("SFXVolume", value);
    }

    public void SetVoiceVolume(float value)
    {
        foreach (var voice in voiceSources) voice.volume = value;
        PlayerPrefs.SetFloat("VoiceVolume", value);
    }

    // In AudioManager
    public void PlaySFX(AudioClip clip)
    {
        if (sfxSources.Length > 0)
            sfxSources[0].PlayOneShot(clip);
    }

}
