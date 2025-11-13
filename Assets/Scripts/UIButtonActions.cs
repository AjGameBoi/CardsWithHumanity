using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIButtonActions : MonoBehaviour
{
    public void PlayGame()
    {
        SceneLoader.Instance.LoadGame();
    }

    public void MainMenu()
    {
        SceneLoader.Instance.LoadMainMenu();
    }

    public void GameOver()
    {
        SceneLoader.Instance.LoadGameOver();
    }

    public void QuitGame()
    {
        SceneLoader.Instance.QuitGame();
    }
}
