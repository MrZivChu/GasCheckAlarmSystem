using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class PointCheckManagerPanel : UIEventHelper
{
    public Button btn_search;
    public Button btn_prePage;
    public Button btn_nextPage;
    public Text txt_pageCount;
    int pageIndex = 1;
    int pageCount = 1;
    int rowCount = 1;
    int pageSize = 16;

    public InputField input_probeName;
    public InputField input_userName;
    public UI.Dates.DatePicker_DateRange dateRange;

    public Transform contentTrans;
    public UnityEngine.Object itemRes;

    private void Start()
    {
        RegisterBtnClick(btn_search, OnSearch);
        RegisterBtnClick(btn_prePage, OnPrePagel);
        RegisterBtnClick(btn_nextPage, OnNextPage);
    }

    void OnSearch(Button btn)
    {
        pageIndex = 1;
        InitData();
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

    void InitData()
    {
        string startTime = string.Empty;
        if (dateRange.FromDate.HasValue)
            startTime = dateRange.FromDate.Date.ToString("yyyy-MM-dd");
        string endTime = string.Empty;
        if (dateRange.ToDate.HasValue)
            endTime = dateRange.ToDate.Date.AddDays(1).ToString("yyyy-MM-dd");

        WWWForm form = new WWWForm();
        form.AddField("requestType", "SelectAllPointCheckByCondition");
        form.AddField("pageIndex", pageIndex);
        form.AddField("pageSize", pageSize);
        form.AddField("userName", input_userName.text);
        form.AddField("probeName", input_probeName.text);
        form.AddField("startTime", startTime);
        form.AddField("endTime", endTime);
        form.AddField("pageCount", pageCount);
        form.AddField("rowCount", rowCount);
        GameUtils.PostHttp("PointCheck.ashx", form, (result) =>
        {
            List<PointCheckModel> pointCheckModelList = new List<PointCheckModel>();
            if (result.Contains("|"))
            {
                string pageResult = result.Split('|')[0];
                pageCount = Convert.ToInt32(pageResult.Split(',')[0]);
                rowCount = Convert.ToInt32(pageResult.Split(',')[1]);
                result = result.Split('|')[1];
                pointCheckModelList = JsonMapper.ToObject<List<PointCheckModel>>(result);
            }
            InitGrid(pointCheckModelList);
            txt_pageCount.text = pageIndex + "/" + pageCount;
        }, null);
    }

    void InitGrid(List<PointCheckModel> pointCheckModelList)
    {
        GameUtils.SpawnCellForTable<PointCheckModel>(contentTrans, pointCheckModelList, (go, data, isSpawn, index) =>
        {
            GameObject currentObj = go;
            if (isSpawn)
            {
                currentObj = Instantiate(itemRes) as GameObject;
                currentObj.transform.SetParent(contentTrans);
                currentObj.transform.localScale = Vector3.one;
                currentObj.GetComponent<RectTransform>().anchoredPosition3D = Vector3.zero;
            }
            PointCheckItem item = currentObj.GetComponent<PointCheckItem>();
            item.InitData(pageSize * (pageIndex - 1) + index + 1, data);
            currentObj.SetActive(true);
        }, false);
    }
}