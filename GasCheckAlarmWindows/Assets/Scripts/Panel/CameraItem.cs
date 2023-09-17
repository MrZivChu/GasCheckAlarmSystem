using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class CameraItem : MonoBehaviour
{
    public YUVRender yuvRender;
    public List<Text> gasNameTexts;
    public List<Text> gasValueTexts;

    public void Connect(CameraModel model)
    {
        InitGasName(model.GasInfos);
        yuvRender.Init();
        Thread thread = new Thread(new ParameterizedThreadStart(RunConnect));
        thread.Start(model);
    }

    List<List<int>> warningValueList_ = new List<List<int>>();
    void InitGasName(string gasInfos)
    {
        warningValueList_.Clear();
        string[] infos = gasInfos.Split(';');
        if (infos.Length >= 4)
        {
            for (int i = 0; i < 4; i++)
            {
                string[] info = infos[i].Split(',');
                if (info.Length >= 4)
                {
                    gasNameTexts[i].text = info[1] + "：";
                    int firstWarnValue = 0;
                    int secondWarnValue = 0;
                    int.TryParse(info[2], out firstWarnValue);
                    int.TryParse(info[3], out secondWarnValue);
                    warningValueList_.Add(new List<int>() {
                        firstWarnValue,
                        secondWarnValue
                    });
                }
            }
        }
    }

    public void UpdateGasValue(string gasValues)
    {
        string[] values = gasValues.Split(',');
        if (values.Length >= 4 && warningValueList_.Count >= 4)
        {
            for (int i = 0; i < 4; i++)
            {
                int value = Convert.ToInt32(values[i]);
                gasValueTexts[i].text = value.ToString();
                EWarningLevel level = value >= warningValueList_[i][1] ? EWarningLevel.SecondAlarm : (
                        value >= warningValueList_[i][0] ? EWarningLevel.FirstAlarm : EWarningLevel.Normal);
                gasValueTexts[i].color = FormatData.warningColorDic[level];
            }
        }
    }

    void RunConnect(object obj)
    {
        CameraModel model = (CameraModel)obj;
        yuvRender.DisConnect();
        yuvRender.Connect(model.IP, model.Port, model.UserName, model.UserPwd);
    }
}
