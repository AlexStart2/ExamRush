using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

namespace Platformer.Mechanics
{
    public class VictoryZone : MonoBehaviour
    {
        public GameObject victoryPanel;       // Assign in Inspector
        public TMP_Text messageText;          // Assign in Inspector
        public Button nextLevelButton;        // Assign in Inspector
        public Button restartButton;          // Assign in Inspector

        private void Start()
        {
            victoryPanel.SetActive(false);
        }

        void OnTriggerEnter2D(Collider2D collider)
        {
            var p = collider.GetComponent<PlayerController>();
            if (p != null && collider.CompareTag("Player"))
            {
                ShowVictoryScreen();
            }
        }

        void ShowVictoryScreen()
        {
            victoryPanel.SetActive(true);
            Time.timeScale = 0f; // Pause the game  

            int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
            int totalScenes = SceneManager.sceneCountInBuildSettings;

            if (currentSceneIndex < totalScenes - 1 && currentSceneIndex < 3)
            {
                messageText.text = "Level Complete!";
                nextLevelButton.gameObject.SetActive(true);
                nextLevelButton.onClick.RemoveAllListeners();
                nextLevelButton.onClick.AddListener(() => LoadNextLevel());
            }
            else
            {
                messageText.text = "Congratulations! You finished all levels!";
                nextLevelButton.gameObject.SetActive(false);
            }

            restartButton.onClick.RemoveAllListeners();
            restartButton.onClick.AddListener(() => RestartLevel());
        }

        void LoadNextLevel()
        {
            Time.timeScale = 1f;
            int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
            SceneManager.LoadScene(nextSceneIndex);
        }

        void RestartLevel()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
