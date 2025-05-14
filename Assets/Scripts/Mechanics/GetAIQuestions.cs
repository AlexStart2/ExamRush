using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;


public class GetAIQuestions : MonoBehaviour
{
    private string prompt = "Generate a JSON object with three properties: \n" +
"- 'easy': an array of 3 question objects.\n" +
"- 'normal': an array of 6 question objects.\n" +
"- 'hard': an array of 10 question objects.\n" +
"Each question object should look like: \n"+
"{ " +
"'question': '…',\n"+
"'options': ['optionA', 'optionB', 'optionC', 'optionD'],\n"+
"'correctIndex': < integer 0–3 >" +
"}\n" +
"All questions must be about computer science topics.\n"+
"Respond with* only* the JSON. No other text.";

    [Serializable]
    public class Question
    {
        public string question;
        public List<string> options;
        public int correctIndex;
    }

    [Serializable]
    public class QuizData
    {
        public List<Question> easy;
        public List<Question> normal;
        public List<Question> hard;
    }

    public static QuizData Data;

    void Awake()
    {
        var envVars = SimpleEnvLoader.LoadEnvFile(".env");

        if (envVars.TryGetValue("GAS_URL", out string apiKey))
        {
            Debug.Log("GAS_URL: " + apiKey);
        }

        //StartCoroutine(Temp(apiUrl));
    }
    private IEnumerator Temp(string url)
    {
        WWWForm form = new WWWForm();
        form.AddField("parameter", prompt);
        Debug.Log("Sending data to GAS: " + prompt);
        UnityWebRequest www = UnityWebRequest.Post(url, form);

        // Measure how much time it takes to send and recieve the data

        DateTime startTime = DateTime.Now;
        yield return www.SendWebRequest();
        string response = "";

        if (www.result == UnityWebRequest.Result.Success)
        {
            response = www.downloadHandler.text;
            Data = JsonUtility.FromJson<QuizData>(www.downloadHandler.text);
        }
        else
        {
            response = "There was an error!";
        }

        // Measure how much time it takes to send and recieve the data
        DateTime endTime = DateTime.Now;
        TimeSpan timeTaken = endTime - startTime;
        Debug.Log("Time taken to send and receive data: " + timeTaken.TotalMilliseconds + " ms");

        // Write the response to a file

        string filePath = "D:/AIResponse.txt";

        System.IO.File.WriteAllText(filePath, response);
    }
}
