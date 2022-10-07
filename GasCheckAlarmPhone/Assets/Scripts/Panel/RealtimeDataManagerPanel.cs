using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class RealtimeDataManagerPanel : UIEventHelper
{
    public Transform contentTrans;
    public UnityEngine.Object itemRes;
    private void Start()
    {
        EventManager.Instance.AddEventListener(NotifyType.UpdateRealtimeDataList, UpdateRealtimeDataListEvent);
    }

    private void OnDestroy()
    {
        EventManager.Instance.DeleteEventListener(NotifyType.UpdateRealtimeDataList, UpdateRealtimeDataListEvent);
    }

    void UpdateRealtimeDataListEvent(object data)
    {
        if (!gameObject || !gameObject.activeSelf)
            return;
        List<ProbeModel> allList = (List<ProbeModel>)data;
        List<ProbeModel> secondAlarm = new List<ProbeModel>();
        List<ProbeModel> firstAlarm = new List<ProbeModel>();
        List<ProbeModel> noResponse = new List<ProbeModel>();
        List<ProbeModel> normal = new List<ProbeModel>();
        foreach (var item in allList)
        {
            if (item.warningLevel == EWarningLevel.SecondAlarm)
            {
                secondAlarm.Add(item);
            }
            else if (item.warningLevel == EWarningLevel.FirstAlarm)
            {
                firstAlarm.Add(item);
            }
            else if (item.warningLevel == EWarningLevel.NoResponse)
            {
                noResponse.Add(item);
            }
            else if (item.warningLevel == EWarningLevel.Normal)
            {
                normal.Add(item);
            }
        }
        List<ProbeModel> result = new List<ProbeModel>() { };
        result.AddRange(secondAlarm);
        result.AddRange(firstAlarm);
        result.AddRange(noResponse);
        result.AddRange(normal);
        InitGrid(result);
    }

    void InitGrid(List<ProbeModel> list)
    {
        GameUtils.SpawnCellForTable<ProbeModel>(contentTrans, list, (go, data, isSpawn, index) =>
        {
            GameObject currentObj = go;
            if (isSpawn)
            {
                currentObj = Instantiate(itemRes) as GameObject;
                currentObj.transform.SetParent(contentTrans);
                currentObj.transform.localScale = Vector3.one;
                currentObj.GetComponent<RectTransform>().anchoredPosition3D = Vector3.zero;
            }
            RealtimeDataItem item = currentObj.GetComponent<RealtimeDataItem>();
            item.InitInfo(data);
            item.SetBackgroundColor(FormatData.warningColorDic[data.warningLevel]);
            currentObj.SetActive(true);
        });
    }
}