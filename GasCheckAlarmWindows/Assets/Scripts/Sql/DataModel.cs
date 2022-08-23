using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class UserModel
{
    public int ID;
    public string AccountName;
    public string AccountPwd;
    public string UserName;
    public string UserNumber;
    public int Authority;
    public string Phone;
}

public class ProbeModel
{
    public int ID;
    public string MailAddress;
    public string ProbeName;
    public string GasKind;
    public string Unit;
    public float FirstAlarmValue;
    public float SecondAlarmValue;
    public string MachineName;
    public int MachineID;
    public string PosDir;
    public int FactoryID;
    public string FactoryName;
    public int MachineType;
    public string Pos2D;
}

public class RealtimeDataModel
{
    public int ID;
    public int ProbeID;
    public string ProbeName;
    public DateTime CheckTime;
    public float GasValue;
    public string GasKind;
    public string Unit;
    public float FirstAlarmValue;
    public float SecondAlarmValue;
    public string MachineName;
    public int MachineID;
    public int FactoryID;
    public string FactoryName;
    public int MachineType;
    public string Pos2D;

    //扩展字段
    public int warningLevel = 0;
}

public class MachineModel
{
    public int ID;
    public string MailAddress;
    public string MachineName;
    public string FactoryName;
    public int FactoryID;
    public int MachineType;
}

public class FactoryModel
{
    public int ID;
    public string FactoryName;
}

public class HistoryDataModel
{
    public int ID;
    public int ProbeID;
    public string ProbeName;
    public DateTime CheckTime;
    public double GasValue;
    public int FactoryID;
    public string FactoryName;
    public int MachineID;
    public string MachineName;
    public string GasKind;
    public double FirstAlarmValue;
    public double SecondAlarmValue;
    public int MachineType;

    //扩展字段
    public int warningLevel = 0;
}

public class MachineSerialPortInfo
{
    public int MachineID;
    public string MachineAddress;
    public string ProbeAddress;
    public string MachineName;
    public string ProbeName;
    public int ProbeID;
    public float FirstAlarmValue;
    public float SecondAlarmValue;
    public string GasKind;
}

public class MachineSerialPortInfoBase
{
    public string MachineAddress;
    public string command = "03";
    public string FirstProbeHexAddress = "0001";
    public string ReadProbeHexCount;
    public List<MachineSerialPortInfo> list;
}

public class FormatData
{
    public static List<string> machineTypeFormat = new List<string>()
    {
        "标1协议","DZ-40-New","DZ-40-Old","标准协议","海湾"
    };

    public static List<string> gasKindList = new List<string>() { "烟感", "一氧化碳", "可燃气", "氧气", "氢气", "硫化氢", "氨气", "氯气", "一氧化氮", "二氧化硫", "二氧化氮", "二氧化碳", "臭氧", "ETO", "氰化氢", "PH", "氯化氢", "LPG", "LNG", "氯甲烷" };

    public static List<string> unitList = new List<string>() { "%OBS/M", "ppm", "%LEL", "kppm", "ppb", "V/V%", "umol/mol", "mg/m3" };

    public static Dictionary<int, string> haiwanDic = new Dictionary<int, string>() {
        {0,"无事件应答" },{1,"火警" },{2,"故障" },{3,"动作" },{4,"恢复" },{5,"启动" },{6,"停动" },{7,"隔离" },{8,"释放" },{9,"主电备电恢复" }
    };

    public static Dictionary<int, Color> warningColorDic = new Dictionary<int, Color>() {
        {2, new Color(1f, 0f, 0f)},
        {1, new Color(1f, 1f, 0f)},
        {0, new Color(0.5f, 1f, 0.5f)},
        {-1, new Color(0.75f, 0.75f, 0.75f)}
    };

    public static UserModel currentUser = new UserModel()
    {
        Authority = 1,
        UserName = "--"
    };
}