using System;
using System.IO;
using UnityEngine;

public struct SGameConfig
{
    public bool isLog;
    public bool isEnterPosDir;
    public bool isOpenWaterSeal;
    public string productName;
    public string sqlIP;
    public string sqlDatabase;
    public string sqlUserId;
    public string sqlUserPwd;
    public string smsPhone;
    public double yanGanMinValue;
    public double yanGanMaxValue;
    public double yiYangHuaTanMinValue;
    public double yiYangHuaTanMaxValue;
    public double yangQiMinValue;
    public double yangQiMaxValue;
}

public class JsonHandleHelper : UIEventHelper
{
    public static SGameConfig gameConfig;
    public static bool isRemoteServer = true;
    static string configPath = string.Empty;
    void Awake()
    {
        configPath = Application.persistentDataPath + "/config.txt";
        File.WriteAllText(Application.streamingAssetsPath + "/configPath.txt", configPath);

        if (File.Exists(configPath))
        {
            string content = File.ReadAllText(configPath);
            gameConfig = LitJson.JsonMapper.ToObject<SGameConfig>(content);
        }
        else
        {
            gameConfig = new SGameConfig();
            gameConfig.isEnterPosDir = false;
            gameConfig.isLog = false;
            gameConfig.sqlIP = "127.0.0.1";
            gameConfig.sqlDatabase = "GasCheckAlarm";
            gameConfig.sqlUserId = "sa";
            gameConfig.sqlUserPwd = "1";
            gameConfig.productName = "钢铁有限责任公司\n气体监控系统";
            gameConfig.isOpenWaterSeal = false;
            gameConfig.smsPhone = string.Empty;
            gameConfig.yanGanMinValue = 5;
            gameConfig.yanGanMaxValue = 15;
            gameConfig.yiYangHuaTanMinValue = 24;
            gameConfig.yiYangHuaTanMaxValue = 100;
            gameConfig.yangQiMinValue = 5;
            gameConfig.yangQiMaxValue = 100;
            string json = LitJson.JsonMapper.ToJson(gameConfig);
            File.WriteAllText(configPath, json);
        }
        SqlHelper.InitSqlConnection(gameConfig.sqlIP, gameConfig.sqlDatabase, gameConfig.sqlUserId, gameConfig.sqlUserPwd);
        isRemoteServer = gameConfig.sqlIP != "127.0.0.1";
        Debug.unityLogger.logEnabled = Application.isEditor ? true : gameConfig.isLog;

        FormatData.gasKindFormat[EGasKind.YanGan].minValue = Convert.ToSingle(gameConfig.yanGanMinValue);
        FormatData.gasKindFormat[EGasKind.YanGan].maxValue = Convert.ToSingle(gameConfig.yanGanMaxValue);
        FormatData.gasKindFormat[EGasKind.YiYangHuaTan].minValue = Convert.ToSingle(gameConfig.yiYangHuaTanMinValue);
        FormatData.gasKindFormat[EGasKind.YiYangHuaTan].maxValue = Convert.ToSingle(gameConfig.yiYangHuaTanMaxValue);
        FormatData.gasKindFormat[EGasKind.YangQi].minValue = Convert.ToSingle(gameConfig.yangQiMinValue);
        FormatData.gasKindFormat[EGasKind.YangQi].maxValue = Convert.ToSingle(gameConfig.yangQiMaxValue);
    }

    public static void UpdateConfig(bool isLog, bool isEnterPosDir, bool isOpenWaterSeal, string productName, string sqlIP, string sqlDatabase, string sqlUserId, string sqlUserPwd, string smsPhone, double yanGanMinValue, double yanGanMaxValue, double yiYangHuaTanMinValue, double yiYangHuaTanMaxValue, double yangQiMinValue, double yangQiMaxValue)
    {
        gameConfig.isEnterPosDir = isEnterPosDir;
        gameConfig.isLog = isLog;
        gameConfig.sqlIP = sqlIP;
        gameConfig.sqlDatabase = sqlDatabase;
        gameConfig.sqlUserId = sqlUserId;
        gameConfig.sqlUserPwd = sqlUserPwd;
        gameConfig.productName = productName;
        gameConfig.isOpenWaterSeal = isOpenWaterSeal;
        gameConfig.smsPhone = smsPhone;
        gameConfig.yanGanMinValue = yanGanMinValue;
        gameConfig.yanGanMaxValue = yanGanMaxValue;
        gameConfig.yiYangHuaTanMinValue = yiYangHuaTanMinValue;
        gameConfig.yiYangHuaTanMaxValue = yiYangHuaTanMaxValue;
        gameConfig.yangQiMinValue = yangQiMinValue;
        gameConfig.yangQiMaxValue = yangQiMaxValue;
        string json = LitJson.JsonMapper.ToJson(gameConfig);
        File.WriteAllText(configPath, json);
    }

}
