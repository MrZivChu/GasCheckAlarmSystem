using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class RealtimeEventData
{
    public List<RealtimeDataModel> secondList;
    public List<RealtimeDataModel> firstList;
    public List<RealtimeDataModel> normalList;
    public List<RealtimeDataModel> noResponseList;
}

public class GlobalCheckGas : MonoBehaviour
{
    private void Start()
    {
        //程序启动执行一次删除历史数据的操作
        HistoryDataDAL.DeleteHistoryDataBeforeWeek();
    }

    float refreshTime = 1;//1秒
    float tempRefreshTime = 0;
    float tempDeleteHistoryDataTime = 0;
    float deleteHistoryDataTime = 60 * 60 * 24 * 1;
    private void Update()
    {
        tempRefreshTime += Time.deltaTime;
        if (tempRefreshTime > refreshTime)
        {
            tempRefreshTime = 0;
            List<RealtimeDataModel> result = RealtimeDataDAL.SelectAllRealtimeDataByCondition();
            RealtimeEventData data = HandleRealtimeData(result);
            JudgeWarningShout(data);
            HandleSMS(data);
            EventManager.Instance.DisPatch(NotifyType.UpdateRealtimeDataList, data);
        }
        tempDeleteHistoryDataTime += Time.deltaTime;
        if (tempDeleteHistoryDataTime >= deleteHistoryDataTime)
        {
            tempDeleteHistoryDataTime = 0;
            HistoryDataDAL.DeleteHistoryDataBeforeWeek();
        }
    }

    RealtimeEventData HandleRealtimeData(List<RealtimeDataModel> list)
    {
        RealtimeEventData realtimeEventData = new RealtimeEventData();
        realtimeEventData.firstList = new List<RealtimeDataModel>();
        realtimeEventData.secondList = new List<RealtimeDataModel>();
        realtimeEventData.normalList = new List<RealtimeDataModel>();
        realtimeEventData.noResponseList = new List<RealtimeDataModel>();

        //level：-1超时 0正常 1低报 2高报
        int overTimeMax = list.Count * 2;
        foreach (var model in list)
        {
            TimeSpan ts = DateTime.Now - model.CheckTime;
            if (ts.TotalSeconds > overTimeMax && !Application.isEditor)
            {
                model.warningLevel = -1;
                realtimeEventData.noResponseList.Add(model);
            }
            else
            {
                if (model.GasKind == "氧气")
                {
                    model.GasValue = model.GasValue / 10.0f;
                    if (model.GasValue > model.SecondAlarmValue)
                    {
                        model.warningLevel = 2;
                        realtimeEventData.secondList.Add(model);
                    }
                    else if (model.GasValue < model.FirstAlarmValue)
                    {
                        model.warningLevel = 1;
                        realtimeEventData.firstList.Add(model);
                    }
                    else
                    {
                        model.warningLevel = 0;
                        realtimeEventData.normalList.Add(model);
                    }
                }
                else
                {
                    if (model.MachineType == 4)
                    {
                        if (model.GasValue == 1)
                        {
                            model.warningLevel = 2;
                            realtimeEventData.secondList.Add(model);
                        }
                        else
                        {
                            model.warningLevel = 0;
                            realtimeEventData.normalList.Add(model);
                        }
                    }
                    else
                    {
                        if (model.GasValue >= model.SecondAlarmValue)
                        {
                            model.warningLevel = 2;
                            realtimeEventData.secondList.Add(model);
                        }
                        else if (model.GasValue >= model.FirstAlarmValue)
                        {
                            model.warningLevel = 1;
                            realtimeEventData.firstList.Add(model);
                        }
                        else
                        {
                            model.warningLevel = 0;
                            realtimeEventData.normalList.Add(model);
                        }
                    }
                }
            }
        }
        return realtimeEventData;
    }

    void JudgeWarningShout(RealtimeEventData realtimeEventData)
    {
        bool hasAlarm = realtimeEventData.firstList.Count + realtimeEventData.secondList.Count > 0;
        if (hasAlarm)
        {
            AudioManager.instance.PlayWarningShout();
            CameraShake.instance.StartShake();
        }
        else
        {
            AudioManager.instance.PauseWarningShout();
            CameraShake.instance.StopShake();
        }
    }

    static List<int> smsDic = new List<int>();
    static bool hasNew = false;
    static DateTime preTime = DateTime.Now;
    void HandleSMS(RealtimeEventData realtimeEventData)
    {
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < realtimeEventData.normalList.Count; i++)
        {
            RealtimeDataModel model = realtimeEventData.normalList[i];
            if (smsDic.Contains(model.ProbeID))
            {
                smsDic.Remove(model.ProbeID);
            }
        }
        hasNew = false;
        for (int i = 0; i < realtimeEventData.firstList.Count; i++)
        {
            RealtimeDataModel model = realtimeEventData.firstList[i];
            sb = sb.Append(HandleSMSProbeStr(model));
        }
        for (int i = 0; i < realtimeEventData.secondList.Count; i++)
        {
            RealtimeDataModel model = realtimeEventData.secondList[i];
            sb = sb.Append(HandleSMSProbeStr(model));
        }
        for (int i = 0; i < realtimeEventData.noResponseList.Count; i++)
        {
            RealtimeDataModel model = realtimeEventData.noResponseList[i];
            sb = sb.Append(HandleSMSProbeStr(model));
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

    string HandleSMSProbeStr(RealtimeDataModel model)
    {
        if (!smsDic.Contains(model.ProbeID))
        {
            smsDic.Add(model.ProbeID);
            hasNew = true;
        }
        return "[" + model.ProbeName + "]";
    }
}
