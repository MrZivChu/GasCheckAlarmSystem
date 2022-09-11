using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
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

    [HideInInspector]
    public Vector3 position = Vector3.zero;
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
        int dd = dropdown_machine.value;
        MachineModel model = machineList[dd];
        string posDir = position.x.ToString("0.00") + "," + position.y.ToString("0.00") + "," + position.z.ToString("0.00") + ";" + direction.x.ToString("0.00") + "," + direction.y.ToString("0.00") + "," + direction.z.ToString("0.00");

        WWWForm form = new WWWForm();
        form.AddField("requestType", "InsertProbe");
        form.AddField("mailAddress", address);
        form.AddField("probeName", probeName);
        form.AddField("gasKind", gasKind);
        form.AddField("unit", unit);
        form.AddField("firstAlarmValue", firstAlarmValue);
        form.AddField("secondAlarmValue", secondAlarmValue);
        form.AddField("posdir", posDir);
        form.AddField("machineID", model.ID);
        form.AddField("machineName", model.MachineName);
        form.AddField("factoryID", model.FactoryID);
        form.AddField("factoryName", model.FactoryName);
        form.AddField("machineType", model.MachineType);
        GameUtils.PostHttpWebRequest("Probe.ashx", form, (result) =>
        {
            string content = Encoding.UTF8.GetString(result);
            int insertID = Convert.ToInt32(content);
            MessageBox.Instance.PopOK("新增成功", () =>
            {
                EventManager.Instance.DisPatch(NotifyType.UpdateProbeList);
                gameObject.SetActive(false);

                WWWForm form1 = new WWWForm();
                form1.AddField("requestType", "SelectProbeByID");
                form1.AddField("id", insertID);
                GameUtils.PostHttpWebRequest("Probe.ashx", form1, (result1) =>
                {
                    string content1 = Encoding.UTF8.GetString(result1);
                    ProbeModel probeModel = JsonMapper.ToObject<ProbeModel>(content1);
                    ProbeInSceneHelper.instance.SpawnProbe(probeModel);
                }, null);

            }, "确定");
        }, null);


    }

    List<MachineModel> machineList;
    private void OnEnable()
    {
        dropdown_machine.ClearOptions();

        WWWForm form = new WWWForm();
        form.AddField("requestType", "SelectAllMachineByCondition");
        GameUtils.PostHttpWebRequest("Machine.ashx", form, (result) =>
        {
            string content = Encoding.UTF8.GetString(result);
            machineList = JsonMapper.ToObject<List<MachineModel>>(content);
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
        }, null);
    }
}
