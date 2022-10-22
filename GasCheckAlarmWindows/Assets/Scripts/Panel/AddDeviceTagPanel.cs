using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AddDeviceTagPanel : UIEventHelper
{
    public InputField input_deviceTagName;
    [HideInInspector]
    public Vector2 position;

    public Button btn_cancel;
    public Button btn_Add;
    void Start()
    {
        RegisterBtnClick(btn_cancel, OnCancel);
        RegisterBtnClick(btn_Add, OnAdd);
    }

    void OnCancel(Button btn)
    {
        gameObject.SetActive(false);
    }

    void OnAdd(Button btn)
    {
        string deviceTagName = input_deviceTagName.text;
        if (!string.IsNullOrEmpty(deviceTagName))
        {
            int insertID = DeviceTagDAL.InsertDeviceTag(deviceTagName, -1, position.x + "," + position.y);
            MessageBox.Instance.PopOK("新增成功", () =>
            {
                TreeMap treeMap = new TreeMap();
                treeMap.ID = insertID;
                treeMap.parentID = -1;
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
