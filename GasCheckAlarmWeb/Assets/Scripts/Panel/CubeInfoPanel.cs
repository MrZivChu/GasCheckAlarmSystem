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

    private void Start()
    {
        EventManager.Instance.AddEventListener(NotifyType.UpdateRealtimeDataList, UpdateRealtimeData);
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
        list.AddRange(realtimeEventData.firstList);
        list.AddRange(realtimeEventData.noResponseList);
        list.AddRange(realtimeEventData.normalList);
        list.AddRange(realtimeEventData.secondList);
        InitGrid(list);
    }

    void InitGrid(List<RealtimeDataModel> list)
    {
        float contentWidth = contentTrans.parent.GetComponent<RectTransform>().rect.size.x;
        float contentHeight = contentTrans.parent.GetComponent<RectTransform>().rect.size.y;
        float sumMianji = contentWidth * contentHeight;
        bool search = true;
        int colCount = 1;
        float cellWidth = 0;
        float cellHeight = 0;
        while (search)
        {
            cellWidth = contentWidth / colCount;
            cellHeight = cellWidth / 2;
            if (cellWidth * cellHeight * list.Count > sumMianji)
            {
                colCount++;
            }
            else
            {
                search = false;
            }
        }
        gridLayoutGroup.cellSize = new Vector2(cellWidth, cellHeight);
        gridLayoutGroup.constraintCount = colCount;
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
