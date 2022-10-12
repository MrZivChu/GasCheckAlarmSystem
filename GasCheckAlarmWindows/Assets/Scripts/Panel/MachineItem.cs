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
    public Text txt_baudRate;
    public Text txt_probeName;

    public Toggle tog_select;
    public Image img_background;

    public void InitData(MachineModel model)
    {
        currentModel = model;
        tog_select.isOn = false;
        txt_machineName.text = model.MachineName;
        txt_machineType.text = FormatData.protocolTypeFormat[(EProtocolType)MachineFactoryDataManager.GetMachineData(model.ID).ProtocolType];
        txt_machineAddress.text = model.MailAddress;
        txt_factory.text = MachineFactoryDataManager.GetFactoryData(MachineFactoryDataManager.GetMachineData(model.ID).FactoryID).FactoryName;
        txt_baudRate.text = model.BaudRate.ToString();
        txt_probeName.text = model.PortName;
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
