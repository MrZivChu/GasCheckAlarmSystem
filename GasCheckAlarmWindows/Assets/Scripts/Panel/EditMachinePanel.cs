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

            int dd = dropdown_factory.value;
            FactoryModel model = factoryList[dd];
            MachineDAL.EditMachineByID(currentModel.ID, machineAddress, machineName, model.ID, model.FactoryName);
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
            int selectIndex = 0;
            for (int i = 0; i < factoryList.Count; i++)
            {
                optionList.Add(factoryList[i].FactoryName);
                if (factoryList[i].ID == model.FactoryID)
                    selectIndex = i;
            }
            dropdown_factory.AddOptions(optionList);
            dropdown_factory.value = selectIndex;
        }
    }
}
