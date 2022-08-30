﻿using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditProbePanel : UIEventHelper
{
    public ProbeModel currentModel;
    public InputField input_probeName;
    public InputField input_probeAddress;
    public InputField input_gasKind;
    public InputField input_unit;
    public InputField input_firstAlarmValue;
    public InputField input_secondAlarmValue;
    public Dropdown dropdown_deviceTag;
    public Dropdown dropdown_machine;
    public InputField input_serialNumber;

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
        if (machineList != null && machineList.Count > 0)
        {
            string probeName = input_probeName.text;
            string address = input_probeAddress.text;
            string gasKind = input_gasKind.text;
            string unit = input_unit.text;
            string firstAlarmValue = input_firstAlarmValue.text;
            string secondAlarmValue = input_secondAlarmValue.text;
            string serialNumber = input_serialNumber.text;

            int dd = dropdown_machine.value;
            MachineModel model = machineList[dd];
            WWWForm form = new WWWForm();
            form.AddField("requestType", "EditProbeByID");
            form.AddField("id", currentModel.ID);
            form.AddField("mailAddress", address);
            form.AddField("probeName", probeName);
            form.AddField("machineID", model.ID);
            form.AddField("gasKind", gasKind);
            form.AddField("unit", unit);
            form.AddField("firstAlarmValue", firstAlarmValue);
            form.AddField("secondAlarmValue", secondAlarmValue);
            form.AddField("machineName", model.MachineName);
            form.AddField("serialNumber", serialNumber);
            form.AddField("tagName", dropdown_deviceTag.captionText.text);
            GameUtils.PostHttp("Probe.ashx", form, null, null);

            MessageBox.Instance.PopOK("修改成功", () =>
            {
                EventManager.Instance.DisPatch(NotifyType.UpdateProbeList);
                gameObject.SetActive(false);
            }, "确定");
        }
    }

    List<MachineModel> machineList;
    public void InitData(ProbeModel model)
    {
        currentModel = model;
        input_probeName.text = model.ProbeName;
        input_probeAddress.text = model.MailAddress;
        input_gasKind.text = model.GasKind;
        input_unit.text = model.Unit;
        input_firstAlarmValue.text = model.FirstAlarmValue.ToString();
        input_secondAlarmValue.text = model.SecondAlarmValue.ToString();
        input_serialNumber.text = model.SerialNumber;
        InitMachine(model);
        InitDeviceTag(model);
    }

    void InitMachine(ProbeModel model)
    {
        dropdown_machine.ClearOptions();
        WWWForm form = new WWWForm();
        form.AddField("requestType", "SelectAllMachineByCondition");
        GameUtils.PostHttp("Machine.ashx", form, (result) =>
        {
            machineList = JsonMapper.ToObject<List<MachineModel>>(result);
            if (machineList != null && machineList.Count > 0)
            {
                List<string> optionList = new List<string>();
                int selectIndex = 0;
                for (int i = 0; i < machineList.Count; i++)
                {
                    optionList.Add(machineList[i].MachineName);
                    if (machineList[i].ID == model.MachineID)
                        selectIndex = i;
                }
                dropdown_machine.AddOptions(optionList);
                dropdown_machine.value = selectIndex;
            }
        }, null);
    }

    void InitDeviceTag(ProbeModel model)
    {
        dropdown_deviceTag.ClearOptions();
        WWWForm form = new WWWForm();
        form.AddField("requestType", "SelectAllDeviceTag");
        GameUtils.PostHttp("DeviceTag.ashx", form, (result) =>
        {
            List<DeviceTag> list = JsonMapper.ToObject<List<DeviceTag>>(result);
            if (list != null && list.Count > 0)
            {
                List<string> optionList = new List<string>();
                int selectIndex = 0;
                for (int i = 0; i < list.Count; i++)
                {
                    DeviceTag deviceTag = list[i];
                    List<DeviceTag> temp = list.FindAll(it => it.ParentID == deviceTag.ID);
                    if (temp.Count == 0)
                    {
                        if (deviceTag.TagName == model.TagName)
                        {
                            selectIndex = optionList.Count;
                        }
                        optionList.Add(deviceTag.TagName);
                    }
                }
                dropdown_deviceTag.AddOptions(optionList);
                dropdown_deviceTag.value = selectIndex;
            }
        }, null);
    }
}
