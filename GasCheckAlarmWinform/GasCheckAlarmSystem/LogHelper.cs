using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace GasCheckAlarmSystem
{
    public class LogHelper
    {
        static string log_ = string.Empty;
        public static void AddLog(string log, params object[] args)
        {
            if (ConfigHandleHelper.GetConfig().isLog)
            {
                string content = string.Format(log, args);
                log_ += DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff ") + content + "\n";
            }
        }

        public static void AddChangeLine()
        {
            if (ConfigHandleHelper.GetConfig().isLog)
            {
                log_ += "\n";
            }
        }

        public static void SaveLog()
        {
            string filePath = AppDomain.CurrentDomain.BaseDirectory + "\\GasCheckAlarmSystemLog.txt";
            File.WriteAllText(filePath, log_);
            Process.Start(filePath);
            log_ = string.Empty;
        }
    }
}
