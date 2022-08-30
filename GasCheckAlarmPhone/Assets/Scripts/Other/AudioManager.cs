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

    bool isShoutWarning = true;
    public AudioSource warningShoutAudioSource;
    public void PlayWarningShout()
    {
        if (warningShoutAudioSource)
        {
            if (isShoutWarning)
                warningShoutAudioSource.Play();
            else
                PauseWarningShout();
        }
    }

    public void SetIsShoutWarning(bool isShoutWarning)
    {
        this.isShoutWarning = isShoutWarning;
    }

    public void PauseWarningShout()
    {
        if (warningShoutAudioSource)
        {
            warningShoutAudioSource.Pause();
        }
    }
}
