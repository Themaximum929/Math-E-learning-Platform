using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioDirector : MonoBehaviour
{
    public AzureTTS audioController;

    public void talk_director(string text)
    {
        StartCoroutine(audioController.ConvertTextToSpeech(text));
    }

    public void stop_director()
    {
        audioController.StopAudio();
    }
}
