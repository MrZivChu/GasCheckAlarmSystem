using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CubeInfoPanel : UIEventHelper
{
    public GameObject itemRes;
    public Transform contentTrans;
    public GridLayoutGroup gridLayoutGroup;
    public InputField nameInput;

    private void Start()
    {
        EventManager.Instance.AddEventListener(NotifyType.UpdateRealtimeDataList, UpdateRealtimeData);
        RegisterInputFieldOnEndEdit(nameInput, OnNameInputEnd);
        nameInput.text = GameUtils.GetString("nameInput", "");
    }

    void OnNameInputEnd(InputField input, string content)
    {
        GameUtils.SetString("nameInput", content);
    }

    void UpdateRealtimeData(object data)
    {
        if (!enabled)
        {
            return;
        }
        RealtimeEventData realtimeEventData = (RealtimeEventData)data;
        InitData(realtimeEventData);
    }

    private void InitData(RealtimeEventData realtimeEventData)
    {
        List<RealtimeDataModel> list = new List<RealtimeDataModel>();
        list.AddRange(realtimeEventData.secondList);
        list.AddRange(realtimeEventData.firstList);
        list.AddRange(realtimeEventData.noResponseList);
        list.AddRange(realtimeEventData.normalList);
        InitGrid(list);
    }

    void InitGrid(List<RealtimeDataModel> list)
    {
        GameUtils.SpawnCellForTable<RealtimeDataModel>(contentTrans, list, (go, data, isSpawn, index) =>
        {
            GameObject currentObj = go;
            if (isSpawn)
            {
                currentObj = Instantiate(itemRes) as GameObject;
                currentObj.transform.SetParent(contentTrans);
                currentObj.transform.localScale = Vector3.one;
            }
            Vector3 position = currentObj.GetComponent<RectTransform>().anchoredPosition3D;
            position.z = 0;
            currentObj.GetComponent<RectTransform>().anchoredPosition3D = position;
            CubeInfoItem cubeInfoItem = currentObj.GetComponent<CubeInfoItem>();
            cubeInfoItem.InitData(data);
            currentObj.SetActive(true);
        }, false);
    }
}
