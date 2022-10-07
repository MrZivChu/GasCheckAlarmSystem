using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditMachinePanel : UIEventHelper
{
    public MachineModel currentModel;
    public InputField input_machineName;
    public InputField input_machineAddress;
    public Dropdown dropdown_factory;
    public Dropdown dropdown_protocol;
    public Dropdown dropdown_baundRate;

    public Button btn_cancel;
    public Button btn_ok;
    void Start()
    {
        RegisterBtnClick(btn_cancel, OnCancel);
        RegisterBtnClick(btn_ok, OnOk);
    }

    void OnCancel(Button btn)
    {
        gameObject.SetActive(false);
    }

    void OnOk(Button btn)
    {
        if (factoryList != null && factoryList.Count > 0)
        {
            string machineName = input_machineName.text;
            string machineAddress = input_machineAddress.text;
            FactoryModel model = factoryList[dropdown_factory.value];
            MachineDAL.EditMachineByID(currentModel.ID, machineAddress, machineName, model.ID, dropdown_protocol.value, dropdown_baundRate.value);
            MessageBox.Instance.PopOK("修改成功", () =>
            {
                EventManager.Instance.DisPatch(NotifyType.UpdateMachineList);
                gameObject.SetActive(false);
            }, "确定");
        }
    }

    List<FactoryModel> factoryList;
    public void InitData(MachineModel model)
    {
        currentModel = model;
        input_machineName.text = model.MachineName;
        input_machineAddress.text = model.MailAddress;
        dropdown_factory.ClearOptions();

        factoryList = FactoryDAL.SelectAllFactoryByCondition();
        if (factoryList != null && factoryList.Count > 0)
        {
            List<string> optionList = new List<string>();
            int selectIndex1 = 0;
            for (int i = 0; i < factoryList.Count; i++)
            {
                optionList.Add(factoryList[i].FactoryName);
                if (factoryList[i].ID == model.FactoryID)
                    selectIndex1 = i;
            }
            dropdown_factory.AddOptions(optionList);
            dropdown_factory.value = selectIndex1;
            dropdown_factory.RefreshShownValue();
        }

        dropdown_protocol.ClearOptions();
        int index = 0;
        int selectIndex = 0;
        foreach (var item in FormatData.protocolTypeFormat)
        {
            if (model.ProtocolType == index)
            {
                selectIndex = index;
            }
            Dropdown.OptionData data = new Dropdown.OptionData(item.Value);
            dropdown_protocol.options.Add(data);
            index++;
        }
        dropdown_protocol.value = selectIndex;
        dropdown_protocol.RefreshShownValue();

        dropdown_baundRate.ClearOptions();
        index = 0;
        foreach (var item in FormatData.baudRateFormat)
        {
            if (model.BaudRate == index)
            {
                selectIndex = index;
            }
            Dropdown.OptionData data = new Dropdown.OptionData(item.ToString());
            dropdown_baundRate.options.Add(data);
            index++;
        }
        dropdown_baundRate.value = selectIndex;
        dropdown_baundRate.RefreshShownValue();
    }
}
