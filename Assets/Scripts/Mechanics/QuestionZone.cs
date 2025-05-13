using UnityEngine;

public class QuestionZone : MonoBehaviour
{
    bool triggered = false;
    void OnTriggerEnter2D(Collider2D other)
    {

        if (triggered) return;

        Debug.Log("Trigger entered");

        triggered = true;

        ////
        //if (other.CompareTag("Player"))
        //{
        //    triggered = true;
        // 
        //    if (uiManager != null)
        //        uiManager.ShowQuestion(questionId);
        //}
    }
}
