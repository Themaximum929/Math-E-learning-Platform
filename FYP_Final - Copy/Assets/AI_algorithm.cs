using Azure;
using Azure.AI.OpenAI;
using Azure.Core;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using System;
using System.Runtime.CompilerServices;

public class AI_algorithm : MonoBehaviour
{
    private Coroutine currentCoroutine;

    private OpenAIClient client;
    private void Start()
    {
        client = new OpenAIClient(
            new Uri("https://fypopenaiservices.openai.azure.com/"),
            new AzureKeyCredential("b5b116cbee5f40f29606a021a52299ae"));
    }

    public IEnumerator AI_responseCoroutine(string input, Action<string> callback)
    {
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }

        Task<string> task = AI_response(input);
        yield return new UnityEngine.WaitUntil(() => task.IsCompleted);

        if (task.Exception != null)
        {
            // Handle the exception if the task failed
            Debug.LogError("Task failed with exception: " + task.Exception);
        }
        else
        {
            // If the task completed successfully, invoke the callback with the result
            callback(task.Result);
        }
        currentCoroutine = null;
    }

    public async Task<string> AI_response(string input)
    {
        try
        {
            var chatCompletionsOptions = new ChatCompletionsOptions()
            {
                DeploymentName = "FYP", // Use DeploymentName for "model" with non-Azure clients
                Messages =
            {
                new ChatRequestSystemMessage("You are a helpful assistant. You will talk like a primary school math teacher."),
                new ChatRequestAssistantMessage("Sure! Of course! What can I do for you?"),
                new ChatRequestUserMessage(input),
            }
            };

            Response<ChatCompletions> response = await client.GetChatCompletionsAsync(chatCompletionsOptions);

            if (response?.Value?.Choices != null && response.Value.Choices.Count > 0)
            {
                ChatResponseMessage responseMessage = response.Value.Choices[0].Message;
                string temp = $"[{responseMessage.Role.ToString().ToUpperInvariant()}]: {responseMessage.Content}";
                return temp;
            }
            else
            {
                // Handle the case where response is null or Choices are empty
                return "Error: The response from the AI was not as expected.";
            }
        }
        catch (Exception ex)
        {
            // Handle any other exceptions that may occur
            Debug.LogError($"An exception occurred: {ex.Message}");
            return $"Error: An exception occurred: {ex.Message}";
        }
    }
}
/*
public class AI_algorithm 
{
    public IEnumerator AI_response(string input, System.Action<string> callback)
    {
        string url = "http://localhost:5000/predict";
        var request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes("{\"prompt\":\"" + input + "\"}");
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            UnityEngine.Debug.Log(request.error);
        }
        else
        {
            callback(request.downloadHandler.text);
        }
    }
}
*/


/*
public class AI_algorithm
{

    public string AI_response(string input)
    {
        string url = "http://localhost:5000/predict";
        var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
        httpWebRequest.ContentType = "application/json";
        httpWebRequest.Method = "POST";

        using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
        {
            string json = "{\"prompt\":\"" + input + "\"}";
            streamWriter.Write(json);
        }

        var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
        using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
        {
            var result = streamReader.ReadToEnd();
            return result;
        }
    }
}
*/