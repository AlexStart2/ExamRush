using Platformer.Mechanics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameEnd: MonoBehaviour
{
    public static GameObject gameOverPanel;
    public static Button restartButton;
    private static PlayerController playerControler;

    public GameObject panel;
    public Button restartBtn;

    // Initialize static fields in a static method

    void Awake()
    {
        gameOverPanel = panel;
        restartButton = restartBtn;
        playerControler = FindFirstObjectByType<PlayerController>();
        if (restartButton != null)
        {
            restartButton.onClick.RemoveAllListeners();
            restartButton.onClick.AddListener(RestartScene);
        }
    }

    public void GameOver()
    {
        gameOverPanel.SetActive(true);

        if (playerControler != null)
            playerControler.controlEnabled = false;
    }

    public void RestartScene()
    {


        if (playerControler != null)
        {
            playerControler.controlEnabled = true;
            Debug.Log("restart button pressed");
        }
        else
        {
            Debug.Log("Player controller undefined");
        }

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
