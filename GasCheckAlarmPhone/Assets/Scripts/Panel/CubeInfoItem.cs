using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CubeInfoItem : MonoBehaviour
{
    public Text probeNameText;
    public Text gasValueText;

    public RealtimeDataModel realtimeDataModel;
    public void InitData(RealtimeDataModel realtimeDataModel)
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
