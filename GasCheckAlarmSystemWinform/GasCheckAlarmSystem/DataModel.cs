using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

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
    public int MachineType;
    public string MachineName;
    public string GasKind;
    public double FirstAlarmValue;
    public double SecondAlarmValue;

    //扩展字段
    public int warningLevel = 0;
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
    public int MachineType;
    public int MachineID;
    public int FactoryID;
    public string FactoryName;
}

public class ProbeSerialPortInfo
{
    public int ProbeID;
    public string ProbeName;
    public int MachineID;
    public string MachineName;
    public int MachineType;
    public string MachineAddress;
    public int FactoryID;
    public string FactoryName;
    public string ProbeAddress;
    public float FirstAlarmValue;
    public float SecondAlarmValue;
    public string GasKind;
}

public class MachineSerialPortInfo
{
    public string MachineAddress;
    public int MachineType;
    public string command = "03";
    public string FirstProbeDecAddress;
    public string EndProbeDecAddress;
    public List<ProbeSerialPortInfo> list;
}

public class FormatData
{
    public static Dictionary<int, int> baudRateFormat = new Dictionary<int, int>() {
        { 0,4800 },{ 1,9600 },{ 2,9600 },{ 3,4800 },{ 4,4800 }
    };
}