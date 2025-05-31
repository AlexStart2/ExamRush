using Platformer.Mechanics;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class QuestionZone : MonoBehaviour
{

    bool triggered = false;
    public QuestionUIManager uiManager;

    void OnTriggerEnter2D(Collider2D other)
    {
        
        if (triggered) return;

        if(GetAIQuestions.Data == null)
        {
            Debug.LogError("AI Questions data is not loaded yet.");
            return;
        }
        else if (GetAIQuestions.Data.easy.Count == 0)
        {
            Debug.LogError("No questions available in the AI Questions data.");
            return;
        }

        // Find the name of the active scene view and assign it to level

        GetAIQuestions.Question q;

        switch (SceneManager.GetActiveScene().name)
        {
            case "Level 1":
                q = GetAIQuestions.Data.easy[0];
                GetAIQuestions.Data.easy.RemoveAt(0);
                break;
            case "Level 2":
                q = GetAIQuestions.Data.normal[0];
                GetAIQuestions.Data.normal.RemoveAt(0);
                break;
            case "Level 3":
                q = GetAIQuestions.Data.hard[0];
                GetAIQuestions.Data.hard.RemoveAt(0);
                break;
            default:
                Debug.LogError("Unknown scene name: " + SceneManager.GetActiveScene().name);
                return;
        }



        var pc = other.GetComponent<PlayerController>();
        

        if (other.CompareTag("Player"))
        {
            triggered = true;

            if (uiManager != null)
            {
                if (pc != null){
                    pc.controlEnabled = false; // Disable player control
                }
                else
                {
                    Debug.LogWarning("PlayerController not found on QuestionZone.");
                }
                uiManager.ShowQuestion(q, pc);
            }
        }      
    }
}
