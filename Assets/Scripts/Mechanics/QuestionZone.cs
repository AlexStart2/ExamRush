using Platformer.Mechanics;
using UnityEngine;

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

        GetAIQuestions.Question q =  GetAIQuestions.Data.easy[0];

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

        GetAIQuestions.Data.easy.RemoveAt(0);
    }
}
