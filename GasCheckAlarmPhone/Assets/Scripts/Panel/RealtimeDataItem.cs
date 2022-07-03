using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RealtimeDataItem : UIEventHelper
{
    public RealtimeDataModel currentModel;
    public Text txt_probeName;
    public Text txt_machineName;
    public Text txt_gasKind;
    public Text txt_gasValue;
    public Text txt_firstAlarmValue;
    public Text txt_secondAlarmValue;

    public Image img_background;
    void Start()
    {

    }

    public void InitData(RealtimeDataModel model)
    {
        currentModel = model;
        txt_probeName.text = model.ProbeName;
        txt_machineName.text = model.MachineName;
        txt_gasKind.text = model.GasKind;
        if (model.MachineType == 4)
        {
            if (FormatData.haiwanDic.ContainsKey((int)model.GasValue))
            {
                txt_gasValue.text = FormatData.haiwanDic[(int)model.GasValue];
            }
            else
            {
                txt_gasValue.text = "未找到此值对应的状态：" + model.GasValue;
            }
        }
        else if (model.MachineType == 1)
        {
            if (model.GasValue.ToString() == "-1")
            {
                txt_gasValue.text = "预热";
            }
            else if (model.GasValue.ToString() == "-2")
            {
                txt_gasValue.text = "不在线";
            }
            else
            {
                txt_gasValue.text = model.GasValue.ToString();
            }
        }
        else
        {
            txt_gasValue.text = model.GasValue.ToString();
        }
        txt_firstAlarmValue.text = model.FirstAlarmValue.ToString();
        txt_secondAlarmValue.text = model.SecondAlarmValue.ToString();
    }

    public void SetBackgroundColor(Color color)
    {
        img_background.color = color;
    }
}
