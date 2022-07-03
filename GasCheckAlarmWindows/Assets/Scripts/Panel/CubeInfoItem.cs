using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CubeInfoItem : MonoBehaviour
{
    public Text probeNameText;
    public Text firstValueText;
    public Text secondValueText;
    public Text gasValueText;
    public Text gasTypeText;

    public RealtimeDataModel realtimeDataModel;
    public void InitData(RealtimeDataModel realtimeDataModel)
    {
        this.realtimeDataModel = realtimeDataModel;
        probeNameText.text = realtimeDataModel.ProbeName;
        firstValueText.text = realtimeDataModel.FirstAlarmValue.ToString();
        secondValueText.text = realtimeDataModel.SecondAlarmValue.ToString();
        gasValueText.text = realtimeDataModel.GasValue.ToString();
        gasTypeText.text = realtimeDataModel.GasKind;
        Color color = FormatData.warningColorDic[realtimeDataModel.warningLevel];
        ColorBlock cb = GetComponent<Button>().colors;
        cb.normalColor = color;
        cb.pressedColor = color;
        cb.highlightedColor = color;
        GetComponent<Button>().colors = cb;
    }
}
