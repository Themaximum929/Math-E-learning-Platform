// <code>
using System;
using System.Threading;
using UnityEngine;
using Microsoft.CognitiveServices.Speech;
using System.Collections;
using System.Threading.Tasks;

public class AzureTTS : MonoBehaviour
{
    // Hook up the three properties below with a Text, InputField and Button object in your UI.
    public AudioSource audioSource;

    // Replace with your own subscription key and service region (e.g., "westus").
    private const string SubscriptionKey = "cf9cd3b039f94297b70fd8d3e4352633";
    private const string Region = "eastasia";

    private const int SampleRate = 24000;

    private object threadLocker = new object();
    private bool waitingForSpeak;
    private bool audioSourceNeedStop;
    private string message;

    private SpeechConfig speechConfig;
    private SpeechSynthesizer synthesizer;

    private async Task<SpeechSynthesisResult> SynthesizeSpeechAsync(string text)
    {
        var result = await synthesizer.StartSpeakingTextAsync(text);
        return result;
    }

    public IEnumerator ConvertTextToSpeech(string text)
    {
        lock (threadLocker)
        {
            waitingForSpeak = true;
        }

        string newMessage = null;
        var startTime = DateTime.Now;

        // Starts speech synthesis asynchronously
        var synthesisTask = SynthesizeSpeechAsync(text);

        // Wait for the synthesis task to complete in the coroutine
        while (!synthesisTask.IsCompleted)
        {
            yield return null;
        }

        // Starts speech synthesis, and returns once the synthesis is started.
        using (var result = synthesisTask.Result)
        {
            // Native playback is not supported on Unity yet (currently only supported on Windows/Linux Desktop).
            // Use the Unity API to play audio here as a short term solution.
            // Native playback support will be added in the future release.
            var audioDataStream = AudioDataStream.FromResult(result);
            var isFirstAudioChunk = true;
            var audioClip = AudioClip.Create(
                "Speech",
                SampleRate * 600, // Can speak 10mins audio as maximum
                1,
                SampleRate,
                true,
                (float[] audioChunk) =>
                {
                    var chunkSize = audioChunk.Length;
                    var audioChunkBytes = new byte[chunkSize * 2];
                    var readBytes = audioDataStream.ReadData(audioChunkBytes);
                    if (isFirstAudioChunk && readBytes > 0)
                    {
                        var endTime = DateTime.Now;
                        var latency = endTime.Subtract(startTime).TotalMilliseconds;
                        newMessage = $"Speech synthesis succeeded!\nLatency: {latency} ms.";
                        isFirstAudioChunk = false;
                    }

                    for (int i = 0; i < chunkSize; ++i)
                    {
                        if (i < readBytes / 2)
                        {
                            audioChunk[i] = (short)(audioChunkBytes[i * 2 + 1] << 8 | audioChunkBytes[i * 2]) / 32768.0F;
                        }
                        else
                        {
                            audioChunk[i] = 0.0f;
                        }
                    }

                    if (readBytes == 0)
                    {
                        Thread.Sleep(200); // Leave some time for the audioSource to finish playback
                        audioSourceNeedStop = true;
                    }
                });

            audioSource.clip = audioClip;
            audioSource.Play();

            while (audioSource.isPlaying)
            {
                yield return null; // Wait for the audio to finish playing
            }

            lock (threadLocker)
            {
                if (newMessage != null)
                {
                    message = newMessage;
                }

                waitingForSpeak = false;
            }
        }

        lock (threadLocker)
        {
            if (newMessage != null)
            {
                message = newMessage;
            }

            waitingForSpeak = false;
        }
    }

    // Stop the audio when the app quits.
    public void StopAudio()
    {
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }

