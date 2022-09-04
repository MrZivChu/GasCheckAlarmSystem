using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public struct SGameConfig
{
    public bool isEnterPosDir;
    public bool isLog;
    public double mainLight;
    public double subLight;
    public bool isSetLightByUI;
    public string commName;
    public string productName;
}

public class JsonHandleHelper : UIEventHelper
{
    public static SGameConfig gameConfig;
    static string configPath = string.Empty;
    void Awake()
    {
        DontDestroyOnLoad(this);
        configPath = Application.persistentDataPath + "/config.txt";
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
            gameConfig.mainLight = 1.5f;
            gameConfig.subLight = 0.3f;
            gameConfig.isSetLightByUI = false;
            gameConfig.productName = "钢铁有限责任公司\n气体监控系统";
            string json = LitJson.JsonMapper.ToJson(gameConfig);
            File.WriteAllText(configPath, json);
        }
        Debug.unityLogger.logEnabled = Application.isEditor ? true : gameConfig.isLog;
        if (LightSetting.instance)
        {
            LightSetting.instance.SetMainLight(System.Convert.ToSingle(gameConfig.mainLight));
            LightSetting.instance.SetSubLight(System.Convert.ToSingle(gameConfig.subLight));
        }
    }

    public static void UpdateConfig(string commName)
    {
        gameConfig.commName = commName;
        string json = LitJson.JsonMapper.ToJson(gameConfig);
        File.WriteAllText(configPath, json);
    }

    void OnApplicationQuit()
    {
        CloseWinformExeProcess();
    }

    void CloseWinformExeProcess()
    {
        CSharpUtils.KillProcess("SerialPortDataCollectionSystem");
    }
}
