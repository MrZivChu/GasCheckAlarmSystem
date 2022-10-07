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
    public InputField nameInput;

    private void Start()
    {
        EventManager.Instance.AddEventListener(NotifyType.UpdateRealtimeDataList, UpdateRealtimeData);
        RegisterInputFieldOnEndEdit(nameInput, OnNameInputEnd);
        nameInput.text = GameUtils.GetString("nameInput", "");
    }

    private void OnDestroy()
    {
        EventManager.Instance.DeleteEventListener(NotifyType.UpdateRealtimeDataList, UpdateRealtimeData);
    }

    void OnNameInputEnd(InputField input, string content)
    {
        GameUtils.SetString("nameInput", content);
    }

    void UpdateRealtimeData(object data)
    {
        if (!gameObject || !gameObject.activeSelf)
        {
            return;
        }
        List<ProbeModel> list = (List<ProbeModel>)data;
        InitGrid(list);
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
