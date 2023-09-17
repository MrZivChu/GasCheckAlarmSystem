using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraHistoryDataItem : UIEventHelper
{
    public Text txt_index;
    public Text txt_ip;
    public Text txt_gasKind;
    public Text txt_gasValue;
    public Text txt_firstAlarmValue;
    public Text txt_secondAlarmValue;
    public Text txt_checkTime;

    public Image img_background;

    void Start()
    {
    }

    public void InitData(int index, string ip, string gasName, int value, int firstWarnValue, int secondWarnValue, int timeStamp)
    {
        txt_index.text = index.ToString();
        txt_ip.text = ip;
        txt_gasKind.text = gasName;
        txt_gasValue.text = value.ToString();
        txt_firstAlarmValue.text = firstWarnValue.ToString();
        txt_secondAlarmValue.text = secondWarnValue.ToString();
        txt_checkTime.text = CSharpUtils.TimeStampToDateTime(timeStamp).ToString("yyyy-MM-dd HH:mm::ss");
    }

    public void SetBackgroundColor(Color color)
    {
        img_background.color = color;
    }
}
