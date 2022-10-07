﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProbeHistoryDataItem : UIEventHelper
{
    public HistoryDataModel currentModel;
    public Text txt_index;
    public Text txt_probeName;
    public Text txt_machineName;
    public Text txt_factoryName;
    public Text txt_gasKind;
    public Text txt_gasValue;
    public Text txt_firstAlarmValue;
    public Text txt_secondAlarmValue;
    public Text txt_checkTime;

    public Toggle tog_select;
    public Image img_background;

    void Start()
    {
    }

    public void InitData(int index, HistoryDataModel model)
    {
        currentModel = model;
        txt_index.text = index.ToString();
        txt_probeName.text = model.probeName;
        txt_gasKind.text = FormatData.gasKindFormat[currentModel.gasKind].name;
        txt_gasValue.text = FormatData.GetGasValue(MachineFactoryDataManager.GetMachineData(model.MachineID).ProtocolType, model.gasKind, Convert.ToSingle(model.GasValue));
        txt_firstAlarmValue.text = FormatData.gasKindFormat[model.gasKind].minValue.ToString();
        txt_secondAlarmValue.text = FormatData.gasKindFormat[model.gasKind].maxValue.ToString();
        txt_checkTime.text = model.CheckTime.ToString("MM-dd HH:mm:ss");
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
