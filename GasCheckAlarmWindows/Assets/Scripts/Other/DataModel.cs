using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GasKindInfo
{
    public string name;
    public string unit;
    public float minValue;
    public float maxValue;
}

public class FormatData
{
    public static Dictionary<EProtocolType, string> protocolTypeFormat = new Dictionary<EProtocolType, string>() {
        { EProtocolType.StandardOne,"标1协议" },
        { EProtocolType.DZ40New,"DZ-40-New" },
        { EProtocolType.DZ40Old,"DZ-40-Old" },
        { EProtocolType.Standard,"标准协议" },
        { EProtocolType.HaiWan,"海湾" },
        { EProtocolType.WeiTai,"惟泰" },
    };

    public static Dictionary<EGasKind, GasKindInfo> gasKindFormat = new Dictionary<EGasKind, GasKindInfo>() {
        { EGasKind.YanGan, new GasKindInfo(){ name="烟感", unit="%OBS/M", minValue=5, maxValue=15} },
        { EGasKind.YiYangHuaTan, new GasKindInfo(){ name="一氧化碳", unit="ppm", minValue=24, maxValue=100} },
        { EGasKind.YangQi, new GasKindInfo(){ name="氧气", unit="O₂", minValue=5, maxValue=100} },
    };

    public static Dictionary<int, string> haiwanDic = new Dictionary<int, string>() {
        {0,"无事件应答" },{1,"火警" },{2,"故障" },{3,"动作" },{4,"恢复" },{5,"启动" },{6,"停动" },{7,"隔离" },{8,"释放" },{9,"主电备电恢复" }
    };

    public static List<int> baudRateFormat = new List<int>() { 4800, 9600 };

    public static Dictionary<EWarningLevel, Color> warningColorDic = new Dictionary<EWarningLevel, Color>() {
        {EWarningLevel.SecondAlarm, new Color(1f, 0f, 0f)},
        {EWarningLevel.FirstAlarm, new Color(1f, 1f, 0f)},
        {EWarningLevel.Normal, new Color(0.2f, 0.6f, 0.2f, 0.5f)},
        {EWarningLevel.NoResponse, new Color(0.75f, 0.75f, 0.75f)}
    };

    public static Dictionary<EAuthority, string> authorityFormat = new Dictionary<EAuthority, string>() {
        {EAuthority.Consumer, "普通用户"},
        {EAuthority.Admin, "管理员"}
    };

    public static UserModel currentUser = new UserModel()
    {
        Authority = EAuthority.Admin,
        UserName = "--"
    };

    public static string GetGasValue(EProtocolType protocolType, EGasKind gasKind, float gasValue)
    {
        if (gasKind == EGasKind.YangQi)
        {
            gasValue = gasValue / 10.0f;

        }
        if (protocolType == EProtocolType.HaiWan)
        {
            if (FormatData.haiwanDic.ContainsKey((int)gasValue))
            {
                return FormatData.haiwanDic[(int)gasValue];
            }
            else
            {
                return "未找到此值对应的状态：" + gasValue;
            }
        }
        else if (protocolType == EProtocolType.DZ40New)
        {
            if (gasValue.ToString() == "-1")
            {
                return "预热";
            }
            else if (gasValue.ToString() == "-2")
            {
                return "不在线";
            }
            else
            {
                return gasValue.ToString();
            }
        }
        else
        {
            return gasValue.ToString();
        }
    }
}
