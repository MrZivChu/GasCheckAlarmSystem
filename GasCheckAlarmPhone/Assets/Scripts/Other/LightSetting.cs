using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightSetting : MonoBehaviour
{
    public Light mainLight;
    public List<Light> subLights;

    bool isShowUI = false;
    float mainValue = 1.5f;
    float subValue = 0.1f;
    private void OnGUI()
    {
        if (isShowUI)
        {
            float itemWidht = 200;
            float itemHeight = 60;
            float halfWidth = Screen.width / 2;
            float halfHeight = Screen.height / 2;
            mainValue = GUI.HorizontalSlider(new Rect(halfWidth - itemWidht, halfHeight, itemWidht, itemHeight), mainValue, 0.0f, 3);
            SetMainLight(mainValue);
            subValue = GUI.HorizontalSlider(new Rect(halfWidth - itemWidht, halfHeight + itemHeight, itemWidht, itemHeight), subValue, 0.0f, 1.0f);
            SetSubLight(subValue);
            if (GUI.Button(new Rect(halfWidth - itemWidht, halfHeight + itemHeight * 2, itemWidht, itemHeight), "关闭"))
            {
                isShowUI = false;
            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F6))
        {
            isShowUI = true;
        }
    }

    public void SetMainLight(float value)
    {
        mainLight.intensity = value;
    }

    public void SetSubLight(float value)
    {
        for (int i = 0; i < subLights.Count; i++)
        {
            subLights[i].intensity = value;
        }
    }
}
