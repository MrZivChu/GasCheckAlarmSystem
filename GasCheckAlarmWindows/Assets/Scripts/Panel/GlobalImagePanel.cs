using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GlobalImagePanel : UIEventHelper
{
    public GameObject tagItemRes;
    public GameObject probeItemRes;
    public Transform tagContentTrans;
    public Transform probeContentTrans;
    public Button preStepBtn;

    private void Start()
    {
        EventManager.Instance.AddEventListener(NotifyType.UpdateRealtimeDataList, UpdateRealtimeData);
        RegisterBtnClick(preStepBtn, OnPreStep);
    }

    void OnPreStep(Button btn)
    {
        if (selectDeviceTagID.Count > 1)
        {
            selectDeviceTagID.RemoveAt(selectDeviceTagID.Count - 1);
        }
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

    List<RealtimeDataModel> realtimeDataModelList = new List<RealtimeDataModel>();
    private void InitData(RealtimeEventData realtimeEventData)
    {
        realtimeDataModelList.Clear();
        realtimeDataModelList.AddRange(realtimeEventData.secondList);
        realtimeDataModelList.AddRange(realtimeEventData.firstList);
        realtimeDataModelList.AddRange(realtimeEventData.noResponseList);
        realtimeDataModelList.AddRange(realtimeEventData.normalList);
        InitTagGrid();
    }

    private void OnEnable()
    {
        HandleTagData();
    }

    List<int> selectDeviceTagID = new List<int>() { -1 };
    Dictionary<int, TreeMap> dic = new Dictionary<int, TreeMap>();
    void HandleTagData()
    {
        dic.Clear();
        WWWForm form = new WWWForm();
        form.AddField("requestType", "SelectAllDeviceTag");
        GameUtils.PostHttp("DeviceTag.ashx", form, (result) =>
        {
            List<DeviceTag> list = JsonMapper.ToObject<List<DeviceTag>>(result);
            for (int i = 0; i < list.Count; i++)
            {
                DeviceTag deviceTag = list[i];
                TreeMap treeMap = new TreeMap();
                treeMap.ID = deviceTag.ID;
                treeMap.parentID = deviceTag.ParentID;
                treeMap.tagName = deviceTag.TagName;
                List<DeviceTag> temp = list.FindAll(it => it.ParentID == deviceTag.ID);
                List<int> childIdList = new List<int>();
                for (int j = 0; j < temp.Count; j++)
                {
                    childIdList.Add(temp[j].ID);
                }
                treeMap.childIDList = childIdList;
                dic[deviceTag.ID] = treeMap;
            }
        }, null);
    }

    void InitTagGrid()
    {
        foreach (Transform child in tagContentTrans.transform)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in probeContentTrans.transform)
        {
            Destroy(child.gameObject);
        }
        if (selectDeviceTagID[selectDeviceTagID.Count - 1] == -1)
        {
            tagContentTrans.gameObject.SetActive(true);
            probeContentTrans.gameObject.SetActive(false);
            foreach (var item in dic)
            {
                if (item.Value.parentID == -1)
                {
                    InstanceDeviceTag(item.Value);
                    break;
                }
            }
        }
        else
        {
            TreeMap treeMap = dic[selectDeviceTagID[selectDeviceTagID.Count - 1]];
            tagContentTrans.gameObject.SetActive(treeMap.childIDList.Count > 0);
            probeContentTrans.gameObject.SetActive(treeMap.childIDList.Count == 0);
            if (treeMap.childIDList.Count > 0)
            {
                for (int i = 0; i < treeMap.childIDList.Count; i++)
                {
                    int childID = treeMap.childIDList[i];
                    InstanceDeviceTag(dic[childID]);
                }
            }
            else
            {
                List<RealtimeDataModel> list = GetRealtimeDataModelList(new List<string>() { treeMap.tagName });
                for (int i = 0; i < list.Count; i++)
                {
                    InstanceProbe(list[i]);
                }
            }
        }
    }

    void InstanceProbe(RealtimeDataModel model)
    {
        GameObject currentObj = Instantiate(probeItemRes) as GameObject;
        currentObj.transform.SetParent(probeContentTrans);
        currentObj.transform.localScale = Vector3.one;
        currentObj.GetComponent<RectTransform>().anchoredPosition3D = Vector3.zero;
        CubeInfoItem cubeInfoItem = currentObj.GetComponentInChildren<CubeInfoItem>();
        cubeInfoItem.InitData(model);
        currentObj.SetActive(true);
    }

    void InstanceDeviceTag(TreeMap treeMap)
    {
        GameObject currentObj = Instantiate(tagItemRes) as GameObject;
        currentObj.transform.SetParent(tagContentTrans);
        currentObj.transform.localScale = Vector3.one;
        currentObj.GetComponent<RectTransform>().anchoredPosition3D = Vector3.zero;
        TagInfoItem tagInfoItem = currentObj.GetComponentInChildren<TagInfoItem>();

        List<string> bottomIDList = new List<string>();
        GetBottomIDList(bottomIDList, treeMap.ID);
        tagInfoItem.InitData(GetRealtimeDataModelList(bottomIDList), treeMap);
        RegisterBtnClick<TreeMap>(currentObj.GetComponent<Button>(), treeMap, OnTagClick);
        currentObj.SetActive(true);
    }

    void OnTagClick(Button btn, TreeMap data)
    {
        TreeMap treeMap = (TreeMap)data;
        selectDeviceTagID.Add(treeMap.ID);
    }

    void GetBottomIDList(List<string> ids, int id)
    {
        foreach (var item in dic)
        {
            if (item.Value.parentID == id)
            {
                if (item.Value.childIDList.Count == 0)
                {
                    ids.Add(item.Value.tagName);
                }
                else
                {
                    GetBottomIDList(ids, item.Value.ID);
                }
            }
        }
        if (ids.Count == 0)
        {
            ids.Add(dic[id].tagName);
        }
    }

    List<RealtimeDataModel> GetRealtimeDataModelList(List<string> ids)
    {
        List<RealtimeDataModel> list = new List<RealtimeDataModel>();
        for (int i = 0; i < ids.Count; i++)
        {
            string tagName = ids[i];
            List<RealtimeDataModel> modelList = realtimeDataModelList.FindAll(it => it.TagName == tagName);
            list.AddRange(modelList);
        }
        return list;
    }
}
