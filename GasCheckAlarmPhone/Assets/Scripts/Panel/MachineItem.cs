using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MachineItem : UIEventHelper
{
    public MachineModel currentModel;
    public Text txt_machineName;
    public Text txt_machineType;
    public Text txt_machineAddress;
    public Text txt_factory;

    public Toggle tog_select;
    public Image img_background;

    void Start()
    {

    }

    public void InitData(MachineModel model)
    {
        currentModel = model;
        tog_select.isOn = false;
        txt_machineName.text = model.MachineName;
        txt_machineType.text = FormatData.machineTypeFormat[model.MachineType];
        txt_machineAddress.text = model.MailAddress;
        txt_factory.text = model.FactoryName;
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
