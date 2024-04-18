using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RealtimeDataItem : UIEventHelper
{
    public ProbeModel currentModel;
    public Text txt_probeName;
    public Text txt_machineName;
    public Text txt_gasKind;
    public Text txt_gasValue;
    public Text txt_firstAlarmValue;
    public Text txt_secondAlarmValue;

    public Image img_background;

    public void InitInfo(ProbeModel model)
    {
        currentModel = model;
        txt_probeName.text = model.ProbeName;
        txt_machineName.text = MachineFactoryDataManager.GetMachineData(model.MachineID).MachineName;
        txt_gasKind.text = FormatData.gasKindFormat[currentModel.GasKind].GasName;
        txt_gasValue.text = currentModel.GasValue.ToString();
        txt_firstAlarmValue.text = FormatData.gasKindFormat[currentModel.GasKind].MinValue.ToString();
        txt_secondAlarmValue.text = FormatData.gasKindFormat[currentModel.GasKind].MaxValue.ToString();
    }

    public void SetBackgroundColor(Color color)
    {
        img_background.color = color;
    }
}
