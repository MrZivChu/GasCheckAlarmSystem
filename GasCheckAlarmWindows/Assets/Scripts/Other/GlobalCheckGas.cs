using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class GlobalCheckGas : MonoBehaviour
{
    private void Awake()
    {
        MachineFactoryDataManager.Init();
        UpdateProbeListEvent(null);
        //程序启动执行一次删除历史数据的操作
        HistoryDataDAL.DeleteHistoryDataBeforeWeek();
    }

    private void Start()
    {
        EventManager.Instance.AddEventListener(NotifyType.UpdateProbeList, UpdateProbeListEvent);
    }

    private void OnDestroy()
    {
        EventManager.Instance.DeleteEventListener(NotifyType.UpdateProbeList, UpdateProbeListEvent);
    }

    Dictionary<int, ProbeModel> baseInfoDic_ = new Dictionary<int, ProbeModel>();
    void UpdateProbeListEvent(object data)
    {
        baseInfoDic_.Clear();
        List<ProbeModel> list = ProbeDAL.SelectIDProbeNameTagName();
        list.ForEach(it =>
        {
            baseInfoDic_[it.ID] = it;
        });
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
            List<ProbeModel> result = ProbeDAL.SelectIDCheckTimeGasValueGasKindMachineID();
            HandleRealtimeData(result);
            SMSHelper.HandleProbeInfo(result);
            EventManager.Instance.DisPatch(NotifyType.UpdateRealtimeDataList, result);
        }
        tempDeleteHistoryDataTime += Time.deltaTime;
        if (tempDeleteHistoryDataTime >= deleteHistoryDataTime)
        {
            tempDeleteHistoryDataTime = 0;
            HistoryDataDAL.DeleteHistoryDataBeforeWeek();
        }
    }

    void HandleRealtimeData(List<ProbeModel> list)
    {
        int abnormalCount = 0;
        int overTimeMax = list.Count * 2;
        foreach (var model in list)
        {
            if (baseInfoDic_.ContainsKey(model.ID))
            {
                model.ProbeName = baseInfoDic_[model.ID].ProbeName;
                model.TagName = baseInfoDic_[model.ID].TagName;
            }
            TimeSpan ts = DateTime.Now - model.CheckTime;
            if (ts.TotalSeconds > overTimeMax && !Application.isEditor)
            {
                model.warningLevel = EWarningLevel.NoResponse;
            }
            else
            {
                if (model.GasKind == EGasKind.YangQi)
                {
                    model.GasValue = model.GasValue / 10.0f;
                    if (model.GasValue >= FormatData.gasKindFormat[model.GasKind].maxValue)
                    {
                        model.warningLevel = EWarningLevel.SecondAlarm;
                        abnormalCount++;
                    }
                    else if (model.GasValue >= FormatData.gasKindFormat[model.GasKind].minValue)
                    {
                        model.warningLevel = EWarningLevel.FirstAlarm;
                        abnormalCount++;
                    }
                    else
                    {
                        model.warningLevel = EWarningLevel.Normal;
                    }
                }
                else
                {
                    if (MachineFactoryDataManager.GetMachineData(model.MachineID).ProtocolType == 4)
                    {
                        if (model.GasValue == 1)
                        {
                            model.warningLevel = EWarningLevel.SecondAlarm;
                            abnormalCount++;
                        }
                        else
                        {
                            model.warningLevel = EWarningLevel.Normal;
                        }
                    }
                    else
                    {
                        if (model.GasValue >= FormatData.gasKindFormat[model.GasKind].maxValue)
                        {
                            model.warningLevel = EWarningLevel.SecondAlarm;
                            abnormalCount++;
                        }
                        else if (model.GasValue >= FormatData.gasKindFormat[model.GasKind].minValue)
                        {
                            model.warningLevel = EWarningLevel.FirstAlarm;
                            abnormalCount++;
                        }
                        else
                        {
                            model.warningLevel = EWarningLevel.Normal;
                        }
                    }
                }
            }
        }
        if (abnormalCount > 0)
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
}
