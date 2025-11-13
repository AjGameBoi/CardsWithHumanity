using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameOver : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI finalScoreText;
    public Button retryButton;
    public Button mainMenuButton;
    public Button quitButton;

    private void Start()
    {
        // Display the final score from SceneLoader
        if (finalScoreText != null)
        {
            int score = SceneLoader.GetFinalScore();
            finalScoreText.text = $"Final Score: {score}";
        }

        // Hook up buttons
        if (retryButton != null)
        {
            retryButton.onClick.AddListener(() => SceneLoader.Instance.LoadGame());
        }

        if (mainMenuButton != null)
        {
            mainMenuButton.onClick.AddListener(() => SceneLoader.Instance.LoadMainMenu());
        }

        if (quitButton != null)
        {
            quitButton.onClick.AddListener(() => SceneLoader.Instance.QuitGame());
        }
    }
}
