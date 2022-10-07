using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using UnityEngine;

public class SMSHelper
{
    static bool hasNew = false;
    static List<int> smsDic = new List<int>();
    static DateTime preTime = DateTime.Now;
    public static void HandleProbeInfo(List<ProbeModel> list)
    {
        StringBuilder sb = new StringBuilder();
        hasNew = false;
        for (int i = 0; i < list.Count; i++)
        {
            ProbeModel model = list[i];
            if (model.warningLevel == EWarningLevel.Normal)
            {
                if (smsDic.Contains(model.ID))
                {
                    smsDic.Remove(model.ID);
                }
            }
            else
            {
                sb = sb.Append(HandleSMSProbeStr(model));
            }
        }
        if (hasNew)
        {
            if (!string.IsNullOrEmpty(sb.ToString()))
            {
                preTime = DateTime.Now;
                SMSHelper.SendSMS(sb.ToString());
            }
        }
        else
        {
            int overTime = Application.isEditor ? 15 : 60 * 60 * 1;//1个小时
            if (DateTime.Now.Subtract(preTime).TotalSeconds > overTime)
            {
                if (!string.IsNullOrEmpty(sb.ToString()))
                {
                    preTime = DateTime.Now;
                    SMSHelper.SendSMS(sb.ToString());
                }
            }
        }
    }

    static string HandleSMSProbeStr(ProbeModel model)
    {
        if (!smsDic.Contains(model.ID))
        {
            smsDic.Add(model.ID);
            hasNew = true;
        }
        return "[" + model.ProbeName + "]";
    }

    static void SendSMS(string probeName)
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