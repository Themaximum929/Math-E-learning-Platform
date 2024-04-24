using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Text;

public class ChatBot : MonoBehaviour
{
    private string endpoint = "https://fypchatbotv1.cognitiveservices.azure.com/";
    private string key = "a693c12ced8e493a86bb9d0265f7206f";
    private string projectName = "FYP";
    private string deploymentName = "FYP_v1";

    // Start is called before the first frame update
    void Start()
    {
    }

    public IEnumerator PostQuestion(string question, System.Action<KnowledgeBaseAnswers> callback)
    {
        string uri = $"{endpoint}/language/:query-knowledgebases/projects/{projectName}/deployments/{deploymentName}/knowledgebases/query?api-version=2021-10-01";

        // Create JSON object with the question
        string requestBody = JsonUtility.ToJson(new { question = question });

        using (UnityWebRequest webRequest = new UnityWebRequest(uri, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(requestBody);
            webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            webRequest.SetRequestHeader("Content-Type", "application/json");
            webRequest.SetRequestHeader("Ocp-Apim-Subscription-Key", key);

            // Send the web request and await a response
            yield return webRequest.SendWebRequest();

            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Error: {webRequest.error}");
                callback(null);
            }
            else
            {
                string jsonResponse = webRequest.downloadHandler.text;
                Debug.Log("Response: " + jsonResponse);

                KnowledgeBaseAnswers answers = JsonUtility.FromJson<KnowledgeBaseAnswers>(jsonResponse);
                callback(answers);
            }
        }
    }

    // Define a method to process and output the answers - you will need to create a class structure that matches the JSON response
    void ProcessAnswers(KnowledgeBaseAnswers answers)
    {
        foreach (var answer in answers.answers)
        {
            Debug.Log($"Score: {answer.score} Answer: {answer.answer}");
        }
    }
}

// You will need to define these classes to match the JSON response structure from Azure
[System.Serializable]
public class KnowledgeBaseAnswers
{
    public Answer[] answers;
}

[System.Serializable]
public class Answer
{
    public string answer;
    public float score;
    // Define other fields as per the JSON response from Azure
}