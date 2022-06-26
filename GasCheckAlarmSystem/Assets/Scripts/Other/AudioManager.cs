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

    public AudioSource warningShoutAudioSource;
    public void PlayWarningShout()
    {
        if (warningShoutAudioSource)
        {
            if (MainPanel.instance.isShoutWarning)
                warningShoutAudioSource.Play();
            else
                PauseWarningShout();
        }
    }

    public void PauseWarningShout()
    {
        if (warningShoutAudioSource)
        {
            warningShoutAudioSource.Pause();
        }
    }
}
