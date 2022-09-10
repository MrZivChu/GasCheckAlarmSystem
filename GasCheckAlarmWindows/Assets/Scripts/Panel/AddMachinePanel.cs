using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AddMachinePanel : UIEventHelper
{
    public InputField input_machineName;
    public InputField input_machineAddress;
    public Dropdown dropdown_factory;
    public Dropdown dropdown_protocol;

    public Button btn_cancel;
    public Button btn_ok;
    void Start()
    {
        RegisterBtnClick(btn_cancel, OnCancel);
        RegisterBtnClick(btn_ok, OnOk);
        dropdown_protocol.ClearOptions();
        dropdown_protocol.AddOptions(FormatData.machineTypeFormat);
        dropdown_protocol.value = 0;
    }

    void OnCancel(Button btn)
    {
        gameObject.SetActive(false);
    }

    void OnOk(Button btn)
    {
        string machineName = input_machineName.text;
        string machineAddress = input_machineAddress.text;
        int machineProtocol = dropdown_protocol.value;
        int dd = dropdown_factory.value;
        FactoryModel model = factoryList[dd];
        MachineDAL.InsertMachine(machineAddress, machineName, model.ID, model.FactoryName, machineProtocol);
        MessageBox.Instance.PopOK("新增成功", () =>
        {
            EventManager.Instance.DisPatch(NotifyType.UpdateMachineList);
            gameObject.SetActive(false);
        }, "确定");
    }

    List<FactoryModel> factoryList;
    private void OnEnable()
    {
        dropdown_factory.ClearOptions();
        factoryList = FactoryDAL.SelectAllFactoryByCondition();
        if (factoryList != null && factoryList.Count > 0)
        {
            List<string> optionList = new List<string>();
            for (int i = 0; i < factoryList.Count; i++)
            {
                optionList.Add(factoryList[i].FactoryName);
            }
            dropdown_factory.AddOptions(optionList);
            dropdown_factory.value = 0;
        }
    }
}
