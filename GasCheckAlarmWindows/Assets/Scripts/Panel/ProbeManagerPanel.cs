﻿using LitJson;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class ProbeManagerPanel : UIEventHelper
{
    public Button btn_add;
    public Button btn_delete;
    public Button btn_edit;
    public Button btn_search;
    public Toggle wholeToggle;

    public GameObject addProbePanel;
    public GameObject editProbePanel;

    public InputField input_probeName;
    public InputField input_gasKind;

    public Transform contentTrans;
    public Object itemRes;
    private void Start()
    {
        RegisterBtnClick(btn_add, OnAddProbe);
        RegisterBtnClick(btn_delete, OnDeleteProbe);
        RegisterBtnClick(btn_edit, OnEditProbe);
        RegisterBtnClick(btn_search, OnSearchProbe);
        RegisterTogClick(wholeToggle, OnWholeToggle);
        EventManager.Instance.AddEventListener(NotifyType.UpdateProbeList, UpdateProbeListEvent);
    }

    void UpdateProbeListEvent(object data)
    {
        InitData();
    }

    void OnSearchProbe(Button btn)
    {
        string probeName = input_probeName.text;
        string gasKind = input_gasKind.text;
        WWWForm form = new WWWForm();
        form.AddField("requestType", "SelectAllProbeByCondition");
        form.AddField("probeName", probeName);
        form.AddField("gasKind", gasKind);
        GameUtils.PostHttp("Probe.ashx", form, (result) =>
        {
            List<ProbeModel> list = JsonMapper.ToObject<List<ProbeModel>>(result);
            InitGrid(list);
        }, null);
    }

    void OnAddProbe(Button btn)
    {
        if (addProbePanel != null)
        {
            addProbePanel.SetActive(true);
        }
    }

    void OnDeleteProbe(Button btn)
    {
        List<ProbeModel> idList = new List<ProbeModel>();
        if (probeItemList != null && probeItemList.Count > 0)
        {
            for (int i = 0; i < probeItemList.Count; i++)
            {
                if (probeItemList[i].GetToggleStatus())
                {
                    idList.Add(probeItemList[i].currentModel);
                }
            }
        }
        if (idList.Count <= 0)
        {
            MessageBox.Instance.PopOK("未选择任何数据", null, "确定");
        }
        else
        {
            MessageBox.Instance.PopYesNo("确认删除？", null, () =>
            {
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < idList.Count; i++)
                {
                    sb.Append(idList[i].ID + ",");
                }
                sb = sb.Remove(sb.Length - 1, 1);
                WWWForm form = new WWWForm();
                form.AddField("requestType", "DeleteProbeByID");
                form.AddField("idList", sb.ToString());
                GameUtils.PostHttp("Probe.ashx", form, (content) =>
                {
                    EventManager.Instance.DisPatch(NotifyType.UpdateProbeList);
                    MessageBox.Instance.PopOK("删除成功", null, "确定");
                }, null);
            }, "取消", "确定");
        }
    }

    void OnEditProbe(Button btn)
    {
        List<ProbeItem> selectItemList = new List<ProbeItem>();
        if (probeItemList != null && probeItemList.Count > 0)
        {
            for (int i = 0; i < probeItemList.Count; i++)
            {
                if (probeItemList[i].GetToggleStatus())
                {
                    selectItemList.Add(probeItemList[i]);
                }
            }
        }
        if (selectItemList.Count <= 0)
        {
            MessageBox.Instance.PopOK("未选择任何数据", null, "确定");
        }
        else if (selectItemList.Count > 1)
        {
            MessageBox.Instance.PopOK("一次只能选择一条数据", null, "确定");
        }
        else
        {
            ProbeItem selectItem = selectItemList[0];
            editProbePanel.SetActive(true);
            EditProbePanel panel = editProbePanel.GetComponent<EditProbePanel>();
            panel.InitData(selectItem.currentModel);
        }
    }

    void OnWholeToggle(Toggle tog, bool isOn)
    {
        if (probeItemList != null && probeItemList.Count > 0)
        {
            for (int i = 0; i < probeItemList.Count; i++)
            {
                probeItemList[i].SetToggle(isOn);
            }
        }
    }

    private void OnEnable()
    {
        InitData();
    }

    List<ProbeItem> probeItemList = new List<ProbeItem>();
    private void InitData()
    {
        WWWForm form = new WWWForm();
        form.AddField("requestType", "SelectAllProbeByCondition");
        GameUtils.PostHttp("Probe.ashx", form, (result) =>
        {
            List<ProbeModel> list = JsonMapper.ToObject<List<ProbeModel>>(result);
            InitGrid(list);
        }, null);
    }

    void InitGrid(List<ProbeModel> list)
    {
        probeItemList.Clear();
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
            ProbeItem item = currentObj.GetComponent<ProbeItem>();
            item.InitData(data);
            item.SetBackgroundColor(index % 2 == 0 ? new Color(239 / 255.0f, 243 / 255.0f, 250 / 255.0f) : new Color(1, 1, 1));
            probeItemList.Add(item);
            currentObj.SetActive(true);
        });
    }
}
