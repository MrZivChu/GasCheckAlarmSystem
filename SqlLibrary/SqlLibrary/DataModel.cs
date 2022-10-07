using System;

public enum EWarningLevel
{
    Normal,
    NoResponse,
    FirstAlarm,
    SecondAlarm,
}

public enum EProtocolType
{
    StandardOne,
    DZ40New,
    DZ40Old,
    Standard,
    HaiWan
}

public enum EGasKind
{
    YanGan,
    YiYangHuaTan,
    YangQi,
}

public enum EAuthority
{
    Consumer,
    Admin,
}

public class DeviceTagModel
{
    public int ID;
    public string TagName;
    public int ParentID;
    public string Position;
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
    public double GasValue;
    public DateTime CheckTime;
    public int MachineID;

    //扩展字段
    public EGasKind gasKind;
    public EWarningLevel warningLevel;
    public string probeName;
}

public class MachineModel
{
    public int ID;
    public string MailAddress;
    public string MachineName;
    public int FactoryID;
    public int ProtocolType;
    public int BaudRate;
}

public class PointCheckModel
{
    public int ID;
    public int DeviceID;
    public string DeviceName;
    public int DeviceType;
    public string UserName;
    public string QrCodePath;
    public DateTime CheckTime;
    public string Description;
    public string Result;
}

public class ProbeModel
{
    public int ID;
    public string MailAddress;
    public string ProbeName;
    public EGasKind GasKind;
    public int MachineID;
    public string Pos3D;
    public string Pos2D;
    public string SerialNumber;
    public string TagName;
    public DateTime CheckTime;
    public double GasValue;

    //扩展字段
    public bool isCheck = false;
    public EWarningLevel warningLevel;
}

public class UserModel
{
    public int ID;
    public string AccountName;
    public string AccountPwd;
    public string UserName;
    public string UserNumber;
    public string Phone;
    public EAuthority Authority;
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