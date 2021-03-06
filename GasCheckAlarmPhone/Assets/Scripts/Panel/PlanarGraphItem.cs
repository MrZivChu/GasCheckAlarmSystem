using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlanarGraphItem : MonoBehaviour
{
    public Text probeNameText;

    public RealtimeDataModel realtimeDataModel;
    public void InitData(RealtimeDataModel realtimeDataModel)
    {
        this.realtimeDataModel = realtimeDataModel;
        probeNameText.text = realtimeDataModel.ProbeName;
        color = FormatData.warningColorDic[realtimeDataModel.warningLevel];
    }

    Color color = new Color(1.0f, 1.0f, 1.0f);
    float tempTime = 0;
    float flashTime = 0.5f;
    void Update()
    {
        if (realtimeDataModel != null)
        {
            if (realtimeDataModel.warningLevel == 2 || realtimeDataModel.warningLevel == 1)
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
