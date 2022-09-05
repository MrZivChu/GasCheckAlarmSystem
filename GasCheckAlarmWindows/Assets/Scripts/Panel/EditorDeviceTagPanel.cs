using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditorDeviceTagPanel : UIEventHelper
{
    public InputField input_deviceTagName;
    [HideInInspector]
    public int parentID;
    [HideInInspector]
    public Vector2 position;
    [HideInInspector]
    public string deleteIDList;

    public Button btn_cancel;
    public Button btn_Add;
    public Button btn_Delete;
    void Start()
    {
        RegisterBtnClick(btn_cancel, OnCancel);
        RegisterBtnClick(btn_Add, OnAdd);
        RegisterBtnClick(btn_Delete, OnDelete);
    }

    void OnCancel(Button btn)
    {
        gameObject.SetActive(false);
    }

    void OnDelete(Button btn)
    {
        DeviceTagDAL.DeleteDeviceTagByID(deleteIDList);
        EventManager.Instance.DisPatch(NotifyType.DeleteDeviceTag);
        gameObject.SetActive(false);
    }

    void OnAdd(Button btn)
    {
        string deviceTagName = input_deviceTagName.text;
        if (!string.IsNullOrEmpty(deviceTagName))
        {
            int insertID = DeviceTagDAL.InsertDeviceTag(deviceTagName, parentID, position.x + "," + position.y);
            MessageBox.Instance.PopOK("新增成功", () =>
            {
                TreeMap treeMap = new TreeMap();
                treeMap.ID = insertID;
                treeMap.parentID = parentID;
                treeMap.position = position;
                treeMap.tagName = deviceTagName;
                treeMap.childIDList = new List<int>() { };
                EventManager.Instance.DisPatch(NotifyType.InsertDeviceTag, treeMap);
                gameObject.SetActive(false);
            }, "确定");
        }
    }

    private void OnEnable()
    {
        input_deviceTagName.text = string.Empty;
    }
}
