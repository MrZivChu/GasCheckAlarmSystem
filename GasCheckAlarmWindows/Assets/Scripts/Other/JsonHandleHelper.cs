using System;
using System.IO;
using UnityEngine;

public struct SGameConfig
{
    public bool isLog;
    public bool isEnterPosDir;
    public bool isOpenWaterSeal;
    public bool isOpenCamera;
    public bool isOpenGlobalImage;
    public string productName;
    public string sqlIP;
    public string sqlDatabase;
    public string sqlUserId;
    public string sqlUserPwd;
    public string smsPhone;
    public int alertWarnValue;
    public int alertWarnSeconds;
}

public class JsonHandleHelper : UIEventHelper
{
    public static SGameConfig gameConfig;
    public static bool isRemoteServer = true;
    static string configPath = string.Empty;
    void Awake()
    {
        configPath = Application.persistentDataPath + "/config.txt";
        Debug.Log(configPath);
        if (File.Exists(configPath))
        {
            string content = File.ReadAllText(configPath);
            try
            {
                gameConfig = LitJson.JsonMapper.ToObject<SGameConfig>(content);
            }
            catch (Exception ex)
            {
                File.Delete(configPath);
                Application.Quit();
            }
        }
        else
        {
            gameConfig = new SGameConfig();
            gameConfig.isEnterPosDir = false;
            gameConfig.isOpenCamera = false;
            gameConfig.isOpenGlobalImage = false;
            gameConfig.isLog = false;
            gameConfig.sqlIP = "127.0.0.1";
            gameConfig.sqlDatabase = "GasCheckAlarm";
            gameConfig.sqlUserId = "sa";
            gameConfig.sqlUserPwd = "1";
            gameConfig.productName = "钢铁有限责任公司\n气体监控系统";
            gameConfig.isOpenWaterSeal = false;
            gameConfig.smsPhone = string.Empty;
            gameConfig.alertWarnValue = 100;
            gameConfig.alertWarnSeconds = 20;
            string json = LitJson.JsonMapper.ToJson(gameConfig);
            File.WriteAllText(configPath, json);
        }
        SqlHelper.InitSqlConnection(gameConfig.sqlIP, gameConfig.sqlDatabase, gameConfig.sqlUserId, gameConfig.sqlUserPwd);
        isRemoteServer = gameConfig.sqlIP != "127.0.0.1";
        Debug.unityLogger.logEnabled = Application.isEditor ? true : gameConfig.isLog;

        //给Winform使用的配置文件
        SWinformConfig winformConfig = new SWinformConfig();
        winformConfig.isLog = gameConfig.isLog;
        winformConfig.sqlIP = gameConfig.sqlIP;
        winformConfig.sqlDatabase = gameConfig.sqlDatabase;
        winformConfig.sqlUserId = gameConfig.sqlUserId;
        winformConfig.sqlUserPwd = gameConfig.sqlUserPwd;
        File.WriteAllText(Application.streamingAssetsPath + "/configPath.txt", LitJson.JsonMapper.ToJson(winformConfig));
    }

    public static void UpdateConfig(bool isLog, bool isEnterPosDir, bool isOpenWaterSeal, string productName, string sqlIP, string sqlDatabase, string sqlUserId, string sqlUserPwd, string smsPhone, int alertWarnValue, int alertWarnSeconds, bool isOpenCamera, bool isOpenGlobalImage)
    {
        gameConfig.isEnterPosDir = isEnterPosDir;
        gameConfig.isOpenCamera = isOpenCamera;
        gameConfig.isOpenGlobalImage = isOpenGlobalImage;
        gameConfig.isLog = isLog;
        gameConfig.sqlIP = sqlIP;
        gameConfig.sqlDatabase = sqlDatabase;
        gameConfig.sqlUserId = sqlUserId;
        gameConfig.sqlUserPwd = sqlUserPwd;
        gameConfig.productName = productName;
        gameConfig.isOpenWaterSeal = isOpenWaterSeal;
        gameConfig.smsPhone = smsPhone;
        gameConfig.alertWarnValue = alertWarnValue;
        gameConfig.alertWarnSeconds = alertWarnSeconds;
        string json = LitJson.JsonMapper.ToJson(gameConfig);
        File.WriteAllText(configPath, json);
    }

}
