using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CubeInfoItem : MonoBehaviour
{
    public Text probeNameText;
    public Text gasValueText;
    public Text firstValueText;
    public Text sencondValueText;
    public Text gasKindText;

    public ProbeModel currentModel;
    public void InitData(ProbeModel realtimeDataModel)
    {
        currentModel = realtimeDataModel;
        GasTypesModel gasTypesModel = FormatData.gasKindFormat[realtimeDataModel.GasKind];
        if (gasKindText)
            gasKindText.text = gasTypesModel.GasName;
        if (firstValueText)
            firstValueText.text = gasTypesModel.MinValue.ToString();
        if (sencondValueText)
            sencondValueText.text = gasTypesModel.MaxValue.ToString();
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
