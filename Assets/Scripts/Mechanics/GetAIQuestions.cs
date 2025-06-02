using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System;
using Platformer.Mechanics;


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

    public static GetAIQuestions Instance { get; private set; }
    public GameObject loadingPanel; // Assign in the Inspector


    [Serializable]
    public class AiResponse
    {
        public Candidate[] candidates;
    }

    [Serializable]
    public class Candidate
    {
        public Content content;
    }

    [Serializable]
    public class Content
    {
        public Part[] parts;
    }

    [Serializable]
    public class Part
    {
        public string text;
    }


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
        PlayerController pc = FindFirstObjectByType<PlayerController>();
        LevelTimer levelTimer = FindFirstObjectByType<LevelTimer>();
        if (levelTimer != null)
        {
            levelTimer.timerIsRunning = false; // Disable LevelTimer if it exists
        }
        
        
        if (pc != null)
        {
            pc.controlEnabled = false; // Disable player control while loading
        }

        var envVars = SimpleEnvLoader.LoadEnvFile(".env");

        if (!envVars.TryGetValue("GAS_URL", out string apiKey))
        {
            Debug.LogError("GAS_URL not found in .env file.");
            return;
        }

        StartCoroutine(Temp(envVars["GAS_URL"]));
        //StartCoroutine(AskOllama(prompt));

        loadingPanel.SetActive(true); // Show loading panel at the start

    }

    private IEnumerator Temp(string url)
    {
        PlayerController pc = FindFirstObjectByType<PlayerController>();

        float rnd = UnityEngine.Random.Range(0.0f,2.0f);
        Debug.Log("Random number for temp: " + rnd);
        WWWForm form = new WWWForm();
        form.AddField("parameter", prompt);
        form.AddField("temp", rnd.ToString());
        //Debug.Log("Sending data to GAS: " + prompt);
        UnityWebRequest www = UnityWebRequest.Post(url, form);

        // Measure how much time it takes to send and recieve the data

        DateTime startTime = DateTime.Now;
        yield return www.SendWebRequest();
        string response = "";

        if (www.result == UnityWebRequest.Result.Success)
        {
            response = www.downloadHandler.text;


            // if in response exists "UNAVAILABLE" read data from file

            if (response.Contains("UNAVAILABLE"))
            {
                Debug.LogWarning("Response contains 'UNAVAILABLE', reading from file instead.");
                response = System.IO.File.ReadAllText("D:/TempAIResponse.txt");
            }



            var outer = JsonUtility.FromJson<AiResponse>(response);

            

            string fenced = outer.candidates[0].content.parts[0].text;

            string withoutFences = fenced
                .Replace("```json\n", "")
                .Replace("```", "")
                .Trim();

            // 4. Now parse that inner quiz JSON:
            Data = JsonUtility.FromJson<QuizData>(withoutFences);

            //Data = JsonUtility.FromJson<QuizData>(www.downloadHandler.text);
        }
        else
        {
            response = "There was an error!";
        }

        // Measure how much time it takes to send and recieve the data
        DateTime endTime = DateTime.Now;
        TimeSpan timeTaken = endTime - startTime;
        Debug.Log("Time taken to send and receive data: " + timeTaken.TotalMilliseconds + " ms");
        loadingPanel.SetActive(false); // Hide loading panel after starting the coroutine
        if (pc != null)
        {
            pc.controlEnabled = true; // Re-enable player control after loading
        }
        LevelTimer levelTimer = FindFirstObjectByType<LevelTimer>();
        if (levelTimer != null)
        {
            levelTimer.timerIsRunning = true; // Disable LevelTimer if it exists
        }
    }
    public static class QuizParser
    {
        //public static QuizData Data;

        /// <summary>
        /// Call this with the full AI response text (including ```json … ```), and it will
        /// extract the inner JSON and populate QuizParser.Data.
        /// </summary>
        public static void ParseAIResponse(string rawResponse)
        {
            // 1. Find the start of the JSON block (after ```json\n)
            const string fenceStart = "```json";
            const string fenceEnd = "```";

            int startIndex = rawResponse.IndexOf(fenceStart, StringComparison.OrdinalIgnoreCase);
            if (startIndex < 0)
            {
                Debug.LogError("Could not find ```json fence in AI response.");
                return;
            }
            startIndex = rawResponse.IndexOf('\n', startIndex) + 1;
            if (startIndex <= 0 || startIndex >= rawResponse.Length)
            {
                Debug.LogError("Malformed AI response: no newline after ```json.");
                return;
            }

            // 2. Find the closing triple backtick
            int endIndex = rawResponse.IndexOf(fenceEnd, startIndex, StringComparison.Ordinal);
            if (endIndex < 0)
            {
                Debug.LogError("Could not find closing ``` fence in AI response.");
                return;
            }

            // 3. Extract just the JSON substring
            string jsonSubstring = rawResponse.Substring(startIndex, endIndex - startIndex).Trim();

            // 4. Parse into QuizData
            try
            {
                Data = JsonUtility.FromJson<QuizData>(jsonSubstring);
                Debug.Log("Successfully parsed QuizData. Easy count: " + (Data.easy?.Count ?? 0));
            }
            catch (Exception e)
            {
                Debug.LogError("Failed to parse QuizData: " + e.Message);
            }
        }
    }

    public class PromptData
    {
        public string prompt;
    }
    IEnumerator AskOllama(string question)
    {
        PromptData data = new PromptData { prompt = question };
        string json = JsonUtility.ToJson(data);

        UnityWebRequest www = new UnityWebRequest("http://localhost:5000/ask", "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
        www.uploadHandler = new UploadHandlerRaw(bodyRaw);
        www.downloadHandler = new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/json");


        Debug.Log("Sending request to Ollama");

        //yield return www.SendWebRequest();

        Debug.Log("Request sent to Ollama");

        if (www.result == UnityWebRequest.Result.Success)
        {
            // read from file
            string response = System.IO.File.ReadAllText("D:/AIResponse.txt");

            QuizParser.ParseAIResponse(response);
        }
        else
        {
            Debug.LogError("Request failed: " + www.error);
        }

        yield return new WaitForSeconds(1f); // Wait a bit before re-enabling controls
    }

}
