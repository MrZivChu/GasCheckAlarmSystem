using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO.Ports;
using System;
using System.IO;
using LitJson;

public class TreeMap
{
    public int ID;
    public int parentID;
    public string tagName;
    public Vector3 position;
    public List<int> childIDList;
    public GameObject target;
}

public class DeviceTagManagerPanel : UIEventHelper
{
    public EditorDeviceTagPanel editorDeviceTagPanel;
    public GameObject panel;
    public Line2DHelper line2DHelperClone;
    public MyButton cloneBtn;
    void Start()
    {
        EventManager.Instance.AddEventListener(NotifyType.InsertDeviceTag, InsertDeviceTagCallBack);
        EventManager.Instance.AddEventListener(NotifyType.DeleteDeviceTag, DeleteDeviceTagCallBack);
    }

    private void OnDestroy()
    {
        EventManager.Instance.DeleteEventListener(NotifyType.InsertDeviceTag, InsertDeviceTagCallBack);
        EventManager.Instance.DeleteEventListener(NotifyType.DeleteDeviceTag, DeleteDeviceTagCallBack);
    }

    private void OnEnable()
    {
        InitPanel();
    }

    //自己的ID，父类的ID，子类ID集和
    List<TreeMap> treeMapList = new List<TreeMap>();
    void InitPanel()
    {
        treeMapList.Clear();
        foreach (Transform child in panel.transform)
        {
            Destroy(child.gameObject);
        }
        List<DeviceTagModel> list = DeviceTagDAL.SelectAllDeviceTag();
        for (int i = 0; i < list.Count; i++)
        {
            DeviceTagModel deviceTag = list[i];
            TreeMap treeMap = new TreeMap();
            treeMap.ID = deviceTag.ID;
            treeMap.parentID = deviceTag.ParentID;
            treeMap.tagName = deviceTag.TagName;
            string[] pos = deviceTag.Position.Split(new string[] { "," }, System.StringSplitOptions.None);
            treeMap.position = new Vector3(Convert.ToSingle(pos[0]), Convert.ToSingle(pos[1]), 0);
            List<DeviceTagModel> temp = list.FindAll(it => it.ParentID == deviceTag.ID);
            List<int> childIdList = new List<int>();
            for (int j = 0; j < temp.Count; j++)
            {
                childIdList.Add(temp[j].ID);
            }
            treeMap.childIDList = childIdList;
            treeMapList.Add(treeMap);
            InstanceCloneBtn(treeMap);
        }
        for (int j = 0; j < treeMapList.Count; j++)
        {
            TreeMap treeMap = treeMapList[j];
            if (treeMap.parentID == -1)
            {
                continue;
            }
            InstanceLine2DHelper(treeMap);
        }
    }

    void InstanceLine2DHelper(TreeMap treeMap)
    {
        TreeMap parentTreeMap = treeMapList.Find(it => it.ID == treeMap.parentID);
        GameObject currentObj = Instantiate(line2DHelperClone.gameObject) as GameObject;
        currentObj.transform.SetParent(panel.transform);
        currentObj.transform.localScale = Vector3.one;
        currentObj.GetComponent<RectTransform>().anchoredPosition3D = Vector3.one;
        Line2DHelper line2DHelper = currentObj.GetComponent<Line2DHelper>();
        line2DHelper.Init(parentTreeMap.target.GetComponent<RectTransform>(), treeMap.target.GetComponent<RectTransform>());
        currentObj.SetActive(true);
    }

    void InstanceCloneBtn(TreeMap treeMap)
    {
        GameObject currentObj = Instantiate(cloneBtn.gameObject) as GameObject;
        currentObj.transform.SetParent(panel.transform);
        currentObj.transform.localScale = Vector3.one;
        currentObj.GetComponent<RectTransform>().anchoredPosition3D = treeMap.position;
        currentObj.GetComponent<DeviceTagItemInfo>().InitData(treeMap);
        treeMap.target = currentObj;
        currentObj.SetActive(true);
        RegisterTargetBtn(currentObj.GetComponent<MyButton>());
    }

    void RegisterTargetBtn(MyButton btn)
    {
        RegisterMyBtnLongPress(btn, OnLongPressTargetBtn);
        RegisterMyBtnClick(btn, OnBtnClickTargetBtn);
        RegisterMyBtnUp(btn, OnBtnUpTargetBtn);
    }

    void InsertDeviceTagCallBack(object obj)
    {
        TreeMap treeMap = (TreeMap)obj;
        treeMapList.Add(treeMap);
        InstanceCloneBtn(treeMap);
        InstanceLine2DHelper(treeMap);
    }

    void DeleteDeviceTagCallBack(object obj)
    {
        InitPanel();
    }

    bool isLongPress = false;
    MyButton targetBtn = null;
    Vector3 targetPosition;
    int parentID = 0;
    void OnLongPressTargetBtn(MyButton btn)
    {
        isLongPress = true;
        targetBtn = btn;
    }

    void OnBtnUpTargetBtn(MyButton btn)
    {
        if (targetBtn)
        {
            Vector2 pos = targetBtn.GetComponent<RectTransform>().anchoredPosition;
            DeviceTagItemInfo deviceTagItemInfo = targetBtn.GetComponent<DeviceTagItemInfo>();
            DeviceTagDAL.EditDeviceTagByID(deviceTagItemInfo.model.ID, pos.x + "," + pos.y);
            targetBtn = null;
        }
        isLongPress = false;
    }

    void OnBtnClickTargetBtn(MyButton btn)
    {
        DeviceTagItemInfo deviceTagItemInfo = btn.GetComponent<DeviceTagItemInfo>();
        parentID = deviceTagItemInfo.model.ID;
        editorDeviceTagPanel.parentID = parentID;
        Vector3 vec = btn.GetComponent<RectTransform>().anchoredPosition;
        Vector3 size = btn.GetComponent<RectTransform>().rect.size;
        targetPosition.x = vec.x;
        targetPosition.y = vec.y - size.y * 1.5f;
        editorDeviceTagPanel.position = targetPosition;
        editorDeviceTagPanel.gameObject.SetActive(true);

        List<int> deleteIDList = new List<int>();
        GetDeleteIDList(deleteIDList, parentID);
        string deleteStr = string.Empty;
        for (int i = 0; i < deleteIDList.Count; i++)
        {
            deleteStr += deleteIDList[i] + ",";
        }
        editorDeviceTagPanel.deleteIDList = deleteStr.TrimEnd(',');
    }

    void GetDeleteIDList(List<int> ids, int id)
    {
        ids.Add(id);
        List<TreeMap> list = treeMapList.FindAll(it => it.parentID == id);
        for (int i = 0; i < list.Count; i++)
        {
            GetDeleteIDList(ids, list[i].ID);
        }
    }

    public Camera uiCamera;
    private void Update()
    {
        if (isLongPress && targetBtn)
        {
            RectTransform graphPanelRT = panel.GetComponent<RectTransform>();
            float halfWidth = graphPanelRT.rect.size.x / 2;
            float halfHeight = graphPanelRT.rect.size.y / 2;
            Vector2 uiPosition;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(graphPanelRT, Input.mousePosition, uiCamera, out uiPosition);
            Vector2 targetSize = targetBtn.GetComponent<RectTransform>().rect.size;
            if (Mathf.Abs(uiPosition.x) > halfWidth || Mathf.Abs(uiPosition.y) > halfHeight)
            {
                print("click out range");
                return;
            }
            uiPosition.y += targetSize.y / 2;
            targetBtn.GetComponent<RectTransform>().localPosition = uiPosition;
        }
    }
}
