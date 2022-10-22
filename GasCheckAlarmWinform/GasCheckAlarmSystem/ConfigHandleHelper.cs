using System;
using System.IO;

namespace GasCheckAlarmSystem
{
    public class SGameConfig
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
    class ConfigHandleHelper
    {
        public static SGameConfig gameConfig_ = null;
        public static void InitConfig()
        {
            string configPath = AppDomain.CurrentDomain.BaseDirectory + "/configPath.txt";
            if (File.Exists(configPath))
            {
                string configPathContent = File.ReadAllText(configPath);
                if (File.Exists(configPathContent))
                {
                    string content = File.ReadAllText(configPathContent);
                    if (!string.IsNullOrEmpty(content))
                    {
                        gameConfig_ = LitJson.JsonMapper.ToObject<SGameConfig>(content);
                        LogHelper.AddLog("解析配置文件成功");
                    }
                }
            }
        }

        public static SGameConfig GetConfig()
        {
            return gameConfig_;
        }
    }
}
