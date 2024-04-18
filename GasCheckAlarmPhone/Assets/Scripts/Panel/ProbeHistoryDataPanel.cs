using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class ProbeHistoryDataPanel : UIEventHelper
{
    public Button btn_search;

    public Button btn_prePage;
    public Button btn_nextPage;
    public Text txt_pageCount;
    public InputField input_probeName;
    int pageIndex = 1;
    int pageCount = 1;
    int rowCount = 1;
    int pageSize = 13;
    int probeID = 0;

    public ChoiceTime startTime;
    public ChoiceTime endTime;

    public Transform contentTrans;
    public UnityEngine.Object itemRes;

    private void Start()
    {
        RegisterBtnClick(btn_search, OnSearch);
        RegisterBtnClick(btn_prePage, OnPrePagel);
        RegisterBtnClick(btn_nextPage, OnNextPage);
        startTime.SetTime(DateTime.Now.AddDays(-7));
        endTime.SetTime(DateTime.Now);
        OnEnable();
    }

    List<ProbeModel> baseInfoList_ = new List<ProbeModel>();
    private void OnEnable()
    {
        baseInfoList_ = ProbeDAL.SelectIDProbeNameGasKindMachineID();
        InitData();
    }

    void OnPrePagel(Button btn)
    {
        if (pageIndex - 1 >= 1)
        {
            pageIndex--;
            InitData();
        }
    }

    void OnNextPage(Button btn)
    {
        if (pageIndex + 1 <= pageCount)
        {
            pageIndex++;
            InitData();
        }
    }

    void OnDeleteAllData(Button btn)
    {
        HistoryDataDAL.DeleteAllHistoryData();
        InitData();
    }

    void OnSearch(Button btn)
    {
        if (!string.IsNullOrEmpty(input_probeName.text))
        {
            probeID = ProbeDAL.SelectProbeIDByProbeName(input_probeName.text);
        }
        else
        {
            probeID = 0;
        }
        pageIndex = 1;
        InitData();
    }

    List<HistoryDataModel> historyDataModelList;
    private void InitData()
    {
        historyDataModelList = HistoryDataDAL.SelectAllHistoryDataByCondition(pageIndex, pageSize, startTime.ShowText.text, endTime.ShowText.text, probeID, out pageCount, out rowCount);
        foreach (var item in historyDataModelList)
        {
            ProbeModel model = baseInfoList_.Find(it => it.ID == item.ProbeID);
            if (model != null)
            {
                item.ProbeID = model.ID;
                item.probeName = model.ProbeName;
                item.gasKind = model.GasKind;
                item.MachineID = model.MachineID;

                if (FormatData.gasKindFormat[model.GasKind].GasName == "氧气" || FormatData.gasKindFormat[model.GasKind].GasName == "天然气" || FormatData.gasKindFormat[model.GasKind].GasName == "石油气" || FormatData.gasKindFormat[model.GasKind].GasName == "可燃气")
                {
                    item.GasValue = item.GasValue / 10.0f;
                }
            }
        }
        txt_pageCount.text = pageIndex + "/" + pageCount;
        InitGrid(historyDataModelList);
    }

    void InitGrid(List<HistoryDataModel> historyDataModelList)
    {
        GameUtils.SpawnCellForTable<HistoryDataModel>(contentTrans, historyDataModelList, (go, data, isSpawn, index) =>
        {
            GameObject currentObj = go;
            if (isSpawn)
            {
                currentObj = Instantiate(itemRes) as GameObject;
                currentObj.transform.SetParent(contentTrans);
                currentObj.transform.localScale = Vector3.one;
                currentObj.GetComponent<RectTransform>().anchoredPosition3D = Vector3.zero;
            }
            ProbeHistoryDataItem item = currentObj.GetComponent<ProbeHistoryDataItem>();
            item.InitData(pageSize * (pageIndex - 1) + index + 1, data);
            item.SetBackgroundColor(index % 2 == 0 ? new Color(239 / 255.0f, 243 / 255.0f, 250 / 255.0f) : new Color(1, 1, 1));
            currentObj.SetActive(true);
        });
    }
}