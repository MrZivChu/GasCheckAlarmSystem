using System;
using System.IO;

namespace GasCheckAlarmSystem
{
    class ConfigHandleHelper
    {
        public static SWinformConfig gameConfig_ = null;
        public static void InitConfig()
        {
            string configPath = AppDomain.CurrentDomain.BaseDirectory + "/configPath.txt";
            if (File.Exists(configPath))
            {
                string content = File.ReadAllText(configPath);
                if (!string.IsNullOrEmpty(content))
                {
                    gameConfig_ = LitJson.JsonMapper.ToObject<SWinformConfig>(content);
                    LogHelper.AddLog("解析配置文件成功");
                }
            }
        }

        public static SWinformConfig GetConfig()
        {
            return gameConfig_;
        }
    }
}
