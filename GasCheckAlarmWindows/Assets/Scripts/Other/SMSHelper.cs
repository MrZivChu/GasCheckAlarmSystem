using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class SMSHelper
{
    public static void SendSMS(string probeName)
    {
        if (string.IsNullOrEmpty(probeName))
        {
            return;
        }
        string content = string.Format("{0} 小盒子科技 SMS_251132221 {1}", JsonHandleHelper.gameConfig.smsPhone, probeName);
        if (Application.isEditor)
        {
            UnityEngine.Debug.Log(content);
            return;
        }
        ProcessStartInfo startInfo = new ProcessStartInfo(Application.streamingAssetsPath + "/SMS/SMS.exe");
        startInfo.UseShellExecute = false;
        startInfo.RedirectStandardOutput = true;
        startInfo.RedirectStandardError = true;
        startInfo.CreateNoWindow = true;//不显示窗口
        startInfo.Arguments = content;//向main函数传参数

        Process p = Process.Start(startInfo);
        //p.WaitForExit();
        //string output = p.StandardOutput.ReadToEnd();
        //UnityEngine.Debug.Log("输出的成功信息为 = " + output);
    }

}