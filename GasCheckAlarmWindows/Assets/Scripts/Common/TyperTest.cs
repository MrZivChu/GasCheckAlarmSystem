using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class TyperTest : MonoBehaviour
{
    public float charsPerSeconds = 0.2f;
    private string content;
    private Text textTest;
    private float timer;
    private int currentPos;
    private bool isActive;
    void Start()
    {
        textTest = GetComponent<Text>();
        textTest.text = string.Empty;
        charsPerSeconds = Mathf.Max(0.2f, charsPerSeconds);
        timer = charsPerSeconds;
        currentPos = 0;
    }

    void Update()
    {
        if (isActive == true)
        {
            StartTyperEffect();
        }
    }

    public void TyperEffect(string _content, Action<GameObject> onFinish)
    {
        content = _content;
        onFinishCallBack = onFinish;
        isActive = true;
    }

    private void StartTyperEffect()
    {
        timer += Time.deltaTime;
        if (timer > charsPerSeconds)
        {
            timer -= charsPerSeconds;
            currentPos++;
            textTest.text = content.Substring(0, currentPos);
            if (currentPos >= content.Length)
            {
                FinishTyperEffect();
            }
        }
    }

    public Action<GameObject> onFinishCallBack;
    private void FinishTyperEffect()
    {
        isActive = false;
        timer = charsPerSeconds;
        currentPos = 0;
        textTest.text = content;
        if (onFinishCallBack != null)
            onFinishCallBack(gameObject);
    }
}