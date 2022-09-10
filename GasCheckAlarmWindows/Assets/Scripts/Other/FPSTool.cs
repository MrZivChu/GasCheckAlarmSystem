using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSTool : MonoBehaviour
{
    public int maxFrameNumber = 30;
    public bool isShowFps = false;
    private void Start()
    {
        Application.targetFrameRate = maxFrameNumber;
    }

    float timer = 0;
    int frameCount = 0;
    string result = string.Empty;
    private void Update()
    {
        if (isShowFps)
        {
            timer += Time.deltaTime;
            frameCount++;
            if (timer >= 1)
            {
                result = "FPS：" + Mathf.RoundToInt(frameCount / timer);
                frameCount = 0;
                timer = 0;
            }
        }
    }

    private void OnGUI()
    {
        if (isShowFps)
        {
            GUI.Label(new Rect(0, 0, 200, 200), result);
        }
    }
}


