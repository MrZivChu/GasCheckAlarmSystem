﻿using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectProbeForGraphPanel : UIEventHelper
{
    ProbeModel probeModel;
    public Transform contentTrans;
    public GameObject itemRes;
    public ToggleGroup togGroup;

    public Button btn_cancel;
    public Button btn_ok;
    [HideInInspector]
    public Vector3 uiPos;
    void Awake()
    {
        RegisterBtnClick(btn_cancel, OnCancel);
        RegisterBtnClick(btn_ok, OnOk);
    }

    void OnCancel(Button btn)
    {
        gameObject.SetActive(false);
    }

    void OnOk(Button btn)
    {
        ProbeDAL.EditProbePos2DByID(probeModel.ID, uiPos.x + "," + uiPos.y);
        MessageBox.Instance.PopOK("新增成功", () =>
        {
            EventManager.Instance.DisPatch(NotifyType.UpdatePos2D);
            gameObject.SetActive(false);
        }, "确定");
    }


    void OnEnable()
    {
        InitData();
    }

    private void InitData()
    {
        List<ProbeModel> list = ProbeDAL.SelectIDProbeNameMachineIDWherePos2DNoValue();
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
                currentObj.GetComponent<RectTransform>().anchoredPosition3D = Vector3.zero;
            }
            SelectProbeForGraphItem item = currentObj.GetComponent<SelectProbeForGraphItem>();
            item.InitData(data);
            Toggle tog = currentObj.GetComponent<Toggle>();
            tog.isOn = false;
            tog.group = togGroup;
            RegisterTogClick<SelectProbeForGraphItem>(tog, item, OnTogSelected);
            currentObj.SetActive(true);
        });
    }

    void OnTogSelected(Toggle tog, bool isOn, SelectProbeForGraphItem data)
    {
        if (isOn)
        {
            probeModel = data.probeModel;
        }
    }
}
