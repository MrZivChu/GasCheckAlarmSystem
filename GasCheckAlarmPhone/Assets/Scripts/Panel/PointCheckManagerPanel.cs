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

    public InputField input_deviceName;
    public InputField input_userName;
    public Dropdown dropdown_deviceType;
    public UI.Dates.DatePicker_DateRange dateRange;

    public Transform contentTrans;
    public UnityEngine.Object itemRes;
    private void Start()
    {
        RegisterBtnClick(btn_search, OnSearch);
        RegisterBtnClick(btn_prePage, OnPrePagel);
        RegisterBtnClick(btn_nextPage, OnNextPage);
        RegisterDropDownOnValueChanged(dropdown_deviceType, OnDropdownChanged);
    }

    void OnSearch(Button btn)
    {
        pageIndex = 1;
        InitData();
    }

    void OnDropdownChanged(Dropdown dropdown, int value)
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

        List<PointCheckModel> pointCheckModelList = PointCheckDAL.SelectAllPointCheckByCondition(pageIndex, pageSize, input_userName.text, input_deviceName.text, dropdown_deviceType.value, startTime, endTime, out pageCount, out rowCount);
        InitGrid(pointCheckModelList);
        txt_pageCount.text = pageIndex + "/" + pageCount;
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
            RegisterBtnClick<PointCheckModel>(currentObj.GetComponent<Button>(), data, ClickItem);
            item.SetBackgroundColor(index % 2 == 0 ? new Color(239 / 255.0f, 243 / 255.0f, 250 / 255.0f) : new Color(1, 1, 1));
            currentObj.SetActive(true);
        }, false);
    }


    public WaterSealDetailPanel waterSealDetailPanel;
    public ProbeDetailPanel probeDetailPanel;
    void ClickItem(Button btn, PointCheckModel model)
    {
        if (model.DeviceType == 0)
        {
            probeDetailPanel.OnInit(model);
            probeDetailPanel.gameObject.SetActive(true);
        }
        else
        {
            waterSealDetailPanel.OnInit(model);
            waterSealDetailPanel.gameObject.SetActive(true);
        }
    }
}