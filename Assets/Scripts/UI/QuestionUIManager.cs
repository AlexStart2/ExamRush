using UnityEngine;
using UnityEngine.UI;
using static GetAIQuestions;
using TMPro;
using Platformer.Mechanics;


public class QuestionUIManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject panel;          // QuestionPanel
    public TMP_Text questionText;         // QuestionText
    public Button[] answerButtons;    // array of 4 Buttons

    private Question currentQuestion;

    void Start()
    {
        panel.SetActive(false);
    }

    public void ShowQuestion(Question q, PlayerController pc)
    {
        currentQuestion = q;
        questionText.text = q.question;
        panel.SetActive(true);

        for (int i = 0; i < answerButtons.Length; i++)
        {
            var btn = answerButtons[i];
            var idx = i;
            if (i < q.options.Count)
            {
                btn.gameObject.SetActive(true);
                btn.GetComponentInChildren<TMP_Text>().text = q.options[i];

                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(() => OnAnswerSelected(idx, pc));
            }
            else
            {
                btn.gameObject.SetActive(false);
            }
        }
    }

    void OnAnswerSelected(int index, PlayerController pc)
    {
        panel.SetActive(false);
        bool correct = (index == currentQuestion.correctIndex);

        if (pc != null) { 
            pc.controlEnabled = true; 
        }
        else {
            Debug.LogWarning("PlayerController not found on QuestionUIManager.");
        }
       

        if (correct)
        {
            pc.maxSpeed = 12.0f;
            pc.jumpTakeOffSpeed = 16.0f;
        }
        else
        {
            pc.maxSpeed = 5.0f;
            pc.jumpTakeOffSpeed = 7.0f;
        }

    }
}
