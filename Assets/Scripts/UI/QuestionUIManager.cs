using UnityEngine;
using UnityEngine.UI;
using static GetAIQuestions;
using TMPro;
using Platformer.Mechanics;
using System.Collections;



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
        bool correct = (index == currentQuestion.correctIndex);
        StartCoroutine(HandleAnswerFeedback(index, correct, pc));
    }

    IEnumerator HandleAnswerFeedback(int index, bool correct, PlayerController pc)
    {
        // Get the selected button and its Image
        var btn = answerButtons[index];
        var btnCorrect = answerButtons[currentQuestion.correctIndex];
        var btnImage = btn.GetComponent<Image>();
        var btnCorrectImage = btnCorrect.GetComponent<Image>();

        // Save original color
        Color originalColor = btnImage.color;

        // Set to green if correct, red if wrong
        btnCorrectImage.color = Color.Lerp(originalColor, Color.green, 0.5f);

        if (!correct)
        {
            btnImage.color =  Color.Lerp(originalColor, Color.red, 0.5f);
        }
       

        // Wait for 1 second
        yield return new WaitForSeconds(1f);

        // Reset button color
        btnImage.color = originalColor;
        btnCorrectImage.color = originalColor;

        // Hide panel
        panel.SetActive(false);

        // Enable player control
        if (pc != null)
        {
            pc.controlEnabled = true;
            pc.maxSpeed = correct ? 12.0f : 6.0f;
            pc.jumpTakeOffSpeed = correct ? 16.0f : 8.0f;
        }
        else
        {
            Debug.LogWarning("PlayerController not found on QuestionUIManager.");
        }
    }

}
