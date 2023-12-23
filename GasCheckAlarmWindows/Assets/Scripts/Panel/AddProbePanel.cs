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
    public Dropdown dropdown_machine;
    public Dropdown dropdown_deviceTag;
    public InputField input_serialNumber;

    [HideInInspector]
    public Vector3 position = Vector3.zero;
    [HideInInspector]
    public Vector3 direction = Vector3.zero;

    public Button btn_cancel;
    public Button btn_ok;
    public Button reduceBtn;
    public Button addBtn;

    void Start()
    {
        RegisterBtnClick(btn_cancel, OnCancel);
        RegisterBtnClick(btn_ok, OnOk);
        RegisterBtnClick(reduceBtn, OnReduce);
        RegisterBtnClick(addBtn, OnAdd);
    }

    void OnAdd(Button btn)
    {
        string content = input_probeAddress.text;
        int result = 0;
        if (int.TryParse(content, out result))
        {
            result = result + 1;
            input_probeAddress.text = result.ToString().PadLeft(4, '0');
        }
    }

    void OnReduce(Button btn)
    {
        string content = input_probeAddress.text;
        int result = 0;
        if (int.TryParse(content, out result))
        {
            result = result - 1;
            input_probeAddress.text = result.ToString().PadLeft(4, '0');
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
        string serialNumber = input_serialNumber.text;
        MachineModel model = machineList[dropdown_machine.value];
        string pos3D = position.x.ToString("0.00") + "," + position.y.ToString("0.00") + "," + position.z.ToString("0.00") + ";" + direction.x.ToString("0.00") + "," + direction.y.ToString("0.00") + "," + direction.z.ToString("0.00");
        int gaskindID = 0;
        foreach (var item in FormatData.gasKindFormat)
        {
            if (item.Value.GasName == dropdown_gasKind.captionText.text)
            {
                gaskindID = item.Value.ID;
                break;
            }
        }
        ProbeDAL.InsertProbe(address, probeName, gaskindID, model.ID, pos3D, dropdown_deviceTag.captionText.text, serialNumber);
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
        InitGasKind();
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
            dropdown_deviceTag.RefreshShownValue();
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
            dropdown_machine.RefreshShownValue();
        }
    }

    void InitGasKind()
    {
        dropdown_gasKind.ClearOptions();
        foreach (var item in FormatData.gasKindFormat)
        {
            Dropdown.OptionData data = new Dropdown.OptionData(item.Value.GasName);
            dropdown_gasKind.options.Add(data);
        }
        dropdown_gasKind.RefreshShownValue();
    }

}
