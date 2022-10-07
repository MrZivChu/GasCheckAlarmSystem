using LitJson;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class MachineFactoryDataManager
{
    static Dictionary<string, MachineModel> machineDic_ = null;
    static Dictionary<string, FactoryModel> factoryDic_ = null;
    public static void Init()
    {
        EventManager.Instance.AddEventListener(NotifyType.UpdateMachineList, UpdateMachineData);
        EventManager.Instance.AddEventListener(NotifyType.UpdateFactoryList, UpdateFactoryData);
        UpdateMachineData(null);
        UpdateFactoryData(null);
    }

    static void UpdateMachineData(object data)
    {
        WWWForm form = new WWWForm();
        form.AddField("requestType", "SelectAllMachineDic");
        GameUtils.PostHttpWebRequest("Machine.ashx", form, (bytes) =>
        {
            string content = Encoding.UTF8.GetString(bytes);
            machineDic_ = JsonMapper.ToObject<Dictionary<string, MachineModel>>(content);
        }, null);
    }

    static void UpdateFactoryData(object data)
    {
        WWWForm form = new WWWForm();
        form.AddField("requestType", "SelectAllFactoryDic");
        GameUtils.PostHttpWebRequest("Factory.ashx", form, (bytes) =>
        {
            string content = Encoding.UTF8.GetString(bytes);
            factoryDic_ = JsonMapper.ToObject<Dictionary<string, FactoryModel>>(content);
        }, null);
    }

    public static MachineModel GetMachineData(int machineID)
    {
        if (machineDic_ != null && machineDic_.ContainsKey(machineID.ToString()))
        {
            return machineDic_[machineID.ToString()];
        }
        return null;
    }

    public static FactoryModel GetFactoryData(int factoryID)
    {
        if (factoryDic_ != null && factoryDic_.ContainsKey(factoryID.ToString()))
        {
            return factoryDic_[factoryID.ToString()];
        }
        return null;
    }
}
