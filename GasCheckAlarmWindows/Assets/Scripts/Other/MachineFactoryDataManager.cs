using System.Collections.Generic;

public class MachineFactoryDataManager
{
    static Dictionary<int, MachineModel> machineDic_ = null;
    static Dictionary<int, FactoryModel> factoryDic_ = null;
    public static void Init()
    {
        EventManager.Instance.AddEventListener(NotifyType.UpdateMachineList, UpdateMachineData);
        EventManager.Instance.AddEventListener(NotifyType.UpdateFactoryList, UpdateFactoryData);
        UpdateMachineData(null);
        UpdateFactoryData(null);
    }

    static void UpdateMachineData(object data)
    {
        machineDic_ = MachineDAL.SelectAllMachineDic();
    }

    static void UpdateFactoryData(object data)
    {
        factoryDic_ = FactoryDAL.SelectAllFactoryDic();
    }

    public static MachineModel GetMachineData(int machineID)
    {
        if (machineDic_ != null && machineDic_.ContainsKey(machineID))
        {
            return machineDic_[machineID];
        }
        return null;
    }

    public static FactoryModel GetFactoryData(int factoryID)
    {
        if (factoryDic_ != null && factoryDic_.ContainsKey(factoryID))
        {
            return factoryDic_[factoryID];
        }
        return null;
    }
}
