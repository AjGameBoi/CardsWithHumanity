using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance;

    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    [Header("Music Clips")]
    public AudioClip mainMenuMusic;
    public AudioClip gameMusic;

    [Header("SFX Clips")]
    public AudioClip flipClip;
    public AudioClip matchClip;
    public AudioClip mismatchClip;
    public AudioClip gameOverClip;

    [Header("Volume Settings")]
    [Range(0f, 1f)] public float musicVolume = 1f;
    [Range(0f, 1f)] public float sfxVolume = 1f;

    [Header("Settings Panel UI")]
    public GameObject settingsPanel;
    private bool isSettingsOpen = false;

    public int finalScore = 0;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadVolumes();
        UpdateVolumes();
        PlayMusic(mainMenuMusic, loop: true);

        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }

    }
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Try to find the settings panel again in the new scene
        if (settingsPanel == null)
        {
            GameObject panelObj = GameObject.FindWithTag("SettingsPanel");
            if (panelObj != null)
            {
                settingsPanel = panelObj;
                settingsPanel.SetActive(false);
            }
        }
    }
    

    #region Music Control

    public void PlayMusic(AudioClip clip, bool loop = true)
    {
        if (musicSource == null || clip == null) return;

        if (musicSource.clip == clip && musicSource.isPlaying) return;

        musicSource.clip = clip;
        musicSource.loop = loop;
        musicSource.Play();
    }

    public void StopMusic()
    {
        if (musicSource != null && musicSource.isPlaying)
            musicSource.Stop();
    }

    #endregion

    #region SFX Control

    public void PlaySFX(AudioClip clip, float volume = 1f)
    {
        if (sfxSource != null && clip != null)
            sfxSource.PlayOneShot(clip, volume);
    }

    // Convenience methods for common SFX
    public void PlayFlip() => PlaySFX(flipClip);
    public void PlayMatch() => PlaySFX(matchClip);
    public void PlayMismatch() => PlaySFX(mismatchClip);
    public void PlayGameOver() => PlaySFX(gameOverClip);

    #endregion

    #region Settings Panel

    public void ToggleSettingsPanel()
    {
        if (settingsPanel == null) return;

        isSettingsOpen = !isSettingsOpen;
        settingsPanel.SetActive(isSettingsOpen);
    }

    public void OpenSettingsPanel()
    {
        if (settingsPanel == null) return;

        isSettingsOpen = true;
        settingsPanel.SetActive(true);
    }

    public void CloseSettingsPanel()
    {
        if (settingsPanel == null) return;

        isSettingsOpen = false;
        settingsPanel.SetActive(false);
    }

    #endregion

    #region Volume Control

    public void UpdateVolumes()
    {
        if (musicSource != null)
        {
            musicSource.volume = musicVolume;
        }        
    }

    public void SetMusicVolume(float value)
    {
        musicVolume = Mathf.Clamp01(value);
        UpdateVolumes();
    }

    public void SetSFXVolume(float value)
    {
        sfxVolume = Mathf.Clamp01(value);
        UpdateVolumes();
    }

    private void LoadVolumes()
    {
        musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
    }

    public void SaveVolumes()
    {
        PlayerPrefs.SetFloat("MusicVolume", musicVolume);
        PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
        PlayerPrefs.Save();
    }
    #endregion

    #region Scene Loading

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
        Instance?.PlayMusic(Instance.mainMenuMusic, loop: true);
    }

    public void LoadGame()
    {
        SceneManager.LoadScene("Game");
        Instance?.PlayMusic(Instance.gameMusic, loop: true);
    }

    public void LoadGameOver()
    {
        SceneManager.LoadScene("GameOver");
        Instance?.StopMusic();
    }

    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();
    }

    #endregion

    #region Score

    public static void SetFinalScore(int score)
    {
        if (Instance != null)
            Instance.finalScore = score;
    }

    public static int GetFinalScore()
    {
        return Instance != null ? Instance.finalScore : 0;
    }

    #endregion
}
