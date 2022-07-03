using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MyTweenAlpha : MonoBehaviour
{
    public float delay = 0;
    public float duration = 5;

    public float from = 0;
    public float to = 1;

    int textCount = 0;
    int imageCount = 0;
    Text[] textList = null;
    Image[] imageList = null;
    float speed = 0;

    bool isduce = false;
    void Start()
    {
        textList = transform.GetComponentsInChildren<Text>();
        imageList = transform.GetComponentsInChildren<Image>();
        textCount = textList.Length;
        imageCount = imageList.Length;
        speed = Mathf.Abs(from - to) / duration;

        isduce = from > to ? true : false;
        InvokeRepeating("MyUpdate", delay, 0.02f);
    }

    Text text = null;
    Image image = null;
    Color c = Color.white;

    float tempDurationTime = 0;
    float alpha = 0;
    void MyUpdate()
    {
        tempDurationTime += Time.deltaTime;
        if (tempDurationTime <= duration)
        {
            alpha = isduce ? from - speed * tempDurationTime : from + speed * tempDurationTime;
            for (int i = 0; i < textCount; i++)
            {
                text = textList[i];
                c = text.color;
                c.a = alpha;
                text.color = c;
            }
            for (int i = 0; i < imageCount; i++)
            {
                image = imageList[i];
                c = image.color;
                c.a = alpha;
                image.color = c;
            }
        }
    }
}
