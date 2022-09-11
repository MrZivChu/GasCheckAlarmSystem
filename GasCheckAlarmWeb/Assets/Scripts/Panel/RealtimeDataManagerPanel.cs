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

    void UpdateRealtimeDataListEvent(object tdata)
    {
        if (!gameObject.activeSelf)
            return;
        RealtimeEventData realtimeEventData = (RealtimeEventData)tdata;
        List<RealtimeDataModel> secondList = realtimeEventData.secondList;
        List<RealtimeDataModel> firstList = realtimeEventData.firstList;
        List<RealtimeDataModel> normalList = realtimeEventData.normalList;
        List<RealtimeDataModel> noResponseList = realtimeEventData.noResponseList;
        List<RealtimeDataModel> allList = new List<RealtimeDataModel>();
        allList.AddRange(secondList);
        allList.AddRange(firstList);
        allList.AddRange(normalList);
        allList.AddRange(noResponseList);
        GameUtils.SpawnCellForTable<RealtimeDataModel>(contentTrans, allList, (go, data, isSpawn, index) =>
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
            item.InitData(data);
            item.SetBackgroundColor(FormatData.warningColorDic[data.warningLevel]);
            currentObj.SetActive(true);
        });
    }
}