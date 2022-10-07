using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlanarGraphItem : MonoBehaviour
{
    public Text probeNameText;

    public ProbeModel currentModel;
    public void InitBaseData(ProbeModel realtimeDataModel)
    {
        currentModel = realtimeDataModel;
        probeNameText.text = realtimeDataModel.ProbeName;
    }

    public void InitRealtimeData(ProbeModel realtimeDataModel)
    {
        currentModel.CheckTime = realtimeDataModel.CheckTime;
        currentModel.GasValue = realtimeDataModel.GasValue;
        currentModel.GasKind = realtimeDataModel.GasKind;
        color = FormatData.warningColorDic[realtimeDataModel.warningLevel];
    }

    Color color = new Color(1.0f, 1.0f, 1.0f);
    float tempTime = 0;
    float flashTime = 0.5f;
    void Update()
    {
        if (currentModel != null)
        {
            if (currentModel.warningLevel == EWarningLevel.FirstAlarm || currentModel.warningLevel == EWarningLevel.SecondAlarm)
            {
                tempTime += Time.deltaTime;
                if (tempTime >= 0 && tempTime <= flashTime)
                {
                    ColorBlock cb = GetComponent<Button>().colors;
                    cb.normalColor = color;
                    GetComponent<Button>().colors = cb;
                }
                else if (tempTime > flashTime && tempTime <= 2 * flashTime)
                {
                    ColorBlock cb = GetComponent<Button>().colors;
                    cb.normalColor = Color.white;
                    GetComponent<Button>().colors = cb;
                }
                else if (tempTime > 2 * flashTime)
                {
                    tempTime = 0;
                }
            }
            else
            {
                ColorBlock cb = GetComponent<Button>().colors;
                cb.normalColor = color;
                GetComponent<Button>().colors = cb;
            }
        }
    }
}
