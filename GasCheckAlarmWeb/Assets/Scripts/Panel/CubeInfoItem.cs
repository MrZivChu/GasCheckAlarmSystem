using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CubeInfoItem : MonoBehaviour
{
    public Text probeNameText;
    public Text gasValueText;

    public ProbeModel realtimeDataModel;
    public void InitData(ProbeModel realtimeDataModel)
    {
        this.realtimeDataModel = realtimeDataModel;
        probeNameText.text = realtimeDataModel.ProbeName;
        gasValueText.text = realtimeDataModel.GasValue.ToString();
        Color color = FormatData.warningColorDic[realtimeDataModel.warningLevel];
        ColorBlock cb = GetComponent<Button>().colors;
        cb.normalColor = color;
        cb.pressedColor = color;
        cb.highlightedColor = color;
        GetComponent<Button>().colors = cb;
    }
}
