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

    void Start()
    {
        
    }

    public void InitData(ProbeModel model)
    {
        currentModel = model;
        tog_select.isOn = false;
        txt_probeName.text = model.ProbeName;
        txt_machineName.text = model.MachineName;     
        txt_probeAddress.text = model.MailAddress;
        txt_gasKind.text = model.GasKind;
        txt_tagName.text = model.TagName;
        txt_firstAlarmValue.text = model.FirstAlarmValue.ToString();
        txt_secondAlarmValue.text = model.SecondAlarmValue.ToString();
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