    void Start()
    {
        // Creates an instance of a speech config with specified subscription key and service region.
        speechConfig = SpeechConfig.FromSubscription(SubscriptionKey, Region);
        speechConfig.SpeechSynthesisLanguage = "en-US";
        speechConfig.SpeechSynthesisVoiceName = "en-US-AnaNeural";

        // The default format is RIFF, which has a riff header.
        // We are playing the audio in memory as audio clip, which doesn't require riff header.
        // So we need to set the format to raw (24KHz for better quality).
        speechConfig.SetSpeechSynthesisOutputFormat(SpeechSynthesisOutputFormat.Raw24Khz16BitMonoPcm);

        // Creates a speech synthesizer.
        // Make sure to dispose the synthesizer after use!
        synthesizer = new SpeechSynthesizer(speechConfig, null);

        synthesizer.SynthesisCanceled += (s, e) =>
        {
            var cancellation = SpeechSynthesisCancellationDetails.FromResult(e.Result);
            message = $"CANCELED:\nReason=[{cancellation.Reason}]\nErrorDetails=[{cancellation.ErrorDetails}]\nDid you update the subscription info?";
        };

    }

    void Update()
    {
        /*
        if (Input.GetKeyDown(KeyCode.P))
        {
            StartCoroutine(ConvertTextToSpeech("Good morning, what can I help you today?"));
        }
        */
        lock (threadLocker)
        {

            if (audioSourceNeedStop)
            {
                audioSource.Stop();
                audioSourceNeedStop = false;
            }
        }
    }

    void OnDestroy()
    {
        if (synthesizer != null)
        {
            synthesizer.Dispose();
        }
    }
}

/*
public class AzureTTS : MonoBehaviour
{
    // Hook up the three properties below with a Text, InputField and Button object in your UI.
    public AudioSource audioSource;

    // Replace with your own subscription key and service region (e.g., "westus").
    private const string SubscriptionKey = "cf9cd3b039f94297b70fd8d3e4352633";
    private const string Region = "eastasia";

    private const int SampleRate = 24000;

    private object threadLocker = new object();
    private bool waitingForSpeak;
    private bool audioSourceNeedStop;
    private string message;

    private SpeechConfig speechConfig;
    private SpeechSynthesizer synthesizer;

    public void ConvertTextToSpeech(string text)
    {
        lock (threadLocker)
        {
            waitingForSpeak = true;
        }

        string newMessage = null;
        var startTime = DateTime.Now;

        // Starts speech synthesis, and returns once the synthesis is started.
        using (var result = synthesizer.StartSpeakingTextAsync(text).Result)
        {
            // Native playback is not supported on Unity yet (currently only supported on Windows/Linux Desktop).
            // Use the Unity API to play audio here as a short term solution.
            // Native playback support will be added in the future release.
            var audioDataStream = AudioDataStream.FromResult(result);
            var isFirstAudioChunk = true;
            var audioClip = AudioClip.Create(
                "Speech",
                SampleRate * 600, // Can speak 10mins audio as maximum
                1,
                SampleRate,
                true,
                (float[] audioChunk) =>
                {
                    var chunkSize = audioChunk.Length;
                    var audioChunkBytes = new byte[chunkSize * 2];
                    var readBytes = audioDataStream.ReadData(audioChunkBytes);
                    if (isFirstAudioChunk && readBytes > 0)
                    {
                        var endTime = DateTime.Now;
                        var latency = endTime.Subtract(startTime).TotalMilliseconds;
                        newMessage = $"Speech synthesis succeeded!\nLatency: {latency} ms.";
                        isFirstAudioChunk = false;
                    }

                    for (int i = 0; i < chunkSize; ++i)
                    {
                        if (i < readBytes / 2)
                        {
                            audioChunk[i] = (short)(audioChunkBytes[i * 2 + 1] << 8 | audioChunkBytes[i * 2]) / 32768.0F;
                        }
                        else
                        {
                            audioChunk[i] = 0.0f;
                        }
                    }

                    if (readBytes == 0)
                    {
                        Thread.Sleep(200); // Leave some time for the audioSource to finish playback
                        audioSourceNeedStop = true;
                    }
                });

            audioSource.clip = audioClip;
            audioSource.Play();
        }

        lock (threadLocker)
        {
            if (newMessage != null)
            {
                message = newMessage;
            }

            waitingForSpeak = false;
        }
    }

    void Start()
    {
        // Creates an instance of a speech config with specified subscription key and service region.
        speechConfig = SpeechConfig.FromSubscription(SubscriptionKey, Region);
        speechConfig.SpeechSynthesisLanguage = "en-US";
        speechConfig.SpeechSynthesisVoiceName = "en-US-AnaNeural";

        // The default format is RIFF, which has a riff header.
        // We are playing the audio in memory as audio clip, which doesn't require riff header.
        // So we need to set the format to raw (24KHz for better quality).
        speechConfig.SetSpeechSynthesisOutputFormat(SpeechSynthesisOutputFormat.Raw24Khz16BitMonoPcm);

        // Creates a speech synthesizer.
        // Make sure to dispose the synthesizer after use!
        synthesizer = new SpeechSynthesizer(speechConfig, null);

        synthesizer.SynthesisCanceled += (s, e) =>
        {
            var cancellation = SpeechSynthesisCancellationDetails.FromResult(e.Result);
            message = $"CANCELED:\nReason=[{cancellation.Reason}]\nErrorDetails=[{cancellation.ErrorDetails}]\nDid you update the subscription info?";
        };
        
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            ConvertTextToSpeech("Good morning, what can I help you today?");
        }
        lock (threadLocker)
        {

            if (audioSourceNeedStop)
            {
                audioSource.Stop();
                audioSourceNeedStop = false;
            }
        }
    }

    void OnDestroy()
    {
        if (synthesizer != null)
        {
            synthesizer.Dispose();
        }
    }
}

*/







