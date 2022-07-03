using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightSetting : MonoBehaviour
{
    public Light mainLight;
    public List<Light> subLights;

    public static LightSetting instance;

    private void Awake()
    {
        instance = this;
    }

    string mainValue = string.Empty;
    string subValue = string.Empty;
    private void OnGUI()
    {
        if (JsonHandleHelper.gameConfig.isSetLightByUI)
        {
            mainValue = GUI.TextField(new Rect(200, 200, 300, 100), mainValue);
            float getMainValue = 1.5f;
            if (float.TryParse(mainValue, out getMainValue))
            {
                SetMainLight(getMainValue);
            }
            subValue = GUI.TextField(new Rect(200, 300, 300, 100), subValue);
            float getSubValue = 0.3f;
            if (float.TryParse(subValue, out getSubValue))
            {
                SetSubLight(getSubValue);
            }
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
