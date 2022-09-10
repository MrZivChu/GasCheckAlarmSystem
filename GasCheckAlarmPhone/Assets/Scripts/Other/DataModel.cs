using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        {0, new Color(0.2f, 0.6f, 0.2f,0.5f)},
        {-1, new Color(0.75f, 0.75f, 0.75f)}
    };

    public static Dictionary<int, string> authorityNameDic = new Dictionary<int, string>() {
        {0, "普通用户"},
        {1, "管理员"}
    };

    public static UserModel currentUser = new UserModel()
    {
        Authority = 1,
        UserName = "--"
    };
}
