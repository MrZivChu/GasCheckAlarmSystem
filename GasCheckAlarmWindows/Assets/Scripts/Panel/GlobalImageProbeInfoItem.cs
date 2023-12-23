using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GlobalImageProbeInfoItem : MonoBehaviour
{
    public Text probeNameText;
    public Text gasValueText;

    public ProbeModel currentModel;
    public void InitData(ProbeModel realtimeDataModel)
    {
        currentModel = realtimeDataModel;
        probeNameText.text = currentModel.ProbeName;
        gasValueText.text = currentModel.GasValue.ToString();
        Color color = FormatData.warningColorDic[currentModel.warningLevel];
        ColorBlock cb = GetComponent<Button>().colors;
        cb.normalColor = color;
        cb.pressedColor = color;
        cb.highlightedColor = color;
        GetComponent<Button>().colors = cb;
    }
}
