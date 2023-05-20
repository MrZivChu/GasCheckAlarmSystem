using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlertHelper
{
    static Dictionary<int, DateTime> alertDic = new Dictionary<int, DateTime>();
    public static void HandleProbeInfo(List<ProbeModel> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            ProbeModel model = list[i];
            if (model.GasValue >= JsonHandleHelper.gameConfig.alertWarnValue)
            {
                if (!alertDic.ContainsKey(model.ID))
                {
                    alertDic.Add(model.ID, DateTime.Now);
                }
                DateTime preTime = alertDic[model.ID];
                TimeSpan span = DateTime.Now - preTime;
                if (span.TotalSeconds >= JsonHandleHelper.gameConfig.alertWarnSeconds)
                {
                    string tip = string.Format("设备[{0}]异常，请立即检查！", model.ProbeName);
                    MessageBox.Instance.PopOK(tip, null, "确定");
                }
            }
            else
            {
                if (alertDic.ContainsKey(model.ID))
                {
                    alertDic.Remove(model.ID);
                }
            }
        }

    }
}
