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
    public Button btn_saveExcel;
    public Button btn_deleteAllData;

    public Button btn_prePage;
    public Button btn_nextPage;
    public Text txt_pageCount;
    int pageIndex = 1;
    int pageCount = 1;
    int rowCount = 1;
    int pageSize = 13;

    public InputField input_probeName;
    public InputField input_gasKind;
    public InputField input_pageSize;
    public UI.Dates.DatePicker_DateRange dateRange;

    public Transform contentTrans;
    UnityEngine.Object itemRes;

    List<HistoryDataModel> historyDataModelList;
    public static ProbeHistoryDataPanel instance;

    void Awake()
    {
        instance = this;
        itemRes = Resources.Load("ProbeHistoryDataItem");
    }

    private void Start()
    {
        RegisterBtnClick(btn_search, OnSearchRealtimeData);
        RegisterBtnClick(btn_saveExcel, OnSaveExcel);
        RegisterBtnClick(btn_deleteAllData, OnDeleteAllData);

        RegisterBtnClick(btn_prePage, OnPrePagel);
        RegisterBtnClick(btn_nextPage, OnNextPage);

        RegisterInputFieldOnEndEdit(input_pageSize, OnPageSizeOnEndEdit);
        btn_deleteAllData.gameObject.SetActive(FormatData.currentUser != null && FormatData.currentUser.Authority == 1);
    }

    void OnPageSizeOnEndEdit(InputField target, string content)
    {
        if (!int.TryParse(content, out pageSize))
        {
            pageSize = 13;
            input_pageSize.text = pageSize.ToString();
        }
    }

    private void Update()
    {
        txt_pageCount.text = pageIndex + "/" + pageCount;
    }

    public List<HistoryDataModel> GetHistoryDataModelList()
    {
        return historyDataModelList;
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

    void OnSaveExcel(Button btn)
    {
        //System.Windows.Forms.SaveFileDialog sfd = new System.Windows.Forms.SaveFileDialog();
        //sfd.Filter = "Excel文件(*.xlsx)|*.xlsx|所有文件|*.*";//设置文件类型
        //sfd.FileName = "探头实时数据";//设置默认文件名
        //sfd.DefaultExt = "xlsx";//设置默认格式（可以不设）
        //sfd.AddExtension = true;//设置自动在文件名中添加扩展名
        //if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
        //{
        //    if (!string.IsNullOrEmpty(sfd.FileName))
        //    {
        //        System.DateTime startTime = System.DateTime.MinValue, endTime = System.DateTime.MinValue;
        //        if (dateRange.FromDate.HasValue)
        //            startTime = dateRange.FromDate.Date;
        //        if (dateRange.ToDate.HasValue)
        //            endTime = dateRange.ToDate.Date;
        //        ExcelAccess.WriteExcel(sfd.FileName, "sheet1", historyDataModelList);
        //        MessageBox.Instance.PopOK("保存成功", null, "确定");
        //    }
        //}
    }

    void OnDeleteAllData(Button btn)
    {
        WWWForm form = new WWWForm();
        form.AddField("requestType", "DeleteAllHistoryData");
        GameUtils.PostHttp("HistoryData.ashx", form, null, null);
        InitData();
    }

    void OnSearchRealtimeData(Button btn)
    {
        System.DateTime startTime = System.DateTime.MinValue, endTime = System.DateTime.MinValue;
        if (dateRange.FromDate.HasValue)
            startTime = dateRange.FromDate.Date;
        if (dateRange.ToDate.HasValue)
            endTime = dateRange.ToDate.Date;
        pageIndex = 1;

        WWWForm form = new WWWForm();
        form.AddField("requestType", "SelectAllHistoryDataByCondition");
        form.AddField("pageIndex", pageIndex);
        form.AddField("pageSize", pageSize);
        form.AddField("probeName", input_probeName.text);
        form.AddField("gasKind", input_gasKind.text);
        form.AddField("startTime", startTime.ToString());
        form.AddField("endTime", endTime.ToString());
        form.AddField("pageCount", pageCount);
        form.AddField("rowCount", rowCount);
        GameUtils.PostHttp("HistoryData.ashx", form, (result) =>
        {
            string pageResult = result.Split('*')[0];
            pageCount = Convert.ToInt32(pageResult.Split(',')[0]);
            rowCount = Convert.ToInt32(pageResult.Split(',')[1]);
            result = result.Split('*')[1];
            historyDataModelList = JsonMapper.ToObject<List<HistoryDataModel>>(result);
            InitGrid();
        }, null);
    }

    private void OnEnable()
    {
        InitData();
    }

    List<ProbeHistoryDataItem> itemList = new List<ProbeHistoryDataItem>();
    private void InitData()
    {
        System.DateTime startTime = System.DateTime.MinValue, endTime = System.DateTime.MinValue;
        if (dateRange.FromDate.HasValue)
            startTime = dateRange.FromDate.Date;
        if (dateRange.ToDate.HasValue)
            endTime = dateRange.ToDate.Date;

        WWWForm form = new WWWForm();
        form.AddField("requestType", "SelectAllHistoryDataByCondition");
        form.AddField("pageIndex", pageIndex);
        form.AddField("pageSize", pageSize);
        form.AddField("probeName", input_probeName.text);
        form.AddField("gasKind", input_gasKind.text);
        form.AddField("startTime", startTime.ToString());
        form.AddField("endTime", endTime.ToString());
        form.AddField("pageCount", pageCount);
        form.AddField("rowCount", rowCount);
        GameUtils.PostHttp("HistoryData.ashx", form, (result) =>
        {
            string pageResult = result.Split('*')[0];
            pageCount = Convert.ToInt32(pageResult.Split(',')[0]);
            rowCount = Convert.ToInt32(pageResult.Split(',')[1]);
            result = result.Split('*')[1];
            historyDataModelList = JsonMapper.ToObject<List<HistoryDataModel>>(result);
            InitGrid();
        }, null);
    }

    void InitGrid()
    {
        itemList.Clear();
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
            itemList.Add(item);
            currentObj.SetActive(true);
        });
    }
}