using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

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
    public double FirstAlarmValue;
    public double SecondAlarmValue;
    public string MachineName;
    public int MachineID;
    public string PosDir;
    public int FactoryID;
    public string FactoryName;
    public int MachineType;
    public string Pos2D;
    public string TagName;
    public string SerialNumber;

    //扩展字段
    public bool isCheck = false;
}

public class RealtimeDataModel
{
    public int ID;
    public int ProbeID;
    public string ProbeName;
    public DateTime CheckTime;
    public double GasValue;
    public string GasKind;
    public string Unit;
    public double FirstAlarmValue;
    public double SecondAlarmValue;
    public string MachineName;
    public int MachineID;
    public int FactoryID;
    public string FactoryName;
    public int MachineType;
    public string Pos2D;
    public string TagName;

    //扩展字段
    public int warningLevel = 0;
}

public class DeviceTagModel
{
    public int ID;
    public string TagName;
    public int ParentID;
    public string Position;
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

public class WaterSealModel
{
    public int ID;
    public string Medium;
    public string Number;
    public string InstallPosition;
    public string Category;
    public int DesignPressure;
    public string SerialNumber;

    //扩展字段
    public bool isCheck = false;
}

public class PointCheckModel
{
    public int ID;
    public int DeviceID;
    public int DeviceType;
    public string DeviceName;
    public string UserName;
    public string QrCodePath;
    public DateTime CheckTime;
    public string Description;
    public string Result;
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
    public double FirstAlarmValue;
    public double SecondAlarmValue;
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