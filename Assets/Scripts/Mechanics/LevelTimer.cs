using Platformer.Mechanics;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelTimer : MonoBehaviour
{
    public float timeRemaining = 120f; // 2 minutes
    public TextMeshProUGUI timerText;
    public bool timerIsRunning = true;

    void Start()
    {
        //timerIsRunning = true;
    }


    void Update()
    {
        if (timerIsRunning)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
                UpdateTimerDisplay(timeRemaining);
            }
            else
            {
                UpdateTimerDisplay(0);
                timeRemaining = 0;
                timerIsRunning = false;

                GameEnd gameEnd = FindFirstObjectByType<GameEnd>();
                if (gameEnd != null)
                {
                    gameEnd.GameOver();
                }
            }
        }
    }

    void UpdateTimerDisplay(float timeToDisplay)
    {
        int minutes = Mathf.FloorToInt(timeToDisplay / 60);
        int seconds = Mathf.FloorToInt(timeToDisplay % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
