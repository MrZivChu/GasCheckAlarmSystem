using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    private void Awake()
    {
        instance = this;
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
