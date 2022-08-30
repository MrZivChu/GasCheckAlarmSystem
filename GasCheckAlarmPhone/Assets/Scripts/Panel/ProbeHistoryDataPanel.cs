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
    public Button btn_deleteAllData;

    public Button btn_prePage;
    public Button btn_nextPage;
    public Text txt_pageCount;
    int pageIndex = 1;
    int pageCount = 1;
    int rowCount = 1;
    int pageSize = 18;

    public InputField input_probeName;
    public InputField input_gasKind;
    public UI.Dates.DatePicker_DateRange dateRange;

    public Transform contentTrans;
    UnityEngine.Object itemRes;
    void Awake()
    {
        itemRes = Resources.Load("ProbeHistoryDataItem");
        btn_deleteAllData.gameObject.SetActive(FormatData.currentUser.Authority == 1);
    }

    private void Start()
    {
        RegisterBtnClick(btn_search, OnSearch);
        RegisterBtnClick(btn_deleteAllData, OnDeleteAllData);

        RegisterBtnClick(btn_prePage, OnPrePagel);
        RegisterBtnClick(btn_nextPage, OnNextPage);
    }

    private void OnEnable()
    {
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
        WWWForm form = new WWWForm();
        form.AddField("requestType", "DeleteAllHistoryData");
        GameUtils.PostHttp("HistoryData.ashx", form, null, null);
        InitData();
    }

    void OnSearch(Button btn)
    {
        pageIndex = 1;
        InitData();
    }

    private void InitData()
    {
        string startTime = string.Empty;
        if (dateRange.FromDate.HasValue)
            startTime = dateRange.FromDate.Date.ToString("yyyy-MM-dd");
        string endTime = string.Empty;
        if (dateRange.ToDate.HasValue)
            endTime = dateRange.ToDate.Date.AddDays(1).ToString("yyyy-MM-dd");

        WWWForm form = new WWWForm();
        form.AddField("requestType", "SelectAllHistoryDataByCondition");
        form.AddField("pageIndex", pageIndex);
        form.AddField("pageSize", pageSize);
        form.AddField("probeName", input_probeName.text);
        form.AddField("gasKind", input_gasKind.text);
        form.AddField("startTime", startTime);
        form.AddField("endTime", endTime);
        form.AddField("pageCount", pageCount);
        form.AddField("rowCount", rowCount);
        GameUtils.PostHttp("HistoryData.ashx", form, (result) =>
        {
            List<HistoryDataModel> historyDataModelList = new List<HistoryDataModel>();
            if (result.Contains("*"))
            {
                string pageResult = result.Split('*')[0];
                pageCount = Convert.ToInt32(pageResult.Split(',')[0]);
                rowCount = Convert.ToInt32(pageResult.Split(',')[1]);
                result = result.Split('*')[1];
                historyDataModelList = JsonMapper.ToObject<List<HistoryDataModel>>(result);

            }
            InitGrid(historyDataModelList);
            txt_pageCount.text = pageIndex + "/" + pageCount;
        }, null);
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