/*
    public void Speak(string text)
    {
        SpVoice voice = new SpVoice();
        foreach (SpObjectToken token in voice.GetVoices(string.Empty, string.Empty))
        {
            if (token.GetDescription(1033).Contains("Zira"))  // Look for "Zira" in the voice descriptions
            {
                voice.Voice = token;
                break;
            }
        }
        voice.Speak(text, SpeechVoiceSpeakFlags.SVSFlagsAsync | SpeechVoiceSpeakFlags.SVSFPurgeBeforeSpeak);
    }
*/
/*
public class AudioController : MonoBehaviour
{

    [SerializeField]
    CubismAudioMouthInput audioMouthInput;

    public AudioClip AudioClip1;
    public AudioClip AudioClip2;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.M))
        {
            StartCoroutine(playAudio("Assets/Audio/audio2.mp3", stopAudio));
        }
        else if (Input.GetKey(KeyCode.N))
        {
            StartCoroutine(playAudio("Assets/Audio/audio2.mp3", stopAudio));
        }
    }

    public delegate void functionType();
    private IEnumerator Checking(functionType callback)
    {
        if (audioMouthInput?.AudioInput == null)
        {
            Debug.LogError("audioMouthInput.AudioInput no set");
            yield return null;
        }

        while (true)
        {
            yield return new WaitForFixedUpdate();

            if (!audioMouthInput.AudioInput.isPlaying)
            {
                callback();
                break;
            }
        }
    }

    private IEnumerator playAudio(string audioFilePath, functionType callback)
    {
        if (audioMouthInput?.AudioInput == null)
        {
            Debug.LogError("audioMouthInput.AudioInput no set");
            yield break;
        }

        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip("file:///" + audioFilePath, AudioType.UNKNOWN))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                AudioClip audioClip = DownloadHandlerAudioClip.GetContent(www);
                audioMouthInput.AudioInput.clip = audioClip;
                audioMouthInput.AudioInput.Play();
                StartCoroutine(Checking(callback));
                Debug.Log("play sound");
            }
            else
            {
                Debug.LogError("Failed to load audio clip: " + www.error);
            }
        }
    }
    

    private void playAudio(AudioClip audioClip, functionType callback)
    {
        if (audioMouthInput?.AudioInput == null)
        {
            Debug.LogError("audioMouthInput.AudioInput no set");
            return;
        }

        audioMouthInput.AudioInput.clip = audioClip;
        audioMouthInput.AudioInput.Play();
        StartCoroutine(Checking(callback));
        Debug.Log("play sound");
    }
    

    private void stopAudio()
    {
        Debug.Log("stop sound");
    }

}
*/