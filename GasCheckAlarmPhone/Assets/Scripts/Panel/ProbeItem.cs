using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProbeItem : UIEventHelper
{
    public ProbeModel currentModel;
    public Text txt_probeName;
    public Text txt_tagName;
    public Text txt_machineName;
    public Text txt_probeAddress;
    public Text txt_gasKind;
    public Text txt_firstAlarmValue;
    public Text txt_secondAlarmValue;

    public Toggle tog_select;
    public Image img_background;

    public void InitData(ProbeModel model)
    {
        currentModel = model;
        tog_select.isOn = false;
        txt_probeName.text = model.ProbeName;
        txt_machineName.text = MachineFactoryDataManager.GetMachineData(model.MachineID).MachineName;
        txt_probeAddress.text = model.MailAddress;
        txt_gasKind.text = FormatData.gasKindFormat[currentModel.GasKind].name;
        txt_tagName.text = model.TagName;
        txt_firstAlarmValue.text = FormatData.gasKindFormat[model.GasKind].minValue.ToString();
        txt_secondAlarmValue.text = FormatData.gasKindFormat[model.GasKind].maxValue.ToString();
    }

    public void SetToggle(bool isOn)
    {
        tog_select.isOn = isOn;
    }

    public bool GetToggleStatus()
    {
        return tog_select.isOn;
    }

    public void SetBackgroundColor(Color color)
    {
        img_background.color = color;
    }
}
