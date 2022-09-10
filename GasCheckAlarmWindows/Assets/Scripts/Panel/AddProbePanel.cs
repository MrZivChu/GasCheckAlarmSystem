﻿using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AddProbePanel : UIEventHelper
{
    public InputField input_probeName;
    public InputField input_probeAddress;
    public Dropdown dropdown_gasKind;
    public Dropdown dropdown_unit;
    public InputField input_firstAlarmValue;
    public InputField input_secondAlarmValue;
    public Dropdown dropdown_machine;
    public Dropdown dropdown_deviceTag;
    public InputField input_serialNumber;

    [HideInInspector]
    public Vector3 position = Vector3.zero;
    [HideInInspector]
    public Vector3 direction = Vector3.zero;

    public Button btn_cancel;
    public Button btn_ok;

    void Start()
    {
        RegisterBtnClick(btn_cancel, OnCancel);
        RegisterBtnClick(btn_ok, OnOk);

        dropdown_gasKind.AddOptions(FormatData.gasKindList);
        dropdown_gasKind.value = 0;
        dropdown_unit.AddOptions(FormatData.unitList);
        dropdown_unit.value = 0;

        dropdown_gasKind.onValueChanged.AddListener(GasKindOnValueChanged);
        GasKindOnValueChanged(0);
    }

    void GasKindOnValueChanged(int value)
    {
        if (value == 0)
        {
            input_firstAlarmValue.text = "5";
            input_secondAlarmValue.text = "15";
            dropdown_unit.value = 0;
        }
        else if (value == 1)
        {
            input_firstAlarmValue.text = "24";
            input_secondAlarmValue.text = "100";
            dropdown_unit.value = 1;
        }
    }

    void OnCancel(Button btn)
    {
        gameObject.SetActive(false);
    }

    void OnOk(Button btn)
    {
        string probeName = input_probeName.text;
        string address = input_probeAddress.text;
        string gasKind = FormatData.gasKindList[dropdown_gasKind.value];
        string unit = FormatData.unitList[dropdown_unit.value];
        string firstAlarmValue = input_firstAlarmValue.text;
        string secondAlarmValue = input_secondAlarmValue.text;
        string serialNumber = input_serialNumber.text;
        MachineModel model = machineList[dropdown_machine.value];
        string posDir = position.x.ToString("0.00") + "," + position.y.ToString("0.00") + "," + position.z.ToString("0.00") + ";" + direction.x.ToString("0.00") + "," + direction.y.ToString("0.00") + "," + direction.z.ToString("0.00");

        ProbeDAL.InsertProbe(address, probeName, gasKind, unit, firstAlarmValue, secondAlarmValue, posDir, model.ID, model.MachineName, model.FactoryID, model.FactoryName, model.MachineType, dropdown_deviceTag.captionText.text, serialNumber);
        MessageBox.Instance.PopOK("新增成功", () =>
        {
            EventManager.Instance.DisPatch(NotifyType.UpdateProbeList);
            gameObject.SetActive(false);
        }, "确定");
    }

    List<MachineModel> machineList;
    private void OnEnable()
    {
        InitDeviceTag();
        InitMachine();
    }

    void InitDeviceTag()
    {
        dropdown_deviceTag.ClearOptions();
        List<DeviceTagModel> list = DeviceTagDAL.SelectAllDeviceTag();
        if (list != null && list.Count > 0)
        {
            List<string> optionList = new List<string>();
            for (int i = 0; i < list.Count; i++)
            {
                DeviceTagModel deviceTag = list[i];
                List<DeviceTagModel> temp = list.FindAll(it => it.ParentID == deviceTag.ID);
                if (temp.Count == 0)
                {
                    optionList.Add(deviceTag.TagName);
                }
            }
            dropdown_deviceTag.AddOptions(optionList);
            dropdown_deviceTag.value = 0;
        }
    }

    void InitMachine()
    {
        dropdown_machine.ClearOptions();
        machineList = MachineDAL.SelectAllMachineByCondition();
        if (machineList != null && machineList.Count > 0)
        {
            List<string> optionList = new List<string>();
            for (int i = 0; i < machineList.Count; i++)
            {
                optionList.Add(machineList[i].MachineName);
            }
            dropdown_machine.AddOptions(optionList);
            dropdown_machine.value = 0;
        }
    }

}
