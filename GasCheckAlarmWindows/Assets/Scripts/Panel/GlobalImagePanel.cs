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
    public Text tipText;

    private void Start()
    {
        EventManager.Instance.AddEventListener(NotifyType.UpdateRealtimeDataList, UpdateRealtimeData);
        RegisterBtnClick(preStepBtn, OnPreStep);
    }

    private void OnDestroy()
    {
        EventManager.Instance.DeleteEventListener(NotifyType.UpdateRealtimeDataList, UpdateRealtimeData);
    }

    void OnPreStep(Button btn)
    {
        if (selectDeviceTagID.Count > 1)
        {
            selectDeviceTagID.RemoveAt(selectDeviceTagID.Count - 1);
            UpdateRealtimeData(realtimeData_);
        }
    }

    List<ProbeModel> realtimeData_ = new List<ProbeModel>();
    void UpdateRealtimeData(object data)
    {
        if (!gameObject || !gameObject.activeSelf)
        {
            return;
        }
        realtimeData_ = (List<ProbeModel>)data;
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
        List<DeviceTagModel> list = DeviceTagDAL.SelectAllDeviceTag();
        tipText.gameObject.SetActive(list.Count == 0);
        preStepBtn.gameObject.SetActive(selectDeviceTagID.Count > 1);
        for (int i = 0; i < list.Count; i++)
        {
            DeviceTagModel deviceTag = list[i];
            TreeMap treeMap = new TreeMap();
            treeMap.ID = deviceTag.ID;
            treeMap.parentID = deviceTag.ParentID;
            treeMap.tagName = deviceTag.TagName;
            List<DeviceTagModel> temp = list.FindAll(it => it.ParentID == deviceTag.ID);
            List<int> childIdList = new List<int>();
            for (int j = 0; j < temp.Count; j++)
            {
                childIdList.Add(temp[j].ID);
            }
            treeMap.childIDList = childIdList;
            dic[deviceTag.ID] = treeMap;
        }
    }

    void InitTagGrid()
    {
        preStepBtn.gameObject.SetActive(selectDeviceTagID.Count > 1);
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
                List<ProbeModel> list = GetRealtimeDataModelList(new List<string>() { treeMap.tagName });
                for (int i = 0; i < list.Count; i++)
                {
                    InstanceProbe(list[i]);
                }
            }
        }
    }

    void InstanceProbe(ProbeModel model)
    {
        GameObject currentObj = Instantiate(probeItemRes) as GameObject;
        currentObj.transform.SetParent(probeContentTrans);
        currentObj.transform.localScale = Vector3.one;
        currentObj.GetComponent<RectTransform>().anchoredPosition3D = Vector3.zero;
        GlobalImageProbeInfoItem cubeInfoItem = currentObj.GetComponentInChildren<GlobalImageProbeInfoItem>();
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
        UpdateRealtimeData(realtimeData_);
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

    List<ProbeModel> GetRealtimeDataModelList(List<string> ids)
    {
        List<ProbeModel> list = new List<ProbeModel>();
        for (int i = 0; i < ids.Count; i++)
        {
            string tagName = ids[i];
            List<ProbeModel> modelList = new List<ProbeModel>();
            realtimeData_.ForEach(it =>
            {
                if (it.TagName == tagName)
                {
                    modelList.Add(it);
                }
            });
            list.AddRange(modelList);
        }
        return list;
    }
}
