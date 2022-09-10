using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RealtimeDataItem : UIEventHelper
{
    public RealtimeDataModel currentModel;
    public Text txt_probeName;
    public Text txt_machineName;
    public Text txt_gasKind;
    public Text txt_gasValue;
    public Text txt_firstAlarmValue;
    public Text txt_secondAlarmValue;

    public Image img_background;

    public void InitData(RealtimeDataModel model)
    {
        currentModel = model;
        txt_probeName.text = model.ProbeName;
        txt_machineName.text = model.MachineName;
        txt_gasKind.text = model.GasKind;
        txt_gasValue.text = FormatData.GetGasValue(model.MachineType, Convert.ToSingle(model.GasValue));
        txt_firstAlarmValue.text = model.FirstAlarmValue.ToString();
        txt_secondAlarmValue.text = model.SecondAlarmValue.ToString();
    }

    public void SetBackgroundColor(Color color)
    {
        img_background.color = color;
    }
}
