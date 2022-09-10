using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        StartCoroutine(GetAudioClip(Application.streamingAssetsPath + "/audio.wav"));
    }

    IEnumerator GetAudioClip(string path)
    {
        path = "file://" + path;
        using (var uwr = UnityWebRequestMultimedia.GetAudioClip(path, AudioType.WAV))
        {
            yield return uwr.SendWebRequest();
            if (uwr.isNetworkError)
            {
                Debug.LogError("load audioClip error:" + uwr.error);
            }
            else
            {
                AudioClip clip = DownloadHandlerAudioClip.GetContent(uwr);
                warningShoutAudioSource.clip = clip;
            }
        }
    }

    static bool isShoutWarning_ = true;
    public AudioSource warningShoutAudioSource;
    public void PlayWarningShout()
    {
        if (warningShoutAudioSource)
        {
            if (isShoutWarning_)
                warningShoutAudioSource.Play();
            else
                PauseWarningShout();
        }
    }

    public void SetIsShoutWarning(bool isShoutWarning)
    {
        isShoutWarning_ = isShoutWarning;
    }

    public void PauseWarningShout()
    {
        if (warningShoutAudioSource)
        {
            warningShoutAudioSource.Pause();
        }
    }
}
